namespace ToDoAzureFunction.Common.Models
{
    public class ToDo
    {
        public DateTime CreatedTime { get; set; }
        public string? TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
