using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLog.Web
{
    public static class CloudTableClientExtensions
    {
        public static async Task<IEnumerable<CloudTable>> ListTablesAsync(this CloudTableClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            TableContinuationToken token = null;
            var tables = new List<CloudTable>();
            do
            {
                var seg = await client.ListTablesSegmentedAsync(token);
                tables.AddRange(seg.Results);
                token = seg.ContinuationToken;
            } while (token != null);

            return tables;
        }
    }
}