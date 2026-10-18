using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>An angular input unit is mandatory for real trigonometry.</summary>
public enum SpecialAngleInput64 { NotApplicable, Radians, Degrees }

/// <summary>The negative real log cut requires explicit principal-branch acceptance.</summary>
public enum ComplexLogCutPolicy64 { RejectNegativeRealCut, CallerAcceptsPrincipalCut }

/// <summary>Typed, nonpartial refusals retain the scientific reason, not a partial value.</summary>
public enum SpecialBoundaryStatus64
{
    Success, InvalidInput, InvalidPolicy, UnsupportedDomain, GammaPole,
    ComplexGammaUnsupported, BranchCutRequiresOptIn, AngleUnitRequired,
    OutsideQualifiedRange, OverflowRisk, UnderflowRisk, NonFiniteResult, Cancelled
}

/// <summary>Finite real observation with optional converted radian input.</summary>
public sealed record SpecialRealBoundaryResult64(
    SpecialBoundaryStatus64 Status, double? Value, double? EffectiveRadians,
    SpecialAngleInput64 InputAngleUnit, string ProviderId, string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded => Status == SpecialBoundaryStatus64.Success &&
        Value is { } v && double.IsFinite(v) && !ErrorBoundGuaranteed;
}

/// <summary>Finite complex principal-value observation, not a branch/error theorem.</summary>
public sealed record SpecialComplexBoundaryResult64(
    SpecialBoundaryStatus64 Status, Complex? Value,
    ComplexLogCutPolicy64 CutPolicy, string ProviderId, string RuleId,
    bool ErrorBoundGuaranteed)
{
    public bool Succeeded => Status == SpecialBoundaryStatus64.Success &&
        Value is { } v && double.IsFinite(v.Real) && double.IsFinite(v.Imaginary) &&
        !ErrorBoundGuaranteed;
}

/// <summary>
/// An explicit, fail-closed boundary sidecar preserving T1's managed backend.
/// All accepted values are approximate binary64 and no precision proof is made.
/// </summary>
public static class SpecialFunctionBoundaryAudit64
{
    public const string RuleId = "dw-math-special-function-boundaries-1";

    public static SpecialRealBoundaryResult64 EvaluateReal(
        SpecialFunctionKind64 kind, double argument,
        SpecialAngleInput64 angleUnit = SpecialAngleInput64.NotApplicable,
        SpecialFunctionPolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RejectReal(SpecialBoundaryStatus64.Cancelled,angleUnit);
        if(!double.IsFinite(argument))
            return RejectReal(SpecialBoundaryStatus64.InvalidInput,angleUnit);
        if(angleUnit is not (SpecialAngleInput64.NotApplicable or
            SpecialAngleInput64.Radians or SpecialAngleInput64.Degrees))
            return RejectReal(SpecialBoundaryStatus64.InvalidPolicy,angleUnit);

        var angular = kind is SpecialFunctionKind64.Sine or SpecialFunctionKind64.Cosine;
        if(angular && angleUnit == SpecialAngleInput64.NotApplicable)
            return RejectReal(SpecialBoundaryStatus64.AngleUnitRequired,angleUnit);
        if(!angular && angleUnit != SpecialAngleInput64.NotApplicable)
            return RejectReal(SpecialBoundaryStatus64.InvalidPolicy,angleUnit);

        double? radians = angular
            ? angleUnit==SpecialAngleInput64.Degrees
                ? argument*(global::System.Math.PI/180d) : argument
            : null;
        if(angular && (!double.IsFinite(radians!.Value) ||
            (angleUnit==SpecialAngleInput64.Degrees && argument!=0d &&
             radians.Value==0d)))
            return RejectReal(SpecialBoundaryStatus64.UnderflowRisk,angleUnit);

        var effective = radians ?? argument;
        if(kind == SpecialFunctionKind64.Gamma && argument<=0d &&
           global::System.Math.Truncate(argument)==argument)
            return RejectReal(SpecialBoundaryStatus64.GammaPole,angleUnit);

        if(kind == SpecialFunctionKind64.Exponential)
        {
            if(effective>709d)
                return RejectReal(SpecialBoundaryStatus64.OverflowRisk,angleUnit);
            if(effective< -745d)
                return RejectReal(SpecialBoundaryStatus64.UnderflowRisk,angleUnit);
        }

        var result=BoundedSpecialFunctions64.EvaluateReal(
            kind,effective,policy,cancellationToken);
        if(!result.Succeeded)
            return RejectReal(Map(result.Status),angleUnit);
        return new(SpecialBoundaryStatus64.Success,result.Value,radians,angleUnit,
            result.ProviderId,RuleId,false);
    }

