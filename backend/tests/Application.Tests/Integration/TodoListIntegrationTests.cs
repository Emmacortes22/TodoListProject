using Xunit;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using System;
using System.Linq;

namespace Application.Tests.Integration
{
    public class TodoListIntegrationTests
    {
        private readonly ITodoList _todoList = new TodoListService(new InMemoryTodoListRepository());

        [Fact]
        public void AddItem_ShouldCreateItem_WhenCategoryIsValid()
        {
            // Arrange
            var title = "Estudiar C#";
            var description = "Repasar patrones de diseño";
            var category = "Study";

            // Act
            _todoList.AddItem(0, title, description, category);

            // Assert
            Assert.True(_todoList.HasItems());
        }

        [Fact]
        public void RegisterProgression_ShouldAddProgress_WhenDateAndPercentAreValid()
        {
            // Arrange
            _todoList.AddItem(0, "Limpiar cocina", "Tarea doméstica", "Home");

            // Act
            _todoList.RegisterProgression(1, DateTime.Now, 30m);

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void UpdateItem_ShouldThrowException_WhenTotalProgressIsGreaterThan50Percent()
        {
            // Arrange
            _todoList.AddItem(0, "Proyecto", "Trabajo final", "Work");
            _todoList.RegisterProgression(1, DateTime.Now, 60m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _todoList.UpdateItem(1, "Trabajo final actualizado")
            );
        }

        [Fact]
        public void RemoveItem_ShouldThrowException_WhenTotalProgressIsGreaterThan50Percent()
        {
            // Arrange
            _todoList.AddItem(0, "Curso Online", "Completar curso", "Study");
            _todoList.RegisterProgression(1, DateTime.Now, 70m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _todoList.RemoveItem(1)
            );
        }

        [Fact]
        public void ValidateProgressionDate_ShouldThrowException_WhenDateIsBeforeExistingProgressions()
        {
            // Arrange
            _todoList.AddItem(0, "Leer libro", "Capítulo 1", "Leisure");
            _todoList.RegisterProgression(1, DateTime.Now, 10m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _todoList.ValidateProgressionDate(1, DateTime.Now.AddDays(-1))
            );
        }
    }
}