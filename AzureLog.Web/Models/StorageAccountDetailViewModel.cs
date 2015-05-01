using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureLog.Web.Models
{
    public class StorageAccountDetailViewModel
    {
        public int Id { get; set; }

        public string Account { get; set; }

        [UIHint("Key")]
        public string Key { get; set; }

        public IEnumerable<string> Tables { get; set; }
    }
}