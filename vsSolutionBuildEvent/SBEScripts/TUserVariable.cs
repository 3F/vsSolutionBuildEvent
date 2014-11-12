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

namespace net.r_eg.vsSBE.SBEScripts
{
    public struct TUserVariable
    {
        /// <summary>
        /// Contains the evaluated data or escaped variable/property (without escape symbol)
        /// Using from current the unevaluated field
        /// </summary>
        public string evaluated;

        /// <summary>
        /// Contains the unevaluated mixed data
        /// May contain the another user-variable etc.
        /// </summary>
        public string unevaluated;

        /// <summary>
        /// Identificator for current variable
        /// </summary>
        public string ident;

        /// <summary>
        /// Current status of evaluation
        /// </summary>
        public StatusType status;

        /// <summary>
        /// Previous TUserVariable if exist.
        /// This probably can be used for self redefinition varname = varname
        /// e.g. for post-processing with MSBuild is required to evaluation of new value etc.
        /// </summary>
        public object prev;

        /// <summary>
        /// Storing in the projects files ~ .csproj, .vcxproj, .. 
        /// or with the external containers
        /// </summary>
        /// <remarks>reserved</remarks>
        public bool persistence;

        /// <summary>
        /// Available states of evaluating
        /// </summary>
        public enum StatusType
        {
            /// <summary>
            /// Stored 'as is'
            /// </summary>
            Unevaluated,
            /// <summary>
            /// Evaluation in progress
            /// </summary>
            Started,
            /// <summary>
            /// End value
            /// </summary>
            Evaluated
        }

        public TUserVariable(TUserVariable origin): this()
        {
            evaluated       = origin.evaluated;
            unevaluated     = origin.unevaluated;
            ident           = origin.ident;
            status          = origin.status;
            persistence     = origin.persistence;
            prev            = origin.prev;
        }
    }
}
