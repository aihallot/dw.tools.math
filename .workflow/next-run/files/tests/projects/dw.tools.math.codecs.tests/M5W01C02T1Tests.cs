using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C02T1Tests
{
    private static LatexImportResult Pass(string tex,
        LatexJuxtapositionConvention convention=
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        var result=BoundedLatexImporter.Import(tex,convention);
        Assert.AreEqual(LatexImportStatus.Admitted,result.Status,
            "Expected exact admitted LaTeX subset: "+tex+
            "; actual="+result.Status);
        Assert.IsTrue(result.IsAdmitted);
        Assert.IsNotNull(result.Expression);
        Assert.IsNotNull(result.CanonicalText);
        Assert.IsNotNull(result.SemanticStructuralSha256);
        Assert.AreEqual(BoundedLatexImporter.Rule,result.RuleId);
        Assert.AreEqual(convention,result.AppliedConvention);
        Assert.AreEqual(result.SemanticStructuralSha256,
            IrCanonicalHashes.SemanticStructuralSha256(result.Expression!));
        Assert.AreEqual(result.CanonicalText,
            CanonicalMathNotation.Render(result.Expression!).CanonicalText);
        return result;
    }

    private static void Refuse(string tex,LatexImportStatus status,
        LatexJuxtapositionConvention convention=
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        var result=BoundedLatexImporter.Import(tex,convention);
        Assert.AreEqual(status,result.Status,
            "Unexpected LaTeX refusal status for: "+tex);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.AppliedConvention);
    }

    [TestMethod]
    public void ExactOneHalfFractionImportsWithoutFloatingPoint()
    {
        var result=Pass(@"\frac{1}{2}");
        Assert.AreEqual(new ExactRational(1,2),
            ((IrExactScalar)result.Expression!).Value);
        Assert.AreEqual("1/2",result.CanonicalText);
    }

    [TestMethod]
    public void ExactNegativeFractionUsesRendererStyleLeadingMinus()
    {
        var result=Pass(@"-\frac{2}{3}");
        Assert.AreEqual(new ExactRational(-2,3),
            ((IrExactScalar)result.Expression!).Value);
        Assert.AreEqual("-2/3",result.CanonicalText);
    }

    [TestMethod]
    public void OneThirdIsExactAndRemainsIrExactScalar() =>
        Assert.AreEqual("1/3",Pass(@"\frac{1}{3}").CanonicalText);

    [TestMethod]
    public void ExactSignedIntegerIsStructuralWithoutEvaluation() =>
        Assert.AreEqual("-7",Pass("-7").CanonicalText);

    [TestMethod]
    public void ExactZeroIsAdmittedAsCanonicalInteger() =>
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Pass("0").Expression!).Value);

    [TestMethod]
    public void FreeXRemainsTheKnownRealIrIdentity() =>
        Assert.AreEqual("free:x",((IrSymbol)Pass("x").Expression!).Identity);

    [TestMethod]
    public void FreeYHasIndependentRealIdentity() =>
        Assert.AreEqual("free:y",((IrSymbol)Pass("y").Expression!).Identity);

    [TestMethod]
    public void FreeZRetainsItsRealMathIdentity() =>
        Assert.AreEqual("free:z",((IrSymbol)Pass("z").Expression!).Identity);

    [TestMethod]
    public void SqrtOfFreeXIsFunctionNotScalarEvaluation()
    {
        var result=Pass(@"\sqrt{x}");
        Assert.AreEqual("sqrt(x)",result.CanonicalText);
        Assert.AreEqual(IrOperation.Sqrt,((IrApply)result.Expression!).Operation);
    }

    [TestMethod]
    public void SqrtOfExactTwoRemainsAnUnevaluatedRadical() =>
        Assert.AreEqual("sqrt(2)",Pass(@"\sqrt{2}").CanonicalText);

    [TestMethod]
    public void AbsFromRendererPreservesFunctionApplication()
    {
        var result=Pass(@"\left|x\right|");
        Assert.AreEqual("abs(x)",result.CanonicalText);
        Assert.AreEqual(IrOperation.Abs,((IrApply)result.Expression!).Operation);
    }

    [TestMethod]
    public void ExplicitCdotIsAlwaysMathematicalMultiplication()
    {
        // No whitespace after the command, unlike the T1 display convention.
        var result=Pass(@"x\cdot"+"y");
        Assert.AreEqual("mul(x,y)",result.CanonicalText);
        Assert.AreEqual(IrOperation.Multiply,
            ((IrApply)result.Expression!).Operation);
    }

    [TestMethod]
    public void ExplicitCdotWithSpaceFromT1RendererIsSupported() =>
        Assert.AreEqual("mul(x,y)",Pass(@"x\cdot y").CanonicalText);

    [TestMethod]
    public void ExplicitCdotAfterExactFractionRetainsExactCoefficient() =>
        Assert.AreEqual("mul(1/2,x)",
            Pass(@"\frac{1}{2}\cdot x").CanonicalText);

    [TestMethod]
    public void AdditionPreservesExactOperandOrderNotAnEvaluatedSum() =>
        Assert.AreEqual("add(x,1)",Pass("x+1").CanonicalText);

    [TestMethod]
    public void SubtractionPreservesSymbolAndExactRational() =>
        Assert.AreEqual("sub(x,1/2)",
            Pass(@"x-\frac{1}{2}").CanonicalText);

    [TestMethod]
    public void AdditionOfRationalsRemainsStructuralWithoutSummation()
    {
        var r=Pass(@"\frac{1}{2}+\frac{1}{3}");
        Assert.AreEqual("add(1/2,1/3)",r.CanonicalText);
        Assert.AreEqual(IrOperation.Add,((IrApply)r.Expression!).Operation);
    }

    [TestMethod]
    public void T1LatexParenthesesAreAcceptedOnlyAsGrouping() =>
        Assert.AreEqual("add(x,1)",Pass(@"\left(x+1\right)").CanonicalText);

    [TestMethod]
    public void ExplicitGroupedCdotPreservesProductIdentity() =>
        Assert.AreEqual("mul(x,y)",
            Pass(@"\left(x\cdot y\right)").CanonicalText);

    [TestMethod]
    public void PlainParenthesesDoNotInventFunctionApplication() =>
        Assert.AreEqual("add(x,y)",Pass("(x+y)").CanonicalText);

    [TestMethod]
    public void PlainJuxtapositionIsAmbiguousByDefault() =>
        Refuse("x(y)",LatexImportStatus.Ambiguous);

    [TestMethod]
    public void ASecondVariableAsAppliedArgumentIsAlsoAmbiguous() =>
        Refuse("y(x)",LatexImportStatus.Ambiguous);

    [TestMethod]
    public void ExplicitMultiplicationConventionResolvesXOfY()
    {
        var r=Pass("x(y)",LatexJuxtapositionConvention.Multiplication);
        Assert.AreEqual("mul(x,y)",r.CanonicalText);
        Assert.AreEqual(LatexJuxtapositionConvention.Multiplication,
            r.AppliedConvention);
    }

    [TestMethod]
    public void ExplicitMultiplicationConventionResolvesSymbolTimesOne() =>
        Assert.AreEqual("mul(x,1)",
            Pass("x(1)",LatexJuxtapositionConvention.Multiplication).CanonicalText);

    [TestMethod]
    public void JuxtapositionWithoutParenthesesIsNeverSilentlyGuessed() =>
        Refuse("xy",LatexImportStatus.Ambiguous);

    [TestMethod]
    public void UnknownFunctionNameIsNotImportedAsMultiplication() =>
        Refuse("f(x)",LatexImportStatus.Unsupported,
            LatexJuxtapositionConvention.Multiplication);

    [TestMethod]
    public void T1SquareAndPowerTwoDisplayHasNoStructuralInverse() =>
        Refuse(@"\left(x\right)^{2}",LatexImportStatus.Ambiguous);

    [TestMethod]
    public void BareExponentTwoCannotChooseSquareOrPower() =>
        Refuse("x^{2}",LatexImportStatus.Ambiguous);

    [TestMethod]
    public void PowerThreeNeedsItsOwnQualifiedImporter() =>
        Refuse(@"\left(x\right)^{3}",LatexImportStatus.Unsupported);

    [TestMethod]
    public void ArbitraryTeXInputMacroIsOutsideClosedGrammar() =>
        Refuse(@"\input{file}",LatexImportStatus.Unsupported);

    [TestMethod]
    public void ArbitraryTeXWriteCommandIsNotExecutableOrImported() =>
        Refuse(@"\write18{file}",LatexImportStatus.Unsupported);

    [TestMethod]
    public void UnknownFunctionMacroIsUnsupportedRatherThanGuessed() =>
        Refuse(@"\sin(x)",LatexImportStatus.Unsupported);

    [TestMethod]
    public void UnknownOperatorNameCannotTurnIntoAFunction() =>
        Refuse(@"\operatorname{sin}(x)",LatexImportStatus.Unsupported);

    [TestMethod]
    public void RawCanonicalTextFractionIsNotAutomaticallyLatex() =>
        Refuse("1/2",LatexImportStatus.Invalid);

    [TestMethod]
    public void EmptyInputProducesInvalidWithoutMathematicalProof() =>
        Refuse("",LatexImportStatus.Invalid);

    [TestMethod]
    public void ZeroDenominatorRefusesUndefinedExactValue() =>
        Refuse(@"\frac{1}{0}",LatexImportStatus.Invalid);

    [TestMethod]
    public void MissingFractionDenominatorCannotBeCompleted() =>
        Refuse(@"\frac{1}",LatexImportStatus.Invalid);

    [TestMethod]
    public void MissingSqrtClosingBraceDoesNotCreateFunction() =>
        Refuse(@"\sqrt{x",LatexImportStatus.Invalid);

    [TestMethod]
    public void MissingMultiplicationOperandIsInvalid() =>
        Refuse(@"x\cdot",LatexImportStatus.Invalid);

    [TestMethod]
    public void NonReducedFractionIsNotSilentlyNormalized() =>
        Refuse(@"\frac{2}{4}",LatexImportStatus.NonCanonical);

    [TestMethod]
    public void LeadingZeroFractionIsExplicitlyNoncanonical() =>
        Refuse(@"\frac{01}{2}",LatexImportStatus.NonCanonical);

    [TestMethod]
    public void ZeroFractionMustUseTheAcceptedIntegerLiteral() =>
        Refuse(@"\frac{0}{2}",LatexImportStatus.NonCanonical);

    [TestMethod]
    public void AbsoluteTemperatureUnreducedFractionCannotBeAutoAccepted() =>
        Refuse(@"\frac{27315}{100}",LatexImportStatus.NonCanonical);

    [TestMethod]
    public void ExtremelyLongInputIsBudgetExceeded() =>
        Refuse(new string('x',BoundedLatexImporter.MaximumInputCharacters+1),
            LatexImportStatus.BudgetExceeded);

    [TestMethod]
    public void DeepFourLevelRadicalExceedsConservativeDepth() =>
        Refuse(@"\sqrt{\sqrt{\sqrt{\sqrt{x}}}}",
            LatexImportStatus.BudgetExceeded);

    [TestMethod]
    public void VeryLargeExactNumeratorExceedsBitBudget() =>
        Refuse((BigInteger.One<<140).ToString(
            System.Globalization.CultureInfo.InvariantCulture),
            LatexImportStatus.BudgetExceeded);

    [TestMethod]
    public void UnicodeGreekLetterIsNotAnImplicitVariable() =>
        Refuse("π",LatexImportStatus.Invalid);

    [TestMethod]
    public void PassingInvalidConventionsDoesNotGuessMeaning() =>
        Refuse(@"x\cdot y",LatexImportStatus.Unsupported,
            (LatexJuxtapositionConvention)999);

    [TestMethod]
    public void ImportedExactIrSurvivesIndependentCanonicalJsonTransport()
    {
        var first=Pass(@"\frac{1}{2}\cdot x");
        var json=IrCanonicalJsonCodec.Encode(first.Expression!);
        var again=IrCanonicalJsonCodec.Decode(json);
        Assert.AreEqual(json,IrCanonicalJsonCodec.Encode(again));
        Assert.AreEqual(first.SemanticStructuralSha256,
            IrCanonicalHashes.SemanticStructuralSha256(again));
        Assert.AreEqual(first.CanonicalText,
            CanonicalMathNotation.Render(again).CanonicalText);
    }

    [TestMethod]
    public void NoQualifiedImporterClaimsToExecuteTeX()
    {
        var admitted=Pass(@"\sqrt{x}");
        Assert.IsTrue(admitted.IsAdmitted);
        Assert.AreEqual("math-bounded-latex-exact-subset/v1",
            admitted.RuleId);
        Assert.IsFalse(admitted.AppliedConvention is null);
    }
}
