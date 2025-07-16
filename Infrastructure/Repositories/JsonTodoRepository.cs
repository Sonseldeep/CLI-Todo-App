using System.Text.Json;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;

namespace TodoApp.Infrastructure.Repositories;

public class JsonTodoRepository : ITodoRepository
{
    private readonly string _filePath;

    public JsonTodoRepository(string filePath = "tasks.json")
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<List<TodoTask>> GetAllTasksAsync()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new List<TodoTask>();
            }

            var jsonContent = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new List<TodoTask>();
            }

            var deserializedTasks = JsonSerializer.Deserialize<List<TodoTask>>(jsonContent);
            return deserializedTasks ?? new List<TodoTask>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load tasks: {ex.Message}", ex);
        }
    }

    public async Task<TodoTask?> GetTaskByIdAsync(int id)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.FirstOrDefault(t => t.Id == id);
    }

    public async Task<TodoTask> AddTaskAsync(TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var tasks = await GetAllTasksAsync();
        var newId = GetNextId(tasks);
        task.Id = newId;

        tasks.Add(task);
        await SaveTasksAsync(tasks);

        return task;
    }

    public async Task<TodoTask> UpdateTaskAsync(TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var tasks = await GetAllTasksAsync();
        var existingTaskIndex = tasks.FindIndex(t => t.Id == task.Id);

        if (existingTaskIndex == -1)
        {
            throw new ArgumentException($"Task with ID {task.Id} not found");
        }

        tasks[existingTaskIndex] = task;
        await SaveTasksAsync(tasks);

        return task;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var tasks = await GetAllTasksAsync();
        var taskToRemove = tasks.FirstOrDefault(t => t.Id == id);

        if (taskToRemove == null)
        {
            return false;
        }

        tasks.Remove(taskToRemove);
        await SaveTasksAsync(tasks);

        return true;
    }

    public async Task<bool> TaskExistsAsync(int id)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.Any(t => t.Id == id);
    }

    private async Task SaveTasksAsync(List<TodoTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonContent = JsonSerializer.Serialize(tasks, options);
            await File.WriteAllTextAsync(_filePath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save tasks: {ex.Message}", ex);
        }
    }

    private static int GetNextId(List<TodoTask> tasks)
    {
        return tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
    }

    Task<List<Core.Entities.TodoTask>> ITodoRepository.GetAllTasksAsync()
    {
        throw new NotImplementedException();
    }

    Task<Core.Entities.TodoTask?> ITodoRepository.GetTaskByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Core.Entities.TodoTask> AddTaskAsync(Core.Entities.TodoTask task)
    {
        throw new NotImplementedException();
    }

    public Task<Core.Entities.TodoTask> UpdateTaskAsync(Core.Entities.TodoTask task)
    {
        throw new NotImplementedException();
    }
}
