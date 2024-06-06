namespace HomeworkApp.Bll.Models;

public record GetCommentModel
{
    public required long TaskId { get; init; }
    public bool TakeDeleted { get; init; } = false;
}