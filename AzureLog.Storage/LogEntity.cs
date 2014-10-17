using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLog.Storage
{
    public class LogEntity : TableEntity
    {
        public LogEntity(string loggerName, string level, string message, DateTime logTimeStamp)
        {
            if (logTimeStamp.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("logTimeStamp must be UTC.", "logTimeStamp");
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixTime = Convert.ToInt64((logTimeStamp - epoch).TotalSeconds);
            var prefix = Convert.ToInt32(unixTime % 5);
            PartitionKey =  string.Format("{0}-{1}", prefix, unixTime);
            RowKey = Convert.ToString(logTimeStamp.Ticks);


            LogTimeStamp = logTimeStamp;
            LoggerName = loggerName;
            Level = level;
            Message = message;
        }

        public LogEntity()
        {

        }

        public DateTime LogTimeStamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
    }
}
