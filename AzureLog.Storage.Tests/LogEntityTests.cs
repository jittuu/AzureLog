using System;
using Xunit;

namespace AzureLog.Storage.Tests
{
    public class LogEntityTests
    {
        [Fact]
        public void Should_set_prefix_with_unix_timestamp_as_PartitionKey()
        {
            var logTime = new DateTime(2014, 10, 17, 15, 25, 1, DateTimeKind.Utc);
            var log = new LogEntity("Logger", "Info", "this is message", logTime);

            Assert.Equal(log.PartitionKey, "1-1413559501");
        }

        [Fact]
        public void Should_set_ticks_as_RowKey()
        {
            var logTime = new DateTime(20000002000L, DateTimeKind.Utc);
            var log = new LogEntity("Logger", "Info", "this is message", logTime);

            Assert.Equal(log.RowKey, "20000002000");
        }

        [Fact]
        public void Should_set_logger()
        {
            var logTime = new DateTime(2014, 10, 17, 15, 25, 0, DateTimeKind.Utc);
            var log = new LogEntity("Logger", "Info", "this is message", logTime);

            Assert.Equal(log.LoggerName, "Logger");
        }

        [Fact]
        public void Should_set_Level()
        {
            
            var logTime = new DateTime(2014, 10, 17, 15, 25, 0, DateTimeKind.Utc);
            var log = new LogEntity("Logger", "Info", "this is message", logTime);

            Assert.Equal(log.Level, "Info");
        }

        [Fact]
        public void Should_set_Message()
        {
            
            var logTime = new DateTime(2014, 10, 17, 15, 25, 0, DateTimeKind.Utc);
            var log = new LogEntity("Logger", "Info", "this is message", logTime);

            Assert.Equal(log.Message, "this is message");
        }
    }
}
