using Domain.Entities;
using Domain.ValueObjects;
using Application.Services;

namespace Application.Tests.Services;

public class TodoItemServiceTests
{
    private readonly TodoItemService _service = new();

    [Fact]
    public void CreateTodoItem_ShouldCreateValidItem()
    {
        // Arrange
        var id = TodoItemId.From(1);
        var title = "Test Item";
        var description = "Test Description";
        var category = 1;

        // Act
        var item = _service.CreateTodoItem(id, title, description, category);

        // Assert
        Assert.Equal(id, item.Id);
        Assert.Equal(title, item.Title);
        Assert.Equal(description, item.Description);
        Assert.Equal(category, (int)item.ItemCategory);
        Assert.Empty(item.Progressions);
    }

    [Fact]
    public void FormatProgressBar_ShouldReturnCorrectFormat()
    {
        // Arrange
        var progress = 60m;
        var expectedWidth = 50;
        var expectedFilled = 30;

        // Act
        var result = _service.FormatProgressBar(progress);

        // Assert
        Assert.Equal(expectedWidth, result.Length);
        Assert.Equal(expectedFilled, result.Count(c => c == 'O'));
        Assert.Equal(expectedWidth - expectedFilled, result.Count(c => c == ' '));
    }

    [Fact]
    public void FormatItemHeader_ShouldReturnCorrectFormat()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );

        // Act
        var result = _service.FormatItemHeader(item);

        // Assert
        Assert.Contains(item.Id.Value.ToString(), result);
        Assert.Contains(item.Title, result);
        Assert.Contains(item.Description, result);
        Assert.Contains(item.ItemCategory.ToString(), result);
        Assert.Contains("Completed:False", result);
    }

    [Fact]
    public void FormatProgression_ShouldReturnCorrectFormat()
    {
        // Arrange
        var item = new TodoItem(
            TodoItemId.From(1),
            "Test Item",
            "Test Description",
            1
        );
        var date = new DateTime(2025, 1, 1, 10, 30, 0);
        item.AddProgression(date, 30m);
        var progression = item.Progressions[0];

        // Act
        var result = _service.FormatProgression(progression, 30m);
        var culture = new System.Globalization.CultureInfo("en-US");

        // Assert
        Assert.Contains(date.ToString("MM/dd/yyyy hh:mm:ss tt", culture), result);
        Assert.Contains("30%", result);
        Assert.Contains("|", result);
        Assert.Contains("O", result);
    }
} 