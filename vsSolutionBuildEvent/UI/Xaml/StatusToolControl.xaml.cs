/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using EnvDTE80;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI.Xaml
{
    public partial class StatusToolControl: UserControl
    {
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
            update(btnPre, SolutionEventType.Pre);
            update(btnPost, SolutionEventType.Post);
            update(btnCancel, SolutionEventType.Cancel);
            update(btnWarnings, SolutionEventType.Warnings);
            update(btnErrors, SolutionEventType.Errors);
            update(btnOutput, SolutionEventType.OWP);
            update(btnTransmitter, SolutionEventType.Transmitter);
        }

        /// <summary>
        /// Enabling all controls for current panel
        /// </summary>
        public void enabledPanel(bool flag)
        {
            grid.IsEnabled = flag;
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
        /// <param name="btn"></param>
        /// <param name="type"></param>
        protected void update(ToggleButton btn, SolutionEventType type)
        {
            try {
                logic.update(type);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    btn.Content = caption(type, false);
                    btn.IsChecked = !isDisabledAll(type);
                }));
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
                logic.executeCommand("Build.vsSBE.Settings");
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
