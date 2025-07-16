using System.Text.Json;

namespace TodoApp;

public class TodoRepository
{
    private readonly string _filePath;

    public TodoRepository(string filePath = "tasks.json")
    {
        _filePath = filePath;
    }

    public async Task<List<TodoTask>> LoadTasksAsync()
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

            return JsonSerializer.Deserialize<List<TodoTask>>(jsonContent) ?? new List<TodoTask>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load tasks: {ex.Message}", ex);
        }
    }

    public async Task SaveTasksAsync(List<TodoTask> tasks)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save tasks: {ex.Message}", ex);
        }
    }
}
