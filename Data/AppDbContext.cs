using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }


    public DbSet<User> Logins { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
}
