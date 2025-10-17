using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Todo.Application.Features.Tasks.Interfaces;
using Todo.Application.Features.Tasks.Validators;
using Todo.Application.Features.Tasks.Services;
using Todo.Infrastructure.Persistence.Db;
using Todo.Infrastructure.Repositories;
using Todo.Application.Abstractions.Persistence;
using Todo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Todo.Api.Filters.ValidationExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for frontend dev (Angular at http://localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// EF Core - Npgsql
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

// FluentValidation - register validators from Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();

// Repositories & Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<TodoDbContext>>();
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
