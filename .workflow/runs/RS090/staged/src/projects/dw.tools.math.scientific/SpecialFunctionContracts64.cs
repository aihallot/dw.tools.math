using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>Precisely named functions and a deliberately small, documented provider subset.</summary>
public enum SpecialFunctionKind64
{
    Sine, Cosine, Exponential, NaturalLogarithm, Gamma
}

/// <summary>No implicit angle-unit conversion: trigonometry uses radians only.</summary>
public enum SpecialAngleConvention64 { Radians }

/// <summary>Typed status for a finite managed numerical computation.</summary>
public enum SpecialFunctionStatus64
{
    Success, InvalidInput, InvalidPolicy, UnsupportedDomain,
    OutsideQualifiedRange, NonFiniteResult, Cancelled
}

/// <summary>Caller-bounded finite ranges, capped by the qualified managed backend.</summary>
public sealed record SpecialFunctionPolicy64(
    double MaximumRealMagnitude = 20d,
    double MaximumComplexComponentMagnitude = 12d);

/// <summary>A finite approximate real result or a nonpartial typed refusal.</summary>
public sealed record SpecialRealResult64(
    SpecialFunctionStatus64 Status, double? Value,
    string ProviderId, string RuleId, SpecialAngleConvention64 AngleConvention,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded => Status == SpecialFunctionStatus64.Success &&
        Value is { } v && double.IsFinite(v) && !ErrorBoundGuaranteed;
}

/// <summary>A finite approximate complex principal-value result or typed refusal.</summary>
public sealed record SpecialComplexResult64(
    SpecialFunctionStatus64 Status, Complex? Value,
    string ProviderId, string RuleId, SpecialAngleConvention64 AngleConvention,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded => Status == SpecialFunctionStatus64.Success &&
        Value is { } v && double.IsFinite(v.Real) && double.IsFinite(v.Imaginary) &&
        !ErrorBoundGuaranteed;
}

/// <summary>Provider boundary keeps backend types, approximation and admissibility visible.</summary>
public interface ISpecialFunctionsProvider64
{
    string ProviderId { get; }
    SpecialRealResult64 EvaluateReal(SpecialFunctionKind64 kind, double argument,
        SpecialFunctionPolicy64? policy = null, CancellationToken cancellationToken = default);
    SpecialComplexResult64 EvaluateComplex(SpecialFunctionKind64 kind, Complex argument,
        SpecialFunctionPolicy64? policy = null, CancellationToken cancellationToken = default);
}
