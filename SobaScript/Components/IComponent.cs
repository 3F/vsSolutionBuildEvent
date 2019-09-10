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

namespace net.r_eg.SobaScript.Components
{
    public interface IComponent
    {
        /// <summary>
        /// An activation status of this component.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        string Activator { get; }

        /// <summary>
        /// Using regex engine in {Activator}.
        /// </summary>
        bool ARegex { get; }

        /// <summary>
        /// Allows post-processing with MSBuild core.
        /// Some components may require immediate processing with evaluation before passing control to the next level.
        /// </summary>
        bool PostProcessingMSBuild { get; set; }

        /// <summary>
        /// Will be located before deepening if true.
        /// </summary>
        bool BeforeDeepening { get; }

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        bool PostParse { get; }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        string Eval(string data);
    }
}
