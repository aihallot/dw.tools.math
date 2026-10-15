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
        throw new InvalidOperationException("M4 exact solving NuGet consumer: "+label,error);
    }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}
static IrExactScalar N(long n)=>new(new ExactRational(n,1));
static IrExactScalar Q(long n,long d)=>new(new ExactRational(n,d));
static IrNode Square(IrSymbol x)=>IrApply.Create(IrOperation.Square,[x]);
static SymbolicRealSolvingResult Eq(IrNode a,IrNode b,IrSymbol x)=>
    SymbolicExactRealSolving.Solve(a,IrRelationKind.Equal,b,x);
static SymbolicRealSolvingResult Ge(IrNode a,IrNode b,IrSymbol x)=>
    SymbolicExactRealSolving.Solve(a,IrRelationKind.GreaterOrEqual,b,x);

var x=IrSymbol.Free("x","x");
Probe("x squared equals one has two exact real roots",()=>
{
    var r=Eq(Square(x),N(1),x);
    Require(r.IsQualified&&r.IsComplete&&
        r.Status==SymbolicRealSolvingStatus.Finite&&r.Roots.Length==2&&
        r.Roots[0]==new ExactRational(-1,1)&&r.Roots[1]==ExactRational.One,
        "x squared equals one must yield {-1,1}");
});
Probe("negative square target is provably empty only over reals",()=>
{
    var r=Eq(Square(x),N(-1),x);
    Require(r.IsQualified&&r.Status==SymbolicRealSolvingStatus.Empty&&
        r.Roots.IsEmpty,"real square cannot equal negative one");
});
Probe("irrational real roots remain unknown not empty",()=>
{
    var r=Eq(Square(x),N(2),x);
    Require(!r.IsQualified&&r.Status==SymbolicRealSolvingStatus.Unknown&&
        r.Roots.IsEmpty&&r.RuleId is null,
        "lack of rational radicals never proves no real roots");
});
Probe("rational square root is exact",()=>
{
    var r=Eq(Square(x),Q(4,9),x);
    Require(r.IsComplete&&r.Roots.Length==2&&
        r.Roots[0]==new ExactRational(-2,3)&&
        r.Roots[1]==new ExactRational(2,3),
        "positive rational square is exact without binary64");
});
Probe("lower closed real ray is admitted",()=>
{
    var r=Ge(x,N(2),x);
    Require(r.IsComplete&&r.Status==SymbolicRealSolvingStatus.LowerClosedRay&&
        r.ClosedBound==new ExactRational(2,1),"x >= 2 is a closed lower ray");
});
Probe("upper closed real ray is admitted",()=>
{
    var r=Ge(N(2),x,x);
    Require(r.IsComplete&&r.Status==SymbolicRealSolvingStatus.UpperClosedRay&&
        r.ClosedBound==new ExactRational(2,1),"2 >= x is a closed upper ray");
});
Probe("quantity input is a typed refusal without proof",()=>
{
    var measure=new IrQuantityLiteral(N(1),"imperial.inch",
        UnitSystem.BritishImperial,DimensionVector.LengthDimension);
    var r=Eq(x,measure,x);
    Require(!r.IsQualified&&r.Status==SymbolicRealSolvingStatus.UnsupportedUnits&&
        r.RuleId is null&&r.LeftSemanticSha256 is null,
        "unit-valued equations are unqualified");
});
Probe("packaged Math product graph is not an agent or external CAS",()=>
{
    var assemblies=new[]{typeof(SymbolicExactRealSolving).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!dependencies.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no non-Math CAS, decision or broker dependencies are admitted");
});
if(passed!=8)
    throw new InvalidOperationException("Exactly eight NuGet-only solving probes required.");
Console.WriteLine("dw.tools.math/m4-real-solving-consumer/0.5 qualified; probes="+passed);
