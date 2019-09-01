/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) E-MSBuild contributors: https://github.com/3F/E-MSBuild/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Build.Evaluation;
using net.r_eg.Components;

namespace net.r_eg.EvMSBuild
{
    internal sealed class VaLier: IDisposable
    {
        private const string CONTAINER = nameof(EvMSBuild) + "__e_container";

        private readonly IEvMSBuildEx evm;

        private readonly Project origin;
        private readonly Project scoped;

        private readonly object sync = new object();

        private IDictionary<string, string> CoProps => new Dictionary<string, string>(origin.GlobalProperties) {
            [$"__e_{nameof(VaLier)}"] = DateTime.UtcNow.Ticks.ToString()
        };

        public string Compute(string value)
            => Compute(value, CultureInfo.InvariantCulture);

        public string Compute(string value, CultureInfo culture)
        {
            if(value == null) {
                return string.Empty;
            }

            lock(sync)
            {
                CultureInfo origincul = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;

                ProjectProperty prop = null;
                try
                {
                    evm.DefProperties(scoped);

                    prop = scoped.SetProperty(CONTAINER, value);
                    return prop.EvaluatedValue;
                }
                finally
                {
                    if(prop != null) scoped.RemoveProperty(prop);

                    Thread.CurrentThread.CurrentCulture = origincul;

                    // NOTE: about solutions for "Save changes to the following items":

                    // 1) Do not use the `.Save();` on `EProject` because of possible "File Modification Detected ... has been modified outside the environment."
                    // 2) Do not use `.Save();` on `DProject` because of possible "Operation aborted (Exception from HRESULT: 0x80004004 (E_ABORT))"
                    // For DProject it also activates "Save As" dialog in VS even inside try/catch
                    // 3) temp project.DisableMarkDirty = true;  ~may not help
                }
            }
        }

        public VaLier(Project project, IEvMSBuildEx evm)
        {
            origin      = project ?? throw new ArgumentNullException(nameof(project));
            this.evm    = evm ?? throw new ArgumentNullException(nameof(evm));

            scoped = new Project(CoProps, origin.ToolsVersion, ProjectCollection.GlobalProjectCollection) {
                SkipEvaluation = true
            };

            CopyProperties(origin, scoped);
            scoped.SkipEvaluation = false;
        }

        private void CopyProperties(Project from, Project to)
        {
            foreach(var p in from.Properties)
            {
                if(!p.IsReservedProperty && !p.IsGlobalProperty)
                {
                    to.SetProperty(p.Name, p.UnevaluatedValue);
                }
            }
        }

        private bool Unload(Project prj)
        {
            if(prj == null) {
                return false;
            }

            try
            {
                if(prj.FullPath != null) {
                    ProjectCollection.GlobalProjectCollection.UnloadProject(prj);
                }
                else if(prj.Xml != null) {
                    ProjectCollection.GlobalProjectCollection.TryUnloadProject(prj.Xml);
                }

                return true;
            }
            catch(Exception ex)
            {
                LSender.Send(this, $"Project cannot be unloaded due to error: '{ex.Message}'", MsgLevel.Debug);
                return false;
            }
        }

        #region IDisposable

        private bool disposed = false;
        public void Dispose() => Dispose(true);

        void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }

            disposed = true;

            Unload(scoped);
        }

        #endregion
    }
}
