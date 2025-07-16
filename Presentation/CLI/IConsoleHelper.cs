using TodoApp.Application.DTOs;

namespace TodoApp.Presentation.CLI;

public interface IConsoleHelper
{
    void ShowWelcomeMessage();
    void ShowGoodbyeMessage();
    void ShowMainMenu();
    void ShowSectionHeader(string title);
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowInfo(string message);
    void ShowTaskList(List<TodoTaskDto> tasks, string? header = null);
    void ShowTaskDetails(TodoTaskDto task);
    string ReadInput(string prompt);
    void PauseForUser();
    void ClearScreen();
}
