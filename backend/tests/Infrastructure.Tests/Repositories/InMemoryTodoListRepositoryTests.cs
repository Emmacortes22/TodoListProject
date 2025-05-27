using Infrastructure.Repositories;

namespace Infrastructure.Tests.Repositories;

public class InMemoryTodoListRepositoryTests
{
    [Fact]
    public void GetNextId_ShouldReturnIncrementalIds()
    {
        // Arrange
        var repository = new InMemoryTodoListRepository();

        var id1 = repository.GetNextId();
        var id2 = repository.GetNextId();
        var id3 = repository.GetNextId();

        // Act & Assert 
        Assert.Equal(1, id1);
        Assert.Equal(2, id2);
        Assert.Equal(3, id3);
    }

    [Fact]
    public void GetAllCategories_ShouldReturnAllValidCategories()
    {
        // Arrange
        var repository = new InMemoryTodoListRepository();

        // Act
        var categories = repository.GetAllCategories();

        // Assert
        Assert.Equal(6, categories.Count);
        Assert.Contains("Trabajo", categories);
        Assert.Contains("Hogar", categories);
        Assert.Contains("Salud", categories);
        Assert.Contains("Estudio", categories);
        Assert.Contains("Ocio", categories);
        Assert.Contains("Otro", categories);
    }
}
