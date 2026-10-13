using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C03T2RedTests
{
    [TestMethod]
    public void ContinuousApproximationAndPortableCheckpointRequirePublicContracts()
    {
        var assembly = Assembly.Load("dw.tools.math.statistics");
        var continuous = assembly.GetType("Dw.Tools.Math.Statistics.ContinuousUniformDistribution64");
        var codec = assembly.GetType("Dw.Tools.Math.Statistics.SplitMix64PositionCodec");
        Assert.IsTrue(continuous is not null && codec is not null,
            "M3-W02-C03-T2 RED: continuous uniform and portable PRNG checkpoint public contracts are absent.");
    }
}
