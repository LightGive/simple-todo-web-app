using Microsoft.AspNetCore.Identity;
using simple_todo_web_app.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	public class ApplicationUser: IdentityUser
	{
		/// <summary>
		/// キャラクター名
		/// </summary>
		[Required]
		[MaxLength(CharacterConstants.NameMaxLength)]
		public string DisplayName { get; private set; }

		/// <summary>
		/// 初期設定が完了しているか
		/// </summary>
		public bool IsInit { get; private set; }

		public ApplicationUser(): base()
		{
			DisplayName = string.Empty;
			IsInit = false;
		}
	}
}
