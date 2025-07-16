using CoreTodoTask = TodoApp.Core.Entities.TodoTask;

namespace TodoApp.Core.Interfaces;

public interface ITodoRepository
{
    Task<List<CoreTodoTask>> GetAllTasksAsync();
    Task<CoreTodoTask?> GetTaskByIdAsync(int id);
    Task<CoreTodoTask> AddTaskAsync(CoreTodoTask task);
    Task<CoreTodoTask> UpdateTaskAsync(CoreTodoTask task);
    Task<bool> DeleteTaskAsync(int id);
    Task<bool> TaskExistsAsync(int id);
}
