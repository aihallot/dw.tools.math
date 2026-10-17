using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C03T2RedTests
{
    [TestMethod]
    public void PublicDifferentialAndOdeBoundaryAuditMustExist()
    {
        var assembly=typeof(BoundedDifferentialAnalysis64).Assembly;
        var publicApi=assembly.GetType("Dw.Tools.Math.Analysis.DifferentialBoundaryAudit64");
        Assert.IsNotNull(publicApi,
            "M6-W01-C03-T2 RED: explicit differential and ODE regime audit API is absent.");
        Assert.IsNotNull(publicApi!.GetMethod("Differentiate"));
        Assert.IsNotNull(publicApi.GetMethod("IntegrateOde"));
    }
}
