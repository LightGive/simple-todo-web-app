using simple_todo_web_app.Common.Utilities;
using simple_todo_web_app.Models.Entities;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models.Entities;

public class ToDoTaskTests
{
    private static ToDoTask CreateTask(string userId = "user-1", TaskCategory category = TaskCategory.Exercise)
    {
        // テスト用タスクを生成するヘルパー
        var taskNameSet = new TaskNameCategorySet(category, new TaskName("筋トレ"));
        return new ToDoTask(userId, taskNameSet);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // コンストラクタで UserId / Category / TaskName が正しく設定される
        var taskName = new TaskName("筋トレ");
        var taskNameSet = new TaskNameCategorySet(TaskCategory.Exercise, taskName);

        var task = new ToDoTask("user-1", taskNameSet);

        Assert.Equal("user-1", task.UserId);
        Assert.Equal(TaskCategory.Exercise, task.Category);
        Assert.Equal("筋トレ", task.TaskName);
        Assert.Null(task.LastCompletedDate);
        Assert.False(task.IsDeleted);
    }

    [Fact]
    public void IsCompletedToday_WhenLastCompletedDateIsNull_ReturnsFalse()
    {
        // LastCompletedDate が null の場合 false を返す
        var task = CreateTask();

        Assert.False(task.IsCompletedToday);
    }

    [Fact]
    public void IsCompletedToday_WhenCompletedToday_ReturnsTrue()
    {
        // 本日完了済みの場合 true を返す
        var task = CreateTask();
        task.CompleteTask();

        Assert.True(task.IsCompletedToday);
    }

    [Fact]
    public void IsCompletedToday_WhenCompletedYesterday_ReturnsFalse()
    {
        // 昨日（JST）完了済みの場合 false を返す
        var task = CreateTask();
        var yesterday = DateTimeUtility.UtcToJstDate(DateTime.UtcNow).AddDays(-1);
        // private setter にリフレクションで昨日の日付を設定する
        typeof(ToDoTask)
            .GetProperty(nameof(ToDoTask.LastCompletedDate))!
            .SetValue(task, yesterday);

        Assert.False(task.IsCompletedToday);
    }

    [Fact]
    public void CompleteTask_SetsLastCompletedDateToToday()
    {
        // CompleteTask 呼び出し後、LastCompletedDate が本日の JST 日付になる
        var task = CreateTask();
        var expectedDate = DateTimeUtility.UtcToJstDate(DateTime.UtcNow);

        task.CompleteTask();

        Assert.Equal(expectedDate, task.LastCompletedDate);
    }

    [Fact]
    public void DeleteTask_SetsIsDeletedToTrue()
    {
        // DeleteTask 呼び出し後、IsDeleted が true になる
        var task = CreateTask();

        task.DeleteTask();

        Assert.True(task.IsDeleted);
    }
}
