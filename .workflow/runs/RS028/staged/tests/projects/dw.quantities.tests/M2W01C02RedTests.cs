using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M2W01C02RedTests
{
    [TestMethod]
    public void StandaloneMathematicalIrAssemblyMustExist()
    {
        Assembly? assembly;
        try { assembly = Assembly.Load("dw.tools.math.ir"); }
        catch (FileNotFoundException) { assembly = null; }
        Assert.IsNotNull(assembly,
            "M2-W01-C02-T1 RED: standalone immutable mathematical IR assembly is missing.");
    }

    [TestMethod]
    public void TypedScalarAndBoundSymbolContractsMustExist()
    {
        Assembly? assembly;
        try { assembly = Assembly.Load("dw.tools.math.ir"); }
        catch (FileNotFoundException) { assembly = null; }
        var exact = assembly?.GetType("Dw.Tools.Math.Ir.IrExactScalar");
        var symbol = assembly?.GetType("Dw.Tools.Math.Ir.IrSymbol");
        Assert.IsTrue(exact is not null && symbol is not null,
            "M2-W01-C02-T1 RED: typed scalar and bound symbol contracts are missing.");
    }
}
