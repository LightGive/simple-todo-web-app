namespace simple_todo_web_app.Common.Utilities
{
	public class DateTimeUtility
	{
		static readonly TimeZoneInfo JstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

		public static DateTime UtcToJst(DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, JstTimeZone);
		}

		public static DateOnly UtcToJstDate(DateTime utcDateTime)
		{
			return DateOnly.FromDateTime(UtcToJst(utcDateTime));
		}
	}
}
