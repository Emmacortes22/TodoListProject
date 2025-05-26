using Application.Interfaces;
using Application.Interfaces.Persistence;

namespace Presentation.Console;

public class ConsoleUI
{
    private readonly ITodoList _todoList;
    private readonly ITodoListRepository _repository;

    public ConsoleUI(ITodoListRepository repository, ITodoList todoList)
    {
        _repository = repository;
        _todoList = todoList;
    }

    public void Run()
    {
        while (true)
        {
            try
            {
                System.Console.Clear();
                System.Console.WriteLine("=== Todo List Application ===");
                System.Console.WriteLine("1. Agregar tarea");
                System.Console.WriteLine("2. Actualizar descripción");
                System.Console.WriteLine("3. Eliminar tarea");
                System.Console.WriteLine("4. Registrar progreso");
                System.Console.WriteLine("5. Ver todas las tareas");
                System.Console.WriteLine("6. Salir");
                System.Console.Write("\nSeleccione una opción: ");

                if (!int.TryParse(System.Console.ReadLine(), out int option))
                {
                    ShowError("Opción inválida. Intente nuevamente.");
                    continue;
                }

                switch (option)
                {
                    case 1:
                        AddTodoItem();
                        break;
                    case 2:
                        UpdateTodoItem();
                        break;
                    case 3:
                        RemoveTodoItem();
                        break;
                    case 4:
                        RegisterProgression();
                        break;
                    case 5:
                        ViewItems();
                        break;
                    case 6:
                        return;
                    default:
                        ShowError("Opción inválida. Intente nuevamente.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    private static void ShowError(string message)
    {
        System.Console.WriteLine($"\nError: {message}\n");
        System.Console.WriteLine("Presione cualquier tecla para continuar...");
        System.Console.ReadKey();
    }

    private void ViewItems()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Lista de Tareas ===\n");

        if (!_todoList.HasItems())
        {
            ShowError("No hay tareas registradas. Debe crear al menos una tarea antes de ver la lista de tareas.");
            return;
        }

        _todoList.PrintItems();
        System.Console.WriteLine("\nPresione cualquier tecla para continuar...");
        System.Console.ReadKey();
    }

    private void AddTodoItem()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Agregar Nueva Tarea ===\n");
        
        string? title;
        do
        {
            System.Console.Write("Título: ");
            title = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                System.Console.WriteLine("Error: El título no puede estar vacío. Intente nuevamente.\n");
            }

        } while (string.IsNullOrWhiteSpace(title));
        
        string? description;
        do
        {
            System.Console.Write("Descripción: ");
            description = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                System.Console.WriteLine("Error: La descripción no puede estar vacía. Intente nuevamente.\n");
            }

        } while (string.IsNullOrWhiteSpace(description));
        
        var categories = _repository.GetAllCategories();
        int categoryIndex;
        do
        {
            System.Console.WriteLine("\nCategorías disponibles:");
            for (int i = 0; i < categories.Count; i++)
            {
                System.Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            System.Console.Write("\nSeleccione el número de la categoría: ");
            var input = System.Console.ReadLine();

            if (!int.TryParse(input, out categoryIndex) || categoryIndex <= 0 || categoryIndex > categories.Count)
            {
                System.Console.WriteLine("Error: Categoría inválida. Intente nuevamente.\n");
                categoryIndex = -1;
            }

        } while (categoryIndex == -1);
        
        var category = categories[categoryIndex - 1];
        _todoList.AddItem(0, title, description, category);
        System.Console.WriteLine("\nTarea agregada con éxito!");
        System.Console.WriteLine("Presione cualquier tecla para continuar...");
        System.Console.ReadKey();
    }

    private void UpdateTodoItem()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Actualizar Descripción ===\n");

        if (!_todoList.HasItems())
        {
            ShowError("No hay tareas registradas. Debe crear al menos una antes de actualizar una descripción.");
            return;
        }
        
        int id;
        bool validId = false;
        do
        {
            System.Console.Write("ID de la tarea: ");
            if (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.WriteLine("Error: ID inválido. Intente nuevamente.\n");
                continue;
            }

            try
            {
                _todoList.EnsureItemExists(id);
                validId = true;
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}. Intente nuevamente.\n");
            }
        } while (!validId);

        try
        {
            _todoList.ValidateUpdate(id);
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
            return;
        }

        string? newDescription;
        do
        {
            System.Console.Write("Nueva descripción: ");
            newDescription = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newDescription))
            {
                System.Console.WriteLine("Error: La descripción no puede estar vacía. Intente nuevamente.\n");
            }

        } while (string.IsNullOrWhiteSpace(newDescription));
        
        _todoList.UpdateItem(id, newDescription);
        System.Console.WriteLine("\nDescripción actualizada con éxito!");
        System.Console.WriteLine("Presione cualquier tecla para continuar...");
        System.Console.ReadKey();
    }

    private void RemoveTodoItem()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Eliminar Tarea ===\n");

        if (!_todoList.HasItems())
        {
            ShowError("No hay tareas registradas. Debe crear al menos una antes de eliminar una tarea.");
            return;
        }

        int id;
        bool validId = false;
        do
        {
            System.Console.Write("ID de la tarea: ");
            if (!int.TryParse(System.Console.ReadLine(), out id))
            {
                System.Console.WriteLine("Error: ID inválido. Intente nuevamente.\n");
                continue;
            }

            try
            {
                _todoList.EnsureItemExists(id);
                validId = true;
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}. Intente nuevamente.\n");
            }
        } while (!validId);

        try
        {
            _todoList.ValidateDelete(id);
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
            return;
        }
        
        _todoList.RemoveItem(id);
        System.Console.WriteLine("\nTarea eliminada con éxito!");
        System.Console.WriteLine("Presione cualquier tecla para continuar...");
        System.Console.ReadKey();
    }

    private void RegisterProgression()
    {
        System.Console.Clear();
        System.Console.WriteLine("=== Registrar Progreso ===\n");
        
        if (!_todoList.HasItems())
        {
            ShowError("No hay tareas registradas. Debe crear al menos una antes de registrar un progreso.");
            return;
        }

        System.Console.Write("ID de la tarea: ");
        if (!int.TryParse(System.Console.ReadLine(), out int id))
        {
            ShowError("ID inválido");
            return;
        }

        try
        {
            _todoList.EnsureItemExists(id);
            _todoList.ValidateIsCompleted(id);
        }
        catch (ArgumentException ex)
        {
            ShowError($"{ex.Message}");
            return;
        }
        
        decimal percent;
        bool validPercent = false;
        do
        {
            System.Console.Write("Porcentaje de progreso (1-99): ");
            if (!decimal.TryParse(System.Console.ReadLine(), out percent))
            {
                System.Console.WriteLine("Error: El porcentaje debe ser un número válido. Intente nuevamente.\n");
                continue;
            }

            try
            {
                _todoList.ValidateProgressionPercent(id, percent);
                validPercent = true;
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}. Intente nuevamente.\n");
            }
        } while (!validPercent);

        DateTime date;
        bool validDate = false;
        do
        {
            System.Console.Write("Fecha y hora (MM/dd/yyyy hh:mm AM/PM) [Enter para fecha actual]: ");
            var dateInput = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(dateInput))
            {
                date = DateTime.Now;
                validDate = true;
            }
            else if (!DateTime.TryParseExact(dateInput, "MM/dd/yyyy hh:mm tt", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out date))
            {
                System.Console.WriteLine("Error: Formato de fecha inválido. Use MM/dd/yyyy hh:mm AM/PM (ejemplo: 12/25/2024 02:30 PM). Intente nuevamente.\n");
                continue;
            }

            try
            {
                _todoList.ValidateProgressionDate(id, date);
                validDate = true;
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}. Intente nuevamente.\n");
            }
        } while (!validDate);

        try
        {
            _todoList.RegisterProgression(id, date, percent);
            System.Console.WriteLine("\nProgreso registrado con éxito!");
            System.Console.WriteLine("Presione cualquier tecla para continuar...");
            System.Console.ReadKey();
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
            return;
        }
    }
}
