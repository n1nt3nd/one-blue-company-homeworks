using DalSubTaskModel = HomeworkApp.Dal.Models.SubTaskModel;
using BllSubTaskModel = HomeworkApp.Bll.Models.GetSubTaskModel;

namespace HomeworkApp.Bll.Extensions;

public static class GetSubTaskModelExtensions
{
    public static BllSubTaskModel ToBll(this DalSubTaskModel src)
    {
        return new BllSubTaskModel()
        {
            TaskId = src.TaskId,
            Title = src.Title,
            Status = src.Status.ToBll(),
            ParentTaskIds = src.ParentTaskIds,
        };
    }
}