/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2016-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) LSender contributors: https://github.com/3F/LSender/graphs/contributors
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

namespace net.r_eg.Components
{
    /// <summary>
    /// Ascetic aggregative repeater for loggers etc.
    /// </summary>
    public sealed class LSender: ISender
    {
        private static readonly Lazy<ISender> _lazy = new Lazy<ISender>(() => new LSender());

        /// <summary>
        /// Thread-safe getting the instance of the LSender class.
        /// </summary>
        public static ISender _ => _lazy.Value;

        /// <summary>
        /// When message is raised.
        /// </summary>
        public event EventHandler<Message> Raised;

        /// <summary>
        /// When the message was sent. Static alias.
        /// </summary>
        public static event EventHandler<Message> Sent
        {
            add => _.Raised += value;
            remove => _.Raised -= value;
        }

        /// <summary>
        /// Static alias for raising new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public static void Send(object sender, Message msg) => _.Raise(sender, msg);

        /// <summary>
        /// Static alias for raising new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public static void Send(object sender, string msg) => _.Raise(sender, msg);

        /// <summary>
        /// Static alias for raising new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public static void Send(object sender, string msg, MsgLevel level)
            => _.Raise(sender, msg, level);

        /// <summary>
        /// To send new message with default sender as typeof(T).
        /// Useful for static methods etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        public static void Send<T>(Message msg) => _.Raise<T>(msg);

        /// <summary>
        /// To send new message with default sender as typeof(T).
        /// Useful for static methods etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        public static void Send<T>(string msg) => _.Raise<T>(msg);

        /// <summary>
        /// To send new message with default sender as typeof(T).
        /// Useful for static methods etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public static void Send<T>(string msg, MsgLevel level) => _.Raise<T>(msg, level);

        /// <summary>
        /// Resets listeners. Static alias to revoke.
        /// </summary>
        public static void Reset() => _.Revoke();

        /// <summary>
        /// Raises new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public void Raise(object sender, Message msg) => Raised(sender ?? this, msg);

        /// <summary>
        /// Raises new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public void Raise(object sender, string msg)
            => Raise(sender, msg, MsgLevel.Debug);

        /// <summary>
        /// Raises new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void Raise(object sender, string msg, MsgLevel level)
            => Raise(sender, new Message(msg, level));

        /// <summary>
        /// Raises new message with default sender as typeof(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        public void Raise<T>(Message msg) => Raise(typeof(T), msg);

        /// <summary>
        /// Raises new message with default sender as typeof(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        public void Raise<T>(string msg) => Raise(typeof(T), msg);

        /// <summary>
        /// Raises new message with default sender as typeof(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void Raise<T>(string msg, MsgLevel level) => Raise(typeof(T), msg, level);

        /// <summary>
        /// Revokes subscription for all listeners.
        /// </summary>
        public void Revoke() => Raised = delegate (object sender, Message msg) { };

        private LSender() => Revoke();
    }
}
