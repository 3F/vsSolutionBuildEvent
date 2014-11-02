using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using Newtonsoft.Json;
using NewSBEEvent = net.r_eg.vsSBE.Events.SBEEvent;
using NewSBEEventEW = net.r_eg.vsSBE.Events.SBEEventEW;
using NewSBEEventOWP = net.r_eg.vsSBE.Events.SBEEventOWP;
using NewSBETransmitter = net.r_eg.vsSBE.Events.SBETransmitter;
using NewSolutionEvents = net.r_eg.vsSBE.SolutionEvents;
using OldSBEEvent = net.r_eg.vsSBE.Upgrade.v08.Events.SBEEvent;
using OldSBEEventEW = net.r_eg.vsSBE.Upgrade.v08.Events.SBEEventEW;
using OldSBEEventOWP = net.r_eg.vsSBE.Upgrade.v08.Events.SBEEventOWP;
using OldSBETransmitter = net.r_eg.vsSBE.Upgrade.v08.Events.SBETransmitter;
using OldSolutionEvents = net.r_eg.vsSBE.Upgrade.v08.Events.SolutionEvents;

namespace net.r_eg.vsSBE.Upgrade.v08
{
    public class Migration08_09
    {
        public static NewSolutionEvents migrate(string cfgFile)
        {
            return defineFrom(loadCfg(cfgFile));
        }

        protected static OldSolutionEvents loadCfg(string cfgFile)
        {
            using(StreamReader stream = new StreamReader(cfgFile, Encoding.UTF8, true))
            {
                XmlSerializer xml = new XmlSerializer(typeof(OldSolutionEvents));
                return (OldSolutionEvents)xml.Deserialize(stream);
            }
        }

        protected static NewSolutionEvents defineFrom(OldSolutionEvents data)
        {
            NewSolutionEvents ret = new NewSolutionEvents();

            ret.PreBuild[0]         = defineFrom(data.preBuild);
            ret.CancelBuild[0]      = defineFrom(data.cancelBuild);
            ret.PostBuild[0]        = defineFrom(data.postBuild);
            ret.WarningsBuild[0]    = defineFrom(data.warningsBuild);
            ret.ErrorsBuild[0]      = defineFrom(data.errorsBuild);
            ret.OWPBuild[0]         = defineFrom(data.outputCustomBuild);
            ret.Transmitter[0]      = defineFrom(data.transmitter);
            return ret;
        }

        protected static NewSBEEvent defineFrom(OldSBEEvent evt)
        {
            NewSBEEventEW ret = new NewSBEEventEW();

            ret.Caption             = evt.caption;
            ret.Enabled             = evt.enabled;            
            ret.IgnoreIfBuildFailed = evt.buildFailedIgnore;
            ret.Name                = "Act1";
            ret.Process.Hidden      = evt.processHide;
            ret.Process.KeepWindow  = evt.processKeep;
            ret.Process.Waiting     = evt.waitForExit;
            ret.SupportMSBuild      = evt.parseVariablesMSBuild;
            ret.SupportSBEScripts   = true;
            ret.ToConfiguration     = evt.toConfiguration;

            if(evt.mode == Events.TModeCommands.File)
            {
                ret.Mode = new ModeFile();
                ((IModeFile)ret.Mode).Command = evt.command;
            }
            else if(evt.mode == Events.TModeCommands.Interpreter)
            {
                ret.Mode = new ModeInterpreter();
                ((IModeInterpreter)ret.Mode).Command = evt.command;
                ((IModeInterpreter)ret.Mode).Handler = evt.interpreter;
                ((IModeInterpreter)ret.Mode).Newline = evt.newline;
                ((IModeInterpreter)ret.Mode).Wrapper = evt.wrapper;
            }
            else if(evt.mode == Events.TModeCommands.Operation)
            {
                ret.Mode = new ModeOperation();
                ((IModeOperation)ret.Mode).AbortOnFirstError = evt.dteExec.abortOnFirstError;
                ((IModeOperation)ret.Mode).Caption = evt.dteExec.caption;
                ((IModeOperation)ret.Mode).Command = evt.dteExec.cmd;
            }

            if(evt.executionOrder != null)
            {
                int len = evt.executionOrder.Length;
                ret.ExecutionOrder = new ExecutionOrder[len];
                for(int i = 0; i < len; ++i) {
                    ret.ExecutionOrder[i].Project = evt.executionOrder[i].project;
                    ret.ExecutionOrder[i].Order   = (ExecutionOrderType)evt.executionOrder[i].order;
                }
            }
            return ret;
        }

        protected static NewSBEEventEW defineFrom(OldSBEEventEW evt)
        {
            NewSBEEventEW ret   = (NewSBEEventEW)defineFrom((OldSBEEvent)evt);
            ret.Codes           = evt.codes;
            ret.IsWhitelist     = evt.isWhitelist;
            return ret;
        }

        protected static NewSBEEventOWP defineFrom(OldSBEEventOWP evt)
        {
            NewSBEEventOWP ret = defineFrom((OldSBEEvent)evt).CloneBySerializationWithType<NewSBEEvent, NewSBEEventOWP>();

            if(evt.eventsOWP == null || evt.eventsOWP.Count < 1){
                return ret;
            }

            int count = evt.eventsOWP.Count;
            ret.Match = new MatchWords[count];

            for(int i = 0; i < count; ++i) {
                ret.Match[i].Condition = evt.eventsOWP[i].term;
                ret.Match[i].Type = (ComparisonType)evt.eventsOWP[i].type;
            }
            return ret;
        }

        protected static NewSBETransmitter defineFrom(OldSBETransmitter evt)
        {
            return defineFrom((OldSBEEvent)evt).CloneBySerializationWithType<NewSBEEvent, NewSBETransmitter>();
        }
    }
}
