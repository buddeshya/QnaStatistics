using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Functions.Entity.Models;

namespace qnastatstrigger
{
    public static class qnaStatsRetreiver
    {
        const string TableNameQnaStaistics = "QnAStatistics";


        [FunctionName("qnaStatsRetreiver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string ConnectionString = Environment.GetEnvironmentVariable($"ConnectionStringTenant", EnvironmentVariableTarget.Process);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            CloudTable _qnaStatisticsTable;
            CloudStorageAccount cloudStorageAccount = null;

            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);
                CloudTableClient cloudTableClient = cloudStorageAccount?.CreateCloudTableClient();
                _qnaStatisticsTable = cloudTableClient?.GetTableReference(TableNameQnaStaistics);

                TableQuery<QnaStatisticsEntity> rangeQuery = new Microsoft.Azure.Cosmos.Table.TableQuery<QnaStatisticsEntity>();
                var entitiList = _qnaStatisticsTable.ExecuteQuery(rangeQuery);

                string responseMessage = JsonConvert.SerializeObject(entitiList);

                return new OkObjectResult(responseMessage);
            }
            catch(Exception e)
            {
                return new OkObjectResult(e);
            }

            
        }
    }
}
