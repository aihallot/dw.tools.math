using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C01T2RedTests
{
    [TestMethod]
    public void PublicUnitAndConditioningBoundaryAuditMustExist()
    {
        var assembly=typeof(CurvePoint64).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Analysis.CurveBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W01-C01-T2 RED: the public unit, extrapolation and relative-conditioning audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("EvaluateLinear"));
        Assert.IsNotNull(audit.GetMethod("FitDescriptive"));
    }
}
