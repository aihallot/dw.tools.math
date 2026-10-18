using System;
using System.Numerics;

namespace Dw.Tools.Math.Scientific;

/// <summary>
/// A managed, provider-neutral BCL backend. The positive-real Gamma subset
/// uses a disclosed finite Lanczos approximation, not a BCL Gamma API.
/// No third-party provider, automatic fallback, precision proof or CAS.
/// </summary>
public sealed class ManagedBclSpecialFunctionsProvider64 : ISpecialFunctionsProvider64
{
    public const string ProviderIdentity="dw-math-managed-bcl-special-1";
    private const double MaximumReal=20d;
    private const double MaximumComplexComponent=12d;
    private static readonly SpecialFunctionPolicy64 DefaultPolicy=new();
    private static readonly double[] GammaCoefficients=
    [
        0.99999999999980993d,
        676.5203681218851d,
        -1259.1392167224028d,
        771.32342877765313d,
        -176.6150291621406d,
        12.507343278686905d,
        -0.13857109526572012d,
        9.9843695780195716e-6d,
        1.5056327351493116e-7d
    ];

    public string ProviderId => ProviderIdentity;

    public SpecialRealResult64 EvaluateReal(
        SpecialFunctionKind64 kind, double argument,
        SpecialFunctionPolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RefuseReal(SpecialFunctionStatus64.Cancelled);
        var selected=policy??DefaultPolicy;
        if(!ValidPolicy(selected)||!ValidKind(kind))
            return RefuseReal(SpecialFunctionStatus64.InvalidPolicy);
        if(!double.IsFinite(argument))
            return RefuseReal(SpecialFunctionStatus64.InvalidInput);
        if(global::System.Math.Abs(argument)>selected.MaximumRealMagnitude)
            return RefuseReal(SpecialFunctionStatus64.OutsideQualifiedRange);
        if(kind==SpecialFunctionKind64.NaturalLogarithm&&argument<=0d)
            return RefuseReal(SpecialFunctionStatus64.UnsupportedDomain);
        if(kind==SpecialFunctionKind64.Gamma&&argument<0.5d)
            return RefuseReal(SpecialFunctionStatus64.UnsupportedDomain);
        double value=kind switch
        {
            SpecialFunctionKind64.Sine=>global::System.Math.Sin(argument),
            SpecialFunctionKind64.Cosine=>global::System.Math.Cos(argument),
            SpecialFunctionKind64.Exponential=>global::System.Math.Exp(argument),
            SpecialFunctionKind64.NaturalLogarithm=>global::System.Math.Log(argument),
            SpecialFunctionKind64.Gamma=>GammaPositiveReal(argument),
            _=>double.NaN
        };
        if(!double.IsFinite(value))
            return RefuseReal(SpecialFunctionStatus64.NonFiniteResult);
        return new(SpecialFunctionStatus64.Success,value,ProviderId,
            BoundedSpecialFunctions64.RuleId,SpecialAngleConvention64.Radians,false);
    }

