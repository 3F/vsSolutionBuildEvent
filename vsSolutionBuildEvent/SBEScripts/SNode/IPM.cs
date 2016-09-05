/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.SBEScripts.SNode
{
    [Guid("DAEF9EE0-F8DA-4A70-BD5B-3517069EBFFF")]
    public interface IPM
    {
        /// <summary>
        /// Found levels.
        /// </summary>
        List<ILevel> Levels { get; }

        /// <summary>
        /// Condition for analyzer.
        /// </summary>
        string Condition { get; }

        /// <summary>
        /// Checks equality for level.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool Is(int level, LevelType type, string data = null);

        /// <summary>
        /// Checks equality for level with additional checking of finalization in levels chain.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool FinalIs(int level, LevelType type, string data = null);

        /// <summary>
        /// Checks equality for level with additional checking of finalization as RightOperandEmpty in levels chain.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool FinalEmptyIs(int level, LevelType type, string data = null);

        /// <summary>
        /// Slicing of current levels to selected.
        /// </summary>
        /// <param name="level">New start position.</param>
        /// <returns>Self reference.</returns>
        IPM pinTo(int level);

        /// <summary>
        /// Get all levels from selected.
        /// </summary>
        /// <param name="level">Start position.</param>
        /// <returns>New instance of IPM.</returns>
        IPM getFrom(int level);

        /// <summary>
        /// The string of diagnostic information about level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        string traceLevel(int level = 0);

        /// <summary>
        /// Throws error for level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ident">Custom id of place where occurred.</param>
        void fail(int level = 0, string ident = null);

        /// <summary>
        /// Checks equality for zero level.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool Is(LevelType type, string data = null);

        /// <summary>
        /// Checks equality for zero level with additional checking of finalization in levels chain.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool FinalIs(LevelType type, string data = null);

        /// <summary>
        /// Checks equality for zero level with additional checking of finalization as RightOperandEmpty in levels chain.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool FinalEmptyIs(LevelType type, string data = null);

        /// <summary>
        /// Checks equality for zero level and move to next level if it is equal to this data.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool It(LevelType type, string data = null);

        /// <summary>
        /// Checks equality for specific level and move to next level if it is equal to this data.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        bool It(int level, LevelType type, string data = null);

        /// <summary>
        /// Checks equality of method for specific level.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="name">Method name.</param>
        /// <param name="types">The arguments that should be.</param>
        /// <returns></returns>
        bool IsMethodWithArgs(int level, string name, params ArgumentType[] types);

        /// <summary>
        /// Checks equality of method for zero level.
        /// </summary>
        /// <param name="name">Method name.</param>
        /// <param name="types">The arguments that should be.</param>
        /// <returns></returns>
        bool IsMethodWithArgs(string name, params ArgumentType[] types);

        /// <summary>
        /// Checks type of right operand for zero level.
        /// </summary>
        /// <param name="type">The right operand should be with level type.</param>
        /// <returns>true value if the right operand is equal to selected level type, otherwise false.</returns>
        bool IsRight(LevelType type);

        /// <summary>
        /// Checks equality of data for zero level.
        /// </summary>
        /// <param name="data">Level should be with data.</param>
        /// <param name="variants">Alternative variants that can be.</param>
        /// <returns>true value if selected level is equal to selected data, otherwise false.</returns>
        bool IsData(string data, params string[] variants);

        /// <summary>
        /// Extracts all arguments from raw data.
        /// </summary>
        /// <param name="raw">Raw data of arguments.</param>
        /// <param name="splitter">A character that delimits arguments.</param>
        /// <returns>List of parsed arguments or null value if data is empty or null.</returns>
        /// <exception cref="SyntaxIncorrectException">If incorrect data.</exception>
        Argument[] arguments(string raw, char splitter = ',');
    }
}