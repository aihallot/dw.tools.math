using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C04T2RedTests
{
    [TestMethod]
    public void ExplicitDatedFinancialBoundaryPublicContractMustExist()
    {
        var assembly = typeof(M6W02C04Contract).Assembly;
        var boundary = assembly.GetType("Dw.Tools.Math.Scientific.M6W02C04Boundary64");
        Assert.IsNotNull(boundary,
            "M6-W02-C04-T2 RED: explicit actual-day financial discount and dated IRR boundary API is absent.");
        Assert.IsNotNull(boundary!.GetMethod("DiscountDated"));
        Assert.IsNotNull(boundary.GetMethod("SolveDatedIrr"));
    }
}
