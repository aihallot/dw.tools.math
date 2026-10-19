using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S03T2RedTests
{
    [TestMethod]
    public void UniformTimeGridAndNumericalAuditPublicContractMustExist()
    {
        var assembly = typeof(FiniteUniformSeries64).Assembly;
        var audit = assembly.GetType("Dw.Tools.Math.Scientific.FiniteUniformSeriesBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W02-C03-S03-T2 RED: explicit uniform time-grid and numerical conditioning audit public API is absent.");
        Assert.IsNotNull(audit!.GetMethod("EvaluateFirstDifferences"));
        Assert.IsNotNull(audit.GetMethod("EvaluateMovingAverage"));
    }
}
