using JWT.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT.Context;

public class DatabaseContext : DbContext
{
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
}