using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models;

public class ApplicationUserTests
{
    private static TaskNameCategorySet CreateTaskSet(TaskCategory category, string name = "テストタスク")
    {
        // テスト用のタスク名カテゴリセットを生成するヘルパー
        return new TaskNameCategorySet(category, new TaskName(name));
    }

    public class InitializeTests
    {
        [Fact]
        public void Initialize_WithValidInput_SetsAllProperties()
        {
            // 有効なキャラクター名とタスク名を渡すと IsInit / DisplayName / TaskList / CharacterStats / UnallocatedPoints が設定される
            var user = new ApplicationUser();
            var taskSet = CreateTaskSet(TaskCategory.Exercise);

            user.Initialize("勇者", taskSet);

            Assert.True(user.IsInit);
            Assert.Equal("勇者", user.DisplayName);
            Assert.Single(user.TaskList);
            Assert.NotNull(user.CharacterStats);
            Assert.NotNull(user.UnallocatedPoints);
        }

        [Fact]
        public void Initialize_WithEmptyDisplayName_ThrowsArgumentException()
        {
            // 空文字のキャラクター名を渡すと ArgumentException が発生する
            var user = new ApplicationUser();
            var taskSet = CreateTaskSet(TaskCategory.Exercise);

            Assert.Throws<ArgumentException>(() => user.Initialize("", taskSet));
        }

        [Fact]
        public void Initialize_WithWhitespaceDisplayName_ThrowsArgumentException()
        {
            // 空白のみのキャラクター名を渡すと ArgumentException が発生する
            var user = new ApplicationUser();
            var taskSet = CreateTaskSet(TaskCategory.Exercise);

            Assert.Throws<ArgumentException>(() => user.Initialize("   ", taskSet));
        }

        [Fact]
        public void Initialize_With32CharDisplayName_SetsSuccessfully()
        {
            // 32文字（上限値）のキャラクター名を渡すと正常に設定される
            var user = new ApplicationUser();
            var taskSet = CreateTaskSet(TaskCategory.Exercise);
            var name = new string('あ', CharacterConstants.NameMaxLength);

            user.Initialize(name, taskSet);

            Assert.Equal(name, user.DisplayName);
        }

        [Fact]
        public void Initialize_With33CharDisplayName_ThrowsArgumentException()
        {
            // 33文字以上のキャラクター名を渡すと ArgumentException が発生する
            var user = new ApplicationUser();
            var taskSet = CreateTaskSet(TaskCategory.Exercise);
            var name = new string('あ', CharacterConstants.NameMaxLength + 1);

            Assert.Throws<ArgumentException>(() => user.Initialize(name, taskSet));
        }

        [Fact]
        public void Initialize_WithMultipleTaskSets_AddsAllToTaskList()
        {
            // 複数のタスク名セットを渡すと TaskList に全件追加される
            var user = new ApplicationUser();
            var exerciseSet = CreateTaskSet(TaskCategory.Exercise, "筋トレ");
            var studySet = CreateTaskSet(TaskCategory.Study, "読書");
            var houseworkSet = CreateTaskSet(TaskCategory.Housework, "掃除");

            user.Initialize("勇者", exerciseSet, studySet, houseworkSet);

            Assert.Equal(3, user.TaskList.Count);
        }
    }
}
