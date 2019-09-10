using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.VS.Owp;

namespace SobaScript.Z.VSTest.Stubs
{
    internal sealed class NullOwpEnv: IOwpEnv
    {
        public const string MOCK_ITEM_NAME = "NotAvailableName";

        public string DefaultItem => "Build";

        public IEWData EWData
        {
            get;
            private set;
        }

        public bool Write(string content, bool newline, string name, bool createIfNo = false)
        {
            if(newline) {
                content += Environment.NewLine;
            }

            Console.Write(content);
            return true;
        }

        public bool Activate(string name) => true;

        public bool Delete(string name) => true;

        public bool Clear(string name) => true;

        public IEWData GetEWData(string item, bool isGuid)
        {
            if(item == MOCK_ITEM_NAME) {
                throw new NotFoundException(item);
            }
            return EWData;
        }

        public NullOwpEnv()
        {
            EWData = new _EWData(string.Empty, new List<string>(), new List<string>());
        }

        public NullOwpEnv(string raw, List<string> errors, List<string> warns)
        {
            EWData = new _EWData(raw, errors, warns);
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
