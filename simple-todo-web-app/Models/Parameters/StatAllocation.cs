using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models.Parameters
{
	public record StatAllocation
	{
		/// <summary>
		/// HP
		/// </summary>
		[Range(0, int.MaxValue)]
		public int HP { get; init; }
		/// <summary>
		/// MP
		/// </summary>
		[Range(0, int.MaxValue)]
		public int MP { get; init; }
		/// <summary>
		/// 攻撃力
		/// </summary>
		[Range(0, int.MaxValue)]
		public int ATK { get; init; }
		/// <summary>
		/// 防御力
		/// </summary>
		[Range(0, int.MaxValue)]
		public int DEF { get; init; }
		/// <summary>
		/// 速度
		/// </summary>
		[Range(0, int.MaxValue)]
		public int SPD { get; init; }
		/// <summary>
		/// 魔法攻撃力
		/// </summary>
		[Range(0, int.MaxValue)]
		public int MATK { get; init; }
		/// <summary>
		/// 運動ポイントの消費量 = HP + ATK
		/// </summary>
		public int ExercisePointsCost => HP + ATK;
		/// <summary>
		/// 勉強ポイントの消費量 = MP + MATK
		/// </summary>
		public int StudyPointsCost => MP + MATK;
		/// <summary>
		/// 家事ポイントの消費量 = DEF + SPD
		/// </summary>
		public int HouseworkPointsCost => DEF + SPD;

		public StatAllocation(int hp, int mp, int atk, int def, int spd, int matk)
		{
			HP = hp;
			MP = mp;
			ATK = atk;
			DEF = def;
			SPD = spd;
			MATK = matk;
		}
	}
}
