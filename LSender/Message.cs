﻿/*
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
    [Serializable]
    public sealed class Message: EventArgs
    {
        public readonly DateTime stamp;

        public readonly string content;

        public readonly Exception exception;

        public readonly object data;

        public readonly MsgLevel level;

        public Message(string msg, MsgLevel level = MsgLevel.Debug)
        {
            content     = msg;
            this.level  = level;
            stamp       = DateTime.Now;
        }

        public Message(string msg, Exception ex, MsgLevel type = MsgLevel.Error)
            : this(msg, type)
        {
            exception = ex;
        }

        public Message(string msg, object data, MsgLevel type = MsgLevel.Debug)
            : this(msg, type)
        {
            this.data = data;
        }
    }
}