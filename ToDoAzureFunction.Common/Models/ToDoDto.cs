namespace ToDoAzureFunction.Common.Models
{
    public class ToDoDto
    {
        public DateTime CreatedTime { get; set; }
        public string? TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }
}
