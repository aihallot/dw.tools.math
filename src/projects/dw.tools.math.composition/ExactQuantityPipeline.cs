using System.Collections.Immutable;
using dw.quantities;

namespace Dw.Tools.Math.Composition;

/// <summary>Closed exact-quantity pipeline vocabulary; no provider or expression evaluation.</summary>
public enum ExactPipelineStepKind { Start, Convert, Divide, Add }

public sealed record ExactPipelineStep
{
    private ExactPipelineStep(ExactPipelineStepKind kind, ExactRational amount, UnitDefinition unit)
    {
        Kind = kind;
        Amount = amount;
        Unit = unit;
    }

    public ExactPipelineStepKind Kind { get; }
    public ExactRational Amount { get; }
    public UnitDefinition Unit { get; }

    public static ExactPipelineStep Start(ExactRational amount, UnitDefinition unit) =>
        new(ExactPipelineStepKind.Start, amount, Require(unit));

    public static ExactPipelineStep Convert(UnitDefinition unit) =>
        new(ExactPipelineStepKind.Convert, ExactRational.Zero, Require(unit));

    public static ExactPipelineStep Divide(ExactRational amount, UnitDefinition unit) =>
        new(ExactPipelineStepKind.Divide, amount, Require(unit));

    public static ExactPipelineStep Add(ExactRational amount, UnitDefinition unit) =>
        new(ExactPipelineStepKind.Add, amount, Require(unit));

    private static UnitDefinition Require(UnitDefinition? unit) =>
        unit ?? throw new ArgumentNullException(nameof(unit));
}

/// <summary>Exact base quantity, optional presentation, and bounded step provenance.</summary>
public sealed record ExactPipelineResult
{
    internal ExactPipelineResult(Quantity baseQuantity, UnitDefinition? presentationUnit,
        ImmutableArray<ExactPipelineStepKind> steps)
    {
        BaseQuantity = baseQuantity;
        PresentationUnit = presentationUnit;
        Steps = steps;
    }

    public Quantity BaseQuantity { get; }
    public UnitDefinition? PresentationUnit { get; }
    public ExactRational DisplayValue => PresentationUnit is null
        ? BaseQuantity.Value : PresentationUnit.FromBase(BaseQuantity.Value);
    public ImmutableArray<ExactPipelineStepKind> Steps { get; }
}

/// <summary>
/// Immutable fluent syntax and direct invocation share one validated evaluation path.
/// All unit conversions are linear and exact; no AURA or provider calls.
/// </summary>
public sealed class ExactQuantityPipeline
{
    public const int MaximumSteps = 16;
    public const int MaximumNumeralDigits = 256;
    private readonly ImmutableArray<ExactPipelineStep> _steps;

    private ExactQuantityPipeline(ImmutableArray<ExactPipelineStep> steps) => _steps = steps;

    public static ExactQuantityPipeline From(ExactRational amount, UnitDefinition unit) =>
        new(ImmutableArray.Create(ExactPipelineStep.Start(amount, unit)));

    public ExactQuantityPipeline ConvertTo(UnitDefinition unit) => Append(ExactPipelineStep.Convert(unit));
    public ExactQuantityPipeline DivideBy(ExactRational amount, UnitDefinition unit) =>
        Append(ExactPipelineStep.Divide(amount, unit));
    public ExactQuantityPipeline Add(ExactRational amount, UnitDefinition unit) =>
        Append(ExactPipelineStep.Add(amount, unit));

    public ExactPipelineResult Evaluate() => Evaluate(_steps);

    /// <summary>Validates the complete chain before performing any quantity arithmetic.</summary>
    public static ExactPipelineResult Evaluate(IEnumerable<ExactPipelineStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        var finite = steps.Take(MaximumSteps + 1).ToImmutableArray();
        Validate(finite);
        var quantity = new Quantity(finite[0].Unit.ToBase(finite[0].Amount),
            finite[0].Unit.Dimension);
        CheckMagnitude(quantity.Value);
        UnitDefinition? presentation = finite[0].Unit;
        for (var i = 1; i < finite.Length; i++)
        {
            var step = finite[i];
            switch (step.Kind)
            {
                case ExactPipelineStepKind.Convert:
                    presentation = step.Unit;
                    break;
                case ExactPipelineStepKind.Divide:
                    quantity = quantity.Divide(new Quantity(step.Unit.ToBase(step.Amount),
                        step.Unit.Dimension));
                    presentation = null;
                    break;
                case ExactPipelineStepKind.Add:
                    if (!quantity.TryAdd(new Quantity(step.Unit.ToBase(step.Amount),
                            step.Unit.Dimension), out var summed))
                        throw new InvalidOperationException("Incompatible quantity dimensions.");
                    quantity = summed;
                    presentation = null;
                    break;
                default:
                    throw new InvalidOperationException("Unrecognized pipeline step.");
            }
            CheckMagnitude(quantity.Value);
        }
        var trace = finite.Select(step => step.Kind).ToImmutableArray();
        var result = new ExactPipelineResult(quantity, presentation, trace);
        CheckMagnitude(result.DisplayValue);
        return result;
    }

    private ExactQuantityPipeline Append(ExactPipelineStep step)
    {
        if (_steps.Length >= MaximumSteps)
            throw new ArgumentOutOfRangeException(nameof(step), "Pipeline step budget exceeded.");
        return new(_steps.Add(step));
    }

    private static void Validate(ImmutableArray<ExactPipelineStep> steps)
    {
        if (steps.IsDefaultOrEmpty || steps.Length > MaximumSteps ||
            steps[0] is null || steps[0].Kind != ExactPipelineStepKind.Start)
            throw new ArgumentException("A bounded pipeline must begin with Start.", nameof(steps));
        var dimension = steps[0].Unit.Dimension;
        for (var i = 0; i < steps.Length; i++)
        {
            var step = steps[i] ??
                throw new ArgumentException("Null step is not admitted.", nameof(steps));
            ValidateUnit(step.Unit);
            CheckMagnitude(step.Amount);
            switch (step.Kind)
            {
                case ExactPipelineStepKind.Start when i == 0:
                    break;
                case ExactPipelineStepKind.Convert when i > 0:
                    if (step.Unit.Dimension != dimension)
                        throw new ArgumentException("Convert requires the current dimension.", nameof(steps));
                    break;
                case ExactPipelineStepKind.Add when i > 0:
                    if (step.Unit.Dimension != dimension)
                        throw new ArgumentException("Addition requires equal dimensions.", nameof(steps));
                    break;
                case ExactPipelineStepKind.Divide when i > 0:
                    if (step.Amount == ExactRational.Zero)
                        throw new DivideByZeroException("Pipeline divisors cannot be zero.");
                    dimension -= step.Unit.Dimension;
                    break;
                default:
                    throw new ArgumentException("Invalid step order or operation.", nameof(steps));
            }
        }
    }

    private static void ValidateUnit(UnitDefinition unit)
    {
        if (unit.TransformKind != UnitTransformKind.Linear ||
            unit.OffsetToBase != ExactRational.Zero)
            throw new NotSupportedException("Affine or non-linear units are outside this exact pipeline.");
        CheckMagnitude(unit.ScaleToBase);
    }

    private static void CheckMagnitude(ExactRational value)
    {
        if (value.Numerator.ToString(System.Globalization.CultureInfo.InvariantCulture).Length >
                MaximumNumeralDigits + 1 ||
            value.Denominator.ToString(System.Globalization.CultureInfo.InvariantCulture).Length >
                MaximumNumeralDigits)
            throw new ArgumentOutOfRangeException(nameof(value), "Exact numerator/denominator budget exceeded.");
    }
}
