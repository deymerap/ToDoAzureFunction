using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ToDoAzureFunction.Function.Entities;

namespace ToDoAzureFunction.Function.Func
{
    public class ScheduleFunc
    {
        [FunctionName("ScheduleFunc")]
        public static async Task Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer,
            [Table("ToDoTable", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"Delete completed function executed at: {DateTime.Now}");

            string vStrFilter = TableQuery.GenerateFilterConditionForBool("IsCompleted",QueryComparisons.Equal,true);
            TableQuery<ToDoEntity> query = new TableQuery<ToDoEntity>().Where(vStrFilter);
            TableQuerySegment<ToDoEntity> vLstToDoEntity = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            if (vLstToDoEntity != null)
            {
                int vIntContador = 0;
                foreach (ToDoEntity vObToDoEntity in vLstToDoEntity)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(vObToDoEntity));
                    vIntContador++;
                }
                log.LogInformation($"Delete: {vIntContador} item at: {DateTime.Now}");
            }

        }
    }
}
