using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class TodoItemTests
{
    [Fact]
    public void AddProgression_ShouldAdd_WhenDateAndPercentAreValid()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        // Act
        item.AddProgression(DateTime.Now, 50m);

        // Assert
        Assert.Single(item.Progressions);
        Assert.Equal(50m, item.Progressions[0].Percent);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_ShouldThrowException_WhenTotalPercentExceeds100Percent()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        item.AddProgression(DateTime.Now, 60m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.AddProgression(DateTime.Now, 50m));
        Assert.Contains("El porcentaje de progreso no puede exceder el 100%. Total actual: 60%", exception.Message);
    }

    [Fact]
    public void AddProgression_ShouldThrowException_WhenPercentageIsInvalid()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.AddProgression(DateTime.Now, 120m));
        Assert.Contains("El porcentaje debe ser mayor a 0 y menor a 100", exception.Message);
    }

    [Fact]
    public void AddProgression_ShouldThrowException_WhenDateIsBeforeExistingProgressions()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        item.AddProgression(new DateTime(2025, 1, 2, 10, 0, 0), 40m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.AddProgression(new DateTime(2025, 1, 1, 10, 0, 0), 50m));
        Assert.Contains("La fecha del progreso no puede ser anterior o igual a la fecha de la última progresión. Última fecha: 01/02/2025 10:00 AM", exception.Message);
    }

    [Fact]
    public void IsCompleted_ShouldBeTrue_WhenAllProgressionsSumTo100Percent()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        // Add progressions
        item.AddProgression(DateTime.Now.AddDays(-1), 50m);
        item.AddProgression(DateTime.Now, 50m);

        // Assert
        Assert.True(item.IsCompleted);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdate_WhenTotalProgressIsLessOrEqualTo50Percent()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );
        item.AddProgression(DateTime.Now, 40m);

        // Act
        item.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", item.Description);
    }

    [Fact]
    public void ValidateUpdate_ShouldThrowException_WhenTotalProgressIsGreaterThan50Percent()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );
        item.AddProgression(DateTime.Now, 60m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.ValidateUpdate());
        Assert.Contains("No se puede actualizar una tarea con un progreso superior a 50%", exception.Message);
    }

    [Fact]
    public void ValidateDelete_ShouldThrowException_WhenTotalProgressIsGreaterThan50Percent()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        item.AddProgression(DateTime.Now, 60m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.ValidateDelete());
        Assert.Contains("No se puede eliminar una tarea con un progreso superior a 50%", exception.Message);
    }

    [Fact]
    public void ValidateIsCompleted_ShouldThrowException_WhenItemIsCompleted()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );
        item.AddProgression(DateTime.Now, 60m);
        item.AddProgression(DateTime.Now.AddDays(1), 40m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => item.AddProgression(DateTime.Now.AddDays(2), 10m));
        Assert.Contains("La tarea esta completada, no se puede añadir más progresos", exception.Message);
    }
}