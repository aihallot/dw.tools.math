using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C03BoundaryTests
{
    private static IrExactScalar Exact(int n) => new(new ExactRational(n,1));

    [TestMethod]
    public void UnrecognizedNodePropertyMustFailInsteadOfBeingIgnored()
    {
        var json = IrCanonicalJsonCodec.Encode(Exact(1));
        var tampered = json.Replace("\"denominator\":\"1\"",
            "\"denominator\":\"1\",\"invalid-extra\":\"x\"", StringComparison.Ordinal);
        Assert.ThrowsExactly<FormatException>(() => IrCanonicalJsonCodec.Decode(tampered));
    }

    [TestMethod]
    public void DuplicateKeysMustFailBothInEnvelopeAndNode()
    {
        var body = IrCanonicalJsonCodec.Encode(Exact(1));
        var duplicateVersion = body.Replace("\"version\":\"math-ir/1\"",
            "\"version\":\"math-ir/1\",\"version\":\"math-ir/1\"",StringComparison.Ordinal);
        var duplicateNumerator = body.Replace("\"numerator\":\"1\"",
            "\"numerator\":\"1\",\"numerator\":\"1\"",StringComparison.Ordinal);
        Assert.ThrowsExactly<FormatException>(() => IrCanonicalJsonCodec.Decode(duplicateVersion));
        Assert.ThrowsExactly<FormatException>(() => IrCanonicalJsonCodec.Decode(duplicateNumerator));
    }

    [TestMethod]
    public void DuplicateOrUnknownAssumptionKeysMustFail()
    {
        var x=IrSymbol.Free("x","x");
        var guarded=new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,Exact(1))]));
        var json=IrCanonicalJsonCodec.Encode(guarded);
        var duplicate=json.Replace("\"relation\":\"NotEqual\"",
            "\"relation\":\"NotEqual\",\"relation\":\"NotEqual\"",StringComparison.Ordinal);
        var unknown=json.Replace("\"relation\":\"NotEqual\"",
            "\"relation\":\"NotEqual\",\"custom\":true",StringComparison.Ordinal);
        Assert.ThrowsExactly<FormatException>(()=>IrCanonicalJsonCodec.Decode(duplicate));
        Assert.ThrowsExactly<FormatException>(()=>IrCanonicalJsonCodec.Decode(unknown));
    }

    [TestMethod]
    public void UnreferencedAssumptionSymbolMustBeRejected()
    {
        var x=IrSymbol.Free("x","x");
        var bad=new IrRestrictedExpression(Exact(2),IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Exact(0))]));
        Assert.ThrowsExactly<FormatException>(()=>
            IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(bad)));
    }

    [TestMethod]
    public void ReferencedSymbolAssumptionsSurviveTheirIdentityAndDomain()
    {
        var x=IrSymbol.Free("x","display-name",IrScalarDomain.Real);
        var expr=IrApply.Create(IrOperation.Add,[x,Exact(1)]);
        var guard=IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,Exact(0))]);
        var input=new IrRestrictedExpression(expr,guard);
        var roundTrip=(IrRestrictedExpression)IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(input));
        Assert.AreEqual(x.Identity,roundTrip.Assumptions.Conditions[0].Symbol.Identity);
        Assert.AreEqual(x.Domain,roundTrip.Assumptions.Conditions[0].Symbol.Domain);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(input),IrCanonicalJsonCodec.Encode(roundTrip));
    }

    [TestMethod]
    public void SemanticStructuralHashExcludesPresentationOnlyLabels()
    {
        var x=IrSymbol.Free("x","x");
        var label=IrSymbol.Free("x","variable one");
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(x),
            IrCanonicalHashes.SemanticStructuralSha256(label));
        Assert.AreNotEqual(IrCanonicalHashes.PresentationSha256(x),
            IrCanonicalHashes.PresentationSha256(label));
    }

    [TestMethod]
    public void SymbolDomainAndExactValuesAffectStructuralHash()
    {
        var x=IrSymbol.Free("x","x",IrScalarDomain.Real);
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Assert.AreNotEqual(IrCanonicalHashes.SemanticStructuralSha256(x),
            IrCanonicalHashes.SemanticStructuralSha256(integer));
        Assert.AreNotEqual(IrCanonicalHashes.SemanticStructuralSha256(Exact(1)),
            IrCanonicalHashes.SemanticStructuralSha256(Exact(2)));
    }

    [TestMethod]
    public void HashesAreStableAcrossQualifiedCanonicalRoundTrip()
    {
        var x=IrSymbol.Free("x","x");
        var graph=IrApply.Create(IrOperation.Add,[x,Exact(7)]);
        var restored=IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(graph));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(graph),
            IrCanonicalHashes.SemanticStructuralSha256(restored));
        Assert.AreEqual(IrCanonicalHashes.PresentationSha256(graph),
            IrCanonicalHashes.PresentationSha256(restored));
        Assert.AreEqual(64,IrCanonicalHashes.SemanticStructuralSha256(graph).Length);
    }

    [TestMethod]
    public void OversizedExactNumeralAndJsonPayloadMustFailEarly()
    {
        var huge=new string('7',IrCanonicalJsonGuardMaximumDigits+1);
        var doc="{\"version\":\"math-ir/1\",\"root\":{\"kind\":\"exact\",\"numerator\":\""+
            huge+"\",\"denominator\":\"1\"}}";
        Assert.ThrowsExactly<FormatException>(()=>IrCanonicalJsonCodec.Decode(doc));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            IrCanonicalJsonCodec.Decode(new string('X',IrCanonicalJsonCodec.MaximumJsonCharacters+1)));
    }

    [TestMethod]
    public void UnknownKindVersionAndNonfiniteBitsMustFail()
    {
        var good=IrCanonicalJsonCodec.Encode(Exact(1));
        Assert.ThrowsExactly<NotSupportedException>(()=>
            IrCanonicalJsonCodec.Decode(good.Replace("math-ir/1","math-ir/2",StringComparison.Ordinal)));
        Assert.ThrowsExactly<NotSupportedException>(()=>
            IrCanonicalJsonCodec.Decode(good.Replace("\"kind\":\"exact\"",
                "\"kind\":\"new-kind\"",StringComparison.Ordinal)));
        var approx=IrCanonicalJsonCodec.Encode(IrFiniteBinary64Scalar.FromDouble(1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            IrCanonicalJsonCodec.Decode(approx.Replace("3ff0000000000000","7ff8000000000000",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void MalformedJsonAndInvalidBoundSymbolAreNotAccepted()
    {
        Assert.Throws<System.Text.Json.JsonException>(()=>
            IrCanonicalJsonCodec.Decode("{"));
        var bound=IrCanonicalJsonCodec.Encode(IrSymbol.Bound("scope-a",0,"x"));
        var invalid=bound.Replace("\"scope\":\"scope-a\"",
            "\"scope\":null",StringComparison.Ordinal);
        Assert.ThrowsExactly<FormatException>(()=>
            IrCanonicalJsonCodec.Decode(invalid));
    }

    private const int IrCanonicalJsonGuardMaximumDigits = 1024;
}
