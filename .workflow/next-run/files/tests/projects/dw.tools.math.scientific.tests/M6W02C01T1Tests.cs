using System.Numerics;
using Dw.Tools.Math.Scientific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Scientific.Tests;

[TestClass]
public sealed class M6W02C01T1Tests
{
    private static ExactScientificResult G(BigInteger x,BigInteger y,ExactScientificBudget? b=null) =>
        BoundedExactScientific.GreatestCommonDivisor(x,y,b);
    private static ExactScientificResult L(BigInteger x,BigInteger y,ExactScientificBudget? b=null) =>
        BoundedExactScientific.LeastCommonMultiple(x,y,b);
    private static ExactScientificResult F(int n,ExactScientificBudget? b=null) =>
        BoundedExactScientific.Factorial(n,b);
    private static ExactScientificResult B(int n,int k,ExactScientificBudget? b=null) =>
        BoundedExactScientific.Binomial(n,k,b);
    private static ExactScientificResult S(BigInteger a,BigInteger d,int n,ExactScientificBudget? b=null) =>
        BoundedExactScientific.SumArithmeticSequence(a,d,n,b);
    private static void Equal(ExactScientificResult r,BigInteger expected)
    {
        Assert.IsTrue(r.Succeeded);
        Assert.AreEqual(ExactScientificStatus.Success,r.Status);
        Assert.IsNotNull(r.Value);
        Assert.AreEqual(expected,r.Value.Value);
        Assert.IsTrue(r.Steps>=0);
        Assert.AreEqual(BoundedExactScientific.RuleId,r.RuleId);
    }
    private static void Refused(ExactScientificResult r,ExactScientificStatus expected)
    {
        Assert.AreEqual(expected,r.Status);
        Assert.IsFalse(r.Succeeded);
        Assert.IsNull(r.Value);
        Assert.AreEqual(0,r.Steps);
        Assert.AreEqual(BoundedExactScientific.RuleId,r.RuleId);
    }

