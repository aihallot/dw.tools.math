using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C03Tests
{
    [TestMethod]
    public void HugeCanonicalRationalRoundTripsWithoutDoubleOrDecimalLoss()
    {
        var n = BigInteger.Pow(new BigInteger(10), 90) + 7;
        var source = new IrExactScalar(new ExactRational(n, 13));
        var json = IrCanonicalJsonCodec.Encode(source);
        Assert.IsTrue(json.Contains(n.ToString(System.Globalization.CultureInfo.InvariantCulture),
            StringComparison.Ordinal));
        var decoded = (IrExactScalar)IrCanonicalJsonCodec.Decode(json);
        Assert.AreEqual(source.Value, decoded.Value);
        Assert.AreEqual(json, IrCanonicalJsonCodec.Encode(decoded));
    }

    [TestMethod]
    public void ApproximateIeeeBitsPreserveNegativeZeroExactly()
    {
        var negativeZero = IrFiniteBinary64Scalar.FromDouble(-0.0);
        var json = IrCanonicalJsonCodec.Encode(negativeZero);
        var parsed = (IrFiniteBinary64Scalar)IrCanonicalJsonCodec.Decode(json);
        Assert.AreEqual(negativeZero.Ieee754Bits, parsed.Ieee754Bits);
        Assert.AreEqual(json, IrCanonicalJsonCodec.Encode(parsed));
        Assert.AreNotEqual(json, IrCanonicalJsonCodec.Encode(
            IrFiniteBinary64Scalar.FromDouble(0.0)));
    }

    [TestMethod]
    public void QuantitiesRoundTripCanonicalUnitEightDimensionsAndTemperature()
    {
        var source = new IrQuantityLiteral(new IrExactScalar(new ExactRational(27315,100)),
            "si.kelvin", UnitSystem.Si, DimensionVector.TemperatureDimension,
            IrTemperatureKind.Absolute);
        var encoded = IrCanonicalJsonCodec.Encode(source);
        var decoded = (IrQuantityLiteral)IrCanonicalJsonCodec.Decode(encoded);
        Assert.AreEqual(source.CanonicalUnitId, decoded.CanonicalUnitId);
        Assert.AreEqual(source.System, decoded.System);
        Assert.AreEqual(source.Dimension, decoded.Dimension);
        Assert.AreEqual(source.Temperature, decoded.Temperature);
        Assert.AreEqual(((IrExactScalar)source.Value).Value, ((IrExactScalar)decoded.Value).Value);
        Assert.AreEqual(encoded, IrCanonicalJsonCodec.Encode(decoded));
    }

    [TestMethod]
    public void InformationDimensionUsesEighthExponentAfterRoundTrip()
    {
        var source = new IrQuantityLiteral(new IrExactScalar(ExactRational.One),
            "iec.kibibyte", UnitSystem.IecBinary, DimensionVector.InformationDimension);
        var decoded = (IrQuantityLiteral)IrCanonicalJsonCodec.Decode(
            IrCanonicalJsonCodec.Encode(source));
        Assert.AreEqual(1, decoded.Dimension.Information);
        Assert.AreEqual(0, decoded.Dimension.Length);
    }

    [TestMethod]
    public void BoundAndFreeSameLabelKeepDifferentStableIdentities()
    {
        var free = IrSymbol.Free("x", "x");
        var bound = IrSymbol.Bound("scope-a", 0, "x");
        var original = IrApply.Create(IrOperation.Add, [free, bound]);
        var encoded = IrCanonicalJsonCodec.Encode(original);
        var parsed = (IrApply)IrCanonicalJsonCodec.Decode(encoded);
        var decodedFree = (IrSymbol)parsed.Arguments[0];
        var decodedBound = (IrSymbol)parsed.Arguments[1];
        Assert.IsFalse(decodedFree.IsBound);
        Assert.IsTrue(decodedBound.IsBound);
        Assert.AreNotEqual(decodedFree.Identity, decodedBound.Identity);
        Assert.AreEqual("scope-a", decodedBound.ScopeId);
        Assert.AreEqual(encoded, IrCanonicalJsonCodec.Encode(parsed));
    }

    [TestMethod]
    public void RationalExclusionAndExpressionRemainStructurallyIdentical()
    {
        var x = IrSymbol.Free("x", "x");
        var numerator = IrApply.Create(IrOperation.Subtract,
            [IrApply.Create(IrOperation.Square, [x]), new IrExactScalar(ExactRational.One)]);
        var denominator = IrApply.Create(IrOperation.Subtract,
            [x, new IrExactScalar(ExactRational.One)]);
        var fraction = IrApply.Create(IrOperation.Divide, [numerator, denominator]);
        var restriction = new IrRelation(x, IrRelationKind.NotEqual,
            new IrExactScalar(ExactRational.One));
        var source = new IrRestrictedExpression(fraction, IrAssumptionSet.Create([restriction]));
        var json = IrCanonicalJsonCodec.Encode(source);
        var parsed = (IrRestrictedExpression)IrCanonicalJsonCodec.Decode(json);
        Assert.AreEqual(IrOperation.Divide, ((IrApply)parsed.Expression).Operation);
        Assert.AreEqual(IrRelationKind.NotEqual, parsed.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(x.Identity, parsed.Assumptions.Conditions[0].Symbol.Identity);
        Assert.AreEqual(ExactRational.One, parsed.Assumptions.Conditions[0].Right.Value);
        Assert.AreEqual(json, IrCanonicalJsonCodec.Encode(parsed));
    }

    [TestMethod]
    public void RectangularMatrixPreservesExactCellOrderAndValues()
    {
        var source = IrMatrix.Create(2, 2, [
            Exact(1,3), Exact(2,1), Exact(-3,5), Exact(4,1)]);
        var json = IrCanonicalJsonCodec.Encode(source);
        var result = (IrMatrix)IrCanonicalJsonCodec.Decode(json);
        Assert.AreEqual(2, result.Rows);
        Assert.AreEqual(2, result.Columns);
        Assert.AreEqual(new ExactRational(1,3), ((IrExactScalar)result.At(0,0)).Value);
        Assert.AreEqual(new ExactRational(-3,5), ((IrExactScalar)result.At(1,0)).Value);
        Assert.AreEqual(json, IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void RepeatedCanonicalEncodingIsByteIdenticalUnderSamePolicy()
    {
        var graph = IrApply.Create(IrOperation.Max,
            [Exact(1,3), Exact(2,5), Exact(1,2)]);
        var first = IrCanonicalJsonCodec.Encode(graph);
        var second = IrCanonicalJsonCodec.Encode(graph);
        var reencoded = IrCanonicalJsonCodec.Encode(IrCanonicalJsonCodec.Decode(first));
        Assert.AreEqual(first, second);
        Assert.AreEqual(first, reencoded);
        Assert.IsTrue(first.StartsWith("{\"version\":\"math-ir/1\",\"root\":",StringComparison.Ordinal));
    }

    [TestMethod]
    public void UnsupportedVersionAndNonCanonicalRationalsAreNotAccepted()
    {
        var example = IrCanonicalJsonCodec.Encode(Exact(1,3));
        Assert.ThrowsExactly<NotSupportedException>(() =>
            IrCanonicalJsonCodec.Decode(example.Replace("math-ir/1","math-ir/2",
                StringComparison.Ordinal)));
        Assert.ThrowsExactly<FormatException>(() =>
            IrCanonicalJsonCodec.Decode(example.Replace("\"numerator\":\"1\",\"denominator\":\"3\"",
                "\"numerator\":\"2\",\"denominator\":\"6\"",StringComparison.Ordinal)));
    }

    [TestMethod]
    public void UnknownBindingIdentityDoesNotSilentlyChangeScope()
    {
        var encoded = IrCanonicalJsonCodec.Encode(IrSymbol.Free("x", "x"));
        Assert.ThrowsExactly<FormatException>(() =>
            IrCanonicalJsonCodec.Decode(encoded.Replace("\"free:x\"","\"bound:scope-a:0\"",
                StringComparison.Ordinal)));
    }

    private static IrExactScalar Exact(int n, int d) => new(new ExactRational(n,d));
}
