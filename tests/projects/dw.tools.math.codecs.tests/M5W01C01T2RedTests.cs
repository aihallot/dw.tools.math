using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C01T2RedTests
{
    [TestMethod]
    public void IndependentCanonicalDisplayBoundaryAuditMustBePublic()
    {
        var assembly=Assembly.Load("dw.tools.math.codecs");
        var type=assembly.GetType(
            "Dw.Tools.Math.Codecs.CanonicalDisplayBoundaryAudit",
            throwOnError:false);
        var text=type?.GetMethod("AuditText",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(string)],modifiers:null);
        var ir=type?.GetMethod("AuditIr",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode)],modifiers:null);
        Assert.IsTrue(text is not null && ir is not null,
            "M5-W01-C01-T2 RED: canonical display-boundary text and IR audit APIs are absent.");
    }
}
