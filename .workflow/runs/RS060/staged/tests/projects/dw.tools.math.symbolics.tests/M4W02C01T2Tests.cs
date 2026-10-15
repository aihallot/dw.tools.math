using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C01T2Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrSymbol Y()=>IrSymbol.Free("y","x");
    private static IrExactScalar Number(long n)=>new(new ExactRational(n,1));
    private static IrNode Cube(IrSymbol x)=>IrApply.Create(IrOperation.Power,[x,Number(3)]);
    private static IrNode Square(IrSymbol x)=>IrApply.Create(IrOperation.Square,[x]);
    private static IrNode Sqrt(IrNode value)=>IrApply.Create(IrOperation.Sqrt,[value]);
    private static IrNode Abs(IrSymbol x)=>IrApply.Create(IrOperation.Abs,[x]);
    private static IrNode Quotient(IrSymbol x)=>
        IrApply.Create(IrOperation.Divide,[
            IrApply.Create(IrOperation.Subtract,[Square(x),Number(1)]),
            IrApply.Create(IrOperation.Subtract,[x,Number(1)])]);
    private static SymbolicDerivativePointResult At(IrNode source,long x)=>
        SymbolicDerivativePointAnalysis.Analyze(source,X(),new ExactRational(x,1));
    private static SymbolicDerivativePointResult At(IrNode source,IrSymbol variable,
        ExactRational point)=>SymbolicDerivativePointAnalysis.Analyze(source,variable,point);
    private static ExactRational Value(SymbolicDerivativePointResult result)
    {
        Assert.IsTrue(result.HasExactValue);
        Assert.AreEqual(SymbolicDerivativePointStatus.ExactValue,result.Status);
        Assert.IsNotNull(result.RuleId);
        Assert.IsNotNull(result.SourceSemanticSha256);
        Assert.IsNotNull(result.VariableIdentity);
        if(result.Value is not ExactRational value)
            throw new AssertFailedException("A qualified exact derivative must carry a rational.");
        return value;
    }
    private static void Refused(SymbolicDerivativePointStatus status,
        SymbolicDerivativePointResult result)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.HasExactValue);
        Assert.IsFalse(result.Value.HasValue);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
    }

    [TestMethod]
    public void CubicDerivativeAtTwoIsTwelveNotAnApproximation()
    {
        Assert.AreEqual(new ExactRational(12,1),Value(At(Cube(X()),2)));
    }

    [TestMethod]
    public void CubicDerivativeAtNegativeTwoIsPositiveTwelve()
    {
        Assert.AreEqual(new ExactRational(12,1),Value(At(Cube(X()),-2)));
    }

    [TestMethod]
    public void CubicDerivativeAtZeroIsExactlyZero()
    {
        Assert.AreEqual(ExactRational.Zero,Value(At(Cube(X()),0)));
    }

    [TestMethod]
    public void SquareDerivativeAtThreeIsExactlySix()
    {
        Assert.AreEqual(new ExactRational(6,1),Value(At(Square(X()),3)));
    }

    [TestMethod]
    public void IndependentDerivativeTargetHasZeroPointwiseValue()
    {
        Assert.AreEqual(ExactRational.Zero,
            Value(At(Cube(X()),Y(),new ExactRational(2,1))));
    }

    [TestMethod]
    public void IndependentFreeSourceSymbolHasZeroPointwiseDerivative()
    {
        Assert.AreEqual(ExactRational.Zero,
            Value(At(Y(),X(),new ExactRational(2,1))));
    }

    [TestMethod]
    public void ConstantHasExactZeroDerivative()
    {
        Assert.AreEqual(ExactRational.Zero,Value(At(Number(11),17)));
    }

    [TestMethod]
    public void SumOfCubicAndXAtTwoDifferentiatesToThirteen()
    {
        var x=X();var sum=IrApply.Create(IrOperation.Add,[Cube(x),x]);
        Assert.AreEqual(new ExactRational(13,1),Value(At(sum,2)));
    }

    [TestMethod]
    public void SubtractionOfConstantCannotChangeCubicDerivative()
    {
        var source=IrApply.Create(IrOperation.Subtract,[Cube(X()),Number(4)]);
        Assert.AreEqual(new ExactRational(12,1),Value(At(source,2)));
    }

    [TestMethod]
    public void ExactConstantMultipliedCubicDerivativeIsTwentyFour()
    {
        var source=IrApply.Create(IrOperation.Multiply,[Number(2),Cube(X())]);
        Assert.AreEqual(new ExactRational(24,1),Value(At(source,2)));
    }

    [TestMethod]
    public void ExactFractionalConstantDoesNotWidenToBinary64()
    {
        var coefficient=new IrExactScalar(new ExactRational(1,2));
        var source=IrApply.Create(IrOperation.Multiply,[coefficient,Cube(X())]);
        Assert.AreEqual(new ExactRational(6,1),Value(At(source,2)));
    }

    [TestMethod]
    public void StructuralProvenanceIncludesOriginalExpressionAndTargetIdentity()
    {
        var x=X();var source=Cube(x);
        var result=At(source,2);
        Assert.AreEqual(SymbolicDerivativePointAnalysis.PolynomialRule,result.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(source),
            result.SourceSemanticSha256);
        Assert.AreEqual(x.Identity,result.VariableIdentity);
        Assert.AreEqual(64,result.SourceSemanticSha256?.Length);
    }

    [TestMethod]
    public void NamedFormalRealFunctionCanBeEvaluatedAtExactPoint()
    {
        var x=X();var body=Cube(x);
        var function=new SymbolicRealFunction(x,body);
        var result=SymbolicDerivativePointAnalysis.Analyze(function,new ExactRational(2,1));
        Assert.AreEqual(new ExactRational(12,1),Value(result));
    }

    [TestMethod]
    public void AbsoluteAwayFromZeroHasPositiveUnitSlope()
    {
        var r=At(Abs(X()),2);
        Assert.AreEqual(ExactRational.One,Value(r));
        Assert.AreEqual(SymbolicDerivativePointAnalysis.AbsoluteRule,r.RuleId);
    }

    [TestMethod]
    public void AbsoluteAwayFromZeroHasNegativeUnitSlope()
    {
        Assert.AreEqual(new ExactRational(-1,1),Value(At(Abs(X()),-3)));
    }

    [TestMethod]
    public void AbsoluteAtZeroIsNonDifferentiableRatherThanZero()
    {
        Refused(SymbolicDerivativePointStatus.NonDifferentiable,At(Abs(X()),0));
    }

    [TestMethod]
    public void RealSqrtOfSquareAboveZeroHasPositiveUnitSlope()
    {
        var r=At(Sqrt(Square(X())),2);
        Assert.AreEqual(ExactRational.One,Value(r));
        Assert.AreEqual(SymbolicDerivativePointAnalysis.AbsoluteSquareRootRule,r.RuleId);
    }

    [TestMethod]
    public void RealSqrtOfSquareBelowZeroHasNegativeUnitSlope()
    {
        Assert.AreEqual(new ExactRational(-1,1),
            Value(At(Sqrt(Square(X())),-2)));
    }

    [TestMethod]
    public void RealSqrtOfSquareAtZeroHasKinkNotAnExactDerivative()
    {
        Refused(SymbolicDerivativePointStatus.NonDifferentiable,
            At(Sqrt(Square(X())),0));
    }

    [TestMethod]
    public void SquareRootAtNegativeInputIsOutsideRealDomain()
    {
        Refused(SymbolicDerivativePointStatus.OutsideRealDomain,
            At(Sqrt(X()),-1));
    }

    [TestMethod]
    public void SquareRootAtZeroHasNoFiniteTwoSidedDerivative()
    {
        Refused(SymbolicDerivativePointStatus.NonDifferentiable,
            At(Sqrt(X()),0));
    }

    [TestMethod]
    public void SquareRootAtPositiveInputIsUnqualifiedNotNumericallyEstimated()
    {
        Refused(SymbolicDerivativePointStatus.Unsupported,
            At(Sqrt(X()),4));
    }

    [TestMethod]
    public void RemovableRationalPoleIsStillOutsideOriginalDomain()
    {
        Refused(SymbolicDerivativePointStatus.OutsideRealDomain,
            At(Quotient(X()),1));
    }

    [TestMethod]
    public void RationalAwayFromPoleDoesNotInventAnUnprovedQuotientDerivative()
    {
        Refused(SymbolicDerivativePointStatus.Unsupported,
            At(Quotient(X()),2));
    }

    [TestMethod]
    public void DirectQuantityRefusesToInventDerivedUnits()
    {
        var quantity=new IrQuantityLiteral(Number(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicDerivativePointStatus.UnsupportedUnits,At(quantity,2));
    }

    [TestMethod]
    public void QuantityInsideProductRemainsTypedUnsupportedUnits()
    {
        var quantity=new IrQuantityLiteral(Number(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        var source=IrApply.Create(IrOperation.Multiply,[Number(3),quantity]);
        Refused(SymbolicDerivativePointStatus.UnsupportedUnits,At(source,2));
    }

    [TestMethod]
    public void ExistingConditionCannotBeSilentlyDiscardedAtEvaluation()
    {
        var x=X();var restricted=new IrRestrictedExpression(Cube(x),
            IrAssumptionSet.Create([new IrRelation(x,IrRelationKind.NotEqual,Number(1))]));
        Refused(SymbolicDerivativePointStatus.UnsupportedAssumptions,At(restricted,2));
    }

    [TestMethod]
    public void BoundDerivativeTargetIsNotAFreeRealVariable()
    {
        var bound=IrSymbol.Bound("scope",0,"x");
        Refused(SymbolicDerivativePointStatus.UnsupportedDomain,
            At(Cube(X()),bound,new ExactRational(2,1)));
    }

    [TestMethod]
    public void IntegerDerivativeTargetDoesNotInheritRealContract()
    {
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Refused(SymbolicDerivativePointStatus.UnsupportedDomain,
            At(Cube(X()),integer,new ExactRational(2,1)));
    }

    [TestMethod]
    public void Binary64SourceIsNotAnExactPolynomial()
    {
        Refused(SymbolicDerivativePointStatus.Unsupported,
            At(IrFiniteBinary64Scalar.FromDouble(.25),2));
    }

    [TestMethod]
    public void DegreeNineCannotProduceAFalsePointwisePolynomialResult()
    {
        var degree9=IrApply.Create(IrOperation.Power,[X(),Number(9)]);
        Refused(SymbolicDerivativePointStatus.BudgetExceeded,At(degree9,2));
    }

    [TestMethod]
    public void NegativeExponentHasNoDefaultSingularityDerivativeRule()
    {
        var reciprocal=IrApply.Create(IrOperation.Power,[X(),Number(-1)]);
        Refused(SymbolicDerivativePointStatus.Unsupported,At(reciprocal,2));
    }

    [TestMethod]
    public void PointWithOversizedExactNumeratorTriggersBudgetRefusal()
    {
        var huge=new ExactRational(BigInteger.One<<129,BigInteger.One);
        Refused(SymbolicDerivativePointStatus.BudgetExceeded,
            At(Cube(X()),X(),huge));
    }

    [TestMethod]
    public void SourceWithUnrelatedAbsoluteSymbolIsUnsupported()
    {
        Refused(SymbolicDerivativePointStatus.Unsupported,
            At(Abs(Y()),X(),new ExactRational(2,1)));
    }

    [TestMethod]
    public void QualifiedResultNeverDependsOnForeignCasOrAgentBroker()
    {
        var references=typeof(SymbolicDerivativePointAnalysis).Assembly
            .GetReferencedAssemblies().Select(a=>a.Name??"").ToArray();
        Assert.IsFalse(references.Any(name=>
            name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
            name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
            name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void RefusedResultContainsNoNumericalValueOrProvenance()
    {
        var result=At(Abs(X()),0);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.VariableIdentity);
        Assert.IsFalse(result.Value.HasValue);
        Assert.IsFalse(result.HasExactValue);
    }

    [TestMethod]
    public void NullInputAndVariableAreNeverAccepted()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicDerivativePointAnalysis.Analyze((IrNode)null!,X(),ExactRational.One));
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicDerivativePointAnalysis.Analyze(Cube(X()),null!,ExactRational.One));
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicDerivativePointAnalysis.Analyze((SymbolicRealFunction)null!,ExactRational.One));
    }
}
