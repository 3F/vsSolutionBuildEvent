// Guids.cs
// MUST match guids.h
using System;

namespace reg.ext.vsSolutionBuildEvent
{
    static class GuidList
    {
        public const string guidvsSolutionBuildEventPkgString = "94ecd13f-15f3-4f51-9afd-17f0275c6266";
        public const string guidvsSolutionBuildEventCmdSetString = "5547e550-6d81-4f97-91ed-338926673efa";

        public static readonly Guid guidvsSolutionBuildEventCmdSet = new Guid(guidvsSolutionBuildEventCmdSetString);
    };
}