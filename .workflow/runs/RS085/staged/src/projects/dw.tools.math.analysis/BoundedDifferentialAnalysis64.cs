using System;

namespace Dw.Tools.Math.Analysis;

/// <summary>Finite, independently bounded scalar numerical calculations.</summary>
public enum DifferentialStatus64
{
    Success, InvalidInput, InvalidPolicy, BudgetExceeded, StepNotRepresentable,
    NonFiniteEvaluation, NonFiniteIntermediate, EvaluationFailed, Cancelled,
    EstimateNotMet
}

/// <summary>A caller-chosen finite difference step and heuristic agreement budget.</summary>
public sealed record DerivativePolicy64(
    double Step = 1e-3,
    double MaximumEstimatedError = 1e-6,
    int MaximumFunctionEvaluations = 4);

/// <summary>A finite forward interval and fixed-step fourth-order Runge-Kutta policy.</summary>
public sealed record OdePolicy64(
    int Steps = 32,
    double MaximumEstimatedError = 1e-6,
    int MaximumFunctionEvaluations = 1024);

/// <summary>A finite-difference approximation with an unverified step-refinement estimate.</summary>
public sealed record DerivativeResult64(
    DifferentialStatus64 Status, double? Value, double? EstimatedAbsoluteError,
    double? UsedStep, int FunctionEvaluations, bool ErrorBoundGuaranteed,
    string RuleId)
{
    public bool Succeeded => Status == DifferentialStatus64.Success &&
        Value is { } value && double.IsFinite(value) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && UsedStep is { } step && double.IsFinite(step) &&
        step > 0d && FunctionEvaluations == 4 && !ErrorBoundGuaranteed;
}

/// <summary>One finite final-time ODE approximation and a noncertified refinement estimate.</summary>
public sealed record OdeResult64(
    DifferentialStatus64 Status, double? FinalValue,
    double? EstimatedAbsoluteError, int Steps, int FunctionEvaluations,
    bool GlobalErrorBoundGuaranteed, string RuleId)
{
    public bool Succeeded => Status == DifferentialStatus64.Success &&
        FinalValue is { } value && double.IsFinite(value) &&
        EstimatedAbsoluteError is { } error && double.IsFinite(error) &&
        error >= 0d && Steps > 0 && FunctionEvaluations > 0 &&
        !GlobalErrorBoundGuaranteed;
}

/// <summary>
/// Bounded central difference and fixed-step RK4 on finite binary64 scalars.
/// Step doubling produces heuristic estimates, not certified error bounds.
/// No stiffness, event detection, PDE, discontinuity proof or agent dependency.
/// </summary>
public static class BoundedDifferentialAnalysis64
{
    public const string RuleId = "dw-math-bounded-differential-binary64-1";
    public const int MaximumOdeSteps = 256;
    private static readonly DerivativePolicy64 DefaultDerivative = new();
    private static readonly OdePolicy64 DefaultOde = new();

