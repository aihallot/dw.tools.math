using System;

namespace Dw.Tools.Math.Analysis;

/// <summary>Explicit policies and nonpartial outcomes for numerical audits.</summary>
public enum DifferentialAuditStatus64
{
    Success, InvalidInput, InvalidPolicy, BudgetExceeded,
    StepNotRepresentable, NonFiniteEvaluation, NonFiniteIntermediate,
    EvaluationFailed, Cancelled, EstimateNotMet,
    RegularityUnspecified, OdeRegimeUnspecified, UnsupportedRegime
}

/// <summary>Caller-provided mathematical assumption, not a regularity test.</summary>
public enum DerivativeRegularity64 { Unspecified, CallerAssumesSmooth }

/// <summary>Only declared nonstiff scalar IVPs without events are admitted.</summary>
public enum OdeRegime64
{
    Unspecified, CallerAssumesNonStiffNoEvents, KnownStiff,
    EventDriven, PartialDifferentialEquation
}

/// <summary>No numerical observation implies an independently proven derivative.</summary>
public sealed record AuditedDerivative64(
    DifferentialAuditStatus64 Status,
    double? Value, double? EstimatedAbsoluteError, double? UsedStep,
    int FunctionEvaluations, bool SmoothnessIndependentlyVerified,
    bool LocalErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded =>
        Status == DifferentialAuditStatus64.Success &&
        Value is { } value && double.IsFinite(value) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && UsedStep is { } step && double.IsFinite(step) &&
        step > 0d && FunctionEvaluations == 4 &&
        !SmoothnessIndependentlyVerified && !LocalErrorBoundGuaranteed;
}

/// <summary>Caller assertions about stiffness and events are never certified.</summary>
public sealed record AuditedOde64(
    DifferentialAuditStatus64 Status,
    double? FinalValue, double? EstimatedAbsoluteError,
    int Steps, int FunctionEvaluations,
    bool NonStiffRegimeIndependentlyVerified,
    bool EventAbsenceIndependentlyVerified,
    bool GlobalErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded =>
        Status == DifferentialAuditStatus64.Success &&
        FinalValue is { } value && double.IsFinite(value) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && Steps > 0 && FunctionEvaluations > 0 &&
        !NonStiffRegimeIndependentlyVerified &&
        !EventAbsenceIndependentlyVerified && !GlobalErrorBoundGuaranteed;
}

/// <summary>
/// A fail-closed policy sidecar around the qualified finite-difference and
/// scalar RK4 algorithms. Smoothness/nonstiffness/event absence must be
/// explicitly asserted by the caller and are NEVER proven by this API.
/// The original T1 calculations remain unchanged.
/// </summary>
public static class DifferentialBoundaryAudit64
{
    public const string RuleId = "dw-math-differential-boundary-binary64-1";

    public static AuditedDerivative64 Differentiate(
        Func<double,double>? function, double at,
        DerivativeRegularity64 regularity = DerivativeRegularity64.Unspecified,
        DerivativePolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RefuseDerivative(DifferentialAuditStatus64.Cancelled);
        if(regularity is not (DerivativeRegularity64.Unspecified or
            DerivativeRegularity64.CallerAssumesSmooth))
            return RefuseDerivative(DifferentialAuditStatus64.InvalidPolicy);
        if(regularity == DerivativeRegularity64.Unspecified)
            return RefuseDerivative(DifferentialAuditStatus64.RegularityUnspecified);

        var observed = BoundedDifferentialAnalysis64.Differentiate(
            function,at,policy,cancellationToken);
        if(!observed.Succeeded)
            return RefuseDerivative(Map(observed.Status));
        if(observed.Value is not { } value ||
           observed.EstimatedAbsoluteError is not { } error ||
           observed.UsedStep is not { } step ||
           !double.IsFinite(value) || !double.IsFinite(error) ||
           !double.IsFinite(step))
            return RefuseDerivative(DifferentialAuditStatus64.NonFiniteIntermediate);
        return new(DifferentialAuditStatus64.Success, value, error, step,
            observed.FunctionEvaluations, false, false, RuleId);
    }

    public static AuditedOde64 IntegrateOde(
        Func<double,double,double>? derivative,
        double startTime, double endTime, double initialValue,
        OdeRegime64 regime = OdeRegime64.Unspecified,
        OdePolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RefuseOde(DifferentialAuditStatus64.Cancelled);
        if(regime == OdeRegime64.Unspecified)
            return RefuseOde(DifferentialAuditStatus64.OdeRegimeUnspecified);
        if(regime is OdeRegime64.KnownStiff or OdeRegime64.EventDriven or
           OdeRegime64.PartialDifferentialEquation)
            return RefuseOde(DifferentialAuditStatus64.UnsupportedRegime);
        if(regime != OdeRegime64.CallerAssumesNonStiffNoEvents)
            return RefuseOde(DifferentialAuditStatus64.InvalidPolicy);

        var observed = BoundedDifferentialAnalysis64.IntegrateOde(
            derivative,startTime,endTime,initialValue,policy,cancellationToken);
        if(!observed.Succeeded)
            return RefuseOde(Map(observed.Status));
        if(observed.FinalValue is not { } value ||
           observed.EstimatedAbsoluteError is not { } error ||
           !double.IsFinite(value) || !double.IsFinite(error))
            return RefuseOde(DifferentialAuditStatus64.NonFiniteIntermediate);
        return new(DifferentialAuditStatus64.Success,value,error,
            observed.Steps,observed.FunctionEvaluations,
            false,false,false,RuleId);
    }

    private static DifferentialAuditStatus64 Map(DifferentialStatus64 status) =>
        status switch
        {
            DifferentialStatus64.InvalidInput => DifferentialAuditStatus64.InvalidInput,
            DifferentialStatus64.InvalidPolicy => DifferentialAuditStatus64.InvalidPolicy,
            DifferentialStatus64.BudgetExceeded => DifferentialAuditStatus64.BudgetExceeded,
            DifferentialStatus64.StepNotRepresentable =>
                DifferentialAuditStatus64.StepNotRepresentable,
            DifferentialStatus64.NonFiniteEvaluation =>
                DifferentialAuditStatus64.NonFiniteEvaluation,
            DifferentialStatus64.NonFiniteIntermediate =>
                DifferentialAuditStatus64.NonFiniteIntermediate,
            DifferentialStatus64.EvaluationFailed =>
                DifferentialAuditStatus64.EvaluationFailed,
            DifferentialStatus64.Cancelled => DifferentialAuditStatus64.Cancelled,
            DifferentialStatus64.EstimateNotMet =>
                DifferentialAuditStatus64.EstimateNotMet,
            _ => DifferentialAuditStatus64.NonFiniteIntermediate
        };

    private static AuditedDerivative64 RefuseDerivative(DifferentialAuditStatus64 status) =>
        new(status,null,null,null,0,false,false,RuleId);

    private static AuditedOde64 RefuseOde(DifferentialAuditStatus64 status) =>
        new(status,null,null,0,0,false,false,false,RuleId);
}
