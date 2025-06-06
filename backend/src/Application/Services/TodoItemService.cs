using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public class TodoItemService
{
    public TodoItem CreateTodoItem(TodoItemId id, string title, string description, int category)
    {
        return new TodoItem(id, title, description, category);
    }

    public string FormatProgressBar(decimal progress)
    {
        const int width = 50;
        var filled = (int)Math.Round(progress * width / 100);
        return new string('O', filled).PadRight(width);
    }

    public string FormatItemHeader(TodoItem item)
    {
        return $"{item.Id.Value}) {item.Title} - {item.Description} ({item.ItemCategory}) Completed:{item.IsCompleted}";
    }

    public string FormatProgression(TodoItem.Progression prog, decimal accumulatedPercent)
    {
        var progressBar = FormatProgressBar(accumulatedPercent);
        var culture = new System.Globalization.CultureInfo("en-US"); // Cambiado a en-US para que muestre AM/PM ya que mi configuración regional es es-ES
        return $"{prog.Date.ToString("MM/dd/yyyy hh:mm:ss tt", culture)} - {accumulatedPercent}%\t|{progressBar}|";
    }
}
