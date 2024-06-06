namespace HomeworkApp.Dal.Models;

public record TaskCommentGetModel
{
    public required long TaskId { get; init; }
    public bool TakeDeleted { get; init; } = false;
};