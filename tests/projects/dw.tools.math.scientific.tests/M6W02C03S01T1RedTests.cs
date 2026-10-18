using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S01T1RedTests
{
    [TestMethod]
    public void PublicFiniteGeometryNormDistanceAndProjectionContractMustExist()
    {
        var assembly = typeof(SpecialFunctionKind64).Assembly;
        var type = assembly.GetType("Dw.Tools.Math.Scientific.FiniteGeometry64");
        Assert.IsNotNull(type,
            "M6-W02-C03-S01-T1 RED: finite 2D/3D Euclidean geometry public contract is absent.");
        Assert.IsNotNull(type!.GetMethod("Norm"));
        Assert.IsNotNull(type.GetMethod("Distance"));
        Assert.IsNotNull(type.GetMethod("Project"));
    }
}
