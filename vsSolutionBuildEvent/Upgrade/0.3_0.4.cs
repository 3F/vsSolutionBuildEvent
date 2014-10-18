/*
 * Copyright (c) 2013 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Upgrade
{
    public class Migration03_04/*: IMigration*/
    {
        public struct Mock
        {
            /// <summary>
            /// emulate required structure of v0.3 for upgrade on v0.4
            /// </summary>
            [Serializable]
            public class SolutionEvents
            {
                public SBEEvent preBuild    = new SBEEvent();
                public SBEEvent postBuild   = new SBEEvent();
                public SBEEvent cancelBuild = new SBEEvent();
            }

            public class SBEEvent
            {
                public bool modeScript = true;
            }
        }

        public static void migrate(FileStream stream)
        {
            //reset cursor
            stream.Position = 0;

            XmlSerializer xml       = new XmlSerializer(typeof(Mock.SolutionEvents));
            Mock.SolutionEvents v03 = (Mock.SolutionEvents)xml.Deserialize(stream);

            // preBuild

            if(v03.preBuild.modeScript) {
                Config._.Data.PreBuild[0].Mode = new ModeInterpreter();
            }
            else {
                Config._.Data.PreBuild[0].Mode = new ModeFile();
            }

            // postBuild

            if(v03.postBuild.modeScript) {
                Config._.Data.PostBuild[0].Mode = new ModeInterpreter();
            }
            else {
                Config._.Data.PostBuild[0].Mode = new ModeFile();
            }

            // cancelBuild

            if(v03.cancelBuild.modeScript) {
                Config._.Data.CancelBuild[0].Mode = new ModeInterpreter();
            }
            else {
                Config._.Data.CancelBuild[0].Mode = new ModeFile();
            }
        }
    }
}
