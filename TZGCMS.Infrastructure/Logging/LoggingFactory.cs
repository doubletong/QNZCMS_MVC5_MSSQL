using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TZGCMS.Infrastructure.Logging
{
    public class LoggingFactory
    {
        private static ILoggingService _logger;

        public static void InitializeLogFactory(ILoggingService logger)
        {
            _logger = logger;
        }

        public static ILoggingService GetLogger()
        {
            return _logger;
        }
    }
}
