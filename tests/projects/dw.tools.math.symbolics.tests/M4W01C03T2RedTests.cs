using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C03T2RedTests
{
    [TestMethod]
    public void BoundedExecutionAndProofProvenanceMustHaveAPublicContract()
    {
        // Real compiled RED against the existing preview.3 product, not a
        // missing source or compiler failure disguised as a scientific test.
        var product=Assembly.Load("dw.tools.math.symbolics");
        var type=product.GetType("Dw.Tools.Math.Symbolics.SymbolicBoundedExecution",
            throwOnError:false);
        var method=type?.GetMethod("Execute",BindingFlags.Public|BindingFlags.Static);
        Assert.IsNotNull(method,
            "M4-W01-C03-T2 RED: bounded exact symbolic execution contract is absent.");
    }
}
