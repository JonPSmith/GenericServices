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
