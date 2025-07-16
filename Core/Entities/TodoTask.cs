using System.Text.Json.Serialization;

namespace TodoApp.Core.Entities;

public class TodoTask
{
    public TodoTask(string description)
    {
        Id = 0; // Will be set by service
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    [JsonConstructor]
    public TodoTask(int id, string description, bool isCompleted, DateTime createdAt)
    {
        Id = id;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsCompleted = isCompleted;
        CreatedAt = createdAt;
    }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; private set; }

    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; private set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; private set; }

    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Task is already completed");
        }

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new ArgumentException("Description cannot be empty", nameof(newDescription));
        }

        Description = newDescription.Trim();
    }

    public string GetStatusSymbol() => IsCompleted ? "✓" : "○";

    public string GetStatusText() => IsCompleted ? "Completed" : "Pending";

    public string GetFormattedCreatedDate() => CreatedAt.ToString("yyyy-MM-dd HH:mm");

    public string GetFormattedCompletedDate() => CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
}
