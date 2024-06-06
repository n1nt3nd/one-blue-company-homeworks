namespace HomeworkApp.Bll.Models;

public record AddCommentModel
{
    public required long TaskId { get; init; }
    public required long AuthorUserId { get; init; }
    public required string Message { get; init; }
}