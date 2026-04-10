using Microsoft.EntityFrameworkCore;
using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models.Parameters;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models.Entities
{
	/// <summary>
	/// キャラクターのステータス
	/// </summary>
	[Index(nameof(UserId), IsUnique = true)]
	public class CharacterStats 
	{
		/// <summary>
		/// 主キー
		/// </summary>
		[Key]
		public int CharacterStatId { get; private set; }

		/// <summary>
		/// 外部キー
		/// </summary>
		[Required]
		public string UserId { get; private set; } = string.Empty;

		/// <summary>
		/// HP
		/// </summary>
		public int HP { get; private set; }

		/// <summary>
		/// MP
		/// </summary>
		public int MP { get; private set; }

		/// <summary>
		/// 攻撃力
		/// </summary>
		public int ATK { get; private set; }

		/// <summary>
		/// 防御力
		/// </summary>
		public int DEF { get; private set; }

		/// <summary>
		/// 速度
		/// </summary>
		public int SPD { get; private set; }

		/// <summary>
		/// 魔法攻撃力
		/// </summary>
		public int MATK { get; private set; }

		public CharacterStats(string userId)
		{
			UserId = userId;
			HP = CharacterConstants.InitialStatValue;
			MP = CharacterConstants.InitialStatValue;
			ATK = CharacterConstants.InitialStatValue;
			DEF = CharacterConstants.InitialStatValue;
			SPD = CharacterConstants.InitialStatValue;
			MATK = CharacterConstants.InitialStatValue;
		}

		public void AddStatus(StatAllocation statAllocation)
		{
			HP += statAllocation.HP;
			MP += statAllocation.MP;
			ATK += statAllocation.ATK;
			DEF += statAllocation.DEF;
			SPD += statAllocation.SPD;
			MATK += statAllocation.MATK;
		}
	}
}