using dw.quantities;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01RedTests
{
    [TestMethod]
    public void StandaloneStandardCatalogIsNowPubliclyAvailable()
    {
        Assert.AreNotEqual(typeof(ExactRational).Assembly, typeof(StandardUnitCatalog).Assembly);
        Assert.AreEqual("dw.quantities.standard", typeof(StandardUnitCatalog).Assembly.GetName().Name);
    }

    [TestMethod]
    public void ExplicitProfileResolverIsNowPubliclyAvailable()
    {
        Assert.AreEqual("dw.quantities.standard", typeof(StandardExpressionUnitResolver).Assembly.GetName().Name);
    }
}
