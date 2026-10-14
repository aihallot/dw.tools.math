using System.Numerics;
using System.Threading;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Symbolics.Tests;

[TestClass]
public sealed class M4W01C03T2Tests
{
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrExactScalar Number(long value)=>
        new(new ExactRational(value,1));
    private static IrNode Binomial(IrSymbol x, ExactRational constant)=>
        IrApply.Create(IrOperation.Square,[
            IrApply.Create(IrOperation.Add,[x,new IrExactScalar(constant)])]);
    private static IrNode Binomial()=>Binomial(X(),ExactRational.One);
    private static SymbolicBoundedExecutionResult Run(
        SymbolicBoundedOperation operation,IrNode source,
        SymbolicWorkBudget? budget=null,IrSymbol? symbol=null,
        IrNode? replacement=null,IrAssumptionSet? assumptions=null,
        CancellationToken cancellation=default)=>
        SymbolicBoundedExecution.Execute(operation,source,
            budget??new SymbolicWorkBudget(),symbol,replacement,assumptions,cancellation);

    private static IrNode Admitted(SymbolicBoundedExecutionResult result)
    {
        if(!result.IsAdmitted || result.Expression is not IrNode expression)
            throw new AssertFailedException("A qualified closed mathematical rule must be admitted.");
        Assert.IsNotNull(result.ProofRule);
        Assert.IsNotNull(result.SourceSemanticSha256);
        return expression;
    }

