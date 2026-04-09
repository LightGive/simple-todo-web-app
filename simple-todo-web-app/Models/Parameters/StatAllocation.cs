namespace simple_todo_web_app.Models.Parameters
{
	public record StatAllocation
	{
		/// <summary>
		/// HP
		/// </summary>
		public int HP { get; init; }
		/// <summary>
		/// MP
		/// </summary>
		public int MP { get; init; }
		/// <summary>
		/// 攻撃力
		/// </summary>
		public int ATK { get; init; }
		/// <summary>
		/// 防御力
		/// </summary>
		public int DEF { get; init; }
		/// <summary>
		/// 速度
		/// </summary>
		public int SPD { get; init; }
		/// <summary>
		/// 魔法攻撃力
		/// </summary>
		public int MATK { get; init; }
		public StatAllocation(int hp, int mp, int atk, int def, int spd, int matk)
		{
			if (hp < 0 || mp < 0 || atk < 0 || def < 0 || spd < 0 || matk < 0)
			{
				throw new ArgumentException("ステータスは負の値にできません。");
			}

			HP = hp;
			MP = mp;
			ATK = atk;
			DEF = def;
			SPD = spd;
			MATK = matk;
		}
	}
}
