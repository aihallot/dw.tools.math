using Dw.Tools.Math.Distribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C01T1RedTests
{
    [TestMethod]
    public void PublicSourceBoundNumericalAndSymbolicVerticalMustExist()
    {
        var assembly=typeof(ExactVerticalStatus).Assembly;
        var runner=assembly.GetType("Dw.Tools.Math.Distribution.ExactMathVerticalRunner");
        Assert.IsNotNull(runner,
            "M5-W02-C01-T1 RED: public exact numerical and symbolic vertical runner is absent.");
        Assert.IsNotNull(runner!.GetMethod("Run"));
    }
}
