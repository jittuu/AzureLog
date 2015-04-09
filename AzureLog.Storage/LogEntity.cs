using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLog.Storage
{
    public class LogEntity : TableEntity
    {
        public LogEntity(string loggerName, string level, string message, string layoutMessage, DateTime logTimestamp)
        {
            if (logTimestamp.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("logTimestamp must be UTC.", "logTimestamp");
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixTime = Convert.ToInt64((logTimestamp - epoch).TotalSeconds);
            var prefix = Convert.ToInt32(unixTime % 5);
            PartitionKey =  string.Format("{0}-{1}", prefix, unixTime);
            RowKey = Convert.ToString(logTimestamp.Ticks);


            LogTimestamp = logTimestamp;
            LoggerName = loggerName;
            Level = level;
            Message = message;
            MessageWithLayout = layoutMessage;
            MachineName = Environment.MachineName;
        }

        public LogEntity()
        {

        }

        public DateTime LogTimestamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string MessageWithLayout { get; set; }
        public string MachineName { get; set; }

    }
}
