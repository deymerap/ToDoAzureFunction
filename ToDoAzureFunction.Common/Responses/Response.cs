using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoAzureFunction.Common.Responses
{
    public class Response
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
