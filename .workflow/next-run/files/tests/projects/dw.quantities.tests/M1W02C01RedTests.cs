using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01RedTests
{
    [TestMethod]
    public void StandardCatalogIsAbsentBeforeAdapterTransfer()
    {
        var assembly = typeof(ExactRational).Assembly;
        Assert.IsFalse(assembly.GetTypes().Any(t => t.Name == "StandardUnitCatalog"),
            "The standard catalogue should not already live in the primitive assembly.");
        Assert.Fail("M1-W02-C01-T1 RED: stand-alone dw.quantities.standard catalog has not been materialized.");
    }

    [TestMethod]
    public void ExplicitProfileResolverIsAbsentBeforeAdapterTransfer()
    {
        Assert.Fail("M1-W02-C01-T2 RED: explicit-profile strict unit resolver has not been materialized.");
    }
}
