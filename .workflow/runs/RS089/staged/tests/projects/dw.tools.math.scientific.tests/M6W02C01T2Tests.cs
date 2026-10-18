using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C01T2Tests
{
    private static ExactScientificAuditResult G(BigInteger a,BigInteger b,
        SignedOperandPolicy sign=SignedOperandPolicy.RejectNegative,
        ExactScientificBudget? budget=null) =>
        ExactScientificBoundaryAudit.GreatestCommonDivisor(a,b,sign,budget);
    private static ExactScientificAuditResult L(BigInteger a,BigInteger b,
        SignedOperandPolicy sign=SignedOperandPolicy.RejectNegative,
        ExactScientificBudget? budget=null) =>
        ExactScientificBoundaryAudit.LeastCommonMultiple(a,b,sign,budget);
    private static ExactScientificAuditResult F(int n,ExactScientificBudget? budget=null) =>
        ExactScientificBoundaryAudit.Factorial(n,budget);
    private static ExactScientificAuditResult B(int n,int k,ExactScientificBudget? budget=null) =>
        ExactScientificBoundaryAudit.Binomial(n,k,budget);
    private static ExactScientificAuditResult S(BigInteger first,BigInteger difference,
        int count,int maximumIntermediateBits=4096,ExactScientificBudget? budget=null) =>
        ExactScientificBoundaryAudit.SumArithmeticSequence(
            first,difference,count,maximumIntermediateBits,budget);

    private static void Exact(ExactScientificAuditResult value,BigInteger expected,
        bool normalized=false)
    {
        Assert.IsTrue(value.Succeeded);
        Assert.AreEqual(ExactScientificStatus.Success,value.Status);
        Assert.IsNotNull(value.Value);
        Assert.AreEqual(expected,value.Value.Value);
        Assert.AreEqual(normalized,value.NegativeOperandsNormalized);
        Assert.IsTrue(value.Steps>=0);
        Assert.AreEqual(ExactScientificBoundaryAudit.RuleId,value.RuleId);
    }
    private static void Refused(ExactScientificAuditResult value,ExactScientificStatus status)
    {
        Assert.AreEqual(status,value.Status);
        Assert.IsFalse(value.Succeeded);
        Assert.IsNull(value.Value);
        Assert.AreEqual(0,value.Steps);
        Assert.IsFalse(value.NegativeOperandsNormalized);
        Assert.IsNull(value.MaximumIntermediateBitsObserved);
        Assert.AreEqual(ExactScientificBoundaryAudit.RuleId,value.RuleId);
    }

    [TestMethod] public void GcdDefaultNegativeLeftIsRefused()
    { Refused(G(-48,18),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void GcdDefaultNegativeRightIsRefused()
    { Refused(G(48,-18),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void GcdDefaultBothNegativeAreRefused()
    { Refused(G(-48,-18),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void GcdExplicitMagnitudeNormalizesLeft()
    { Exact(G(-48,18,SignedOperandPolicy.NormalizeMagnitude),6,true); }
    [TestMethod] public void GcdExplicitMagnitudeNormalizesRight()
    { Exact(G(48,-18,SignedOperandPolicy.NormalizeMagnitude),6,true); }
    [TestMethod] public void GcdExplicitMagnitudeNormalizesBoth()
    { Exact(G(-48,-18,SignedOperandPolicy.NormalizeMagnitude),6,true); }
    [TestMethod] public void GcdPositiveInputDoesNotClaimNormalization()
    { Exact(G(48,18,SignedOperandPolicy.NormalizeMagnitude),6); }
    [TestMethod] public void GcdNormalizedZeroAndNegativeIsEight()
    { Exact(G(0,-8,SignedOperandPolicy.NormalizeMagnitude),8,true); }
    [TestMethod] public void GcdBothZeroRemainZero()
    { Exact(G(0,0,SignedOperandPolicy.NormalizeMagnitude),0); }
    [TestMethod] public void GcdPositiveCoprimeIsOne()
    { Exact(G(35,64),1); }
    [TestMethod] public void GcdDefaultPositiveValuesRetainT1()
    { Exact(G(48,18),6); }
    [TestMethod] public void GcdUnknownSignedPolicyRefuses()
    { Refused(G(-48,18,(SignedOperandPolicy)99),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void GcdNormalizedOperandBitsStillBounded()
    { Refused(G(-(BigInteger.One<<129),1,SignedOperandPolicy.NormalizeMagnitude),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void GcdNormalizationDoesNotOverrideResultSize()
    { Refused(G(-12,6,SignedOperandPolicy.NormalizeMagnitude,new(128,2,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void GcdStepBudgetStillAppliesAfterNormalization()
    { Refused(G(-48,18,SignedOperandPolicy.NormalizeMagnitude,new(128,4096,2)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void GcdJustEnoughEuclidIterationsSucceeds()
    { var r=G(-48,18,SignedOperandPolicy.NormalizeMagnitude,new(128,4096,3));Exact(r,6,true);Assert.AreEqual(3,r.Steps); }
    [TestMethod] public void GcdInvalidScientificBudgetRefuses()
    { Refused(G(48,18,budget:new(0,4096,4096)),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void GcdCancelledBeforeMagnitudeNormalizationRefuses()
    { using var token=new CancellationTokenSource();token.Cancel();Refused(ExactScientificBoundaryAudit.GreatestCommonDivisor(-48,18,SignedOperandPolicy.NormalizeMagnitude,cancellationToken:token.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void LcmDefaultNegativeLeftIsRefused()
    { Refused(L(-6,8),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void LcmDefaultNegativeRightIsRefused()
    { Refused(L(6,-8),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void LcmExplicitMagnitudeNormalizesLeft()
    { Exact(L(-6,8,SignedOperandPolicy.NormalizeMagnitude),24,true); }
    [TestMethod] public void LcmExplicitMagnitudeNormalizesRight()
    { Exact(L(6,-8,SignedOperandPolicy.NormalizeMagnitude),24,true); }
    [TestMethod] public void LcmExplicitMagnitudeNormalizesBoth()
    { Exact(L(-6,-8,SignedOperandPolicy.NormalizeMagnitude),24,true); }
    [TestMethod] public void LcmPositiveSixEightRemainsTwentyFour()
    { Exact(L(6,8),24); }
    [TestMethod] public void LcmPositiveInputsDoNotSignalNormalization()
    { Exact(L(6,8,SignedOperandPolicy.NormalizeMagnitude),24); }
    [TestMethod] public void LcmSignedZeroAdmittedWithNormalization()
    { Exact(L(-8,0,SignedOperandPolicy.NormalizeMagnitude),0,true); }
    [TestMethod] public void LcmBothZeroStayZero()
    { Exact(L(0,0),0); }
    [TestMethod] public void LcmUnknownSignPolicyIsInvalid()
    { Refused(L(-6,8,(SignedOperandPolicy)99),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void LcmSignedTooLargeOperandRefuses()
    { Refused(L(-(BigInteger.One<<129),1,SignedOperandPolicy.NormalizeMagnitude),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void LcmSignedOutputLimitIsEnforced()
    { Refused(L(-6,8,SignedOperandPolicy.NormalizeMagnitude,new(128,4,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void LcmSignedGcdPlusMultiplyStepsAreBounded()
    { Refused(L(-48,18,SignedOperandPolicy.NormalizeMagnitude,new(128,4096,3)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void LcmFourStepsIncludeFinalMultiplication()
    { var r=L(-48,18,SignedOperandPolicy.NormalizeMagnitude,new(128,4096,4));Exact(r,144,true);Assert.AreEqual(4,r.Steps); }
    [TestMethod] public void LcmInvalidBudgetRefuses()
    { Refused(L(6,8,budget:new(128,0,4096)),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void LcmCancelledBeforeExecutionRefuses()
    { using var token=new CancellationTokenSource();token.Cancel();Refused(ExactScientificBoundaryAudit.LeastCommonMultiple(-6,8,SignedOperandPolicy.NormalizeMagnitude,cancellationToken:token.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void ZeroFactorialStillExactlyOne()
    { Exact(F(0),1); }
    [TestMethod] public void OneFactorialStillExactlyOne()
    { Exact(F(1),1); }
    [TestMethod] public void FiveFactorialStillExactlyOneHundredTwenty()
    { Exact(F(5),120); }
    [TestMethod] public void FactorialTwentyMatchesIndependentOracle()
    { Exact(F(20),BigInteger.Parse("2432902008176640000")); }
    [TestMethod] public void NegativeFactorialRefuses()
    { Refused(F(-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void ExcessiveFactorialArgumentRefuses()
    { Refused(F(257),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void FactorialIntermediateGrowthKeepsResultCap()
    { Refused(F(10,new(128,16,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void FactorialStrictStepBudgetRefuses()
    { Refused(F(5,new(128,4096,3)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void FactorialInvalidBudgetRefuses()
    { Refused(F(5,new(128,4096,0)),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void FactorialNeverClaimsSignedNormalization()
    { var r=F(10);Exact(r,3628800);Assert.IsNull(r.MaximumIntermediateBitsObserved); }
    [TestMethod] public void FactorialPrecancellationReturnsNonpartial()
    { using var token=new CancellationTokenSource();token.Cancel();Refused(ExactScientificBoundaryAudit.Factorial(5,cancellationToken:token.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void FiveChooseTwoStillTen()
    { Exact(B(5,2),10); }
    [TestMethod] public void BinomialEndPointsAreOne()
    { Exact(B(5,0),1);Exact(B(5,5),1); }
    [TestMethod] public void BinomialFiftyTwoChooseFiveMatchesOracle()
    { Exact(B(52,5),2598960); }
    [TestMethod] public void BinomialTwentyChooseTenMatchesOracle()
    { Exact(B(20,10),184756); }
    [TestMethod] public void BinomialNegativeNRefuses()
    { Refused(B(-1,0),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialNegativeKRefuses()
    { Refused(B(5,-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialKGreaterNRefuses()
    { Refused(B(5,6),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialOverHardArgumentBoundRefuses()
    { Refused(B(257,1),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void BinomialOutputBitLimitRefuses()
    { Refused(B(5,2,new(128,3,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void BinomialIterationLimitRefuses()
    { Refused(B(10,3,new(128,4096,2)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void BinomialDoesNotDeclareNormalizationOrIntermediateAudit()
    { var r=B(10,3);Exact(r,120);Assert.IsNull(r.MaximumIntermediateBitsObserved); }
    [TestMethod] public void BinomialCancelledBeforeWorkRefuses()
    { using var token=new CancellationTokenSource();token.Cancel();Refused(ExactScientificBoundaryAudit.Binomial(10,3,cancellationToken:token.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void ArithmeticProgressionOneThreeFiveSevenNineIsTwentyFive()
    { Exact(S(1,2,5),25); }
    [TestMethod] public void ArithmeticSeriesSignedStartIsExact()
    { Exact(S(-3,1,5),-5); }
    [TestMethod] public void ArithmeticSeriesNegativeDifferenceIsExact()
    { Exact(S(10,-3,4),22); }
    [TestMethod] public void ArithmeticSeriesConstantIsExact()
    { Exact(S(3,0,4),12); }
    [TestMethod] public void ArithmeticSeriesZeroCountReturnsZero()
    { var r=S(99,5,0);Exact(r,0);Assert.IsNotNull(r.MaximumIntermediateBitsObserved);Assert.AreEqual(0L,r.MaximumIntermediateBitsObserved.Value); }
    [TestMethod] public void ArithmeticSeriesOneCountIsFirst()
    { Exact(S(-7,3,1),-7); }
    [TestMethod] public void ArithmeticSeriesMaxCountMatchesClosedOracle()
    { Exact(S(1,1,4096),8390656); }
    [TestMethod] public void ArithmeticSeriesCancellationToZeroIsStillExact()
    { Exact(S(1000,-2000,2),0); }
    [TestMethod] public void ArithmeticSeriesReportsElevenBitIntermediates()
    { var r=S(1000,-2000,2);Exact(r,0);Assert.IsNotNull(r.MaximumIntermediateBitsObserved);Assert.AreEqual(11L,r.MaximumIntermediateBitsObserved.Value); }
    [TestMethod] public void ArithmeticSeriesCancellationDoesNotHideOversizedIntermediates()
    { Refused(S(1000,-2000,2,8),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void ArithmeticSeriesExplicitElevenBitBoundaryAdmitsCancellation()
    { var r=S(1000,-2000,2,11);Exact(r,0);Assert.IsNotNull(r.MaximumIntermediateBitsObserved);Assert.AreEqual(11L,r.MaximumIntermediateBitsObserved.Value); }
    [TestMethod] public void ArithmeticSeriesIndependentResultBudgetAdmitsCancellation()
    { Exact(S(1000,-2000,2,16,new(128,1,4096)),0); }
    [TestMethod] public void ArithmeticSeriesInputBitsAreStillBounded()
    { Refused(S(BigInteger.One<<129,1,2),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void ArithmeticSeriesInvalidNegativeCountRefuses()
    { Refused(S(1,1,-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void ArithmeticSeriesCountOverHardBoundRefuses()
    { Refused(S(1,1,4097),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void ArithmeticSeriesLogicalStepsAreBounded()
    { Refused(S(1,1,5,4096,new(128,4096,4)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void ArithmeticSeriesFinalResultLimitIsIndependent()
    { Refused(S(100,0,10,4096,new(128,8,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void ArithmeticSeriesInvalidZeroIntermediateLimitRefuses()
    { Refused(S(1,1,5,0),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void ArithmeticSeriesInvalidOverHardIntermediateLimitRefuses()
    { Refused(S(1,1,5,16385),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void ArithmeticSeriesInvalidOperandBudgetRefuses()
    { Refused(S(1,1,5,4096,new(0,4096,4096)),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void ArithmeticSeriesPrecancellationRefuses()
    { using var token=new CancellationTokenSource();token.Cancel();Refused(ExactScientificBoundaryAudit.SumArithmeticSequence(1,1,5,cancellationToken:token.Token),ExactScientificStatus.Cancelled); }
    [TestMethod] public void ArithmeticSeriesResultHasNoSignNormalization()
    { var r=S(-3,1,5);Exact(r,-5);Assert.IsFalse(r.NegativeOperandsNormalized); }
    [TestMethod] public void ArithmeticSeriesAllZeroInputsHaveZeroIntermediateBits()
    { var r=S(0,0,5);Exact(r,0);Assert.IsNotNull(r.MaximumIntermediateBitsObserved);Assert.AreEqual(0L,r.MaximumIntermediateBitsObserved.Value); }

    [TestMethod] public void PriorT1NegativeGcdStillRefuses()
    { var r=BoundedExactScientific.GreatestCommonDivisor(-48,18);Assert.AreEqual(ExactScientificStatus.InvalidInput,r.Status);Assert.IsNull(r.Value); }
    [TestMethod] public void PriorT1PositiveGcdStillSucceeds()
    { var r=BoundedExactScientific.GreatestCommonDivisor(48,18);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.AreEqual(new BigInteger(6),r.Value.Value); }
    [TestMethod] public void PriorT1FiniteSequenceResultIsUnchanged()
    { var r=BoundedExactScientific.SumArithmeticSequence(-3,1,5);Assert.IsTrue(r.Succeeded);Assert.IsNotNull(r.Value);Assert.AreEqual(new BigInteger(-5),r.Value.Value); }
    [TestMethod] public void NoProbabilisticPrimalityApiIsClaimed()
    { var names=typeof(ExactScientificBoundaryAudit).GetMethods(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static).Select(m=>m.Name);Assert.IsFalse(names.Any(n=>n.Contains("Prime",StringComparison.OrdinalIgnoreCase))); }
    [TestMethod] public void AuditAssemblyRemainsIndependentOfAgentsAndCas()
    { var names=typeof(ExactScientificBoundaryAudit).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(names.Any(n=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}.Any(token=>n.Contains(token,StringComparison.OrdinalIgnoreCase)))); }
    [TestMethod] public void RepeatExactAuditsAreDeterministic()
    { Assert.AreEqual(G(-48,18,SignedOperandPolicy.NormalizeMagnitude),G(-48,18,SignedOperandPolicy.NormalizeMagnitude));Assert.AreEqual(S(1000,-2000,2),S(1000,-2000,2)); }
    [TestMethod] public void RefusalNeverLeaksIntermediateObservations()
    { Refused(S(1000,-2000,2,8),ExactScientificStatus.ResultTooLarge);Refused(G(-48,18),ExactScientificStatus.InvalidInput); }
}
