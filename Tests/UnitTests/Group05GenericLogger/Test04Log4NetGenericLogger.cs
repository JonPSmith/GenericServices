using System;
using GenericServices;
using GenericServices.Logger;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests.Group05GenericLogger
{
    class Test04Log4NetGenericLogger
    {
        private MemoryAppender _log4NetMemoryLog;

        private IGenericLogger _loggerA;
        private IGenericLogger _loggerB;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _log4NetMemoryLog = new MemoryAppender();
            BasicConfigurator.Configure(_log4NetMemoryLog);
            GenericLoggerFactory.SetLoggerMethod = name => new Log4NetGenericLogger(name);
            _loggerA = GenericLoggerFactory.GetLogger("A");
            _loggerB = GenericLoggerFactory.GetLogger("B");
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
