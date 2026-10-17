namespace Dw.Tools.Math.Analysis;

/// <summary>Typed admission status for bounded, finite binary64 curve analysis.</summary>
public enum CurveAnalysisStatus64
{
    Success,
    InvalidInput,
    BudgetExceeded,
    DuplicateAbscissa,
    OutsideDomain,
    RankDeficient,
    NonFiniteIntermediate
}

/// <summary>Only a successful finite, scoped interpolation has a value.</summary>
public sealed record CurveValueResult64(
    CurveAnalysisStatus64 Status,double? Value,string RuleId)
{
    public bool Succeeded => Status==CurveAnalysisStatus64.Success &&
        Value is { } result && double.IsFinite(result);
}

/// <summary>
/// An unweighted descriptive linear fit, never causal or predictive proof.
/// Null numbers mean refusal; finite SSE is recorded with the fit.
/// </summary>
public sealed record CurveFitResult64(
    CurveAnalysisStatus64 Status,double? Slope,double? Intercept,
    double? SumSquaredResiduals,string RuleId)
{
    public bool Succeeded =>
        Status==CurveAnalysisStatus64.Success &&
        Slope is { } slope && double.IsFinite(slope) &&
        Intercept is { } intercept && double.IsFinite(intercept) &&
        SumSquaredResiduals is { } residual &&
        double.IsFinite(residual) && residual>=0d;
}

/// <summary>
/// Provider-free, bounded deterministic interpolation and descriptive OLS.
/// Callers supply a strictly ascending list of unweighted finite points.
/// There is no default extrapolation, unit conversion, randomness, model
/// forecast, confidence interval, general regression or causal inference.
/// </summary>
public static class BoundedCurveAnalysis64
{
    public const string RuleId="dw-math-curve-binary64-1";
    public const int MaximumSamples=32;

    public static CurveValueResult64 InterpolateLinear(
        IReadOnlyList<CurvePoint64>? samples,double x)
    {
        var status=Validate(samples,out var points);
        if(status!=CurveAnalysisStatus64.Success)return ValueRefusal(status);
        return Linear(points,x);
    }

