#region licence
// The MIT License (MIT)
// 
// Filename: GenericLibsBaseConfig.cs
// Date Created: 2014/10/28
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;

namespace GenericLibsBase
{
    /// <summary>
    /// This static class holds the GenericLibsBase configuration
    /// </summary>
    public static class GenericLibsBaseConfig
    {
        /// <summary>
        /// This is a constant noLogger. Used when no logging is needed.
        /// </summary>
        public static readonly IGenericLogger NoLoggerInstance = new NoLoggingGenericLogger();
           
        /// <summary>
        /// This should be given a method that takes a string, to have the logger, and returns an IGenericLogger instance. 
        /// </summary>
        public static Func<string, IGenericLogger> SetLoggerMethod { private get; set; }

        /// <summary>
        /// This returns the logger set in the Generic Service package
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IGenericLogger GetLogger( string name)
        {
            return SetLoggerMethod(name);
        }

        static GenericLibsBaseConfig()
        {
            SetLoggerMethod = name => NoLoggerInstance;
        }
    }
}
