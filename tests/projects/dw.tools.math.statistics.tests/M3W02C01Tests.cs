using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Statistics.Tests;

[TestClass]
public sealed class M3W02C01Tests
{
    private static void Near(double observed, double expected, double tolerance = 1e-12)
    {
        Assert.IsTrue(double.IsFinite(observed), "A reported binary64 result must be finite.");
        Assert.IsTrue(global::System.Math.Abs(observed - expected) <= tolerance,
            $"Expected {expected:R}, observed {observed:R}, tolerance {tolerance:R}.");
    }

    [TestMethod]
    public void FourObservationHandOraclesDistinguishPopulationAndSample()
    {
        var result=DescriptiveStatistics64.Analyze([1d,2d,3d,4d]);
        Assert.AreEqual(4,result.Count);
        Near(result.Sum,10);
        Near(result.Mean,2.5d);
        Near(result.PopulationVariance,1.25d);
        Near(result.SampleVariance!.Value,5d/3d);
        Near(result.Minimum,1d);
        Near(result.Maximum,4d);
        Assert.AreEqual(DescriptivePrecisionKind.FiniteBinary64,result.Precision);
    }

    [TestMethod]
    public void CompensatedSummationPreservesLowOrderUnitInCancellation()
    {
        var result=DescriptiveStatistics64.Analyze([1e16,1d,-1e16]);
        Near(result.Sum,1d,0d);
        Near(result.Mean,1d/3d,1e-16);
    }

    [TestMethod]
    public void CompensatedSummationPreservesLowOrderUnitInReverseOrder()
    {
        var result=DescriptiveStatistics64.Analyze([-1e16,1d,1e16]);
        Near(result.Sum,1d,0d);
    }

    [TestMethod]
    public void VarianceUsesCenteredDifferenceRatherThanTwoLargeSquares()
    {
        var result=DescriptiveStatistics64.Analyze([1e16,1e16+2d]);
        Near(result.PopulationVariance,1d,0d);
        Near(result.SampleVariance!.Value,2d,0d);
    }

    [TestMethod]
    public void TwoObservationsUseDifferentVarianceDivisors()
    {
        var result=DescriptiveStatistics64.Analyze([1d,3d]);
        Near(result.Mean,2d);
        Near(result.PopulationVariance,1d);
        Near(result.SampleVariance!.Value,2d);
    }

    [TestMethod]
    public void SingletonProvidesPopulationVarianceButNoSampleVariance()
    {
        var result=DescriptiveStatistics64.Analyze([7d]);
        Assert.AreEqual(1,result.Count);
        Near(result.Mean,7d);
        Near(result.PopulationVariance,0d);
        Assert.IsNull(result.SampleVariance);
    }

    [TestMethod]
    public void AllZeroObservationsAreFinite()
    {
        var result=DescriptiveStatistics64.Analyze([0d,0d,0d]);
        Near(result.Sum,0d);
        Near(result.PopulationVariance,0d);
        Near(result.Minimum,0d);
        Near(result.Maximum,0d);
    }

    [TestMethod]
    public void ExactRationalsPreserveOneThirdPlusOneSixth()
    {
        var exact=DescriptiveStatistics64.AnalyzeExact([
            new ExactRational(1,3),new ExactRational(1,6)]);
        Assert.AreEqual(2,exact.Count);
        Assert.AreEqual(new ExactRational(1,2),exact.Sum);
        Assert.AreEqual(new ExactRational(1,4),exact.Mean);
        Assert.AreEqual(new ExactRational(1,6),exact.Minimum);
        Assert.AreEqual(new ExactRational(1,3),exact.Maximum);
    }

    [TestMethod]
    public void ExactMeanAndExtremaDoNotWidenToBinary64()
    {
        var exact=DescriptiveStatistics64.AnalyzeExact([
            new ExactRational(1,1),new ExactRational(2,1),
            new ExactRational(3,1),new ExactRational(4,1)]);
        Assert.AreEqual(new ExactRational(10,1),exact.Sum);
        Assert.AreEqual(new ExactRational(5,2),exact.Mean);
        Assert.AreEqual(ExactRational.One,exact.Minimum);
        Assert.AreEqual(new ExactRational(4,1),exact.Maximum);
    }

    [TestMethod]
    public void ExactSummationAllowsSignedCancellation()
    {
        var exact=DescriptiveStatistics64.AnalyzeExact([
            new ExactRational(-1,3),new ExactRational(1,3)]);
        Assert.AreEqual(ExactRational.Zero,exact.Sum);
        Assert.AreEqual(ExactRational.Zero,exact.Mean);
    }

    [TestMethod]
    public void EmptyFloatingInputsRefuseImplicitStatistics()
    {
        Assert.ThrowsExactly<ArgumentException>(()=>
            DescriptiveStatistics64.Analyze(Array.Empty<double>()));
    }

    [TestMethod]
    public void EmptyExactInputsRefuseImplicitStatistics()
    {
        Assert.ThrowsExactly<ArgumentException>(()=>
            DescriptiveStatistics64.AnalyzeExact(Array.Empty<ExactRational>()));
    }

    [TestMethod]
    public void NonfiniteInputsCannotLeakIntoFiniteStatistics()
    {
        foreach(var value in new[]{double.NaN,double.PositiveInfinity,double.NegativeInfinity})
            Assert.ThrowsExactly<ArithmeticException>(()=>
                DescriptiveStatistics64.Analyze([1d,value,2d]));
    }

    [TestMethod]
    public void OverflowedSumIsNotMisreportedAsFinite()
    {
        Assert.ThrowsExactly<ArithmeticException>(()=>
            DescriptiveStatistics64.Analyze([double.MaxValue,double.MaxValue]));
    }

    [TestMethod]
    public void OverflowedVarianceIsNotMisreportedAsFinite()
    {
        Assert.ThrowsExactly<ArithmeticException>(()=>
            DescriptiveStatistics64.Analyze([double.MaxValue,-double.MaxValue]));
    }

    [TestMethod]
    public void ObservationLimitIsCheckedBeforeUnboundedMaterialization()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            DescriptiveStatistics64.Analyze(Enumerable.Repeat(1d,4097)));
        var result=DescriptiveStatistics64.Analyze(Enumerable.Repeat(1d,4096));
        Assert.AreEqual(4096,result.Count);
    }

    [TestMethod]
    public void ExactObservationLimitIsAlsoBounded()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            DescriptiveStatistics64.AnalyzeExact(
                Enumerable.Repeat(ExactRational.One,4097)));
    }

    [TestMethod]
    public void Exact4097BitOperandIsRefused()
    {
        var huge=new ExactRational(BigInteger.One<<4096,BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            DescriptiveStatistics64.AnalyzeExact([huge]));
    }

    [TestMethod]
    public void ProductAssemblyDoesNotReferenceProviderOrAgent()
    {
        var references=typeof(DescriptiveStatistics64).Assembly.GetReferencedAssemblies()
            .Select(x=>x.Name??string.Empty).ToArray();
        Assert.IsFalse(references.Any(x=>
            x.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            x.Contains("Decision",StringComparison.OrdinalIgnoreCase)));
    }
}
