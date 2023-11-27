/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

namespace ClientDemo
{
    internal interface IStatus
    {
        void Report(string message);
        
        void Show();
    }
}