using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Codecs;

var passed=0;
void Probe(string title,Action oracle)
{
    try {oracle();passed++;}
    catch(Exception exception)
    {
        throw new InvalidOperationException("M5 LaTeX loss-audit isolated NuGet consumer: "+title,exception);
    }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}

Probe("exact one-half keeps lossless Math meaning and source provenance",()=>
{
    var r=LatexImportBoundaryAudit.Audit(@"\frac{1}{2}");
    Require(r.IsAdmitted&&r.SemanticFidelityQualified&&
        r.Status==LatexImportStatus.Admitted&&
        r.LossKind==LatexImportLossKind.PassiveNotationChange&&
        r.CanonicalText=="1/2"&&
        r.Expression is IrExactScalar half&&
        half.Value==new ExactRational(1,2)&&
        r.SourceSha256?.Length==64&&!r.LatexInverseQualified,
        "fraction presentation change must not change exact rational meaning");
});
Probe("original x identity has no reformatting loss",()=>
{
    var r=LatexImportBoundaryAudit.Audit("x");
    Require(r.IsAdmitted&&r.LossKind==LatexImportLossKind.None&&
        r.Expression is IrSymbol symbol&&symbol.Identity=="free:x"&&
        !r.UsedExplicitMultiplicationConvention,
        "free real x remains original source identity");
});
Probe("function-versus-product juxtaposition refuses by default",()=>
{
    var r=LatexImportBoundaryAudit.Audit("x(y)");
    Require(r.Status==LatexImportStatus.Ambiguous&&
        r.LossKind==LatexImportLossKind.AmbiguousInterpretation&&
        !r.IsAdmitted&&r.Expression is null&&r.RuleId is null&&
        r.SourceSha256?.Length==64,
        "a fingerprint is not a mathematical permission to interpret x(y)");
});
Probe("explicit product convention is preserved in admitted result",()=>
{
    var r=LatexImportBoundaryAudit.Audit("x(y)",
        LatexJuxtapositionConvention.Multiplication);
    Require(r.IsAdmitted&&r.UsedExplicitMultiplicationConvention&&
        r.LossKind==LatexImportLossKind.ExplicitMultiplicationConvention&&
        r.CanonicalText=="mul(x,y)"&&
        r.AppliedConvention==LatexJuxtapositionConvention.Multiplication,
        "caller policy cannot be silently hidden from result provenance");
});
Probe("non-injective T1 square/power display remains ambiguous",()=>
{
    var r=LatexImportBoundaryAudit.Audit(@"\left(x\right)^{2}");
    Require(r.Status==LatexImportStatus.Ambiguous&&
        r.LossKind==LatexImportLossKind.AmbiguousInterpretation&&
        !r.IsAdmitted&&r.SemanticStructuralSha256 is null,
        "identical typography is not an inverse Math IR codec");
});
Probe("hostile input macro is unsupported without a mathematical proof",()=>
{
    var r=LatexImportBoundaryAudit.Audit(@"\input{file}");
    Require(r.Status==LatexImportStatus.Unsupported&&
        r.LossKind==LatexImportLossKind.UnsupportedShape&&
        r.SourceSha256?.Length==64&&
        r.Expression is null&&r.SemanticStructuralSha256 is null&&
        !r.SemanticFidelityQualified,
        "external arbitrary TeX must never run or imply admitted meaning");
});
Probe("invalid and noncanonical fractions are typed separate refusals",()=>
{
    var noncanonical=LatexImportBoundaryAudit.Audit(@"\frac{2}{4}");
    var invalid=LatexImportBoundaryAudit.Audit(@"\frac{1}{0}");
    Require(noncanonical.Status==LatexImportStatus.NonCanonical&&
        noncanonical.LossKind==LatexImportLossKind.NonCanonicalRepresentation&&
        invalid.Status==LatexImportStatus.Invalid&&
        invalid.LossKind==LatexImportLossKind.InvalidSyntax&&
        noncanonical.Expression is null&&invalid.Expression is null,
        "unqualified normalization and undefined denominator must not pass");
});
Probe("package-only exact IR plus no CAS or agent broker dependencies",()=>
{
    var r=LatexImportBoundaryAudit.Audit(@"\sqrt{x}");
    Require(r.IsAdmitted&&r.Expression is not null&&
        r.SemanticStructuralSha256==
            IrCanonicalHashes.SemanticStructuralSha256(
                IrCanonicalJsonCodec.Decode(
                    IrCanonicalJsonCodec.Encode(r.Expression))),
        "independent canonical IR replay");
    var deps=new[]{typeof(LatexImportBoundaryAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(x=>x.GetReferencedAssemblies())
        .Select(x=>x.Name??string.Empty);
    Require(!deps.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "Math must remain usable without any CAS or agent broker runtime");
});
if(passed!=8)
    throw new InvalidOperationException("Eight independent NuGet-only LaTeX boundary oracles required.");
Console.WriteLine("dw.tools.math/m5-latex-loss-consumer/1.0 qualified; probes="+passed);
