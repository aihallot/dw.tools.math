using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W02C01T1Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrSymbol Y()=>IrSymbol.Free("y","x");
    private static IrExactScalar Exact(ExactRational n)=>new(n);
    private static IrExactScalar Number(long n)=>Exact(new ExactRational(n,1));
    private static IrNode Power(IrSymbol symbol,long exponent)=>
        IrApply.Create(IrOperation.Power,[symbol,Number(exponent)]);
    private static SymbolicDerivativeResult D(IrNode source,IrSymbol? wrt=null)=>
        SymbolicExactDifferentiation.TryDifferentiate(source,wrt??X());
    private static IrNode Admitted(SymbolicDerivativeResult result)
    {
        if(!result.IsAdmitted || result.Derivative is not IrNode derivative)
            throw new AssertFailedException("The closed exact derivative was not admitted.");
        Assert.AreEqual(SymbolicExactDifferentiation.Rule,result.ProofRule);
        Assert.AreEqual(64,result.SourceSemanticSha256?.Length);
        Assert.IsNotNull(result.DifferentiationVariableIdentity);
        return derivative;
    }
    private static void Refused(
        SymbolicDifferentiationStatus status,SymbolicDerivativeResult result)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Derivative);
        Assert.IsNull(result.ProofRule);
        Assert.IsNull(result.SourceSemanticSha256);
        Assert.IsNull(result.DifferentiationVariableIdentity);
    }

    [TestMethod]
    public void FormalPolynomialCubicDifferentiatesToThreeTimesXSquared()
    {
        var x=X();var source=Power(x,3);
        var derivative=Admitted(D(source,x));
        var expected=IrApply.Create(IrOperation.Multiply,[
            Number(3),Power(x,2)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(derivative));
    }

    [TestMethod]
    public void DifferentiationWithRespectToAbsentFreeSymbolIsExactZero()
    {
        var x=X();var y=Y();
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admitted(D(Power(x,3),y))).Value);
    }

    [TestMethod]
    public void IndependentFreeSymbolIsExactZero()
    {
        var x=X();var y=Y();
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admitted(D(y,x))).Value);
    }

    [TestMethod]
    public void VariableItselfHasExactUnitDerivative()
    {
        Assert.AreEqual(ExactRational.One,
            ((IrExactScalar)Admitted(D(X()))).Value);
    }

    [TestMethod]
    public void AnyCanonicalExactConstantHasZeroDerivative()
    {
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admitted(D(Exact(new ExactRational(-7,3))))).Value);
    }

    [TestMethod]
    public void SquareOperatorHasExactTwiceVariableDerivative()
    {
        var x=X();
        var source=IrApply.Create(IrOperation.Square,[x]);
        var expected=IrApply.Create(IrOperation.Multiply,[Number(2),x]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(Admitted(D(source,x))));
    }

    [TestMethod]
    public void ExplicitPowerTwoMatchesSquareOperator()
    {
        var x=X();
        var square=Admitted(D(IrApply.Create(IrOperation.Square,[x]),x));
        var power=Admitted(D(Power(x,2),x));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(square),
            IrCanonicalJsonCodec.Encode(power));
    }

    [TestMethod]
    public void ExactPowerOneDifferentiatesToOne()
    {
        Assert.AreEqual(ExactRational.One,
            ((IrExactScalar)Admitted(D(Power(X(),1)))).Value);
    }

    [TestMethod]
    public void MaximumQualifiedPowerEightUsesExactIntegerCoefficient()
    {
        var x=X();var output=Admitted(D(Power(x,8)));
        var expected=IrApply.Create(IrOperation.Multiply,[
            Number(8),Power(x,7)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(output));
    }

    [TestMethod]
    public void PowerNineIsBudgetExceededNotWrongMathematics()
    {
        Refused(SymbolicDifferentiationStatus.BudgetExceeded,
            D(Power(X(),9)));
    }

    [TestMethod]
    public void ZeroPowerIsUnqualifiedBecauseOfZeroToZeroDomain()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(Power(X(),0)));
    }

    [TestMethod]
    public void NegativePowerIsNotDeclaredAUniversalPolynomial()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(Power(X(),-1)));
    }

    [TestMethod]
    public void RationalNonIntegerPowerDoesNotClaimComplexOrBranchPolicy()
    {
        var x=X();var source=IrApply.Create(IrOperation.Power,[
            x,Exact(new ExactRational(1,2))]);
        Refused(SymbolicDifferentiationStatus.Unsupported,D(source));
    }

    [TestMethod]
    public void PolynomialSumAndIndependentConstantDifferentiatesCorrectly()
    {
        var x=X();
        var polynomial=IrApply.Create(IrOperation.Add,[
            Power(x,3),Number(7)]);
        var output=Admitted(D(polynomial));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(
            IrApply.Create(IrOperation.Multiply,[Number(3),Power(x,2)])),
            IrCanonicalJsonCodec.Encode(output));
    }

    [TestMethod]
    public void SumOfTwoIndependentTermsYieldsExactZero()
    {
        var y=Y();
        var expression=IrApply.Create(IrOperation.Add,[Power(y,3),Number(7)]);
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admitted(D(expression))).Value);
    }

    [TestMethod]
    public void SubtractionOfAnExactConstantKeepsPolynomialDerivative()
    {
        var x=X();
        var expression=IrApply.Create(IrOperation.Subtract,[Power(x,3),Number(4)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(Admitted(D(Power(x,3)))),
            IrCanonicalJsonCodec.Encode(Admitted(D(expression))));
    }

    [TestMethod]
    public void ConstantMultiplierOfPolynomialDoesNotBecomeApproximate()
    {
        var x=X();
        var input=IrApply.Create(IrOperation.Multiply,[Number(2),Power(x,3)]);
        var result=Admitted(D(input));
        var expected=IrApply.Create(IrOperation.Multiply,[
            Number(2),IrApply.Create(IrOperation.Multiply,[Number(3),Power(x,2)])]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void ConstantMultiplierInReverseOrderHasTheSameExactDerivative()
    {
        var x=X();var source=IrApply.Create(IrOperation.Multiply,[
            Power(x,3),Number(2)]);
        var forward=IrApply.Create(IrOperation.Multiply,[Number(2),Power(x,3)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(Admitted(D(forward))),
            IrCanonicalJsonCodec.Encode(Admitted(D(source))));
    }

    [TestMethod]
    public void ZeroFactorDoesNotAccidentallyProduceAResultWithUnits()
    {
        var x=X();
        var zero=IrApply.Create(IrOperation.Multiply,[Number(0),Power(x,3)]);
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admitted(D(zero))).Value);
    }

    [TestMethod]
    public void NonconstantProductRuleIsExplicitlyNotAdmitted()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(IrApply.Create(IrOperation.Multiply,[X(),X()])));
    }

    [TestMethod]
    public void RationalDivisionHasNoUnqualifiedSingularityDerivative()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(IrApply.Create(IrOperation.Divide,[X(),Number(2)])));
    }

    [TestMethod]
    public void AbsoluteValueAtNondifferentiableZeroIsUnqualified()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(IrApply.Create(IrOperation.Abs,[X()])));
    }

    [TestMethod]
    public void RealSquareRootNeedsBranchAndDomainDifferentiabilityProof()
    {
        var sqrt=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[X()])]);
        Refused(SymbolicDifferentiationStatus.Unsupported,D(sqrt));
    }

    [TestMethod]
    public void QuantityWithUnitsIsExplicitlyUnsupported()
    {
        var length=new IrQuantityLiteral(Number(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicDifferentiationStatus.UnsupportedUnits,D(length));
    }

    [TestMethod]
    public void QuantityNestedUnderExactConstantStillRefusesDerivedUnits()
    {
        var length=new IrQuantityLiteral(Number(2),"imperial.inch",
            UnitSystem.BritishImperial,DimensionVector.LengthDimension);
        Refused(SymbolicDifferentiationStatus.UnsupportedUnits,
            D(IrApply.Create(IrOperation.Multiply,[Number(3),length])));
    }

    [TestMethod]
    public void RestrictedPolynomialRefusesToDiscardItsDomainCondition()
    {
        var x=X();
        var restricted=new IrRestrictedExpression(Power(x,3),
            IrAssumptionSet.Create([
                new IrRelation(x,IrRelationKind.NotEqual,Number(1))]));
        Refused(SymbolicDifferentiationStatus.UnsupportedAssumptions,D(restricted));
    }

    [TestMethod]
    public void BoundSourceSymbolDoesNotImpersonateFreeVariable()
    {
        var bound=IrSymbol.Bound("scope",0,"x");
        Refused(SymbolicDifferentiationStatus.UnsupportedDomain,D(bound));
    }

    [TestMethod]
    public void IntegerSourceSymbolCannotBeSilentlyPromotedToReal()
    {
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Refused(SymbolicDifferentiationStatus.UnsupportedDomain,D(integer));
    }

    [TestMethod]
    public void IntegerDifferentiationTargetIsUnsupportedDomain()
    {
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Refused(SymbolicDifferentiationStatus.UnsupportedDomain,
            D(Power(X(),3),integer));
    }

    [TestMethod]
    public void BoundDifferentiationTargetIsUnsupportedDomain()
    {
        var bound=IrSymbol.Bound("other",0,"x");
        Refused(SymbolicDifferentiationStatus.UnsupportedDomain,
            D(Power(X(),3),bound));
    }

    [TestMethod]
    public void ApproximateBinary64IsNotPassedOffAsExactDifferentiation()
    {
        Refused(SymbolicDifferentiationStatus.Unsupported,
            D(IrFiniteBinary64Scalar.FromDouble(.125)));
    }

    [TestMethod]
    public void MatrixCalculusIsNotImplicitlyARealScalarFunction()
    {
        var matrix=IrMatrix.Create(1,1,[Number(3)]);
        Refused(SymbolicDifferentiationStatus.Unsupported,D(matrix));
    }

    [TestMethod]
    public void RealFunctionHasExplicitDomainCodomainAndParameter()
    {
        var x=X();var body=Power(x,3);
        var f=new SymbolicRealFunction(x,body);
        Assert.AreEqual(IrScalarDomain.Real,f.Domain);
        Assert.AreEqual(IrScalarDomain.Real,f.Codomain);
        Assert.AreEqual(x.Identity,f.Parameter.Identity);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(body),
            IrCanonicalJsonCodec.Encode(f.Body));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(Admitted(D(body,x))),
            IrCanonicalJsonCodec.Encode(Admitted(
                SymbolicExactDifferentiation.TryDifferentiate(f))));
    }

    [TestMethod]
    public void IncompatibleFunctionCodomainAndParameterAreRejected()
    {
        Assert.ThrowsExactly<NotSupportedException>(()=>
            new SymbolicRealFunction(X(),Power(X(),3),
                codomain:IrScalarDomain.Integer));
        Assert.ThrowsExactly<NotSupportedException>(()=>
            new SymbolicRealFunction(IrSymbol.Bound("scope",0,"x"),Power(X(),3)));
        Assert.ThrowsExactly<NotSupportedException>(()=>
            new SymbolicRealFunction(X(),Power(X(),3),
                domain:IrScalarDomain.Integer));
    }

    [TestMethod]
    public void ResultProvenanceIdentifiesSourceAndDerivativeVariable()
    {
        var x=X();var y=Y();var source=Power(x,3);
        var dx=D(source,x);var dy=D(source,y);
        Assert.IsTrue(dx.IsAdmitted);
        Assert.IsTrue(dy.IsAdmitted);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(source),
            dx.SourceSemanticSha256);
        Assert.AreEqual(dx.SourceSemanticSha256,dy.SourceSemanticSha256);
        Assert.AreEqual(x.Identity,dx.DifferentiationVariableIdentity);
        Assert.AreEqual(y.Identity,dy.DifferentiationVariableIdentity);
        Assert.AreNotEqual(dx.DifferentiationVariableIdentity,
            dy.DifferentiationVariableIdentity);
    }

    [TestMethod]
    public void MathematicalResultRoundTripsThroughCanonicalTypedIR()
    {
        var x=X();var derivative=Admitted(D(Power(x,3)));
        var transported=IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(derivative));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(derivative),
            IrCanonicalJsonCodec.Encode(transported));
    }

    [TestMethod]
    public void PublicSymbolicProductHasNoCasOrBrokerReferences()
    {
        var deps=typeof(SymbolicExactDifferentiation).Assembly
            .GetReferencedAssemblies().Select(a=>a.Name??"").ToArray();
        Assert.IsFalse(deps.Any(n=>n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
            n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
            n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void NullInputsNeverProduceInventedDerivatives()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicExactDifferentiation.TryDifferentiate((IrNode)null!,X()));
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicExactDifferentiation.TryDifferentiate(Power(X(),3),null!));
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicExactDifferentiation.TryDifferentiate((SymbolicRealFunction)null!));
    }
}
