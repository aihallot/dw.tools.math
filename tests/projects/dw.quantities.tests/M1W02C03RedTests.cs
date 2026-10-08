using System.Reflection;
using dw.quantities.expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C03RedTests
{
    [TestMethod]
    public void StableSelectionOriginContractIsAbsentBeforeImplementation()
    {
        var type = typeof(ExpressionParser).Assembly.GetType("dw.quantities.expression.ExactSelection");
        Assert.IsNotNull(type, "M1-W02-C03-T1 RED: stable-selection operand index is not exposed.");
    }

    [TestMethod]
    public void NeutralDiagnosticContractIsAbsentBeforeImplementation()
    {
        var type = typeof(ExpressionParser).Assembly.GetType("dw.quantities.expression.ExpressionDiagnostics");
        Assert.IsNotNull(type, "M1-W02-C03-T2 RED: stable bounded neutral diagnostic API is not exposed.");
    }
}