    [TestMethod] public void GcdFortyEightAndEighteenIsSix()
    { Equal(G(48,18),6); }
    [TestMethod] public void GcdIsSymmetric()
    { Equal(G(18,48),6); }
    [TestMethod] public void GcdCoprimeNumbersIsOne()
    { Equal(G(35,64),1); }
    [TestMethod] public void GcdEqualNumbersIsNumber()
    { Equal(G(17,17),17); }
    [TestMethod] public void GcdWithZeroPreservesPositiveOperand()
    { Equal(G(91,0),91); }
    [TestMethod] public void GcdZeroWithPositiveIsOperand()
    { Equal(G(0,91),91); }
    [TestMethod] public void GcdOfTwoZeroInputsIsZeroByDeclaredConvention()
    { Equal(G(0,0),0); }
    [TestMethod] public void GcdNoPartialOnNegativeLeft()
    { Refused(G(-48,18),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void GcdNoPartialOnNegativeRight()
    { Refused(G(48,-18),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void GcdBudgetRefusesTooManyEuclidSteps()
    { Refused(G(48,18,new(128,4096,2)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void GcdBudgetAdmitsExactlyThreeEuclidSteps()
    { var r=G(48,18,new(128,4096,3));Equal(r,6);Assert.AreEqual(3,r.Steps); }
    [TestMethod] public void GcdOversizedOperandRefuses()
    { Refused(G(BigInteger.One<<129,1),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void GcdSmallResultBudgetCanRefuseLargeGcd()
    { Refused(G(12,6,new(128,2,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void GcdInvalidZeroBudgetRefuses()
    { Refused(G(6,4,new(0,4096,4096)),ExactScientificStatus.InvalidPolicy); }
    [TestMethod] public void GcdCancelledBeforeEvaluatingRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(BoundedExactScientific.GreatestCommonDivisor(48,18,cancellationToken:c.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void LcmSixAndEightIsTwentyFour()
    { Equal(L(6,8),24); }
    [TestMethod] public void LcmSymmetric()
    { Equal(L(8,6),24); }
    [TestMethod] public void LcmZeroLeftReturnsZero()
    { Equal(L(0,14),0); }
    [TestMethod] public void LcmZeroRightReturnsZero()
    { Equal(L(14,0),0); }
    [TestMethod] public void LcmCoprimeNumbersMultiply()
    { Equal(L(7,15),105); }
    [TestMethod] public void LcmEqualNumbersReturnsNumber()
    { Equal(L(17,17),17); }
    [TestMethod] public void LcmNegativeOperandIsInvalid()
    { Refused(L(-6,8),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void LcmTinyOutputBudgetRefuses()
    { Refused(L(6,8,new(128,4,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void LcmEuclidPlusMultiplicationIsBudgeted()
    { Refused(L(48,18,new(128,4096,3)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void LcmCountIncludesFinalMultiplication()
    { var r=L(48,18,new(128,4096,4));Equal(r,144);Assert.AreEqual(4,r.Steps); }
    [TestMethod] public void LcmCancelledBeforeWorkRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(BoundedExactScientific.LeastCommonMultiple(6,8,cancellationToken:c.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void ZeroFactorialIsExactlyOne()
    { Equal(F(0),1); }
    [TestMethod] public void OneFactorialIsExactlyOne()
    { Equal(F(1),1); }
    [TestMethod] public void FiveFactorialIsOneHundredTwenty()
    { Equal(F(5),120); }
    [TestMethod] public void TenFactorialMatchesIndependentInteger()
    { Equal(F(10),3628800); }
    [TestMethod] public void TwentyFactorialMatchesIndependentBigInteger()
    { Equal(F(20),BigInteger.Parse("2432902008176640000")); }
    [TestMethod] public void FactorialCountsMultiplications()
    { var r=F(5);Equal(r,120);Assert.AreEqual(4,r.Steps); }
    [TestMethod] public void NegativeFactorialIsInvalid()
    { Refused(F(-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void FactorialBeyondHardBoundRefuses()
    { Refused(F(257),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void FactorialStepBudgetRefusesBeforeComputation()
    { Refused(F(5,new(128,4096,3)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void FactorialTooLargeForOutputBudgetRefuses()
    { Refused(F(10,new(128,16,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void FactorialCancelledBeforeWorkRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(BoundedExactScientific.Factorial(5,cancellationToken:c.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void FiveChooseTwoIsTen()
    { Equal(B(5,2),10); }
    [TestMethod] public void FiveChooseZeroIsOne()
    { Equal(B(5,0),1); }
    [TestMethod] public void FiveChooseFiveIsOne()
    { Equal(B(5,5),1); }
    [TestMethod] public void TenChooseThreeIsOneHundredTwenty()
    { Equal(B(10,3),120); }
    [TestMethod] public void FiftyTwoChooseFiveIndependentOracle()
    { Equal(B(52,5),2598960); }
    [TestMethod] public void BinomialSymmetry()
    { Equal(B(10,7),120); }
    [TestMethod] public void PascalIdentityIsExact()
    { var a=B(10,3);var b=B(10,4);var c=B(11,4);Assert.IsTrue(a.Succeeded&&b.Succeeded);Equal(c,a.Value!.Value+b.Value!.Value); }
    [TestMethod] public void BinomialTwentyChooseTenKnownNumber()
    { Equal(B(20,10),184756); }
    [TestMethod] public void BinomialNegativeNIsInvalid()
    { Refused(B(-1,0),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialNegativeKIsInvalid()
    { Refused(B(5,-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialKGreaterThanNIsInvalid()
    { Refused(B(5,6),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void BinomialArgumentOverHardBoundRefuses()
    { Refused(B(257,1),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void BinomialOutputBudgetRefuses()
    { Refused(B(5,2,new(128,3,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void BinomialStepBudgetRefuses()
    { Refused(B(10,3,new(128,4096,2)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void BinomialCancelledBeforeWorkRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(BoundedExactScientific.Binomial(5,2,cancellationToken:c.Token),ExactScientificStatus.Cancelled); }

    [TestMethod] public void ArithmeticSeriesOneThreeFiveSevenNineSumsToTwentyFive()
    { Equal(S(1,2,5),25); }
    [TestMethod] public void ArithmeticSeriesWithZeroCountSumsToZero()
    { Equal(S(99,5,0),0); }
    [TestMethod] public void ArithmeticSeriesConstantElementSumsExactly()
    { Equal(S(3,0,4),12); }
    [TestMethod] public void ArithmeticSeriesNegativeDifferenceIsExact()
    { Equal(S(10,-3,4),22); }
    [TestMethod] public void ArithmeticSeriesNegativeStartIsExact()
    { Equal(S(-3,1,5),-5); }
    [TestMethod] public void ArithmeticSeriesOneItemReturnsStart()
    { Equal(S(-7,3,1),-7); }
    [TestMethod] public void ArithmeticSeriesThousandIntegersMatchesFormula()
    { Equal(S(1,1,1000),500500); }
    [TestMethod] public void ArithmeticSeriesMaximumCountIsFiniteAndExact()
    { Equal(S(1,1,4096),8390656); }
    [TestMethod] public void NegativeSeriesCountIsInvalid()
    { Refused(S(1,2,-1),ExactScientificStatus.InvalidInput); }
    [TestMethod] public void SeriesOverMaxLengthRefuses()
    { Refused(S(1,2,4097),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void SeriesExceedingLogicalStepBudgetRefuses()
    { Refused(S(1,2,5,new(128,4096,4)),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void SeriesInputBitBudgetRefuses()
    { Refused(S(BigInteger.One<<129,1,2),ExactScientificStatus.BudgetExceeded); }
    [TestMethod] public void SeriesOutputBitBudgetRefuses()
    { Refused(S(100,0,10,new(128,8,4096)),ExactScientificStatus.ResultTooLarge); }
    [TestMethod] public void SeriesCancelledBeforeWorkRefuses()
    { using var c=new CancellationTokenSource();c.Cancel();Refused(BoundedExactScientific.SumArithmeticSequence(1,2,5,cancellationToken:c.Token),ExactScientificStatus.Cancelled); }
    [TestMethod] public void ScientificOperationsContainNoForeignProviders()
    { var refs=typeof(BoundedExactScientific).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");Assert.IsFalse(refs.Any(x=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}.Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase)))); }
    [TestMethod] public void RepeatedExactCalculationsAreDeterministic()
    { Assert.AreEqual(G(48,18),G(48,18));Assert.AreEqual(F(10),F(10));Assert.AreEqual(S(1,2,5),S(1,2,5)); }
    [TestMethod] public void DifferentResultKindsShareTypedNonpartialRefusal()
    { Refused(G(-1,2),ExactScientificStatus.InvalidInput);Refused(F(-1),ExactScientificStatus.InvalidInput);Refused(B(-1,0),ExactScientificStatus.InvalidInput);Refused(S(1,1,-1),ExactScientificStatus.InvalidInput); }
}
