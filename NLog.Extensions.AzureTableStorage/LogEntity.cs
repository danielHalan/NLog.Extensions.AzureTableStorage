using System;
using System.Collections;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Extensions.AzureTableStorage
{
    public class LogEntity : TableEntity
    {
        private readonly object _syncRoot = new object();

        public LogEntity(string partitionKeyPrefix, string tenant, LogEventInfo logEvent, string layoutMessage, string timestampFormatString = "g")
        {
            lock (_syncRoot)
            {
                LoggerName = logEvent.LoggerName;
                Tenant = tenant;
                LogTimeStamp = logEvent.TimeStamp.ToString(timestampFormatString);
                Level = logEvent.Level.Name;
                Message = logEvent.FormattedMessage;
                MessageWithLayout = layoutMessage;
                if (logEvent.Exception != null)
                {
                    Exception = logEvent.Exception.ToString();
                    if (logEvent.Exception.Data.Count > 0)
                    {
                        ExceptionData = GetExceptionDataAsString(logEvent.Exception);
                    }
                    if (logEvent.Exception.InnerException != null)
                    {
                        InnerException = logEvent.Exception.InnerException.ToString();
                    }
                }
                if (logEvent.StackTrace != null)
                {
                    StackTrace = logEvent.StackTrace.ToString();
                }
                MachineName = Environment.MachineName;
                var prefix = tenant ?? partitionKeyPrefix;
                PartitionKey = string.Concat(prefix, MachineName, LoggerName);
                RowKey = ((DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks) + Guid.NewGuid().GetHashCode() ).ToString("d19");
            }

        }

        private static string GetExceptionDataAsString(Exception exception)
        {
            var data = new StringBuilder();
            foreach (DictionaryEntry entry in exception.Data)
            {
                data.AppendLine(entry.Key + "=" + entry.Value);
            }
            return data.ToString();
        }

        public LogEntity()
        {
        }

        public string LogTimeStamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public string MessageWithLayout { get; set; }
        public string ExceptionData { get; set; }
        public string MachineName { get; set; }
        public string Tenant { get; set; }

  }
}
