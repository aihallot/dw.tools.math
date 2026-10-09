using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C02BoundaryRedTests
{
    [TestMethod]
    public void Positive257DigitNumeratorMustBeRefusedBeforeEvaluation()
    {
        var metre = new UnitDefinition("m", "m", "metre", UnitSystem.Si,
            DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
            UnitTransformKind.Linear);
        var oversized = new ExactRational(BigInteger.Pow(10, 256), BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(oversized, metre).Evaluate(),
            "M2-W02-C02-T2 RED: 257-digit positive numerator wrongly passed the 256-digit bound.");
    }
}
