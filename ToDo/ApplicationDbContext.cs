using Microsoft.EntityFrameworkCore;
using ToDo.Models;

namespace ToDo
{
    public class ApplicationDbContext : DbContext
    {
        private const string ConnectionString =
            "Host=localhost;Port=54320;Username=Klara;Password=123;Database=Galieva";

        /// <summary>
        /// DbSet для операций над пользователями.
        /// </summary>
        public DbSet<ToDoItem> ToDos { get; set; }


        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<ToDoItem>()
                .Property(u => u.Name)
                .HasDefaultValueSql("now()");

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(ConnectionString);
    }
}
