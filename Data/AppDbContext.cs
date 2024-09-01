using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		if (Database.GetPendingMigrations().Any())
			Database.Migrate();
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>().HasData(new Category() { Name = "Default", Description = "Default Category", Id = -1 });
		modelBuilder.Entity<Role>().HasData(new Role() { Name = "Author", Description = "Basic logged in user", IsAdmin = false, Id = -1 },
		new Role() { Name = "Admin", Description = "The bo$$", IsAdmin = true, Id = -2 });


		base.OnModelCreating(modelBuilder);
	}

	public DbSet<TagBlogpostMapping> TagBlogpostMappings { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Tag> Tags { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<BlogPost> BlogPosts { get; set; }
}
