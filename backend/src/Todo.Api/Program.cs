using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Todo.Application.Tasks;
using Todo.Infrastructure.Data;
using Todo.Infrastructure.Repositories;
using Todo.Application.Common.Persistence;
using Todo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core - Npgsql
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

// FluentValidation - register validators from Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();

// Repositories & Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<TodoDbContext>>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
