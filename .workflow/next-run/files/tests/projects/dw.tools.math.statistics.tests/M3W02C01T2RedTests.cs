using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C01T2RedTests
{
    [TestMethod]
    public void FrequencyWeightAndMissingPolicyRequireAnExplicitPublicContract()
    {
        var assembly=Assembly.Load("dw.tools.math.statistics");
        Assert.IsNotNull(assembly.GetType(
            "Dw.Tools.Math.Statistics.FrequencyWeightedStatistics64"),
            "M3-W02-C01-T2 RED: explicit frequency weights and missing-value policy contract is absent.");
    }
}
