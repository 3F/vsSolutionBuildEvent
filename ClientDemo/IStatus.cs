/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

namespace ClientDemo
{
    internal interface IStatus
    {
        /// <summary>
        /// Report about status
        /// </summary>
        /// <param name="message"></param>
        void report(string message);

        /// <summary>
        /// Show form
        /// </summary>
        void show();
    }
}