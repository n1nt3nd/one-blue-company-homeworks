using HomeworkApp.Bll.Models;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface ITaskCommentService
{
    Task<TaskMessage[]> GetComments(GetCommentModel model, CancellationToken token);

    Task<long> AddComment(AddCommentModel model, CancellationToken token);

    Task SetDeleted(long taskCommentId, CancellationToken token);

    Task Update(UpdateCommentModel model, CancellationToken token);
}