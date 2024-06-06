namespace HomeworkApp.Bll.Models;

public record UpdateCommentModel
{
    public required long Id { get; init; }
    public required long TaskId { get; init; }
    public required long AuthorUserId { get; init; }
    public required string Message { get; init; }
}