using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C02Tests
{
    private const string BinaryMarker = "M1-W01-C02-T1 RED: ExactBinaryNumber and ExactDecimalFormatter are absent";
    private const string BoundaryMarker = "M1-W01-C02-T2 RED: finite/subnormal/signed-zero and precision boundaries are not qualified";

    [TestMethod]
    public void TransferContractRemainsRedUntilBothTypesAreMaterialized()
    {
        var assembly = typeof(dw.quantities.ExactRational).Assembly;
        var names = assembly.GetTypes().Select(t => t.Name).ToHashSet(StringComparer.Ordinal);
        Assert.IsTrue(names.Contains("ExactBinaryNumber") && names.Contains("ExactDecimalFormatter"), BinaryMarker);
    }

    [TestMethod]
    public void BoundaryContractRemainsRedUntilBothTypesAreMaterialized()
    {
        var assembly = typeof(dw.quantities.ExactRational).Assembly;
        var names = assembly.GetTypes().Select(t => t.Name).ToHashSet(StringComparer.Ordinal);
        Assert.IsTrue(names.Contains("ExactBinaryNumber") && names.Contains("ExactDecimalFormatter"), BoundaryMarker);
    }
}
