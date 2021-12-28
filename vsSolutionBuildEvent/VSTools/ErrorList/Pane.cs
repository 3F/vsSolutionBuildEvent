﻿/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
