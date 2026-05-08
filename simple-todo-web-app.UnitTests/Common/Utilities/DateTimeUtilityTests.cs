using simple_todo_web_app.Common.Utilities;

namespace simple_todo_web_app.UnitTests.Common.Utilities;

public class DateTimeUtilityTests
{
    [Fact]
    public void UtcToJst_Utc1500_ReturnsJstMidnight()
    {
        // UTC 15:00 は JST 翌日 00:00 になる（日またぎ）
        var utc = new DateTime(2026, 5, 8, 15, 0, 0, DateTimeKind.Utc);

        var result = DateTimeUtility.UtcToJst(utc);

        Assert.Equal(new DateTime(2026, 5, 9, 0, 0, 0), result);
    }

    [Fact]
    public void UtcToJstDate_Utc1459_ReturnsSameDateAsUtc()
    {
        // UTC 14:59（JST 23:59）の場合、UTC と同じ日付を返す
        var utc = new DateTime(2026, 5, 8, 14, 59, 0, DateTimeKind.Utc);

        var result = DateTimeUtility.UtcToJstDate(utc);

        Assert.Equal(new DateOnly(2026, 5, 8), result);
    }

    [Fact]
    public void UtcToJstDate_Utc1500_ReturnsOneDayAheadOfUtc()
    {
        // UTC 15:00（JST 00:00）の場合、UTC より 1 日進んだ日付を返す
        var utc = new DateTime(2026, 5, 8, 15, 0, 0, DateTimeKind.Utc);

        var result = DateTimeUtility.UtcToJstDate(utc);

        Assert.Equal(new DateOnly(2026, 5, 9), result);
    }

    [Theory]
    [InlineData(DayOfWeek.Sunday, 1, 1)]    // 日曜始まりで1日 → 1週
    [InlineData(DayOfWeek.Monday, 1, 1)]    // 月曜始まりで1日 → 1週
    [InlineData(DayOfWeek.Saturday, 1, 1)]  // 土曜始まりで1日 → 1週
    [InlineData(DayOfWeek.Sunday, 7, 1)]    // 日曜始まりで7日 → 1週
    [InlineData(DayOfWeek.Monday, 7, 2)]    // 月曜始まりで7日 → 2週
    [InlineData(DayOfWeek.Saturday, 2, 2)]  // 土曜始まりで2日 → 2週
    public void CalcWeekNeeded_VariousStartDaysAndDays_ReturnsCorrectWeekCount(
        DayOfWeek startDayOfWeek, int days, int expectedWeeks)
    {
        // 各曜日・日数のパターンで正しい週数が返る
        // 指定曜日の日付を検索する
        var date = DateOnly.FromDateTime(DateTime.Today);
        while (date.DayOfWeek != startDayOfWeek)
        {
            date = date.AddDays(1);
        }

        var result = DateTimeUtility.CalcWeekNeeded(date, days);

        Assert.Equal(expectedWeeks, result);
    }

    [Fact]
    public void CalcWeekNeeded_WithZeroDays_ThrowsArgumentOutOfRangeException()
    {
        // days が 0 の場合 ArgumentOutOfRangeException が発生する
        var date = DateOnly.FromDateTime(DateTime.Today);

        Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeUtility.CalcWeekNeeded(date, 0));
    }

    [Fact]
    public void CalcWeekNeeded_WithNegativeDays_ThrowsArgumentOutOfRangeException()
    {
        // days が負の値の場合 ArgumentOutOfRangeException が発生する
        var date = DateOnly.FromDateTime(DateTime.Today);

        Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeUtility.CalcWeekNeeded(date, -1));
    }
}
