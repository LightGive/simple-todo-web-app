using Microsoft.AspNetCore.Identity;
using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models.Entities;
using simple_todo_web_app.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models
{
	/// <summary>
	/// IdentityUserを継承したアプリケーションユーザークラス
	/// </summary>
	public class ApplicationUser : IdentityUser
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

		/// <summary>
		/// キャラクターステータス
		/// </summary>
		public CharacterStats? CharacterStats { get; private set; }

		/// <summary>
		/// 未振り分けのステータスポイント
		/// </summary>
		public UnallocatedPoints? UnallocatedPoints { get; private set; }

		/// <summary>
		/// タスクのリスト
		/// </summary>
		public List<ToDoTask> TaskList { get; private set; }

		public ApplicationUser() : base()
		{
			DisplayName = string.Empty;
			TaskList = new();
		}

		/// <summary>
		/// 初期化終了時
		/// </summary>
		/// <param name="displayName"></param>
		public void Initialize(string displayName)
		{
			if (string.IsNullOrWhiteSpace(displayName))
			{
				throw new ArgumentException("キャラクター名は必須です。", nameof(displayName));
			}

			if (displayName.Length > CharacterConstants.NameMaxLength)
			{
				throw new ArgumentException($"キャラクター名は{CharacterConstants.NameMaxLength}文字以内で入力してください。", nameof(displayName));
			}

			DisplayName = displayName;
			IsInit = true;
			CharacterStats = new CharacterStats(Id);
			UnallocatedPoints = new UnallocatedPoints(Id);

			TaskList.Add(new ToDoTask(Id, TaskCategory.Exercise));
			TaskList.Add(new ToDoTask(Id, TaskCategory.Study));
			TaskList.Add(new ToDoTask(Id, TaskCategory.Housework));
		}
	}
}
