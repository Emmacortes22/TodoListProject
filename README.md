# TodoList Project

Este proyecto es una aplicación de gestión de tareas (Todo List) desarrollada con una arquitectura hexagonal, siguiendo buenas prácticas de DDD y separación de responsabilidades.

## Dos formas de usar la aplicación

La aplicación ofrece dos formas de interactuar con las tareas:

### 1. Modo Consola (In-Memory)

- Ejecuta una aplicación de consola donde los datos se almacenan temporalmente en memoria RAM.
- Útil para pruebas rápidas sin necesidad de base de datos.
- Implementación: `InMemoryTodoListRepository`
- Entrada desde teclado, resultados en consola.
- No requiere Docker ni SQL Server.

### 2. Modo Web (API REST + SQL Server)

- Interfaz Web (Frontend en Vue 3 + TypeScript) que se comunica con una Web API construida en ASP.NET Core.
- Los datos se guardan en una base de datos SQL Server contenida en Docker.
- Implementación: `SqlTodoListRepository`
- Permite persistencia real de los datos.
- Rutas disponibles para `GET`, `POST`, `PUT`, `DELETE`.

---

## Estructura del Proyecto

```bash
TodoListProject/
│
├── backend/
│   ├── src/
│   │   ├── Application/                  # Lógica de aplicación: servicios, interfaces, casos de uso
│   │   │   ├── Interfaces/
│   │   │   │   ├── Persistence/          # Interfaces para repositorios (persistencia)
│   │   │   │   │   └── ITodoListRepository.cs
│   │   │   │   └── ITodoList.cs          # Interface para los métodos del todo list (puertos)
│   │   │   └── Services/                 # Servicios de aplicación con lógica concreta
│   │   │       └── TodoItemService.cs
│   │   │       └── TodoListService.cs
│   │   │
│   │   ├── Domain/                       # Modelo de dominio: entidades, objetos valor, reglas de negocio
│   │   │   ├── Entities/                 # Entidades principales del dominio
│   │   │   │   └── TodoItem.cs
│   │   │   └── ValueObjects/             # Objetos valor para validaciones fuertes y encapsulación
│   │   │       └── TodoItemId.cs
│   │   │
│   │   ├── Infrastructure/               # Implementación de persistencia y repositorios
│   │   │   ├── Data/                     # Contexto de datos (Entity Framework, DbContext)
│   │   │   │   └── TodoListDbContext.cs
│   │   │   ├── Migrations/               # Migraciones de la base de datos EF Core
│   │   │   └── Repositories/             # Repositorios concretos
│   │   │       ├── InMemoryTodoListRepository.cs  # Repo para modo consola (RAM)
│   │   │       ├── SqlTodoListRepository.cs       # Repo para base de datos SQL Server
│   │   │       └── SqlTodoListService.cs          # Lógica de servicio con acceso a SQL Server
│   │   │
│   │   ├── Presentation.Api/              # Web API ASP.NET Core
│   │   │   └── Controllers/               # Controladores REST para exponer la API
│   │   │       └── TodoListController.cs
│   │   │
│   │   └── Presentation.Console/           # Aplicación consola para modo in-memory
│   │       └── ConsoleUI.cs                # Lógica y UI de consola
│   │
│   └── TodoList.sln                        # Solución .NET que agrupa todos los proyectos
│
├── frontend/                              
│   ├── src/
│   │   ├── api/                          # Lógica para llamadas a la Web API
│   │   │   ├── todoListApi.ts
│   │   │   └── types.ts                  # Tipos TypeScript para datos
│   │   ├── components/                   # Componentes Vue reutilizables
│   │   │   └── TodoList.vue
│   │   ├── App.vue                       # Componente raíz
│   │   └── style.css                     # Estilos globales
│   └── .env                             # Variables entorno para frontend
│
├── docker-compose.yml                   # Contenedor Docker para SQL Server y servicios
├── README.md                           
└── .gitignore                         
```

---

## Puesta en marcha

### Requisitos
- .NET 9 SDK  
- Docker  
- Node.js y npm (para el frontend)

### Consola

Ejecuta la aplicación consola (modo en memoria):
```bash
dotnet run --project Presentation.Console
```

### Backend + SQL Server

Inicia el contenedor Docker:
```bash
docker compose up -d
```

Aplica las migraciones y ejecuta la API:
```bash
dotnet ef database update --project Infrastructure
dotnet run --project Presentation.Api
```

### Frontend

Navega a la carpeta del frontend:
```bash
cd frontend
```

Instala dependencias y ejecuta el frontend:
```bash
npm install
npm run dev
```

---

## Endpoints Web API

| Método | Ruta                              | Descripción                             |
|--------|-----------------------------------|-----------------------------------------|
| GET    | /todo-list                        | Obtiene todas las tareas                |
| POST   | /todo-list                        | Crea una nueva tarea                    |
| PUT    | /todo-list/{id}                   | Actualiza la descripción                |
| DELETE | /todo-list/{id}                   | Elimina una tarea                       |
| POST   | /todo-list/{id}/progressions      | Crea una nueva progression para la tarea|

---

## Consideraciones Técnicas

- Diseño basado en Domain-Driven Design (DDD).
- Uso de Value Object `TodoItemId` para validaciones fuertes.
- Control de estado y progreso de cada tarea con la entidad `Progression`.
- Uso de Entity Framework Core con conversiones personalizadas para el modelo.

---

## Mejoras Futuras

1. Implementar autenticación y autorización para proteger la API y controlar accesos.
2. Extender pruebas automatizadas con pruebas de integración y end-to-end.
3. Añadir nuevas reglas de negocio como eliminar o modificar los progresos.
4. Mejoras en la UI/UX del frontend.

---

## Autora

Emma CM – Full-stack developer (Vue.js + .NET)
