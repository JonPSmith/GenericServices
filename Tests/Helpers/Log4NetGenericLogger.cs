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
