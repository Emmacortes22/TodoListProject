using Application.Interfaces;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public class TodoListService(ITodoListRepository repository) : ITodoList
{
    private readonly List<TodoItem> _items = [];
    private readonly ITodoListRepository _repository = repository;
    private readonly TodoItemService _todoItemService = new();

    public void AddItem(int id, string title, string description, string category)
    {
        if (!Enum.TryParse<TodoItem.Category>(category, true, out var catEnum))
            throw new ArgumentException($"Categoría inválida: {category}");

        var nextId = _repository.GetNextId();
        var todoId = TodoItemId.From(nextId);
        var categoryObj = (int)catEnum;
        
        var item = _todoItemService.CreateTodoItem(todoId, title, description, categoryObj);
        _items.Add(item);
    }

    public void UpdateItem(int id, string description)
    {
        EnsureItemExists(id);
        var item = _items.First(i => i.Id.Value == id);
        item.UpdateDescription(description);
    }

    public void RemoveItem(int id)
    {
        EnsureItemExists(id);
        var item = _items.First(i => i.Id.Value == id);
        item.ValidateDelete();
        _items.Remove(item);
    }

    public void RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        EnsureItemExists(id);
        var item = _items.First(i => i.Id.Value == id);
        item.AddProgression(dateTime, percent);
    }

    public void PrintItems()
    {
        var orderedItems = _items.OrderBy(i => i.Id.Value);
        
        foreach (var item in orderedItems)
        {
            Console.WriteLine(_todoItemService.FormatItemHeader(item));
            
            decimal accumulatedPercent = 0;
            foreach (var prog in item.Progressions.OrderBy(p => p.Date))
            {
                accumulatedPercent += prog.Percent;
                Console.WriteLine(_todoItemService.FormatProgression(prog, accumulatedPercent));
            }
            Console.WriteLine();
        }
    }

    public IEnumerable<TodoItem> GetItems()
    {
        return _items.OrderBy(i => i.Id.Value);
    }

    public bool HasItems()
    {
        return _items.Any();
    }

    public void EnsureItemExists(int id)
    {
        if (!_items.Any(i => i.Id.Value == id))
            throw new ArgumentException($"No se encontró el item con ID {id}");
    }

    public TodoItem GetItemById(int id)
    {
        EnsureItemExists(id);
        return _items.First(i => i.Id.Value == id);
    }

    private void ValidateOperation(int id, Action<TodoItem> validation)
    {
        EnsureItemExists(id);
        var item = _items.First(i => i.Id.Value == id);
        validation(item);
    }

    public void ValidateIsCompleted(int id)
    {
        ValidateOperation(id, item => item.ValidateIsCompleted());
    }

    public void ValidateProgressionPercent(int id, decimal percent)
    {
        ValidateOperation(id, item =>
        {
            item.ValidateProgressPercent(percent);
            item.ValidateTotalProgress(percent);
        });
    }

    public void ValidateProgressionDate(int id, DateTime date)
    {
        ValidateOperation(id, item => item.ValidateProgressionDate(date));
    }

    public void ValidateUpdate(int id)
    {
        ValidateOperation(id, item => item.ValidateUpdate());
    }

    public void ValidateDelete(int id)
    {
        ValidateOperation(id, item => item.ValidateDelete());
    }
} 