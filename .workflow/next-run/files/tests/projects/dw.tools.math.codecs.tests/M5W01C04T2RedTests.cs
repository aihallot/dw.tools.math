using Dw.Tools.Math.Codecs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C04T2RedTests
{
    [TestMethod]
    public void IndependentOmmlAndMarkdownBoundaryAuditPublicApiMustExist()
    {
        var assembly=typeof(ExactOfficeOmmlCodec).Assembly;
        var audit=assembly.GetType("Dw.Tools.Math.Codecs.MathDisplayBoundaryAudit");
        Assert.IsNotNull(audit,
            "M5-W01-C04-T2 RED: independent OMML and Markdown display boundary audit API is absent.");
        Assert.IsNotNull(audit!.GetMethod("AuditOmml"));
        Assert.IsNotNull(audit!.GetMethod("AuditMarkdown"));
    }
}
