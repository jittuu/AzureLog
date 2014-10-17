using System.ComponentModel.DataAnnotations;
using AzureLog.Storage;
using NLog.Targets;

namespace AzureLog.NLogTarget
{
    public class AzureLogTarget : TargetWithLayout
    {
        LogTableClient _client;

        [Required]
        public string ConnectionStringKey { get; set; }

        [Required]
        public string TableName { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            _client = LogTableClient.Create(this.ConnectionStringKey, this.TableName);
        }

        protected override void Write(NLog.LogEventInfo logEvent)
        {
            if (_client != null)
            {
                var layoutMessage = Layout.Render(logEvent);
                var log = new LogEntity(
                    logEvent.LoggerName, logEvent.Level.Name, 
                    layoutMessage, logEvent.TimeStamp.ToUniversalTime());
                _client.Insert(log);
            }
        }
    }
}
