namespace simple_todo_web_app.Common.Utilities
{
	public class DateTimeUtility
	{
		static readonly TimeZoneInfo JstTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "Tokyo Standard Time" : "Asia/Tokyo");

		public static DateTime UtcToJst(DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, JstTimeZone);
		}

		public static DateOnly UtcToJstDate(DateTime utcDateTime)
		{
			return DateOnly.FromDateTime(UtcToJst(utcDateTime));
		}

		public static int CalcWeekNeeded(DateOnly startDate, int days)
		{
			if (days <= 0)
			{
				throw new ArgumentOutOfRangeException($"{nameof(days)}は1以上を指定して下さい");
			}
			return ((int)startDate.DayOfWeek + days + 6) / 7;
		}
	}
}