    private static void Refused(SymbolicBoundedStatus status,SymbolicBoundedExecutionResult result)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.ProofRule);
        Assert.IsNull(result.SourceSemanticSha256);
    }

    [TestMethod]
    public void DefaultBoundedExpansionUsesTheQualifiedExactRule()
    {
        var source=Binomial();
        var result=Run(SymbolicBoundedOperation.ExpandSquare,source);
        var output=Admitted(result);
        Assert.AreEqual(SymbolicExactTransformations.ExpandRule,result.ProofRule);
        var x=X();
        var expected=IrApply.Create(IrOperation.Add,[
            IrApply.Create(IrOperation.Square,[x]),
            IrApply.Create(IrOperation.Multiply,[Number(2),x]),
            Number(1)]);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(expected),IrCanonicalJsonCodec.Encode(output));
    }

    [TestMethod]
    public void ExpansionReportsActualBoundedInputAndOutputNodes()
    {
        var actual=Run(SymbolicBoundedOperation.ExpandSquare,Binomial());
        Assert.IsTrue(actual.IsAdmitted);
        Assert.AreEqual(4,actual.InputNodes);
        Assert.AreEqual(7,actual.OutputNodes);
        Assert.AreEqual(11,actual.WorkUnits);
        Assert.IsTrue(actual.Elapsed>=TimeSpan.Zero);
    }

    [TestMethod]
    public void ProvenanceIdentifiesOriginalStructuralIrNotProviderText()
    {
        var source=Binomial();
        var result=Run(SymbolicBoundedOperation.ExpandSquare,source);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(source),
            result.SourceSemanticSha256);
        Assert.AreEqual(64,result.SourceSemanticSha256?.Length);
    }

    [TestMethod]
    public void FactoringRetainsTheClosedExactRoundTrip()
    {
        var source=Binomial();
        var expanded=Admitted(Run(SymbolicBoundedOperation.ExpandSquare,source));
        var factored=Run(SymbolicBoundedOperation.FactorSquare,expanded);
        Assert.AreEqual(SymbolicExactTransformations.FactorRule,factored.ProofRule);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(source),
            IrCanonicalJsonCodec.Encode(Admitted(factored)));
    }

    [TestMethod]
    public void SubstitutionUsesSymbolIdentityAndExactReplacement()
    {
        var x=X();var y=IrSymbol.Free("y","x");
        var source=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        var result=Run(SymbolicBoundedOperation.SubstituteFree,source,
            symbol:x,replacement:y);
        Assert.AreEqual(SymbolicExactTransformations.SubstitutionRule,result.ProofRule);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(
            IrApply.Create(IrOperation.Add,[y,Number(1)])),
            IrCanonicalJsonCodec.Encode(Admitted(result)));
    }

    [TestMethod]
    public void BoundScopedSourceCannotBeRewrittenAsUnbound()
    {
        var x=X();var bound=IrSymbol.Bound("local",0,"x");
        var source=IrApply.Create(IrOperation.Add,[bound,x]);
        Refused(SymbolicBoundedStatus.Unsupported,Run(
            SymbolicBoundedOperation.SubstituteFree,source,
            symbol:x,replacement:Number(2)));
    }

    [TestMethod]
    public void UnknownReplacementVariableIdentityIsNotCaptured()
    {
        var x=X();var y=IrSymbol.Free("y","x");
        var source=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        Refused(SymbolicBoundedStatus.Unsupported,Run(
            SymbolicBoundedOperation.SubstituteFree,source,
            symbol:y,replacement:Number(2)));
    }

    [TestMethod]
    public void SqrtOfRealSquareWithoutSignProofIsAbsolute()
    {
        var x=X();
        var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var result=Run(SymbolicBoundedOperation.NormalizeRealSqrtSquare,source);
        Assert.AreEqual(SymbolicExactTransformations.SqrtRule,result.ProofRule);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(
            IrApply.Create(IrOperation.Abs,[x])),
            IrCanonicalJsonCodec.Encode(Admitted(result)));
    }

    [TestMethod]
    public void NonnegativeSqrtProofRemainsAttachedAsARestriction()
    {
        var x=X();var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var assumptions=IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]);
        var result=Run(SymbolicBoundedOperation.NormalizeRealSqrtSquare,
            source,assumptions:assumptions);
        if(Admitted(result) is not IrRestrictedExpression restricted)
            throw new AssertFailedException("Typed sign condition must survive normalization.");
        Assert.AreEqual(x,restricted.Expression);
        Assert.AreEqual(IrRelationKind.GreaterOrEqual,
            restricted.Assumptions.Conditions[0].Kind);
    }

    [TestMethod]
    public void ConflictingSignAssumptionsNeverProduceAProof()
    {
        var x=X();var source=IrApply.Create(IrOperation.Sqrt,[
            IrApply.Create(IrOperation.Square,[x])]);
        var impossible=IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,Number(-1)),
            new IrRelation(x,IrRelationKind.GreaterOrEqual,Number(0))]);
        Refused(SymbolicBoundedStatus.ContradictoryAssumptions,Run(
            SymbolicBoundedOperation.NormalizeRealSqrtSquare,
            source,assumptions:impossible));
    }

    [TestMethod]
    public void UnrecognizedTransformShapeIsUnsupportedWithoutFalseProvenance()
    {
        var x=X();
        var unproved=IrApply.Create(IrOperation.Multiply,[x,x]);
        Refused(SymbolicBoundedStatus.Unsupported,
            Run(SymbolicBoundedOperation.ExpandSquare,unproved));
    }

    [TestMethod]
    public void UnexpectedArgumentsCannotBeIgnoredByAnOtherwiseProvedRule()
    {
        Refused(SymbolicBoundedStatus.Unsupported,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),
            symbol:X(),replacement:Number(1)));
    }

    [TestMethod]
    public void SubstitutionRequiresExplicitNamedVariableAndReplacement()
    {
        var x=X();var source=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        Refused(SymbolicBoundedStatus.Unsupported,Run(
            SymbolicBoundedOperation.SubstituteFree,source));
    }

    [TestMethod]
    public void UnrecognizedOperationCannotDispatchAnyMathResult()
    {
        Refused(SymbolicBoundedStatus.Unsupported,
            Run((SymbolicBoundedOperation)999,Binomial()));
    }

    [TestMethod]
    public void InputNodeBudgetRefusesOtherwiseValidBinomial()
    {
        var budget=new SymbolicWorkBudget(maximumNodes:3);
        Refused(SymbolicBoundedStatus.BudgetExceeded,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),budget));
    }

    [TestMethod]
    public void InputDepthBudgetRefusesNestedBinomial()
    {
        var budget=new SymbolicWorkBudget(maximumDepth:2);
        Refused(SymbolicBoundedStatus.BudgetExceeded,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),budget));
    }

    [TestMethod]
    public void CombinedInputOutputWorkBudgetRejectsLateOversizedResult()
    {
        var budget=new SymbolicWorkBudget(maximumWorkUnits:10);
        var result=Run(SymbolicBoundedOperation.ExpandSquare,Binomial(),budget);
        Refused(SymbolicBoundedStatus.BudgetExceeded,result);
        Assert.AreEqual(4,result.InputNodes);
    }

    [TestMethod]
    public void CancellationAlreadyRequestedIsNotSuccessfulComputation()
    {
        using var cancellation=new CancellationTokenSource();
        cancellation.Cancel();
        Refused(SymbolicBoundedStatus.Cancelled,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),
            cancellation:cancellation.Token));
    }

    [TestMethod]
    public void ZeroSoftWallClockBudgetCannotAdmitAResult()
    {
        var budget=new SymbolicWorkBudget(maximumElapsed:TimeSpan.Zero);
        Refused(SymbolicBoundedStatus.DeadlineExceeded,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),budget));
    }

    [TestMethod]
    public void CancellationIsNotHiddenByAnAlreadyExpiredDeadline()
    {
        using var cancellation=new CancellationTokenSource();
        cancellation.Cancel();
        var budget=new SymbolicWorkBudget(maximumElapsed:TimeSpan.Zero);
        Refused(SymbolicBoundedStatus.Cancelled,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(),
            budget,cancellation:cancellation.Token));
    }

    [TestMethod]
    public void AlreadyQualifiedCoefficientOverflowStaysTypedBudgetExceeded()
    {
        var x=X();
        var huge=new ExactRational(BigInteger.One<<129,BigInteger.One);
        Refused(SymbolicBoundedStatus.BudgetExceeded,Run(
            SymbolicBoundedOperation.ExpandSquare,Binomial(x,huge)));
    }

    [TestMethod]
    public void BoundSymbolAsReplacementStillCannotBeCaptured()
    {
        var x=X();var bound=IrSymbol.Bound("scope",0,"x");
        var source=IrApply.Create(IrOperation.Add,[x,Number(1)]);
        Refused(SymbolicBoundedStatus.Unsupported,Run(
            SymbolicBoundedOperation.SubstituteFree,source,
            symbol:x,replacement:bound));
    }

    [TestMethod]
    public void InvalidBudgetValuesFailAtConstruction()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            new SymbolicWorkBudget(maximumNodes:IrGraphLimits.MaximumNodes+1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            new SymbolicWorkBudget(maximumDepth:0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            new SymbolicWorkBudget(maximumWorkUnits:0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            new SymbolicWorkBudget(maximumElapsed:TimeSpan.FromSeconds(31)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()=>
            new SymbolicWorkBudget(maximumElapsed:TimeSpan.FromTicks(-1)));
    }

    [TestMethod]
    public void ExistingTypedDomainRestrictionSurvivesBudgetedExpansion()
    {
        var x=X();var restricted=new IrRestrictedExpression(Binomial(),
            IrAssumptionSet.Create([
                new IrRelation(x,IrRelationKind.NotEqual,Number(1))]));
        var result=Run(SymbolicBoundedOperation.ExpandSquare,restricted);
        if(Admitted(result) is not IrRestrictedExpression typed)
            throw new AssertFailedException("Known x != 1 restriction disappeared.");
        Assert.AreEqual(IrRelationKind.NotEqual,typed.Assumptions.Conditions[0].Kind);
        Assert.AreEqual(ExactRational.One,typed.Assumptions.Conditions[0].Right.Value);
    }

    [TestMethod]
    public void ProductRemainsFreeOfUnqualifiedCasAndAgentDependencies()
    {
        var deps=typeof(SymbolicBoundedExecution).Assembly.GetReferencedAssemblies()
            .Select(a=>a.Name??"").ToArray();
        Assert.IsFalse(deps.Any(name=>
            name.Contains("Angouri",StringComparison.OrdinalIgnoreCase) ||
            name.Contains("MathNet",StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Aura",StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Decision",StringComparison.OrdinalIgnoreCase) ||
            name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void NullInputAndBudgetAreNeverSilentlyAccepted()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicBoundedExecution.Execute(
                SymbolicBoundedOperation.ExpandSquare,null!,new SymbolicWorkBudget()));
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            SymbolicBoundedExecution.Execute(
                SymbolicBoundedOperation.ExpandSquare,Binomial(),null!));
    }
}
