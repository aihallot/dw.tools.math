using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C01T1RedTests
{
    [TestMethod]
    public void BoundedExactScientificOperationsMustExist()
    {
        var assembly=typeof(ExactScientificBudget).Assembly;
        var type=assembly.GetType("Dw.Tools.Math.Scientific.BoundedExactScientific");
        Assert.IsNotNull(type,
            "M6-W02-C01-T1 RED: exact gcd, factorial, binomial and finite sequence public API is absent.");
        Assert.IsNotNull(type!.GetMethod("GreatestCommonDivisor"));
        Assert.IsNotNull(type.GetMethod("LeastCommonMultiple"));
        Assert.IsNotNull(type.GetMethod("Factorial"));
        Assert.IsNotNull(type.GetMethod("Binomial"));
        Assert.IsNotNull(type.GetMethod("SumArithmeticSequence"));
    }
}
