/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    internal sealed class ExecLocator(ISolutionEvent evt, SolutionEventType type)
    {
        public ISolutionEvent Evt { get; } = evt;

        public SolutionEventType EvtType { get; } = type;
    }
}
