using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C01RedTests
{
    [TestMethod]
    public void ProviderNeutralStatisticsMustExposeAnExplicitFiniteAggregateContract()
    {
        var assembly=System.Reflection.Assembly.Load("dw.tools.math.statistics");
        Assert.IsNotNull(
            assembly.GetType("Dw.Tools.Math.Statistics.DescriptiveStatistics64"),
            "M3-W02-C01-T1 RED: exact and finite descriptive statistics contract is missing.");
    }
}
