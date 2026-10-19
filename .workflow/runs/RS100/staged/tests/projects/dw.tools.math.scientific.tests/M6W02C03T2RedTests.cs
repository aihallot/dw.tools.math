using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03T2RedTests
{
    [TestMethod]
    public void CrossFamilyConservativeBoundaryIntegrationPublicContractMustExist()
    {
        var assembly = typeof(M6W02C03Contract).Assembly;
        var facade = assembly.GetType("Dw.Tools.Math.Scientific.M6W02C03BoundaryAggregate64");
        Assert.IsNotNull(facade,
            "M6-W02-C03-T2 RED: cross-family conservative audits and explicit caller-unit policy public API is absent.");
        Assert.IsNotNull(facade!.GetMethod("Evaluate"));
    }
}
