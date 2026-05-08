using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models.Parameters;

public class TaskNameTests
{
    [Fact]
    public void Constructor_WithValidString_SetsTaskNameValue()
    {
        // 有効な文字列を渡すと TaskNameValue に設定される
        var taskName = new TaskName("勉強");

        Assert.Equal("勉強", taskName.TaskNameValue);
    }

    [Fact]
    public void Constructor_WithEmptyString_ThrowsArgumentException()
    {
        // 空文字を渡すと ArgumentException が発生する
        Assert.Throws<ArgumentException>(() => new TaskName(""));
    }

    [Fact]
    public void Constructor_WithWhitespaceOnly_ThrowsArgumentException()
    {
        // 空白のみを渡すと ArgumentException が発生する
        Assert.Throws<ArgumentException>(() => new TaskName("   "));
    }

    [Fact]
    public void Constructor_With100CharString_CreatesSuccessfully()
    {
        // 100文字（上限値）を渡すと正常に作成される
        var name = new string('a', 100);

        var taskName = new TaskName(name);

        Assert.Equal(name, taskName.TaskNameValue);
    }

    [Fact]
    public void Constructor_With101CharString_ThrowsArgumentException()
    {
        // 101文字を渡すと ArgumentException が発生する
        var name = new string('a', 101);

        Assert.Throws<ArgumentException>(() => new TaskName(name));
    }
}
