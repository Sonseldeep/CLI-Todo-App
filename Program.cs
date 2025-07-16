using TodoApp.Core.Interfaces;
using TodoApp.Core.Services;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Presentation.CLI;

// Dependency injection setup
var oldRepository = new TodoApp.TodoRepository();
var repository = new TodoRepositoryAdapter(oldRepository);
var todoService = new TodoService(repository);
var consoleHelper = new ConsoleHelper();
var commandLineHandler = new CommandLineHandler(todoService, consoleHelper);

// Run the application
var exitCode = await commandLineHandler.ProcessAsync(args);
Environment.Exit(exitCode);
