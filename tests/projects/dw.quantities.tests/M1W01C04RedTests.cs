using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C04RedTests
{
    [TestMethod]
    public void ExactComparisonTypesAreAbsentBeforeMathTransfer()
    {
        var names = typeof(ExactRational).Assembly.GetTypes().Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
        Assert.IsTrue(names.Contains("ApproximateEquivalence") &&
            names.Contains("ExactQuantityComparison"),
            "M1-W01-C04-T1 RED: imported exact comparison and quantity comparison contracts are absent.");
    }

    [TestMethod]
    public void TypedTemperatureToleranceIsAbsentBeforeImplementation()
    {
        var names = typeof(ExactRational).Assembly.GetTypes().Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
        Assert.IsTrue(names.Contains("ExactTemperatureComparison"),
            "M1-W01-C04-T2 RED: tagged temperature comparison contract is absent.");
    }
}
