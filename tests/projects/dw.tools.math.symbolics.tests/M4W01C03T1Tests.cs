using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C03T1Tests
{
    private static IrSymbol X() => IrSymbol.Free("x","x",IrScalarDomain.Real);
    private static IrExactScalar Exact(ExactRational r) => new(r);
    private static IrExactScalar Number(long n) => Exact(new ExactRational(n,1));
    private static IrNode SquareOfSum(IrSymbol x,ExactRational c) =>
        IrApply.Create(IrOperation.Square,[
            IrApply.Create(IrOperation.Add,[x,Exact(c)])]);

    private static IrNode Admitted(SymbolicTransformResult result)
    {
        if (!result.IsAdmitted || result.Expression is null)
            throw new AssertFailedException("The bounded exact symbolic rule was not admitted.");
        Assert.IsNotNull(result.ProofRule);
        return result.Expression;
    }

    private static IrNode Expanded(IrNode source) =>
        Admitted(SymbolicExactTransformations.ExpandSquare(source));

    [TestMethod]
    public void ExpandSquareOfXPlusOneYieldsExactThreeTermPolynomial()
    {
        var x=X();
        var result=Expanded(SquareOfSum(x,ExactRational.One));
        var expected=IrApply.Create(IrOperation.Add,[
            IrApply.Create(IrOperation.Square,[x]),
            IrApply.Create(IrOperation.Multiply,[Number(2),x]),
            Number(1)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void ExpansionIsHandComputableForXEqualTwo()
    {
        // 2^2 + (2*2) + 1 = 9, agreeing with (2+1)^2.
        var x=X();
        var expanded=Expanded(SquareOfSum(x,ExactRational.One));
        if(expanded is not IrApply {Operation:IrOperation.Add} add)
            throw new AssertFailedException("Expected an exact three-term IR polynomial.");
        var middle=(IrApply)add.Arguments[1];
        var coefficient=((IrExactScalar)middle.Arguments[0]).Value;
        var constant=((IrExactScalar)add.Arguments[2]).Value;
        Assert.AreEqual(new ExactRational(9,1),
            new ExactRational(4,1)+coefficient*new ExactRational(2,1)+constant);
    }

    [TestMethod]
    public void NegativeOffsetIsNotSilentlyMadePositive()
    {
        var x=X();
        var expanded=Expanded(SquareOfSum(x,new ExactRational(-2,1)));
        if(expanded is not IrApply {Operation:IrOperation.Add} add)
            throw new AssertFailedException("Expected exact expansion.");
        var twice=(IrExactScalar)((IrApply)add.Arguments[1]).Arguments[0];
        Assert.AreEqual(new ExactRational(-4,1),twice.Value);
        Assert.AreEqual(new ExactRational(4,1),
            ((IrExactScalar)add.Arguments[2]).Value);
    }

    [TestMethod]
    public void FractionalOffsetNeverPassesThroughBinary64()
    {
        var x=X();
        var half=new ExactRational(1,2);
        var expanded=Expanded(SquareOfSum(x,half));
        var add=(IrApply)expanded;
        var twice=(IrExactScalar)((IrApply)add.Arguments[1]).Arguments[0];
        Assert.AreEqual(ExactRational.One,twice.Value);
        Assert.AreEqual(new ExactRational(1,4),
            ((IrExactScalar)add.Arguments[2]).Value);
        Assert.IsFalse(add.Arguments.Any(a=>a is IrFiniteBinary64Scalar));
    }

    [TestMethod]
    public void ZeroOffsetIsAllowedAndKeepsAnExactZeroCoefficient()
    {
        var x=X();
        var add=(IrApply)Expanded(SquareOfSum(x,ExactRational.Zero));
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)((IrApply)add.Arguments[1]).Arguments[0]).Value);
    }

    [TestMethod]
    public void FactorReversesOnlyTheQualifiedCanonicalExpansion()
    {
        var x=X();var original=SquareOfSum(x,new ExactRational(3,1));
        var expanded=Expanded(original);
        var factored=Admitted(SymbolicExactTransformations.FactorSquare(expanded));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(original),
            IrCanonicalJsonCodec.Encode(factored));
    }

    [TestMethod]
    public void FactorRefusesAnIncorrectConstantTerm()
    {
        var x=X();
        var wrong=IrApply.Create(IrOperation.Add,[
            IrApply.Create(IrOperation.Square,[x]),
            IrApply.Create(IrOperation.Multiply,[Number(4),x]),
            Number(3)]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.FactorSquare(wrong).Status);
    }

    [TestMethod]
    public void FactorRefusesAnUnknownShapeInsteadOfGuessing()
    {
        var x=X();
        var wrong=IrApply.Create(IrOperation.Multiply,[x,x]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.FactorSquare(wrong).Status);
    }

    [TestMethod]
    public void TypedOriginalRestrictionIsRetainedThroughExpansionAndFactor()
    {
        var x=X();
        var original=new IrRestrictedExpression(
            SquareOfSum(x,ExactRational.One),
            IrAssumptionSet.Create([new IrRelation(x,IrRelationKind.NotEqual,Number(1))]));
        var expanded=Expanded(original);
        if(expanded is not IrRestrictedExpression restricted)
            throw new AssertFailedException("The existing domain restriction disappeared.");
        Assert.AreEqual(IrRelationKind.NotEqual,restricted.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(ExactRational.One,restricted.Assumptions.Conditions[0].Right.Value);
        var factored=Admitted(SymbolicExactTransformations.FactorSquare(restricted));
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(original),
            IrCanonicalJsonCodec.Encode(factored));
    }

    [TestMethod]
    public void TypedExpansionSurvivesCanonicalIrTransport()
    {
        var x=X();var original=new IrRestrictedExpression(
            SquareOfSum(x,new ExactRational(2,1)),
            IrAssumptionSet.Create([new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]));
        var expanded=Expanded(original);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expanded),
            IrCanonicalJsonCodec.Encode(IrCanonicalJsonCodec.Decode(
                IrCanonicalJsonCodec.Encode(expanded))));
    }

    [TestMethod]
    public void ContradictoryOriginalEqualAndNotEqualRestrictionsAreRejected()
    {
        var x=X();
        var source=new IrRestrictedExpression(SquareOfSum(x,ExactRational.One),
            IrAssumptionSet.Create([
                new IrRelation(x,IrRelationKind.Equal,Number(2)),
                new IrRelation(x,IrRelationKind.NotEqual,Number(2))]));
        var refused=SymbolicExactTransformations.ExpandSquare(source);
        Assert.AreEqual(SymbolicTransformStatus.ContradictoryAssumptions,refused.Status);
        Assert.IsNull(refused.Expression);
    }

    [TestMethod]
    public void RestrictionsWithUnrelatedSymbolIdentitiesAreUnsupported()
    {
        var x=X();var other=IrSymbol.Free("y","x");
        var source=new IrRestrictedExpression(SquareOfSum(x,ExactRational.One),
            IrAssumptionSet.Create([new IrRelation(other,IrRelationKind.NotEqual,Number(1))]));
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.ExpandSquare(source).Status);
    }

    [TestMethod]
    public void OversizedExactBinomialCoefficientIsRefused()
    {
        var x=X();var large=new ExactRational(BigInteger.One<<129,BigInteger.One);
        Assert.AreEqual(SymbolicTransformStatus.BudgetExceeded,
            SymbolicExactTransformations.ExpandSquare(SquareOfSum(x,large)).Status);
    }

    [TestMethod]
    public void BoundSymbolCannotBeExpandedAsAnUnboundReal()
    {
        var bound=IrSymbol.Bound("scope",0,"x");
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.ExpandSquare(SquareOfSum(bound,ExactRational.One)).Status);
    }

    [TestMethod]
    public void IntegerDomainSymbolMustNotBeSilentlyTreatedAsReal()
    {
        var integer=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.ExpandSquare(SquareOfSum(integer,ExactRational.One)).Status);
    }

    [TestMethod]
    public void FreeIdentitySubstitutionChangesOnlyMatchingSymbol()
    {
        var x=X();var y=IrSymbol.Free("y","x");
        var expression=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        var result=Admitted(SymbolicExactTransformations.SubstituteFree(expression,x,y));
        var expected=IrApply.Create(IrOperation.Add,[y,Number(1)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void SubstitutingAClosedExactScalarDoesNotWidenIt()
    {
        var x=X();var source=SquareOfSum(x,ExactRational.One);
        var result=Admitted(SymbolicExactTransformations.SubstituteFree(
            source,x,Number(3)));
        var expected=IrApply.Create(IrOperation.Square,[
            IrApply.Create(IrOperation.Add,[Number(3),Number(1)])]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void SameDisplayDifferentIdentityDoesNotSubstitute()
    {
        var x=X();var other=IrSymbol.Free("y","x");
        var expression=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.SubstituteFree(expression,other,Number(5)).Status);
    }

    [TestMethod]
    public void BinderScopedSymbolRefusesSubstitutionRatherThanCapturing()
    {
        var bound=IrSymbol.Bound("bound-scope",0,"x");
        var x=X();
        var expression=IrApply.Create(IrOperation.Add,[bound,x]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.SubstituteFree(expression,x,Number(2)).Status);
    }

    [TestMethod]
    public void BoundReplacementIsNotAcceptedAsAFreeSymbol()
    {
        var x=X();var expression=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        var bound=IrSymbol.Bound("scope",0,"x");
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.SubstituteFree(expression,x,bound).Status);
    }

    [TestMethod]
    public void RestrictedSubstitutionCannotSilentlyDropAnAssumption()
    {
        var x=X();
        var restricted=new IrRestrictedExpression(
            IrApply.Create(IrOperation.Add,[x,Number(1)]),
            IrAssumptionSet.Create([new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]));
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.SubstituteFree(restricted,x,Number(2)).Status);
    }

    [TestMethod]
    public void ApproximateBinary64ReplacementIsUnsupported()
    {
        var x=X();var expr=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.SubstituteFree(expr,x,
                IrFiniteBinary64Scalar.FromDouble(.25d)).Status);
    }

    [TestMethod]
    public void SqrtSquareWithoutSignProofIsAbsoluteValue()
    {
        var x=X();var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var result=Admitted(SymbolicExactTransformations.NormalizeRealSqrtSquare(
            source,IrAssumptionSet.Empty));
        var expected=IrApply.Create(IrOperation.Abs,[x]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),
            IrCanonicalJsonCodec.Encode(result));
    }

    [TestMethod]
    public void SqrtSquareWithNonnegativeHypothesisReturnsRestrictedX()
    {
        var x=X();var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var conditions=IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]);
        var result=Admitted(SymbolicExactTransformations.NormalizeRealSqrtSquare(
            source,conditions));
        if(result is not IrRestrictedExpression restricted)
            throw new AssertFailedException("Sign assumption must be preserved.");
        Assert.AreEqual(x,restricted.Expression);
        Assert.AreEqual(IrRelationKind.GreaterOrEqual,
            restricted.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(ExactRational.Zero,
            restricted.Assumptions.Conditions[0].Right.Value);
    }

    [TestMethod]
    public void WrongScopeCannotProveSignOfSqrtSquare()
    {
        var x=X();var other=IrSymbol.Free("other","x");
        var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var wrong=IrAssumptionSet.Create([
            new IrRelation(other,IrRelationKind.GreaterOrEqual,Number(0))]);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.NormalizeRealSqrtSquare(source,wrong).Status);
    }

    [TestMethod]
    public void ContradictorySignAssumptionsDoNotProduceSuccess()
    {
        var x=X();var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var wrong=IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,Number(-1)),
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]);
        Assert.AreEqual(SymbolicTransformStatus.ContradictoryAssumptions,
            SymbolicExactTransformations.NormalizeRealSqrtSquare(source,wrong).Status);
    }

    [TestMethod]
    public void NoExternalCasBrokerOrModelDependencyIsAdded()
    {
        var names=typeof(SymbolicExactTransformations).Assembly.GetReferencedAssemblies()
            .Select(x=>x.Name??"").ToArray();
        Assert.IsFalse(names.Any(name=>
            name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
            name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
            name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
            name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void NullOrUnknownSymbolicSourcesCannotInventSuccess()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            SymbolicExactTransformations.ExpandSquare(null!));
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.FactorSquare(Number(3)).Status);
        Assert.AreEqual(SymbolicTransformStatus.Unsupported,
            SymbolicExactTransformations.NormalizeRealSqrtSquare(Number(1),
                IrAssumptionSet.Empty).Status);
    }
}
