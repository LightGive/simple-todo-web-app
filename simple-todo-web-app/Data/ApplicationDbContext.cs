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
	}
}