using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLog.Web.Models
{
    public class QueryLog
    {
        public CloudTable Table { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Text { get; set; }

        public async Task<IEnumerable<AzureLogEntity>> ExecuteAsync()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixFromTime = Convert.ToInt64((From - epoch).TotalSeconds);
            var unixToTime = Convert.ToInt64((To - epoch).TotalSeconds);

            var results = await Task.WhenAll(
                                    Enumerable.Range(0, 5)
                                    .Select(i =>
                                        Table.ExecuteQuerySegmentedAsync<AzureLogEntity>(
                                            CreateTableQuery(i, unixFromTime, unixToTime), null)));
            var list = results.SelectMany(q => q.Results);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                var reg = new Regex(Text);
                list = list.Where(l => reg.IsMatch(l.Message));
            }
            return list.ToList().OrderBy(l => l.LogTimeStamp).ToList();
        }

        private static TableQuery<AzureLogEntity> CreateTableQuery(int partition, long unixFromTime, long unixToTime)
        {
            Func<int, long, string> partitionKeyFormat = (p, t) => string.Format(CultureInfo.InvariantCulture, "{0}-{1}", p, t);

            return new TableQuery<AzureLogEntity>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.GreaterThanOrEqual,
                    partitionKeyFormat(partition, unixFromTime)),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.LessThanOrEqual,
                    partitionKeyFormat(partition, unixToTime))));
        }
    }
}