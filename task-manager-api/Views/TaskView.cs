namespace TaskAPI.Views;
public enum Priority
{
    Low,
    Medium,
    High
}

public enum Status
{
    Pending,
    InProgress,
    Completed,
    Archived
}

public class TaskView
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime DueDate { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
}
