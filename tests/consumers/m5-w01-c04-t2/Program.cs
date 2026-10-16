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
            "M5 exact Markdown/OMML boundary NuGet consumer: "+name,error);
    }
}
static void Require(bool accepted,string message)
{
    if(!accepted)throw new InvalidOperationException(message);
}
static IrExactScalar N(long n,long d=1)=>new(new ExactRational(n,d));
static IrSymbol X()=>IrSymbol.Free("x","x");
static IrNode Equality()
{
    var x=X();
    return new IrRestrictedExpression(x,IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.Equal,N(1))]));
}
Probe("standalone canonical exact OMML fraction",()=>
{
    var source=N(1,2);
    var xml=ExactOfficeOmmlCodec.Render(source).Omml!;
    var result=MathDisplayBoundaryAudit.AuditOmml(xml,source);
    Require(result.IsAdmitted&&result.CanonicalText=="1/2"&&
        result.SemanticStructuralSha256==IrCanonicalHashes.SemanticStructuralSha256(source)&&
        result.SourceSha256?.Length==64&&result.StructuralFidelityQualified,
        "a standalone exact rational must retain original source IR");
});
Probe("original equality restriction is not solved",()=>
{
    var source=Equality();
    var xml=ExactOfficeOmmlCodec.Render(source).Omml!;
    var result=MathDisplayBoundaryAudit.AuditOmml(xml,source);
    Require(result.IsAdmitted&&result.Expression is IrRestrictedExpression&&
        IrCanonicalJsonCodec.Encode(source)==IrCanonicalJsonCodec.Encode(result.Expression!)&&
        !result.TypographyInverseQualified,
        "original equality cannot become a typography-derived solution");
});
Probe("Word document root is rejected",()=>
{
    var xml="<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"/>";
    var result=MathDisplayBoundaryAudit.AuditOmml(xml,Equality());
    Require(result.Status==MathDisplayBoundaryStatus.Unsupported&&
        result.LossKind==MathDisplayLossKind.UnsupportedDocumentOrProfile&&
        result.Expression is null&&result.RuleId is null,
        "whole Word documents are outside the standalone OMML profile");
});
Probe("Office font run/style cannot claim fidelity",()=>
{
    var xml=ExactOfficeOmmlCodec.Render(N(1,2)).Omml!;
    var decorated=xml.Replace("<m:r>","<m:r><m:rPr><m:sty m:val=\"b\" /></m:rPr>",
        StringComparison.Ordinal);
    var result=MathDisplayBoundaryAudit.AuditOmml(decorated,N(1,2));
    Require(decorated!=xml&&
        result.Status==MathDisplayBoundaryStatus.IntegrityMismatch&&
        result.LossKind==MathDisplayLossKind.UnqualifiedLayoutOrSemanticMarkup&&
        !result.FontFidelityQualified&&!result.PositioningFidelityQualified&&
        result.SemanticStructuralSha256 is null,
        "font and positioning are not silently normalized");
});
Probe("DTD and entities remain non-executable",()=>
{
    var xml="<!DOCTYPE m:oMath [<!ENTITY x '9'>]>"+
        ExactOfficeOmmlCodec.Render(N(1,2)).Omml;
    var result=MathDisplayBoundaryAudit.AuditOmml(xml,N(1,2));
    Require(result.Status==MathDisplayBoundaryStatus.InvalidSource&&
        result.LossKind==MathDisplayLossKind.InvalidXml&&
        result.Expression is null,"XML entities may not enter the Math trust boundary");
});
Probe("inline and block Markdown remain different source placements",()=>
{
    var source=N(1,2);
    var inline=MarkdownUnicodeMathCodec.Render(source,MarkdownMathPlacement.Inline).Markdown!;
    var accepted=MathDisplayBoundaryAudit.AuditMarkdown(
        inline,MarkdownMathPlacement.Inline,source);
    var rejected=MathDisplayBoundaryAudit.AuditMarkdown(
        inline,MarkdownMathPlacement.Block,source);
    Require(accepted.IsAdmitted&&accepted.CanonicalText=="1/2"&&
        rejected.Status==MathDisplayBoundaryStatus.PresentationAmbiguous&&
        rejected.LossKind==MathDisplayLossKind.MarkdownPlacementMismatch&&
        rejected.Expression is null,
        "Markdown metadata cannot silently exchange inline and block placement");
});
Probe("unqualified Unicode slash has no mathematical inverse",()=>
{
    var result=MathDisplayBoundaryAudit.AuditMarkdown(
        "$1/2$",MarkdownMathPlacement.Inline,N(1,2));
    Require(result.Status==MathDisplayBoundaryStatus.Unsupported&&
        result.LossKind==MathDisplayLossKind.UnqualifiedMarkdownOrUnicode&&
        !result.TypographyInverseQualified&&result.RuleId is null,
        "ASCII slash is not a proven inverse of exact mathematical Unicode");
});
Probe("no installed Office SDK, external CAS or agent dependency",()=>
{
    var admitted=MathDisplayBoundaryAudit.AuditMarkdown(
        "$$"+"\n"+"x = 1"+"\n"+"$$",
        MarkdownMathPlacement.Block,Equality());
    var names=new[]{typeof(MathDisplayBoundaryAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(admitted.IsAdmitted&&!admitted.RequiresOfficeInstallation&&
        !admitted.WholeDocumentConversionQualified&&!admitted.FontFidelityQualified&&
        !admitted.PositioningFidelityQualified&&
        !names.Any(n=>new[]{"Office","OpenXml","Angouri","MathNet","Aura","Decision","MCDM"}
            .Any(f=>n.Contains(f,StringComparison.OrdinalIgnoreCase))),
        "standalone qualified Math must require no installed Office or agent broker");
});
if(passed!=8)
    throw new InvalidOperationException("M5 math display boundary NuGet gate requires eight probes.");
Console.WriteLine("dw.tools.math/m5-markdown-omml-boundary-consumer/1.0 qualified; probes="+passed);
