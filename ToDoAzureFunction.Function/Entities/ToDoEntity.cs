using Microsoft.Azure.Cosmos.Table;
//using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ToDoAzureFunction.Function.Entities
{
    public class ToDoEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
