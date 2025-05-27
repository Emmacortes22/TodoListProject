using Domain.Entities;
using Infrastructure.Data;
using Application.Interfaces.Persistence;

namespace Infrastructure.Repositories;

public class SqlTodoListRepository(TodoListDbContext context) : ITodoListRepository
{
    private readonly TodoListDbContext _context = context;

    public int GetNextId()
    {
        return _context.TodoItems
            .AsEnumerable()
            .Select(t => t.Id.Value)
            .DefaultIfEmpty(0)
            .Max() + 1;
    }

    public List<string> GetAllCategories()
    {
        return [.. Enum.GetNames<TodoItem.Category>()];
    }
}