using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name,Action check)
{
    try {check();passed++;}
    catch(Exception e){throw new InvalidOperationException("M4 exact pointwise derivative NuGet consumer: "+name,e);}
}
static void Require(bool yes,string message)
{
    if(!yes)throw new InvalidOperationException(message);
}
static IrExactScalar Number(long value)=>new(new ExactRational(value,1));
static IrNode Cube(IrSymbol x)=>IrApply.Create(IrOperation.Power,[x,Number(3)]);
static SymbolicDerivativePointResult Inspect(IrNode source,IrSymbol x,long point)=>
    SymbolicDerivativePointAnalysis.Analyze(source,x,new ExactRational(point,1));

var x=IrSymbol.Free("x","x");
var y=IrSymbol.Free("y","x");
var cubic=Cube(x);

Probe("cubic derivative at two is exact twelve",()=>
{
    var result=Inspect(cubic,x,2);
    Require(result.HasExactValue&&result.Value==new ExactRational(12,1) &&
        result.RuleId==SymbolicDerivativePointAnalysis.PolynomialRule,
        "D(x^3,2) is exactly twelve");
});
Probe("independent derivative target yields exact zero",()=>
{
    var result=Inspect(cubic,y,2);
    Require(result.HasExactValue&&result.Value==ExactRational.Zero &&
        result.VariableIdentity==y.Identity,"absent free derivative variable is zero");
});
Probe("absolute kink at zero is non-differentiable",()=>
{
    var abs=IrApply.Create(IrOperation.Abs,[x]);
    var result=Inspect(abs,x,0);
    Require(result.Status==SymbolicDerivativePointStatus.NonDifferentiable &&
        !result.Value.HasValue && result.RuleId is null,
        "no invented absolute derivative at the kink");
});
Probe("absolute derivative below zero is exact minus one",()=>
{
    var abs=IrApply.Create(IrOperation.Abs,[x]);
    var result=Inspect(abs,x,-2);
    Require(result.HasExactValue&&result.Value==new ExactRational(-1,1),
        "absolute left-branch slope is minus one");
});
Probe("square root below zero is outside real domain",()=>
{
    var sqrt=IrApply.Create(IrOperation.Sqrt,[x]);
    var result=Inspect(sqrt,x,-1);
    Require(result.Status==SymbolicDerivativePointStatus.OutsideRealDomain &&
        !result.Value.HasValue,"negative argument has no real square root derivative");
});
Probe("real sqrt of x squared below zero has negative slope",()=>
{
    var source=IrApply.Create(IrOperation.Sqrt,[
        IrApply.Create(IrOperation.Square,[x])]);
    var result=Inspect(source,x,-2);
    Require(result.HasExactValue&&result.Value==new ExactRational(-1,1),
        "sqrt(square(real x)) equals abs(x)");
});
Probe("dimensional quantity is explicitly refused",()=>
{
    var source=new IrQuantityLiteral(Number(2),"imperial.inch",
        UnitSystem.BritishImperial,DimensionVector.LengthDimension);
    var result=Inspect(source,x,2);
    Require(result.Status==SymbolicDerivativePointStatus.UnsupportedUnits &&
        !result.Value.HasValue,"no implicit derived unit arithmetic");
});
Probe("published product reference graph remains independent of CAS and agent broker",()=>
{
    var assemblies=new[]{typeof(SymbolicDerivativePointAnalysis).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!dependencies.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "Math public symbolics package cannot depend on a CAS or broker");
});
if(passed!=8)throw new InvalidOperationException("Exactly eight package-only pointwise derivative probes required.");
Console.WriteLine("dw.tools.math/m4-pointwise-derivative-consumer/0.5 qualified; probes="+passed);
