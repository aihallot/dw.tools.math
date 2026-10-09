using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C02Tests
{
    private static readonly UnitDefinition Km = Linear("km", DimensionVector.LengthDimension,
        new ExactRational(1000, 1));
    private static readonly UnitDefinition Metre = Linear("m", DimensionVector.LengthDimension,
        ExactRational.One);
    private static readonly UnitDefinition Minute = Linear("min", DimensionVector.TimeDimension,
        new ExactRational(60, 1));
    private static readonly UnitDefinition Second = Linear("s", DimensionVector.TimeDimension,
        ExactRational.One);
    private static readonly UnitDefinition KilometresPerHour = Linear("km/h",
        DimensionVector.LengthDimension - DimensionVector.TimeDimension, new ExactRational(5, 18));
    private static readonly UnitDefinition Kilogram = Linear("kg", DimensionVector.MassDimension,
        ExactRational.One);

    private static UnitDefinition Linear(string symbol, DimensionVector dimension, ExactRational scale) =>
        new(symbol, symbol, symbol, UnitSystem.Si, dimension, scale,
            ExactRational.Zero, UnitTransformKind.Linear);

    [TestMethod]
    public void FiveKilometresInTwoMinutesEqualsExactly150KilometresPerHour()
    {
        var fluent = ExactQuantityPipeline.From(new ExactRational(5, 1), Km)
            .ConvertTo(Metre)
            .DivideBy(new ExactRational(2, 1), Minute)
            .ConvertTo(KilometresPerHour)
            .Evaluate();
        Assert.AreEqual(new ExactRational(150, 1), fluent.DisplayValue,
            "M2-W02-C02-T1 RED: 5 km / 2 min must equal exactly 150 km/h.");
        Assert.AreEqual(DimensionVector.LengthDimension - DimensionVector.TimeDimension,
            fluent.BaseQuantity.Dimension);
        Assert.AreEqual(new ExactRational(125, 3), fluent.BaseQuantity.Value);
        Assert.AreSame(KilometresPerHour, fluent.PresentationUnit);
        Assert.AreEqual(4, fluent.Steps.Length);
    }

    [TestMethod]
    public void DirectAndFluentApisUseTheSameSemanticEvaluation()
    {
        var direct = ExactQuantityPipeline.Evaluate(new[] {
            ExactPipelineStep.Start(new ExactRational(5, 1), Km),
            ExactPipelineStep.Convert(Metre),
            ExactPipelineStep.Divide(new ExactRational(2, 1), Minute),
            ExactPipelineStep.Convert(KilometresPerHour)
        });
        var fluent = ExactQuantityPipeline.From(new ExactRational(5, 1), Km)
            .ConvertTo(Metre).DivideBy(new ExactRational(2, 1), Minute)
            .ConvertTo(KilometresPerHour).Evaluate();
        Assert.AreEqual(fluent.BaseQuantity, direct.BaseQuantity);
        Assert.AreEqual(fluent.DisplayValue, direct.DisplayValue);
        CollectionAssert.AreEqual(fluent.Steps.ToArray(), direct.Steps.ToArray());
    }

    [TestMethod]
    public void MassPlusTimeIsRejectedAtPrecomputationValidation()
    {
        var steps = new[] {
            ExactPipelineStep.Start(new ExactRational(1, 1), Kilogram),
            ExactPipelineStep.Add(new ExactRational(3, 1), Second)
        };
        Assert.ThrowsExactly<ArgumentException>(() => ExactQuantityPipeline.Evaluate(steps));
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactQuantityPipeline.From(new ExactRational(1, 1), Kilogram)
                .Add(new ExactRational(3, 1), Second).Evaluate());
    }

    [TestMethod]
    public void IncompatibleConversionIsRejectedBeforeArithmetic()
    {
        var steps = new[] {
            ExactPipelineStep.Start(new ExactRational(5, 1), Km),
            ExactPipelineStep.Convert(Minute)
        };
        Assert.ThrowsExactly<ArgumentException>(() => ExactQuantityPipeline.Evaluate(steps));
    }

    [TestMethod]
    public void ZeroDivisorIsRefusedBeforeRunningAnyStep()
    {
        var steps = new[] {
            ExactPipelineStep.Start(new ExactRational(5, 1), Km),
            ExactPipelineStep.Divide(ExactRational.Zero, Minute)
        };
        Assert.ThrowsExactly<DivideByZeroException>(() => ExactQuantityPipeline.Evaluate(steps));
    }

    [TestMethod]
    public void PipelinesHaveFiniteSixteenStepBudget()
    {
        var pipeline = ExactQuantityPipeline.From(ExactRational.One, Metre);
        for (var i = 0; i < 15; i++) pipeline = pipeline.ConvertTo(Metre);
        Assert.AreEqual(16, pipeline.Evaluate().Steps.Length);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => pipeline.ConvertTo(Metre));
        var overBudget = new List<ExactPipelineStep> {
            ExactPipelineStep.Start(ExactRational.One, Metre) };
        for (var i = 0; i < 16; i++) overBudget.Add(ExactPipelineStep.Convert(Metre));
        Assert.ThrowsExactly<ArgumentException>(() => ExactQuantityPipeline.Evaluate(overBudget));
    }

    [TestMethod]
    public void ExactFractionAndDisplayConversionsNeverUseBinaryFloatingPoint()
    {
        var result = ExactQuantityPipeline.From(new ExactRational(1, 3), Km)
            .ConvertTo(Metre).Evaluate();
        Assert.AreEqual(new ExactRational(1000, 3), result.BaseQuantity.Value);
        Assert.AreEqual(new ExactRational(1000, 3), result.DisplayValue);
        Assert.AreSame(Metre, result.PresentationUnit);
    }

    [TestMethod]
    public void InputChainCannotStartWithConversionOrRepeatStart()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactQuantityPipeline.Evaluate(new[] { ExactPipelineStep.Convert(Metre) }));
        Assert.ThrowsExactly<ArgumentException>(() => ExactQuantityPipeline.Evaluate(
            new[] { ExactPipelineStep.Start(ExactRational.One, Metre),
                ExactPipelineStep.Start(ExactRational.One, Metre) }));
    }

    [TestMethod]
    public void AffineUnitsAndOversizedNumbersAreRefused()
    {
        var absolute = new UnitDefinition("degC", "degC", "Celsius", UnitSystem.Si,
            DimensionVector.TemperatureDimension, ExactRational.One,
            new ExactRational(27315, 100), UnitTransformKind.AbsoluteTemperature);
        Assert.ThrowsExactly<NotSupportedException>(() =>
            ExactQuantityPipeline.From(ExactRational.One, absolute).Evaluate());
        var huge = new ExactRational(BigInteger.Pow(10, 260), BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(huge, Metre).Evaluate());
    }

    [TestMethod]
    public void FluentAppendDoesNotMutateAnExistingPipeline()
    {
        var original = ExactQuantityPipeline.From(new ExactRational(5, 1), Km);
        var converted = original.ConvertTo(Metre);
        Assert.AreEqual(1, original.Evaluate().Steps.Length);
        Assert.AreEqual(new ExactRational(5, 1), original.Evaluate().DisplayValue);
        Assert.AreEqual(2, converted.Evaluate().Steps.Length);
        Assert.AreEqual(new ExactRational(5000, 1), converted.Evaluate().DisplayValue);
    }
}
