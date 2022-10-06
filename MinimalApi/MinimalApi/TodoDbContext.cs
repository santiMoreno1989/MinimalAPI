using Microsoft.EntityFrameworkCore;

namespace MinimalApi
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {

        }
        public DbSet<Todo> Todos => Set<Todo>();
    }
}
