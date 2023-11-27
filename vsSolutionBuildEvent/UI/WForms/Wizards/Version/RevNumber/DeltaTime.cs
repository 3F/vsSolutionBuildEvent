/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version.RevNumber
{
    internal sealed class DeltaTime: IRevNumber
    {
        /// <summary>
        /// Interval for time.
        /// </summary>
        public IntervalType interval = IntervalType.TotalMinutes;

        /// <summary>
        /// Basis for time.
        /// </summary>
        public DateTime timeBase = DateTime.Today;

        /// <summary>
        /// The revision of modulo
        /// </summary>
        public TRevModulo revMod;

        /// <summary>
        /// The type of this revision number.
        /// </summary>
        public Type Type
        {
            get { return Type.DeltaTime; }
        }

        /// <summary>
        /// Available types of intervals for 'Delta of time' method.
        /// </summary>
        public enum IntervalType
        {
            TotalMinutes,
            TotalHours,
            TotalSeconds,
            TotalDays,
            //TotalMilliseconds, :)
        }

        /// <summary>
        /// The type for revision of modulo
        /// </summary>
        public struct TRevModulo
        {
            public bool enabled;
            public int min;
            public int max;
        }

        /// <summary>
        /// List of available types of intervals for 'Delta of time' method.
        /// </summary>
        public List<KeyValuePair<IntervalType, string>> IntervalTypeList
        {
            get;
            private set;
        }

        public DeltaTime()
        {
            timeBase = DateTime.Today.AddDays(-2);

            revMod = new TRevModulo() {
                enabled = true,
                min     = 100,
                max     = 100000
            };

            IntervalTypeList = Enum.GetValues(typeof(IntervalType))
                                    .Cast<IntervalType>()
                                    .Select(v => new KeyValuePair<IntervalType, string>(v, v.ToString()))
                                    .ToList();
        }
    }
}
