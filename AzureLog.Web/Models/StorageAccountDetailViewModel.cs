using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLog.Web.Models
{
    public class StorageAccountDetailViewModel
    {
        public int Id { get; set; }

        public string Account { get; set; }

        public string Key { get; set; }

        public IEnumerable<string> Tables { get; set; }
    }
}