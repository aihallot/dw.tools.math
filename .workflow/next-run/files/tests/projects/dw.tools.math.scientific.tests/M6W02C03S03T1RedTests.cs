using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S03T1RedTests
{
    [TestMethod]
    public void UniformSeriesDifferencesAndSlidingMeansPublicContractMustExist()
    {
        var assembly = typeof(FiniteSignal64).Assembly;
        var series = assembly.GetType("Dw.Tools.Math.Scientific.FiniteUniformSeries64");
        Assert.IsNotNull(series,
            "M6-W02-C03-S03-T1 RED: bounded uniform-series first differences and sliding mean public API is absent.");
        Assert.IsNotNull(series!.GetMethod("FirstDifferences"));
        Assert.IsNotNull(series.GetMethod("MovingAverage"));
    }
}
