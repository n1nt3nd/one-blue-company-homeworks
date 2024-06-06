using BllTaskStatus = HomeworkApp.Bll.Enums.TaskStatus;
using DalTaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.Bll.Extensions;

public static class TaskStatusExtensions
{
    public static DalTaskStatus ToDal(this BllTaskStatus src)
    {
        return src switch
        {
            BllTaskStatus.Draft => DalTaskStatus.Draft,
            BllTaskStatus.ToDo => DalTaskStatus.ToDo,
            BllTaskStatus.InProgress => DalTaskStatus.InProgress,
            BllTaskStatus.Done => DalTaskStatus.Done,
            BllTaskStatus.Canceled => DalTaskStatus.Canceled,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };
    }
    
    public static BllTaskStatus ToBll(this DalTaskStatus src)
    {
        return src switch
        {
            DalTaskStatus.Draft => BllTaskStatus.Draft,
            DalTaskStatus.ToDo => BllTaskStatus.ToDo,
            DalTaskStatus.InProgress => BllTaskStatus.InProgress,
            DalTaskStatus.Done => BllTaskStatus.Done,
            DalTaskStatus.Canceled => BllTaskStatus.Canceled,
            _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
        };
    }
}