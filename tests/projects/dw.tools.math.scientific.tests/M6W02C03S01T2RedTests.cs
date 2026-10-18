using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S01T2RedTests
{
    [TestMethod]
    public void FiniteEuclideanBoundaryRiskAuditPublicContractMustExist()
    {
        var assembly = typeof(FiniteGeometry64).Assembly;
        var audit = assembly.GetType("Dw.Tools.Math.Scientific.FiniteGeometryBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W02-C03-S01-T2 RED: finite Euclidean cancellation and resolution boundary audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("EvaluateNorm"));
        Assert.IsNotNull(audit.GetMethod("EvaluateDistance"));
        Assert.IsNotNull(audit.GetMethod("EvaluateProjection"));
    }
}