    public static CurveValueResult64 InterpolateNaturalSpline(
        IReadOnlyList<CurvePoint64>? samples,double x)
    {
        var status=Validate(samples,out var points);
        if(status!=CurveAnalysisStatus64.Success)return ValueRefusal(status);
        if(!double.IsFinite(x))return ValueRefusal(CurveAnalysisStatus64.InvalidInput);
        if(x<points[0].X || x>points[^1].X)
            return ValueRefusal(CurveAnalysisStatus64.OutsideDomain);
        if(points.Length==2)return Linear(points,x);
        for(var i=0;i<points.Length;i++)
            if(x==points[i].X)return new(CurveAnalysisStatus64.Success,points[i].Y,RuleId);

        var n=points.Length;
        var h=new double[n-1];
        var slopes=new double[n-1];
        for(var i=0;i<n-1;i++)
        {
            h[i]=points[i+1].X-points[i].X;
            var dy=points[i+1].Y-points[i].Y;
            slopes[i]=dy/h[i];
            if(!(h[i]>0d)||!double.IsFinite(h[i])||
               !double.IsFinite(dy)||!double.IsFinite(slopes[i]))
                return ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        var cPrime=new double[n];
        var dPrime=new double[n];
        var second=new double[n];
        for(var i=1;i<n-1;i++)
        {
            var pivot=2d*(h[i-1]+h[i])-h[i-1]*cPrime[i-1];
            var rhs=6d*(slopes[i]-slopes[i-1])-h[i-1]*dPrime[i-1];
            if(!(pivot>0d)||!double.IsFinite(pivot)||!double.IsFinite(rhs))
                return ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
            cPrime[i]=h[i]/pivot;
            dPrime[i]=rhs/pivot;
            if(!double.IsFinite(cPrime[i])||!double.IsFinite(dPrime[i]))
                return ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        for(var i=n-2;i>=1;i--)
        {
            second[i]=dPrime[i]-cPrime[i]*second[i+1];
            if(!double.IsFinite(second[i]))
                return ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        var segment=FindSegment(points,x);
        var width=h[segment];
        var a=(points[segment+1].X-x)/width;
        var b=(x-points[segment].X)/width;
        var correction=((a*a*a-a)*second[segment]+
            (b*b*b-b)*second[segment+1])*(width*width)/6d;
        var answer=a*points[segment].Y+b*points[segment+1].Y+correction;
        return double.IsFinite(answer)&&double.IsFinite(correction)?
            new(CurveAnalysisStatus64.Success,answer,RuleId):
            ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
    }

    public static CurveFitResult64 FitLeastSquares(
        IReadOnlyList<CurvePoint64>? samples)
    {
        var status=Validate(samples,out var points);
        if(status!=CurveAnalysisStatus64.Success)return FitRefusal(status);
        var n=points.Length;
        // Online means avoid artificially overflowing simple repeated inputs.
        var meanX=0d;
        var meanY=0d;
        for(var i=0;i<n;i++)
        {
            meanX+=(points[i].X-meanX)/(i+1);
            meanY+=(points[i].Y-meanY)/(i+1);
            if(!double.IsFinite(meanX)||!double.IsFinite(meanY))
                return FitRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        var xx=0d;
        var xy=0d;
        for(var i=0;i<n;i++)
        {
            var dx=points[i].X-meanX;
            var dy=points[i].Y-meanY;
            xx+=dx*dx;
            xy+=dx*dy;
            if(!double.IsFinite(xx)||!double.IsFinite(xy))
                return FitRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        if(xx<=0d)return FitRefusal(CurveAnalysisStatus64.RankDeficient);
        var slope=xy/xx;
        var intercept=meanY-slope*meanX;
        if(!double.IsFinite(slope)||!double.IsFinite(intercept))
            return FitRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        var residual=0d;
        for(var i=0;i<n;i++)
        {
            var error=slope*points[i].X+intercept-points[i].Y;
            residual+=error*error;
            if(!double.IsFinite(residual))
                return FitRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        }
        return new(CurveAnalysisStatus64.Success,slope,intercept,residual,RuleId);
    }

    private static CurveValueResult64 Linear(CurvePoint64[] points,double x)
    {
        if(!double.IsFinite(x))return ValueRefusal(CurveAnalysisStatus64.InvalidInput);
        if(x<points[0].X || x>points[^1].X)
            return ValueRefusal(CurveAnalysisStatus64.OutsideDomain);
        for(var i=0;i<points.Length;i++)
            if(x==points[i].X)
                return new(CurveAnalysisStatus64.Success,points[i].Y,RuleId);
        var iSegment=FindSegment(points,x);
        var left=points[iSegment];
        var right=points[iSegment+1];
        var dx=right.X-left.X;
        var dy=right.Y-left.Y;
        if(!(dx>0d)||!double.IsFinite(dx)||!double.IsFinite(dy))
            return ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
        var fraction=(x-left.X)/dx;
        var value=left.Y+fraction*dy;
        return double.IsFinite(fraction)&&double.IsFinite(value)?
            new(CurveAnalysisStatus64.Success,value,RuleId):
            ValueRefusal(CurveAnalysisStatus64.NonFiniteIntermediate);
    }

    private static int FindSegment(CurvePoint64[] points,double x)
    {
        for(var i=0;i<points.Length-1;i++)
            if(x>=points[i].X && x<=points[i+1].X)return i;
        throw new InvalidOperationException(
            "Caller must validate the sorted interpolated domain.");
    }

    private static CurveAnalysisStatus64 Validate(
        IReadOnlyList<CurvePoint64>? input,out CurvePoint64[] points)
    {
        points=[];
        if(input is null || input.Count<2)
            return CurveAnalysisStatus64.InvalidInput;
        if(input.Count>MaximumSamples)
            return CurveAnalysisStatus64.BudgetExceeded;
        var copy=new CurvePoint64[input.Count];
        for(var i=0;i<copy.Length;i++)
        {
            var point=input[i];
            if(!double.IsFinite(point.X)||!double.IsFinite(point.Y))
                return CurveAnalysisStatus64.InvalidInput;
            if(i>0)
            {
                if(point.X==copy[i-1].X)
                    return CurveAnalysisStatus64.DuplicateAbscissa;
                if(point.X<copy[i-1].X)
                    return CurveAnalysisStatus64.InvalidInput;
            }
            copy[i]=point;
        }
        points=copy;
        return CurveAnalysisStatus64.Success;
    }

    private static CurveValueResult64 ValueRefusal(CurveAnalysisStatus64 status)=>
        new(status,null,RuleId);

    private static CurveFitResult64 FitRefusal(CurveAnalysisStatus64 status)=>
        new(status,null,null,null,RuleId);
}
