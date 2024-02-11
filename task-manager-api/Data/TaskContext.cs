using Microsoft.EntityFrameworkCore;
using TaskAPI.Data.Entities;

namespace TaskAPI.Data;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options)
    {
    }
    
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TaskModel>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Status);
            e.Property(t => t.DueDate);
            e.Property(t => t.Priority);
            e.Property(t => t.Description);
            e.Property(t => t.Title).IsRequired(true);
        });
        
        builder.Entity<UserModel>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).IsRequired(true);
            e.Property(t => t.Email).IsRequired(true);
            e.Property(t => t.PasswordHash).IsRequired(true);
        });
    }
}