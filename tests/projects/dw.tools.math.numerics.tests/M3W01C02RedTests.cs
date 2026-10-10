using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C02RedTests
{
    [TestMethod]
    public void BoundedFiniteMatrixMustHavePublicProviderNeutralContract()
    {
        var assembly = Assembly.Load("dw.tools.math.numerics");
        Assert.IsNotNull(assembly.GetType("Dw.Tools.Math.Numerics.FiniteMatrix64"),
            "M3-W01-C02-T1 RED: provider-neutral bounded finite matrix contract is absent.");
    }
}
