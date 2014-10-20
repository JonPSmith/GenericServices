#region licence
// The MIT License (MIT)
// 
// Filename: Test06TraceGenericLogger.cs
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
using System.IO;
using System.Linq;
using System.Text;
using GenericServices;
using GenericServices.Logger;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group01Configuration
{
    class Test06TraceGenericLogger
    {
        private readonly StringBuilder _loggedData = new StringBuilder();

        private IGenericLogger _loggerA;
        private IGenericLogger _loggerB;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            var memoryWriter = new StringWriter(_loggedData);
            Trace.Listeners.Add(new TextWriterTraceListener(memoryWriter));

            GenericServicesConfig.SetLoggerMethod = name => new TraceGenericLogger(name);
            _loggerA = GenericServicesConfig.GetLogger("A");
            _loggerB = GenericServicesConfig.GetLogger("B");
        }

        [SetUp]
        public void Setup()
        {
            _loggedData.Clear();
        }

        [Test]
        public void Check01LoggerAOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerA.Info("a message");

            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where( x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(1);
            logs[0].ShouldEndWith("A: a message");
        }

        [Test]
        public void Check02LoggerBOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerB.Info("a message");

            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where(x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(1);
            logs[0].ShouldEndWith("B: a message");
        }

        [Test]
        public void Check03LoggerAandBOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerA.Info("message 1");
            _loggerB.Info("message 2");

            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where(x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(2);
            logs[0].ShouldEndWith("A: message 1");
            logs[1].ShouldEndWith("B: message 2");
        }

        //----------------------------------------------------------

        [Test]
        public void Check05LoggerVerboseNotIncludedOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerA.Verbose("a message");

            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where(x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(0);
        }

        [Test]
        public void Check06LoggerVerboseIncludedOk()
        {

            //SETUP  
            ((TraceGenericLogger)_loggerA).IncludeVerbose = true;

            //ATTEMPT
            _loggerA.Verbose("a message");

            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where(x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(1);
            logs[0].ShouldEndWith("A: a message");
        }


        //-----------------------------------------------------------

        [Test]
        public void Check10LogExceptionOk()
        {

            //SETUP  

            //ATTEMPT
            try
            {
                throw new InvalidOperationException("This is the exception message.");
            }
            catch (Exception ex)
            {
                _loggerA.Critical("an exception", ex);
            }


            //VERIFY
            Trace.Flush();
            var logs = _loggedData.ToString().Split('\n').Where(x => x != string.Empty).Select(x => x.Trim()).ToArray();
            logs.Length.ShouldEqual(2);
            logs[0].ShouldEndWith("A, expection InvalidOperationException: an exception");
        }

    }
}
