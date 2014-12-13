using AzureLog.Web.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AzureLog.Web.Controllers
{
    public class HomeController : AsyncController
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public ActionResult Index()
        {
            // _logger.Info("Home is called!");
            return View(Enumerable.Empty<AzureLogEntity>());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string accountName, string key, string tableName, DateTime from, DateTime to, string regex)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(key) ?
                CloudStorageAccount.DevelopmentStorageAccount :
                new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, key), true);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixFromTime = Convert.ToInt64((from - epoch).TotalSeconds);
            var unixToTime = Convert.ToInt64((to - epoch).TotalSeconds);

            var t0 = ExecuteQueryWithSegmentMode(table, CreateTableQuery(0, unixFromTime, unixToTime));
            var t1 = ExecuteQueryWithSegmentMode(table, CreateTableQuery(1, unixFromTime, unixToTime));
            var t2 = ExecuteQueryWithSegmentMode(table, CreateTableQuery(2, unixFromTime, unixToTime));
            var t3 = ExecuteQueryWithSegmentMode(table, CreateTableQuery(3, unixFromTime, unixToTime));
            var t4 = ExecuteQueryWithSegmentMode(table, CreateTableQuery(4, unixFromTime, unixToTime));

            await Task.WhenAll(t0, t1, t2, t3, t4);

            List<AzureLogEntity> result = new List<AzureLogEntity>();

            result.AddRange(t0.Result);
            result.AddRange(t1.Result);
            result.AddRange(t2.Result);
            result.AddRange(t3.Result);
            result.AddRange(t4.Result);

            Regex reg = new Regex(regex);

            ViewBag.AccountName = accountName;
            ViewBag.Key = key;
            ViewBag.TableName = tableName;
            ViewBag.From = from.ToString("yyyy-MM-dd");
            ViewBag.To = to.ToString("yyyy-MM-dd");
            ViewBag.Regex = regex;

            return View(result
                .Where(r => reg.Matches(r.Message).Count > 0)
                .OrderBy(r => r.Timestamp));
        }

        private static string PartionKeyFormat(int part, long unixTime)
        {
            return string.Format("{0}-{1}", part, unixTime);
        }

        private static TableQuery<AzureLogEntity> CreateTableQuery(int partition, long unixFromTime, long unixToTime)
        {
            Func<int, long, string> partitionKeyFormat = (p, t) => string.Format("{0}-{1}", p, t);

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

        private async Task<IEnumerable<AzureLogEntity>> ExecuteQueryWithSegmentMode(CloudTable table, TableQuery<AzureLogEntity> query)
        {
            TableQuerySegment<AzureLogEntity> querySegment = null;
            List<AzureLogEntity> result = new List<AzureLogEntity>();

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await table.ExecuteQuerySegmentedAsync(
                    query,
                    querySegment != null ? querySegment.ContinuationToken : null);

                result.AddRange(querySegment);
            }

            return result;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}