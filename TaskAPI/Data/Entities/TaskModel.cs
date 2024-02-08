using TaskAPI.Views;

namespace TaskAPI.Data.Entities;

public class TaskModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Status Status { get; set; }
    public DateTime DueDate { get; set; }
    public Priority Priority { get; set; }
    public string Description { get; set; }
}