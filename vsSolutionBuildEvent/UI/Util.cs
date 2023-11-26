/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using net.r_eg.vsSBE.UI.WForms.Components;

namespace net.r_eg.vsSBE.UI
{
    public class Util
    {
        /// <summary>
        /// Fixes bug with height on first row of DataGridView
        /// Height used from RowTemplate.Height
        /// </summary>
        /// <param name="grid"></param>
        public static void fixDGVRowHeight(DataGridView grid)
        {
            foreach(DataGridViewRow row in grid.Rows) {
                row.Height = grid.RowTemplate.Height;
            }
        }

        /// <summary>
        /// Getting the all controls based on predicate
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="predicate"></param>
        /// <returns>found controls</returns>
        public static IEnumerable<Control> getControls(Control ctrl, Func<Control, bool> predicate)
        {
            IEnumerable<Control> tctrl = ctrl.Controls.Cast<Control>();
            return tctrl.SelectMany(c => getControls(c, predicate)).Concat(tctrl).Where(predicate);
        }

        /// <summary>
        /// Subscribing to any changes of the controls by type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="control">where to look</param>
        /// <param name="callback"></param>
        /// <param name="except">Optional the exception list with names what should be ignored</param>
        public static void noticeAboutChanges(Type type, Form control, EventHandler callback, string[] except = null)
        {
            foreach(Control ctrl in getControls(control, 
                                                c =>
                                                    c.GetType() == type
                                                    &&
                                                    (
                                                        (except != null && !except.Contains(c.Name))
                                                        ||
                                                        except == null
                                                    )
                                               ))
            {
                if(type == typeof(CheckBox)) {
                    ((CheckBox)ctrl).CheckedChanged -= callback;
                    ((CheckBox)ctrl).CheckedChanged += callback;
                    continue;
                }

                if(type == typeof(RadioButton)) {
                    ((RadioButton)ctrl).CheckedChanged -= callback;
                    ((RadioButton)ctrl).CheckedChanged += callback;
                    continue;
                }

                if(type == typeof(TextBox)) {
                    ((TextBox)ctrl).TextChanged -= callback;
                    ((TextBox)ctrl).TextChanged += callback;
                    continue;
                }

                if(type == typeof(ListBox)) {
                    ((ListBox)ctrl).SelectedIndexChanged -= callback;
                    ((ListBox)ctrl).SelectedIndexChanged += callback;
                    continue;
                }

                if(type == typeof(ComboBox)) {
                    ((ComboBox)ctrl).TextChanged -= callback;
                    ((ComboBox)ctrl).TextChanged += callback;
                    continue;
                }

                if(type == typeof(RichTextBox)) {
                    ((RichTextBox)ctrl).TextChanged -= callback;
                    ((RichTextBox)ctrl).TextChanged += callback;
                    continue;
                }

                if(type == typeof(CheckedListBox)) {
                    ItemCheckEventHandler call = (sender, e) => { callback(sender, (EventArgs)e); };
                    ((CheckedListBox)ctrl).ItemCheck -= call;
                    ((CheckedListBox)ctrl).ItemCheck += call;
                    continue;
                }

                if(type == typeof(DataGridView) || type == typeof(DataGridViewExt))
                {
                    DataGridViewCellEventHandler call = (sender, e) => { callback(sender, (EventArgs)e); };
                    ((DataGridView)ctrl).CellValueChanged -= call;
                    ((DataGridView)ctrl).CellValueChanged += call;
                    continue;
                }

                if(type == typeof(PropertyGrid))
                {
                    PropertyValueChangedEventHandler call = (sender, e) => { callback(sender, (EventArgs)e); };
                    ((PropertyGrid)ctrl).PropertyValueChanged -= call;
                    ((PropertyGrid)ctrl).PropertyValueChanged += call;
                    continue;
                }
            }
        }

        /// <summary>
        /// Open url in default web-browser
        /// </summary>
        /// <param name="url"></param>
        /// <param name="exceptions">flag of suppress exceptions</param>
        public static void openUrl(string url, bool exceptions = false)
        {
            if(exceptions) {
                defaultBrowserOpen(url);
                return;
            }

            try {
                defaultBrowserOpen(url);
            }
            catch(Exception ex) {
                Log.Warn(ex.Message);
            }
        }

        /// <summary>
        /// Simply changes the Width property with smoothing, for any controls
        /// </summary>
        /// <param name="control"></param>
        /// <param name="max">max Width</param>
        /// <param name="distance">Steps in the range (0-1)</param>
        /// <param name="delay">in milliseconds</param>
        public static void effectSmoothChangeWidth(Control control, int max, float distance, int delay)
        {
            distance = Math.Max(0, Math.Min(distance, 1));
            (new System.Threading.Tasks.Task(() =>
            {
                int step = (int)(max * distance);
                while(control.Width < max)
                {
                    control.BeginInvoke((MethodInvoker)delegate {
                        control.Width += step;
                    });
                    System.Threading.Thread.Sleep(delay);
                }
                control.BeginInvoke((MethodInvoker)delegate {
                    control.Width = max;
                });
            })).Start();
        }

        /// <summary>
        /// Helper to focusing form
        /// </summary>
        /// <param name="frm"></param>
        /// <returns>false value if form not created or already is disposed</returns>
        public static bool focusForm(Form frm)
        {
            if(frm != null && !frm.IsDisposed) {
                frm.WindowState = FormWindowState.Normal;
                frm.Focus();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for closing tool
        /// </summary>
        /// <param name="frm"></param>
        public static void closeTool(Form frm)
        {
            if(frm != null && !frm.IsDisposed) {
                frm.Close(); //+Dispose
            }
        }

        /// <summary>
        /// Find the enum definition by Guid string / Id
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string enumViewBy(string guid, int id)
        {
            return enumViewBy(new Guid(guid), id);
        }

        /// <summary>
        /// Find the enum definition by Guid / Id
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string enumViewBy(Guid guid, int id)
        {
            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Type type in asm.SelectMany(a => 
                                                {
                                                    try {
                                                        return a.GetTypes();
                                                    }
                                                    catch(ReflectionTypeLoadException ex) {
                                                        Log.Trace("Enum parser: types cannot be loaded.. so we don't know what is it - '{0}':{1} ", guid, id);
                                                        return ex.Types.Where(t => t != null);
                                                    }
                                                })
                                                .Where(t => t.IsEnum))
            {
                if(guid != type.GUID) {
                    continue;
                }

                string prefix   = type.ToString();
                string value    = id.ToString();

                try {
                    value = Enum.Parse(type, value).ToString();
                }
                catch(Exception ex) {
                    Log.Debug("Enum parser failed: guid({0}), id({1}) -> '{2}' /error: '{3}'", guid, id, prefix, ex.Message);
                }
                return String.Format("{0}.{1}", prefix, value);
            }
            return null;
        }

        protected static void defaultBrowserOpen(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
