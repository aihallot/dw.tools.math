using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string label,Action check)
{
    try {check();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M4 calculus boundary package-only consumer: "+label,error);
    }
}
static void Require(bool claim,string message)
{
    if(!claim)throw new InvalidOperationException(message);
}
static ExactRational R(long n,long d=1)=>new(n,d);
static IrExactScalar N(long n)=>new(R(n));
static IrNode Sub(IrNode a,IrNode b)=>
    IrApply.Create(IrOperation.Subtract,[a,b]);
static IrNode Sq(IrNode node)=>IrApply.Create(IrOperation.Square,[node]);
static IrNode Div(IrNode a,IrNode b)=>
    IrApply.Create(IrOperation.Divide,[a,b]);
var x=IrSymbol.Free("x","x");
var simple=Div(N(1),Sub(x,N(1)));
var cancel=Div(Sub(Sq(x),N(1)),Sub(x,N(1)));
SymbolicCalculusBoundaryResult Integral(IrNode source,long a,long b)=>
    SymbolicRealCalculusBoundaries.AnalyzeIntegral(source,x,R(a),R(b));
SymbolicCalculusBoundaryResult Limit(IrNode source,long p)=>
    SymbolicRealCalculusBoundaries.AnalyzeLimit(source,x,R(p));

Probe("previously qualified polynomial integral remains exact",()=>
{
    var r=Integral(x,0,1);
    Require(r.HasExactValue&&r.ExactValue==R(1,2)&&
        r.ConstantConvention==SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
        "polynomial integral must be exact and retain +C convention");
});
Probe("nonzero simple pole on a closed interval has no integral claim",()=>
{
    var r=Integral(simple,0,2);
    Require(r.Status==SymbolicCalculusBoundaryStatus.SimplePole&&
        r.HasQualifiedBoundary&&!r.HasExactValue&&
        r.ExceptionalPoint==R(1)&&r.ExactValue is null,
        "integral crossing a simple pole must not return a number");
});
Probe("removable original-domain hole is distinct from a pole",()=>
{
    var r=Integral(cancel,0,2);
    Require(r.Status==SymbolicCalculusBoundaryStatus.OriginalDomainHole&&
        r.ExceptionalPoint==R(1)&&r.HasQualifiedBoundary&&
        !r.HasExactValue,
        "do not integrate through an original-domain hole as a proper integral");
});
Probe("hole-free cancelled rational identity integrates exactly",()=>
{
    var r=Integral(cancel,2,3);
    Require(r.HasExactValue&&r.ExactValue==R(7,2)&&
        r.RuleId==SymbolicRealCalculusBoundaries.HoleFreeIntegralRule&&
        r.ConstantConvention==SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
        "original hole exclusion must be proven before exact substitution");
});
Probe("removable limit is exact but not the original function value",()=>
{
    var r=Limit(cancel,1);
    Require(r.Status==SymbolicCalculusBoundaryStatus.ExactRemovableLimit&&
        r.HasExactValue&&r.HasQualifiedBoundary&&
        r.ExactValue==R(2)&&r.ExceptionalPoint==R(1),
        "finite removable limit and undefined original point stay distinct");
});
Probe("rational finite limit away from pole is exact",()=>
{
    var r=Limit(simple,2);
    Require(r.HasExactValue&&r.ExactValue==R(1)&&
        r.Status==SymbolicCalculusBoundaryStatus.Exact,
        "rational function is continuous away from simple pole");
});
Probe("unsupported radical stays unqualified, not numerical",()=>
{
    var radical=IrApply.Create(IrOperation.Sqrt,[x]);
    var r=Integral(radical,0,1);
    Require(r.Status==SymbolicCalculusBoundaryStatus.UnknownConvergence&&
        r.ExactValue is null&&r.RuleId is null,
        "unknown analytic shape must not be promoted into exact calculus");
});
Probe("local product packages do not require agent or CAS providers",()=>
{
    var assemblies=new[]{typeof(SymbolicRealCalculusBoundaries).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!deps.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "product dependency graph cannot include unqualified CAS or agent broker");
});
if(passed!=8)
    throw new InvalidOperationException("Exactly eight independent NuGet-only calculus boundary probes required.");
Console.WriteLine("dw.tools.math/m4-calculus-boundary-consumer/0.5 qualified; probes="+passed);
