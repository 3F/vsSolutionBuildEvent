/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Bridge;

namespace ClientDemo
{
    public class Event: IEvent2
    {
        protected readonly ILog log;

        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            log.Info("Entering solutionOpened(object pUnkReserved, int fNewSolution)");
            return Codes.Success;
        }

        public int solutionClosed(object pUnkReserved)
        {
            log.Info("Entering solutionClosed(object pUnkReserved)");
            return Codes.Success;
        }

        public int onPre(ref int pfCancelUpdate)
        {
            log.Info("Entering onPre(ref int pfCancelUpdate)");
            return Codes.Success;
        }

        public int onCancel()
        {
            log.Info("Entering onCancel()");
            return Codes.Success;
        }

        public int onPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            log.Info("Entering onPost(int fSucceeded, int fModified, int fCancelCommand)");
            return Codes.Success;
        }

        public int onProjectPre(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, ref int pfCancel)
        {
            log.Info("Entering onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel)");
            return Codes.Success;
        }

        public int onProjectPre(string project)
        {
            log.Info($"Entering onProjectPre(project: {project})");
            return Codes.Success;
        }

        public int onProjectPost(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            log.Info("Entering onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel)");
            return Codes.Success;
        }

        public int onProjectPost(string project, int fSuccess)
        {
            log.Info($"Entering onProjectPost(project: {project}, int fSuccess)");
            return Codes.Success;
        }

        public int onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            log.Info($"({guid}, id: {id}, customIn, customOut, ref cancelDefault)");
            return Codes.Success;
        }

        public int onCommandDtePost(string guid, int id, object customIn, object customOut)
        {
            log.Info($"({guid}, id: {id}, customIn, customOut)");
            return Codes.Success;
        }

        public Event(ILog log)
        {
            this.log = log;
        }
    }
}
