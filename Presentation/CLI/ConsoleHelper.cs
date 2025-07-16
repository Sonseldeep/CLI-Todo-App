using TodoApp.Application.DTOs;

namespace TodoApp.Presentation.CLI;

public class ConsoleHelper : IConsoleHelper
{
    public void ShowWelcomeMessage()
    {
        ClearScreen();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║           📋 Todo CLI App              ║");
        Console.WriteLine("║      Your Personal Task Manager       ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    public void ShowGoodbyeMessage()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n🎉 Thank you for using Todo CLI App!");
        Console.WriteLine("Stay productive and have a great day! 👋");
        Console.ResetColor();
    }

    public void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════ MAIN MENU ═══════════════");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("1. 📝 Add a new task");
        Console.WriteLine("2. 📋 List all tasks");
        Console.WriteLine("3. ✅ Complete a task");
        Console.WriteLine("4. 🗑️  Remove a task");
        Console.WriteLine("5. ✏️  Update a task");
        Console.WriteLine("6. 🔍 View task details");
        Console.WriteLine("7. 🚪 Exit");
        Console.WriteLine();
        Console.WriteLine("══════════════════════════════════════════");
        Console.WriteLine();
    }

    public void ShowSectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"═══ {title.ToUpper()} ═══");
        Console.ResetColor();
        Console.WriteLine();
    }

    public void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ {message}");
        Console.ResetColor();
    }

    public void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Error: {message}");
        Console.ResetColor();
    }

    public void ShowInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ️  {message}");
        Console.ResetColor();
    }

    public void ShowTaskList(List<TodoTaskDto> tasks, string? header = null)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        if (!string.IsNullOrEmpty(header))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n{header}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("┌─────┬────────────────────────────────────────────────────────────────┬─────────────┬──────────────────────┐");
        Console.WriteLine("│ ID  │ Description                                                    │ Status      │ Created              │");
        Console.WriteLine("├─────┼────────────────────────────────────────────────────────────────┼─────────────┼──────────────────────┤");

        foreach (var task in tasks.OrderBy(t => t.Id))
        {
            var statusColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.Yellow;
            var description = task.Description.Length > 60 ? task.Description.Substring(0, 57) + "..." : task.Description;

            Console.Write("│ ");
            Console.Write($"{task.Id,-3}");
            Console.Write(" │ ");
            Console.Write($"{description,-62}");
            Console.Write(" │ ");

            Console.ForegroundColor = statusColor;
            Console.Write($"{task.StatusSymbol} {task.StatusText,-9}");
            Console.ResetColor();

            Console.Write(" │ ");
            Console.Write($"{task.FormattedCreatedDate,-20}");
            Console.WriteLine(" │");
        }

        Console.WriteLine("└─────┴────────────────────────────────────────────────────────────────┴─────────────┴──────────────────────┘");
        Console.WriteLine();
    }

    public void ShowTaskDetails(TodoTaskDto task)
    {
        ArgumentNullException.ThrowIfNull(task);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════ TASK DETAILS ═══════════════");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"📋 Task ID: {task.Id}");
        Console.WriteLine($"📝 Description: {task.Description}");

        Console.ForegroundColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.Yellow;
        Console.WriteLine($"📊 Status: {task.StatusSymbol} {task.StatusText}");
        Console.ResetColor();

        Console.WriteLine($"📅 Created: {task.FormattedCreatedDate}");

        if (task.IsCompleted && task.CompletedAt.HasValue)
        {
            Console.WriteLine($"✅ Completed: {task.FormattedCompletedDate}");
        }

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════");
        Console.WriteLine();
    }

    public string ReadInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public void PauseForUser()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    public void ClearScreen()
    {
        Console.Clear();
    }
}
