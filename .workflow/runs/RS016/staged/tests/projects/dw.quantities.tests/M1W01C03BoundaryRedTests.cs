using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C03BoundaryRedTests
{
    public const string OverflowMarker =
        "M1-W01-C03-T2 RED: direct DimensionVector exponent overflow is unchecked";

    [TestMethod]
    public void DimensionExponentOverflowIsRejectedAtDirectPublicOperator()
    {
        var largest = new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0);
        Assert.ThrowsExactly<OverflowException>(
            () => _ = largest + DimensionVector.LengthDimension, OverflowMarker);
    }
}
