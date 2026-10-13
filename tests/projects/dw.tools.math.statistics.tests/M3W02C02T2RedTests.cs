using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C02T2RedTests
{
    [TestMethod]
    public void TypedUndefinedCorrelationAndMissingPairContractsMustExist()
    {
        var assembly = Assembly.Load("dw.tools.math.statistics");
        var outcome = assembly.GetType("Dw.Tools.Math.Statistics.CorrelationOutcome64");
        var policy = assembly.GetType("Dw.Tools.Math.Statistics.MissingPairPolicy64");
        var status = assembly.GetType("Dw.Tools.Math.Statistics.CorrelationDefinitionStatus64");
        var method = assembly.GetType("Dw.Tools.Math.Statistics.CorrelationStatistics64")?
            .GetMethod("AnalyzeWithStatus", BindingFlags.Public | BindingFlags.Static);
        Assert.IsTrue(outcome is not null && policy is not null &&
            status is not null && method is not null,
            "M3-W02-C02-T2 RED: typed undefined correlation and explicit pair-missing contracts are absent.");
    }
}
