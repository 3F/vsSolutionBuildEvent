/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    using TFieldLeft    = Func<string, string, bool, string>;
    using TFieldRight   = Func<string, bool, string>;
    using TFields       = Dictionary<Fields.Type, string>;
    using TFieldsMap    = KeyValuePair<Dictionary<Fields.Type, string>, string>;

    internal class StepFinal: IStep
    {
        protected const int INDENT_SIZE     = 4;
        protected const string LINE_BREAK   = "\r\n";

        /// <summary>
        /// The pattern value of fileds in user script.
        /// </summary>
        protected const string PAT_FIELD_VALUE  = "%{0}%";

        /// <summary>
        /// Manager of used steps for generation.
        /// </summary>
        protected Manager req;

        /// <summary>
        /// Wrapper of TFields for returning the default value if some key is not exists.
        /// </summary>
        protected struct FMap
        {
            public string defaultValue;
            public TFields map;

            public string this[Fields.Type type]
            {
                get {
                    if(map == null || !map.ContainsKey(type)) {
                        return defaultValue;
                    }
                    return map[type];
                }
                set {
                    map[type] = value; //allowing exception for null map
                }
            }

            public FMap(TFields map, string defVal)
            {
                defaultValue    = defVal;
                this.map        = map;
            }
        }

        /// <summary>
        /// The type of step.
        /// </summary>
        public StepsType Type
        {
            get { return StepsType.Final; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Final script.</returns>
        public string construct()
        {
            switch(req.StepGen.gtype)
            {
                case GenType.CSharpStruct: {
                    return genCSharpStruct();
                }
                case GenType.CppStruct: {
                    return genCppStruct();
                }
                case GenType.CppDefinitions: {
                    return genCppDefinitions();
                }
                case GenType.Direct: {
                    return genDirect();
                }
            }
            throw new NotFoundException("The '{0}' is not found to construct the final step.", req.StepGen.gtype);
        }

        /// <param name="req">Manager of used steps for generation.</param>
        public StepFinal(Manager req)
        {
            this.req = req;
        }

        /// <summary>
        /// Constructs fields.
        /// </summary>
        /// <param name="left">The part of left definition of field.</param>
        /// <param name="right">The part of right definition of field</param>
        /// <param name="indent">Initial indent for all fields.</param>
        /// <returns>Map of used fields and definitions for user script.</returns>
        protected TFieldsMap fields(TFieldLeft left, TFieldRight right, string indent)
        {
            TFields fmap = new TFields();
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            int maxlen = 0;
            foreach(var f in req.StepFields.items)
            {
                if(f.disabled) {
                    continue;
                }

                string name = f.newname;
                if(String.IsNullOrWhiteSpace(name))
                {
                    if(req.StepGen.gtype == GenType.CppDefinitions) {
                        name = f.originConst;
                    }
                    else {
                        name = (req.StepStruct.upperCase)? f.originUpperCase : f.origin;
                    }
                }

                if(!req.StepFields.isAllow(f.type, req.StepCfgData.scm) 
                    || !req.StepFields.isAllow(f.type, req.StepCfgData.revType)
                    || !req.StepFields.isAllow(f.type, req.StepGen.gtype))
                {
                    continue;
                }
                fmap[f.type] = String.Format(PAT_FIELD_VALUE, name);

                string fleft    = left(indent, name, f.type == Fields.Type.Number);
                string fright   = right(name, f.type == Fields.Type.Number);

                if(!String.IsNullOrEmpty(fright) && fleft.Length > maxlen) {
                    maxlen = fleft.Length;
                }

                items.Add(new KeyValuePair<string, string>(fleft, fright));
            }

            // formatting for user script

            int spaces          = INDENT_SIZE - (maxlen % INDENT_SIZE);
            StringBuilder sb    = new StringBuilder();

            foreach(var item in items)
            {
                if(String.IsNullOrEmpty(item.Value)) {
                    sb.Append(item.Key + LINE_BREAK);
                    continue;
                }
                sb.Append(String.Format("{0}{1}{2}{3}", item.Key, new String(' ', (maxlen - item.Key.Length) + spaces), item.Value, LINE_BREAK));
            }

            if(sb.Length > LINE_BREAK.Length) {
                sb.Remove(sb.Length - LINE_BREAK.Length, LINE_BREAK.Length); //to remove the last line break
            }
            return new TFieldsMap(fmap, sb.ToString());
        }

        /// <summary>
        /// Constructs of revision calculation.
        /// </summary>
        /// <returns></returns>
        protected string scRevision()
        {
            switch(req.StepCfgData.revType)
            {
                case RevNumber.Type.DeltaTime:
                {
                    string rev  = Resource.ScriptRevisionTimeDelta;
                    var delta   = (RevNumber.DeltaTime)req.StepCfgData.revVal;

                    // sortable format for InvariantCulture
                    rev = rev.Replace("!RevTime!", delta.timeBase.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
                    rev = rev.Replace("!RevType!", delta.interval.ToString());

                    string revMod = String.Empty;
                    if(delta.revMod.enabled) {
                        revMod = Resource.ScriptRevTimeModulo
                                            .Replace("!revMin!", delta.revMod.min.ToString())
                                            .Replace("!revMax!", delta.revMod.max.ToString());

                        revMod = String.Format("{0}{0}{1}", System.Environment.NewLine, revMod);
                    }
                    rev = rev.Replace("!RevModulo!", revMod);

                    return rev + LINE_BREAK;
                }
                case RevNumber.Type.Raw: {
                    return Resource.ScriptRevisionRaw + LINE_BREAK;
                }
            }
            throw new NotFoundException("scRevision: the `{0}` is not supported.", req.StepCfgData.revType);
        }

        /// <summary>
        /// Constructs removing of placeholders from version-fields.
        /// </summary>
        /// <param name="sc">User script where it used.</param>
        /// <param name="fmap">Map of used fields.</param>
        /// <returns></returns>
        protected string scVersion(string sc, FMap fmap)
        {
            if(fmap[Fields.Type.Number] != null) {
                string rev = (req.StepCfgData.revType == RevNumber.Type.Raw)? String.Empty : ", $(Revision)";
                string raw = "#[var tpl = $(tpl.Replace(\"{0}\", \"$(ver.Replace('.', ', ')){1}\"))]{2}";

                sc = sc.Replace("!ReplaceVersion!", String.Format(raw, fmap[Fields.Type.Number], rev, LINE_BREAK));
            }
            else {
                sc = sc.Replace("!ReplaceVersion!", String.Empty);
            }

            string ver = String.Empty;

            if(fmap[Fields.Type.NumberWithRevString] != null) {
                ver += String.Format(".Replace(\"{0}\", \"$(ver).$(Revision)\")", fmap[Fields.Type.NumberWithRevString]);
            }

            if(fmap[Fields.Type.NumberString] != null) {
                ver += String.Format(".Replace(\"{0}\", \"$(ver)\")", fmap[Fields.Type.NumberString]);
            }

            if(ver.Length > 0) {
                return sc.Replace("!ReplaceVerString!", String.Format("#[var tpl = $(tpl{0})]", ver));
            }
            return sc.Replace("!ReplaceVerString!", String.Empty);
        }

        /// <summary>
        /// Constructs variables.
        /// </summary>
        /// <param name="sc">User script where it used.</param>
        /// <returns></returns>
        protected string scVariables(string sc)
        {
            sc = sc.Replace("!Fout!", req.StepCfgData.output);

            if(req.StepCfgData.inputNumberType == StepCfgData.InputNumberType.File)
            {
                return sc.Replace("!InputNum!", 
                                    String.Format(
                                            "#[var input = {0}]{1}#[var ver   = #[File get(\"#[var input]\")]]",
                                            req.StepCfgData.inputNumber,
                                            LINE_BREAK));
            }
            return sc.Replace("!InputNum!", String.Format("#[var ver   = {0}]", req.StepCfgData.inputNumber));
        }

        /// <summary>
        /// Constructs SCM calculation.
        /// </summary>
        /// <param name="fmap">Map of used fields.</param>
        /// <param name="fields">List of fields from user script where it used.</param>
        /// <param name="empty">Empty value for user script.</param>
        /// <returns></returns>
        protected string scScm(FMap fmap, ref string fields, string empty = "-")
        {
            if(req.StepCfgData.scm != StepCfgData.SCMType.None)
            {
                // to modify the 'Informational' fields:

                string inf = String.Format("{0} [ {1} ]", 
                                            fmap[Fields.Type.NumberWithRevString]?? fmap[Fields.Type.NumberString]?? empty, 
                                            fmap[Fields.Type.BranchSha1]?? empty);

                if(fmap[Fields.Type.Informational] != null) {
                    fields = fields.Replace(fmap[Fields.Type.Informational], inf);
                }

                if(fmap[Fields.Type.InformationalFull] != null) {
                    fields = fields.Replace(fmap[Fields.Type.InformationalFull], 
                                                    String.Format("{0} /'{1}':{2}", 
                                                                    inf, 
                                                                    fmap[Fields.Type.BranchName]?? empty, 
                                                                    fmap[Fields.Type.BranchRevCount]?? empty));
                }
            }

            string scm = String.Empty;

            if(req.StepCfgData.scm == StepCfgData.SCMType.Git)
            {
                string cData  = String.Empty;
                string cEmpty = String.Empty;

                if(fmap[Fields.Type.BranchName] != null) {
                    cData   += String.Format(".Replace(\"{0}\", \"#[var bName]\")", fmap[Fields.Type.BranchName]);
                    cEmpty  += String.Format(".Replace(\"{0}\", \"{1}\")", fmap[Fields.Type.BranchName], empty);
                }
                if(fmap[Fields.Type.BranchSha1] != null) {
                    cData   += String.Format(".Replace(\"{0}\", \"#[var bSha1]\")", fmap[Fields.Type.BranchSha1]);
                    cEmpty  += String.Format(".Replace(\"{0}\", \"{1}\")", fmap[Fields.Type.BranchSha1], empty);
                }
                if(fmap[Fields.Type.BranchRevCount] != null) {
                    cData   += String.Format(".Replace(\"{0}\", \"#[var bRevCount]\")", fmap[Fields.Type.BranchRevCount]);
                    cEmpty  += String.Format(".Replace(\"{0}\", \"{1}\")", fmap[Fields.Type.BranchRevCount], empty);
                }

                scm = Resource.ScriptScmGit;
                if(cData.Length > 0) {
                    scm = scm.Replace("!RScmData!", String.Format("{0}    #[var tpl = $(tpl{1})]", LINE_BREAK, cData));
                    scm = scm.Replace("!RScmEmpty!", String.Format("#[var tpl = $(tpl{0})]", cEmpty));
                }
                else {
                    scm = scm.Replace("!RScmData!", String.Empty);
                    scm = scm.Replace("!RScmEmpty!", String.Empty);
                }

                return scm + LINE_BREAK;
            }

            return scm;
        }

        /// <summary>
        /// Constructs SCM calculation for direct using.
        /// </summary>
        /// <param name="type">Field type.</param>
        /// <param name="fieldName">Name of the defined field.</param>
        /// <returns></returns>
        protected string scScmDirect(Fields.Type type, out string fieldName)
        {
            if(req.StepCfgData.scm == StepCfgData.SCMType.Git)
            {
                string tpl      = "#[var {0} = #[IO sout(\"git\", \"{1}\")]]";
                string tplEmpty = "#[var {0} = ]";
                switch(type) {
                    case Fields.Type.BranchName:
                    {
                        fieldName   = "bName";
                        string fld  = String.Format(tpl, fieldName, "rev-parse --abbrev-ref HEAD");
                        string no   = String.Format(tplEmpty, fieldName);
                        return Resource.ScriptScmGitBox.Replace("!Var!", fld).Replace("!Else!", no);
                    }
                    case Fields.Type.BranchRevCount:
                    {
                        fieldName   = "bRevCount";
                        string fld  = String.Format(tpl, fieldName, "rev-list HEAD --count");
                        string no   = String.Format(tplEmpty, fieldName);
                        return Resource.ScriptScmGitBox.Replace("!Var!", fld).Replace("!Else!", no);
                    }
                    case Fields.Type.BranchSha1:
                    {
                        fieldName   = "bSha1";
                        string fld  = String.Format(tpl, fieldName, "rev-parse --short HEAD");
                        string no   = String.Format(tplEmpty, fieldName);
                        return Resource.ScriptScmGitBox.Replace("!Var!", fld).Replace("!Else!", no);
                    }
                    case Fields.Type.Informational:
                    {
                        string bSha1;
                        fieldName = "info";

                        string scm = scScmDirect(Fields.Type.BranchSha1, out bSha1);
                        string val = String.Format("#[var {0} = $(ver).$(Revision) [ $({1}) ]]", fieldName, bSha1);

                        return String.Format("{0}{1}{2}", scm, LINE_BREAK, val);
                    }
                    case Fields.Type.InformationalFull:
                    {
                        fieldName = "infoFull";

                        string scm = Resource.ScriptScmGit
                                                .Replace("!RScmData!", String.Empty)
                                                .Replace("!RScmEmpty!", "#[var bSha1 = ]#[var bName = ]#[var bRevCount = ]");

                        string val = String.Format("#[var {0} = $(ver).$(Revision) [ $(bSha1) ] /'#[var bName]':$(bRevCount)]", fieldName);

                        return String.Format("{0}{1}{2}", scm, LINE_BREAK, val);
                    }
                }
                throw new NotFoundException("The `{0}` is not found for used scm `git`.", type);
            }
            throw new NotFoundException("The `{0}` is not found for handling scm data.", req.StepCfgData.scm);
        }

        /// <summary>
        /// Constructs direct variable from version-fields.
        /// </summary>
        /// <param name="type">Type of field.</param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected string scVersionDirect(Fields.Type type, out string fieldName)
        {
            string tpl = "#[var {0} = {1}]";
            switch(type) {
                case Fields.Type.Null: {
                    fieldName = String.Empty;
                    return String.Empty;
                }
                case Fields.Type.NumberString: {
                    fieldName = "numString";
                    return String.Format(tpl, fieldName, "$(ver)");
                }
                case Fields.Type.NumberWithRevString: {
                    fieldName = "numRevString";
                    return String.Format(tpl, fieldName, "$(ver).$(Revision)");
                }
            }
            throw new NotFoundException("The `{0}` is not found for single variable from version-fields.", type);
        }

        /// <summary>
        /// Generate script for `C# struct` variant.
        /// </summary>
        /// <returns></returns>
        protected string genCSharpStruct()
        {
            Func<string, string, bool, string> left = delegate(string _indent, string _name, bool isNum) {
                if(isNum) {
                    return String.Format("{0}public static readonly System.Version {1}", _indent, _name);
                }
                return String.Format("{0}public const string {1}", _indent, _name);
            };

            Func<string, bool, string> right = delegate(string _name, bool isNum) {
                if(isNum) {
                    return String.Format(String.Format("= new System.Version({0});", PAT_FIELD_VALUE), _name);
                }
                return String.Format(String.Format("= \"{0}\";", PAT_FIELD_VALUE), _name);
            };

            TFieldsMap fld  = fields(left, right, new String(' ', INDENT_SIZE * 2));
            string fname    = fld.Value;
            FMap fmap       = new FMap() { map = fld.Key };

            string sc = scVariables(Resource.ScriptMain);

            sc = sc.Replace("!Revision!", scRevision());
            sc = scVersion(sc, fmap);
            sc = sc.Replace("!SCM!", scScm(fmap, ref fname));

            string tpl = Resource.CSharpStructTpl
                                 .Replace("!Namespace!", req.StepStruct.namspace)
                                 .Replace("!StructName!", req.StepStruct.name);

            tpl = String.Format("{0}{1}{2}", Resource.Header, LINE_BREAK, tpl);

            return sc.Replace("!Template!", tpl.Replace("!Items!", fname));
        }

        /// <summary>
        /// Generate script for `C++ struct` variant.
        /// </summary>
        /// <returns></returns>
        protected string genCppStruct()
        {
            Func<string, string, bool, string> leftRef = delegate(string _indent, string _name, bool isNum) {
                if(isNum) {
                    return String.Format("{0}static System::Version^ const {1}", _indent, _name);
                }
                return String.Format("{0}static System::String^ const {1}", _indent, _name);
            };

            Func<string, bool, string> rightRef = delegate(string _name, bool isNum) {
                if(isNum) {
                    return String.Format(String.Format("= gcnew System::Version({0});", PAT_FIELD_VALUE), _name);
                }
                return String.Format(String.Format("= \"{0}\";", PAT_FIELD_VALUE), _name);
            };

            Func<string, string, bool, string> left = delegate(string _indent, string _name, bool isNum) {
                if(isNum) {
                    return String.Format("{0}{1}{2}", _indent, Resource.CppStructNumTpl, LINE_BREAK)
                                 .Replace("!FieldName!", _name)
                                 .Replace("!VerNum!", String.Format(PAT_FIELD_VALUE, _name));
                }
                return String.Format("{0}const std::wstring {1}", _indent, _name);
            };

            Func<string, bool, string> right = delegate(string _name, bool isNum) {
                if(isNum) {
                    return String.Empty;
                }
                return String.Format(String.Format("= L\"{0}\";", PAT_FIELD_VALUE), _name);
            };

            bool clr        = (req.StepStruct.fnumber == StepStruct.NumberType.SystemVersion);
            TFieldsMap fld  = fields(
                                (clr)? leftRef : left, 
                                (clr)? rightRef : right, 
                                new String(' ', INDENT_SIZE * 2));

            string fname    = fld.Value;
            FMap fmap       = new FMap() { map = fld.Key };

            string sc = scVariables(Resource.ScriptMain);

            sc = sc.Replace("!Revision!", scRevision());
            sc = scVersion(sc, fmap);
            sc = sc.Replace("!SCM!", scScm(fmap, ref fname));

            string tpl = Resource.CppStructTpl
                                 .Replace("!Namespace!", req.StepStruct.namspace)
                                 .Replace("!StructName!", req.StepStruct.name);

            tpl = String.Format("{0}{1}{2}", Resource.Header, LINE_BREAK, tpl);

            if(clr) {
                tpl = tpl.Replace("!KWRef!", "ref ")
                         .Replace("!DefVariable!", String.Empty)
                         .Replace("!IncString!", String.Empty);
            }
            else {
                tpl = tpl.Replace("!KWRef!", String.Empty)
                         .Replace("!DefVariable!", " " + req.StepStruct.name)
                         .Replace("!IncString!", String.Format("{1}{0}{1}", "#include <string>", LINE_BREAK));
            }

            return sc.Replace("!Template!", tpl.Replace("!Items!", fname));
        }

        /// <summary>
        /// Generate script for `C++ macro definitions` variant.
        /// </summary>
        /// <returns></returns>
        protected string genCppDefinitions()
        {
            Func<string, string, bool, string> left = delegate(string _indent, string _name, bool isNum) {
                if(isNum) {
                    return String.Empty;
                }
                return String.Format("{0}#define {1}", _indent, _name);
            };

            Func<string, bool, string> right = delegate(string _name, bool isNum) {
                if(isNum) {
                    return String.Empty;
                }
                return String.Format(String.Format("L\"{0}\";", PAT_FIELD_VALUE), _name);
            };

            TFieldsMap fld  = fields(left, right, String.Empty);
            string fname    = fld.Value;
            FMap fmap       = new FMap() { map = fld.Key };

            string sc = scVariables(Resource.ScriptMain);

            sc = sc.Replace("!Revision!", scRevision());
            sc = scVersion(sc, fmap);
            sc = sc.Replace("!SCM!", scScm(fmap, ref fname));

            string tpl  = Resource.CppDefineTpl;
            tpl         = String.Format("{0}{1}{2}", Resource.Header, LINE_BREAK, tpl);

            return sc.Replace("!Template!", tpl.Replace("!Items!", fname));
        }

        /// <summary>
        /// Generate script for `Direct replacement` variant.
        /// </summary>
        /// <returns></returns>
        protected string genDirect()
        {
            string sc   = scVariables(Resource.ScriptDirectRepl);
            sc          = sc.Replace("!Revision!", scRevision());

            string source;
            if(req.StepRepl.IsSourceSCM) {
                sc = sc.Replace("!BasicData!", scScmDirect(req.StepRepl.source, out source) + LINE_BREAK);
            }
            else {
                var fld = String.Format("{0}{1}", 
                                        scVersionDirect(req.StepRepl.source, out source), 
                                        (req.StepRepl.source == Fields.Type.Null)? String.Empty : LINE_BREAK);

                sc = sc.Replace("!BasicData!", fld);
            }

            sc = sc.Replace("!RType!", (req.StepRepl.rtype == StepRepl.ReplType.Regex)? "Regexp" : "Wildcards");
            sc = sc.Replace("!Pattern!", escapeAsString(req.StepRepl.pattern));

            string replacement;
            if(String.IsNullOrEmpty(source)) {
                replacement = req.StepRepl.prefix + req.StepRepl.postfix;
            }
            else {
                replacement = String.Format("{0}#[var {1}]{2}", req.StepRepl.prefix, source, req.StepRepl.postfix);
            }

            return sc.Replace("!Replacement!", escapeAsString(replacement));
        }

        /// <summary>
        /// Escapes symbols in data that used as string argument.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string escapeAsString(string data)
        {
            return data.Replace("\"", "\\\"");
        }
    }
}