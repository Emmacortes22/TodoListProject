using Application.Services;
using Infrastructure.Repositories;
using Presentation.Console;

var repository = new InMemoryTodoListRepository();
var todoList = new TodoListService(repository);
var consoleUI = new ConsoleUI(repository, todoList);

consoleUI.Run(); 