using System;
using System.Linq;
using System.Xml.Linq;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;

var passed=0;
void Probe(string name, Action oracle)
{
    try { oracle(); passed++; }
    catch(Exception error)
    {
        throw new InvalidOperationException("M5 MathML isolated NuGet consumer: "+name,error);
    }
}
static void Require(bool valid,string reason)
{
    if(!valid)throw new InvalidOperationException(reason);
}
static IrExactScalar N(long numerator,long denominator=1)=>
    new(new ExactRational(numerator,denominator));
static IrSymbol X()=>IrSymbol.Free("x","x");

Probe("half preserves exact semantic IR after annotated MathML",()=>
{
    var original=ExactMathMlInterop.Render(N(1,2));
    var imported=ExactMathMlInterop.ImportAnnotated(original.AnnotatedMathMl!);
    Require(original.IsAdmitted&&imported.IsAdmitted&&
        imported.CanonicalText=="1/2"&&
        ((IrExactScalar)imported.Expression!).Value==new ExactRational(1,2),
        "exact rational half and qualified MathML semantic annotation");
});
Probe("Content MathML fraction is explicitly structured and typed",()=>
{
    var r=ExactMathMlInterop.Render(N(1,2));
    XNamespace m=ExactMathMlInterop.Namespace;
    var apply=XDocument.Parse(r.ContentMathMl!).Root!.Element(m+"apply")!;
    Require(apply.Elements().First().Name.LocalName=="divide"&&
        apply.Elements(m+"cn").All(c=>(string?)c.Attribute("type")=="integer")&&
        apply.Elements(m+"cn").Count()==2,
        "no binary64 or decorative fraction assumed as content proof");
});
Probe("three real symbol identity is bound by canonical annotation",()=>
{
    var r=ExactMathMlInterop.Render(X());
    var again=ExactMathMlInterop.ImportAnnotated(r.AnnotatedMathMl!);
    Require(again.IsAdmitted&&again.Expression is IrSymbol symbol&&
        symbol.Identity=="free:x"&&symbol.Domain==IrScalarDomain.Real&&
        again.CanonicalText=="x",
        "symbol presentation alone cannot provide typed real identity");
});
Probe("equation restriction survives content and presentation exchange",()=>
{
    var x=X();
    var source=new IrRestrictedExpression(x,IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.Equal,N(1))]));
    var rendered=ExactMathMlInterop.Render(source);
    var imported=ExactMathMlInterop.ImportAnnotated(rendered.AnnotatedMathMl!);
    Require(imported.IsAdmitted&&
        imported.Expression is IrRestrictedExpression condition&&
        condition.Assumptions.Conditions.Single().Kind==IrRelationKind.Equal&&
        imported.CanonicalText=="where(x,eq(x,1))"&&
        imported.ContentMathMl!.Contains("<eq",StringComparison.Ordinal),
        "an exact typed original restriction is not a solved equation");
});
Probe("simple sum preserves operand order and structural hash",()=>
{
    var expression=IrApply.Create(IrOperation.Add,[X(),N(1)]);
    var r=ExactMathMlInterop.Render(expression);
    var imported=ExactMathMlInterop.ImportAnnotated(r.AnnotatedMathMl!);
    Require(imported.IsAdmitted&&imported.CanonicalText=="add(x,1)"&&
        imported.SemanticStructuralSha256==
            IrCanonicalHashes.SemanticStructuralSha256(expression)&&
        r.PresentationMathMl!.Contains("<mo>+</mo>",StringComparison.Ordinal),
        "original x+1 is not silently numerically simplified");
});
Probe("presentation-only fraction remains ambiguous without source proof",()=>
{
    var annotated=ExactMathMlInterop.Render(N(1,2));
    var r=ExactMathMlInterop.ImportAnnotated(annotated.PresentationMathMl!);
    Require(r.Status==MathMlInteropStatus.PresentationAmbiguous&&
        !r.IsAdmitted&&r.Expression is null&&r.SemanticStructuralSha256 is null,
        "display typography cannot authorize exact Math IR reconstruction");
});
Probe("tampered Content MathML and external DTD are refused",()=>
{
    var r=ExactMathMlInterop.Render(N(1,2));
    var changed=r.AnnotatedMathMl!.Replace(
        "<cn type=\"integer\">1</cn>","<cn type=\"integer\">9</cn>",
        StringComparison.Ordinal);
    var tampered=ExactMathMlInterop.ImportAnnotated(changed);
    var dtd=ExactMathMlInterop.ImportAnnotated(
        "<!DOCTYPE math SYSTEM \"https://example.invalid/file.dtd\">"+
        r.AnnotatedMathMl);
    Require(changed!=r.AnnotatedMathMl&&
        tampered.Status==MathMlInteropStatus.IntegrityMismatch&&
        dtd.Status==MathMlInteropStatus.InvalidXml&&
        tampered.Expression is null&&dtd.Expression is null,
        "untrusted XML, external entities and altered annotations are not proof");
});
Probe("pinned package has no foreign CAS or agent-broker dependency",()=>
{
    var deps=new[]{typeof(ExactMathMlInterop).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(x=>x.GetReferencedAssemblies())
        .Select(x=>x.Name??string.Empty).ToArray();
    Require(!deps.Any(n=>
        n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "neutral math interchange must not require a foreign CAS or broker");
});
if(passed!=8)
    throw new InvalidOperationException("MathML science gate requires eight independent NuGet-only probes.");
Console.WriteLine("dw.tools.math/m5-mathml-consumer/1.0 qualified; probes="+passed);
