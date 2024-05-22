/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.UI;

#if !SDK15_OR_HIGH
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
#endif

namespace net.r_eg.vsSBE.VSTools.ErrorList
{
    public class Pane(IServiceProvider sp): IPane, IDisposable
    {
        protected readonly ErrorListProvider provider = new(sp);

        protected readonly CancellationToken cancellationToken;

        private UI.WForms.EventsFrm frm;

        public void error(string message, string src, string type)
        {
            push(message, src, type, TaskErrorCategory.Error);
        }

        public void warn(string message, string source, string type)
        {
            push(message, source, type, TaskErrorCategory.Warning);
        }

        public void info(string message, string source, string type)
        {
            push(message, source, type, TaskErrorCategory.Message);
        }

        public void clear() => provider.Tasks.Clear();

        public Pane(IServiceProvider sp, CancellationToken ct)
            : this(sp)
        {
            cancellationToken = ct;
        }

        protected void push(string msg, string src, string type, TaskErrorCategory level = TaskErrorCategory.Message)
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
                string loc;

                if(!string.IsNullOrEmpty(src))
                {
                    loc = $"{src}@{Settings.APP_CFG}";
                }
                else
                {
                    loc = Settings.APP_CFG;
                }

                ErrorTask err = new()
                {
                    Text = msg,
                    Document = loc,
                    Category = TaskCategory.User,
                    Checked = true,
                    IsCheckedEditable = true,
                    ErrorCategory = level,
                };

                err.Navigate += (sender, e) =>
                {
                    if(string.IsNullOrEmpty(src) || string.IsNullOrEmpty(type)) return;

                    show().activateAction
                    (
                        src,
                        (SolutionEventType)Enum.Parse(typeof(SolutionEventType), type)
                    );
                };

                provider.Tasks.Add(err);

#if SDK15_OR_HIGH
            });
#else
            }, 
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);
#endif
        }

        private ICodeInspector show()
        {
            try
            {
                if(UI.Util.focusForm(frm)) return frm;

                frm = new UI.WForms.EventsFrm(Bootloader._);
                frm.Show();

                return frm;
            }
            catch(Exception ex)
            {
                Log.Error($"Failed UI: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            return null;
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

                if(frm?.IsDisposed == false)
                {
                    frm.Close();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
