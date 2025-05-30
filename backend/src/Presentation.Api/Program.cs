using Application.Interfaces;
using Application.Interfaces.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<TodoListDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

builder.Services.AddScoped<ITodoListRepository, SqlTodoListRepository>();
builder.Services.AddScoped<ITodoList, SqlTodoListService>();

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