/*
 * Copyright (c) 2013-2014 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnvDTE80;

namespace net.r_eg.vsSBE.UI
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl: UserControl
    {
        /// <summary>
        /// Simple message counter
        /// </summary>
        protected int exclamationCounter = 0;

        protected DTE2 dte;

        public StatusControl()
        {
            InitializeComponent();
            enabled(false);
        }

        public void updateData()
        {
            btnPre.IsChecked            = Config._.Data.preBuild.enabled;
            btnPost.IsChecked           = Config._.Data.postBuild.enabled;
            btnCancel.IsChecked         = Config._.Data.cancelBuild.enabled;
            btnWarnings.IsChecked       = Config._.Data.warningsBuild.enabled;
            btnErrors.IsChecked         = Config._.Data.errorsBuild.enabled;
            btnOutput.IsChecked         = Config._.Data.outputCustomBuild.enabled;
            btnTransmitter.IsChecked    = Config._.Data.transmitter.enabled;
        }

        public void notify()
        {
            textInfo.Text = (++exclamationCounter).ToString();
        }

        public void enabled(bool flag)
        {
            grid.IsEnabled = flag;
        }

        public void setDTE(DTE2 dte)
        {
            this.dte = dte;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if(dte == null) {
                Log.nlog.Error("UI-Settings :: DTE not initialized");
                return;
            }

            try {
                dte.ExecuteCommand("Build.EventsSolution");
                Log.nlog.Trace("'Build.EventsSolution' pushed");
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed call 'Build.EventsSolution' {0}", ex.Message);
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Log.show();
            exclamationCounter = 0;
            textInfo.Text = "0";
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.preBuild.enabled = btnPre.IsChecked.Value;
            Config._.save();
        }

        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.postBuild.enabled = btnPost.IsChecked.Value;
            Config._.save();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.cancelBuild.enabled = btnCancel.IsChecked.Value;
            Config._.save();
        }

        private void btnWarnings_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.warningsBuild.enabled = btnWarnings.IsChecked.Value;
            Config._.save();
        }

        private void btnErrors_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.errorsBuild.enabled = btnErrors.IsChecked.Value;
            Config._.save();
        }

        private void btnOutput_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.outputCustomBuild.enabled = btnOutput.IsChecked.Value;
            Config._.save();
        }

        private void btnTransmitter_Click(object sender, RoutedEventArgs e)
        {
            Config._.Data.transmitter.enabled = btnTransmitter.IsChecked.Value;
            Config._.save();
        }
    }
}
