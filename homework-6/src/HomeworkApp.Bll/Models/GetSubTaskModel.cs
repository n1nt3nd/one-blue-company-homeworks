namespace HomeworkApp.Bll.Models;

public record GetSubTaskModel
{
    public required long TaskId { get; init; }
    public required string Title { get; init; }
    public required Bll.Enums.TaskStatus Status { get; init; }
    public required long[] ParentTaskIds { get; init; }
}