using System.Reflection;
using Dw.Tools.Math.Ir;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C03T1RedTests
{
    [TestMethod]
    public void ExactPolynomialIntegralAndFiniteLimitPublicContractsMustExist()
    {
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicExactPolynomialCalculus",
            throwOnError:false);
        var integral=type?.GetMethod("Integrate",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrSymbol),
                typeof(ExactRational),typeof(ExactRational)],
            modifiers:null);
        var limit=type?.GetMethod("FiniteLimit",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrSymbol),typeof(ExactRational)],
            modifiers:null);
        Assert.IsTrue(integral is not null && limit is not null,
            "M4-W02-C03-T1 RED: exact real polynomial integral and finite limit APIs are absent.");
    }
}
