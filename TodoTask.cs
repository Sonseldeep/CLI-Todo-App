using System.Text.Json.Serialization;

namespace TodoApp;

public class TodoTask
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string GetStatusSymbol() => IsCompleted ? "✓" : "○";

    public string GetStatusText() => IsCompleted ? "Completed" : "Pending";
}