    public static DerivativeResult64 Differentiate(
        Func<double,double>? function, double at,
        DerivativePolicy64? policy = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return DerivativeRefuse(DifferentialStatus64.Cancelled);
        if(function is null || !double.IsFinite(at))
            return DerivativeRefuse(DifferentialStatus64.InvalidInput);
        var selected = policy ?? DefaultDerivative;
        if(!double.IsFinite(selected.Step) || selected.Step <= 0d ||
           selected.Step > 1d ||
           !double.IsFinite(selected.MaximumEstimatedError) ||
           selected.MaximumEstimatedError <= 0d ||
           selected.MaximumFunctionEvaluations is < 1 or > 1024)
            return DerivativeRefuse(DifferentialStatus64.InvalidPolicy);
        if(selected.MaximumFunctionEvaluations < 4)
            return DerivativeRefuse(DifferentialStatus64.BudgetExceeded);
        var half = selected.Step / 2d;
        var plus = at + selected.Step;
        var minus = at - selected.Step;
        var plusHalf = at + half;
        var minusHalf = at - half;
        if(!double.IsFinite(plus) || !double.IsFinite(minus) ||
           !double.IsFinite(plusHalf) || !double.IsFinite(minusHalf))
            return DerivativeRefuse(DifferentialStatus64.NonFiniteIntermediate);
        if(half <= 0d || plus == at || minus == at ||
           plusHalf == at || minusHalf == at)
            return DerivativeRefuse(DifferentialStatus64.StepNotRepresentable);
        if(cancellationToken.IsCancellationRequested)
            return DerivativeRefuse(DifferentialStatus64.Cancelled);
        var status = Evaluate(function,plus,out var upper);
        if(status!=DifferentialStatus64.Success) return DerivativeRefuse(status);
        if(cancellationToken.IsCancellationRequested)
            return DerivativeRefuse(DifferentialStatus64.Cancelled);
        status=Evaluate(function,minus,out var lower);
        if(status!=DifferentialStatus64.Success) return DerivativeRefuse(status);
        if(cancellationToken.IsCancellationRequested)
            return DerivativeRefuse(DifferentialStatus64.Cancelled);
        status=Evaluate(function,plusHalf,out var upperHalf);
        if(status!=DifferentialStatus64.Success) return DerivativeRefuse(status);
        if(cancellationToken.IsCancellationRequested)
            return DerivativeRefuse(DifferentialStatus64.Cancelled);
        status=Evaluate(function,minusHalf,out var lowerHalf);
        if(status!=DifferentialStatus64.Success) return DerivativeRefuse(status);
        var coarse=(upper-lower)/(2d*selected.Step);
        var fine=(upperHalf-lowerHalf)/(2d*half);
        var estimate=global::System.Math.Abs(fine-coarse)/3d;
        if(!double.IsFinite(coarse) || !double.IsFinite(fine) ||
           !double.IsFinite(estimate))
            return DerivativeRefuse(DifferentialStatus64.NonFiniteIntermediate);
        if(estimate>selected.MaximumEstimatedError)
            return DerivativeRefuse(DifferentialStatus64.EstimateNotMet);
        return new(DifferentialStatus64.Success,fine,estimate,half,4,false,RuleId);
    }

    public static OdeResult64 IntegrateOde(
        Func<double,double,double>? derivative, double startTime,
        double endTime, double initialValue,
        OdePolicy64? policy = null, CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return OdeRefuse(DifferentialStatus64.Cancelled);
        if(derivative is null || !double.IsFinite(startTime) ||
           !double.IsFinite(endTime) || endTime<=startTime ||
           !double.IsFinite(initialValue))
            return OdeRefuse(DifferentialStatus64.InvalidInput);
        var selected=policy??DefaultOde;
        if(selected.Steps is < 1 or > MaximumOdeSteps ||
           !double.IsFinite(selected.MaximumEstimatedError) ||
           selected.MaximumEstimatedError<=0d ||
           selected.MaximumFunctionEvaluations is < 1 or > 4096)
            return OdeRefuse(DifferentialStatus64.InvalidPolicy);
        var totalEvaluations=12*selected.Steps;
        if(totalEvaluations>selected.MaximumFunctionEvaluations)
            return OdeRefuse(DifferentialStatus64.BudgetExceeded);
        var duration=endTime-startTime;
        if(!double.IsFinite(duration))
            return OdeRefuse(DifferentialStatus64.NonFiniteIntermediate);
        var coarseStatus=SolveSteps(derivative,startTime,endTime,initialValue,
            selected.Steps,cancellationToken,out var coarse);
        if(coarseStatus!=DifferentialStatus64.Success)
            return OdeRefuse(coarseStatus);
        var fineStatus=SolveSteps(derivative,startTime,endTime,initialValue,
            2*selected.Steps,cancellationToken,out var fine);
        if(fineStatus!=DifferentialStatus64.Success)
            return OdeRefuse(fineStatus);
        var estimate=global::System.Math.Abs(fine-coarse)/15d;
        if(!double.IsFinite(estimate))
            return OdeRefuse(DifferentialStatus64.NonFiniteIntermediate);
        if(estimate>selected.MaximumEstimatedError)
            return OdeRefuse(DifferentialStatus64.EstimateNotMet);
        return new(DifferentialStatus64.Success,fine,estimate,
            2*selected.Steps,totalEvaluations,false,RuleId);
    }

