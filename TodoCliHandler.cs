namespace TodoApp;

public class TodoCliHandler
{
    private readonly TodoService _todoService;

    public TodoCliHandler(TodoService todoService)
    {
        _todoService = todoService;
    }

    public async Task RunInteractiveMenuAsync()
    {
        var isRunning = true;

        while (isRunning)
        {
            try
            {
                ShowMenu();
                var choice = Console.ReadLine()?.Trim();

                isRunning = await ProcessMenuChoiceAsync(choice);

                if (isRunning)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    private async Task<bool> ProcessMenuChoiceAsync(string? choice)
    {
        return choice switch
        {
            "1" => await HandleInteractiveAddAsync(),
            "2" => await HandleInteractiveListAsync(),
            "3" => await HandleInteractiveCompleteAsync(),
            "4" => await HandleInteractiveRemoveAsync(),
            "5" => false, // Exit
            _ => HandleInvalidChoice()
        };
    }

    public async Task<bool> ProcessCommandAsync(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                await RunInteractiveMenuAsync();
                return true;
            }

            var command = args[0].ToLower();

            return command switch
            {
                "add" => await HandleAddCommandAsync(args),
                "list" => await HandleListCommandAsync(),
                "complete" => await HandleCompleteCommandAsync(args),
                "remove" => await HandleRemoveCommandAsync(args),
                "help" => HandleHelpCommand(),
                "interactive" => await RunInteractiveMenuAndReturn(),
                _ => HandleUnknownCommand(command)
            };
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            return false;
        }
    }

    private async Task<bool> RunInteractiveMenuAndReturn()
    {
        await RunInteractiveMenuAsync();
        return true;
    }

    private async Task<bool> HandleAddCommandAsync(string[] args)
    {
        if (args.Length < 2)
        {
            DisplayError("Usage: add <task description>");
            return false;
        }

        var description = string.Join(" ", args[1..]);
        var task = await _todoService.AddTaskAsync(description);
        DisplaySuccess($"Task added: {task.Id}. {task.Description}");
        return true;
    }

    private async Task<bool> HandleListCommandAsync()
    {
        var tasks = await _todoService.GetAllTasksAsync();

        if (!tasks.Any())
        {
            DisplayInfo("No tasks found. Add a task with: add <task description>");
            return true;
        }

        DisplayInfo("Your tasks:");
        foreach (var task in tasks.OrderBy(t => t.Id))
        {
            var status = task.GetStatusSymbol();
            var statusText = task.GetStatusText();
            Console.WriteLine($"  {task.Id}. {status} {task.Description} [{statusText}]");
        }

        return true;
    }

    private async Task<bool> HandleCompleteCommandAsync(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int taskId))
        {
            DisplayError("Usage: complete <task number>");
            return false;
        }

        var task = await _todoService.CompleteTaskAsync(taskId);
        DisplaySuccess($"Task {task.Id} completed: {task.Description}");
        return true;
    }

    private async Task<bool> HandleRemoveCommandAsync(string[] args)
    {
        if (args.Length < 2 || !int.TryParse(args[1], out int taskId))
        {
            DisplayError("Usage: remove <task number>");
            return false;
        }

        await _todoService.RemoveTaskAsync(taskId);
        DisplaySuccess($"Task {taskId} removed");
        return true;
    }

    private static bool HandleHelpCommand()
    {
        ShowUsage();
        return true;
    }

    private static bool HandleUnknownCommand(string command)
    {
        DisplayError($"Unknown command: {command}");
        ShowUsage();
        return false;
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Todo CLI Application");
        Console.WriteLine("Usage:");
        Console.WriteLine("  (no arguments)              - Start interactive menu");
        Console.WriteLine("  add <task description>      - Add a new task");
        Console.WriteLine("  list                        - Show all tasks");
        Console.WriteLine("  complete <task number>      - Mark a task as complete");
        Console.WriteLine("  remove <task number>        - Remove a task");
        Console.WriteLine("  help                        - Show this help message");
        Console.WriteLine("  interactive                 - Start interactive menu");
    }

    private static void DisplayError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {message}");
        Console.ResetColor();
    }

    private static void DisplaySuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void DisplayInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private async Task<bool> HandleInteractiveAddAsync()
    {
        Console.Write("Enter task description: ");
        var description = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(description))
        {
            DisplayError("Task description cannot be empty");
            return true;
        }

        var task = await _todoService.AddTaskAsync(description);
        DisplaySuccess($"Task added: {task.Id}. {task.Description}");
        return true;
    }

    private async Task<bool> HandleInteractiveListAsync()
    {
        var tasks = await _todoService.GetAllTasksAsync();

        if (!tasks.Any())
        {
            DisplayInfo("No tasks found. Add a task first!");
            return true;
        }

        DisplayInfo("Your tasks:");
        foreach (var task in tasks.OrderBy(t => t.Id))
        {
            var status = task.GetStatusSymbol();
            var statusText = task.GetStatusText();
            Console.WriteLine($"  {task.Id}. {status} {task.Description} [{statusText}]");
        }

        return true;
    }

    private async Task<bool> HandleInteractiveCompleteAsync()
    {
        var tasks = await _todoService.GetAllTasksAsync();

        if (!tasks.Any())
        {
            DisplayInfo("No tasks found. Add a task first!");
            return true;
        }

        DisplayInfo("Your tasks:");
        foreach (var todoTask in tasks.OrderBy(t => t.Id))
        {
            var status = todoTask.GetStatusSymbol();
            var statusText = todoTask.GetStatusText();
            Console.WriteLine($"  {todoTask.Id}. {status} {todoTask.Description} [{statusText}]");
        }

        Console.Write("\nEnter task number to complete: ");
        var input = Console.ReadLine()?.Trim();

        if (!int.TryParse(input, out int taskId))
        {
            DisplayError("Please enter a valid task number");
            return true;
        }

        var task = await _todoService.CompleteTaskAsync(taskId);
        DisplaySuccess($"Task {task.Id} completed: {task.Description}");
        return true;
    }

    private async Task<bool> HandleInteractiveRemoveAsync()
    {
        var tasks = await _todoService.GetAllTasksAsync();

        if (!tasks.Any())
        {
            DisplayInfo("No tasks found. Add a task first!");
            return true;
        }

        DisplayInfo("Your tasks:");
        foreach (var task in tasks.OrderBy(t => t.Id))
        {
            var status = task.GetStatusSymbol();
            var statusText = task.GetStatusText();
            Console.WriteLine($"  {task.Id}. {status} {task.Description} [{statusText}]");
        }

        Console.Write("\nEnter task number to remove: ");
        var input = Console.ReadLine()?.Trim();

        if (!int.TryParse(input, out int taskId))
        {
            DisplayError("Please enter a valid task number");
            return true;
        }

        await _todoService.RemoveTaskAsync(taskId);
        DisplaySuccess($"Task {taskId} removed");
        return true;
    }

    private static bool HandleInvalidChoice()
    {
        DisplayError("Invalid choice. Please select a number from 1-5");
        return true;
    }

    private static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Todo CLI Application ===");
        Console.WriteLine();
        Console.WriteLine("1. Add a new task");
        Console.WriteLine("2. List all tasks");
        Console.WriteLine("3. Complete a task");
        Console.WriteLine("4. Remove a task");
        Console.WriteLine("5. Exit");
        Console.WriteLine();
        Console.Write("Please select an option (1-5): ");
    }
}
