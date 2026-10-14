using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C02T2Tests
{
    private static IrSymbol X() => IrSymbol.Free("x","x",IrScalarDomain.Real);
    private static IrExactScalar Exact(ExactRational r) => new(r);
    private static IrExactScalar Num(long i) => Exact(new ExactRational(i,1));
    private static IrNode Ratio(IrSymbol x,ExactRational c,bool power=false)
    {
        var square=power
            ? IrApply.Create(IrOperation.Power,[x,Num(2)])
            : IrApply.Create(IrOperation.Square,[x]);
        var numerator=IrApply.Create(IrOperation.Subtract,[square,Exact(c*c)]);
        var denominator=IrApply.Create(IrOperation.Subtract,[x,Exact(c)]);
        return IrApply.Create(IrOperation.Divide,[numerator,denominator]);
    }
    private static IrRestrictedExpression LocallyCertified(IrNode source)
    {
        var proven=SymbolicRationalCancellationBridge.TryCancel(source);
        if(!proven.IsAdmitted || proven.Conditional is not IrRestrictedExpression conditional)
            throw new AssertFailedException("Cannot build the independent exact mathematical oracle.");
        return conditional;
    }
    private static SymbolicProviderAdmissionResult Gate(IrNode source,object? candidate)=>
        SymbolicProviderResultAdmission.TryAdmit(source,candidate);
    private static void Refused(SymbolicProviderAdmissionStatus reason,
        SymbolicProviderAdmissionResult actual)
    {
        Assert.AreEqual(reason,actual.Status);
        Assert.IsFalse(actual.IsAdmitted);
        Assert.IsNull(actual.Conditional);
    }
    private sealed class HostileResult
    {
        public int ToStringCalls {get;private set;}
        public override string ToString()
        {
            ToStringCalls++;
            throw new InvalidOperationException("The untrusted observation must not be executed.");
        }
    }
    private sealed record ForeignIrNode : IrNode;

    [TestMethod]
    public void ExactTypedRestrictionIsAdmittedOnlyAgainstCertifiedSource()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var candidate=LocallyCertified(source);
        var result=Gate(source,candidate);
        Assert.AreEqual(SymbolicProviderAdmissionStatus.Admitted,result.Status);
        Assert.IsTrue(result.IsAdmitted);
        Assert.IsNotNull(result.Conditional);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(candidate),
            IrCanonicalJsonCodec.Encode(result.Conditional));
    }

    [TestMethod]
    public void SuccessfulResultReturnsLocallyOwnedConditionalNotForeignInstance()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var candidate=LocallyCertified(source);
        var admitted=Gate(source,candidate);
        Assert.IsNotNull(admitted.Conditional);
        Assert.AreNotSame(candidate,admitted.Conditional);
    }

    [TestMethod]
    public void RevalidatedPoleStillHasNoExactValueAtOne()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var admitted=Gate(source,LocallyCertified(source));
        var verified=SymbolicRationalCancellationBridge.TryCancel(source);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(verified.Conditional!),
            IrCanonicalJsonCodec.Encode(admitted.Conditional!));
        var result=SymbolicRationalCancellationBridge.EvaluateExactAt(verified,
            x,ExactRational.One);
        Assert.AreEqual(SymbolicPointStatus.Excluded,result.Status);
        Assert.IsFalse(result.Value.HasValue);
    }

    [TestMethod]
    public void RevalidatedOrdinaryPointKeepsExactThree()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        Assert.IsTrue(Gate(source,LocallyCertified(source)).IsAdmitted);
        var observed=SymbolicRationalCancellationBridge.EvaluateExactAt(
            SymbolicRationalCancellationBridge.TryCancel(source),
            x,new ExactRational(2,1));
        Assert.AreEqual(new ExactRational(3,1),observed.Value);
    }

    [TestMethod]
    public void UnconditionalReducedExpressionIsRejectedEvenIfAlgebraMatches()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var bare=IrApply.Create(IrOperation.Add,[x,Num(1)]);
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,Gate(source,bare));
    }

    [TestMethod]
    public void HumanReadableProvidedClauseIsNotAnExecutableProof()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,
            Gate(source,"(x+1) provided x != 1"));
    }

    [TestMethod]
    public void HumanReadablePlainInequalityIsAlsoRefused()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,Gate(source,"x != 1"));
    }

    [TestMethod]
    public void HostileOpaqueProviderObjectCannotInvokeToString()
    {
        var x=X();var hostile=new HostileResult();
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,
            Gate(Ratio(x,ExactRational.One),hostile));
        Assert.AreEqual(0,hostile.ToStringCalls);
    }

    [TestMethod]
    public void MissingProposedObjectIsTypedRefusal()
    {
        var x=X();
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,
            Gate(Ratio(x,ExactRational.One),null));
    }

    [TestMethod]
    public void CandidateWithoutPoleConstraintCannotBeAccepted()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var expected=LocallyCertified(source);
        var unguarded=new IrRestrictedExpression(expected.Expression,IrAssumptionSet.Empty);
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,unguarded));
    }

    [TestMethod]
    public void CandidateWithIncorrectExcludedPointCannotBeAccepted()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var expected=LocallyCertified(source);
        var wrong=new IrRestrictedExpression(expected.Expression,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,Num(0))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,wrong));
    }

    [TestMethod]
    public void CandidateWithContradictoryEqualityAtPoleIsNotAccepted()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var expected=LocallyCertified(source);
        var wrong=new IrRestrictedExpression(expected.Expression,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,wrong));
    }

    [TestMethod]
    public void UnsupportedAddedAssumptionDoesNotBecomeAccepted()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var expected=LocallyCertified(source);
        var extra=new IrRestrictedExpression(expected.Expression,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,Num(1)),
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Num(0))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,extra));
    }

    [TestMethod]
    public void AlteredReducedExpressionCannotStealOriginalProof()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var wrong=new IrRestrictedExpression(
            IrApply.Create(IrOperation.Add,[x,Num(2)]),IrAssumptionSet.Create([
                new IrRelation(x,IrRelationKind.NotEqual,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,wrong));
    }

    [TestMethod]
    public void RenamedIdentityWithSamePresentationCannotBeTrusted()
    {
        var x=X();var y=IrSymbol.Free("y","x");
        var source=Ratio(x,ExactRational.One);
        var wrong=new IrRestrictedExpression(
            IrApply.Create(IrOperation.Add,[y,Num(1)]),IrAssumptionSet.Create([
                new IrRelation(y,IrRelationKind.NotEqual,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,wrong));
    }

    [TestMethod]
    public void ChangedSymbolDisplayDoesNotPassStrictCanonicalBoundary()
    {
        var x=X();var alternate=IrSymbol.Free("x","external-label");
        var source=Ratio(x,ExactRational.One);
        var wrong=new IrRestrictedExpression(
            IrApply.Create(IrOperation.Add,[alternate,Num(1)]),IrAssumptionSet.Create([
                new IrRelation(alternate,IrRelationKind.NotEqual,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,Gate(source,wrong));
    }

    [TestMethod]
    public void UnsupportedSquareRootSourceRemainsUnsupported()
    {
        var x=X();
        var source=IrApply.Create(IrOperation.Sqrt,[IrApply.Create(IrOperation.Square,[x])]);
        Refused(SymbolicProviderAdmissionStatus.UnsupportedSource,
            Gate(source,LocallyCertified(Ratio(x,ExactRational.One))));
    }

    [TestMethod]
    public void IntegerDomainCannotBeUpgradedToRealByProvider()
    {
        var x=IrSymbol.Free("x","x",IrScalarDomain.Integer);
        var real=X();
        Refused(SymbolicProviderAdmissionStatus.UnsupportedSource,
            Gate(Ratio(x,ExactRational.One),LocallyCertified(Ratio(real,ExactRational.One))));
    }

    [TestMethod]
    public void ContradictoryOriginalEqualityAtPoleIsRefusedBeforeAdmission()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var contradictory=new IrRestrictedExpression(source,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.ContradictoryAssumptions,
            Gate(contradictory,LocallyCertified(source)));
    }

    [TestMethod]
    public void OversizedExactCoefficientCannotClaimBoundedProviderSuccess()
    {
        var x=X();var huge=new ExactRational(BigInteger.One<<129,BigInteger.One);
        Refused(SymbolicProviderAdmissionStatus.BudgetExceeded,
            Gate(Ratio(x,huge),LocallyCertified(Ratio(x,ExactRational.One))));
    }

    [TestMethod]
    public void ExistingNonnegativeRestrictionIsRetainedInAcceptedOutput()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var restricted=new IrRestrictedExpression(source,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Num(0))]));
        var admitted=Gate(restricted,LocallyCertified(restricted));
        Assert.IsTrue(admitted.IsAdmitted);
        Assert.AreEqual(2,admitted.Conditional!.Assumptions.Conditions.Length);
    }

    [TestMethod]
    public void OriginalNonnegativeRestrictionCannotBeDroppedByProposal()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var restricted=new IrRestrictedExpression(source,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Num(0))]));
        Refused(SymbolicProviderAdmissionStatus.ConditionalMismatch,
            Gate(restricted,LocallyCertified(source)));
    }

    [TestMethod]
    public void FractionalPoleUsesCanonicalExactTypedOutput()
    {
        var x=X();var half=new ExactRational(1,2);
        var source=Ratio(x,half);
        var admitted=Gate(source,LocallyCertified(source));
        Assert.IsTrue(admitted.IsAdmitted);
        Assert.AreEqual(half,admitted.Conditional!.Assumptions.Conditions[0].Right.Value);
    }

    [TestMethod]
    public void EquivalentSquareAndExplicitPowerHaveCanonicalAgreement()
    {
        var x=X();var source=Ratio(x,ExactRational.One,power:true);
        Assert.IsTrue(Gate(source,LocallyCertified(source)).IsAdmitted);
    }

    [TestMethod]
    public void ForeignSubclassOfIrNodeIsRefusedNotAcceptedAsOpaqueMath()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        var foreign=new IrRestrictedExpression(new ForeignIrNode(),IrAssumptionSet.Empty);
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,Gate(source,foreign));
    }

    [TestMethod]
    public void UnrelatedRestrictionSymbolFailsCanonicalDecode()
    {
        var x=X();var unrelated=IrSymbol.Free("unrelated","x");
        var source=Ratio(x,ExactRational.One);
        var expr=LocallyCertified(source).Expression;
        var wrong=new IrRestrictedExpression(expr,IrAssumptionSet.Create([
            new IrRelation(unrelated,IrRelationKind.NotEqual,Num(1))]));
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,Gate(source,wrong));
    }

    [TestMethod]
    public void ExternalApproximateScalarIsNotAcceptedAsExactMath()
    {
        var x=X();var source=Ratio(x,ExactRational.One);
        Refused(SymbolicProviderAdmissionStatus.UnrepresentableOutput,
            Gate(source,IrFiniteBinary64Scalar.FromDouble(.25d)));
    }

    [TestMethod]
    public void PublicProductNeverReferencesExternalCasOrBrokerAssemblies()
    {
        var names=typeof(SymbolicProviderResultAdmission).Assembly.GetReferencedAssemblies()
            .Select(x=>x.Name??"").ToArray();
        Assert.IsFalse(names.Any(n=>n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
            n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
            n.Contains("AURA",StringComparison.OrdinalIgnoreCase)||
            n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
            n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void NullSourceIsNeverTreatedAsAProviderClaim()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            SymbolicProviderResultAdmission.TryAdmit(null!,new object()));
    }
}
