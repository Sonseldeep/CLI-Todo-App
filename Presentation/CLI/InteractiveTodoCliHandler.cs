using TodoApp.Core.Interfaces;
using TodoApp.Application.DTOs;

namespace TodoApp.Presentation.CLI;

public class InteractiveTodoCliHandler
{
    private readonly ITodoService _todoService;
    private readonly IConsoleHelper _consoleHelper;

    public InteractiveTodoCliHandler(ITodoService todoService, IConsoleHelper consoleHelper)
    {
        _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        _consoleHelper = consoleHelper ?? throw new ArgumentNullException(nameof(consoleHelper));
    }

    public async Task RunAsync()
    {
        _consoleHelper.ShowWelcomeMessage();

        var isRunning = true;
        while (isRunning)
        {
            try
            {
                _consoleHelper.ShowMainMenu();
                var choice = _consoleHelper.ReadInput("Please select an option (1-7): ");

                isRunning = await ProcessMenuChoiceAsync(choice);

                if (isRunning)
                {
                    _consoleHelper.PauseForUser();
                    _consoleHelper.ClearScreen();
                }
            }
            catch (Exception ex)
            {
                _consoleHelper.ShowError($"An unexpected error occurred: {ex.Message}");
                _consoleHelper.PauseForUser();
                _consoleHelper.ClearScreen();
            }
        }

        _consoleHelper.ShowGoodbyeMessage();
    }

    private async Task<bool> ProcessMenuChoiceAsync(string? choice)
    {
        return choice?.Trim() switch
        {
            "1" => await HandleAddTaskAsync(),
            "2" => await HandleListTasksAsync(),
            "3" => await HandleCompleteTaskAsync(),
            "4" => await HandleRemoveTaskAsync(),
            "5" => await HandleUpdateTaskAsync(),
            "6" => await HandleViewTaskDetailsAsync(),
            "7" => HandleExit(),
            _ => HandleInvalidChoice()
        };
    }

    private async Task<bool> HandleAddTaskAsync()
    {
        _consoleHelper.ShowSectionHeader("Add New Task");

        var description = _consoleHelper.ReadInput("Enter task description: ");

        if (string.IsNullOrWhiteSpace(description))
        {
            _consoleHelper.ShowError("Task description cannot be empty");
            return true;
        }

        var result = await _todoService.AddTaskAsync(description);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess($"Task added successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private async Task<bool> HandleListTasksAsync()
    {
        _consoleHelper.ShowSectionHeader("All Tasks");

        var result = await _todoService.GetAllTasksAsync();

        if (result.IsSuccess && result.Value != null)
        {
            if (result.Value.Count == 0)
            {
                _consoleHelper.ShowInfo("No tasks found. Add a task first!");
                return true;
            }

            _consoleHelper.ShowTaskList(result.Value);
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private async Task<bool> HandleCompleteTaskAsync()
    {
        _consoleHelper.ShowSectionHeader("Complete Task");

        var allTasksResult = await _todoService.GetAllTasksAsync();
        if (!allTasksResult.IsSuccess || allTasksResult.Value == null || allTasksResult.Value.Count == 0)
        {
            _consoleHelper.ShowInfo("No tasks available to complete.");
            return true;
        }

        var pendingTasks = allTasksResult.Value.Where(t => !t.IsCompleted).ToList();
        if (pendingTasks.Count == 0)
        {
            _consoleHelper.ShowInfo("All tasks are already completed!");
            return true;
        }

        _consoleHelper.ShowTaskList(pendingTasks, "Pending Tasks:");

        var taskIdInput = _consoleHelper.ReadInput("Enter task number to complete: ");

        if (!int.TryParse(taskIdInput, out int taskId))
        {
            _consoleHelper.ShowError("Please enter a valid task number");
            return true;
        }

        var result = await _todoService.CompleteTaskAsync(taskId);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess("Task completed successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private async Task<bool> HandleRemoveTaskAsync()
    {
        _consoleHelper.ShowSectionHeader("Remove Task");

        var allTasksResult = await _todoService.GetAllTasksAsync();
        if (!allTasksResult.IsSuccess || allTasksResult.Value == null || allTasksResult.Value.Count == 0)
        {
            _consoleHelper.ShowInfo("No tasks available to remove.");
            return true;
        }

        _consoleHelper.ShowTaskList(allTasksResult.Value);

        var taskIdInput = _consoleHelper.ReadInput("Enter task number to remove: ");

        if (!int.TryParse(taskIdInput, out int taskId))
        {
            _consoleHelper.ShowError("Please enter a valid task number");
            return true;
        }

        var confirmInput = _consoleHelper.ReadInput($"Are you sure you want to remove task {taskId}? (y/N): ");

        if (confirmInput?.ToLower() != "y")
        {
            _consoleHelper.ShowInfo("Task removal cancelled.");
            return true;
        }

        var result = await _todoService.RemoveTaskAsync(taskId);

        if (result.IsSuccess)
        {
            _consoleHelper.ShowSuccess($"Task {taskId} removed successfully!");
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private async Task<bool> HandleUpdateTaskAsync()
    {
        _consoleHelper.ShowSectionHeader("Update Task");

        var allTasksResult = await _todoService.GetAllTasksAsync();
        if (!allTasksResult.IsSuccess || allTasksResult.Value == null || allTasksResult.Value.Count == 0)
        {
            _consoleHelper.ShowInfo("No tasks available to update.");
            return true;
        }

        _consoleHelper.ShowTaskList(allTasksResult.Value);

        var taskIdInput = _consoleHelper.ReadInput("Enter task number to update: ");

        if (!int.TryParse(taskIdInput, out int taskId))
        {
            _consoleHelper.ShowError("Please enter a valid task number");
            return true;
        }

        var newDescription = _consoleHelper.ReadInput("Enter new task description: ");

        if (string.IsNullOrWhiteSpace(newDescription))
        {
            _consoleHelper.ShowError("Task description cannot be empty");
            return true;
        }

        var result = await _todoService.UpdateTaskAsync(taskId, newDescription);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess("Task updated successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private async Task<bool> HandleViewTaskDetailsAsync()
    {
        _consoleHelper.ShowSectionHeader("View Task Details");

        var allTasksResult = await _todoService.GetAllTasksAsync();
        if (!allTasksResult.IsSuccess || allTasksResult.Value == null || allTasksResult.Value.Count == 0)
        {
            _consoleHelper.ShowInfo("No tasks available to view.");
            return true;
        }

        _consoleHelper.ShowTaskList(allTasksResult.Value);

        var taskIdInput = _consoleHelper.ReadInput("Enter task number to view details: ");

        if (!int.TryParse(taskIdInput, out int taskId))
        {
            _consoleHelper.ShowError("Please enter a valid task number");
            return true;
        }

        var result = await _todoService.GetTaskByIdAsync(taskId);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowTaskDetails(result.Value);
        }
        else
        {
            _consoleHelper.ShowError(result.Error);
        }

        return true;
    }

    private static bool HandleExit()
    {
        return false;
    }

    private bool HandleInvalidChoice()
    {
        _consoleHelper.ShowError("Invalid choice. Please select a number from 1-7");
        return true;
    }
}
