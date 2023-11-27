/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EnvDTE;
using net.r_eg.SobaScript.Z.VS.Owp;
using net.r_eg.vsSBE.Receiver.Output;
using OWPIdent = net.r_eg.vsSBE.Receiver.Output.Ident;
using OWPItems = net.r_eg.vsSBE.Receiver.Output.Items;

namespace net.r_eg.vsSBE
{
    internal sealed class OwpEnv: IOwpEnv
    {
        private IEnvironment env;

        public string DefaultItem => Settings.OWP_ITEM_BUILD;

        public IEWData EWData
        {
            get;
            private set;
        }

        private IOW Owp
        {
            get
            {
                if(env.OutputWindowPane == null) {
                    throw new NotSupportedException("Owp is not available for current environment.");
                }
                return env.OutputWindowPane;
            }
        }

        public bool Write(string content, bool newline, string name, bool createIfNo = false)
        {
            var pane = GetPane(name, createIfNo);
            if(pane == null) {
                return false;
            }

            if(newline) {
                content += System.Environment.NewLine;
            }

            pane.OutputString(content);
            return true;
        }

        public bool Activate(string name)
        {
            var pane = GetPane(name);
            if(pane == null) {
                return false;
            }

            pane.Activate();
            return true;
        }

        public bool Delete(string name)
        {
            if(GetPane(name) == null) {
                return false;
            }

            Owp.deleteByName(name);
            return true;
        }

        public bool Clear(string name)
        {
            var pane = GetPane(name);
            if(pane == null) {
                return false;
            }

            pane.Clear();
            return true;
        }

        public IEWData GetEWData(string item, bool isGuid)
        {
            IItemEW ew = OWPItems._.getEW
            (
                isGuid ? new OWPIdent() { guid = item } 
                       : new OWPIdent() { item = item }
            );

            return new _EWData(ew.Raw, ew.Errors, ew.Warnings);
        }

        public OwpEnv(IEnvironment env)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        private OutputWindowPane GetPane(string name, bool createIfNo = false)
        {
            if(name == null) {
                return null;
            }

            if(createIfNo) {
                return Owp.getByName(name, true);
            }

            try {
                return Owp.getByName(name, false);
            }
            catch(ArgumentException ex)
            {
                Log.Debug
                (
                    $"'{name}' pane is not available. Use 'force' flag for automatic creation if needed. /{ex.Message}"
                );
            }

            return null;
        }

        private sealed class _EWData: IEWData
        {
            public string Raw { get; private set; }

            public ReadOnlyCollection<string> Errors { get; private set; }

            public ReadOnlyCollection<string> Warnings { get; private set; }

            public _EWData(string raw, List<string> errors, List<string> warns)
            {
                Raw         = raw;
                Errors      = errors.AsReadOnly();
                Warnings    = warns.AsReadOnly();
            }
        }
    }
}
