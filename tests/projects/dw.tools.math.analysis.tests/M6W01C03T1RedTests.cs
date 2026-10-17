using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C03T1RedTests
{
    [TestMethod]
    public void PublicBoundedDerivativeAndOdeApiMustExist()
    {
        var assembly=typeof(BoundedUnivariateAnalysis64).Assembly;
        var type=assembly.GetType("Dw.Tools.Math.Analysis.BoundedDifferentialAnalysis64");
        Assert.IsNotNull(type,
            "M6-W01-C03-T1 RED: bounded public derivative and ODE API is absent.");
        Assert.IsNotNull(type!.GetMethod("Differentiate"));
        Assert.IsNotNull(type.GetMethod("IntegrateOde"));
    }
}
