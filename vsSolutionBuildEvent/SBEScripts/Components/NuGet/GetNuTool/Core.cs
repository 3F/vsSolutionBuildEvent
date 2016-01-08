/*
 * Copyright (c) 2015-2016  Denis Kuzmin (reg) [ entry.reg@gmail.com ]
 *
 * Distributed under the MIT license
 * (see accompanying file LICENSE or a copy at http://opensource.org/licenses/MIT)
 * 
 * Source code from - https://github.com/3F/GetNuTool
 *                    v1.3
 * Modifications:
 * - Base path
 * - Logger
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace net.r_eg.vsSBE.SBEScripts.Components.NuGet.GetNuTool
{
    public abstract class Core
    {
        /// <summary>
        /// Where to look the packages.config files.
        /// </summary>
        protected string ngconfig = ".nuget\\packages.config";

        /// <summary>
        /// NuGet server.
        /// </summary>
        protected string ngserver = "https://www.nuget.org/api/v2/package/";

        /// <summary>
        /// Common path for all packages.
        /// </summary>
        protected string ngpath = "packages";

        /// <summary>
        /// Get path to item with location for current context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract string location(string item);

        /// <param name="config"></param>
        /// <param name="plist"></param>
        /// <returns></returns>
        public string prepareList(string config, string plist)
        {
            if(!String.IsNullOrEmpty(plist)) {
                return plist;
            }

            Action<string, Queue<string>> h = delegate(string cfg, Queue<string> list)
            {
                foreach(var pkg in XDocument.Load(cfg).Descendants("package"))
                {
                    var id      = pkg.Attribute("id");
                    var version = pkg.Attribute("version");
                    var output  = pkg.Attribute("output");

                    if(id == null) {
                        throw new ArgumentException(String.Format("Attribute - 'id' is not found in '{0}'", cfg));
                    }
                    var link = id.Value;

                    if(version != null) {
                        link += "/" + version.Value;
                    }

                    if(output != null) {
                        list.Enqueue(link + ":" + output.Value);
                        continue;
                    }
                    list.Enqueue(link);
                }
            };

            var ret = new Queue<string>();
            foreach(var cfg in config.Split('|')) {
                if(File.Exists(cfg)) {
                    h(cfg, ret);
                }
            }

            if(ret.Count < 1) {
                throw new FileNotFoundException("List of packages is empty. Use packages.config or property like /p:ngpackages=\"...\"");
            }
            return String.Join(";", ret.ToArray());
        }

        /// <param name="plist"></param>
        /// <param name="url"></param>
        /// <param name="defpath"></param>
        /// <param name="debug"></param>
        public void downloader(string plist, string url, string defpath, bool debug)
        {
            // to ignore from package
            var ignore = new string[] { "/_rels/", "/package/", "/[Content_Types].xml" };

            Action<string, object> dbg = delegate(string s, object p) {
                if(debug) {
                    Log.Debug(s, p);
                }
            };

            Action<string, string, string> get = delegate(string link, string name, string path)
            {
                string output = Path.GetFullPath(location(path ?? name));
                if(Directory.Exists(output)) {
                    Log.Debug("The `{0}` is already exists. /pass -> `{1}`", name, output);
                    return;
                }
                Log.Debug("Getting `{0}` ... ", link);

                string temp = Path.Combine(Path.GetTempPath(), name);
                using(WebClient wc = new WebClient()) {
                    wc.DownloadFile(url + link, temp);
                }

                Log.Debug("Extracting into `{0}`", output);
                using(Package package = ZipPackage.Open(temp, FileMode.Open, FileAccess.Read))
                {
                    foreach(PackagePart part in package.GetParts()) 
                    {
                        var uri = Uri.UnescapeDataString(part.Uri.OriginalString);
                        if(ignore.Any(x => uri.StartsWith(x, StringComparison.Ordinal))) {
                            continue;
                        }

                        var dest = Path.Combine(output, uri.TrimStart('/'));
                        dbg("-> `{0}`", uri);

                        var dir = Path.GetDirectoryName(dest);
                        if(!Directory.Exists(dir)) {
                            Directory.CreateDirectory(dir);
                        }

                        using(Stream source = part.GetStream(FileMode.Open, FileAccess.Read))
                        using(FileStream target = File.OpenWrite(dest)) {
                            source.CopyTo(target);
                        } 
                    } 
                }
                dbg("Done.{0}", System.Environment.NewLine);
            };

            //Format: id/version[:path];id2/version[:path];...

            foreach(var package in plist.Split(';'))
            {
                var ident   = package.Split(':');
                var link    = ident[0];
                var path    = (ident.Length > 1) ? ident[1] : null;
                var name    = link.Replace('/', '.');

                if(!String.IsNullOrEmpty(defpath)) {
                    path = Path.Combine(defpath, path ?? name);
                }
                get(link, name, path);
            }
        }

        /// <param name="dir"></param>
        /// <param name="dout"></param>
        /// <param name="debug"></param>
        public void packing(string dir, string dout, bool debug)
        {
            const string EXT_NUSPEC         = ".nuspec";
            const string EXT_NUPKG          = ".nupkg";
            const string TAG_META           = "metadata";
            const string DEF_CONTENT_TYPE   = "application/octet"; //System.Net.Mime.MediaTypeNames.Application.Octet
            const string MANIFEST_URL       = "http://schemas.microsoft.com/packaging/2010/07/manifest";

            // Tags
            const string ID     = "id";
            const string VER    = "version";
                
            Action<string, object> dbg = delegate(string s, object p) {
                if(debug) {
                    Log.Debug(s, p);
                }
            };

            // Get metadata

            var nuspec = Directory.GetFiles(dir, "*" + EXT_NUSPEC, SearchOption.TopDirectoryOnly).FirstOrDefault();
            if(nuspec == null) {
                throw new FileNotFoundException(String.Format("The {0} file is not found in `{1}`", EXT_NUSPEC, dir));
            }
            Log.Debug("Found {0}: `{1}`", EXT_NUSPEC, nuspec);

            var root = XDocument.Load(nuspec).Root.Elements().FirstOrDefault(x => x.Name.LocalName == TAG_META);
            if(root == null) {
                throw new FileNotFoundException(String.Format("The `{0}` not contains {1}.", nuspec, TAG_META));
            }

            var metadata = new Dictionary<string, string>();
            foreach(var tag in root.Elements()) {
                metadata[tag.Name.LocalName.ToLower()] = tag.Value;
            }

            // Validate data - rules of nuget core

            if(metadata[ID].Length > 100 || !Regex.IsMatch(metadata[ID], 
                                                            @"^\w+([_.-]\w+)*$", 
                                                            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                throw new FormatException(String.Format("The data format of `{0}` is not correct.", ID));
            }
            new System.Version(metadata[VER]); // check with System.Version

            // Format package

            var ignore = new string[] { // to ignore from package
                                Path.Combine(dir, "_rels"),
                                Path.Combine(dir, "package"),
                                Path.Combine(dir, "[Content_Types].xml") };

            string pout = String.Format("{0}.{1}{2}", metadata[ID], metadata[VER], EXT_NUPKG);
            if(!String.IsNullOrWhiteSpace(dout)) {
                if(!Directory.Exists(dout)) {
                    Directory.CreateDirectory(dout);
                }
                pout = Path.Combine(dout, pout);
            }

            Log.Debug("Started packing `{0}` ...", pout);
            using(Package package = Package.Open(pout, FileMode.Create))
            {
                // manifest relationship

                Uri manifestUri = new Uri(String.Format("/{0}{1}", metadata[ID], EXT_NUSPEC), UriKind.Relative);
                package.CreateRelationship(manifestUri, TargetMode.Internal, MANIFEST_URL);

                // content

                foreach(var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    if(ignore.Any(x => file.StartsWith(x, StringComparison.Ordinal))) {
                        continue;
                    }

                    string pUri;
                    if(file.StartsWith(dir, StringComparison.OrdinalIgnoreCase)) {
                        pUri = file.Substring(dir.Length).TrimStart(Path.DirectorySeparatorChar);
                    }
                    else {
                        pUri = file;
                    }
                    dbg("-> `{0}`", pUri);

                    // to protect path without separators
                    var escaped = String.Join("/", pUri.Split('\\', '/').Select(p => Uri.EscapeDataString(p)));
                    Uri uri     = PackUriHelper.CreatePartUri(new Uri(escaped, UriKind.Relative));

                    PackagePart part = package.CreatePart(uri, DEF_CONTENT_TYPE, CompressionOption.Maximum);

                    using (Stream tstream = part.GetStream())
                    using(FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                        fileStream.CopyTo(tstream);
                    }
                }

                // metadata for package

                Func<string, string> getmeta = delegate(string key) {
                    return (metadata.ContainsKey(key))? metadata[key] : "";
                };

                package.PackageProperties.Creator           = getmeta("authors");
                package.PackageProperties.Description       = getmeta("description");
                package.PackageProperties.Identifier        = metadata[ID];
                package.PackageProperties.Version           = metadata[VER];
                package.PackageProperties.Keywords          = getmeta("tags");
                package.PackageProperties.Title             = getmeta("title");
                package.PackageProperties.LastModifiedBy    = String.Format("{0} v{1} - GetNuTool Core", Settings.APP_NAME, Version.numberWithRevString);
            }
        }
    }
}