    private static DifferentialStatus64 SolveSteps(
        Func<double,double,double> derivative, double begin, double end,
        double initial, int steps, CancellationToken cancellationToken,
        out double final)
    {
        final=double.NaN;
        var h=(end-begin)/steps;
        var half=h/2d;
        if(!double.IsFinite(h) || !(h>0d) || !(half>0d))
            return DifferentialStatus64.StepNotRepresentable;
        var y=initial;
        for(var i=0;i<steps;i++)
        {
            if(cancellationToken.IsCancellationRequested)
                return DifferentialStatus64.Cancelled;
            var t=begin+h*i;
            var next=i==steps-1?end:begin+h*(i+1);
            var middle=t+half;
            if(!double.IsFinite(t) || !double.IsFinite(next) ||
               !double.IsFinite(middle))
                return DifferentialStatus64.NonFiniteIntermediate;
            if(!(next>t) || !(middle>t) || !(middle<next))
                return DifferentialStatus64.StepNotRepresentable;
            var status=Evaluate(derivative,t,y,out var k1);
            if(status!=DifferentialStatus64.Success)return status;
            var middleY1=y+half*k1;
            if(!double.IsFinite(middleY1))
                return DifferentialStatus64.NonFiniteIntermediate;
            if(cancellationToken.IsCancellationRequested)
                return DifferentialStatus64.Cancelled;
            status=Evaluate(derivative,middle,middleY1,out var k2);
            if(status!=DifferentialStatus64.Success)return status;
            var middleY2=y+half*k2;
            if(!double.IsFinite(middleY2))
                return DifferentialStatus64.NonFiniteIntermediate;
            if(cancellationToken.IsCancellationRequested)
                return DifferentialStatus64.Cancelled;
            status=Evaluate(derivative,middle,middleY2,out var k3);
            if(status!=DifferentialStatus64.Success)return status;
            var finalY=y+h*k3;
            if(!double.IsFinite(finalY))
                return DifferentialStatus64.NonFiniteIntermediate;
            if(cancellationToken.IsCancellationRequested)
                return DifferentialStatus64.Cancelled;
            status=Evaluate(derivative,next,finalY,out var k4);
            if(status!=DifferentialStatus64.Success)return status;
            var combined=k1+2d*k2+2d*k3+k4;
            var nextY=y+(h/6d)*combined;
            if(!double.IsFinite(combined) || !double.IsFinite(nextY))
                return DifferentialStatus64.NonFiniteIntermediate;
            y=nextY;
        }
        final=y;
        return DifferentialStatus64.Success;
    }

    private static DifferentialStatus64 Evaluate(
        Func<double,double> function,double x,out double result)
    {
        result=double.NaN;
        try { result=function(x); }
        catch(OperationCanceledException) { return DifferentialStatus64.Cancelled; }
        catch(Exception) { return DifferentialStatus64.EvaluationFailed; }
        return double.IsFinite(result)?DifferentialStatus64.Success:
            DifferentialStatus64.NonFiniteEvaluation;
    }

    private static DifferentialStatus64 Evaluate(
        Func<double,double,double> function,double x,double y,out double result)
    {
        result=double.NaN;
        try { result=function(x,y); }
        catch(OperationCanceledException) { return DifferentialStatus64.Cancelled; }
        catch(Exception) { return DifferentialStatus64.EvaluationFailed; }
        return double.IsFinite(result)?DifferentialStatus64.Success:
            DifferentialStatus64.NonFiniteEvaluation;
    }

    private static DerivativeResult64 DerivativeRefuse(DifferentialStatus64 status) =>
        new(status,null,null,null,0,false,RuleId);

    private static OdeResult64 OdeRefuse(DifferentialStatus64 status) =>
        new(status,null,null,0,0,false,RuleId);
}
