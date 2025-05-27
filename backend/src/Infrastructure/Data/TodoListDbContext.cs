using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.Data
{
    public class TodoListDbContext : DbContext
    {
        public TodoListDbContext(DbContextOptions<TodoListDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasConversion(
                        id => id.Value,
                        value => TodoItemId.From(value))
                    .HasColumnName("Id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.ItemCategory)
                    .HasConversion<string>()
                    .IsRequired()
                    .HasColumnName("Category");

                entity.OwnsMany(e => e.Progressions, progression =>
                {
                    progression.WithOwner().HasForeignKey("TodoItemId");

                    progression.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    progression.HasKey("Id");

                    progression.Property(p => p.Date)
                        .IsRequired()
                        .HasColumnName("Date");

                    progression.Property(p => p.Percent)
                        .IsRequired()
                        .HasColumnType("decimal(5,2)")
                        .HasColumnName("Percent");

                    progression.ToTable("Progressions");
                });

                entity.Ignore(e => e.IsCompleted);
            });
        }
    }
}
