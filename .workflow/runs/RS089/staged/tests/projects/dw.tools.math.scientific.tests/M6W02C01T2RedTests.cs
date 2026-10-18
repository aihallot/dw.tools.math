using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C01T2RedTests
{
    [TestMethod]
    public void SignedNormalizationAndFiniteIntermediateAuditMustExist()
    {
        var assembly=typeof(ExactScientificBudget).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Scientific.ExactScientificBoundaryAudit");
        Assert.IsNotNull(audit,
            "M6-W02-C01-T2 RED: the public signed-normalization and intermediate-budget audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("GreatestCommonDivisor"));
        Assert.IsNotNull(audit.GetMethod("LeastCommonMultiple"));
        Assert.IsNotNull(audit.GetMethod("Factorial"));
        Assert.IsNotNull(audit.GetMethod("Binomial"));
        Assert.IsNotNull(audit.GetMethod("SumArithmeticSequence"));
    }
}
