using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Codecs;

var passed=0;
void Probe(string label, Action oracle)
{
    try { oracle(); passed++; }
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 canonical notation NuGet-only consumer: "+label,error);
    }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}

Probe("positive rational transports as exact fraction LaTeX",()=>
{
    var result=CanonicalMathNotation.Parse("1/2");
    Require(result.IsAdmitted&&result.Latex=="\\frac{1}{2}"&&
        ((IrExactScalar)result.Expression!).Value==new ExactRational(1,2),
        "canonical exact rational and display-only LaTeX");
});
Probe("polynomial preserves IR argument structure",()=>
{
    const string input="add(square(x),mul(2,x),1)";
    var result=CanonicalMathNotation.Parse(input);
    Require(result.IsAdmitted&&result.CanonicalText==input&&
        result.Expression is IrApply {Operation:IrOperation.Add} sum&&
        sum.Arguments.Length==3&&result.Latex!.Contains("\\cdot",StringComparison.Ordinal),
        "exact polynomial operations must remain structural");
});
Probe("radical renders as a function without being evaluated",()=>
{
    var result=CanonicalMathNotation.Parse("sqrt(square(x))");
    Require(result.IsAdmitted&&result.Expression is IrApply
        {Operation:IrOperation.Sqrt}&&
        result.Latex!.Contains("\\sqrt",StringComparison.Ordinal),
        "a display rule is not mathematical simplification");
});
Probe("dimensional inch metadata survives parse and render",()=>
{
    var result=CanonicalMathNotation.Parse("unit(5/2,imperial.inch,None)");
    Require(result.IsAdmitted&&result.Expression is IrQuantityLiteral q&&
        q.System==UnitSystem.BritishImperial&&
        q.Dimension==DimensionVector.LengthDimension&&
        ((IrExactScalar)q.Value).Value==new ExactRational(5,2)&&
        result.Latex!.Contains("\\mathrm{in}",StringComparison.Ordinal),
        "unit meaning cannot be discarded by text display");
});
Probe("original rational exclusion is retained as IR condition",()=>
{
    var input="where(div(sub(square(x),1),sub(x,1)),ne(x,1))";
    var result=CanonicalMathNotation.Parse(input);
    Require(result.IsAdmitted&&result.Expression is IrRestrictedExpression r&&
        r.Assumptions.Conditions.Length==1&&
        r.Assumptions.Conditions[0].Kind==IrRelationKind.NotEqual&&
        r.Assumptions.Conditions[0].Right.Value==ExactRational.One&&
        result.Latex!.Contains("\\ne",StringComparison.Ordinal),
        "original-domain hole must not be hidden during rendering");
});
Probe("semantic IR identity round-trips across text and transport",()=>
{
    var parsed=CanonicalMathNotation.Parse("where(add(x,1/2),ge(x,0))");
    Require(parsed.IsAdmitted,"source transport");
    var again=CanonicalMathNotation.Render(
        IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(parsed.Expression!)));
    Require(again.IsAdmitted&&
        again.CanonicalText==parsed.CanonicalText&&
        again.SemanticStructuralSha256==parsed.SemanticStructuralSha256,
        "exact semantic identity must survive IR transport");
});
Probe("a noncanonical fraction is a refusal without meaning loss",()=>
{
    var result=CanonicalMathNotation.Parse("2/4");
    Require(result.Status==CanonicalNotationStatus.NonCanonical&&
        !result.IsAdmitted&&result.Expression is null&&result.Latex is null,
        "noncanonical alias must not become an admitted expression");
});
Probe("package has no TeX engine, CAS or AURA/Decision runtime",()=>
{
    var result=CanonicalMathNotation.Parse(@"\input{evil}");
    Require(result.Status==CanonicalNotationStatus.InvalidInput&&
        !result.IsAdmitted&&result.Latex is null,
        "unsupported TeX macros are data refusals");
    var assemblies=new[]{typeof(CanonicalMathNotation).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!deps.Any(d=>
        d.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        d.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        d.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        d.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        d.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no provider or agent broker product dependency");
});
if(passed!=8)
    throw new InvalidOperationException("Exactly eight canonical notation NuGet-only oracles required.");
Console.WriteLine("dw.tools.math/m5-canonical-text-consumer/1.0 qualified; probes="+passed);
