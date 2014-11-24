// Guids.cs
// MUST match guids.h
using System;
using Microsoft.VisualStudio;

namespace net.r_eg.vsSBE
{
    public static class GuidList
    {
        public const string PACKAGE_STRING          = "94ecd13f-15f3-4f51-9afd-17f0275c6266";
        public const string MAIN_CMD_STRING         = "5547e550-6d81-4f97-91ed-338926673efa";
        public const string PANEL_STRING            = "71D73518-E66A-4EA7-952A-3AF7A23E4C78";
        public const string PANEL_CMD_STRING        = "B53B115A-F433-44EB-85E0-94767FAA23E0";

        public static readonly Guid MAIN_CMD_SET    = new Guid(MAIN_CMD_STRING);
        public static readonly Guid PANEL_CMD_SET   = new Guid(PANEL_CMD_STRING);
        public static readonly Guid VSStd97CmdID    = VSConstants.CMDSETID.StandardCommandSet97_guid;
        public static readonly Guid VSStd2KCmdID    = VSConstants.CMDSETID.StandardCommandSet2K_guid;
    };
}