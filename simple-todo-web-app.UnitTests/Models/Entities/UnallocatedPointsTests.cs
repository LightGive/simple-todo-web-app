using simple_todo_web_app.Models.Entities;

namespace simple_todo_web_app.UnitTests.Models.Entities;

public class UnallocatedPointsTests
{
    public class AddPointsTests
    {
        [Fact]
        public void AddPoints_WithPositiveValues_IncreasesEachPoint()
        {
            // 正の値を加算すると各ポイントが増加する
            var points = new UnallocatedPoints("user-1");

            points.AddPoints(exercisePoints: 1, studyPoints: 2, houseworkPoints: 3);

            Assert.Equal(1, points.ExercisePoints);
            Assert.Equal(2, points.StudyPoints);
            Assert.Equal(3, points.HouseworkPoints);
        }

        [Fact]
        public void AddPoints_WithZeroValues_NoChange()
        {
            // 0 を加算しても変化なし
            var points = new UnallocatedPoints("user-1");

            points.AddPoints(exercisePoints: 0, studyPoints: 0, houseworkPoints: 0);

            Assert.Equal(0, points.ExercisePoints);
            Assert.Equal(0, points.StudyPoints);
            Assert.Equal(0, points.HouseworkPoints);
        }

        [Fact]
        public void AddPoints_WithNegativeValue_ThrowsArgumentException()
        {
            // いずれかのポイントに負の値を渡すと ArgumentException が発生する
            var points = new UnallocatedPoints("user-1");

            Assert.Throws<ArgumentException>(() => points.AddPoints(exercisePoints: -1, studyPoints: 0, houseworkPoints: 0));
        }
    }

    public class UsePointsTests
    {
        [Fact]
        public void UsePoints_WithinAvailablePoints_DecreasesEachPoint()
        {
            // 所持ポイント内で消費すると各ポイントが減少する
            var points = new UnallocatedPoints("user-1");
            points.AddPoints(exercisePoints: 5, studyPoints: 3, houseworkPoints: 2);

            points.UsePoints(exercisePoints: 2, studyPoints: 1, houseworkPoints: 1);

            Assert.Equal(3, points.ExercisePoints);
            Assert.Equal(2, points.StudyPoints);
            Assert.Equal(1, points.HouseworkPoints);
        }

        [Fact]
        public void UsePoints_ExactlyAvailablePoints_BecomesZero()
        {
            // 所持ポイントと同値を消費すると 0 になる（境界値）
            var points = new UnallocatedPoints("user-1");
            points.AddPoints(exercisePoints: 3, studyPoints: 0, houseworkPoints: 0);

            points.UsePoints(exercisePoints: 3, studyPoints: 0, houseworkPoints: 0);

            Assert.Equal(0, points.ExercisePoints);
        }

        [Fact]
        public void UsePoints_ExceedingAvailablePoints_ThrowsInvalidOperationException()
        {
            // 所持ポイントを超えた消費は InvalidOperationException が発生する
            var points = new UnallocatedPoints("user-1");
            points.AddPoints(exercisePoints: 1, studyPoints: 0, houseworkPoints: 0);

            Assert.Throws<InvalidOperationException>(() => points.UsePoints(exercisePoints: 2, studyPoints: 0, houseworkPoints: 0));
        }

        [Fact]
        public void UsePoints_WithNegativeValue_ThrowsArgumentException()
        {
            // いずれかのポイントに負の値を渡すと ArgumentException が発生する
            var points = new UnallocatedPoints("user-1");

            Assert.Throws<ArgumentException>(() => points.UsePoints(exercisePoints: -1, studyPoints: 0, houseworkPoints: 0));
        }

        [Fact]
        public void UsePoints_WhenOnlySomeCategoryInsufficient_ThrowsInvalidOperationException()
        {
            // 一部カテゴリのみポイントが不足している場合 InvalidOperationException が発生する
            var points = new UnallocatedPoints("user-1");
            points.AddPoints(exercisePoints: 5, studyPoints: 0, houseworkPoints: 5);

            // 勉強ポイントのみ不足
            Assert.Throws<InvalidOperationException>(() => points.UsePoints(exercisePoints: 1, studyPoints: 1, houseworkPoints: 1));
        }
    }
}
