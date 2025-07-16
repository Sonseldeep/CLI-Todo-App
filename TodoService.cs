namespace TodoApp;

public class TodoService
{
    private readonly TodoRepository _repository;

    public TodoService(TodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoTask> AddTaskAsync(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Task description cannot be empty");
        }

        var tasks = await _repository.LoadTasksAsync();
        var newId = GetNextId(tasks);

        var newTask = new TodoTask
        {
            Id = newId,
            Description = description.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };

        tasks.Add(newTask);
        await _repository.SaveTasksAsync(tasks);

        return newTask;
    }

    public async Task<List<TodoTask>> GetAllTasksAsync()
    {
        return await _repository.LoadTasksAsync();
    }

    public async Task<TodoTask> CompleteTaskAsync(int taskId)
    {
        var tasks = await _repository.LoadTasksAsync();
        var task = FindTaskById(tasks, taskId);

        if (task.IsCompleted)
        {
            throw new InvalidOperationException($"Task {taskId} is already completed");
        }

        task.IsCompleted = true;
        await _repository.SaveTasksAsync(tasks);

        return task;
    }

    public async Task RemoveTaskAsync(int taskId)
    {
        var tasks = await _repository.LoadTasksAsync();
        var task = FindTaskById(tasks, taskId);

        tasks.Remove(task);
        await _repository.SaveTasksAsync(tasks);
    }

    private static TodoTask FindTaskById(List<TodoTask> tasks, int taskId)
    {
        var task = tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            throw new ArgumentException($"Task with ID {taskId} not found");
        }
        return task;
    }

    private static int GetNextId(List<TodoTask> tasks)
    {
        return tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
    }
}
