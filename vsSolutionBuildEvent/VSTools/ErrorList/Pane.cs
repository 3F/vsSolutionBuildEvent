/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;

#if !SDK15_OR_HIGH
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
#endif

namespace net.r_eg.vsSBE.VSTools.ErrorList
{
    public class Pane: IPane, IDisposable
    {
        protected ErrorListProvider provider;

        protected CancellationToken cancellationToken;

        /// <summary>
        /// To add new error in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        public void error(string message)
        {
            task(message, TaskErrorCategory.Error);
        }

        /// <summary>
        /// To add new warning in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        public void warn(string message)
        {
            task(message, TaskErrorCategory.Warning);
        }

        /// <summary>
        /// To add new information in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        public void info(string message)
        {
            task(message, TaskErrorCategory.Message);
        }

        /// <summary>
        /// To clear all messages.
        /// </summary>
        public void clear()
        {
            provider.Tasks.Clear();
        }

        public Pane(IServiceProvider sp, CancellationToken ct)
            : this(sp)
        {
            cancellationToken = ct;
        }

        public Pane(IServiceProvider sp)
        {
            provider = new ErrorListProvider(sp);
        }

        protected void task(string msg, TaskErrorCategory type = TaskErrorCategory.Message)
        {
            // prevents possible bug from `Process.ErrorDataReceived` because of NLog

#if SDK15_OR_HIGH
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () => 
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
#else
            Task.Factory.StartNew(() =>
            {
#endif
                provider.Tasks.Add(new ErrorTask() {
                    Text = msg,
                    Document = Settings.APP_NAME_SHORT,
                    Category = TaskCategory.User,
                    Checked = true,
                    IsCheckedEditable = true,
                    ErrorCategory = type,
                });

#if SDK15_OR_HIGH
            });
#else
            }, 
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);
#endif
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                provider?.Dispose();
                disposed = true;
            }
        }

        #endregion
    }
}
