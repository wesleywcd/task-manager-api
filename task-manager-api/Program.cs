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
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        b =>
        {
            b.WithOrigins(builder.Configuration.GetSection("AllowedHosts").Value)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<LogsMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicy");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();