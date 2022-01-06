using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace QnaStatisticsHTTPtrigger
{


    public static class QnaStatisticsHTTPtrigger
    {

        public static CloudTable GetTable(string connectionString, string tableName)
        {
            CloudStorageAccount cloudStorageAccount = null;

            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch
            {
                throw;
            }

            CloudTableClient cloudTableClient = cloudStorageAccount?.CreateCloudTableClient();
            return cloudTableClient?.GetTableReference(tableName);
        }


        public static async Task<IList<T>> ExecuteTableQueryAsync<T>(
            this CloudTable cloudTable, TableQuery<T> tableQuery,
            CancellationToken cancellationToken = default(CancellationToken),
            Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken tableContinuationToken = null;

            do
            {
                TableQuerySegment<T> tableQuerySegment = null;

                try
                {
                    tableQuerySegment = await cloudTable.ExecuteQuerySegmentedAsync<T>(tableQuery, tableContinuationToken);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to execute a table query: {e.Message}");
                    return items;
                }

                tableContinuationToken = tableQuerySegment.ContinuationToken;
                items.AddRange(tableQuerySegment);
                onProgress?.Invoke(items);
            }
            while (tableContinuationToken != null && !cancellationToken.IsCancellationRequested);

            return items;
        }



        public class RoutingDataEntity
        {
            public string Body { get; set; }
        }



        private static Task<IList<RoutingDataEntity>> GetAllEntitiesFromTable(CloudTable table)
        {
            var query = new TableQuery<RoutingDataEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey", QueryComparisons.Equal, "question"));

            return  table.ExecuteTableQueryAsync(query);
        }





        [FunctionName("QnaStatisticsHTTPtrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            public static CloudTable _qnaStatisticsTable;
            public const string TableNameQnaStaistics = "QnAStatistics";
            _qnaStatisticsTable = GetTable(connectionString, TableNameQnaStaistics);


                var entities = GetAllEntitiesFromTable(_qnaStatisticsTable).Result;
                var qnaStatistics = new List<QnaStatistics>();

                foreach (RoutingDataEntity entity in entities)
                {
                    var qnaStatistics =
                        JsonConvert.DeserializeObject<QnaStatistics>(entity.Body);
                    qnaStatistics.Add(qnaStatistics);
                }
       


            var json = JsonConvert.SerializeObject(qnaStatistics);
            return new OkObjectResult(json);
 
        }

    }
}