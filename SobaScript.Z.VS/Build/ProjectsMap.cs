/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.VS contributors: https://github.com/3F/Varhead/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.SobaScript.Z.VS.Build
{
    /// <summary>
    /// Detects the first / last project for build-order inside Visual Studio.
    /// Based on https://gist.github.com/3F/a77129e3978841241927
    /// And represents final box-solution from Sample 1 - http://vssbe.r-eg.net/doc/Examples/Demo/#sample-1
    /// 
    /// TODO: !now we have MvsSln project https://github.com/3F/MvsSln
    /// </summary>
    internal class ProjectsMap
    {
        /// <summary>
        /// Guid of Solution Folder.
        /// </summary>
        public const string GUID_SLN_FOLDER = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        /// <summary>
        /// Map of projects in direct order.
        /// </summary>
        protected List<string> order = new List<string>();

        /// <summary>
        /// Map of projects by Guid.
        /// </summary>
        private protected IDictionary<string, ProjectItem> projects = new Dictionary<string, ProjectItem>();

        /// <summary>
        /// Pattern of 'Project(' line that was based on crackProjectLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProject = new Regex("^Project\\(\"(?<TypeGuid>.*)\"\\)\\s*=\\s*\"(?<Name>.*)\"\\s*,\\s*\"(?<Path>.*)\"\\s*,\\s*\"(?<Guid>.*)\"$");

        /// <summary>
        /// Pattern of 'ProjectSection(ProjectDependencies)' lines that was based on crackPropertyLine from Microsoft.Build.BuildEngine.Shared.SolutionParser
        /// </summary>
        protected Regex rProperty = new Regex("^(?<PName>[^=]*)\\s*=\\s*(?<PValue>[^=]*)$");

        /// <summary>
        /// Get list of project Guids.
        /// In direct order of definition.
        /// </summary>
        public List<string> GuidList => order;

        /// <summary>
        /// Get first project from defined list.
        /// Ignores used Build type.
        /// </summary>
        internal ProjectItem First
        {
            get
            {
                if(order.Count < 1) {
                    return new ProjectItem() { name = "The First project is Undefined", path = "?" };
                }
                return projects[order[0]];
            }
        }

        /// <summary>
        /// Get last project from defined list.
        /// Ignores used Build type.
        /// </summary>
        internal ProjectItem Last
        {
            get
            {
                if(order.Count < 1) {
                    return new ProjectItem() { name = "The Last project is Undefined", path = "?" };
                }
                return projects[order[order.Count - 1]];
            }
        }

        /// <summary>
        /// Get first project in Project Build Order.
        /// </summary>
        /// <param name="isCleanOperation"></param>
        /// <returns></returns>
        public ProjectItem FirstBy(bool isCleanOperation)
            => isCleanOperation ? Last // reverse order when 'Clean;CleanCtx;CleanOnlyProject;CleanSelection' types
                                : First;

        /// <summary>
        /// Get last project in Project Build Order.
        /// </summary>
        /// <param name="isCleanOperation"></param>
        /// <returns></returns>
        public ProjectItem LastBy(bool isCleanOperation)
            => isCleanOperation ? First // reverse order when 'Clean;CleanCtx;CleanOnlyProject;CleanSelection' types
                                : Last;

        /// <summary>
        /// Get project by Guid string.
        /// </summary>
        /// <param name="guid">Identifier of project.</param>
        /// <returns></returns>
        public ProjectItem GetProjectBy(string guid) => projects[guid];

        /// <summary>
        /// Detect projects from solution file.
        /// </summary>
        /// <param name="sln">Full path to solution</param>
        /// <param name="flush">Resets prev. data if true.</param>
        public void Detect(string sln, bool flush = false)
        {
            if(flush) {
                projects.Clear();
                order.Clear();
            }

            var map = new Dictionary<string, List<string>>();
            using(StreamReader reader = new StreamReader(sln, Encoding.Default))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    Extract(reader, line.Trim(), ref map);
                }
            }

            // Build order

            bool _h(string id)
            {
                map[id].ForEach(dep => _h(dep));

                if(!order.Contains(id)) {
                    order.Add(id);
                }
                return true;
            }

            foreach(KeyValuePair<string, List<string>> project in map)
            {
                _h(project.Key);

                if(!order.Contains(project.Key)) {
                    order.Add(project.Key);
                }
            }
        }

        /// <param name="sln">Full path to solution</param>
        public ProjectsMap(string sln) => Detect(sln);

        /// <summary>
        /// Only to initialize analyzer.
        /// </summary>
        public ProjectsMap()
        {

        }

        /// <param name="reader">Used reader.</param>
        /// <param name="line">Current line.</param>
        /// <param name="map">Container of projects.</param>
        protected void Extract(StreamReader reader, string line, ref Dictionary<string, List<string>> map)
        {
            if(!line.StartsWith("Project(", StringComparison.Ordinal)) {
                return;
            }

            Match m = rProject.Match(line);
            if(!m.Success) {
                throw new Exception("incorrect line");
            }

            if(string.Equals(GUID_SLN_FOLDER, m.Groups["TypeGuid"].Value.Trim(), StringComparison.OrdinalIgnoreCase)) {
                return; //SolutionFolder
            }

            string pGuid    = m.Groups["Guid"].Value.Trim();
            map[pGuid]      = new List<string>();

            SetProjectRecord
            (
                pGuid, 
                m.Groups["Name"].Value,
                m.Groups["Path"].Value, 
                m.Groups["TypeGuid"].Value
            );

            while((line = reader.ReadLine()) != null && (line != "EndProject"))
            {
                line = line.Trim();
                if(!line.StartsWith("ProjectSection(ProjectDependencies)", StringComparison.Ordinal)) {
                    continue;
                }

                for(line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    line = line.Trim();
                    if(line.StartsWith("EndProjectSection", StringComparison.Ordinal)) {
                        break;
                    }

                    map[pGuid].Add(rProperty.Match(line).Groups["PName"].Value.Trim());
                }
            }
        }

        protected void SetProjectRecord(string pGuid, string name, string path, string type)
        {
            projects[pGuid] = new ProjectItem()
            { 
                name = name,
                path = path,
                type = type,
                guid = pGuid
            };
        }
    }
}