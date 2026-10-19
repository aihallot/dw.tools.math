using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C04T1RedTests
{
    [TestMethod]
    public void EffectiveRateDiscountAndIrrPublicContractMustExist()
    {
        var assembly = typeof(M6W02C03Contract).Assembly;
        var facade = assembly.GetType("Dw.Tools.Math.Scientific.M6W02C04Contract");
        Assert.IsNotNull(facade,
            "M6-W02-C04-T1 RED: explicit effective-rate discount and bracketed IRR public API is absent.");
        Assert.IsNotNull(facade!.GetMethod("Discount"));
        Assert.IsNotNull(facade.GetMethod("SolveIrr"));
    }
}
