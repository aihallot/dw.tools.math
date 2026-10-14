using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name,Action test)
{
    try { test();passed++; }
    catch(Exception e) { throw new InvalidOperationException("M4 C03 isolated NuGet client: "+name,e); }
}
static void Require(bool yes,string message)
{
    if(!yes)throw new InvalidOperationException(message);
}
static IrExactScalar Number(long n)=>new(new ExactRational(n,1));
static IrNode Binomial(IrSymbol x,ExactRational a)=>
    IrApply.Create(IrOperation.Square,[
        IrApply.Create(IrOperation.Add,[x,new IrExactScalar(a)])]);
static IrNode Admitted(SymbolicTransformResult result)
{
    if(!result.IsAdmitted||result.Expression is null)
        throw new InvalidOperationException("Expected proven exact symbolic rule.");
    return result.Expression;
}

var x=IrSymbol.Free("x","x",IrScalarDomain.Real);
var binomial=Binomial(x,ExactRational.One);
var expanded=Admitted(SymbolicExactTransformations.ExpandSquare(binomial));
Probe("exact binomial expansion has three terms",()=>
{
    var e=expanded as IrApply;
    Require(e?.Operation==IrOperation.Add&&e.Arguments.Length==3,"exact three-term expansion");
    var twice=(IrExactScalar)((IrApply)e.Arguments[1]).Arguments[0];
    Require(twice.Value==new ExactRational(2,1),"coefficient 2");
});
Probe("factor recovers the initial exact polynomial",()=>
{
    var restored=Admitted(SymbolicExactTransformations.FactorSquare(expanded));
    Require(IrCanonicalJsonCodec.Encode(restored)==IrCanonicalJsonCodec.Encode(binomial),
        "canonical closed round-trip");
});
Probe("fractional binomial coefficients remain exact",()=>
{
    var half=new ExactRational(1,2);
    var outNode=Admitted(SymbolicExactTransformations.ExpandSquare(Binomial(x,half)));
    var sum=(IrApply)outNode;
    Require(((IrExactScalar)sum.Arguments[2]).Value==new ExactRational(1,4),
        "exact one quarter");
});
Probe("typed assumptions remain attached to expansion",()=>
{
    var source=new IrRestrictedExpression(binomial,IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.NotEqual,Number(1))]));
    var outNode=Admitted(SymbolicExactTransformations.ExpandSquare(source));
    Require(outNode is IrRestrictedExpression typed &&
        typed.Assumptions.Conditions[0].Kind==IrRelationKind.NotEqual &&
        typed.Assumptions.Conditions[0].Right.Value==ExactRational.One,
        "existing restriction cannot disappear");
});
Probe("substitution changes only a free symbol identity",()=>
{
    var y=IrSymbol.Free("y","x");
    var substituted=Admitted(SymbolicExactTransformations.SubstituteFree(
        IrApply.Create(IrOperation.Add,[x,Number(1)]),x,y));
    Require(IrCanonicalJsonCodec.Encode(substituted)==IrCanonicalJsonCodec.Encode(
        IrApply.Create(IrOperation.Add,[y,Number(1)])),"capture-free scope");
});
Probe("bound scope causes typed refusal, not capture",()=>
{
    var bound=IrSymbol.Bound("scope",0,"x");
    var input=IrApply.Create(IrOperation.Add,[bound,x]);
    Require(SymbolicExactTransformations.SubstituteFree(input,x,Number(3)).Status==
        SymbolicTransformStatus.Unsupported,"bound variable must be rejected");
});
Probe("real sqrt of square is absolute without a sign proof",()=>
{
    var source=IrApply.Create(IrOperation.Sqrt,[
        IrApply.Create(IrOperation.Square,[x])]);
    var output=Admitted(SymbolicExactTransformations.NormalizeRealSqrtSquare(
        source,IrAssumptionSet.Empty));
    Require(output is IrApply {Operation:IrOperation.Abs},"real absolute value");
});
Probe("product NuGet graph has no external CAS dependency",()=>
{
    var deps=typeof(SymbolicExactTransformations).Assembly.GetReferencedAssemblies()
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!deps.Any(n=>n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),"no third-party CAS/broker dependency");
});
if(passed!=8)throw new InvalidOperationException("Eight bounded local NuGet-only oracles required.");
Console.WriteLine("dw.tools.math/m4-exact-transform-consumer/0.5 qualified; probes="+passed);
