using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Codecs;

var passed=0;
void Probe(string name,Action oracle)
{
    try {oracle();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 bounded MathML security and provenance NuGet consumer: "+name,error);
    }
}
static void Require(bool success,string message)
{
    if(!success)throw new InvalidOperationException(message);
}
static IrExactScalar N(long n,long d=1)=>new(new ExactRational(n,d));
static IrSymbol X()=>IrSymbol.Free("x","x");

Probe("exact annotated half survives a second structural IR audit",()=>
{
    var original=ExactMathMlInterop.Render(N(1,2));
    var result=MathMlBoundaryAudit.Audit(original.AnnotatedMathMl!);
    Require(result.IsAdmitted&&result.StructuralFidelityQualified&&
        result.CanonicalText=="1/2"&&
        ((IrExactScalar)result.Expression!).Value==new ExactRational(1,2)&&
        result.SourceSha256?.Length==64&&
        !result.PresentationInverseQualified,
        "typed rational fidelity is not a generic typography inverse");
});
Probe("content math identity preserves an original equality restriction",()=>
{
    var x=X();
    var condition=new IrRestrictedExpression(x,IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.Equal,N(1))]));
    var xml=ExactMathMlInterop.Render(condition).AnnotatedMathMl!;
    var result=MathMlBoundaryAudit.Audit(xml);
    Require(result.IsAdmitted&&result.Expression is IrRestrictedExpression ir&&
        ir.Assumptions.Conditions.Single().Kind==IrRelationKind.Equal&&
        result.CanonicalText=="where(x,eq(x,1))",
        "the equality remains an original IR restriction, not solved roots");
});
Probe("ordered x+1 returns the original Math semantic digest",()=>
{
    var expr=IrApply.Create(IrOperation.Add,[X(),N(1)]);
    var xml=ExactMathMlInterop.Render(expr).AnnotatedMathMl!;
    var result=MathMlBoundaryAudit.Audit(xml);
    Require(result.IsAdmitted&&result.CanonicalText=="add(x,1)"&&
        result.SemanticStructuralSha256==
            IrCanonicalHashes.SemanticStructuralSha256(expr),
        "MathML presentation cannot silently rearrange the original operands");
});
Probe("presentation-only MathML has no source mathematical proof",()=>
{
    var presentation=ExactMathMlInterop.Render(N(1,2)).PresentationMathMl!;
    var result=MathMlBoundaryAudit.Audit(presentation);
    Require(result.Status==MathMlInteropStatus.PresentationAmbiguous&&
        result.LossKind==MathMlBoundaryLossKind.PresentationWithoutAnnotation&&
        result.Expression is null&&result.RuleId is null&&
        result.SourceSha256?.Length==64,
        "source fingerprint on refusal does not constitute a content proof");
});
Probe("DTD external entity is rejected without fetching a resource",()=>
{
    var xml="<!DOCTYPE math SYSTEM \"https://example.invalid/private.dtd\">"+
        ExactMathMlInterop.Render(X()).AnnotatedMathMl;
    var result=MathMlBoundaryAudit.Audit(xml);
    Require(result.Status==MathMlInteropStatus.InvalidXml&&
        result.LossKind==MathMlBoundaryLossKind.InvalidXml&&
        !result.IsAdmitted&&result.SemanticStructuralSha256 is null,
        "external XML entities must not enter the Math trust boundary");
});
Probe("tampered typed content rational is not silently normalized",()=>
{
    var original=ExactMathMlInterop.Render(N(1,2)).AnnotatedMathMl!;
    var changed=original.Replace("<cn type=\"integer\">1</cn>",
        "<cn type=\"integer\">9</cn>",StringComparison.Ordinal);
    var result=MathMlBoundaryAudit.Audit(changed);
    Require(changed!=original&&
        result.Status==MathMlInteropStatus.IntegrityMismatch&&
        result.LossKind==MathMlBoundaryLossKind.AlteredSemanticBinding&&
        result.Expression is null&&result.RuleId is null,
        "an altered Content MathML branch invalidates the whole annotation");
});
Probe("unreduced annotation and unrecognized namespace are separate refusals",()=>
{
    var text=ExactMathMlInterop.Render(N(1,2)).AnnotatedMathMl!;
    var unreduced=MathMlBoundaryAudit.Audit(text.Replace(
        ">1/2</annotation>",">2/4</annotation>",StringComparison.Ordinal));
    var foreign=MathMlBoundaryAudit.Audit(
        "<math xmlns=\"https://example.invalid/math\"><mi>x</mi></math>");
    Require(unreduced.Status==MathMlInteropStatus.NonCanonical&&
        unreduced.LossKind==MathMlBoundaryLossKind.NonCanonicalSerialization&&
        foreign.Status==MathMlInteropStatus.Unsupported&&
        foreign.LossKind==MathMlBoundaryLossKind.UnsupportedProfile,
        "noncanonical rational and foreign XML profile must remain separate");
});
Probe("over-budget source has no hash and no foreign runtime dependency",()=>
{
    var result=MathMlBoundaryAudit.Audit(
        new string('x',MathMlBoundaryAudit.MaximumInputCharacters+1));
    Require(result.Status==MathMlInteropStatus.BudgetExceeded&&
        result.LossKind==MathMlBoundaryLossKind.ResourceLimit&&
        result.SourceSha256 is null&&!result.IsAdmitted,
        "unbounded XML does not allocate a source fingerprint");
    var names=new[]{typeof(MathMlBoundaryAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!names.Any(n=>
        n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "Math must be usable without external CAS or agent broker dependencies");
});
if(passed!=8)
    throw new InvalidOperationException("MathML boundary gate requires eight isolated NuGet oracles.");
Console.WriteLine("dw.tools.math/m5-mathml-boundary-consumer/1.0 qualified; probes="+passed);
