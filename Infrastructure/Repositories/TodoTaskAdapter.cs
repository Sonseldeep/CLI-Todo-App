using CoreTodoTask = TodoApp.Core.Entities.TodoTask;
using OldTodoTask = TodoApp.TodoTask;

namespace TodoApp.Infrastructure.Repositories;

public class TodoTaskAdapter
{
    public static CoreTodoTask FromOldTask(OldTodoTask oldTask)
    {
        ArgumentNullException.ThrowIfNull(oldTask);

        return new CoreTodoTask(oldTask.Id, oldTask.Description, oldTask.IsCompleted, oldTask.CreatedAt);
    }

    public static OldTodoTask ToOldTask(CoreTodoTask newTask)
    {
        ArgumentNullException.ThrowIfNull(newTask);

        return new OldTodoTask
        {
            Id = newTask.Id,
            Description = newTask.Description,
            IsCompleted = newTask.IsCompleted,
            CreatedAt = newTask.CreatedAt
        };
    }

    public static List<CoreTodoTask> FromOldTasks(List<OldTodoTask> oldTasks)
    {
        ArgumentNullException.ThrowIfNull(oldTasks);

        return oldTasks.Select(FromOldTask).ToList();
    }

    public static List<OldTodoTask> ToOldTasks(List<CoreTodoTask> newTasks)
    {
        ArgumentNullException.ThrowIfNull(newTasks);

        return newTasks.Select(ToOldTask).ToList();
    }
}
