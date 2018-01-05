using System;

namespace TZGCMS.Infrastructure.Logging
{
    public interface ILoggingService
    {
        void Info(string message);
        void Warn(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception x);
        void Error(string message, Exception x);
        void Fatal(string message);
        void Fatal(Exception x);
    }
}