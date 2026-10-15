using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name,Action verify)
{
    try {verify();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException("M4 original-root audit NuGet consumer: "+name,error);
    }
}
static void Require(bool claim,string message)
{
    if(!claim)throw new InvalidOperationException(message);
}
static IrExactScalar N(long n)=>new(new ExactRational(n,1));
static IrNode Square(IrNode node)=>IrApply.Create(IrOperation.Square,[node]);
static IrNode Minus(IrNode a,IrNode b)=>IrApply.Create(IrOperation.Subtract,[a,b]);
static IrNode Divide(IrNode a,IrNode b)=>IrApply.Create(IrOperation.Divide,[a,b]);
static ExactRational R(long n)=>new(n,1);
var x=IrSymbol.Free("x","x");
SymbolicRootAuditResult At(IrNode left,IrNode right,params long[] candidates)=>
    SymbolicRootAudit.Verify(left,right,x,candidates.Select(R));

Probe("complete quadratic roots are both exactly substituted",()=>
{
    var result=At(Square(x),N(1),-1,1);
    Require(result.IsComplete&&result.VerifiedRoots.Length==2&&
        result.VerifiedRoots[0].Value==R(-1)&&
        result.VerifiedRoots[1].Value==R(1)&&
        !result.ProvenEmpty,"both roots and completeness are required");
});
Probe("one candidate is only a verified subset",()=>
{
    var result=At(Square(x),N(1),1);
    Require(result.Status==SymbolicRootAuditStatus.VerifiedSubset&&
        !result.IsComplete&&result.VerifiedRoots.Length==1,
        "one root cannot certify the missing second root");
});
Probe("double root is proved only for admitted square equation",()=>
{
    var result=At(Square(x),N(0),0);
    Require(result.IsComplete&&result.VerifiedRoots.Length==1&&
        result.VerifiedRoots[0].Multiplicity==2,
        "x squared equals zero has multiplicity two");
});
Probe("candidate at original removable quotient hole is excluded",()=>
{
    var original=Divide(Minus(Square(x),N(1)),Minus(x,N(1)));
    var result=At(original,N(0),-1,1);
    Require(result.Status==SymbolicRootAuditStatus.VerifiedSubset&&
        result.VerifiedRoots.Length==1&&result.VerifiedRoots[0].Value==R(-1)&&
        result.ExcludedFromDomain.Length==1&&
        result.ExcludedFromDomain[0]==R(1)&&!result.IsComplete,
        "do not silently fill x=1 original quotient hole");
});
Probe("no rational candidate does not prove irrational roots are absent",()=>
{
    var result=At(Square(x),N(2),1);
    Require(result.Status==SymbolicRootAuditStatus.NoVerifiedCandidates&&
        !result.ProvenEmpty&&!result.IsComplete&&
        result.RejectedCandidates.Length==1,
        "unknown positive nonsquare rational root remains possible");
});
Probe("source restriction is tested before candidate acceptance",()=>
{
    var condition=IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.NotEqual,N(1))]);
    var restricted=new IrRestrictedExpression(Square(x),condition);
    var result=At(restricted,N(1),-1,1);
    Require(result.VerifiedRoots.Length==1&&
        result.VerifiedRoots[0].Value==R(-1)&&
        result.ExcludedFromDomain.Length==1&&
        result.ExcludedFromDomain[0]==R(1),
        "a known restriction is not silently erased");
});
Probe("dimensional literal remains an explicit scientific refusal",()=>
{
    var measure=new IrQuantityLiteral(N(1),"imperial.inch",
        UnitSystem.BritishImperial,DimensionVector.LengthDimension);
    var result=At(x,measure,1);
    Require(result.Status==SymbolicRootAuditStatus.UnsupportedUnits&&
        !result.HasVerifiedRoots&&result.RuleId is null,
        "no implicit dimensional exactness");
});
Probe("NuGet-only dependencies remain Math-owned without CAS or broker",()=>
{
    var assemblies=new[]{typeof(SymbolicRootAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var dependencies=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!dependencies.Any(name=>
        name.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        name.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        name.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "math must not require an external CAS or sibling agent");
});
if(passed!=8)
    throw new InvalidOperationException("Exactly eight NuGet-only original-root audit probes required.");
Console.WriteLine("dw.tools.math/m4-original-root-audit-consumer/0.5 qualified; probes="+passed);
