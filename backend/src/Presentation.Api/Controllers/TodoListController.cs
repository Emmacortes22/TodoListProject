using Application.Interfaces;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/todo-list")]
public class TodoListController(ITodoList todoList, ITodoListRepository repository) : ControllerBase
{
    private readonly ITodoList _todoList = todoList;
    private readonly ITodoListRepository _repository = repository;

    [HttpPost]
    public IActionResult AddItem([FromBody] AddItemRequest request)
    {
        try
        {
            if (!_repository.GetAllCategories().Contains(request.Category))
                return BadRequest($"Categoría inválida: {request.Category}");

            _todoList.AddItem(request.Id, request.Title, request.Description, request.Category);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItem(int id, [FromBody] UpdateItemRequest request)
    {
        try
        {
            _todoList.UpdateItem(id, request.Description);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveItem(int id)
    {
        try
        {
            _todoList.RemoveItem(id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/progressions")]
    public IActionResult RegisterProgression(int id, [FromBody] RegisterProgressionRequest request)
    {
        try
        {
            _todoList.RegisterProgression(id, request.DateTime, request.Percent);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetAllItems()
    {
        try
        {
            var items = _todoList.GetItems()
                .Select(item => new TodoItemDto
                {
                    Id = item.Id.Value,
                    Title = item.Title,
                    Description = item.Description,
                    Category = item.ItemCategory.ToString(),
                    IsCompleted = item.IsCompleted,
                    Progressions = item.Progressions.Select(p => new ProgressionDto
                    {
                        Date = p.Date,
                        Percent = p.Percent
                    }).ToList()
                }).ToList();
                
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    public record AddItemRequest(int Id, string Title, string Description, string Category);
    public record UpdateItemRequest(string Description);
    public record RegisterProgressionRequest(DateTime DateTime, decimal Percent);
    
    public class TodoItemDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public bool IsCompleted { get; set; }
        public List<ProgressionDto> Progressions { get; set; } = [];
    }
    
    public class ProgressionDto
    {
        public required DateTime Date { get; set; }
        public required decimal Percent { get; set; }
    }
}