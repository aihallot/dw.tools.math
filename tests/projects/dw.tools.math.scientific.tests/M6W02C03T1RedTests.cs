using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03T1RedTests
{
    [TestMethod]
    public void CrossFamilyScientificAggregatePublicContractMustExist()
    {
        var assembly = typeof(FiniteGeometry64).Assembly;
        var facade = assembly.GetType("Dw.Tools.Math.Scientific.M6W02C03Contract");
        Assert.IsNotNull(facade,
            "M6-W02-C03-T1 RED: cross-family geometry signals and uniform-series public aggregate API is absent.");
        Assert.IsNotNull(facade!.GetMethod("Evaluate"));
    }
}
