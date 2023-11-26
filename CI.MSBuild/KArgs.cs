/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    using ArgsData = Dictionary<KArgType, IDictionary<string, string>>;

    /// <summary>
    /// Extract arguments from various compatible formats as `key-value` strings.
    ///     multi-keys `/p:prop1=val1 /p:prop2=val2` same as `/p:prop1=val1;prop2=val2`
    ///     "D:\projects\App1.sln" /l:"CI.MSBuild.dll";lib=D:\bin\vsSolutionBuildEvent\;cultureUI=en-US /v:d /t:Rebuild /p:Configuration=Debug;Platform="Any CPU" /m:8
    ///     nowarn=1591;1701;1702 and nowarn=1591,1701,1702 in addition to Configuration=Debug;Platform="Any CPU"
    /// etc. http://msdn.microsoft.com/en-us/library/ms164311.aspx
    /// </summary>
    internal class KArgs
    {
        protected ArgsData Data { get; }

        public IDictionary<string, string> this[KArgType key]
            => Data.ContainsKey(key) ? Data[key] : null;

        public IEnumerable<string> GetKeys(KArgType type, bool? val = null)
        {
            if(val == null)
            {
                return this[type]?.Select(a => a.Key);
            }

            return this[type]?.Where(a => 
                (val == true && a.Value != null) || (val == false && a.Value == null)
            )
            .Select(a => a.Key);
        }

        public bool Exists(KArgType type) => GetCount(type) > 0;

        public int GetCount(KArgType type) => (this[type]?.Count).GetValueOrDefault();

        internal KArgs(string[] raw)
        {
            if(raw == null) throw new ArgumentNullException(nameof(raw));

            Data = InitData();

            foreach(string arg in raw)
            {
                if(AddMSBuildArgTo(KArgType.Targets, arg, "target:", "t:")
                    || AddMSBuildArgTo(KArgType.Properties, arg, "property:", "p:")
                    || AddMSBuildArgTo(KArgType.Loggers, arg, "logger:", "l:"))
                {
                    continue;
                }

                if(!string.IsNullOrWhiteSpace(arg))
                {
                    Data[KArgType.Common][arg] = null;
                }
            }
        }

        internal KArgs()
            : this(Environment.GetCommandLineArgs())
        {

        }

        private ArgsData InitData()
        {
            var data = new ArgsData();
            foreach(var type in ((KArgType[])Enum.GetValues(typeof(KArgType))).Distinct())
            {
                data[type] = new Dictionary<string, string>();
            }
            return data;
        }

        private bool AddMSBuildArgTo(KArgType type, string arg, string key, string kshort = null)
        {
            if(AddArgTo(type, arg, '/' + key, kshort == null ? null : '/' + kshort))
            {
                return true;
            }
            return AddArgTo(type, arg, '-' + key, kshort == null ? null : '-' + kshort);
        }

        private bool AddArgTo(KArgType type, string arg, string key, string kshort = null)
        {
            var val = ExtractKey(arg.TrimStart(), key, kshort);
            if(val == null)
            {
                return false;
            }

            foreach(var v in SplitIfRight(val, ';'))
            {
                var kv = v.Split('=');

                if(!string.IsNullOrWhiteSpace(kv[0]))
                {
                    Data[type][kv[0]] = kv.Length > 1 ? kv[1] : null;
                }
            }            
            return true;
        }

        private string ExtractKey(string raw, string key, string kshort = null)
        {
            if(raw.StartsWith(key, StringComparison.OrdinalIgnoreCase)
                || (kshort != null && raw.StartsWith(kshort, StringComparison.OrdinalIgnoreCase)))
            {
                int vpos = raw.IndexOf(':', 2);
                if(vpos == -1)
                {
                    return string.Empty;
                }

                return raw.Substring(vpos + 1);
            }

            return null;
        }

        /// <summary>
        /// Since MSBuild support both format:
        /// nowarn=1591;1701;1702 and nowarn=1591,1701,1702
        /// in addition to Configuration=Debug;Platform="Any CPU"
        /// <see cref="SplitIfRight"/> helps to split key1=val;key2=val without affecting key=val1;val2;val3
        /// </summary>
        private IEnumerable<string> SplitIfRight(string str, char split, char right = '=')
        {
            int left = -1;
            for(int i = 0; i < str.Length; ++i)
            {
                if(str[i] == split && str.IndexOf(right, i + 1) /*msbuild*/ != -1)
                {
                    yield return str.Substring(++left, i - left);
                    left = i;
                }
            }
            yield return str.Substring(left + 1);
        }
    }
}
