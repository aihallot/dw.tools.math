using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C03S02T2RedTests
{
    [TestMethod]
    public void DiscreteSignalBoundaryAndTopologyAuditPublicContractMustExist()
    {
        var assembly = typeof(FiniteSignal64).Assembly;
        var audit = assembly.GetType("Dw.Tools.Math.Scientific.FiniteSignalBoundaryAudit64");
        Assert.IsNotNull(audit,
            "M6-W02-C03-S02-T2 RED: explicit sampling, circular refusal and cancellation audit public API is absent.");
        Assert.IsNotNull(audit!.GetMethod("EvaluateDft"));
        Assert.IsNotNull(audit.GetMethod("EvaluateConvolution"));
    }
}
