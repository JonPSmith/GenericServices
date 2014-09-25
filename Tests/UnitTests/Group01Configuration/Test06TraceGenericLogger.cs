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

            ServicesConfiguration.SetLoggerMethod = name => new TraceGenericLogger(name);
            _loggerA = ServicesConfiguration.GetLogger("A");
            _loggerB = ServicesConfiguration.GetLogger("B");
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
