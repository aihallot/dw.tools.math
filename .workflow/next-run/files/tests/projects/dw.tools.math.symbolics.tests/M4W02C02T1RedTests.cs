using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C02T1RedTests
{
    [TestMethod]
    public void PublicExactRealEquationSolverMustExist()
    {
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType("Dw.Tools.Math.Symbolics.SymbolicExactRealSolving",
            throwOnError:false);
        var method=type?.GetMethod("Solve",BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrRelationKind),typeof(IrNode),
                typeof(IrSymbol),typeof(IrScalarDomain)],
            modifiers:null);
        Assert.IsNotNull(method,
            "M4-W02-C02-T1 RED: real exact equation solution contract is absent.");
    }
}