    public SpecialComplexResult64 EvaluateComplex(
        SpecialFunctionKind64 kind, Complex argument,
        SpecialFunctionPolicy64? policy = null,
        CancellationToken cancellationToken = default)
    {
        if(cancellationToken.IsCancellationRequested)
            return RefuseComplex(SpecialFunctionStatus64.Cancelled);
        var selected=policy??DefaultPolicy;
        if(!ValidPolicy(selected)||!ValidKind(kind))
            return RefuseComplex(SpecialFunctionStatus64.InvalidPolicy);
        if(!double.IsFinite(argument.Real)||!double.IsFinite(argument.Imaginary))
            return RefuseComplex(SpecialFunctionStatus64.InvalidInput);
        if(global::System.Math.Abs(argument.Real)>selected.MaximumComplexComponentMagnitude||
           global::System.Math.Abs(argument.Imaginary)>selected.MaximumComplexComponentMagnitude)
            return RefuseComplex(SpecialFunctionStatus64.OutsideQualifiedRange);
        // Complex Gamma and its poles/branches are explicitly outside T1.
        if(kind==SpecialFunctionKind64.Gamma)
            return RefuseComplex(SpecialFunctionStatus64.UnsupportedDomain);
        if(kind==SpecialFunctionKind64.NaturalLogarithm&&argument==Complex.Zero)
            return RefuseComplex(SpecialFunctionStatus64.UnsupportedDomain);
        var value=kind switch
        {
            SpecialFunctionKind64.Sine=>Complex.Sin(argument),
            SpecialFunctionKind64.Cosine=>Complex.Cos(argument),
            SpecialFunctionKind64.Exponential=>Complex.Exp(argument),
            SpecialFunctionKind64.NaturalLogarithm=>Complex.Log(argument),
            _=>new Complex(double.NaN,double.NaN)
        };
        if(!double.IsFinite(value.Real)||!double.IsFinite(value.Imaginary))
            return RefuseComplex(SpecialFunctionStatus64.NonFiniteResult);
        return new(SpecialFunctionStatus64.Success,value,ProviderId,
            BoundedSpecialFunctions64.RuleId,SpecialAngleConvention64.Radians,false);
    }

    private static bool ValidPolicy(SpecialFunctionPolicy64 p) =>
        double.IsFinite(p.MaximumRealMagnitude) &&
        p.MaximumRealMagnitude>0d && p.MaximumRealMagnitude<=MaximumReal &&
        double.IsFinite(p.MaximumComplexComponentMagnitude) &&
        p.MaximumComplexComponentMagnitude>0d &&
        p.MaximumComplexComponentMagnitude<=MaximumComplexComponent;

    private static bool ValidKind(SpecialFunctionKind64 kind) =>
        kind is SpecialFunctionKind64.Sine or SpecialFunctionKind64.Cosine or
            SpecialFunctionKind64.Exponential or
            SpecialFunctionKind64.NaturalLogarithm or SpecialFunctionKind64.Gamma;

    private static double GammaPositiveReal(double x)
    {
        // g=7 Lanczos approximation for 0.5<=x<=20. Not an exact
        // analytic continuation, error certificate, or general Gamma.
        var shifted=x-1d;
        var accumulated=GammaCoefficients[0];
        for(var i=1;i<GammaCoefficients.Length;i++)
            accumulated+=GammaCoefficients[i]/(shifted+i);
        var t=shifted+7.5d;
        return global::System.Math.Sqrt(2d*global::System.Math.PI)*
            global::System.Math.Pow(t,shifted+0.5d)*
            global::System.Math.Exp(-t)*accumulated;
    }

    private SpecialRealResult64 RefuseReal(SpecialFunctionStatus64 status) =>
        new(status,null,ProviderId,BoundedSpecialFunctions64.RuleId,
            SpecialAngleConvention64.Radians,false);

    private SpecialComplexResult64 RefuseComplex(SpecialFunctionStatus64 status) =>
        new(status,null,ProviderId,BoundedSpecialFunctions64.RuleId,
            SpecialAngleConvention64.Radians,false);
}

/// <summary>Bounded special-function facade with exactly one explicit managed backend.</summary>
public static class BoundedSpecialFunctions64
{
    public const string RuleId="dw-math-bounded-special-functions-real-complex-1";
    private static readonly ISpecialFunctionsProvider64 ManagedProvider=
        new ManagedBclSpecialFunctionsProvider64();

    public static SpecialRealResult64 EvaluateReal(
        SpecialFunctionKind64 kind,double argument,
        SpecialFunctionPolicy64? policy=null,CancellationToken cancellationToken=default) =>
        ManagedProvider.EvaluateReal(kind,argument,policy,cancellationToken);

    public static SpecialComplexResult64 EvaluateComplex(
        SpecialFunctionKind64 kind,Complex argument,
        SpecialFunctionPolicy64? policy=null,CancellationToken cancellationToken=default) =>
        ManagedProvider.EvaluateComplex(kind,argument,policy,cancellationToken);
}
