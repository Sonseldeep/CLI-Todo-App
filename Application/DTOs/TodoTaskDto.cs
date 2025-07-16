namespace TodoApp.Application.DTOs;

public class TodoTaskDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string StatusSymbol { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string FormattedCreatedDate { get; set; } = string.Empty;
    public string FormattedCompletedDate { get; set; } = string.Empty;
}
