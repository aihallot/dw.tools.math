using System.Reflection;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C02T1RedTests
{
    [TestMethod]
    public void PublicTypedExactCancellationContractMustExist()
    {
        // A true compiled RED against a real but empty symbolics assembly.
        // A build error or missing project cannot substitute for a scientific
        // absence test. RED and GREEN use the same reflection assertion.
        var product = Assembly.Load("dw.tools.math.symbolics");
        var bridge = product.GetType(
            "Dw.Tools.Math.Symbolics.SymbolicRationalCancellationBridge",
            throwOnError: false);
        var method = bridge?.GetMethod("TryCancel", BindingFlags.Public | BindingFlags.Static,
            binder: null, types: [typeof(IrNode)], modifiers: null);
        Assert.IsNotNull(method,
            "M4-W01-C02-T1 RED: public typed domain-preserving cancellation contract is absent.");
    }
}
