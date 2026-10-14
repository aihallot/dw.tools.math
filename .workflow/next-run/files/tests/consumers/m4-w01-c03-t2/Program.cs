using System;
using System.Linq;
using System.Threading;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string name, Action assertion)
{
    try { assertion(); passed++; }
    catch(Exception error)
    { throw new InvalidOperationException("M4 bounded NuGet-only consumer: "+name,error); }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}
static IrExactScalar Number(long value)=>new(new ExactRational(value,1));
static IrNode Binomial(IrSymbol x)=>
    IrApply.Create(IrOperation.Square,[
        IrApply.Create(IrOperation.Add,[x,Number(1)])]);

var x=IrSymbol.Free("x","x");
var source=Binomial(x);

Probe("exact bounded expansion keeps three canonical terms",()=>
{
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,source,new SymbolicWorkBudget());
    if(!r.IsAdmitted || r.Expression is not IrApply {Operation:IrOperation.Add} sum ||
        sum.Arguments.Length!=3)
        throw new InvalidOperationException("Missing exact three-term binomial polynomial.");
    var middle=(IrApply)sum.Arguments[1];
    var coefficient=(IrExactScalar)middle.Arguments[0];
    Require(coefficient.Value==new ExactRational(2,1),
        "exact binomial coefficient was not 2");
});
Probe("provenance identifies original IR without foreign source",()=>
{
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,source,new SymbolicWorkBudget());
    Require(r.IsAdmitted &&
        r.ProofRule==SymbolicExactTransformations.ExpandRule &&
        r.SourceSemanticSha256==IrCanonicalHashes.SemanticStructuralSha256(source),
        "the local rule/source proof is not independently attributable");
});
Probe("node/work budgets refuse otherwise admitted rule",()=>
{
    var budget=new SymbolicWorkBudget(maximumWorkUnits:10);
    var r=SymbolicBoundedExecution.Execute(SymbolicBoundedOperation.ExpandSquare,
        source,budget);
    Require(r.Status==SymbolicBoundedStatus.BudgetExceeded &&
        r.Expression is null && r.ProofRule is null,
        "work budget cannot be represented as mathematical success");
});
Probe("pre-cancelled computation cannot report a result",()=>
{
    using var cts=new CancellationTokenSource();
    cts.Cancel();
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,source,
        new SymbolicWorkBudget(),cancellationToken:cts.Token);
    Require(r.Status==SymbolicBoundedStatus.Cancelled &&
        r.Expression is null,"cancelled means no algebraic result");
});
Probe("expired soft budget is an explicit refused result",()=>
{
    var budget=new SymbolicWorkBudget(maximumElapsed:TimeSpan.Zero);
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,source,budget);
    Require(r.Status==SymbolicBoundedStatus.DeadlineExceeded &&
        !r.IsAdmitted,"deadline cannot be disguised as complete proof");
});
Probe("bound symbol substitution refuses capture rather than renaming",()=>
{
    var bound=IrSymbol.Bound("scope",0,"x");
    var input=IrApply.Create(IrOperation.Add,[bound,x]);
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.SubstituteFree,input,new SymbolicWorkBudget(),
        symbol:x,replacement:Number(3));
    Require(r.Status==SymbolicBoundedStatus.Unsupported,
        "unknown binder conditions must be explicitly refused");
});
Probe("real square root stays absolute without a sign proof",()=>
{
    var sqrt=IrApply.Create(IrOperation.Sqrt,[
        IrApply.Create(IrOperation.Square,[x])]);
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.NormalizeRealSqrtSquare,sqrt,new SymbolicWorkBudget());
    Require(r.IsAdmitted && r.Expression is IrApply {Operation:IrOperation.Abs},
        "sqrt(x^2) must be abs(x), not x by default");
});
Probe("production package does not transitively reference CAS or agent broker",()=>
{
    var assemblies=new[]{typeof(SymbolicBoundedExecution).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??"").ToArray();
    Require(!deps.Any(d=>d.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        d.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        d.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        d.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        d.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "product graph should be provider-neutral");
});
if(passed!=8)throw new InvalidOperationException("Exactly eight scoped package-only scientific probes are required.");
Console.WriteLine("dw.tools.math/m4-symbolic-budget-consumer/0.5 qualified; probes="+passed);
