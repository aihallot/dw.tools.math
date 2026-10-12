using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C02T2RedTests
{
    [TestMethod]
    public void AgentIndependentRationalToBinary64RequiresExplicitStatusContract()
    {
        var assembly = Assembly.Load("dw.tools.math.numerics");
        Assert.IsNotNull(assembly.GetType("Dw.Tools.Math.Numerics.RationalBinary64Conversion"),
            "M3-W01-C02-T2 RED: explicit exact-rational binary64 conversion result contract is absent.");
    }
}
