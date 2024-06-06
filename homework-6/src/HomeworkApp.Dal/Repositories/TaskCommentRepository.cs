using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using HomeworkApp.Utils.Helpers;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{

    private readonly IDateTimeProvider _dateTimeProvider;

    public TaskCommentRepository(
        IOptions<DalOptions> dalSettings, IDateTimeProvider dateTimeProvider) : base(dalSettings.Value)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        var sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at)
values (@TaskId, @AuthorUserId, @Message, @At)
returning id
";

        await using var connection = await GetConnection();
        
        var ids = await connection.QuerySingleAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AuthorUserId = model.AuthorUserId,
                    Message = model.Message,
                    At = model.At,
                },
                cancellationToken: token));

        return ids;
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        var sqlQuery = @"
update task_comments
set task_id        = @TaskId,
    author_user_id = @AuthorUserId,
    message        = @Message,
    modified_at    = @ModifiedAt
where id = @TaskCommentId
";

        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskCommentId = model.Id,
                    TaskId = model.TaskId,
                    AuthorUserId = model.AuthorUserId,
                    Message = model.Message,
                    ModifiedAt = model.ModifiedAt
                },
                cancellationToken: token));
    }

    public async Task SetDeleted(long taskCommentId, CancellationToken token)
    {
        var sqlQuery = @"
update task_comments
set deleted_at = @DeletedAt
where id = @TaskCommentId
";

        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskCommentId = taskCommentId,
                    DeletedAt = _dateTimeProvider.GetUtcNow(),
                },
                cancellationToken: token));
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var baseSql = @"
select id, task_id, author_user_id, message, at, modified_at, deleted_at
from task_comments
where task_id = @TaskId
";

        if (!model.TakeDeleted)
            baseSql += " and deleted_at is null";

        baseSql += " order by at desc";

        var cmd = new CommandDefinition(
            baseSql,
            new
            {
                TaskId = model.TaskId
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();

        return (await connection.QueryAsync<TaskCommentEntityV1>(cmd))
            .ToArray();
    }
}