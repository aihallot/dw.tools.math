using Dw.Tools.Math.Distribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C01T2RedTests
{
    [TestMethod]
    public void PublicIndependentOriginalDomainAndNumericLossAuditMustExist()
    {
        var assembly=typeof(ExactMathVerticalRunner).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Distribution.ExactVerticalBoundaryAudit");
        Assert.IsNotNull(audit,
            "M5-W02-C01-T2 RED: independent numerical/symbolic provenance and loss audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("Audit"));
    }
}
