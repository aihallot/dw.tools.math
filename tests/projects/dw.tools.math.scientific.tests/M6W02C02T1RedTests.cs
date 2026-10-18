using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C02T1RedTests
{
    [TestMethod]
    public void BoundedManagedSpecialFunctionsProviderAndFacadeMustExist()
    {
        var assembly=typeof(SpecialFunctionPolicy64).Assembly;
        var facade=assembly.GetType("Dw.Tools.Math.Scientific.BoundedSpecialFunctions64");
        Assert.IsNotNull(facade,
            "M6-W02-C02-T1 RED: public bounded real and complex special-function facade is absent.");
        Assert.IsNotNull(facade!.GetMethod("EvaluateReal"));
        Assert.IsNotNull(facade.GetMethod("EvaluateComplex"));
        var provider=assembly.GetType("Dw.Tools.Math.Scientific.ManagedBclSpecialFunctionsProvider64");
        Assert.IsNotNull(provider,"M6-W02-C02-T1 RED: explicit managed BCL provider is absent.");
    }
}
