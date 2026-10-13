using Dw.Tools.Math.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Numerics.Tests;

[TestClass]
public sealed class M3W01C03T2RedTests
{
    [TestMethod]
    public void AUniqueTwoByTwoSolveMustExposeConditionAvailability()
    {
        var property=typeof(LinearSolveResult64).GetProperty(
            "TwoByTwoSpectralConditionEstimate");
        Assert.IsNotNull(property,
            "M3-W01-C03-T2 RED: bounded optional 2x2 condition estimate missing from solver result.");
    }
}
