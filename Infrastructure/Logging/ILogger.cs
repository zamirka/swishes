namespace swishes.Infrastructure.Logging
{
    using System;

    public interface ILogger
    {
        void Info(string message);
        void ErrorException(string message, Exception ex);
        void Error(string message);
        void Warn(string message);
        void WarnException(string message, Exception ex);
    }
}