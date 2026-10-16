using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C03T1RedTests
{
    [TestMethod]
    public void IndependentContentAndAnnotatedPresentationMathMlApiMustExist()
    {
        var codec=Assembly.Load("dw.tools.math.codecs");
        var type=codec.GetType("Dw.Tools.Math.Codecs.ExactMathMlInterop",false);
        var render=type?.GetMethod("Render",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode)],modifiers:null);
        var import=type?.GetMethod("ImportAnnotated",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(string)],modifiers:null);
        Assert.IsTrue(render is not null && import is not null,
            "M5-W01-C03-T1 RED: exact MathML Content/Presentation semantic annotation and import APIs are absent.");
    }
}
