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
            [Table("ToDoTable", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new ToDo");
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


        [FunctionName(nameof(UpdateToDo))]
        public static async Task<IActionResult> UpdateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "ToDo/{id}")] HttpRequest pvObReq,
            [Table("ToDoTable", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for ToDo:{id}, recieved");

            string requestBody = await new StreamReader(pvObReq.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ToDoDto>(requestBody);

            //Validate ToDo id
            TableOperation vObToUpdateOperation = TableOperation.Retrieve<ToDoEntity>("ToDoTable", id);
            TableResult tableResult = await todoTable.ExecuteAsync(vObToUpdateOperation);
            if (tableResult == null)
            {
                return new BadRequestObjectResult(new Response()
                {
                    isSuccess = false,
                    Message = "ToDo not found.",
                });
            }

            if (String.IsNullOrEmpty(data?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response()
                {
                    isSuccess = false,
                    Message = "The request must have a TaskDescription.",
                });
            }

            //Update ToDo
            ToDoEntity vObToDoEntity = (ToDoEntity)tableResult.Result;
            vObToDoEntity.IsCompleted = data.IsCompleted; //Act state 
            vObToDoEntity.TaskDescription = data.TaskDescription;

            TableOperation vObAddOperation = TableOperation.Replace(vObToDoEntity);
            await todoTable.ExecuteAsync(vObAddOperation);

            string message = $"ToDo: {id}, update in Table";
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
