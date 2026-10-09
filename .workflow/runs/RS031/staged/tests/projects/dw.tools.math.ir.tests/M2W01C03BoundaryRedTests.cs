using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C03BoundaryRedTests
{
    [TestMethod]
    public void UnknownNodeFieldMustBeRefusedBeforeMaterialization()
    {
        var valid = IrCanonicalJsonCodec.Encode(new IrExactScalar(ExactRational.One));
        var injected = valid.Replace("\"denominator\":\"1\"",
            "\"denominator\":\"1\",\"unexpected\":\"value\"", StringComparison.Ordinal);
        Assert.ThrowsExactly<FormatException>(() => IrCanonicalJsonCodec.Decode(injected),
            "M2-W01-C03-T2 RED: unknown JSON node fields were accepted.");
    }

    [TestMethod]
    public void UnreferencedRestrictedSymbolMustNotBeAdmitted()
    {
        var x = IrSymbol.Free("x","x");
        var original = new IrRestrictedExpression(
            new IrExactScalar(ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(x,IrRelationKind.NotEqual,new IrExactScalar(ExactRational.Zero))]));
        var json = IrCanonicalJsonCodec.Encode(original);
        Assert.ThrowsExactly<FormatException>(() => IrCanonicalJsonCodec.Decode(json),
            "M2-W01-C03-T2 RED: restriction references no symbol in its expression.");
    }

    [TestMethod]
    public void DistinctVersionedHashContractsMustExist()
    {
        var hashType = typeof(IrNode).Assembly.GetType("Dw.Tools.Math.Ir.IrCanonicalHashes");
        Assert.IsNotNull(hashType,
            "M2-W01-C03-T2 RED: distinct versioned structural and presentation hashes are absent.");
    }
}
