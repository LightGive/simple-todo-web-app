using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Models.Entities;
using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models.Entities;

public class CharacterStatsTests
{
    [Fact]
    public void Constructor_AllStatsAreInitialValue()
    {
        // コンストラクタ呼び出し後、全ステータスが初期値（10）になる
        var stats = new CharacterStats("user-1");

        Assert.Equal(CharacterConstants.InitialStatValue, stats.HP);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.MP);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.ATK);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.DEF);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.SPD);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.MATK);
    }

    [Fact]
    public void AddStatus_WithPositiveValues_IncreasesEachStat()
    {
        // 正の値を渡すと各ステータスが加算される
        var stats = new CharacterStats("user-1");
        var allocation = new StatAllocation(hp: 1, mp: 2, atk: 3, def: 4, spd: 5, matk: 6);

        stats.AddStatus(allocation);

        Assert.Equal(CharacterConstants.InitialStatValue + 1, stats.HP);
        Assert.Equal(CharacterConstants.InitialStatValue + 2, stats.MP);
        Assert.Equal(CharacterConstants.InitialStatValue + 3, stats.ATK);
        Assert.Equal(CharacterConstants.InitialStatValue + 4, stats.DEF);
        Assert.Equal(CharacterConstants.InitialStatValue + 5, stats.SPD);
        Assert.Equal(CharacterConstants.InitialStatValue + 6, stats.MATK);
    }

    [Fact]
    public void AddStatus_WithAllZero_NoChange()
    {
        // 全て 0 を渡すと変化なし
        var stats = new CharacterStats("user-1");
        var allocation = new StatAllocation(hp: 0, mp: 0, atk: 0, def: 0, spd: 0, matk: 0);

        stats.AddStatus(allocation);

        Assert.Equal(CharacterConstants.InitialStatValue, stats.HP);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.MP);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.ATK);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.DEF);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.SPD);
        Assert.Equal(CharacterConstants.InitialStatValue, stats.MATK);
    }
}
