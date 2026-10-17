using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C02T2RedTests
{
    [TestMethod]
    public void ExplicitResidualAndRegularityAuditPublicApiMustExist()
    {
        var assembly = typeof(BoundedUnivariateAnalysis64).Assembly;
        var audit = assembly.GetType("Dw.Tools.Math.Analysis.UnivariateBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W01-C02-T2 RED: the public residual and regularity audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("AuditRoot"));
        Assert.IsNotNull(audit.GetMethod("AuditQuadrature"));
    }
}
