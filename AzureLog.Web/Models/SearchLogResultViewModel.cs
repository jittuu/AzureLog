using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLog.Web.Models
{
    public class SearchLogResultViewModel
    {
        public int Id { get; set; }

        public string Table { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Regex { get; set; }

        public IEnumerable<AzureLogEntity> Results { get; set; }
    }
}