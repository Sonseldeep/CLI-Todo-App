using TodoApp.Application.DTOs;
using CoreTodoTask = TodoApp.Core.Entities.TodoTask;

namespace TodoApp.Application.Common;

public static class TodoTaskMapper
{
    public static TodoTaskDto ToDto(CoreTodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TodoTaskDto
        {
            Id = task.Id,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            StatusSymbol = task.GetStatusSymbol(),
            StatusText = task.GetStatusText(),
            FormattedCreatedDate = task.GetFormattedCreatedDate(),
            FormattedCompletedDate = task.GetFormattedCompletedDate()
        };
    }

    public static List<TodoTaskDto> ToDto(List<CoreTodoTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        return tasks.Select(ToDto).ToList();
    }
}
