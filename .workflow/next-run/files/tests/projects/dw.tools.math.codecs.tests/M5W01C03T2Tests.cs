using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C03T2Tests
{
    private static IrExactScalar N(long numerator,long denominator=1)=>
        new(new ExactRational(numerator,denominator));
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrNode Sum()=>
        IrApply.Create(IrOperation.Add,[X(),N(1)]);
    private static IrNode Equality()
    {
        var x=X();
        return new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,N(1))]));
    }
    private static MathMlBoundaryResult Admit(IrNode original)
    {
        var created=ExactMathMlInterop.Render(original);
        Assert.IsTrue(created.IsAdmitted);
        var audited=MathMlBoundaryAudit.Audit(created.AnnotatedMathMl!);
        Assert.AreEqual(MathMlInteropStatus.Admitted,audited.Status);
        Assert.AreEqual(MathMlBoundaryLossKind.None,audited.LossKind);
        Assert.IsTrue(audited.IsAdmitted);
        Assert.IsTrue(audited.StructuralFidelityQualified);
        Assert.IsFalse(audited.PresentationInverseQualified);
        Assert.IsNotNull(audited.Expression);
        Assert.AreEqual(MathMlBoundaryAudit.Rule,audited.RuleId);
        Assert.AreEqual(created.AnnotatedMathMl,audited.AnnotatedMathMl);
        Assert.AreEqual(created.ContentMathMl,audited.ContentMathMl);
        Assert.AreEqual(created.PresentationMathMl,audited.PresentationMathMl);
        Assert.AreEqual(created.CanonicalText,audited.CanonicalText);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(original),
            audited.SemanticStructuralSha256);
        Assert.AreEqual(Sha(created.AnnotatedMathMl!),audited.SourceSha256);
        return audited;
    }
    private static MathMlBoundaryResult Refuse(string xml,
        MathMlInteropStatus status,MathMlBoundaryLossKind loss)
    {
        var result=MathMlBoundaryAudit.Audit(xml);
        Assert.AreEqual(status,result.Status,"Unexpected MathML refusal for "+xml);
        Assert.AreEqual(loss,result.LossKind);
        Assert.IsFalse(result.IsAdmitted);
        Assert.IsFalse(result.StructuralFidelityQualified);
        Assert.IsFalse(result.PresentationInverseQualified);
        Assert.IsNull(result.Expression);
        Assert.IsNull(result.CanonicalText);
        Assert.IsNull(result.AnnotatedMathMl);
        Assert.IsNull(result.ContentMathMl);
        Assert.IsNull(result.PresentationMathMl);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
        if(xml.Length > MathMlBoundaryAudit.MaximumInputCharacters)
            Assert.IsNull(result.SourceSha256);
        else
            Assert.AreEqual(Sha(xml),result.SourceSha256);
        return result;
    }
    private static string Sha(string text)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)))
            .ToLowerInvariant();
    private static string Annotated(IrNode ir)=>
        ExactMathMlInterop.Render(ir).AnnotatedMathMl ??
        throw new InvalidOperationException("Expected qualified source MathML.");

    [TestMethod]
    public void OneHalfPreservesExactRationalAndSourceFingerprint()
    {
        var r=Admit(N(1,2));
        Assert.AreEqual("1/2",r.CanonicalText);
        Assert.AreEqual(new ExactRational(1,2),
            ((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void NegativeTwoThirdsHasExactOriginalSign()
    {
        var r=Admit(N(-2,3));
        Assert.AreEqual("-2/3",r.CanonicalText);
        Assert.IsTrue(r.ContentMathMl!.Contains("-2",StringComparison.Ordinal));
    }

    [TestMethod]
    public void IntegerSevenRetainsCanonicalContentMathMl() =>
        Assert.IsTrue(Admit(N(7)).ContentMathMl!
            .Contains("<cn type=\"integer\">7</cn>",StringComparison.Ordinal));

    [TestMethod]
    public void ExactZeroNeverBecomesImplicitBinary64() =>
        Assert.AreEqual(ExactRational.Zero,
            ((IrExactScalar)Admit(N(0)).Expression!).Value);

    [TestMethod]
    public void SourceMathMlCarriesTypedIntegerFractionOperands()
    {
        var r=Admit(N(1,2));
        XNamespace m=ExactMathMlInterop.Namespace;
        var content=XDocument.Parse(r.ContentMathMl!);
        var values=content.Descendants(m+"cn").ToArray();
        Assert.AreEqual(2,values.Length);
        CollectionAssert.AreEqual(new[]{"1","2"},values.Select(x=>x.Value).ToArray());
        Assert.IsTrue(values.All(x=>(string?)x.Attribute("type")=="integer"));
    }

    [TestMethod]
    public void AnnotatedContentAndPresentationAreBothRequired()
    {
        var r=Admit(N(1,2));
        XNamespace m=ExactMathMlInterop.Namespace;
        var root=XDocument.Parse(r.AnnotatedMathMl!).Root!;
        Assert.AreEqual(1,root.Descendants(m+"annotation").Count());
        Assert.AreEqual(1,root.Descendants(m+"annotation-xml").Count());
    }

    [TestMethod]
    public void XRemainsFreeRealExactSourceIdentity() =>
        Assert.AreEqual("free:x",((IrSymbol)Admit(X()).Expression!).Identity);

    [TestMethod]
    public void YRetainsIndependentOriginalRealIdentity() =>
        Assert.AreEqual("free:y",((IrSymbol)Admit(IrSymbol.Free("y","y")).Expression!).Identity);

    [TestMethod]
    public void ZRetainsIndependentOriginalRealIdentity() =>
        Assert.AreEqual("free:z",((IrSymbol)Admit(IrSymbol.Free("z","z")).Expression!).Identity);

    [TestMethod]
    public void SimpleAdditionKeepsOrderedExactIr()
    {
        var r=Admit(Sum());
        Assert.AreEqual("add(x,1)",r.CanonicalText);
        Assert.AreEqual(IrOperation.Add,((IrApply)r.Expression!).Operation);
    }

    [TestMethod]
    public void EqualityRemainsOriginalTypedRestriction()
    {
        var r=Admit(Equality());
        var restriction=(IrRestrictedExpression)r.Expression!;
        Assert.AreEqual(IrRelationKind.Equal,
            restriction.Assumptions.Conditions.Single().Kind);
        Assert.AreEqual("where(x,eq(x,1))",r.CanonicalText);
    }

    [TestMethod]
    public void EqualityContentAndPresentationMustAgree()
    {
        var r=Admit(Equality());
        Assert.IsTrue(r.ContentMathMl!.Contains("<eq",StringComparison.Ordinal));
        Assert.IsTrue(r.PresentationMathMl!.Contains("<mo>=</mo>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void AnnotatedMathMlKeepsStandardNamespaceAndTypedContent()
    {
        var r=Admit(X());
        XNamespace m=ExactMathMlInterop.Namespace;
        var root=XDocument.Parse(r.AnnotatedMathMl!).Root!;
        Assert.AreEqual(m,root.Name.Namespace);
        var identifier=root.Descendants(m+"ci").Single();
        Assert.AreEqual("real",(string?)identifier.Attribute("type"));
    }

    [TestMethod]
    public void CanonicalIrJsonTransportRetainsOriginalStructuralHash()
    {
        var r=Admit(Equality());
        var serialized=IrCanonicalJsonCodec.Encode(r.Expression!);
        var reloaded=IrCanonicalJsonCodec.Decode(serialized);
        Assert.AreEqual(serialized,IrCanonicalJsonCodec.Encode(reloaded));
        Assert.AreEqual(r.SemanticStructuralSha256,
            IrCanonicalHashes.SemanticStructuralSha256(reloaded));
    }

    [TestMethod]
    public void RuntimeRuleIdIsQualifiedWithoutConstantTautology()
    {
        var r=Admit(X());
        Assert.AreEqual(MathMlBoundaryAudit.Rule,r.RuleId);
        Assert.AreEqual("x",r.CanonicalText);
    }

    [TestMethod]
    public void SameBoundedSourceBytesHaveStableFingerprint()
    {
        var source=Annotated(N(1,2));
        var first=MathMlBoundaryAudit.Audit(source);
        var second=MathMlBoundaryAudit.Audit(source);
        Assert.AreEqual(first.SourceSha256,second.SourceSha256);
        Assert.AreEqual(64,first.SourceSha256!.Length);
    }

    [TestMethod]
    public void PresentationOnlyFractionNeedsAnOriginalAnnotation()
    {
        var presentation=ExactMathMlInterop.Render(N(1,2)).PresentationMathMl!;
        Refuse(presentation,MathMlInteropStatus.PresentationAmbiguous,
            MathMlBoundaryLossKind.PresentationWithoutAnnotation);
    }

    [TestMethod]
    public void PresentationOnlyIdentifierCannotForgeFreeRealIdentity()
    {
        var presentation=ExactMathMlInterop.Render(X()).PresentationMathMl!;
        Refuse(presentation,MathMlInteropStatus.PresentationAmbiguous,
            MathMlBoundaryLossKind.PresentationWithoutAnnotation);
    }

    [TestMethod]
    public void StandaloneContentXmlIsNotOurAnnotatedContract()
    {
        var content=ExactMathMlInterop.Render(N(1,2)).ContentMathMl!;
        Refuse(content,MathMlInteropStatus.PresentationAmbiguous,
            MathMlBoundaryLossKind.PresentationWithoutAnnotation);
    }

    [TestMethod]
    public void NonstandardXmlNamespaceHasNoQualifiedMathMeaning()
    {
        const string xml="<math xmlns=\"https://example.invalid/math\"><mi>x</mi></math>";
        Refuse(xml,MathMlInteropStatus.Unsupported,
            MathMlBoundaryLossKind.UnsupportedProfile);
    }

    [TestMethod]
    public void UnknownSourceAnnotationEncodingIsRefused()
    {
        var xml=Annotated(N(1,2)).Replace(ExactMathMlInterop.TextEncoding,
            "application/x-unknown",StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.Unsupported,
            MathMlBoundaryLossKind.UnsupportedProfile);
    }

    [TestMethod]
    public void UnknownContentAnnotationEncodingIsRefused()
    {
        var xml=Annotated(N(1,2)).Replace(ExactMathMlInterop.ContentEncoding,
            "application/x-unknown",StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.Unsupported,
            MathMlBoundaryLossKind.UnsupportedProfile);
    }

    [TestMethod]
    public void RemovingContentAnnotationCannotCertifyItsPresentation()
    {
        var root=XDocument.Parse(Annotated(N(1,2))).Root!;
        XNamespace m=ExactMathMlInterop.Namespace;
        root.Descendants(m+"annotation-xml").Single().Remove();
        Refuse(root.ToString(SaveOptions.DisableFormatting),
            MathMlInteropStatus.Unsupported,
            MathMlBoundaryLossKind.UnsupportedProfile);
    }

    [TestMethod]
    public void WrongRationalInContentBranchIsAnIntegrityMismatch()
    {
        var xml=Annotated(N(1,2)).Replace(
            "<cn type=\"integer\">1</cn>",
            "<cn type=\"integer\">9</cn>",StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.IntegrityMismatch,
            MathMlBoundaryLossKind.AlteredSemanticBinding);
    }

    [TestMethod]
    public void ChangedPresentationCannotAlterOriginalContent()
    {
        var xml=Annotated(N(1,2)).Replace("<mn>1</mn>","<mn>9</mn>",
            StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.IntegrityMismatch,
            MathMlBoundaryLossKind.AlteredSemanticBinding);
    }

    [TestMethod]
    public void ChangedCanonicalSourceCannotReplaceExactIr()
    {
        var xml=Annotated(N(1,2)).Replace(">1/2</annotation>",
            ">x</annotation>",StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.IntegrityMismatch,
            MathMlBoundaryLossKind.AlteredSemanticBinding);
    }

    [TestMethod]
    public void UnreducedSourceAnnotationIsExplicitlyNoncanonical()
    {
        var xml=Annotated(N(1,2)).Replace(">1/2</annotation>",
            ">2/4</annotation>",StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.NonCanonical,
            MathMlBoundaryLossKind.NonCanonicalSerialization);
    }

    [TestMethod]
    public void XmlDeclarationIsNotCanonicalMathInterchange()
    {
        var xml="<?xml version=\"1.0\"?>"+Annotated(N(1,2));
        Refuse(xml,MathMlInteropStatus.NonCanonical,
            MathMlBoundaryLossKind.NonCanonicalSerialization);
    }

    [TestMethod]
    public void ExtraOuterWhitespaceRequiresCanonicalLexicalSerialization()
    {
        var xml=" "+Annotated(X());
        Refuse(xml,MathMlInteropStatus.NonCanonical,
            MathMlBoundaryLossKind.NonCanonicalSerialization);
    }

    [TestMethod]
    public void ExternalDtdIsNeverResolvedOrFetched()
    {
        var xml="<!DOCTYPE math SYSTEM \"https://example.invalid/foo.dtd\">"+
            Annotated(X());
        Refuse(xml,MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
    }

    [TestMethod]
    public void InternalXmlEntityDeclarationIsRefused()
    {
        var xml="<!DOCTYPE math [<!ENTITY x \"private\">]>"+Annotated(X());
        Refuse(xml,MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
    }

    [TestMethod]
    public void UndefinedXmlEntityIsInvalidMarkup()
    {
        var xml=Annotated(X()).Replace("<mi>x</mi>","<mi>&unknown;</mi>",
            StringComparison.Ordinal);
        Refuse(xml,MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
    }

    [TestMethod]
    public void EmptyInputHasNoMathematicalProofOrResult() =>
        Refuse("",MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);

    [TestMethod]
    public void UnclosedXmlIsNotRepairedOrExecuted() =>
        Refuse("<math",MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);

    [TestMethod]
    public void TruncatedAnnotationXmlIsInvalidRatherThanGuessed()
    {
        var xml=Annotated(N(1,2));
        Refuse(xml[..^6],MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
    }

    [TestMethod]
    public void OverlengthInputIsRejectedWithoutFingerprint()
    {
        var xml=new string('x',MathMlBoundaryAudit.MaximumInputCharacters+1);
        var r=Refuse(xml,MathMlInteropStatus.BudgetExceeded,
            MathMlBoundaryLossKind.ResourceLimit);
        Assert.IsNull(r.SourceSha256);
    }

    [TestMethod]
    public void TooManyXmlNodesAreExplicitlyBudgetRefused()
    {
        var xml="<math xmlns=\""+ExactMathMlInterop.Namespace+"\">"+
            string.Concat(Enumerable.Repeat("<mi>x</mi>",40))+"</math>";
        Refuse(xml,MathMlInteropStatus.BudgetExceeded,
            MathMlBoundaryLossKind.ResourceLimit);
    }

    [TestMethod]
    public void TooDeepXmlNestingIsExplicitlyBudgetRefused()
    {
        var xml="<math xmlns=\""+ExactMathMlInterop.Namespace+"\">"+
            string.Concat(Enumerable.Repeat("<mrow>",14))+"<mi>x</mi>"+
            string.Concat(Enumerable.Repeat("</mrow>",14))+"</math>";
        Refuse(xml,MathMlInteropStatus.BudgetExceeded,
            MathMlBoundaryLossKind.ResourceLimit);
    }

    [TestMethod]
    public void SourceShaOnRefusalIsNotSemanticStructuralProof()
    {
        var result=Refuse(
            ExactMathMlInterop.Render(X()).PresentationMathMl!,
            MathMlInteropStatus.PresentationAmbiguous,
            MathMlBoundaryLossKind.PresentationWithoutAnnotation);
        Assert.IsNotNull(result.SourceSha256);
        Assert.IsNull(result.SemanticStructuralSha256);
        Assert.IsNull(result.RuleId);
    }

    [TestMethod]
    public void DifferentBoundedMarkupFingerprintsHaveDifferentHashes()
    {
        var first=Refuse("<math",MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
        var second=Refuse("<math ",MathMlInteropStatus.InvalidXml,
            MathMlBoundaryLossKind.InvalidXml);
        Assert.AreNotEqual(first.SourceSha256,second.SourceSha256);
    }

    [TestMethod]
    public void RefusedSourceNeverQualifiesInversePresentation()
    {
        var r=Refuse(ExactMathMlInterop.Render(X()).PresentationMathMl!,
            MathMlInteropStatus.PresentationAmbiguous,
            MathMlBoundaryLossKind.PresentationWithoutAnnotation);
        Assert.IsFalse(r.PresentationInverseQualified);
        Assert.IsFalse(r.StructuralFidelityQualified);
    }

    [TestMethod]
    public void TheProfileRemainsExactIrRatherThanGeneralXml()
    {
        var r=Admit(Sum());
        Assert.AreEqual(MathMlBoundaryAudit.Rule,r.RuleId);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(Sum()),
            r.SemanticStructuralSha256);
    }
}
