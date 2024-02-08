using Microsoft.EntityFrameworkCore;
using TaskAPI.Data;
using TaskAPI.Mapper;
using TaskAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskContext>(o => o.UseInMemoryDatabase("TasksDB"));
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<LogsMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();