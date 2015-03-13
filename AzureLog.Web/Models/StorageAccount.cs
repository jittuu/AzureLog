using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzureLog.Web.Models
{
    public class StorageAccount
    {
        public int Id { get; set; }

        [StringLength(256)]
        public string UserEmail { get; set; }

        [StringLength(256)]
        [Display(Name="Account Name")]
        public string AccountName { get; set; }

        [UIHint("Key")]
        [StringLength(1024)]
        public string Key { get; set; }
    }
}