using TodoApp.Application.DTOs;
using TodoApp.Application.Common;

namespace TodoApp.Core.Interfaces;

public interface ITodoService
{
    Task<Result<TodoTaskDto>> AddTaskAsync(string description);
    Task<Result<List<TodoTaskDto>>> GetAllTasksAsync();
    Task<Result<TodoTaskDto>> CompleteTaskAsync(int taskId);
    Task<Result<bool>> RemoveTaskAsync(int taskId);
    Task<Result<TodoTaskDto>> UpdateTaskAsync(int taskId, string newDescription);
    Task<Result<TodoTaskDto>> GetTaskByIdAsync(int taskId);
}
