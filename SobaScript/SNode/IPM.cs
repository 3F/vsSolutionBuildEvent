/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Runtime.InteropServices;

namespace net.r_eg.SobaScript.SNode
{
    [Guid("DAEF9EE0-F8DA-4A70-BD5B-3517069EBFFF")]
    public interface IPM
    {
        /// <summary>
        /// Access to found levels.
        /// </summary>
        RLevels Levels { get; }

        /// <summary>
        /// Access to first level.
        /// </summary>
        ILevel FirstLevel { get; set; }

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
        IPM PinTo(int level);

        /// <summary>
        /// Get all levels from selected.
        /// </summary>
        /// <param name="level">Start position.</param>
        /// <returns>New instance of IPM.</returns>
        IPM GetFrom(int level);

        /// <summary>
        /// The string of diagnostic information about level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        string TraceLevel(int level = 0);

        /// <summary>
        /// Throws error for level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ident">Custom id of place where occurred.</param>
        void Fail(int level = 0, string ident = null);

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
        RArgs GetArguments(string raw, char splitter = ',');
    }
}