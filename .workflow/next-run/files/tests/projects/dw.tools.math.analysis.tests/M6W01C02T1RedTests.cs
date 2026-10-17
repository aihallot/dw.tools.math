using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C02T1RedTests
{
    [TestMethod]
    public void BoundedRootAndQuadraturePublicContractMustExist()
    {
        var assembly=typeof(CurvePoint64).Assembly;
        var api=assembly.GetType("Dw.Tools.Math.Analysis.BoundedUnivariateAnalysis64");
        Assert.IsNotNull(api,
            "M6-W01-C02-T1 RED: bounded public root and quadrature API is absent.");
        Assert.IsNotNull(api!.GetMethod("SolveBracketed"));
        Assert.IsNotNull(api.GetMethod("IntegrateSimpson"));
    }
}
