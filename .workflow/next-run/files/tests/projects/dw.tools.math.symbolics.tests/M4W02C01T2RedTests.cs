using System.Reflection;
using Dw.Tools.Math.Ir;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C01T2RedTests
{
    [TestMethod]
    public void ExplicitPointwiseDerivativeBoundaryContractMustExist()
    {
        // Compile against the actual preview.5 assembly. Only the absent
        // public mathematical boundary API is the expected RED assertion.
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicDerivativePointAnalysis",
            throwOnError:false);
        var method=type?.GetMethod("Analyze",BindingFlags.Public|BindingFlags.Static,
            binder:null,
            types:[typeof(IrNode),typeof(IrSymbol),typeof(ExactRational)],
            modifiers:null);
        Assert.IsNotNull(method,
            "M4-W02-C01-T2 RED: pointwise real derivative boundary API is absent.");
    }
}
