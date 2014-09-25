#region licence
// The MIT License (MIT)
// 
// Filename: TraceGenericLogger.cs
// Date Created: 2014/07/07
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
using System.Diagnostics;
using GenericServices.Logger;

namespace Tests.Helpers
{

    public class TraceGenericLogger : IGenericLogger
    {

        private readonly string _loggerName;

        /// <summary>
        /// Controls whether Verbose is written to Trace.
        /// Defaults to excluding Verbose
        /// </summary>
        public bool IncludeVerbose { get; set; }

        public TraceGenericLogger(string name)
        {
            _loggerName = name;
        }

        public void Verbose(object message)
        {
            if (IncludeVerbose)
                Trace.TraceInformation("{0}: {1}", _loggerName, message);
        }

        public void VerboseFormat(string format, params object[] args)
        {
            Verbose(string.Format(format, args));
        }

        public void Info(object message)
        {
            Trace.TraceInformation("{0}: {1}", _loggerName, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        public void Warn(object message)
        {
            Trace.TraceWarning("{0}: {1}", _loggerName, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }

        public void Error(object message)
        {
            Trace.TraceError("{0}: {1}", _loggerName, message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public void Critical(object message)
        {
            Trace.TraceError("{0}: {1}", _loggerName, message);
        }

        public void Critical(object message, Exception ex)
        {
            Trace.TraceError("{0}, expection {1}: {2}\n{3}", _loggerName, ex.GetType().Name, message, ex.StackTrace);
        }

        public void CriticalFormat(string format, params object[] args)
        {
            Critical(string.Format(format, args));
        }

    }
}
