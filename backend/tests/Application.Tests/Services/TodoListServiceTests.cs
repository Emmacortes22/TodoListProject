using Application.Services;
using Infrastructure.Repositories;

namespace Application.Tests.Services;

public class TodoListServiceTests
{
    private readonly TodoListService _service = new(new InMemoryTodoListRepository());

    [Fact]
    public void AddItem_ShouldAdd_WhenCategoryIsValid()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Trabajo");

        // Assert
        Assert.True(_service.HasItems());
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenCategoryIsInvalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.AddItem(0, "Test Item", "Test Description", "InvalidCategory")
        );
    }

    [Fact]
    public void UpdateItem_ShouldChangeDescription_WhenItemExists()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Hogar");

        // Act
        _service.UpdateItem(1, "New Description");

        // Assert
        Assert.Contains("New Description", _service.GetItemById(1).Description);
    }

    [Fact]
    public void UpdateItem_ShouldThrowException_WhenTotalProgressIsGreaterThan50()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Estudio");
        _service.RegisterProgression(1, DateTime.Now, 60m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.UpdateItem(1, "New Desc")
        );
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenTotalProgressIsLessOrEqualTo50Percent()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Ocio");

        // Act
        _service.RemoveItem(1);

        // Assert
        Assert.False(_service.HasItems());
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenTotalProgressIsGreaterThan50Percent()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Salud");
        _service.RegisterProgression(1, DateTime.Now, 60m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.RemoveItem(1)
        );
    }

    [Fact]
    public void RegisterProgression_ShouldAddProgression_WhenDateAndPercentAreValid()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Trabajo");

        // Act
        _service.RegisterProgression(1, DateTime.Now.AddDays(-1), 30m);
        _service.RegisterProgression(1, DateTime.Now, 20m);

        // Assert
        var item = _service.GetItemById(1);
        Assert.Equal(2, item.Progressions.Count);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void RegisterProgression_ShouldThrowException_WhenPercentageIsInvalid()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Otro");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.RegisterProgression(1, DateTime.Now, 0m)
        );
    }

    [Fact]
    public void RegisterProgression_ShouldThrowException_WhenDateIsBeforeExistingProgressions()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Hogar");
        _service.RegisterProgression(1, DateTime.Now, 10m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.RegisterProgression(1, DateTime.Now.AddDays(-1), 10m)
        );
    }

    [Fact]
    public void RegisterProgression_ShouldThrowException_WhenTotalPercentExceeds100Percent()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Trabajo");
        _service.RegisterProgression(1, DateTime.Now, 60m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.RegisterProgression(1, DateTime.Now, 50m)
        );
    }
    
    [Fact]
    public void RegisterProgression_ShouldThrowException_WhenItemIsCompleted()
    {
        // Arrange
        _service.AddItem(0, "Test Item", "Test Description", "Trabajo");
        _service.RegisterProgression(1, DateTime.Now, 60m);
        _service.RegisterProgression(1, DateTime.Now.AddDays(1), 40m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.RegisterProgression(1, DateTime.Now.AddDays(2), 10m));
        Assert.Contains("La tarea esta completada, no se puede añadir más progresos", exception.Message);
    }
}
