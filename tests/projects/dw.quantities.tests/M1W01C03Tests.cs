using System.Reflection;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C03Tests
{
    [TestMethod]
    public void DimensionVectorIsAbsentBeforeAuthorizedTransfer()
    {
        AssertMaterialized("DimensionVector",
            "M1-W01-C03 RED: DimensionVector is not materialized in Math.");
    }

    [TestMethod]
    public void QuantityIsAbsentBeforeAuthorizedTransfer()
    {
        AssertMaterialized("Quantity",
            "M1-W01-C03 RED: Quantity is not materialized in Math.");
    }

    [TestMethod]
    public void UnitDefinitionIsAbsentBeforeAuthorizedTransfer()
    {
        AssertMaterialized("UnitDefinition",
            "M1-W01-C03 RED: UnitDefinition is not materialized in Math.");
    }

    private static void AssertMaterialized(string simpleName, string failure)
    {
        var candidates = typeof(ExactRational).Assembly.GetTypes()
            .Where(type => string.Equals(type.Name, simpleName, StringComparison.Ordinal))
            .ToArray();
        Assert.AreEqual(1, candidates.Length, failure);
        Assert.IsTrue(candidates[0].IsPublic || candidates[0].IsNestedPublic,
            "The transferred mathematical type must be publicly consumable.");
    }
}
