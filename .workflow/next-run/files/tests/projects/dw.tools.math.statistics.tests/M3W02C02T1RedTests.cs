using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C02T1RedTests
{
    [TestMethod]
    public void ConventionedQuantilesAndPairedCorrelationRequirePublicContracts()
    {
        var assembly = Assembly.Load("dw.tools.math.statistics");
        var quantile = assembly.GetType("Dw.Tools.Math.Statistics.QuantileStatistics64");
        var correlation = assembly.GetType("Dw.Tools.Math.Statistics.CorrelationStatistics64");
        Assert.IsTrue(quantile is not null && correlation is not null,
            "M3-W02-C02-T1 RED: conventioned quantile and paired correlation contracts are absent.");
    }
}
