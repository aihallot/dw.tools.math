using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C03T1RedTests
{
    [TestMethod]
    public void VersionedDeterministicRandomAndBernoulliContractsMustExist()
    {
        var assembly = Assembly.Load("dw.tools.math.statistics");
        var generator = assembly.GetType("Dw.Tools.Math.Statistics.SplitMix64Random");
        var distribution = assembly.GetType("Dw.Tools.Math.Statistics.BernoulliDistribution64");
        var checkpoint = assembly.GetType("Dw.Tools.Math.Statistics.SplitMix64Position");
        Assert.IsTrue(generator is not null && distribution is not null && checkpoint is not null,
            "M3-W02-C03-T1 RED: versioned PRNG and Bernoulli distribution contracts are absent.");
    }
}
