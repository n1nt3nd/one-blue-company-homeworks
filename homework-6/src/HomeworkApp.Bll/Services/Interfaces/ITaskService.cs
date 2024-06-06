using HomeworkApp.Bll.Models;
using HomeworkApp.Dal.Models;
using TaskStatus = HomeworkApp.Bll.Enums.TaskStatus;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface ITaskService
{
    Task<long> CreateTask(CreateTaskModel model, CancellationToken token);

    Task<GetTaskModel?> GetTask(long taskId, CancellationToken token);

    Task AssignTask(Bll.Models.AssignTaskModel model, CancellationToken token);
    
    Task<GetSubTaskModel[]> GetSubTasksInStatus(long parentTaskId, Bll.Enums.TaskStatus[] statuses, CancellationToken token);
}