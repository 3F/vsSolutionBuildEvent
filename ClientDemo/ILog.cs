/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

namespace ClientDemo
{
    internal interface ILog
    {
        /// <summary>
        /// Message for information level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void info(string message, params object[] args);

        /// <summary>
        /// Show messages if it's possible
        /// </summary>
        void show();

        /// <summary>
        /// Initialize with IStatus
        /// </summary>
        /// <param name="status"></param>
        /// <returns>self reference</returns>
        ILog init(IStatus status);
    }
}