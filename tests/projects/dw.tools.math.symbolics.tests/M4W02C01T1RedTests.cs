using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C01T1RedTests
{
    [TestMethod]
    public void ExactRealPolynomialDerivativeNeedsAPublicTypedContract()
    {
        // RED against the real compiled preview.4 symbolics assembly.
        // A missing API assertion, not a missing project or build failure.
        var assembly=Assembly.Load("dw.tools.math.symbolics");
        var type=assembly.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicExactDifferentiation",
            throwOnError:false);
        var method=type?.GetMethod("TryDifferentiate",
            BindingFlags.Public|BindingFlags.Static,
            binder:null,types:[typeof(IrNode),typeof(IrSymbol)],modifiers:null);
        Assert.IsNotNull(method,
            "M4-W02-C01-T1 RED: exact provider-neutral polynomial derivative API is absent.");
    }
}
