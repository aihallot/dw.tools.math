using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string id,Action test)
{
    try {test();passed++;}
    catch(Exception e){throw new InvalidOperationException("M4-T2 package-only admission: "+id,e);}
}
static void Require(bool yes,string why)
{
    if(!yes)throw new InvalidOperationException(why);
}
static IrExactScalar Number(long n)=>new(new ExactRational(n,1));
static IrNode Rational(IrSymbol x,ExactRational c)
{
    var numerator=IrApply.Create(IrOperation.Subtract,[
        IrApply.Create(IrOperation.Square,[x]),
        new IrExactScalar(c*c)]);
    var denominator=IrApply.Create(IrOperation.Subtract,[x,new IrExactScalar(c)]);
    return IrApply.Create(IrOperation.Divide,[numerator,denominator]);
}
static SymbolicProviderAdmissionResult Admit(IrNode source,object? candidate)=>
    SymbolicProviderResultAdmission.TryAdmit(source,candidate);

var x=IrSymbol.Free("x","x");
var input=Rational(x,ExactRational.One);
var locallyProven=SymbolicRationalCancellationBridge.TryCancel(input);
if(locallyProven.Conditional is null)throw new InvalidOperationException("Math local exact proof unavailable.");

Probe("typed proof matches local IR and is admissible",()=>
{
    var r=Admit(input,locallyProven.Conditional);
    Require(r.IsAdmitted&&r.Conditional is not null,"admitted typed result");
    Require(IrCanonicalJsonCodec.Encode(r.Conditional!)==
        IrCanonicalJsonCodec.Encode(locallyProven.Conditional),"local canonical IR equality");
});
Probe("accepted output is a fresh local object",()=>
{
    var r=Admit(input,locallyProven.Conditional);
    Require(!ReferenceEquals(r.Conditional,locallyProven.Conditional),
        "do not return the supplied untrusted candidate");
});
Probe("simplification must not provide a value at the original pole",()=>
{
    var r=Admit(input,locallyProven.Conditional);
    var typed=SymbolicRationalCancellationBridge.TryCancel(input);
    var excluded=SymbolicRationalCancellationBridge.EvaluateExactAt(typed,x,ExactRational.One);
    Require(r.IsAdmitted&&excluded.Status==SymbolicPointStatus.Excluded&&
        excluded.Value is null,"x=1 exclusion");
});
Probe("exact original valid point is preserved",()=>
{
    var value=SymbolicRationalCancellationBridge.EvaluateExactAt(locallyProven,
        x,new ExactRational(2,1));
    Require(value.Status==SymbolicPointStatus.ExactValue &&
        value.Value==new ExactRational(3,1),"exact (4-1)/(2-1)=3");
});
Probe("opaque provider condition text is never admitted",()=>
{
    var bad=Admit(input,"(x+1) provided x != 1");
    Require(bad.Status==SymbolicProviderAdmissionStatus.UnrepresentableOutput &&
        bad.Conditional is null,"string never proves a predicate");
});
Probe("unconditional typed expression remains refused",()=>
{
    var bare=IrApply.Create(IrOperation.Add,[x,Number(1)]);
    var refused=Admit(input,bare);
    Require(refused.Status==SymbolicProviderAdmissionStatus.UnrepresentableOutput,
        "exact expression alone is missing a restriction");
});
Probe("wrong condition is typed mismatch not conditional success",()=>
{
    var wrong=new IrRestrictedExpression(locallyProven.Conditional!.Expression,
        IrAssumptionSet.Create([new IrRelation(x,IrRelationKind.NotEqual,Number(0))]));
    var result=Admit(input,wrong);
    Require(result.Status==SymbolicProviderAdmissionStatus.ConditionalMismatch &&
        result.Conditional is null,"original x!=1 cannot become x!=0");
});
Probe("no external symbolic provider is a product runtime reference",()=>
{
    var deps=typeof(SymbolicProviderResultAdmission).Assembly.GetReferencedAssemblies()
        .Select(a=>a.Name??string.Empty);
    Require(!deps.Any(name=>name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),"provider-independent package");
});
if(passed!=8)throw new InvalidOperationException("All eight package-only admission probes must execute.");
Console.WriteLine("dw.tools.math/m4-provider-admission-consumer/0.5 qualified; probes="+passed);
