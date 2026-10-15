using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name,Action assertion)
{
    try {assertion();passed++;}
    catch(Exception error)
    {throw new InvalidOperationException("M4 exact derivative NuGet consumer: "+name,error);}
}
static void Require(bool check,string message)
{
    if(!check)throw new InvalidOperationException(message);
}
static IrExactScalar Number(long n)=>new(new ExactRational(n,1));
static IrNode Cube(IrSymbol symbol)=>
    IrApply.Create(IrOperation.Power,[symbol,Number(3)]);
static IrNode RequiredDerivative(SymbolicDerivativeResult result)
{
    if(!result.IsAdmitted || result.Derivative is not IrNode derivative)
        throw new InvalidOperationException("Qualified exact derivative missing.");
    return derivative;
}

var x=IrSymbol.Free("x","x");
var y=IrSymbol.Free("y","x");
var cubic=Cube(x);
var derivative=SymbolicExactDifferentiation.TryDifferentiate(cubic,x);

Probe("cubic derivative equals three times x squared",()=>
{
    var expected=IrApply.Create(IrOperation.Multiply,[
        Number(3),IrApply.Create(IrOperation.Power,[x,Number(2)])]);
    Require(IrCanonicalJsonCodec.Encode(RequiredDerivative(derivative))==
        IrCanonicalJsonCodec.Encode(expected),"d(x^3)/dx=3x^2 exact oracle");
});
Probe("absent symbol differentiation gives exact zero",()=>
{
    var result=SymbolicExactDifferentiation.TryDifferentiate(cubic,y);
    Require(RequiredDerivative(result) is IrExactScalar exact &&
        exact.Value==ExactRational.Zero,"d(x^3)/dy must be zero");
});
Probe("real domain and codomain are explicit on a formal function",()=>
{
    var f=new SymbolicRealFunction(x,cubic);
    Require(f.Domain==IrScalarDomain.Real &&
        f.Codomain==IrScalarDomain.Real &&
        f.Parameter.Identity==x.Identity &&
        SymbolicExactDifferentiation.TryDifferentiate(f).IsAdmitted,
        "closed real function and differentiated output");
});
Probe("canonical original and derivative-variable provenance survives",()=>
{
    Require(derivative.ProofRule==SymbolicExactDifferentiation.Rule &&
        derivative.SourceSemanticSha256==
            IrCanonicalHashes.SemanticStructuralSha256(cubic) &&
        derivative.DifferentiationVariableIdentity==x.Identity,
        "local exact formal rule ID and source/wrt identities");
});
Probe("quantity units are explicitly refused rather than implicitly divided",()=>
{
    var quantity=new IrQuantityLiteral(Number(2),"imperial.inch",
        UnitSystem.BritishImperial,DimensionVector.LengthDimension);
    var response=SymbolicExactDifferentiation.TryDifferentiate(quantity,x);
    Require(response.Status==SymbolicDifferentiationStatus.UnsupportedUnits &&
        response.Derivative is null && response.ProofRule is null,
        "units require separate dimensional differentiation contract");
});
Probe("non-differentiable absolute shape has no false derivative",()=>
{
    var response=SymbolicExactDifferentiation.TryDifferentiate(
        IrApply.Create(IrOperation.Abs,[x]),x);
    Require(response.Status==SymbolicDifferentiationStatus.Unsupported &&
        response.Derivative is null,"piecewise nondifferentiable point unqualified");
});
Probe("bounded polynomial exponent refuses expansion outside admitted family",()=>
{
    var source=IrApply.Create(IrOperation.Power,[x,Number(9)]);
    var response=SymbolicExactDifferentiation.TryDifferentiate(source,x);
    Require(response.Status==SymbolicDifferentiationStatus.BudgetExceeded &&
        response.Derivative is null,"degree nine is beyond qualified degree eight");
});
Probe("consumer package references no external CAS or broker assemblies",()=>
{
    var assemblies=new[]{typeof(SymbolicExactDifferentiation).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!dependencies.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "provider-free transitive Math package graph");
});
if(passed!=8)throw new InvalidOperationException("Eight scoped, independent derivative oracles required.");
Console.WriteLine("dw.tools.math/m4-exact-derivative-consumer/0.5 qualified; probes="+passed);
