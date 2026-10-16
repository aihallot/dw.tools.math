using System.Reflection;
using Dw.Tools.Math.Codecs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C03T2RedTests
{
    [TestMethod]
    public void IndependentMathMlXmlLossAndSourceProvenanceAuditMustExist()
    {
        var assembly=typeof(ExactMathMlInterop).Assembly;
        var type=assembly.GetType("Dw.Tools.Math.Codecs.MathMlBoundaryAudit",
            throwOnError:false);
        var method=type?.GetMethod("Audit",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(string)],modifiers:null);
        Assert.IsNotNull(method,
            "M5-W01-C03-T2 RED: independent bounded MathML XML provenance and semantic-loss audit API is absent.");
    }
}
