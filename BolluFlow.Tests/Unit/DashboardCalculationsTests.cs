using BolluFlow.Application.Features.Dashboard;

namespace BolluFlow.Tests.Unit;

public class DashboardCalculationsTests
{
    [Theory]
    [InlineData(8, 10, "Critical")]
    [InlineData(10, 10, "Critical")]
    [InlineData(11, 10, "Warning")]
    public void GetSeverity_ShouldFollowRules(decimal stock, decimal minimum, string expected)
    {
        var result = DashboardCalculations.GetSeverity(stock, minimum);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(8, 10, 80)]
    [InlineData(12, 10, 120)]
    [InlineData(1, 3, 33.33)]
    public void GetCoveragePercent_ShouldCalculateRoundedPercentage(decimal stock, decimal minimum, decimal expected)
    {
        var result = DashboardCalculations.GetCoveragePercent(stock, minimum);

        Assert.Equal(expected, result);
    }
}
