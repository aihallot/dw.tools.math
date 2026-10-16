using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Ir;

var passed=0;
void Probe(string name,Action oracle)
{
    try {oracle();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 exact Markdown/OMML NuGet consumer: "+name,error);
    }
}
static void Require(bool accepted,string message)
{
    if(!accepted)throw new InvalidOperationException(message);
}
static IrExactScalar N(long n,long d=1)=>new(new ExactRational(n,d));
static IrSymbol X()=>IrSymbol.Free("x","x");
static IrNode Equation()
{
    var x=X();
    return new IrRestrictedExpression(x,IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.Equal,N(1))]));
}
Probe("exact fraction OMML round-trip",()=>
{
    var render=ExactOfficeOmmlCodec.Render(N(1,2));
    var import=ExactOfficeOmmlCodec.ImportCanonical(render.Omml!,N(1,2));
    Require(render.IsAdmitted&&import.IsAdmitted&&
        ((IrExactScalar)import.Expression!).Value==new ExactRational(1,2)&&
        import.SemanticStructuralSha256==IrCanonicalHashes.SemanticStructuralSha256(N(1,2)),
        "exact rational identity must traverse OMML without floating conversion");
});
Probe("original equality OMML round-trip",()=>
{
    var source=Equation();
    var render=ExactOfficeOmmlCodec.Render(source);
    var import=ExactOfficeOmmlCodec.ImportCanonical(render.Omml!,source);
    Require(import.IsAdmitted&&import.Expression is IrRestrictedExpression&&
        IrCanonicalJsonCodec.Encode(import.Expression!)==IrCanonicalJsonCodec.Encode(source),
        "an equation must remain the original typed restriction, not a solved root");
});
Probe("inline Markdown wrapper",()=>
{
    var output=MarkdownUnicodeMathCodec.Render(N(1,2),MarkdownMathPlacement.Inline);
    Require(output.IsAdmitted&&output.Markdown=="$1\u20442$"&&
        output.CanonicalText=="1/2","inline Unicode display needs explicit delimiters");
});
Probe("block Markdown wrapper",()=>
{
    var output=MarkdownUnicodeMathCodec.Render(Equation(),MarkdownMathPlacement.Block);
    Require(output.IsAdmitted&&output.Markdown=="$$\nx = 1\n$$"&&
        output.Placement==MarkdownMathPlacement.Block,
        "block wrapper must remain distinct from inline");
});
Probe("changed denominator refuses semantic replay",()=>
{
    var source=ExactOfficeOmmlCodec.Render(N(1,2)).Omml!;
    var changed=source.Replace(">2</m:t>",">9</m:t>",StringComparison.Ordinal);
    var decoded=ExactOfficeOmmlCodec.ImportCanonical(changed,N(1,2));
    Require(changed!=source&&decoded.Status==OfficeOmmlStatus.IntegrityMismatch&&
        decoded.Expression is null&&decoded.RuleId is null,
        "rewriting OMML typography cannot rewrite original exact fraction");
});
Probe("mismatched original IR refuses import",()=>
{
    var source=ExactOfficeOmmlCodec.Render(N(1,2)).Omml!;
    var decoded=ExactOfficeOmmlCodec.ImportCanonical(source,N(2,3));
    Require(decoded.Status==OfficeOmmlStatus.IntegrityMismatch&&
        decoded.Expression is null&&decoded.RuleId is null,
        "OMML typography cannot invent caller-supplied source identity");
});
Probe("whole Office document cannot enter the equation codec",()=>
{
    var xml="<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"/>";
    var result=ExactOfficeOmmlCodec.ImportCanonical(xml,Equation());
    Require(result.Status==OfficeOmmlStatus.UnsupportedProfile&&
        !result.IsAdmitted&&result.Expression is null,
        "a standalone math codec must not act as a document parser");
});
Probe("no Office, foreign CAS, decision or agent dependency",()=>
{
    var output=ExactOfficeOmmlCodec.Render(N(7));
    Require(output.IsAdmitted&&output.CanonicalText=="7"&&
        !new[]{typeof(ExactOfficeOmmlCodec).Assembly,
            typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
            .SelectMany(a=>a.GetReferencedAssemblies())
            .Select(a=>a.Name??string.Empty)
            .Any(n=>new[]{"Office","OpenXml","Angouri","MathNet","Aura","Decision","MCDM"}
                .Any(f=>n.Contains(f,StringComparison.OrdinalIgnoreCase))),
        "the pinned Math package graph must have no Office installation or agent runtime");
});
if(passed!=8)
    throw new InvalidOperationException("M5 Office/Markdown NuGet-only gate requires eight independent oracles.");
Console.WriteLine("dw.tools.math/m5-markdown-omml-consumer/1.0 qualified; probes="+passed);
