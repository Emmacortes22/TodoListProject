namespace Application.Interfaces.Persistence;

public interface ITodoListRepository
{
    int GetNextId();
    List<string> GetAllCategories();
} 