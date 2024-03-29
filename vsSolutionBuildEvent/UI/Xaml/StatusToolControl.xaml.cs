﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using net.r_eg.vsSBE.Events;

#if SDK15_OR_HIGH
using Microsoft.VisualStudio.Shell;
#endif

namespace net.r_eg.vsSBE.UI.Xaml
{
    public partial class StatusToolControl: UserControl, IStatusTool
    {
        protected Logic.StatusTool logic;

        /// <summary>
        /// Get number from Warnings counter
        /// </summary>
        public int Warnings
        {
            get => logic.Warnings;
        }

        /// <summary>
        /// Availability of main panel for user
        /// </summary>
        /// <param name="enabled"></param>
        public void enabledPanel(bool enabled)
        {
            if(!enabled) {
                resetCounter();
            }
            grid.IsEnabled = enabled;
        }

        /// <summary>
        /// Resets the Warnings counter
        /// </summary>
        public void resetCounter()
        {
            logic.resetWarnings();
            textInfo.Text = "0";
        }

        /// <summary>
        /// Notification about any warnings
        /// </summary>
        public void warn()
        {
#if SDK15_OR_HIGH
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
#else
            Application.Current.Dispatcher.Invoke(() =>
            {
#endif
                textInfo.Text = logic.addWarning().ToString();

            });
        }

        /// <summary>
        /// Updates data for controls
        /// </summary>
        public void refresh()
        {
            update(btnPre, SolutionEventType.Pre);
            update(btnPost, SolutionEventType.Post);
            update(btnCancel, SolutionEventType.Cancel);
            update(btnDTE, SolutionEventType.CommandEvent);
            update(btnWarnings, SolutionEventType.Warnings);
            update(btnErrors, SolutionEventType.Errors);
            update(btnOutput, SolutionEventType.OWP);
            update(btnTransmitter, SolutionEventType.Transmitter);
            update(btnLogging, SolutionEventType.Logging);
        }

        public StatusToolControl()
        {
            logic = new Logic.StatusTool();
            InitializeComponent();
            enabledPanel(false);
        }

        /// <summary>
        /// Captions for buttons
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selected"></param>
        protected string caption(SolutionEventType type, bool selected)
        {
            try {
                return logic.caption(type, selected);
            }
            catch(Exception ex){
                Log.Warn("StatusToolControl: problem with caption '{0}'", ex.Message);
            }
            return logic.caption(type);
        }

        /// <summary>
        /// Updating status for used event type
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="type"></param>
        protected void update(ToggleButton btn, SolutionEventType type)
        {
            try
            {
                logic.update(type);
            }
            catch(Exception ex) {
                Log.Warn("StatusToolControl: Failed update for type - '{0}' :: '{1}'", type, ex.Message);
            }

#if SDK15_OR_HIGH
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
#else
            Application.Current.Dispatcher.Invoke(() =>
            {
#endif
                btn.Content     = caption(type, false);
                btn.IsChecked   = !isDisabledAll(type);

            });
        }

        protected bool isDisabledAll(SolutionEventType type)
        {
            try {
                return logic.isDisabledAll(type);
            }
            catch(Exception ex) {
                Log.Warn("StatusToolControl: Failed checking the Enabled status for type - '{0}' :: '{1}'", type, ex.Message);
            }
            return true;
        }

        protected void toggleRestored(SolutionEventType type, bool enabled)
        {
            if(enabled) {
                restore(type);
            }
            else {
                this.enabled(type, false);
            }
            Settings._.Config.Sln.save();
        }

        protected bool toggleEnabled(SolutionEventType type, bool enabled)
        {
            bool ret;
            if(!enabled) {
                this.enabled(type, true);
                ret = true;
            }
            else {
                this.enabled(type, false);
                ret = false;
            }
            Settings._.Config.Sln.save();
            return ret;
        }

        protected void enabled(SolutionEventType type, bool status)
        {
            try {
                logic.enabled(type, status);
            }
            catch(Exception ex) {
                Log.Warn("StatusToolControl: Failed - enabled() for type - '{0}' :: '{1}'", type, ex.Message);
            }
        }