    public static SpecialComplexBoundaryResult64 EvaluateComplex(
        SpecialFunctionKind64 kind,Complex argument,
        ComplexLogCutPolicy64 cutPolicy = ComplexLogCutPolicy64.RejectNegativeRealCut,
        SpecialFunctionPolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RejectComplex(SpecialBoundaryStatus64.Cancelled,cutPolicy);
        if(!double.IsFinite(argument.Real)||!double.IsFinite(argument.Imaginary))
            return RejectComplex(SpecialBoundaryStatus64.InvalidInput,cutPolicy);
        if(cutPolicy is not (ComplexLogCutPolicy64.RejectNegativeRealCut or
            ComplexLogCutPolicy64.CallerAcceptsPrincipalCut))
            return RejectComplex(SpecialBoundaryStatus64.InvalidPolicy,cutPolicy);
        if(kind!=SpecialFunctionKind64.NaturalLogarithm &&
           cutPolicy==ComplexLogCutPolicy64.CallerAcceptsPrincipalCut)
            return RejectComplex(SpecialBoundaryStatus64.InvalidPolicy,cutPolicy);

        if(kind==SpecialFunctionKind64.Gamma)
            return RejectComplex(SpecialBoundaryStatus64.ComplexGammaUnsupported,cutPolicy);
        if(kind==SpecialFunctionKind64.NaturalLogarithm &&
           argument.Real<0d && argument.Imaginary==0d &&
           cutPolicy==ComplexLogCutPolicy64.RejectNegativeRealCut)
            return RejectComplex(SpecialBoundaryStatus64.BranchCutRequiresOptIn,cutPolicy);
        if(kind==SpecialFunctionKind64.Exponential)
        {
            if(argument.Real>709d)
                return RejectComplex(SpecialBoundaryStatus64.OverflowRisk,cutPolicy);
            if(argument.Real< -745d)
                return RejectComplex(SpecialBoundaryStatus64.UnderflowRisk,cutPolicy);
        }
        var result=BoundedSpecialFunctions64.EvaluateComplex(kind,argument,
            policy,cancellationToken);
        if(!result.Succeeded)
            return RejectComplex(Map(result.Status),cutPolicy);
        return new(SpecialBoundaryStatus64.Success,result.Value,cutPolicy,
            result.ProviderId,RuleId,false);
    }

    private static SpecialBoundaryStatus64 Map(SpecialFunctionStatus64 status) =>
        status switch
        {
            SpecialFunctionStatus64.InvalidInput => SpecialBoundaryStatus64.InvalidInput,
            SpecialFunctionStatus64.InvalidPolicy => SpecialBoundaryStatus64.InvalidPolicy,
            SpecialFunctionStatus64.UnsupportedDomain => SpecialBoundaryStatus64.UnsupportedDomain,
            SpecialFunctionStatus64.OutsideQualifiedRange => SpecialBoundaryStatus64.OutsideQualifiedRange,
            SpecialFunctionStatus64.NonFiniteResult => SpecialBoundaryStatus64.NonFiniteResult,
            SpecialFunctionStatus64.Cancelled => SpecialBoundaryStatus64.Cancelled,
            _ => SpecialBoundaryStatus64.NonFiniteResult
        };

    private static SpecialRealBoundaryResult64 RejectReal(
        SpecialBoundaryStatus64 status,SpecialAngleInput64 unit) =>
        new(status,null,null,unit,
            ManagedBclSpecialFunctionsProvider64.ProviderIdentity,RuleId,false);

    private static SpecialComplexBoundaryResult64 RejectComplex(
        SpecialBoundaryStatus64 status,ComplexLogCutPolicy64 policy) =>
        new(status,null,policy,
            ManagedBclSpecialFunctionsProvider64.ProviderIdentity,RuleId,false);
}
