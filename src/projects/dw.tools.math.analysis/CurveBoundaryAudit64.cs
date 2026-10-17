namespace Dw.Tools.Math.Analysis;

/// <summary>Explicit policy. No extrapolation or inferred precision is the default.</summary>
public enum CurveExtrapolation64 { Refuse, LinearSegment }

/// <summary>Caller-declared axis symbols, never a physical unit conversion.</summary>
public readonly record struct CurveAxisUnits64(string XUnit,string YUnit);

/// <summary>A relative abscissa-span threshold is a refusal policy, not an error bar.</summary>
public sealed record CurveBoundaryPolicy64(
    double MinimumRelativeXSpan=1e-12,
    CurveExtrapolation64 Extrapolation=CurveExtrapolation64.Refuse);

public enum CurveBoundaryStatus64
{
    Success, InvalidInput, BudgetExceeded, DuplicateAbscissa,
    OutsideDomain, InvalidUnit, IncompatibleUnit, InvalidPolicy,
    NearRankDeficiency, NonFiniteIntermediate
}

/// <summary>Bounded line evaluation, with observed-unit tags and explicit extrapolation.</summary>
public sealed record CurveBoundaryValue64(
    CurveBoundaryStatus64 Status,double? Value,double? LocalSlope,
    string? XUnit,string? YUnit,string? SlopeUnit,
    double? RelativeXSpan,bool Extrapolated,bool UnitsConverted,
    bool PredictiveGuarantee,string RuleId)
{
    public bool Admitted =>
        Status==CurveBoundaryStatus64.Success &&
        Value is { } value && double.IsFinite(value) &&
        LocalSlope is { } slope && double.IsFinite(slope) &&
        RelativeXSpan is { } span && double.IsFinite(span) && span>0 &&
        XUnit is not null && YUnit is not null && SlopeUnit is not null &&
        !UnitsConverted && !PredictiveGuarantee;
}

/// <summary>Descriptive OLS only; null numerical fields on every refusal.</summary>
public sealed record CurveBoundaryFit64(
    CurveBoundaryStatus64 Status,double? Slope,double? Intercept,
    double? SumSquaredResiduals,string? XUnit,string? YUnit,string? SlopeUnit,
    double? RelativeXSpan,bool UnitsConverted,bool CausalGuarantee,string RuleId)
{
    public bool Admitted =>
        Status==CurveBoundaryStatus64.Success &&
        Slope is { } slope && double.IsFinite(slope) &&
        Intercept is { } intercept && double.IsFinite(intercept) &&
        SumSquaredResiduals is { } sse && double.IsFinite(sse) && sse>=0 &&
        RelativeXSpan is { } span && double.IsFinite(span) && span>0 &&
        XUnit is not null && YUnit is not null && SlopeUnit is not null &&
        !UnitsConverted && !CausalGuarantee;
}

/// <summary>
/// Reuses the qualified T1 algorithms behind explicit caller unit labels,
/// relative-conditioning refusal and optional piecewise LINEAR extrapolation.
/// Never silently converts coordinates, extrapolates splines or asserts a
/// forecast/causal model. Limit policies and samples before arithmetic.
/// </summary>
public static class CurveBoundaryAudit64
{
    public const string RuleId="dw-math-curve-boundaries-binary64-1";
    public const int MaximumUnitSymbolCharacters=24;
    private static readonly CurveBoundaryPolicy64 DefaultPolicy=new();

    public static CurveBoundaryValue64 EvaluateLinear(
        IReadOnlyList<CurvePoint64>? samples,double query,
        CurveAxisUnits64 units,string? queryXUnit,
        CurveBoundaryPolicy64? policy=null)
    {
        if(!ValidUnits(units))return ValueRefuse(CurveBoundaryStatus64.InvalidUnit);
        if(!string.Equals(units.XUnit,queryXUnit,StringComparison.Ordinal))
            return ValueRefuse(CurveBoundaryStatus64.IncompatibleUnit);
        var selected=policy??DefaultPolicy;
        if(!ValidPolicy(selected))
            return ValueRefuse(CurveBoundaryStatus64.InvalidPolicy);
        var status=ValidateSamples(samples,selected,
            out var points,out var relative);
        if(status!=CurveBoundaryStatus64.Success)return ValueRefuse(status);
        if(!double.IsFinite(query))return ValueRefuse(CurveBoundaryStatus64.InvalidInput);
        var outside=query<points[0].X||query>points[^1].X;
        if(outside && selected.Extrapolation==CurveExtrapolation64.Refuse)
            return ValueRefuse(CurveBoundaryStatus64.OutsideDomain);
        var segment=points.Length-2;
        for(var i=0;i<points.Length-1;i++)
        {
            if(query<=points[i+1].X) {segment=i;break;}
        }
        var left=points[segment];
        var right=points[segment+1];
        var dx=right.X-left.X;
        var dy=right.Y-left.Y;
        var slope=dy/dx;
        if(!(dx>0d)||!double.IsFinite(dx)||
            !double.IsFinite(dy)||!double.IsFinite(slope))
            return ValueRefuse(CurveBoundaryStatus64.NonFiniteIntermediate);
        double value;
        if(outside)
        {
            var fraction=(query-left.X)/dx;
            value=left.Y+fraction*dy;
            if(!double.IsFinite(fraction))
                return ValueRefuse(CurveBoundaryStatus64.NonFiniteIntermediate);
        }
        else
        {
            var admitted=BoundedCurveAnalysis64.InterpolateLinear(points,query);
            if(!admitted.Succeeded || admitted.Value is null)
                return ValueRefuse(Map(admitted.Status));
            value=admitted.Value.Value;
        }
        if(!double.IsFinite(value))
            return ValueRefuse(CurveBoundaryStatus64.NonFiniteIntermediate);
        return new(CurveBoundaryStatus64.Success,value,slope,
            units.XUnit,units.YUnit,units.YUnit+"/"+units.XUnit,
            relative,outside,false,false,RuleId);
    }

