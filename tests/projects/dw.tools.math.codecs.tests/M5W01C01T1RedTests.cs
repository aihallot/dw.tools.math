using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C01T1RedTests
{
    [TestMethod]
    public void PublicCanonicalMathTextAndLatexContractMustExist()
    {
        var assembly=Assembly.Load("dw.tools.math.codecs");
        var type=assembly.GetType("Dw.Tools.Math.Codecs.CanonicalMathNotation",
            throwOnError:false);
        var parse=type?.GetMethod("Parse",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(string)],modifiers:null);
        var render=type?.GetMethod("Render",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode)],modifiers:null);
        Assert.IsTrue(parse is not null && render is not null,
            "M5-W01-C01-T1 RED: canonical math text parser and display-only LaTeX renderer APIs are absent.");
    }
}
