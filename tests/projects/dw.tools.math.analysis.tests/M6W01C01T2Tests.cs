using Dw.Tools.Math.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Analysis.Tests;

[TestClass]
public sealed class M6W01C01T2Tests
{
    private static readonly CurveAxisUnits64 Axes=new("s","m");
    private static CurvePoint64[] Straight()=>[new(0,1),new(1,3),new(2,5)];
    private static CurvePoint64[] Points(params (double X,double Y)[] coordinates)=>
        coordinates.Select(p=>new CurvePoint64(p.X,p.Y)).ToArray();
    private static CurveBoundaryPolicy64 Extrapolate()=>
        new(1e-12,CurveExtrapolation64.LinearSegment);
    private static void Near(double? actual,double expected,double tolerance=1e-10)
    {
        Assert.IsNotNull(actual,"Admitted numerical evidence must be present.");
        Assert.IsTrue(double.IsFinite(actual.Value),"The actual number must be finite.");
        Assert.IsTrue(global::System.Math.Abs(actual.Value-expected)<=tolerance,
            $"Expected {expected:R}, got {actual.Value:R}");
    }
    private static void ValueNear(CurveBoundaryValue64 result,double expected,bool extrapolated)
    {
        Assert.AreEqual(CurveBoundaryStatus64.Success,result.Status);
        Assert.IsTrue(result.Admitted);
        Near(result.Value,expected);
        Assert.AreEqual(extrapolated,result.Extrapolated);
        Assert.IsFalse(result.UnitsConverted);
        Assert.IsFalse(result.PredictiveGuarantee);
    }
    private static void FitNear(CurveBoundaryFit64 actual,double slope,double intercept,double residual)
    {
        Assert.AreEqual(CurveBoundaryStatus64.Success,actual.Status);
        Assert.IsTrue(actual.Admitted);
        Near(actual.Slope,slope);
        Near(actual.Intercept,intercept);
        Near(actual.SumSquaredResiduals,residual);
        Assert.IsFalse(actual.UnitsConverted);
        Assert.IsFalse(actual.CausalGuarantee);
    }
    private static void Refuse(CurveBoundaryValue64 value,CurveBoundaryStatus64 expected)
    {
        Assert.AreEqual(expected,value.Status);
        Assert.IsFalse(value.Admitted);
        Assert.IsNull(value.Value);
        Assert.IsNull(value.LocalSlope);
        Assert.IsNull(value.RelativeXSpan);
        Assert.IsNull(value.SlopeUnit);
    }
    private static void FitRefuse(CurveBoundaryFit64 fit,CurveBoundaryStatus64 expected)
    {
        Assert.AreEqual(expected,fit.Status);
        Assert.IsFalse(fit.Admitted);
        Assert.IsNull(fit.Slope);
        Assert.IsNull(fit.Intercept);
        Assert.IsNull(fit.SumSquaredResiduals);
        Assert.IsNull(fit.RelativeXSpan);
        Assert.IsNull(fit.SlopeUnit);
    }
    [TestMethod]
    public void DefaultLinearRefusesOutsideLeft()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),-1,Axes,"s"),CurveBoundaryStatus64.OutsideDomain);
    }

    [TestMethod]
    public void DefaultLinearRefusesOutsideRight()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),3,Axes,"s"),CurveBoundaryStatus64.OutsideDomain);
    }

    [TestMethod]
    public void ExplicitLeftLinearExtrapolationIsAdmitted()
    {
        ValueNear(CurveBoundaryAudit64.EvaluateLinear(Straight(),-1,Axes,"s",Extrapolate()),-1,true);
    }

    [TestMethod]
    public void ExplicitRightLinearExtrapolationIsAdmitted()
    {
        ValueNear(CurveBoundaryAudit64.EvaluateLinear(Straight(),3,Axes,"s",Extrapolate()),7,true);
    }

    [TestMethod]
    public void LeftExtrapolationSlopeAndAxisUnitsRetained()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),-1,Axes,"s",Extrapolate());ValueNear(x,-1,true);Near(x.LocalSlope,2);Assert.AreEqual("m/s",x.SlopeUnit);
    }

    [TestMethod]
    public void RightExtrapolationRetainsNoPredictionGuarantee()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),3,Axes,"s",Extrapolate());ValueNear(x,7,true);Assert.IsFalse(x.PredictiveGuarantee);
    }

    [TestMethod]
    public void InsideDomainNeverClaimedExtrapolatedEvenWithOptIn()
    {
        ValueNear(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s",Extrapolate()),2,false);
    }

    [TestMethod]
    public void OldT1ContractStillRefusesExtrapolation()
    {
        var old=BoundedCurveAnalysis64.InterpolateLinear(Straight(),3);Assert.AreEqual(CurveAnalysisStatus64.OutsideDomain,old.Status);Assert.IsNull(old.Value);
    }

    [TestMethod]
    public void InsideDomainMatchesOriginalLinearSemantics()
    {
        ValueNear(CurveBoundaryAudit64.EvaluateLinear(Straight(),1.5,Axes,"s"),4,false);
    }

    [TestMethod]
    public void ExactKnotPreservesOriginalValue()
    {
        ValueNear(CurveBoundaryAudit64.EvaluateLinear(Straight(),1,Axes,"s"),3,false);
    }

    [TestMethod]
    public void QueryUnitMismatchNeverConverts()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"min"),CurveBoundaryStatus64.IncompatibleUnit);
    }

    [TestMethod]
    public void QueryUnitsAreCaseSensitive()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"S"),CurveBoundaryStatus64.IncompatibleUnit);
    }

    [TestMethod]
    public void NullQueryUnitIsIncompatible()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,null),CurveBoundaryStatus64.IncompatibleUnit);
    }

    [TestMethod]
    public void InvalidBlankAbscissaUnitRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,new("","m"),"s"),CurveBoundaryStatus64.InvalidUnit);
    }

    [TestMethod]
    public void InvalidBlankYUnitRefused()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),new("s"," ")),CurveBoundaryStatus64.InvalidUnit);
    }

    [TestMethod]
    public void CompositeUnitSyntaxIsNotImplicitlyParsed()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),new("s","m/s")),CurveBoundaryStatus64.InvalidUnit);
    }

    [TestMethod]
    public void LongAxisSymbolIsRefusedByBudget()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,new(new string('x',25),"m"),new string('x',25)),CurveBoundaryStatus64.InvalidUnit);
    }

    [TestMethod]
    public void NonAsciiSymbolsDoNotImplyConversion()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),new("°C","m")),CurveBoundaryStatus64.InvalidUnit);
    }

    [TestMethod]
    public void ValidUnderbarAxisSymbolRetained()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,new("time_s","length_m"),"time_s");ValueNear(x,2,false);Assert.AreEqual("length_m/time_s",x.SlopeUnit);
    }

    [TestMethod]
    public void ValidDigitSuffixUnitRetained()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,new("s2","m2"),"s2");ValueNear(x,2,false);Assert.AreEqual("m2",x.YUnit);
    }

    [TestMethod]
    public void InterpolationProvidesDeclaredUnitsWithoutConversion()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s");ValueNear(x,2,false);Assert.AreEqual("s",x.XUnit);Assert.AreEqual("m",x.YUnit);Assert.IsFalse(x.UnitsConverted);
    }

    [TestMethod]
    public void FitExposesPhysicalSlopeTagFromCallerMetadata()
    {
        var f=CurveBoundaryAudit64.FitDescriptive(Straight(),Axes);FitNear(f,2,1,0);Assert.AreEqual("m/s",f.SlopeUnit);
    }

    [TestMethod]
    public void FitRetainsInputAxisUnitLabels()
    {
        var f=CurveBoundaryAudit64.FitDescriptive(Straight(),Axes);FitNear(f,2,1,0);Assert.AreEqual("s",f.XUnit);Assert.AreEqual("m",f.YUnit);
    }

    [TestMethod]
    public void FitDoesNotConvertUnits()
    {
        var f=CurveBoundaryAudit64.FitDescriptive(Straight(),Axes);FitNear(f,2,1,0);Assert.IsFalse(f.UnitsConverted);
    }

    [TestMethod]
    public void FitDoesNotClaimCausalInference()
    {
        var f=CurveBoundaryAudit64.FitDescriptive(Straight(),Axes);FitNear(f,2,1,0);Assert.IsFalse(f.CausalGuarantee);
    }

    [TestMethod]
    public void FitZeroResidualStraightThreePointOracle()
    {
        FitNear(CurveBoundaryAudit64.FitDescriptive(Straight(),Axes),2,1,0);
    }

    [TestMethod]
    public void FitSymmetricNoisySseIsTwoThirds()
    {
        var p=Points((0,0),(1,1),(2,0));FitNear(CurveBoundaryAudit64.FitDescriptive(p,Axes),0,1d/3d,2d/3d);
    }

    [TestMethod]
    public void FitNegativeSlopeAndNonzeroIntercept()
    {
        FitNear(CurveBoundaryAudit64.FitDescriptive(Points((0,4),(1,2),(2,0)),Axes),-2,4,0);
    }

    [TestMethod]
    public void FitConstantOrdinateIsAdmitted()
    {
        FitNear(CurveBoundaryAudit64.FitDescriptive(Points((0,7),(1,7),(2,7)),Axes),0,7,0);
    }

    [TestMethod]
    public void SuccessCarriesActualRelativeSpan()
    {
        var f=CurveBoundaryAudit64.FitDescriptive(Straight(),Axes);FitNear(f,2,1,0);Near(f.RelativeXSpan,1);
    }

    [TestMethod]
    public void NearRankDeficiencyIsExplicitFitRefusal()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Points((1e12,0),(1e12+0.01,1)),Axes),CurveBoundaryStatus64.NearRankDeficiency);
    }

    [TestMethod]
    public void NearRankDeficiencyIsExplicitInterpolationRefusal()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((1e12,0),(1e12+0.01,1)),1e12,Axes,"s"),CurveBoundaryStatus64.NearRankDeficiency);
    }

    [TestMethod]
    public void SubnormalAbscissaSpanRejectedAsNearRank()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Points((0,0),(double.Epsilon,1)),Axes),CurveBoundaryStatus64.NearRankDeficiency);
    }

    [TestMethod]
    public void ExplicitTighterRankPolicyCanAdmitSmallSpan()
    {
        var p=Points((1e12,0),(1e12+0.01,1));var x=CurveBoundaryAudit64.EvaluateLinear(p,1e12,Axes,"s",new(1e-15));ValueNear(x,0,false);
    }

    [TestMethod]
    public void PolicyZeroThresholdIsInvalid()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s",new(0)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void PolicyNegativeThresholdIsInvalid()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),Axes,new(-1)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void PolicyNaNThresholdIsInvalid()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),Axes,new(double.NaN)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void PolicyInfiniteThresholdIsInvalid()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s",new(double.PositiveInfinity)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void PolicyOverlargeRelativeThresholdIsInvalid()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Straight(),Axes,new(0.1)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void UnknownExtrapolationEnumIsTypedInvalidPolicy()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s",new(1e-12,(CurveExtrapolation64)99)),CurveBoundaryStatus64.InvalidPolicy);
    }

    [TestMethod]
    public void NullSamplesAreInvalid()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(null,0.5,Axes,"s"),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void EmptySamplesAreInvalid()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive([],Axes),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void OneSampleCannotProveUniqueLine()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((0,1)),0,Axes,"s"),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void OversizedSamplesCannotExceedBudget()
    {
        var p=Enumerable.Range(0,33).Select(i=>new CurvePoint64(i,i)).ToArray();FitRefuse(CurveBoundaryAudit64.FitDescriptive(p,Axes),CurveBoundaryStatus64.BudgetExceeded);
    }

    [TestMethod]
    public void DuplicateXIsTypedInvalidCoordinateSample()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Points((0,1),(0,2)),Axes),CurveBoundaryStatus64.DuplicateAbscissa);
    }

    [TestMethod]
    public void UnsortedXIsTypedInvalid()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((1,1),(0,0)),0.5,Axes,"s"),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void NaNAbscissaRefused()
    {
        FitRefuse(CurveBoundaryAudit64.FitDescriptive(Points((0,1),(double.NaN,2)),Axes),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void InfiniteOrdinateRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((0,1),(1,double.PositiveInfinity)),0.5,Axes,"s"),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void NaNQueryRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),double.NaN,Axes,"s"),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void InfiniteQueryRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Straight(),double.PositiveInfinity,Axes,"s",Extrapolate()),CurveBoundaryStatus64.InvalidInput);
    }

    [TestMethod]
    public void AbscissaDifferenceOverflowRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((-double.MaxValue,0),(double.MaxValue,1)),0,Axes,"s"),CurveBoundaryStatus64.NonFiniteIntermediate);
    }

    [TestMethod]
    public void OrdinateDifferenceOverflowRefused()
    {
        Refuse(CurveBoundaryAudit64.EvaluateLinear(Points((0,double.MaxValue),(1,-double.MaxValue)),0.5,Axes,"s"),CurveBoundaryStatus64.NonFiniteIntermediate);
    }

    [TestMethod]
    public void RefusedResultCannotContainPartialUnitClaim()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"other");Refuse(x,CurveBoundaryStatus64.IncompatibleUnit);Assert.IsNull(x.SlopeUnit);Assert.IsFalse(x.UnitsConverted);
    }

    [TestMethod]
    public void RefusedFitDoesNotExposePartialRankSpan()
    {
        var x=CurveBoundaryAudit64.FitDescriptive(Points((1e12,0),(1e12+0.01,1)),Axes);FitRefuse(x,CurveBoundaryStatus64.NearRankDeficiency);Assert.IsNull(x.RelativeXSpan);
    }

    [TestMethod]
    public void LegacySplineCurvatureStillAdmitted()
    {
        var x=BoundedCurveAnalysis64.InterpolateNaturalSpline(Points((0,0),(1,1),(2,0)),0.5);Assert.AreEqual(CurveAnalysisStatus64.Success,x.Status);Near(x.Value,0.6875);
    }

    [TestMethod]
    public void AcceptedBoundariesAreDeterministic()
    {
        var first=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s");var second=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s");Assert.AreEqual(first,second);
    }

    [TestMethod]
    public void PolicyIsReflectedByRuntimeRuleNotConstantTautology()
    {
        var x=CurveBoundaryAudit64.EvaluateLinear(Straight(),0.5,Axes,"s");ValueNear(x,2,false);Assert.AreEqual(CurveBoundaryAudit64.RuleId,x.RuleId);
    }

    [TestMethod]
    public void FitDoesNotModifyCallerSuppliedSamples()
    {
        var a=Straight();var x=CurveBoundaryAudit64.FitDescriptive(a,Axes);a[0]=new(0,100);FitNear(x,2,1,0);
    }

    [TestMethod]
    public void NoBrokerOrForeignCASDependencyIntroduced()
    {
        var deps=typeof(CurveBoundaryAudit64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(deps.Any(name=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}.Any(q=>name.Contains(q,StringComparison.OrdinalIgnoreCase))));
    }
}
