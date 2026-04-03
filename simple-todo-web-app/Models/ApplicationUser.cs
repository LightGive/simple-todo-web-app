using Microsoft.AspNetCore.Identity;
using simple_todo_web_app.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class ApplicationUser: IdentityUser
	{
		[Required]
		[MaxLength(CharacterConstants.NameMaxLength)]
		public string DisplayName { get; private set; } = string.Empty;
	}
}
