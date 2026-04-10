using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using simple_todo_web_app.Models;
using simple_todo_web_app.Models.Entities;

namespace simple_todo_web_app.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
		IdentityDbContext<ApplicationUser>(options)
	{
		public DbSet<CharacterStats> CharacterStats { get; set; }
		public DbSet<UnallocatedPoints> UnallocatedPoints { get; set; }
		public DbSet<ToDoTask> Tasks { get; set; }
		public DbSet<TaskCompletionLog> TaskCompletionLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>()
				.HasMany(u => u.TaskList)
				.WithOne()
				.HasForeignKey(t => t.UserId);

			builder.Entity<ApplicationUser>()
				.HasOne(u => u.CharacterStats)
				.WithOne()
				.HasForeignKey<CharacterStats>(c => c.UserId);

			builder.Entity<ApplicationUser>()
				.HasOne(u => u.UnallocatedPoints)
				.WithOne()
				.HasForeignKey<UnallocatedPoints>(u => u.UserId);
		}
	}
}