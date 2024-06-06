using System.Text.Json;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Utils.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Bll.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskCommentRepository _taskCommentRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly IDateTimeProvider _dateTimeProvider;


    public TaskCommentService(
        ITaskCommentRepository taskCommentRepository,
        IDistributedCache distributedCache,
        IDateTimeProvider dateTimeProvider)
    {
        _taskCommentRepository = taskCommentRepository;
        _distributedCache = distributedCache;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TaskMessage[]> GetComments(GetCommentModel model, CancellationToken token)
    {
        var cacheKey = $"cache_comments:{model.TaskId}";
        var cachedTask = await _distributedCache.GetStringAsync(cacheKey, token);

        if (!string.IsNullOrEmpty(cachedTask))
        {
            var cacheMessages = JsonSerializer.Deserialize<TaskMessage[]>(cachedTask);
            if (cacheMessages is not null)
                return cacheMessages;
        }

        var comments = await _taskCommentRepository.Get(
            new TaskCommentGetModel()
            {
                TaskId = model.TaskId,
                TakeDeleted = model.TakeDeleted,
            }, token);

        var messages = comments.Select(x => new TaskMessage()
        {
            TaskId = x.TaskId,
            Comment = x.Message,
            At = x.At,
            IsDeleted = (x.DeletedAt is null)
        }).ToArray();

        var messagesJson = JsonSerializer.Serialize(messages);

        await _distributedCache.SetStringAsync(
            cacheKey,
            messagesJson,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
            },
            token
        );

        return messages;
    }

    public async Task<long> AddComment(AddCommentModel model, CancellationToken token)
    {
        var commentId = await _taskCommentRepository.Add(new TaskCommentEntityV1()
        {
            TaskId = model.TaskId,
            AuthorUserId = model.AuthorUserId,
            Message = model.Message,
            At = _dateTimeProvider.GetUtcNow()
        }, token);

        return commentId;
    }

    public async Task SetDeleted(long taskCommentId, CancellationToken token)
    {
        await _taskCommentRepository.SetDeleted(taskCommentId, token);
    }

    public async Task Update(UpdateCommentModel model, CancellationToken token)
    {
        await _taskCommentRepository.Update(new TaskCommentEntityV1()
        {
            Id = model.Id,
            TaskId = model.TaskId,
            AuthorUserId = model.AuthorUserId,
            Message = model.Message,
        }, token);

        var cacheKey = $"cache_comments:{model.TaskId}";
        await _distributedCache.RemoveAsync(cacheKey, token);
    }
}