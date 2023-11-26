/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Text;
using net.r_eg.SobaScript.Z.Ext;

namespace net.r_eg.vsSBE
{
    internal sealed class EncDetector: IEncDetector
    {
        /// <summary>
        /// Detects encoding for specified stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="confidence">Detected confidence.</param>
        /// <returns>null if can't be detected.</returns>
        public Encoding Detect(Stream stream, out float confidence)
        {
            confidence = 0;

            if(stream == null) {
                return null;
            }

            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(stream);
            cdet.DataEnd();

            if(cdet.Charset == null) {
                return null;
            }

            confidence = cdet.Confidence;

            Log.Debug($"Detected charset '{cdet.Charset}' confidence: '{cdet.Confidence}'");
            return Encoding.GetEncoding(cdet.Charset);
        }

        /// <summary>
        /// Detects encoding for specified stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>null if can't be detected.</returns>
        public Encoding Detect(Stream stream)
            => Detect(stream, out float confidence);

        /// <summary>
        /// Try to fix the wrong encoded string.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <param name="container">Known information about bytes.</param>
        /// <param name="confidence">To limit accepted confidence.</param>
        /// <returns>Returns null if detected confidence less than input limit. Otherwise, re-encoded string.</returns>
        public string FixEncoding(string input, Encoding container, float confidence = 0.92f)
        {
            if(string.IsNullOrWhiteSpace(input)) {
                return input;
            }

            if(container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            byte[] bytes = container.GetBytes(input);

            var cdet = new Ude.CharsetDetector();
            cdet.Feed(bytes, 0, bytes.Length);
            cdet.DataEnd();

            if(cdet.Charset == null) {
                return null;
            }

            Log.Debug($"{nameof(FixEncoding)}: charset '{cdet.Charset}' confidence: '{cdet.Confidence}'");

            if(cdet.Confidence < confidence) {
                Log.Debug($"{nameof(FixEncoding)}: Confidence < {confidence}");
                return null;
            }

            Encoding to = Encoding.GetEncoding(cdet.Charset);

            Log.Debug($"ReEncodeString: '{container.EncodingName}' -> '{to.EncodingName}'");
            Log.Trace($"ReEncodeString: original - '{input}'");
            return to.GetString(bytes);
        }
    }
}
