using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Ir.Tests;

[TestClass]
public sealed class M2W01C03RedTests
{
    [TestMethod]
    public void CanonicalExactIrJsonPublicCodecMustExist()
    {
        var codec = typeof(IrNode).Assembly.GetType("Dw.Tools.Math.Ir.IrCanonicalJsonCodec");
        Assert.IsNotNull(codec,
            "M2-W01-C03-T1 RED: lossless canonical IR JSON codec is absent.");
    }

    [TestMethod]
    public void PublicEncodeAndDecodeContractsMustBeInstalled()
    {
        var codec = typeof(IrNode).Assembly.GetType("Dw.Tools.Math.Ir.IrCanonicalJsonCodec");
        var encode = codec?.GetMethod("Encode", BindingFlags.Public | BindingFlags.Static);
        var decode = codec?.GetMethod("Decode", BindingFlags.Public | BindingFlags.Static);
        Assert.IsTrue(encode is not null && decode is not null,
            "M2-W01-C03-T1 RED: versioned IR Encode and Decode are missing.");
    }
}
