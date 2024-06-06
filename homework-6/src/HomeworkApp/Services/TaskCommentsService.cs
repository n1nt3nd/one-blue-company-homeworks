using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using WorkshopApp.Proto.Client;

namespace HomeworkApp.Services;

public class TaskCommentsService : WorkshopApp.Proto.Client.TaskCommentsService.TaskCommentsServiceBase
{
    private readonly ITaskCommentService _taskCommentService;

    public TaskCommentsService(ITaskCommentService taskCommentService)
    {
        _taskCommentService = taskCommentService;
    }

    public override async Task<V1GetCommentsResponse> V1GetComments(V1GetCommentsRequest request, ServerCallContext context)
    {
        var comments = await _taskCommentService.GetComments(new GetCommentModel()
        {
            TaskId = request.TaskId,
            TakeDeleted = request.TakeDeleted
        }, context.CancellationToken);

        return new V1GetCommentsResponse()
        {
            Comments =
            {
                comments.Select(x => new Comment()
                    {
                        TaskId = x.TaskId,
                        Comment_ = x.Comment,
                        CreatedAt = x.At.ToTimestamp(),
                        IsDeleted = x.IsDeleted,
                    }
                ).ToArray()
            }
        };
    }

    public override async Task<V1AddCommentsResponse> V1AddComment(V1AddCommentsRequest request, ServerCallContext context)
    {
        var id = await _taskCommentService.AddComment(new AddCommentModel()
        {
            TaskId = request.TaskId,
            Message = request.Message,
            AuthorUserId = request.AuthorUserId
        }, context.CancellationToken);

        return new V1AddCommentsResponse()
        {
            CommentId = id,
        };
    }

    public override async Task<Empty> V1SetDeleted(V1SetDeletedRequest request, ServerCallContext context)
    {
        await _taskCommentService.SetDeleted(request.TaskCommentId, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> V1Update(V1UpdateRequest request, ServerCallContext context)
    {
        await _taskCommentService.Update(new UpdateCommentModel()
        {
            Id = request.CommentId,
            TaskId = request.TaskId,
            AuthorUserId = request.AuthorUserId,
            Message = request.Message,
        }, context.CancellationToken);

        return new Empty();
    }
}