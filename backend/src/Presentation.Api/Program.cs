using Application.Interfaces;
using Application.Interfaces.Persistence;
using Infrastructure.Repositories;
using Application.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

builder.Services.AddSingleton<ITodoListRepository, InMemoryTodoListRepository>();
builder.Services.AddSingleton<ITodoList, TodoListService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();

app.UseCors("AllowVueFrontend");

app.UseAuthorization();

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Problem(
        title: exception?.Message,
        statusCode: exception is ArgumentException ? 400 : 500);
});

app.MapControllers();

app.Run();