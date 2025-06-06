using System.Globalization;
using Domain.ValueObjects;

namespace Domain.Entities;

public class TodoItem
{
    private TodoItem() 
    {
        _progressions = new List<Progression>();
    }

    public TodoItem(TodoItemId id, string title, string description, int category)
    {
        Id = id;
        Title = title;
        Description = description;
        ItemCategory = (Category)category;
        _progressions = new List<Progression>();
    }

    public TodoItemId Id { get; private set; } = default!;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public enum Category
    {
        Trabajo,
        Hogar,
        Salud,
        Estudio,
        Ocio,
        Otro
    }

    public Category ItemCategory { get; private set; }

    private readonly List<Progression> _progressions;
    public IReadOnlyList<Progression> Progressions => _progressions.AsReadOnly();

    public bool IsCompleted => GetTotalProgress() == 100;

    public void UpdateDescription(string description)
    {
        ValidateUpdate();
        Description = description;
    }

    public void ValidateUpdate()
    {
        if (GetTotalProgress() > 50m)
            throw new ArgumentException("No se puede actualizar una tarea con un progreso superior a 50%");
    }

    public void ValidateDelete()
    {
        if (GetTotalProgress() > 50m)
            throw new ArgumentException("No se puede eliminar una tarea con un progreso superior a 50%");
    }

    public void AddProgression(DateTime date, decimal percent)
    {
        ValidateIsCompleted();
        ValidateProgressPercent(percent);
        ValidateProgressionDate(date);
        ValidateTotalProgress(percent);

        _progressions.Add(new Progression(date, percent));
    }

    public void ValidateProgressPercent(decimal percent)
    {
        if (percent <= 0 || percent >= 100)
            throw new ArgumentException("El porcentaje debe ser mayor a 0 y menor a 100");
    }

    public void ValidateProgressionDate(DateTime newDate)
    {
        // Tengo la duda de si se puede del mismo dia pero hora posterior al ultimo progreso? Si solo se puede del dia siguiente seria (p => newDate.Date <= p.Date.Date)
        if (_progressions.Any(p => newDate <= p.Date)) // Se usa Any() porque con Max() puede saltar exception con una lista vacia
        {
            var latestDate = _progressions.Max(p => p.Date);
            throw new ArgumentException(
                $"La fecha del progreso no puede ser anterior o igual a la fecha de la última progresión. Última fecha: {latestDate.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture)
            }");
        }
    }

    public void ValidateTotalProgress(decimal newPercent)
    {
        var totalAfterAdd = GetTotalProgress() + newPercent;
        if (totalAfterAdd > 100)
            throw new ArgumentException($"El porcentaje de progreso no puede exceder el 100%. Total actual: {GetTotalProgress()}%");
    }

    public void ValidateIsCompleted()
    {
        if (IsCompleted)
            throw new ArgumentException("La tarea esta completada, no se puede añadir más progresos");
    }

    public class Progression
    {
        private Progression() { }

        public Progression(DateTime date, decimal percent)
        {
            Date = date;
            Percent = percent;
        }

        public DateTime Date { get; private set; }
        public decimal Percent { get; private set; }
    }

    private decimal GetTotalProgress()
    {
        return _progressions.Sum(p => p.Percent);
    }
}
