using Application.Interfaces.Persistence;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class InMemoryTodoListRepository : ITodoListRepository
{
    private int _currentId = 0;
    private readonly Lock _lock = new(); // Para asegurar que la generación de IDs sea thread-safe

    public int GetNextId()
    {
        lock (_lock)
        {
            _currentId++;
            return _currentId;
        }
    }

    public List<string> GetAllCategories()
    {
        return [.. Enum.GetNames<TodoItem.Category>()];
    }
}
