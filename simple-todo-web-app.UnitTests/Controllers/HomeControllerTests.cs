using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using simple_todo_web_app.Controllers;
using simple_todo_web_app.Data;
using simple_todo_web_app.Models;
using simple_todo_web_app.Models.Entities;
using simple_todo_web_app.Models.Enums;
using simple_todo_web_app.Models.Parameters;
using System.Security.Claims;

namespace simple_todo_web_app.UnitTests.Controllers;

public class HomeControllerTests
{
    private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        // UserManager のモックを生成するヘルパー（省略可能なパラメーターは null で渡す）
        var store = new Mock<IUserStore<ApplicationUser>>();
#pragma warning disable CS8625
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
#pragma warning restore CS8625
    }

    private static ApplicationDbContext CreateInMemoryContext()
    {
        // インメモリ DB コンテキストを生成するヘルパー（テストごとに独立したDBを使用）
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static HomeController CreateController(
        Mock<UserManager<ApplicationUser>> userManagerMock,
        ApplicationDbContext context)
    {
        // コントローラーを生成するヘルパー
        var controller = new HomeController(userManagerMock.Object, context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    public class CompleteTaskTests
    {
        [Fact]
        public async Task CompleteTask_WhenUserIdIsNull_RedirectsToLogin()
        {
            // 未認証（UserIdがnull）の場合、ログイン画面へリダイレクトされる
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns((string?)null);
            var controller = CreateController(userManagerMock, CreateInMemoryContext());

            var result = await controller.CompleteTask(1) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact]
        public async Task CompleteTask_WhenTaskNotFound_RedirectsToHome()
        {
            // 存在しないタスクIDを指定した場合、ホームへリダイレクトされる
            const string userId = "user-1";
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            var context = CreateInMemoryContext();
            context.UnallocatedPoints.Add(new UnallocatedPoints(userId));
            await context.SaveChangesAsync();
            var controller = CreateController(userManagerMock, context);

            var result = await controller.CompleteTask(999) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.ActionName);
        }

        [Fact]
        public async Task CompleteTask_WhenAlreadyCompletedToday_RedirectsToHome()
        {
            // 本日既に完了済みのタスクを完了しようとすると、ホームへリダイレクトされる
            const string userId = "user-1";
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            var context = CreateInMemoryContext();
            var taskNameSet = new TaskNameCategorySet(TaskCategory.Exercise, new TaskName("筋トレ"));
            var task = new ToDoTask(userId, taskNameSet);
            task.CompleteTask(); // 本日完了済みにする
            context.Tasks.Add(task);
            context.UnallocatedPoints.Add(new UnallocatedPoints(userId));
            await context.SaveChangesAsync();
            var controller = CreateController(userManagerMock, context);

            var result = await controller.CompleteTask(task.TaskId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.ActionName);
        }

        [Fact]
        public async Task CompleteTask_WhenExerciseTaskCompleted_IncreasesExercisePoints()
        {
            // 運動カテゴリのタスクを正常完了すると ExercisePoints が +1 されホームへリダイレクトされる
            const string userId = "user-1";
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            var context = CreateInMemoryContext();
            var taskNameSet = new TaskNameCategorySet(TaskCategory.Exercise, new TaskName("筋トレ"));
            var task = new ToDoTask(userId, taskNameSet);
            var points = new UnallocatedPoints(userId);
            context.Tasks.Add(task);
            context.UnallocatedPoints.Add(points);
            await context.SaveChangesAsync();
            var controller = CreateController(userManagerMock, context);

            var result = await controller.CompleteTask(task.TaskId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Home", result.ActionName);
            var updatedPoints = await context.UnallocatedPoints.FirstAsync(p => p.UserId == userId);
            Assert.Equal(1, updatedPoints.ExercisePoints);
        }
    }

    public class AllocateTests
    {
        [Fact]
        public async Task Allocate_WithInvalidModelState_ReturnsBadRequest()
        {
            // ModelState が無効（負の値など）の場合、400 Bad Request が返る
            var userManagerMock = CreateMockUserManager();
            var controller = CreateController(userManagerMock, CreateInMemoryContext());
            controller.ModelState.AddModelError("HP", "0以上の値を入力してください。");

            var result = await controller.Allocate(new StatAllocation(hp: -1, mp: 0, atk: 0, def: 0, spd: 0, matk: 0));

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Allocate_WhenPointsInsufficient_ReturnsBadRequest()
        {
            // ポイントが不足している場合、400 Bad Request が返る
            const string userId = "user-1";
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            var context = CreateInMemoryContext();
            // ExercisePoints = 0 のまま HP に 1 振り分けようとする
            context.UnallocatedPoints.Add(new UnallocatedPoints(userId));
            context.CharacterStats.Add(new CharacterStats(userId));
            await context.SaveChangesAsync();
            var controller = CreateController(userManagerMock, context);

            var result = await controller.Allocate(new StatAllocation(hp: 1, mp: 0, atk: 0, def: 0, spd: 0, matk: 0));

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Allocate_WithValidAllocation_ReturnsOk()
        {
            // 有効なポイント振り分けの場合、200 OK が返る
            const string userId = "user-1";
            var userManagerMock = CreateMockUserManager();
            userManagerMock
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(userId);
            var context = CreateInMemoryContext();
            var points = new UnallocatedPoints(userId);
            points.AddPoints(exercisePoints: 2, studyPoints: 0, houseworkPoints: 0);
            context.UnallocatedPoints.Add(points);
            context.CharacterStats.Add(new CharacterStats(userId));
            await context.SaveChangesAsync();
            var controller = CreateController(userManagerMock, context);

            // 運動ポイント 2 を HP と ATK に 1 ずつ振り分ける
            var result = await controller.Allocate(new StatAllocation(hp: 1, mp: 0, atk: 1, def: 0, spd: 0, matk: 0));

            Assert.IsType<OkResult>(result);
        }
    }
}
