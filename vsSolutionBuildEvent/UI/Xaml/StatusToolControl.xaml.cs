/*
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EnvDTE80;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI.Xaml
{
    public partial class StatusToolControl: UserControl
    {
        /// <summary>
        /// Used DTE context
        /// </summary>
        protected DTE2 dte2;

        /// <summary>
        /// Logic for this UI
        /// </summary>
        protected Logic.StatusTool logic;

        public StatusToolControl()
        {
            logic = new Logic.StatusTool();
            InitializeComponent();
            enabledPanel(false);
        }

        /// <summary>
        /// Called with any warnings / errors
        /// </summary>
        public void notify()
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    textInfo.Text = logic.addWarning().ToString();
                }));
            }
            catch(Exception ex) {
                Log.nlog.Debug("Failed StatusToolControl::notify: '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Called with updating config
        /// </summary>
        public void updateData()
        {
            update(SolutionEventType.Pre);
            btnPre.Content = caption(SolutionEventType.Pre, false);
            btnPre.IsChecked = !isDisabledAll(SolutionEventType.Pre);

            update(SolutionEventType.Post);
            btnPost.Content = caption(SolutionEventType.Post, false);
            btnPost.IsChecked = !isDisabledAll(SolutionEventType.Post);

            update(SolutionEventType.Cancel);
            btnCancel.Content = caption(SolutionEventType.Cancel, false);
            btnCancel.IsChecked = !isDisabledAll(SolutionEventType.Cancel);

            update(SolutionEventType.Warnings);
            btnWarnings.Content = caption(SolutionEventType.Warnings, false);
            btnWarnings.IsChecked = !isDisabledAll(SolutionEventType.Warnings);

            update(SolutionEventType.Errors);
            btnErrors.Content = caption(SolutionEventType.Errors, false);
            btnErrors.IsChecked = !isDisabledAll(SolutionEventType.Errors);

            update(SolutionEventType.OWP);
            btnOutput.Content = caption(SolutionEventType.OWP, false);
            btnOutput.IsChecked = !isDisabledAll(SolutionEventType.OWP);

            update(SolutionEventType.Transmitter);
            btnTransmitter.Content = caption(SolutionEventType.Transmitter, false);
            btnTransmitter.IsChecked = !isDisabledAll(SolutionEventType.Transmitter);
        }

        /// <summary>
        /// Enabling all controls for current panel
        /// </summary>
        public void enabledPanel(bool flag)
        {
            grid.IsEnabled = flag;
        }

        /// <summary>
        /// Update DTE context
        /// </summary>
        public void setDTE(DTE2 dte2)
        {
            this.dte2 = dte2;
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
                Log.nlog.Warn("StatusToolControl: problem with caption '{0}'", ex.Message);
            }
            return logic.caption(type);
        }

        /// <summary>
        /// Updating status for used event type
        /// </summary>
        /// <param name="type"></param>
        protected void update(SolutionEventType type)
        {
            try {
                logic.update(type);
            }
            catch(Exception ex) {
                Log.nlog.Warn("StatusToolControl: Failed update for type - '{0}' :: '{1}'", type, ex.Message);
            }
        }

        protected bool isDisabledAll(SolutionEventType type)
        {
            try {
                return logic.isDisabledAll(type);
            }
            catch(Exception ex) {
                Log.nlog.Warn("StatusToolControl: Failed checking the Enabled status for type - '{0}' :: '{1}'", type, ex.Message);
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
            Config._.save();
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
            Config._.save();
            return ret;
        }

        protected void enabled(SolutionEventType type, bool status)
        {
            try {
                logic.enabled(type, status);
            }
            catch(Exception ex) {
                Log.nlog.Warn("StatusToolControl: Failed - enabled() for type - '{0}' :: '{1}'", type, ex.Message);
            }
        }

        protected void restore(SolutionEventType type)
        {
            try {
                logic.restore(type);
            }
            catch(Exception ex) {
                Log.nlog.Warn("StatusToolControl: Failed - restore() for type - '{0}' :: '{1}'", type, ex.Message);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            try {
                logic.executeCommand(dte2, "Build.EventsSolution");
            }
            catch(Exception ex) {
                Log.nlog.Warn("Cannot open window with settings'{0}'", ex.Message);
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Log.show();
            logic.resetWarnings();
            textInfo.Text = "0";
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

        private void btnTransmitter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnTransmitter.Content = caption(SolutionEventType.Transmitter, false);
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
    }
}
