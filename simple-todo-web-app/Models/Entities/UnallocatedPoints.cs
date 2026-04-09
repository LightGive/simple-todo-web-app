using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace simple_todo_web_app.Models.Entities
{
	/// <summary>
	/// 未振り分けポイント
	/// </summary>
	[Index(nameof(UserId), IsUnique = true)]
	public class UnallocatedPoints
	{
		/// <summary>
		/// 主キー
		/// </summary>
		[Key]
		public int PointId { get; private set; }

		/// <summary>
		/// 外部キー
		/// </summary>
		[Required]
		public string UserId { get; private set; } = string.Empty;

		/// <summary>
		/// 運動ポイント
		/// </summary>
		public int ExercisePoints { get; private set; }

		/// <summary>
		/// 勉強ポイント
		/// </summary>
		public int StudyPoints { get; private set; }

		/// <summary>
		/// 家事ポイント
		/// </summary>
		public int HouseworkPoints { get; private set; }

		public UnallocatedPoints(string userId)
		{
			UserId = userId;
		}

		public void AddPoints(int exercisePoints, int studyPoints, int houseworkPoints)
		{
			if (exercisePoints < 0 || studyPoints < 0 || houseworkPoints < 0)
			{
				throw new ArgumentException("ポイントは負の値にできません。");
			}
			ExercisePoints += exercisePoints;
			StudyPoints += studyPoints;
			HouseworkPoints += houseworkPoints;
		}


		public void UsePoints(int exercisePoints, int studyPoints, int houseworkPoints)
		{
			if (exercisePoints < 0 || studyPoints < 0 || houseworkPoints < 0)
			{
				throw new ArgumentException("ポイントは負の値にできません。");
			}
			if (exercisePoints > ExercisePoints || studyPoints > StudyPoints || houseworkPoints > HouseworkPoints)
			{
				throw new InvalidOperationException("ポイントが不足しています。");
			}
			ExercisePoints -= exercisePoints;
			StudyPoints -= studyPoints;
			HouseworkPoints -= houseworkPoints;
		}
	}
}
