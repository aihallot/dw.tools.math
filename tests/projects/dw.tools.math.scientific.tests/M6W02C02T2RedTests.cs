using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C02T2RedTests
{
    [TestMethod]
    public void BoundedRealComplexAnglePoleAndBranchAuditMustExist()
    {
        var assembly=typeof(SpecialFunctionKind64).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Scientific.SpecialFunctionBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W02-C02-T2 RED: explicit special-function angular, pole and complex branch audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("EvaluateReal"));
        Assert.IsNotNull(audit.GetMethod("EvaluateComplex"));
    }
}
