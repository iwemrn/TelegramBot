using Microsoft.EntityFrameworkCore;
using test.Models;

public class AppDbContext : DbContext
{
    public DbSet<MessageLog> MessageLogs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
