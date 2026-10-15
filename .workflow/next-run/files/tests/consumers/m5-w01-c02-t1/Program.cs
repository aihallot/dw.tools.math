using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Codecs;

var passed=0;
void Probe(string label, Action check)
{
    try { check(); passed++; }
    catch(Exception error)
    {
        throw new InvalidOperationException("M5 isolated LaTeX importer consumer: "+label,error);
    }
}
static void Require(bool assertion,string message)
{
    if(!assertion)throw new InvalidOperationException(message);
}
Probe("exact LaTeX fraction one half imports without floating point",()=>
{
    var r=BoundedLatexImporter.Import(@"\frac{1}{2}");
    Require(r.IsAdmitted&&r.Status==LatexImportStatus.Admitted&&
        r.CanonicalText=="1/2"&&r.Expression is IrExactScalar scalar&&
        scalar.Value==new ExactRational(1,2),"exact LaTeX fraction");
});
Probe("T1 sqrt LaTeX becomes typed unevaluated function",()=>
{
    var r=BoundedLatexImporter.Import(@"\sqrt{x}");
    Require(r.IsAdmitted&&r.CanonicalText=="sqrt(x)"&&
        r.Expression is IrApply { Operation:IrOperation.Sqrt },
        "the importer must not evaluate radical functions");
});
Probe("explicit cdot denotes typed real multiplication",()=>
{
    var r=BoundedLatexImporter.Import(@"x\cdot y");
    Require(r.IsAdmitted&&r.CanonicalText=="mul(x,y)"&&
        r.Expression is IrApply { Operation:IrOperation.Multiply },
        "explicit multiplication must retain original operands");
});
Probe("juxtaposed x of y remains ambiguous by default",()=>
{
    var r=BoundedLatexImporter.Import("x(y)");
    Require(r.Status==LatexImportStatus.Ambiguous&&
        !r.IsAdmitted&&r.Expression is null&&r.RuleId is null,
        "implicit function application is not silently multiplication");
});
Probe("caller-selected multiplication convention is recorded",()=>
{
    var r=BoundedLatexImporter.Import("x(y)",
        LatexJuxtapositionConvention.Multiplication);
    Require(r.IsAdmitted&&r.CanonicalText=="mul(x,y)"&&
        r.AppliedConvention==LatexJuxtapositionConvention.Multiplication,
        "explicit convention resolves this bounded ambiguity");
});
Probe("T1 square/power-two display is non-injective",()=>
{
    var original=CanonicalMathNotation.Render(
        IrApply.Create(IrOperation.Square,[IrSymbol.Free("x","x")]));
    var r=BoundedLatexImporter.Import(original.Latex!);
    Require(original.IsAdmitted&&r.Status==LatexImportStatus.Ambiguous&&
        r.Expression is null&&r.SemanticStructuralSha256 is null,
        "LaTeX display does not identify one unique source IR constructor");
});
Probe("unsafe TeX commands are unsupported, never executed",()=>
{
    var r=BoundedLatexImporter.Import(@"\input{private}");
    var alias=BoundedLatexImporter.Import(@"\frac{2}{4}");
    Require(r.Status==LatexImportStatus.Unsupported&&
        alias.Status==LatexImportStatus.NonCanonical&&
        !r.IsAdmitted&&!alias.IsAdmitted&&r.CanonicalText is null,
        "only the closed bounded non-executing grammar is admitted");
});
Probe("semantic hash survives canonical IR transport without CAS runtime",()=>
{
    var r=BoundedLatexImporter.Import(@"\frac{1}{2}\cdot x");
    Require(r.IsAdmitted&&r.Expression is not null&&
        r.SemanticStructuralSha256==IrCanonicalHashes.SemanticStructuralSha256(
            IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(r.Expression))),
        "exact source structural identity cannot be invented");
    var loaded=new[]{typeof(BoundedLatexImporter).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=loaded.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty);
    Require(!deps.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "the local Math API has no mandatory foreign CAS or agent runtime");
});
if(passed!=8)
    throw new InvalidOperationException("Exactly eight isolated LaTeX import oracles required.");
Console.WriteLine("dw.tools.math/m5-latex-import-consumer/1.0 qualified; probes="+passed);
