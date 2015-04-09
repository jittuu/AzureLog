using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLog.Web.Models
{
    public class AzureLogEntity : TableEntity
    {
        public DateTime LogTimestamp { get; set; }

        public string Level { get; set; }

        public string LoggerName { get; set; }

        public string Message { get; set; }

        public string MessageWithLayout { get; set; }

        public string MachineName { get; set; }
    }
}