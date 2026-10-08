using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C02Tests
{
    private const string BinaryMarker = "M1-W01-C02-T1 RED: ExactBinaryNumber and ExactDecimalFormatter are absent";

    [TestMethod]
    public void TransferContractRemainsRedUntilBothTypesAreMaterialized()
    {
        var assembly = typeof(dw.quantities.ExactRational).Assembly;
        var names = assembly.GetTypes().Select(t => t.Name).ToHashSet(StringComparer.Ordinal);
        Assert.IsTrue(names.Contains("ExactBinaryNumber") && names.Contains("ExactDecimalFormatter"), BinaryMarker);
    }


}
