using TodoApp.Core.Interfaces;
using CoreTodoTask = TodoApp.Core.Entities.TodoTask;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepositoryAdapter : ITodoRepository
{
    private readonly TodoApp.TodoRepository _oldRepository;

    public TodoRepositoryAdapter(TodoApp.TodoRepository oldRepository)
    {
        _oldRepository = oldRepository ?? throw new ArgumentNullException(nameof(oldRepository));
    }

    public async Task<List<CoreTodoTask>> GetAllTasksAsync()
    {
        var oldTasks = await _oldRepository.LoadTasksAsync();
        return TodoTaskAdapter.FromOldTasks(oldTasks);
    }

    public async Task<CoreTodoTask?> GetTaskByIdAsync(int id)
    {
        var allTasks = await GetAllTasksAsync();
        return allTasks.FirstOrDefault(t => t.Id == id);
    }

    public async Task<CoreTodoTask> AddTaskAsync(CoreTodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var oldTasks = await _oldRepository.LoadTasksAsync();
        var newId = GetNextId(oldTasks);
        task.Id = newId;

        var oldTask = TodoTaskAdapter.ToOldTask(task);
        oldTasks.Add(oldTask);
        await _oldRepository.SaveTasksAsync(oldTasks);

        return task;
    }

    public async Task<CoreTodoTask> UpdateTaskAsync(CoreTodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var oldTasks = await _oldRepository.LoadTasksAsync();
        var existingTaskIndex = oldTasks.FindIndex(t => t.Id == task.Id);
        
        if (existingTaskIndex == -1)
        {
            throw new ArgumentException($"Task with ID {task.Id} not found");
        }

        var oldTask = TodoTaskAdapter.ToOldTask(task);
        oldTasks[existingTaskIndex] = oldTask;
        await _oldRepository.SaveTasksAsync(oldTasks);

        return task;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var oldTasks = await _oldRepository.LoadTasksAsync();
        var taskToRemove = oldTasks.FirstOrDefault(t => t.Id == id);
        
        if (taskToRemove == null)
        {
            return false;
        }

        oldTasks.Remove(taskToRemove);
        await _oldRepository.SaveTasksAsync(oldTasks);

        return true;
    }

    public async Task<bool> TaskExistsAsync(int id)
    {
        var oldTasks = await _oldRepository.LoadTasksAsync();
        return oldTasks.Any(t => t.Id == id);
    }

    private static int GetNextId(List<TodoApp.TodoTask> tasks)
    {
        return tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
    }
}
