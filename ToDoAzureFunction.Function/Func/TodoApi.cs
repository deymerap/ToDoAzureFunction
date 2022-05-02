using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ToDoAzureFunction.Common.Responses;
using ToDoAzureFunction.Common.Models;
using ToDoAzureFunction.Function.Entities;
using Microsoft.Azure.Cosmos.Table;
//using Microsoft.WindowsAzure.Storage.Table;

namespace ToDoAzureFunction.Function.Func
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateToDo))]
        public static async Task<IActionResult> CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ToDo")] HttpRequest pvObReq,
            [Table("ToDo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,

            ILogger log)
        {
            log.LogInformation("Recieved a new ToDo");

            string name = pvObReq.Query["name"];

            string requestBody = await new StreamReader(pvObReq.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ToDoDto>(requestBody);

            if (String.IsNullOrEmpty(data?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response() {
                    isSuccess = false,
                    Message = "The request must have a TaskDescription.",
                });
            }
            var vObToDoEntity = new ToDoEntity() { 
                PartitionKey = "ToDoTable",
                RowKey = Guid.NewGuid().ToString(),
                ETag = "*",
                CreatedTime = DateTime.UtcNow,
                TaskDescription = data.TaskDescription,
                IsCompleted = false,
            };

            TableOperation vObAddOperation = TableOperation.Insert(vObToDoEntity);
            await todoTable.ExecuteAsync(vObAddOperation);

            string message = "New todo stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                Message = message,
                Result = vObToDoEntity
            });
        }


       

    }
}