    public static CurveBoundaryFit64 FitDescriptive(
        IReadOnlyList<CurvePoint64>? samples,CurveAxisUnits64 units,
        CurveBoundaryPolicy64? policy=null)
    {
        if(!ValidUnits(units))return FitRefuse(CurveBoundaryStatus64.InvalidUnit);
        var selected=policy??DefaultPolicy;
        if(!ValidPolicy(selected))return FitRefuse(CurveBoundaryStatus64.InvalidPolicy);
        var status=ValidateSamples(samples,selected,
            out var points,out var relative);
        if(status!=CurveBoundaryStatus64.Success)return FitRefuse(status);
        var fit=BoundedCurveAnalysis64.FitLeastSquares(points);
        if(!fit.Succeeded || fit.Slope is null ||
           fit.Intercept is null || fit.SumSquaredResiduals is null)
            return FitRefuse(Map(fit.Status));
        return new(CurveBoundaryStatus64.Success,fit.Slope,fit.Intercept,
            fit.SumSquaredResiduals,units.XUnit,units.YUnit,
            units.YUnit+"/"+units.XUnit,relative,false,false,RuleId);
    }

    private static CurveBoundaryStatus64 ValidateSamples(
        IReadOnlyList<CurvePoint64>? input,CurveBoundaryPolicy64 policy,
        out CurvePoint64[] points,out double relative)
    {
        points=[];
        relative=0d;
        if(input is null||input.Count<2)return CurveBoundaryStatus64.InvalidInput;
        if(input.Count>BoundedCurveAnalysis64.MaximumSamples)
            return CurveBoundaryStatus64.BudgetExceeded;
        var copy=new CurvePoint64[input.Count];
        for(var i=0;i<copy.Length;i++)
        {
            var current=input[i];
            if(!double.IsFinite(current.X)||!double.IsFinite(current.Y))
                return CurveBoundaryStatus64.InvalidInput;
            if(i>0)
            {
                if(current.X==copy[i-1].X)
                    return CurveBoundaryStatus64.DuplicateAbscissa;
                if(current.X<copy[i-1].X)
                    return CurveBoundaryStatus64.InvalidInput;
            }
            copy[i]=current;
        }
        var span=copy[^1].X-copy[0].X;
        var scale=global::System.Math.Max(1d,
            global::System.Math.Max(
                global::System.Math.Abs(copy[0].X),
                global::System.Math.Abs(copy[^1].X)));
        if(!double.IsFinite(span)||!double.IsFinite(scale))
            return CurveBoundaryStatus64.NonFiniteIntermediate;
        relative=span/scale;
        if(!(relative>0d)||!double.IsFinite(relative))
            return CurveBoundaryStatus64.NearRankDeficiency;
        if(relative<policy.MinimumRelativeXSpan)
            return CurveBoundaryStatus64.NearRankDeficiency;
        points=copy;
        return CurveBoundaryStatus64.Success;
    }

    private static bool ValidPolicy(CurveBoundaryPolicy64 policy)=>
        double.IsFinite(policy.MinimumRelativeXSpan) &&
        policy.MinimumRelativeXSpan>0d && policy.MinimumRelativeXSpan<=1e-2d &&
        policy.Extrapolation is
            CurveExtrapolation64.Refuse or CurveExtrapolation64.LinearSegment;

    private static bool ValidUnits(CurveAxisUnits64 units)=>
        ValidSymbol(units.XUnit)&&ValidSymbol(units.YUnit);

    private static bool ValidSymbol(string? value)
    {
        if(string.IsNullOrWhiteSpace(value) ||
           value.Length>MaximumUnitSymbolCharacters ||
           !AsciiLetter(value[0]))return false;
        foreach(var ch in value)
            if(!AsciiLetter(ch)&&!AsciiDigit(ch)&&ch!='_')
                return false;
        return true;
    }
    private static bool AsciiLetter(char ch)=>
        ch is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
    private static bool AsciiDigit(char ch)=>ch is >= '0' and <= '9';

    private static CurveBoundaryStatus64 Map(CurveAnalysisStatus64 status)=>status switch
    {
        CurveAnalysisStatus64.Success => CurveBoundaryStatus64.NonFiniteIntermediate,
        CurveAnalysisStatus64.BudgetExceeded => CurveBoundaryStatus64.BudgetExceeded,
        CurveAnalysisStatus64.DuplicateAbscissa =>
            CurveBoundaryStatus64.DuplicateAbscissa,
        CurveAnalysisStatus64.OutsideDomain => CurveBoundaryStatus64.OutsideDomain,
        CurveAnalysisStatus64.RankDeficient =>
            CurveBoundaryStatus64.NearRankDeficiency,
        CurveAnalysisStatus64.NonFiniteIntermediate =>
            CurveBoundaryStatus64.NonFiniteIntermediate,
        _ => CurveBoundaryStatus64.InvalidInput
    };

    private static CurveBoundaryValue64 ValueRefuse(CurveBoundaryStatus64 status)=>
        new(status,null,null,null,null,null,null,false,false,false,RuleId);

    private static CurveBoundaryFit64 FitRefuse(CurveBoundaryStatus64 status)=>
        new(status,null,null,null,null,null,null,null,false,false,RuleId);
}
