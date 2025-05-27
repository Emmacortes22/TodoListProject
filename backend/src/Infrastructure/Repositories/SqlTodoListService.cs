using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Application.Interfaces.Persistence;
using Application.Services;
using Domain.ValueObjects;

namespace Infrastructure.Repositories;

public class SqlTodoListService : ITodoList
{
    private readonly TodoListDbContext _context;
    private readonly ITodoListRepository _repository;
    private readonly TodoItemService _todoItemService = new();

    public SqlTodoListService(ITodoListRepository repository, TodoListDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public void AddItem(int id, string title, string description, string category)
    {
        if (!Enum.TryParse<TodoItem.Category>(category, true, out var catEnum))
            throw new ArgumentException($"Categoría inválida: {category}");

        var todoId = id <= 0
            ? TodoItemId.From(_repository.GetNextId())
            : TodoItemId.From(id);

        var categoryObj = (int)catEnum;

        var item = _todoItemService.CreateTodoItem(todoId, title, description, categoryObj);
        _context.TodoItems.Add(item);
        _context.SaveChanges();
    }

    public void UpdateItem(int id, string description)
    {
        var item = GetItemById(id);
        item.UpdateDescription(description);
        _context.SaveChanges();
    }

    public void RemoveItem(int id)
    {
        var item = GetItemById(id);
        item.ValidateDelete();
        _context.TodoItems.Remove(item);
        _context.SaveChanges();
    }

    public void RegisterProgression(int id, DateTime date, decimal percent)
    {
        var item = GetItemByIdWithProgressions(id);
        item.AddProgression(date, percent);
        _context.SaveChanges();
    }

    public IEnumerable<TodoItem> GetItems()
    {
        return _context.TodoItems
            .Include(t => t.Progressions)
            .ToList()
            .OrderBy(t => t.Id.Value); // No puede ordenar un int encapusaldo en un TodoItem
    }

    public void PrintItems()
    {
        throw new NotSupportedException("Este método solo está disponible en el modo consola.");
    }

    public bool HasItems()
    {
        throw new NotSupportedException("Este método solo está disponible en el modo consola.");
    }

    public void EnsureItemExists(int id)
    {
        throw new NotSupportedException("Este método solo está disponible en el modo consola.");
    }

    public TodoItem GetItemById(int id)
    {
        var item = _context.TodoItems
            .AsEnumerable()
            .FirstOrDefault(i => i.Id.Value == id);

        return item ?? throw new ArgumentException($"No se encontró el item con ID {id}");
    }

    private TodoItem GetItemByIdWithProgressions(int id)
    {
        return _context.TodoItems
            .Include(i => i.Progressions)
            .AsEnumerable()
            .FirstOrDefault(i => i.Id.Value == id)
            ?? throw new ArgumentException($"No se encontró el item con ID {id}");
    }

    public void ValidateIsCompleted(int id)
    {
        var item = GetItemByIdWithProgressions(id);
        item.ValidateIsCompleted();
    }

    public void ValidateProgressionPercent(int id, decimal percent)
    {
        var item = GetItemByIdWithProgressions(id);
        item.ValidateProgressPercent(percent);
        item.ValidateTotalProgress(percent);
    }

    public void ValidateProgressionDate(int id, DateTime date)
    {
        var item = GetItemByIdWithProgressions(id);
        item.ValidateProgressionDate(date);
    }

    public void ValidateUpdate(int id)
    {
        var item = GetItemByIdWithProgressions(id);
        item.ValidateUpdate();
    }

    public void ValidateDelete(int id)
    {
        var item = GetItemByIdWithProgressions(id);
        item.ValidateDelete();
    }
}