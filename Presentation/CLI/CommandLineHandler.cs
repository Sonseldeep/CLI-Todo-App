using TodoApp.Core.Interfaces;
using TodoApp.Presentation.CLI;

namespace TodoApp.Presentation.CLI;

public class CommandLineHandler
{
    private readonly ITodoService _todoService;
    private readonly IConsoleHelper _consoleHelper;

    public CommandLineHandler(ITodoService todoService, IConsoleHelper consoleHelper)
    {
        _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        _consoleHelper = consoleHelper ?? throw new ArgumentNullException(nameof(consoleHelper));
    }

    public async Task<int> ProcessAsync(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                var interactiveHandler = new InteractiveTodoCliHandler(_todoService, _consoleHelper);
                await interactiveHandler.RunAsync();
                return 0;
            }

            var command = args[0].ToLower();

            return command switch
            {
                "add" => await HandleAddCommandAsync(args),
                "list" => await HandleListCommandAsync(),
                "complete" => await HandleCompleteCommandAsync(args),
                "remove" => await HandleRemoveCommandAsync(args),
                "update" => await HandleUpdateCommandAsync(args),
                "view" => await HandleViewCommandAsync(args),
                "help" => HandleHelpCommand(),
                "interactive" => await HandleInteractiveCommandAsync(),
                _ => HandleUnknownCommand(command)
            };
        }
        catch (Exception ex)
        {
            _consoleHelper.ShowError($"An unexpected error occurred: {ex.Message}");
            return 1;
        }
    }

    private async Task<int> HandleAddCommandAsync(string[] args)
    {
        if (args.Length < 2)
        {
            _consoleHelper.ShowError("Usage: add <task description>");
            return 1;
        }

        var description = string.Join(" ", args[1..]);
        var result = await _todoService.AddTaskAsync(description);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess($"Task added successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleListCommandAsync()
    {
        var result = await _todoService.GetAllTasksAsync();

        if (result.IsSuccess && result.Value != null)
        {
            if (result.Value.Count == 0)
            {
                _consoleHelper.ShowInfo("No tasks found. Add a task with: add <task description>");
                return 0;
            }

            _consoleHelper.ShowTaskList(result.Value, "All Tasks:");
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleCompleteCommandAsync(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int taskId))
        {
            _consoleHelper.ShowError("Usage: complete <task number>");
            return 1;
        }

        var result = await _todoService.CompleteTaskAsync(taskId);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess("Task completed successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleRemoveCommandAsync(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int taskId))
        {
            _consoleHelper.ShowError("Usage: remove <task number>");
            return 1;
        }

        var result = await _todoService.RemoveTaskAsync(taskId);

        if (result.IsSuccess)
        {
            _consoleHelper.ShowSuccess($"Task {taskId} removed successfully!");
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleUpdateCommandAsync(string[] args)
    {
        if (args.Length < 3 || !int.TryParse(args[1], out int taskId))
        {
            _consoleHelper.ShowError("Usage: update <task number> <new description>");
            return 1;
        }

        var newDescription = string.Join(" ", args[2..]);
        var result = await _todoService.UpdateTaskAsync(taskId, newDescription);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowSuccess("Task updated successfully!");
            _consoleHelper.ShowTaskDetails(result.Value);
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleViewCommandAsync(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int taskId))
        {
            _consoleHelper.ShowError("Usage: view <task number>");
            return 1;
        }

        var result = await _todoService.GetTaskByIdAsync(taskId);

        if (result.IsSuccess && result.Value != null)
        {
            _consoleHelper.ShowTaskDetails(result.Value);
            return 0;
        }

        _consoleHelper.ShowError(result.Error);
        return 1;
    }

    private async Task<int> HandleInteractiveCommandAsync()
    {
        var interactiveHandler = new InteractiveTodoCliHandler(_todoService, _consoleHelper);
        await interactiveHandler.RunAsync();
        return 0;
    }

    private int HandleHelpCommand()
    {
        ShowUsage();
        return 0;
    }

    private int HandleUnknownCommand(string command)
    {
        _consoleHelper.ShowError($"Unknown command: {command}");
        ShowUsage();
        return 1;
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Todo CLI Application - Your Personal Task Manager");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  (no arguments)                      - Start interactive menu");
        Console.WriteLine("  add <task description>              - Add a new task");
        Console.WriteLine("  list                                - Show all tasks");
        Console.WriteLine("  complete <task number>              - Mark a task as complete");
        Console.WriteLine("  remove <task number>                - Remove a task");
        Console.WriteLine("  update <task number> <description>  - Update a task description");
        Console.WriteLine("  view <task number>                  - View task details");
        Console.WriteLine("  help                                - Show this help message");
        Console.WriteLine("  interactive                         - Start interactive menu");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  add \"Buy groceries\"");
        Console.WriteLine("  complete 1");
        Console.WriteLine("  update 1 \"Buy groceries and cook dinner\"");
        Console.WriteLine("  remove 1");
    }
}
