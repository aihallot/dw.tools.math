using Dw.Tools.Math.Codecs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C04T1RedTests
{
    [TestMethod]
    public void StandaloneExactOfficeAndMarkdownUnicodePublicContractsMustExist()
    {
        var assembly = typeof(ExactMathMlInterop).Assembly;
        var office = assembly.GetType("Dw.Tools.Math.Codecs.ExactOfficeOmmlCodec");
        Assert.IsNotNull(office,
            "M5-W01-C04-T1 RED: exact Office OMML public codec is absent.");
        var markdown = assembly.GetType("Dw.Tools.Math.Codecs.MarkdownUnicodeMathCodec");
        Assert.IsNotNull(markdown,
            "M5-W01-C04-T1 RED: Markdown Unicode public codec is absent.");
    }
}
