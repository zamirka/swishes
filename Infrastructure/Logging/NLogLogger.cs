namespace swishes.Infrastructure.Logging
{
    using System;
    
    using NLog;
    
    public class NLogLogger : ILogger
    {
        private Logger _logger;

        public NLogLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void ErrorException(string message, Exception ex)
        {
            _logger.ErrorException(message, ex);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void WarnException(string message, Exception ex)
        {
            _logger.WarnException(message, ex);
        }

    }
}