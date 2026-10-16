using System.Xml.Linq;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C04T1Tests
{
    private static IrExactScalar N(long numerator,long denominator=1) =>
        new(new ExactRational(numerator,denominator));
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrSymbol Y()=>IrSymbol.Free("y","y");
    private static IrNode Addition()=>IrApply.Create(IrOperation.Add,[X(),N(1)]);
    private static IrNode Equation()
    {
        var x=X();
        return new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,N(1))]));
    }
    private static OfficeOmmlResult Admit(IrNode original)
    {
        var displayed=ExactOfficeOmmlCodec.Render(original);
        Assert.IsTrue(displayed.IsAdmitted,"OMML render refused: "+displayed.Status);
        Assert.AreEqual(OfficeOmmlStatus.Admitted,displayed.Status);
        Assert.AreEqual(ExactOfficeOmmlCodec.Rule,displayed.RuleId);
        var decoded=ExactOfficeOmmlCodec.ImportCanonical(displayed.Omml!,original);
        Assert.IsTrue(decoded.IsAdmitted,"OMML import refused: "+decoded.Status);
        Assert.AreEqual(displayed.Omml,decoded.Omml);
        Assert.AreEqual(displayed.CanonicalText,decoded.CanonicalText);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(original),
            IrCanonicalJsonCodec.Encode(decoded.Expression!));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            decoded.SemanticStructuralSha256);
        return decoded;
    }
    private static void Refuse(OfficeOmmlResult result,OfficeOmmlStatus status)
    {
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.Omml);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
    }
    private static MarkdownUnicodeMathResult Display(
        IrNode original,MarkdownMathPlacement placement)
    {
        var result=MarkdownUnicodeMathCodec.Render(original,placement);
        Assert.IsTrue(result.IsAdmitted,"Markdown render refused: "+result.Status);
        Assert.AreEqual(MarkdownUnicodeMathStatus.Admitted,result.Status);
        Assert.AreEqual(placement,result.Placement);
        Assert.AreEqual(MarkdownUnicodeMathCodec.Rule,result.RuleId);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(original),
            IrCanonicalJsonCodec.Encode(result.Expression!));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            result.SemanticStructuralSha256);
        return result;
    }

    [TestMethod]
    public void FractionHalfPreservesExactOriginalIr()
    {
        var result=Admit(N(1,2));
        Assert.AreEqual("1/2",result.CanonicalText);
        Assert.AreEqual(new ExactRational(1,2),((IrExactScalar)result.Expression!).Value);
    }
    [TestMethod]
    public void FractionEmitsStandardOmmlNumeratorAndDenominator()
    {
        XNamespace m=ExactOfficeOmmlCodec.NamespaceUri;
        var root=XDocument.Parse(Admit(N(1,2)).Omml!).Root!;
        Assert.AreEqual(m+"oMath",root.Name);
        var fraction=root.Elements(m+"f").Single();
        Assert.AreEqual("1",fraction.Element(m+"num")!.Element(m+"r")!.Element(m+"t")!.Value);
        Assert.AreEqual("2",fraction.Element(m+"den")!.Element(m+"r")!.Element(m+"t")!.Value);
    }
    [TestMethod]
    public void NegativeTwoThirdsIsNotConvertedToBinary64()
    {
        var result=Admit(N(-2,3));
        Assert.AreEqual("-2/3",result.CanonicalText);
        Assert.AreEqual(new ExactRational(-2,3),((IrExactScalar)result.Expression!).Value);
    }
    [TestMethod]
    public void IntegerHasNoInventedDenominator()
    {
        XNamespace m=ExactOfficeOmmlCodec.NamespaceUri;
        var root=XDocument.Parse(Admit(N(7)).Omml!).Root!;
        Assert.AreEqual("7",root.Element(m+"r")!.Element(m+"t")!.Value);
        Assert.IsFalse(root.Descendants(m+"f").Any());
    }
    [TestMethod]
    public void OriginalEqualityIsPreservedNotSolved()
    {
        var result=Admit(Equation());
        Assert.AreEqual("where(x,eq(x,1))",result.CanonicalText);
        var restricted=(IrRestrictedExpression)result.Expression!;
        Assert.AreEqual(IrRelationKind.Equal,restricted.Assumptions.Conditions.Single().Kind);
    }
    [TestMethod]
    public void EqualityContainsIndependentOmmlEqualsRun()
    {
        XNamespace m=ExactOfficeOmmlCodec.NamespaceUri;
        var root=XDocument.Parse(Admit(Equation()).Omml!).Root!;
        CollectionAssert.AreEqual(new[]{"x","=","1"},
            root.Elements(m+"r").Select(run=>run.Element(m+"t")!.Value).ToArray());
    }
    [TestMethod]
    public void OriginalIrIdentityControlsOmmlImport()
    {
        var source=ExactOfficeOmmlCodec.Render(Equation());
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(source.Omml!,N(1)),
            OfficeOmmlStatus.IntegrityMismatch);
    }
    [TestMethod]
    public void OriginalRationalValueControlsImport()
    {
        var source=ExactOfficeOmmlCodec.Render(N(1,2));
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(source.Omml!,N(2,3)),
            OfficeOmmlStatus.IntegrityMismatch);
    }
    [TestMethod]
    public void OriginalRealSymbolRetainsTypedIdentity()
    {
        var result=Admit(Y());
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(Y()),
            IrCanonicalJsonCodec.Encode(result.Expression!));
    }
    [TestMethod]
    public void OrderedAdditionKeepsItsOriginalMathIr()
    {
        var result=Admit(Addition());
        Assert.AreEqual("add(x,1)",result.CanonicalText);
    }
    [TestMethod]
    public void ForeignDocumentRootIsNotAnOmmlEquation()
    {
        var xml="<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"/>";
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(xml,Equation()),
            OfficeOmmlStatus.UnsupportedProfile);
    }
    [TestMethod]
    public void AlteredDenominatorCannotOverwriteOriginalFraction()
    {
        var original=ExactOfficeOmmlCodec.Render(N(1,2)).Omml!;
        var changed=original.Replace(">2</m:t>",">9</m:t>",StringComparison.Ordinal);
        Assert.AreNotEqual(original,changed);
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(changed,N(1,2)),
            OfficeOmmlStatus.IntegrityMismatch);
    }
    [TestMethod]
    public void UnknownOmmlOperatorIsRefused()
    {
        var original=ExactOfficeOmmlCodec.Render(Equation()).Omml!;
        var changed=original.Replace(">=</m:t>",">&lt;</m:t>",StringComparison.Ordinal);
        Assert.AreNotEqual(original,changed);
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(changed,Equation()),
            OfficeOmmlStatus.IntegrityMismatch);
    }
    [TestMethod]
    public void UnclosedXmlCannotBecomeMath()
    {
        Refuse(ExactOfficeOmmlCodec.ImportCanonical("<m:oMath",N(1,2)),
            OfficeOmmlStatus.InvalidXml);
    }
    [TestMethod]
    public void DtdEntityCannotBeLoaded()
    {
        var xml="<!DOCTYPE m:oMath [<!ENTITY attack '1'>]>"+
            ExactOfficeOmmlCodec.Render(N(1,2)).Omml;
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(xml,N(1,2)),
            OfficeOmmlStatus.InvalidXml);
    }
    [TestMethod]
    public void EmptyOmmlHasNoTypedMathematicalMeaning()
    {
        Refuse(ExactOfficeOmmlCodec.ImportCanonical("",Equation()),
            OfficeOmmlStatus.InvalidXml);
    }
    [TestMethod]
    public void OverlengthOmmlIsBudgetRefused()
    {
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(
            new string('x',ExactOfficeOmmlCodec.MaximumXmlCharacters+1),N(1,2)),
            OfficeOmmlStatus.BudgetExceeded);
    }
    [TestMethod]
    public void LexicallyRewrittenOmmlIsNotSilentlyCanonicalized()
    {
        var source=ExactOfficeOmmlCodec.Render(N(1,2)).Omml!;
        var changed=" "+source;
        Refuse(ExactOfficeOmmlCodec.ImportCanonical(changed,N(1,2)),
            OfficeOmmlStatus.NonCanonical);
    }
    [TestMethod]
    public void MarkdownInlineAndBlockAreDistinctContainers()
    {
        var inline=Display(Equation(),MarkdownMathPlacement.Inline);
        var block=Display(Equation(),MarkdownMathPlacement.Block);
        Assert.AreEqual("$x = 1$",inline.Markdown);
        Assert.AreEqual("$$\nx = 1\n$$",block.Markdown);
        Assert.AreNotEqual(inline.Placement,block.Placement);
    }
    [TestMethod]
    public void UnicodeFractionSlashIsDisplayNotIrInference()
    {
        var inline=Display(N(1,2),MarkdownMathPlacement.Inline);
        Assert.AreEqual("1\u20442",inline.UnicodeText);
        Assert.AreEqual("$1\u20442$",inline.Markdown);
        Assert.AreEqual("1/2",inline.CanonicalText);
    }
    [TestMethod]
    public void MarkdownBlockKeepsOriginalExactRational()
    {
        var block=Display(N(-2,3),MarkdownMathPlacement.Block);
        Assert.AreEqual("$$\n-2\u20443\n$$",block.Markdown);
        Assert.AreEqual(new ExactRational(-2,3),((IrExactScalar)block.Expression!).Value);
    }
    [TestMethod]
    public void UnicodeAdditionIsNotAnEvaluatedResult()
    {
        var rendered=Display(Addition(),MarkdownMathPlacement.Inline);
        Assert.AreEqual("x + 1",rendered.UnicodeText);
        Assert.AreEqual("add(x,1)",rendered.CanonicalText);
    }
    [TestMethod]
    public void UnsupportedMathIrNeverGetsAmbiguousOmmlOrMarkdown()
    {
        var product=IrApply.Create(IrOperation.Multiply,[X(),N(2)]);
        var office=ExactOfficeOmmlCodec.Render(product);
        Refuse(office,OfficeOmmlStatus.UnsupportedProfile);
        var markdown=MarkdownUnicodeMathCodec.Render(product,MarkdownMathPlacement.Inline);
        Assert.AreEqual(MarkdownUnicodeMathStatus.Unsupported,markdown.Status);
        Assert.IsNull(markdown.Expression);
        Assert.IsNull(markdown.Markdown);
        Assert.IsNull(markdown.SemanticStructuralSha256);
        Assert.IsNull(markdown.RuleId);
    }
    [TestMethod]
    public void InvalidMarkdownPlacementRefusesWithoutIr()
    {
        var output=MarkdownUnicodeMathCodec.Render(N(1,2),(MarkdownMathPlacement)42);
        Assert.AreEqual(MarkdownUnicodeMathStatus.Unsupported,output.Status);
        Assert.IsNull(output.Expression);
        Assert.IsNull(output.Markdown);
        Assert.IsNull(output.RuleId);
    }
}
