using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string title,Action check)
{
    try {check();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M4 polynomial calculus NuGet consumer: "+title,error);
    }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}
static ExactRational R(long n,long d=1)=>new(n,d);
static IrExactScalar N(long n)=>new(R(n));
static IrNode Square(IrSymbol x)=>IrApply.Create(IrOperation.Square,[x]);
static SymbolicPolynomialIntegralResult Integrate(
    IrNode source,IrSymbol x,long lo,long hi)=>
    SymbolicExactPolynomialCalculus.Integrate(source,x,R(lo),R(hi));
var x=IrSymbol.Free("x","x");

Probe("x integral over zero one is exactly one half",()=>
{
    var result=Integrate(x,x,0,1);
    Require(result.IsExact&&result.DefiniteValue==R(1,2)
        &&result.ConstantConvention==
            SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
        "integral of x must be 1/2 and arbitrary constant must remain explicit");
});
Probe("square over zero one is exactly one third",()=>
{
    var result=Integrate(Square(x),x,0,1);
    Require(result.IsExact&&result.DefiniteValue==R(1,3),
        "no numerical approximation is permitted for x squared integral");
});
Probe("formal derivative of primitive stays qualified",()=>
{
    var result=Integrate(x,x,0,1);
    var derivative=SymbolicExactDifferentiation.TryDifferentiate(
        result.ZeroConstantPrimitive!,x);
    Require(result.IsExact&&derivative.IsAdmitted&&
        result.DerivativeRuleId==SymbolicExactDifferentiation.Rule,
        "formal derivative qualification is mandatory");
});
Probe("exact pointwise slope of primitive recovers integrand",()=>
{
    var result=Integrate(x,x,0,1);
    var derivative=SymbolicDerivativePointAnalysis.Analyze(
        result.ZeroConstantPrimitive!,x,R(2));
    Require(derivative.HasExactValue&&derivative.Value==R(2),
        "derivative of x primitive at two is exactly two");
});
Probe("reverse oriented bounds preserve negative signed integral",()=>
{
    var result=Integrate(x,x,1,0);
    Require(result.IsExact&&result.DefiniteValue==R(-1,2),
        "reverse integral orientation must be signed");
});
Probe("finite real polynomial limit is continuity-based exact",()=>
{
    var result=SymbolicExactPolynomialCalculus.FiniteLimit(
        Square(x),x,R(2));
    Require(result.IsExact&&result.Value==R(4)&&
        result.RuleId==SymbolicExactPolynomialCalculus.LimitRule,
        "finite polynomial limit at two must be exactly four");
});
Probe("retained original restrictions must be refused",()=>
{
    var assumptions=IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.GreaterOrEqual,N(0))]);
    var restricted=new IrRestrictedExpression(Square(x),assumptions);
    var result=Integrate(restricted,x,0,1);
    Require(!result.IsExact&&
        result.Status==SymbolicPolynomialCalculusStatus.UnsupportedAssumptions&&
        result.ZeroConstantPrimitive is null&&result.RuleId is null,
        "conditional source is outside admitted polynomial domain");
});
Probe("NuGet-only package contains no CAS or agent broker dependencies",()=>
{
    var assemblies=new[]{typeof(SymbolicExactPolynomialCalculus).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!dependencies.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no foreign CAS or agent broker dependency is admitted");
});
if(passed!=8)
    throw new InvalidOperationException("Expected exactly eight NuGet-only Math calculus probes.");
Console.WriteLine("dw.tools.math/m4-exact-polynomial-calculus-consumer/0.5 qualified; probes="+passed);
