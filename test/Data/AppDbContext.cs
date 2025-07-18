using Microsoft.EntityFrameworkCore;
using test.Models;

public class AppDbContext : DbContext
{
    public DbSet<MessageLog> messagelogs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
