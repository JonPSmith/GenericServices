#region licence
// The MIT License (MIT)
// 
// Filename: Test06Log4NetGenericLogger.cs
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
using GenericLibsBase;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group00GenericLibsBase
{
    class Test06Log4NetGenericLogger
    {
        private MemoryAppender _log4NetMemoryLog;

        private IGenericLogger _loggerA;
        private IGenericLogger _loggerB;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _log4NetMemoryLog = new MemoryAppender();
            BasicConfigurator.Configure(_log4NetMemoryLog);
            GenericLibsBaseConfig.SetLoggerMethod = name => new Log4NetGenericLogger(name);
            _loggerA = GenericLibsBaseConfig.GetLogger("A");
            _loggerB = GenericLibsBaseConfig.GetLogger("B");
        }

        [SetUp]
        public void Setup()
        {
            _log4NetMemoryLog.Clear();
        }

        [Test]
        public void Check01LoggerAOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerA.Verbose("a message");

            //VERIFY
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Length.ShouldEqual(1);
            logs[0].LoggerName.ShouldEqual("A");
            logs[0].RenderedMessage.ShouldEqual("a message");
        }

        [Test]
        public void Check02LoggerBOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerB.Verbose("a message");

            //VERIFY
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Length.ShouldEqual(1);
            logs[0].LoggerName.ShouldEqual("B");
            logs[0].RenderedMessage.ShouldEqual("a message");
        }

        [Test]
        public void Check03LoggerAandBOk()
        {

            //SETUP  

            //ATTEMPT
            _loggerA.Verbose("message 1");
            _loggerB.Verbose("message 2");

            //VERIFY
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Length.ShouldEqual(2);
            logs[0].LoggerName.ShouldEqual("A");
            logs[0].RenderedMessage.ShouldEqual("message 1");
            logs[1].LoggerName.ShouldEqual("B");
            logs[1].RenderedMessage.ShouldEqual("message 2");
        }

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
            var logs = _log4NetMemoryLog.GetEvents();
            logs.Length.ShouldEqual(1);
            logs[0].LoggerName.ShouldEqual("A");
            logs[0].RenderedMessage.ShouldEqual("an exception");
            logs[0].ExceptionObject.Message.ShouldEqual("This is the exception message.");
            logs[0].ExceptionObject.IsA<InvalidOperationException>();
        }

    }
}
