using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C03T1RedTests
{
    [TestMethod]
    public void PublicExactTransformationsContractMustExist()
    {
        // The existing preview.2 library compiles; only this new public
        // mathematical contract is missing before the RED/GREEN transition.
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType("Dw.Tools.Math.Symbolics.SymbolicExactTransformations",
            throwOnError:false);
        var method=type?.GetMethod("ExpandSquare",BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode)],modifiers:null);
        Assert.IsNotNull(method,
            "M4-W01-C03-T1 RED: bounded exact symbolic transformations API is absent.");
    }
}
