/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

namespace ClientDemo
{
    public interface ILog
    {
        void Info(string message, params object[] args);
    }
}