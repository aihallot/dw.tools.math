using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C03RedTests
{
    [TestMethod]
    public void BoundedProviderNeutralLinearAlgebraMustBeExplicitlyImplemented()
    {
        var assembly = Assembly.Load("dw.tools.math.numerics");
        Assert.IsNotNull(assembly.GetType("Dw.Tools.Math.Numerics.BoundedLinearAlgebra64"),
            "M3-W01-C03-T1 RED: bounded provider-neutral LU solve QR and singular spectrum contract is absent.");
    }
}
