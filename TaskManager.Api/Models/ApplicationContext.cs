using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Models;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<ProjectAdmin> ProjectAdmins { get; set; }
    public DbSet<Desk> Desks { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Project configuration
        modelBuilder.Entity<Project>()
            .HasMany(p => p.AllUsers)
            .WithMany(u => u.Projects);

        // Task configuration
        modelBuilder.Entity<Task>()
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.Executor)
            .WithMany(u => u.ExecutedTasks)
            .HasForeignKey(t => t.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Desk configuration
        modelBuilder.Entity<Desk>()
            .HasOne(d => d.Admin)
            .WithMany(u => u.Desks)
            .HasForeignKey(d => d.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProjectAdmin configuration
        modelBuilder.Entity<ProjectAdmin>()
            .HasOne(pa => pa.User)
            .WithMany(u => u.AdminProjects)
            .HasForeignKey(pa => pa.UserId);
    }
}