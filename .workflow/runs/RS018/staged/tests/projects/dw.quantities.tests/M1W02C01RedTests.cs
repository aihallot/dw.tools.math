using System.Reflection;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01RedTests
{
    [TestMethod]
    public void StandardCatalogIsAbsentBeforeAdapterTransfer()
    {
        Assert.IsFalse(typeof(ExactRational).Assembly.GetTypes().Any(t => t.Name == "StandardUnitCatalog"));
        Assert.IsTrue(MissingStandardAssembly(),
            "M1-W02-C01-T1 RED: expected the stand-alone standard catalogue assembly to be absent.");
        Assert.Fail("M1-W02-C01-T1 RED: stand-alone dw.quantities.standard catalog has not been materialized.");
    }

    [TestMethod]
    public void ExplicitProfileResolverIsAbsentBeforeAdapterTransfer()
    {
        Assert.IsTrue(MissingStandardAssembly(),
            "M1-W02-C01-T2 RED: expected explicit-profile resolver assembly absence.");
        Assert.Fail("M1-W02-C01-T2 RED: explicit-profile strict unit resolver has not been materialized.");
    }

    private static bool MissingStandardAssembly()
    {
        try { _ = Assembly.Load("dw.quantities.standard"); return false; }
        catch (FileNotFoundException) { return true; }
    }
}
