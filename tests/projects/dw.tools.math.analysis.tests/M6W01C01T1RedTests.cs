using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C01T1RedTests
{
    [TestMethod]
    public void BoundedPublicLinearSplineAndLeastSquaresContractMustExist()
    {
        var assembly=typeof(CurvePoint64).Assembly;
        var type=assembly.GetType("Dw.Tools.Math.Analysis.BoundedCurveAnalysis64");
        Assert.IsNotNull(type,
            "M6-W01-C01-T1 RED: bounded public linear/spline interpolation and least-squares analysis API is absent.");
        Assert.IsNotNull(type!.GetMethod("InterpolateLinear"));
        Assert.IsNotNull(type.GetMethod("InterpolateNaturalSpline"));
        Assert.IsNotNull(type.GetMethod("FitLeastSquares"));
    }
}
