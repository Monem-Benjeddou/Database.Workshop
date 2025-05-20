using Database.Workshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Workshop.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}