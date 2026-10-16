using System.Xml.Linq;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Codecs.Tests;

[TestClass]
public sealed class M5W01C03T1Tests
{
    private static IrExactScalar N(long n,long d=1)=>
        new(new ExactRational(n,d));
    private static IrSymbol X()=>IrSymbol.Free("x","x");
    private static IrNode Sum()=>IrApply.Create(IrOperation.Add,[X(),N(1)]);
    private static IrNode Equation()
    {
        var x=X();
        return new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.Equal,N(1))]));
    }
    private static MathMlInteropResult Admit(IrNode input)
    {
        var rendered=ExactMathMlInterop.Render(input);
        Assert.AreEqual(MathMlInteropStatus.Admitted,rendered.Status);
        Assert.IsTrue(rendered.IsAdmitted);
        Assert.AreEqual(ExactMathMlInterop.Rule,rendered.RuleId);
        Assert.IsNotNull(rendered.AnnotatedMathMl);
        Assert.IsNotNull(rendered.ContentMathMl);
        Assert.IsNotNull(rendered.PresentationMathMl);
        Assert.IsNotNull(rendered.CanonicalText);
        Assert.IsNotNull(rendered.Expression);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(input),
            rendered.SemanticStructuralSha256);
        var imported=ExactMathMlInterop.ImportAnnotated(rendered.AnnotatedMathMl!);
        Assert.IsTrue(imported.IsAdmitted,
            "Render -> annotated MathML -> IR import refused: "+imported.Status);
        Assert.AreEqual(rendered.CanonicalText,imported.CanonicalText);
        Assert.AreEqual(rendered.AnnotatedMathMl,imported.AnnotatedMathMl);
        Assert.AreEqual(rendered.SemanticStructuralSha256,
            imported.SemanticStructuralSha256);
        Assert.AreEqual(IrCanonicalJsonCodec.Encode(rendered.Expression!),
            IrCanonicalJsonCodec.Encode(imported.Expression!));
        return rendered;
    }
    private static void Refuse(MathMlInteropResult r,MathMlInteropStatus kind)
    {
        Assert.AreEqual(kind,r.Status);
        Assert.IsFalse(r.IsAdmitted);
        Assert.IsNull(r.Expression);
        Assert.IsNull(r.CanonicalText);
        Assert.IsNull(r.AnnotatedMathMl);
        Assert.IsNull(r.ContentMathMl);
        Assert.IsNull(r.PresentationMathMl);
        Assert.IsNull(r.SemanticStructuralSha256);
        Assert.IsNull(r.RuleId);
    }

    [TestMethod]
    public void ExactHalfPreservesOriginalRationalValue()
    {
        var r=Admit(N(1,2));
        Assert.AreEqual("1/2",r.CanonicalText);
        Assert.AreEqual(new ExactRational(1,2),
            ((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void HalfHasContentDivideAndNumeratorDenominator()
    {
        var r=Admit(N(1,2));
        var content=XDocument.Parse(r.ContentMathMl!);
        XNamespace m=ExactMathMlInterop.Namespace;
        var apply=content.Root!.Element(m+"apply")!;
        Assert.AreEqual("divide",apply.Elements().First().Name.LocalName);
        Assert.AreEqual("1",apply.Elements(m+"cn").First().Value);
        Assert.AreEqual("2",apply.Elements(m+"cn").Last().Value);
    }

    [TestMethod]
    public void HalfHasPresentationMfracWithoutBinary64()
    {
        var r=Admit(N(1,2));
        XNamespace m=ExactMathMlInterop.Namespace;
        Assert.AreEqual("mfrac",XDocument.Parse(r.PresentationMathMl!)
            .Root!.Elements().Single().Name.LocalName);
        Assert.IsTrue(r.AnnotatedMathMl!.Contains("mfrac",StringComparison.Ordinal));
    }

    [TestMethod]
    public void NegativeRationalRetainsExactSign()
    {
        var r=Admit(N(-2,3));
        Assert.AreEqual("-2/3",r.CanonicalText);
        Assert.IsTrue(r.ContentMathMl!.Contains("-2",StringComparison.Ordinal));
    }

    [TestMethod]
    public void IntegerSevenIsContentCnAndPresentationMn()
    {
        var r=Admit(N(7));
        Assert.IsTrue(r.ContentMathMl!.Contains("<cn type=\"integer\">7</cn>",StringComparison.Ordinal));
        Assert.IsTrue(r.PresentationMathMl!.Contains("<mn>7</mn>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void ZeroIsExactCanonicalInteger()
    {
        var r=Admit(N(0));
        Assert.AreEqual("0",r.CanonicalText);
        Assert.AreEqual(ExactRational.Zero,((IrExactScalar)r.Expression!).Value);
    }

    [TestMethod]
    public void FreeXPreservesIdentityAndNoScope()
    {
        var r=Admit(X());
        Assert.AreEqual("free:x",((IrSymbol)r.Expression!).Identity);
        Assert.IsTrue(r.ContentMathMl!.Contains("<ci type=\"real\">x</ci>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void FreeYPreservesIndependentIdentity()
    {
        var r=Admit(IrSymbol.Free("y","y"));
        Assert.AreEqual("free:y",((IrSymbol)r.Expression!).Identity);
        Assert.IsTrue(r.PresentationMathMl!.Contains("<mi>y</mi>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void FreeZPreservesIndependentIdentity()
    {
        var r=Admit(IrSymbol.Free("z","z"));
        Assert.AreEqual("free:z",((IrSymbol)r.Expression!).Identity);
    }

    [TestMethod]
    public void SimpleAdditionUsesContentPlusWithoutEvaluating()
    {
        var r=Admit(Sum());
        Assert.AreEqual("add(x,1)",r.CanonicalText);
        var ir=(IrApply)r.Expression!;
        Assert.AreEqual(IrOperation.Add,ir.Operation);
        Assert.AreEqual(2,ir.Arguments.Length);
    }

    [TestMethod]
    public void SimpleAdditionHasPresentationOperatorAndContentApply()
    {
        var r=Admit(Sum());
        Assert.IsTrue(r.ContentMathMl!.Contains("<plus",StringComparison.Ordinal));
        Assert.IsTrue(r.PresentationMathMl!.Contains("<mo>+</mo>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void EquationIsAQualifiedExactIrRestrictionNotASolvedRoot()
    {
        var r=Admit(Equation());
        Assert.AreEqual("where(x,eq(x,1))",r.CanonicalText);
        var original=(IrRestrictedExpression)r.Expression!;
        Assert.AreEqual(IrRelationKind.Equal,
            original.Assumptions.Conditions.Single().Kind);
    }

    [TestMethod]
    public void ExactEquationHasContentEqAndPresentationEquals()
    {
        var r=Admit(Equation());
        Assert.IsTrue(r.ContentMathMl!.Contains("<eq",StringComparison.Ordinal));
        Assert.IsTrue(r.PresentationMathMl!.Contains("<mo>=</mo>",StringComparison.Ordinal));
    }

    [TestMethod]
    public void AnnotationTextBindsCanonicalOriginalIr()
    {
        var r=Admit(Equation());
        XNamespace m=ExactMathMlInterop.Namespace;
        var document=XDocument.Parse(r.AnnotatedMathMl!);
        var annotation=document.Descendants(m+"annotation").Single();
        Assert.AreEqual(ExactMathMlInterop.TextEncoding,
            (string?)annotation.Attribute("encoding"));
        Assert.AreEqual("where(x,eq(x,1))",annotation.Value);
    }

    [TestMethod]
    public void ContentAnnotationUsesTheMathMlContentMediaType()
    {
        var r=Admit(N(1,2));
        XNamespace m=ExactMathMlInterop.Namespace;
        var annotation=XDocument.Parse(r.AnnotatedMathMl!)
            .Descendants(m+"annotation-xml").Single();
        Assert.AreEqual(ExactMathMlInterop.ContentEncoding,
            (string?)annotation.Attribute("encoding"));
        Assert.AreEqual("apply",annotation.Elements().Single().Name.LocalName);
    }

    [TestMethod]
    public void XmlNamespaceIsPinnedToTheMathMlStandard()
    {
        var r=Admit(N(1,2));
        Assert.AreEqual("http://www.w3.org/1998/Math/MathML",
            XDocument.Parse(r.AnnotatedMathMl!).Root!.Name.NamespaceName);
    }

    [TestMethod]
    public void ExactlyOneSemanticsElementBindsThreeForms()
    {
        var r=Admit(Sum());
        XNamespace m=ExactMathMlInterop.Namespace;
        var root=XDocument.Parse(r.AnnotatedMathMl!).Root!;
        var semantics=root.Elements(m+"semantics").Single();
        Assert.AreEqual(3,semantics.Elements().Count());
    }

    [TestMethod]
    public void OriginalIrStructuralHashSurvivesIndependentXmlRoundTrip()
    {
        var r=Admit(Equation());
        var decoded=ExactMathMlInterop.ImportAnnotated(r.AnnotatedMathMl!);
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(
            IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(Equation()))),
            decoded.SemanticStructuralSha256);
    }

    [TestMethod]
    public void PresentationOnlyMfracCannotAuthorizeExactContent()
    {
        var r=Admit(N(1,2));
        Refuse(ExactMathMlInterop.ImportAnnotated(r.PresentationMathMl!),
            MathMlInteropStatus.PresentationAmbiguous);
    }

    [TestMethod]
    public void PresentationOnlyMiCannotInventFreeSymbolIdentity()
    {
        var r=Admit(X());
        Refuse(ExactMathMlInterop.ImportAnnotated(r.PresentationMathMl!),
            MathMlInteropStatus.PresentationAmbiguous);
    }

    [TestMethod]
    public void ContentOnlyXmlNeedsAnnotationBeforeImport()
    {
        var r=Admit(N(1,2));
        Refuse(ExactMathMlInterop.ImportAnnotated(r.ContentMathMl!),
            MathMlInteropStatus.PresentationAmbiguous);
    }

    [TestMethod]
    public void TamperedContentNumeratorIsNotAdmitted()
    {
        var r=Admit(N(1,2));
        var xml=r.AnnotatedMathMl!.Replace("<cn type=\"integer\">1</cn>","<cn type=\"integer\">9</cn>",
            StringComparison.Ordinal);
        Assert.AreNotEqual(r.AnnotatedMathMl,xml);
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.IntegrityMismatch);
    }

    [TestMethod]
    public void TamperedPresentationNumeratorCannotOverwriteMeaning()
    {
        var r=Admit(N(1,2));
        var xml=r.AnnotatedMathMl!.Replace("<mn>1</mn>","<mn>9</mn>",
            StringComparison.Ordinal);
        Assert.AreNotEqual(r.AnnotatedMathMl,xml);
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.IntegrityMismatch);
    }

    [TestMethod]
    public void TamperedCanonicalAnnotationIsRejected()
    {
        var r=Admit(N(1,2));
        var xml=r.AnnotatedMathMl!.Replace(">1/2</annotation>",">x</annotation>",
            StringComparison.Ordinal);
        Assert.AreNotEqual(r.AnnotatedMathMl,xml);
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.IntegrityMismatch);
    }

    [TestMethod]
    public void NonCanonicalTextAnnotationIsNotSilentlySimplified()
    {
        var r=Admit(N(1,2));
        var xml=r.AnnotatedMathMl!.Replace(">1/2</annotation>",
            ">2/4</annotation>",StringComparison.Ordinal);
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.NonCanonical);
    }

    [TestMethod]
    public void MissingContentAnnotationDoesNotCreateACertificate()
    {
        var r=Admit(N(1,2));
        XNamespace m=ExactMathMlInterop.Namespace;
        var root=XDocument.Parse(r.AnnotatedMathMl!).Root!;
        root.Descendants(m+"annotation-xml").Single().Remove();
        Refuse(ExactMathMlInterop.ImportAnnotated(
                root.ToString(SaveOptions.DisableFormatting)),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void EmptyXmlIsInvalidWithoutAnIr()
    {
        Refuse(ExactMathMlInterop.ImportAnnotated(""),
            MathMlInteropStatus.InvalidXml);
    }

    [TestMethod]
    public void MalformedUnclosedXmlCannotBeRecovered()
    {
        Refuse(ExactMathMlInterop.ImportAnnotated("<math"),
            MathMlInteropStatus.InvalidXml);
    }

    [TestMethod]
    public void ExternalDtdIsNeverProcessedOrFetched()
    {
        var r=Admit(X());
        var xml="<!DOCTYPE math SYSTEM \"https://example.invalid/external.dtd\">"+
            r.AnnotatedMathMl;
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.InvalidXml);
    }

    [TestMethod]
    public void InternalEntityDeclarationIsProhibited()
    {
        var r=Admit(X());
        var xml="<!DOCTYPE math [<!ENTITY x \"secret\">]>"+
            r.AnnotatedMathMl;
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.InvalidXml);
    }

    [TestMethod]
    public void WrongXmlNamespaceCannotBecomeMathMeaning()
    {
        Refuse(ExactMathMlInterop.ImportAnnotated(
                "<math xmlns=\"https://example.invalid/math\"><mi>x</mi></math>"),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void ChangedXmlDeclarationIsNotCanonicalTransport()
    {
        var r=Admit(N(1,2));
        Refuse(ExactMathMlInterop.ImportAnnotated(
                "<?xml version=\"1.0\"?>"+r.AnnotatedMathMl),
            MathMlInteropStatus.NonCanonical);
    }

    [TestMethod]
    public void ExcessXmlCharactersAreRefusedBeforeParsing()
    {
        Refuse(ExactMathMlInterop.ImportAnnotated(
            new string('x',ExactMathMlInterop.MaximumXmlCharacters+1)),
            MathMlInteropStatus.BudgetExceeded);
    }

    [TestMethod]
    public void ExcessXmlNodesAreRefused()
    {
        var xml="<math xmlns=\""+ExactMathMlInterop.Namespace+"\">"+
            string.Concat(Enumerable.Repeat("<mi>x</mi>",40))+"</math>";
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.BudgetExceeded);
    }

    [TestMethod]
    public void ExcessNestedXmlDepthIsRefused()
    {
        var xml="<math xmlns=\""+ExactMathMlInterop.Namespace+"\">"+
            string.Concat(Enumerable.Repeat("<mrow>",14))+"<mi>x</mi>"+
            string.Concat(Enumerable.Repeat("</mrow>",14))+"</math>";
        Refuse(ExactMathMlInterop.ImportAnnotated(xml),
            MathMlInteropStatus.BudgetExceeded);
    }

    [TestMethod]
    public void BoundSymbolCannotBeDisplayedAsAnUnqualifiedRealFreeVariable()
    {
        Refuse(ExactMathMlInterop.Render(IrSymbol.Bound("scope",0,"x")),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void UnqualifiedForeignSymbolLabelCannotEnterPresentation()
    {
        Refuse(ExactMathMlInterop.Render(IrSymbol.Free("x",@"\input{secret}")),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void Binary64ScalarCannotMasqueradeAsExactMathMl()
    {
        Refuse(ExactMathMlInterop.Render(IrFiniteBinary64Scalar.FromDouble(0.5)),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void NonEqualityRestrictionNeedsAnotherQualifiedMathMlProfile()
    {
        var x=X();
        var restricted=new IrRestrictedExpression(x,IrAssumptionSet.Create([
            new IrRelation(x,IrRelationKind.NotEqual,N(1))]));
        Refuse(ExactMathMlInterop.Render(restricted),
            MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void UnsupportedMultiplicationIsNotGuesswork()
    {
        var expr=IrApply.Create(IrOperation.Multiply,[X(),N(2)]);
        Refuse(ExactMathMlInterop.Render(expr),MathMlInteropStatus.Unsupported);
    }

    [TestMethod]
    public void VersionedContractRejectsUnannotatedAndOverBudgetMathMl()
    {
        var qualified=ExactMathMlInterop.Render(N(1,2));
        Assert.AreEqual(MathMlInteropStatus.Admitted,qualified.Status);
        Assert.AreEqual(ExactMathMlInterop.Rule,qualified.RuleId);
        var presentation=new XElement(
            XNamespace.Get(ExactMathMlInterop.Namespace)+"math",
            new XElement(XNamespace.Get(ExactMathMlInterop.Namespace)+"mi","x"))
            .ToString(SaveOptions.DisableFormatting);
        Refuse(ExactMathMlInterop.ImportAnnotated(presentation),
            MathMlInteropStatus.PresentationAmbiguous);
        var oversized=new string('x',ExactMathMlInterop.MaximumXmlCharacters+1);
        Refuse(ExactMathMlInterop.ImportAnnotated(oversized),
            MathMlInteropStatus.BudgetExceeded);
    }
}
