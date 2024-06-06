using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.Dal.Repositories;

public class TaskRepository : PgRepository, ITaskRepository
{
    public TaskRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(TaskEntityV1[] tasks, CancellationToken token)
    {
        const string sqlQuery = @"
insert into tasks (parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at) 
select parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at
  from UNNEST(@Tasks)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Tasks = tasks
                },
                cancellationToken: token));

        return ids
            .ToArray();
    }

    public async Task<TaskEntityV1[]> Get(TaskGetModel query, CancellationToken token)
    {
        var baseSql = @"
select id
     , parent_task_id
     , number
     , title
     , description
     , status
     , created_at
     , created_by_user_id
     , assigned_to_user_id
     , completed_at
  from tasks
";

        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (query.TaskIds.Any())
        {
            conditions.Add($"id = ANY(@TaskIds)");
            @params.Add($"TaskIds", query.TaskIds);
        }

        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
                commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);


        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskEntityV1>(cmd))
            .ToArray();
    }

    public async Task Assign(AssignTaskModel model, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set assigned_to_user_id = @AssignToUserId
     , status = @Status
 where id = @TaskId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AssignToUserId = model.AssignToUserId,
                    Status = model.Status
                },
                cancellationToken: token));
    }

    public async Task<SubTaskModel[]> GetSubTasksInStatus(long parentTaskId, TaskStatus[] statuses, CancellationToken token)
    {
        var sqlQuery = @"
with recursive tasks_tree as (select t.id
                                   , t.title
                                   , t.status
                                   , array [t.id] as path
                              from tasks t
                              where t.parent_task_id = @ParentTaskId
                              union all
                              select t1.id
                                   , t1.title
                                   , t1.status
                                   , path || t1.id
                              from tasks t1
                                       join tasks_tree on tasks_tree.id = t1.parent_task_id)
select tasks_tree.id       as TaskId,
       tasks_tree.title    as Title,
       task_statuses.alias as Status,
       tasks_tree.path     as ParentTaskIds
from tasks_tree
         join task_statuses
              on tasks_tree.status = task_statuses.id
where task_statuses.alias = ANY (@TaskStatuses)
";

        var @params = new DynamicParameters();

        @params.Add("ParentTaskId", parentTaskId);
        @params.Add("TaskStatuses", statuses.Select(x => x.ToString()).ToArray());

        var cmd = new CommandDefinition(
            sqlQuery,
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();

        return (await connection.QueryAsync<SubTaskModel>(cmd))
            .ToArray();
    }
    
    public async Task SetParentTask(long taskId, long? parentTaskId, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set parent_task_id = @ParentTaskId
 where id = @TaskId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = taskId,
                    ParentTaskId = parentTaskId,
                },
                cancellationToken: token));
    }
}