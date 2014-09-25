#region licence
// The MIT License (MIT)
// 
// Filename: Log4NetGenericLogger.cs
// Date Created: 2014/07/03
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
using GenericServices.Logger;

namespace Tests.Helpers
{

    public class Log4NetGenericLogger : IGenericLogger
    {

        private readonly log4net.ILog _logger;

        public Log4NetGenericLogger(string name)
        {
            _logger = log4net.LogManager.GetLogger(name);
        }

        public void Verbose(object message)
        {
            _logger.Debug(message);
        }

        public void VerboseFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void Info(object message)
        {
            _logger.Info(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            _logger.Error(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void Critical(object message)
        {
            _logger.Fatal(message);
        }

        public void Critical(object message, Exception ex)
        {
            _logger.Fatal(message, ex);
        }

        public void CriticalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

    }
}
