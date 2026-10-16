using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C04T2Tests
{
    private static IrExactScalar N(long numerator,long denominator=1) =>
        new(new ExactRational(numerator,denominator));
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrSymbol Y()=>IrSymbol.Free("y","y");
    private static IrNode Sum()=>IrApply.Create(IrOperation.Add,[X(),N(1)]);
    private static IrNode Equation()
    {
        var x=X();
        return new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,N(1))]));
    }
    private static string Sha(string input)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)))
            .ToLowerInvariant();
    private static string Omml(IrNode original)
    {
        var rendered=ExactOfficeOmmlCodec.Render(original);
        Assert.IsTrue(rendered.IsAdmitted,"Prerequisite OMML refused: "+rendered.Status);
        return rendered.Omml!;
    }
    private static string Markdown(IrNode original,MarkdownMathPlacement placement)
    {
        var rendered=MarkdownUnicodeMathCodec.Render(original,placement);
        Assert.IsTrue(rendered.IsAdmitted,"Prerequisite Markdown refused: "+rendered.Status);
        return rendered.Markdown!;
    }
    private static MathDisplayBoundaryResult OfficeAdmit(IrNode original)
    {
        var xml=Omml(original);
        var result=MathDisplayBoundaryAudit.AuditOmml(xml,original);
        CheckAdmission(result,MathDisplayInputKind.StandaloneOmml,original,xml);
        return result;
    }
    private static MathDisplayBoundaryResult MarkdownAdmit(
        IrNode original,MarkdownMathPlacement placement)
    {
        var text=Markdown(original,placement);
        var result=MathDisplayBoundaryAudit.AuditMarkdown(text,placement,original);
        CheckAdmission(result,MathDisplayInputKind.MarkdownMath,original,text);
        return result;
    }
    private static void CheckAdmission(MathDisplayBoundaryResult result,
        MathDisplayInputKind kind, IrNode original,string source)
    {
        Assert.IsTrue(result.IsAdmitted,"Math display audit refused: "+result.Status+
            " / "+result.LossKind);
        Assert.AreEqual(kind,result.InputKind);
        Assert.AreEqual(MathDisplayBoundaryStatus.Admitted,result.Status);
        Assert.AreEqual(MathDisplayLossKind.None,result.LossKind);
        Assert.AreEqual(MathDisplayBoundaryAudit.Rule,result.RuleId);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(original),
            IrCanonicalJsonCodec.Encode(result.Expression!));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SemanticStructuralSha256);
        Assert.AreEqual(Sha(source),result.SourceSha256);
        Assert.AreEqual(source,result.CanonicalRepresentation);
        Assert.IsTrue(result.StructuralFidelityQualified);
        Assert.IsFalse(result.TypographyInverseQualified);
        Assert.IsFalse(result.RequiresOfficeInstallation);
        Assert.IsFalse(result.WholeDocumentConversionQualified);
        Assert.IsFalse(result.FontFidelityQualified);
        Assert.IsFalse(result.PositioningFidelityQualified);
    }
    private static void CheckRefusal(MathDisplayBoundaryResult result,
        MathDisplayBoundaryStatus status,MathDisplayLossKind loss)
    {
        Assert.AreEqual(status,result.Status);
        Assert.AreEqual(loss,result.LossKind);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.CanonicalRepresentation);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
        Assert.IsFalse(result.StructuralFidelityQualified);
        Assert.IsFalse(result.TypographyInverseQualified);
        Assert.IsFalse(result.RequiresOfficeInstallation);
        Assert.IsFalse(result.WholeDocumentConversionQualified);
        Assert.IsFalse(result.FontFidelityQualified);
        Assert.IsFalse(result.PositioningFidelityQualified);
    }

    [TestMethod]
    public void OriginalHalfRetainsCanonicalStructuralHash()
    {
        var result=OfficeAdmit(N(1,2));
        Assert.AreEqual("1/2",result.CanonicalText);
    }
    [TestMethod]
    public void SignedRationalPreservesOriginalNumeratorAndDenominator()
    {
        var result=OfficeAdmit(N(-2,3));
        Assert.AreEqual(new ExactRational(-2,3),((IrExactScalar)result.Expression!).Value);
    }
    [TestMethod]
    public void IntegerOmmlCannotBecomeApproximateDisplay()
    {
        var result=OfficeAdmit(N(7));
        Assert.AreEqual("7",result.CanonicalText);
    }
    [TestMethod]
    public void OriginalEqualityRestrictionIsNotAutomaticallySolved()
    {
        var result=OfficeAdmit(Equation());
        Assert.AreEqual(IrRelationKind.Equal,
            ((IrRestrictedExpression)result.Expression!).Assumptions.Conditions.Single().Kind);
    }
    [TestMethod]
    public void OrderedAdditionPreservesOperandOrder()
    {
        var result=OfficeAdmit(Sum());
        Assert.AreEqual("add(x,1)",result.CanonicalText);
    }
    [TestMethod]
    public void OriginalYIdentityIsRetainedRatherThanInferredFromGlyph()
    {
        var result=OfficeAdmit(Y());
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(Y()),
            IrCanonicalJsonCodec.Encode(result.Expression!));
    }
    [TestMethod]
    public void BoundedSourceFingerprintIsNotAnInverseProof()
    {
        var result=OfficeAdmit(N(1,2));
        Assert.AreEqual(64,result.SourceSha256!.Length);
        Assert.IsFalse(result.TypographyInverseQualified);
    }
    [TestMethod]
    public void ChangedOriginalRationalCannotAuthorizeOriginalOmml()
    {
        var xml=Omml(N(1,2));
        var refused=MathDisplayBoundaryAudit.AuditOmml(xml,N(2,3));
        CheckRefusal(refused,MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
        Assert.AreEqual(Sha(xml),refused.SourceSha256);
    }
    [TestMethod]
    public void AlteredNumeratorInvalidatesSourceBoundExactness()
    {
        var xml=Omml(N(1,2));
        var changed=xml.Replace(">1</m:t>",">8</m:t>",StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,N(1,2)),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void AlteredDenominatorIsAnExplicitLossRefusal()
    {
        var xml=Omml(N(1,2));
        var changed=xml.Replace(">2</m:t>",">9</m:t>",StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,N(1,2)),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void AlteredEqualsOperatorDoesNotInventDifferentEquation()
    {
        var xml=Omml(Equation());
        var changed=xml.Replace(">=</m:t>",">&lt;</m:t>",StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,Equation()),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void WholeWordDocumentIsNotAnOmmlEquation()
    {
        var xml="<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"/>";
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,Equation()),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnsupportedDocumentOrProfile);
    }
    [TestMethod]
    public void OfficeParagraphMathContainerIsNotStandaloneOmml()
    {
        var xml="<m:oMathPara xmlns:m=\""+
            ExactOfficeOmmlCodec.NamespaceUri+"\"/>";
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,Equation()),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnsupportedDocumentOrProfile);
    }
    [TestMethod]
    public void ForeignXmlNamespaceCannotClaimOfficeMathematics()
    {
        var xml="<q:oMath xmlns:q=\"https://example.invalid/omml\"/>";
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnsupportedDocumentOrProfile);
    }
    [TestMethod]
    public void OfficeRunFontStylingIsNotFidelityQualified()
    {
        var xml=Omml(Equation());
        var changed=xml.Replace("<m:r>","<m:r><m:rPr><m:sty m:val=\"b\" /></m:rPr>",
            StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,Equation()),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void OfficeFractionFormattingIsNotReinterpreted()
    {
        var xml=Omml(N(1,2));
        var changed=xml.Replace("<m:num>",
            "<m:fPr><m:type m:val=\"lin\" /></m:fPr><m:num>",StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,N(1,2)),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void OfficePositioningMarkupIsNotQuietlyDropped()
    {
        var xml=Omml(Equation());
        var changed=xml.Replace("<m:t>","<m:pos m:val=\"0\" /><m:t>",
            StringComparison.Ordinal);
        Assert.AreNotEqual(xml,changed);
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(changed,Equation()),
            MathDisplayBoundaryStatus.IntegrityMismatch,
            MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup);
    }
    [TestMethod]
    public void ExternalDtdNeverImportsOrFetches()
    {
        var xml="<!DOCTYPE m:oMath SYSTEM \"https://example.invalid/private.dtd\">"+
            Omml(N(1,2));
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2)),
            MathDisplayBoundaryStatus.InvalidSource,MathDisplayLossKind.InvalidXml);
    }
    [TestMethod]
    public void InternalEntityDeclarationsCannotChangeNumericalContent()
    {
        var xml="<!DOCTYPE m:oMath [<!ENTITY alternate '9'>]>"+Omml(N(1,2));
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2)),
            MathDisplayBoundaryStatus.InvalidSource,MathDisplayLossKind.InvalidXml);
    }
    [TestMethod]
    public void MalformedXmlHasNoRecoverableSemanticSource()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml("<m:oMath",N(1,2)),
            MathDisplayBoundaryStatus.InvalidSource,MathDisplayLossKind.InvalidXml);
    }
    [TestMethod]
    public void LexicalWhitespaceMustRemainCanonical()
    {
        var xml=" "+Omml(N(1,2));
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2)),
            MathDisplayBoundaryStatus.NonCanonical,
            MathDisplayLossKind.NonCanonicalSerialization);
    }
    [TestMethod]
    public void XmlDeclarationIsNotAHiddenCanonicalTransport()
    {
        var xml="<?xml version=\"1.0\"?>"+Omml(N(1,2));
        CheckRefusal(MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2)),
            MathDisplayBoundaryStatus.NonCanonical,
            MathDisplayLossKind.NonCanonicalSerialization);
    }
    [TestMethod]
    public void OversizedOmmlDoesNotAllocateFingerprintOrIr()
    {
        var xml=new string('x',MathDisplayBoundaryAudit.MaximumOmmlCharacters+1);
        var result=MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2));
        CheckRefusal(result,MathDisplayBoundaryStatus.BudgetExceeded,
            MathDisplayLossKind.ResourceLimit);
        Assert.IsNull(result.SourceSha256);
    }
    [TestMethod]
    public void InlineUnicodeAndOriginalIrRemainBound()
    {
        var result=MarkdownAdmit(N(1,2),MarkdownMathPlacement.Inline);
        Assert.AreEqual("$1\u20442$",result.CanonicalRepresentation);
    }
    [TestMethod]
    public void BlockMarkdownIsASeparateExactPlacement()
    {
        var result=MarkdownAdmit(Equation(),MarkdownMathPlacement.Block);
        Assert.AreEqual("$$\nx = 1\n$$",result.CanonicalRepresentation);
    }
    [TestMethod]
    public void BlockMarkdownFractionDoesNotGainAnInverse()
    {
        var result=MarkdownAdmit(N(-2,3),MarkdownMathPlacement.Block);
        Assert.AreEqual("-2/3",result.CanonicalText);
        Assert.IsFalse(result.TypographyInverseQualified);
    }
    [TestMethod]
    public void InlineSourceUsedAsBlockIsAnExplicitMismatch()
    {
        var text=Markdown(N(1,2),MarkdownMathPlacement.Inline);
        var result=MathDisplayBoundaryAudit.AuditMarkdown(
            text,MarkdownMathPlacement.Block,N(1,2));
        CheckRefusal(result,MathDisplayBoundaryStatus.PresentationAmbiguous,
            MathDisplayLossKind.MarkdownPlacementMismatch);
        Assert.AreEqual(Sha(text),result.SourceSha256);
    }
    [TestMethod]
    public void BlockSourceUsedAsInlineIsAlsoAPlacementMismatch()
    {
        var text=Markdown(Equation(),MarkdownMathPlacement.Block);
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                text,MarkdownMathPlacement.Inline,Equation()),
            MathDisplayBoundaryStatus.PresentationAmbiguous,
            MathDisplayLossKind.MarkdownPlacementMismatch);
    }
    [TestMethod]
    public void RemovingMarkdownDelimitersCannotInventMathematicalSource()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "x = 1",MarkdownMathPlacement.Inline,Equation()),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void AsciiSlashDoesNotEqualExactUnicodeFractionGlyph()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "$1/2$",MarkdownMathPlacement.Inline,N(1,2)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void UnicodeMinusVariantCannotBeNormalizedWithoutProof()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "$\u22122\u20443$",MarkdownMathPlacement.Inline,N(-2,3)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void LatexFractionsAreNotThisUnicodeMarkdownGrammar()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "$\\frac{1}{2}$",MarkdownMathPlacement.Inline,N(1,2)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void MarkdownCodeFencesAreNotMathWrappers()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "\u0060\u0060\u0060math\n1/2\n\u0060\u0060\u0060",
                MarkdownMathPlacement.Block,N(1,2)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void MarkdownLinkOrHtmlEnvelopeIsNotMathSemantics()
    {
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "[$x$](https://example.invalid/math)",
                MarkdownMathPlacement.Inline,X()),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void WrongOriginalSymbolIdentityDoesNotAuthorizeDisplay()
    {
        var markup=Markdown(Y(),MarkdownMathPlacement.Inline);
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                markup,MarkdownMathPlacement.Inline,X()),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnqualifiedMarkdownOrUnicode);
    }
    [TestMethod]
    public void UnsupportedOriginalIrHasNoDisplayProof()
    {
        var product=IrApply.Create(IrOperation.Multiply,[X(),N(2)]);
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                "$x \u00d7 2$",MarkdownMathPlacement.Inline,product),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnsupportedOriginalIrOrPlacement);
    }
    [TestMethod]
    public void InvalidMarkdownPlacementIsExplicitlyUnsupported()
    {
        var text=Markdown(N(1,2),MarkdownMathPlacement.Inline);
        CheckRefusal(MathDisplayBoundaryAudit.AuditMarkdown(
                text,(MarkdownMathPlacement)42,N(1,2)),
            MathDisplayBoundaryStatus.Unsupported,
            MathDisplayLossKind.UnsupportedOriginalIrOrPlacement);
    }
    [TestMethod]
    public void OversizedMarkdownHasNoSourceDigestOrMathematicalProof()
    {
        var text=new string('x',MathDisplayBoundaryAudit.MaximumMarkdownCharacters+1);
        var result=MathDisplayBoundaryAudit.AuditMarkdown(
            text,MarkdownMathPlacement.Inline,N(1,2));
        CheckRefusal(result,MathDisplayBoundaryStatus.BudgetExceeded,
            MathDisplayLossKind.ResourceLimit);
        Assert.IsNull(result.SourceSha256);
    }
}
