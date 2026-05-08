using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models.Parameters;

public class TaskNameCategorySetTests
{
    [Fact]
    public void Constructor_WithValidCategoryAndTaskName_CreatesSuccessfully()
    {
        // 有効なカテゴリとタスク名を渡すと正常に作成される
        var taskName = new TaskName("筋トレ");

        var set = new TaskNameCategorySet(TaskCategory.Exercise, taskName);

        Assert.Equal(TaskCategory.Exercise, set.Category);
        Assert.Equal(taskName, set.TaskName);
    }

    [Fact]
    public void Constructor_WithNullTaskName_ThrowsArgumentException()
    {
        // TaskName が null の場合 ArgumentException が発生する
        Assert.Throws<ArgumentException>(() => new TaskNameCategorySet(TaskCategory.Exercise, null!));
    }
}
