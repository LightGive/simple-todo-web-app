using simple_todo_web_app.Models.Parameters;

namespace simple_todo_web_app.UnitTests.Models.Parameters;

public class StatAllocationTests
{
    [Fact]
    public void ExercisePointsCost_EqualsHpPlusAtk()
    {
        // ExercisePointsCost は HP + ATK の合計になる
        var allocation = new StatAllocation(hp: 3, mp: 0, atk: 2, def: 0, spd: 0, matk: 0);

        Assert.Equal(5, allocation.ExercisePointsCost);
    }

    [Fact]
    public void StudyPointsCost_EqualsMpPlusMatk()
    {
        // StudyPointsCost は MP + MATK の合計になる
        var allocation = new StatAllocation(hp: 0, mp: 2, atk: 0, def: 0, spd: 0, matk: 4);

        Assert.Equal(6, allocation.StudyPointsCost);
    }

    [Fact]
    public void HouseworkPointsCost_EqualsDefPlusSpd()
    {
        // HouseworkPointsCost は DEF + SPD の合計になる
        var allocation = new StatAllocation(hp: 0, mp: 0, atk: 0, def: 1, spd: 3, matk: 0);

        Assert.Equal(4, allocation.HouseworkPointsCost);
    }
}
