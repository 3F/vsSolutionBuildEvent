using System;
using Microsoft.VisualStudio;

namespace net.r_eg.vsSBE
{
    public static class GuidList
    {
        /// <summary>
        /// General package identifier
        /// </summary>
        public const string PACKAGE_STRING      = "94ecd13f-15f3-4f51-9afd-17f0275c6266";

        /// <summary>
        /// Identifier for used logger
        /// </summary>
        public const string PACKAGE_LOGGER      = "0EED6224-1E37-43ED-A5A9-A29510020722";

        /// <summary>
        /// Command of main UI
        /// </summary>
        public const string MAIN_CMD_STRING     = "5547e550-6d81-4f97-91ed-338926673efa";

        /// <summary>
        /// Command of StatusPanel
        /// </summary>
        public const string PANEL_CMD_STRING    = "B53B115A-F433-44EB-85E0-94767FAA23E0";

        /// <summary>
        /// Identifier for StatusPanel
        /// </summary>
        public const string PANEL_STRING        = "71D73518-E66A-4EA7-952A-3AF7A23E4C78";

        /// <summary>
        /// Main item for OutputWindow
        /// </summary>
        public const string OWP_SBE_STRING      = "3000844A-77CB-4081-BAA0-915E29EA6EB2";

        /// <summary>
        /// BuildOutput pane
        /// </summary>
        public const string OWP_BUILD_STRING    = "1BD8A850-02D1-11d1-BEE7-00A0C913D1F8";

        /// <summary>
        /// Guid of main UI command
        /// </summary>
        public static readonly Guid MAIN_CMD_SET = new Guid(MAIN_CMD_STRING);

        /// <summary>
        /// Guid of StatusPanel command
        /// </summary>
        public static readonly Guid PANEL_CMD_SET = new Guid(PANEL_CMD_STRING);

        /// <summary>
        /// Guid of main item for OutputWindow
        /// </summary>
        public static readonly Guid OWP_SBE = new Guid(OWP_SBE_STRING);



        /// <summary>
        /// Guid alias to StandardCommandSet97 
        /// </summary>
        public static readonly Guid VSStd97CmdID = VSConstants.CMDSETID.StandardCommandSet97_guid;

        /// <summary>
        /// Guid alias to StandardCommandSet2K
        /// </summary>
        public static readonly Guid VSStd2KCmdID = VSConstants.CMDSETID.StandardCommandSet2K_guid;

    }
}