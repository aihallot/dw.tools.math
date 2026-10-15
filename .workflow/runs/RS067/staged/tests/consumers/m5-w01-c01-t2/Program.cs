using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Codecs;

var passed=0;
void Probe(string title,Action oracle)
{
    try { oracle(); passed++; }
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 display-boundary package-only consumer: "+title,error);
    }
}
static void Require(bool condition,string reason)
{
    if(!condition)throw new InvalidOperationException(reason);
}
Probe("exact half retains safe display and original IR identity",()=>
{
    var r=CanonicalDisplayBoundaryAudit.AuditText("1/2");
    Require(r.IsAdmitted&&
        r.Status==CanonicalDisplayBoundaryStatus.SafeDisplay&&
        r.DisplayLatex=="\\frac{1}{2}"&&
        ((IrExactScalar)r.Expression!).Value==new ExactRational(1,2),
        "a structural rational cannot become a binary64 display");
});
Probe("square and exponent-two are distinct trees sharing display",()=>
{
    var square=CanonicalDisplayBoundaryAudit.AuditText("square(x)");
    var power=CanonicalDisplayBoundaryAudit.AuditText("pow(x,2)");
    Require(square.IsAdmitted&&power.IsAdmitted&&
        square.PresentationMayCollide&&power.PresentationMayCollide&&
        square.DisplayLatex==power.DisplayLatex&&
        square.SemanticStructuralSha256!=power.SemanticStructuralSha256&&
        !square.LatexInverseQualified&&!power.LatexInverseQualified,
        "LaTeX presentation is not an inverse structural codec");
});
Probe("retained original-domain exclusion survives text and IR",()=>
{
    var result=CanonicalDisplayBoundaryAudit.AuditText(
        "where(div(sub(square(x),1),sub(x,1)),ne(x,1))");
    Require(result.IsAdmitted&&result.PresentationMayCollide&&
        result.Expression is IrRestrictedExpression restricted&&
        restricted.Assumptions.Conditions[0].Kind==IrRelationKind.NotEqual&&
        result.DisplayLatex!.Contains("\\ne",StringComparison.Ordinal),
        "original exclusions must not be simplified away");
});
Probe("condition on absent y is refused without provenance",()=>
{
    var r=CanonicalDisplayBoundaryAudit.AuditText("where(x,ne(y,1))");
    Require(r.Status==CanonicalDisplayBoundaryStatus.UnboundRestriction&&
        !r.IsAdmitted&&r.Expression is null&&r.RuleId is null,
        "an unrelated condition is not a certified math display");
});
Probe("contradictory equal and not-equal hypothesis is refused",()=>
{
    var r=CanonicalDisplayBoundaryAudit.AuditText("where(x,eq(x,1),ne(x,1))");
    Require(r.Status==CanonicalDisplayBoundaryStatus.ContradictoryAssumptions&&
        !r.IsAdmitted&&r.DisplayLatex is null&&r.SemanticStructuralSha256 is null,
        "no mathematical source proof for contradictory hypotheses");
});
Probe("untrusted TeX macro is not interpreted as canonical math",()=>
{
    var r=CanonicalDisplayBoundaryAudit.AuditText(@"\input{private}");
    Require(r.Status==CanonicalDisplayBoundaryStatus.RefusedInput&&
        r.UnderlyingStatus==CanonicalNotationStatus.InvalidInput&&
        !r.IsAdmitted&&r.DisplayLatex is null,
        "TeX parsing and execution are not admitted");
});
Probe("exact Kelvin canonicalization is preserved and aliases refused",()=>
{
    var good=CanonicalDisplayBoundaryAudit.AuditText(
        "unit(5463/20,si.kelvin,Absolute)");
    var bad=CanonicalDisplayBoundaryAudit.AuditText(
        "unit(27315/100,si.kelvin,Absolute)");
    Require(good.IsAdmitted&&
        ((IrExactScalar)((IrQuantityLiteral)good.Expression!).Value).Value==
            new ExactRational(27315,100)&&
        bad.Status==CanonicalDisplayBoundaryStatus.RefusedInput&&
        bad.UnderlyingStatus==CanonicalNotationStatus.NonCanonical,
        "semantically equal but unreduced fractions are not canonical inputs");
});
Probe("independent NuGet product has no executable CAS or agent broker",()=>
{
    var assemblies=new[]{typeof(CanonicalDisplayBoundaryAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!deps.Any(n=>
        n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no external symbolic provider or agent runtime is needed");
});
if(passed!=8)
    throw new InvalidOperationException(
        "Exactly eight independently pinned Math display-boundary oracles required.");
Console.WriteLine("dw.tools.math/m5-display-boundary-consumer/1.0 qualified; probes="+passed);
