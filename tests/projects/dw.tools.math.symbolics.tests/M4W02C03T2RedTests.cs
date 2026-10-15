using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C03T2RedTests
{
    [TestMethod]
    public void IndependentRealCalculusBoundaryContractMustExist()
    {
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicRealCalculusBoundaries",
            throwOnError:false);
        var integral=type?.GetMethod("AnalyzeIntegral",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrSymbol),
                typeof(ExactRational),typeof(ExactRational)],
            modifiers:null);
        var limit=type?.GetMethod("AnalyzeLimit",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrSymbol),typeof(ExactRational)],
            modifiers:null);
        Assert.IsTrue(integral is not null && limit is not null,
            "M4-W02-C03-T2 RED: original-domain integral and limit boundary APIs are absent.");
    }
}
