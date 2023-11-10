using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

public class BlogDbContext : DbContext
{
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=BlogDb;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
modelBuilder.Entity<BlogPost>().HasMany(b => b.Tags);
        modelBuilder.Entity<BlogPost>().HasOne(b => b.Category);
        modelBuilder.Entity<BlogPost>()
            .Property(record => record.Id)
            .HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<Tag>()
            .Property(record => record.Id)
            .HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<Category>()
            .Property(record => record.Id)
            .HasDefaultValueSql("NEWID()");

       modelBuilder.Entity<Comment>()
            .Property(record => record.Id)
            .HasDefaultValueSql("NEWID()");
    }
}