        protected void restore(SolutionEventType type)
        {
            try {
                logic.restore(type);
            }
            catch(Exception ex) {
                Log.Warn("StatusToolControl: Failed - restore() for type - '{0}' :: '{1}'", type, ex.Message);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            try {
                logic.executeCommand("Build.vsSBE.Settings");
            }
            catch(Exception ex) {
                Log.Warn("Cannot open window with settings'{0}'", ex.Message);
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Log._.show();
            //resetCounter();
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Pre, btnPre.IsChecked.Value);
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Post, btnPost.IsChecked.Value);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Cancel, btnCancel.IsChecked.Value);
        }

        private void btnDTE_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.CommandEvent, btnDTE.IsChecked.Value);
        }

        private void btnWarnings_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Warnings, btnWarnings.IsChecked.Value);
        }

        private void btnErrors_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Errors, btnErrors.IsChecked.Value);
        }

        private void btnOutput_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.OWP, btnOutput.IsChecked.Value);
        }

        private void btnTransmitter_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Transmitter, btnTransmitter.IsChecked.Value);
        }

        private void btnLogging_Click(object sender, RoutedEventArgs e)
        {
            toggleRestored(SolutionEventType.Logging, btnLogging.IsChecked.Value);
        }

        private void btnPre_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnPre.Content = caption(SolutionEventType.Pre, true);
        }

        private void btnPre_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnPre.Content = caption(SolutionEventType.Pre, false);
        }

        private void btnPost_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnPost.Content = caption(SolutionEventType.Post, true);
        }

        private void btnPost_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnPost.Content = caption(SolutionEventType.Post, false);
        }

        private void btnCancel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCancel.Content = caption(SolutionEventType.Cancel, true);
        }

        private void btnCancel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnCancel.Content = caption(SolutionEventType.Cancel, false);
        }

        private void btnDTE_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnDTE.Content = caption(SolutionEventType.CommandEvent, true);
        }

        private void btnDTE_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnDTE.Content = caption(SolutionEventType.CommandEvent, false);
        }

        private void btnWarnings_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnWarnings.Content = caption(SolutionEventType.Warnings, true);
        }

        private void btnWarnings_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnWarnings.Content = caption(SolutionEventType.Warnings, false);
        }

        private void btnErrors_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnErrors.Content = caption(SolutionEventType.Errors, true);
        }

        private void btnErrors_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnErrors.Content = caption(SolutionEventType.Errors, false);
        }

        private void btnOutput_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnOutput.Content = caption(SolutionEventType.OWP, true);
        }

        private void btnOutput_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnOutput.Content = caption(SolutionEventType.OWP, false);
        }

        private void btnTransmitter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTransmitter.Content = caption(SolutionEventType.Transmitter, true);
        }

        private void btnLogging_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnLogging.Content = caption(SolutionEventType.Logging, true);
        }

        private void btnTransmitter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTransmitter.Content = caption(SolutionEventType.Transmitter, false);
        }

        private void btnLogging_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnLogging.Content = caption(SolutionEventType.Logging, false);
        }
        
        private void btnPre_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnPre.IsChecked = toggleEnabled(SolutionEventType.Pre, btnPre.IsChecked.Value);
            }
        }

        private void btnPost_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnPost.IsChecked = toggleEnabled(SolutionEventType.Post, btnPost.IsChecked.Value);
            }
        }

        private void btnCancel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnCancel.IsChecked = toggleEnabled(SolutionEventType.Cancel, btnCancel.IsChecked.Value);
            }
        }

        private void btnDTE_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnDTE.IsChecked = toggleEnabled(SolutionEventType.CommandEvent, btnDTE.IsChecked.Value);
            }
        }

        private void btnWarnings_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnWarnings.IsChecked = toggleEnabled(SolutionEventType.Warnings, btnWarnings.IsChecked.Value);
            }
        }

        private void btnErrors_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnErrors.IsChecked = toggleEnabled(SolutionEventType.Errors, btnErrors.IsChecked.Value);
            }
        }

        private void btnOutput_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnOutput.IsChecked = toggleEnabled(SolutionEventType.OWP, btnOutput.IsChecked.Value);
            }
        }

        private void btnTransmitter_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnTransmitter.IsChecked = toggleEnabled(SolutionEventType.Transmitter, btnTransmitter.IsChecked.Value);
            }
        }

        private void btnLogging_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Middle) {
                btnLogging.IsChecked = toggleEnabled(SolutionEventType.Logging, btnLogging.IsChecked.Value);
            }
        }
    }
}
