using TodoApp.Core.Interfaces;
using TodoApp.Application.DTOs;
using TodoApp.Application.Common;
using CoreTodoTask = TodoApp.Core.Entities.TodoTask;

namespace TodoApp.Core.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<TodoTaskDto>> AddTaskAsync(string description)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return Result<TodoTaskDto>.Failure("Task description cannot be empty");
            }

            var task = new CoreTodoTask(description.Trim());
            var addedTask = await _repository.AddTaskAsync(task);
            var taskDto = TodoTaskMapper.ToDto(addedTask);

            return Result<TodoTaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TodoTaskDto>.Failure($"Failed to add task: {ex.Message}");
        }
    }

    public async Task<Result<List<TodoTaskDto>>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _repository.GetAllTasksAsync();
            var taskDtos = TodoTaskMapper.ToDto(tasks);

            return Result<List<TodoTaskDto>>.Success(taskDtos);
        }
        catch (Exception ex)
        {
            return Result<List<TodoTaskDto>>.Failure($"Failed to retrieve tasks: {ex.Message}");
        }
    }

    public async Task<Result<TodoTaskDto>> CompleteTaskAsync(int taskId)
    {
        try
        {
            var existingTask = await _repository.GetTaskByIdAsync(taskId);
            if (existingTask == null)
            {
                return Result<TodoTaskDto>.Failure($"Task with ID {taskId} not found");
            }

            if (existingTask.IsCompleted)
            {
                return Result<TodoTaskDto>.Failure($"Task {taskId} is already completed");
            }

            // Create a new completed task
            var completedTask = new CoreTodoTask(existingTask.Id, existingTask.Description, true, existingTask.CreatedAt);
            var updatedTask = await _repository.UpdateTaskAsync(completedTask);
            var taskDto = TodoTaskMapper.ToDto(updatedTask);

            return Result<TodoTaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TodoTaskDto>.Failure($"Failed to complete task: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveTaskAsync(int taskId)
    {
        try
        {
            var taskExists = await _repository.TaskExistsAsync(taskId);
            if (!taskExists)
            {
                return Result<bool>.Failure($"Task with ID {taskId} not found");
            }

            var result = await _repository.DeleteTaskAsync(taskId);
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to remove task: {ex.Message}");
        }
    }

    public async Task<Result<TodoTaskDto>> UpdateTaskAsync(int taskId, string newDescription)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                return Result<TodoTaskDto>.Failure("Task description cannot be empty");
            }

            var existingTask = await _repository.GetTaskByIdAsync(taskId);
            if (existingTask == null)
            {
                return Result<TodoTaskDto>.Failure($"Task with ID {taskId} not found");
            }

            // Create a new task with updated description
            var updatedTask = new CoreTodoTask(existingTask.Id, newDescription.Trim(), existingTask.IsCompleted, existingTask.CreatedAt);
            var result = await _repository.UpdateTaskAsync(updatedTask);
            var taskDto = TodoTaskMapper.ToDto(result);

            return Result<TodoTaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TodoTaskDto>.Failure($"Failed to update task: {ex.Message}");
        }
    }

    public async Task<Result<TodoTaskDto>> GetTaskByIdAsync(int taskId)
    {
        try
        {
            var task = await _repository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return Result<TodoTaskDto>.Failure($"Task with ID {taskId} not found");
            }

            var taskDto = TodoTaskMapper.ToDto(task);
            return Result<TodoTaskDto>.Success(taskDto);
        }
        catch (Exception ex)
        {
            return Result<TodoTaskDto>.Failure($"Failed to retrieve task: {ex.Message}");
        }
    }
}
