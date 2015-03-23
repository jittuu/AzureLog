using AzureLog.Storage;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets;

namespace AzureLog.NLogTarget
{
    [Target("AzureLog")] 
    public class AzureLogTarget : TargetWithLayout
    {
        LogTableClient _client;

        [RequiredParameter]
        public string ConnectionStringKey { get; set; }

        [RequiredParameter]
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
                    logEvent.FormattedMessage, layoutMessage,
                    logEvent.TimeStamp.ToUniversalTime());
                _client.Insert(log);
            }
        }
    }
}
