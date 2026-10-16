using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C02T2Tests
{
    private static LatexImportBoundaryResult Pass(
        string input,LatexImportLossKind expected,
        LatexJuxtapositionConvention convention=
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        var result=LatexImportBoundaryAudit.Audit(input,convention);
        Assert.AreEqual(LatexImportStatus.Admitted,result.Status,
            "Unexpected refusal for input: "+input+"; kind="+result.LossKind);
        Assert.AreEqual(expected,result.LossKind);
        Assert.IsTrue(result.IsAdmitted);
        Assert.IsTrue(result.SemanticFidelityQualified);
        Assert.IsFalse(result.LatexInverseQualified);
        Assert.IsNotNull(result.Expression);
        Assert.IsNotNull(result.CanonicalText);
        Assert.IsNotNull(result.SemanticStructuralSha256);
        Assert.AreEqual(LatexImportBoundaryAudit.Rule,result.RuleId);
        Assert.IsNotNull(result.AppliedConvention);
        Assert.AreEqual(convention,result.AppliedConvention.Value);
        Assert.AreEqual(Sha(input),result.SourceSha256);
        Assert.AreEqual(
            IrCanonicalHashes.SemanticStructuralSha256(result.Expression!),
            result.SemanticStructuralSha256);
        var transported=IrCanonicalJsonCodec.Decode(
            IrCanonicalJsonCodec.Encode(result.Expression!));
        Assert.AreEqual(result.SemanticStructuralSha256,
            IrCanonicalHashes.SemanticStructuralSha256(transported));
        return result;
    }

    private static LatexImportBoundaryResult Refuse(
        string input,LatexImportStatus status,LatexImportLossKind loss,
        LatexJuxtapositionConvention convention=
            LatexJuxtapositionConvention.RequireExplicitOperator)
    {
        var result=LatexImportBoundaryAudit.Audit(input,convention);
        Assert.AreEqual(status,result.Status,"Status for "+input);
        Assert.AreEqual(loss,result.LossKind,"Loss type for "+input);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsFalse(result.SemanticFidelityQualified);
        Assert.IsFalse(result.LatexInverseQualified);
        Assert.IsFalse(result.UsedExplicitMultiplicationConvention);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
        Assert.IsNull(result.AppliedConvention);
        if(input.Length>LatexImportBoundaryAudit.MaximumInputCharacters)
            Assert.IsNull(result.SourceSha256);
        else
            Assert.AreEqual(Sha(input),result.SourceSha256);
        return result;
    }

    private static string Sha(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)))
            .ToLowerInvariant();

    [TestMethod]
    public void StandaloneXHasNoRepresentationLoss() =>
        Assert.AreEqual("x",Pass("x",LatexImportLossKind.None).CanonicalText);

    [TestMethod]
    public void StandaloneYRetainsFreeSymbolIdentity() =>
        Assert.AreEqual("free:y",
            ((IrSymbol)Pass("y",LatexImportLossKind.None).Expression!).Identity);

    [TestMethod]
    public void StandaloneZRemainsAnExactRealVariable() =>
        Assert.AreEqual("free:z",
            ((IrSymbol)Pass("z",LatexImportLossKind.None).Expression!).Identity);

    [TestMethod]
    public void ZeroNeedsNoPresentationNormalization() =>
        Assert.AreEqual("0",Pass("0",LatexImportLossKind.None).CanonicalText);

    [TestMethod]
    public void SignedIntegerIsExactWithoutAnyNormalization() =>
        Assert.AreEqual("-7",Pass("-7",LatexImportLossKind.None).CanonicalText);

    [TestMethod]
    public void LatexFractionReformatsWithoutLosingExactOneHalf()
    {
        var r=Pass(@"\frac{1}{2}",LatexImportLossKind.PassiveNotationChange);
        Assert.AreEqual("1/2",r.CanonicalText);
        Assert.AreEqual(new ExactRational(1,2),
            ((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void NegativeFractionKeepsItsExactSignAndDenominator() =>
        Assert.AreEqual("-2/3",
            Pass(@"-\frac{2}{3}",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void RadicalRemainsUnevaluatedAfterPresentationNormalization() =>
        Assert.AreEqual("sqrt(x)",
            Pass(@"\sqrt{x}",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void AbsoluteBarsHaveQualifiedStructuralFunctionMeaning() =>
        Assert.AreEqual("abs(x)",
            Pass(@"\left|x\right|",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void ExplicitMultiplicationPreservesOrderedOperands() =>
        Assert.AreEqual("mul(x,y)",
            Pass(@"x\cdot y",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void ExactFractionTimesXPreservesTheCoefficient() =>
        Assert.AreEqual("mul(1/2,x)",
            Pass(@"\frac{1}{2}\cdot x",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void AdditionIsNotNumericallySimplified() =>
        Assert.AreEqual("add(x,1)",
            Pass("x+1",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void SubtractionFromHalfIsExactlyStructural() =>
        Assert.AreEqual("sub(x,1/2)",
            Pass(@"x-\frac{1}{2}",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void RendererParenthesesRemainPassiveGrouping() =>
        Assert.AreEqual("add(x,1)",
            Pass(@"\left(x+1\right)",
                LatexImportLossKind.PassiveNotationChange).CanonicalText);

    [TestMethod]
    public void PlainGroupedAdditionDoesNotInventFunctionApplication() =>
        Assert.AreEqual("add(x,y)",
            Pass("(x+y)",LatexImportLossKind.PassiveNotationChange)
                .CanonicalText);

    [TestMethod]
    public void JuxtaposedXYWithDeclaredConventionCarriesEvidence()
    {
        var r=Pass("x(y)",LatexImportLossKind.ExplicitMultiplicationConvention,
            LatexJuxtapositionConvention.Multiplication);
        Assert.AreEqual("mul(x,y)",r.CanonicalText);
        Assert.IsTrue(r.UsedExplicitMultiplicationConvention);
    }

    [TestMethod]
    public void JuxtaposedXOneWithDeclaredConventionKeepsTheChoice() =>
        Assert.IsTrue(
            Pass("x(1)",LatexImportLossKind.ExplicitMultiplicationConvention,
                LatexJuxtapositionConvention.Multiplication)
            .UsedExplicitMultiplicationConvention);

    [TestMethod]
    public void ExplicitConventionDoesNotClaimItWasUsedForFractions()
    {
        var r=Pass(@"\frac{1}{2}",
            LatexImportLossKind.PassiveNotationChange,
            LatexJuxtapositionConvention.Multiplication);
        Assert.IsFalse(r.UsedExplicitMultiplicationConvention);
        Assert.AreEqual(LatexJuxtapositionConvention.Multiplication,
            r.AppliedConvention!.Value);
    }

    [TestMethod]
    public void ExactSourceBytesHaveStableShaRegardlessOfInterpretation()
    {
        const string source="x(y)";
        var refused=Refuse(source,LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);
        var accepted=Pass(source,
            LatexImportLossKind.ExplicitMultiplicationConvention,
            LatexJuxtapositionConvention.Multiplication);
        Assert.AreEqual(refused.SourceSha256,accepted.SourceSha256);
        Assert.AreEqual(64,accepted.SourceSha256!.Length);
    }

    [TestMethod]
    public void DefaultFunctionVersusMultiplicationRemainsAmbiguous() =>
        Refuse("x(y)",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);

    [TestMethod]
    public void ReversedFunctionNotationIsStillAmbiguous() =>
        Refuse("y(x)",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);

    [TestMethod]
    public void AdjacentSymbolsWithoutDelimiterHaveNoPresumedMeaning() =>
        Refuse("xy",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);

    [TestMethod]
    public void T1SquareAndPowerTwoDisplayCannotRestoreOneConstructor() =>
        Refuse(@"\left(x\right)^{2}",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);

    [TestMethod]
    public void BareExponentTwoDoesNotChooseSquareOrPower() =>
        Refuse("x^{2}",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);

    [TestMethod]
    public void PowerThreeRemainsOutsideTheClosedImporterGrammar() =>
        Refuse(@"\left(x\right)^{3}",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void ArbitraryInputMacroIsUnsupportedNeverExecuted() =>
        Refuse(@"\input{private}",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void Write18MacroCannotBecomeACodeExecutionPath() =>
        Refuse(@"\write18{private}",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void UndelimitedCdotControlWordIsNotMultiplied() =>
        Refuse(@"x\cdoty",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void UnknownTrigMacroIsNotAnInventedMathFunction() =>
        Refuse(@"\sin(x)",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void UnknownGenericFunctionIsNotAdmitted() =>
        Refuse("f(x)",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape);

    [TestMethod]
    public void InvalidCallerConventionIsNotAValidAmbiguityPolicy() =>
        Refuse(@"x\cdot y",LatexImportStatus.Unsupported,
            LatexImportLossKind.UnsupportedShape,
            (LatexJuxtapositionConvention)999);

    [TestMethod]
    public void UnreducedExactFractionIsNotSilentlyCanonicalized() =>
        Refuse(@"\frac{2}{4}",LatexImportStatus.NonCanonical,
            LatexImportLossKind.NonCanonicalRepresentation);

    [TestMethod]
    public void UnreducedKelvinNumericSourceCannotLoseProvenance() =>
        Refuse(@"\frac{27315}{100}",LatexImportStatus.NonCanonical,
            LatexImportLossKind.NonCanonicalRepresentation);

    [TestMethod]
    public void LeadingZeroNumeralsRequireExplicitRejection() =>
        Refuse(@"\frac{01}{2}",LatexImportStatus.NonCanonical,
            LatexImportLossKind.NonCanonicalRepresentation);

    [TestMethod]
    public void UndefinedZeroDenominatorNeverProducesMathematicalIR() =>
        Refuse(@"\frac{1}{0}",LatexImportStatus.Invalid,
            LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void MissingFractionDenominatorIsInvalid() =>
        Refuse(@"\frac{1}",LatexImportStatus.Invalid,
            LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void MissingSqrtBraceIsInvalid() =>
        Refuse(@"\sqrt{x",LatexImportStatus.Invalid,
            LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void MissingMultiplicationOperandIsInvalid() =>
        Refuse(@"x\cdot",LatexImportStatus.Invalid,
            LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void EmptySourceIsInvalidButItsShortSourceHashIsRetained() =>
        Refuse("",LatexImportStatus.Invalid,LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void UnicodeInputDoesNotEnterTheAsciiTeXImportGrammar() =>
        Refuse("π",LatexImportStatus.Invalid,LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void RawCanonicalFractionTextDoesNotMasqueradeAsLatex() =>
        Refuse("1/2",LatexImportStatus.Invalid,LatexImportLossKind.InvalidSyntax);

    [TestMethod]
    public void OverMaximumTextBudgetDoesNotAllocateAHashBuffer() =>
        Refuse(new string('x',LatexImportBoundaryAudit.MaximumInputCharacters+1),
            LatexImportStatus.BudgetExceeded,LatexImportLossKind.ResourceLimit);

    [TestMethod]
    public void FourDeepRadicalInputIsExplicitlyBudgetRefused() =>
        Refuse(@"\sqrt{\sqrt{\sqrt{\sqrt{x}}}}",
            LatexImportStatus.BudgetExceeded,LatexImportLossKind.ResourceLimit);

    [TestMethod]
    public void VeryLongExactNumeralHasResourceLimitStatus() =>
        Refuse((BigInteger.One<<140).ToString(CultureInfo.InvariantCulture),
            LatexImportStatus.BudgetExceeded,LatexImportLossKind.ResourceLimit);

    [TestMethod]
    public void SourceHashOnRefusalIsNotAMathematicalProof()
    {
        var result=Refuse("x(y)",LatexImportStatus.Ambiguous,
            LatexImportLossKind.AmbiguousInterpretation);
        Assert.IsNotNull(result.SourceSha256);
        Assert.IsNull(result.RuleId);
        Assert.IsFalse(result.SemanticFidelityQualified);
    }

    [TestMethod]
    public void AuditRuleDoesNotQualifyInverseLatexOrTeX()
    {
        var result=Pass("x",LatexImportLossKind.None);
        Assert.AreEqual("math-latex-import-boundary-loss/v1",result.RuleId);
        Assert.IsFalse(result.LatexInverseQualified);
    }
}
