using Domain.Entities;

namespace Application.Interfaces;

public interface ITodoList
{
    // Agrega un nuevo item a la lista
    void AddItem(int id, string title, string description, string category);
    // Actualiza la descripción de un item existente
    void UpdateItem(int id, string description);
    // Elimina un item de la lista
    void RemoveItem(int id);
    // Registra un nuevo progreso para un item
    void RegisterProgression(int id, DateTime dateTime, decimal percent);
    // Imprime todos los items de la lista
    void PrintItems();
    // Obtiene todos los items de la lista
    IEnumerable<TodoItem> GetItems();
    // Verifica si hay items en la lista
    bool HasItems();
    // Verifica si existe un item con el id especificado
    void EnsureItemExists(int id);
    // Obtiene el item a partir de su ID
    TodoItem GetItemById(int id);
    // Valida si la tarea esta completada
    void ValidateIsCompleted(int id);
    // Valida el porcentaje de progreso
    void ValidateProgressionPercent(int id, decimal percent);
    // Valida la fecha del progreso
    void ValidateProgressionDate(int id, DateTime date);
    // Valida si un item puede ser actualizado
    void ValidateUpdate(int id);
    // Valida si un item puede ser eliminado
    void ValidateDelete(int id);
} 