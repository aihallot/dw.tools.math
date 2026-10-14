using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name,Action action)
{
    try { action(); passed++; }
    catch(Exception e) { throw new InvalidOperationException("M4 NuGet-only IR consumer: "+name,e); }
}
static void Require(bool yes,string message)
{
    if(!yes)throw new InvalidOperationException(message);
}
static IrExactScalar Exact(ExactRational r)=>new(r);
static IrNode Input(IrSymbol x,ExactRational c)
{
    var numerator=IrApply.Create(IrOperation.Subtract,[
        IrApply.Create(IrOperation.Square,[x]),Exact(c*c)]);
    var denominator=IrApply.Create(IrOperation.Subtract,[x,Exact(c)]);
    return IrApply.Create(IrOperation.Divide,[numerator,denominator]);
}
static SymbolicCancellationResult Cancel(IrSymbol x,ExactRational c)=>
    SymbolicRationalCancellationBridge.TryCancel(Input(x,c));
static SymbolicPointResult At(SymbolicCancellationResult r,IrSymbol x,ExactRational value)=>
    SymbolicRationalCancellationBridge.EvaluateExactAt(r,x,value);

var x=IrSymbol.Free("x","x",IrScalarDomain.Real);
var one=ExactRational.One;
var result=Cancel(x,one);

Probe("typed restriction survives exact cancellation",() =>
    Require(result.IsAdmitted && result.Conditional is not null &&
        result.Status==SymbolicCancellationStatus.Admitted,"typed accepted result"));
Probe("old pole does not become an exact numeric value",() =>
{
    var excluded=At(result,x,one);
    Require(excluded.Status==SymbolicPointStatus.Excluded &&
        excluded.Value is null && !excluded.HasExactValue,"x=1 must remain undefined");
});
Probe("point two produces exact three",()=>
{
    var observed=At(result,x,new ExactRational(2,1));
    Require(observed.Status==SymbolicPointStatus.ExactValue &&
        observed.Value==new ExactRational(3,1),"hand oracle (4-1)/(2-1)=3");
});
Probe("point zero produces exact one",()=>
    Require(At(result,x,ExactRational.Zero).Value==one,
        "hand oracle (-1)/(-1)=1"));
Probe("other integer singularity stays excluded",()=>
{
    var other=Cancel(x,new ExactRational(2,1));
    Require(At(other,x,new ExactRational(2,1)).Status==SymbolicPointStatus.Excluded &&
        At(other,x,new ExactRational(3,1)).Value==new ExactRational(5,1),
        "other pole and valid point");
});
Probe("fractional pole is exact rather than binary64",()=>
{
    var half=new ExactRational(1,2);
    var halfResult=Cancel(x,half);
    Require(At(halfResult,x,half).Status==SymbolicPointStatus.Excluded &&
        At(halfResult,x,ExactRational.One).Value==new ExactRational(3,2),
        "fractional exclusions and values");
});
Probe("canonical IR transport retains non-equality condition",()=>
{
    var canonical=IrCanonicalJsonCodec.Encode(result.Conditional!);
    var decoded=IrCanonicalJsonCodec.Decode(canonical);
    Require(decoded is IrRestrictedExpression restrictions &&
        restrictions.Assumptions.Conditions.Length==1 &&
        restrictions.Assumptions.Conditions[0].Kind==IrRelationKind.NotEqual &&
        restrictions.Assumptions.Conditions[0].Right.Value==one &&
        restrictions.Assumptions.Conditions[0].Symbol.Identity==x.Identity,
        "canonical typed restriction");
});
Probe("binary product has no external CAS decision or broker dependency",()=>
{
    var assemblies=new[]{typeof(SymbolicRationalCancellationBridge).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var names=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??"").ToArray();
    Require(!names.Any(name=>name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),"product dependency isolation");
});
if(passed!=8)throw new InvalidOperationException("Exactly eight package-only IR oracles are required.");
Console.WriteLine("dw.tools.math/m4-ir-bridge-consumer/0.5 qualified; probes="+passed);
