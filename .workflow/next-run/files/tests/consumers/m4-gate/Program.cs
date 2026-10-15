using System;
using System.Linq;
using System.Threading;
using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Symbolics;

var passed=0;
void Probe(string label,Action oracle)
{
    try {oracle();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException("M4 isolated NuGet gate: "+label,error);
    }
}
static void Require(bool claim,string why)
{
    if(!claim)throw new InvalidOperationException(why);
}
static ExactRational R(long numerator,long denominator=1)=>
    new(numerator,denominator);
static IrExactScalar N(long n)=>new(R(n));
static IrNode Add(params IrNode[] nodes)=>IrApply.Create(IrOperation.Add,nodes);
static IrNode Sub(IrNode a,IrNode b)=>IrApply.Create(IrOperation.Subtract,[a,b]);
static IrNode Mul(IrNode a,IrNode b)=>IrApply.Create(IrOperation.Multiply,[a,b]);
static IrNode Square(IrNode a)=>IrApply.Create(IrOperation.Square,[a]);
static IrNode Power(IrNode a,long n)=>IrApply.Create(IrOperation.Power,[a,N(n)]);
static IrNode Divide(IrNode a,IrNode b)=>IrApply.Create(IrOperation.Divide,[a,b]);
var x=IrSymbol.Free("x","x");
var y=IrSymbol.Free("y","y");
var squared=Square(x);
var squareBinomial=Square(Add(x,N(1)));
var rational=Divide(Sub(squared,N(1)),Sub(x,N(1)));

Probe("one third plus one sixth remains exactly one half",()=>
    Require(R(1,3)+R(1,6)==R(1,2),"exact rational canonical arithmetic"));

Probe("qualified rational cancellation preserves original point restriction",()=>
{
    var cancelled=SymbolicRationalCancellationBridge.TryCancel(rational);
    Require(cancelled.IsAdmitted&&cancelled.Conditional is not null,
        "typed original-domain cancellation");
    var atHole=SymbolicRationalCancellationBridge.EvaluateExactAt(cancelled,x,R(1));
    Require(atHole.Status==SymbolicPointStatus.Excluded&&!atHole.HasExactValue,
        "x=1 is still undefined in original quotient");
});
Probe("rational cancellation evaluated at two is exactly three",()=>
{
    var q=SymbolicRationalCancellationBridge.TryCancel(rational);
    var r=SymbolicRationalCancellationBridge.EvaluateExactAt(q,x,R(2));
    Require(r.HasExactValue&&r.Value==R(3),"original-domain x=2 value");
});
Probe("binomial square expands by admitted exact rule",()=>
{
    var expanded=SymbolicExactTransformations.ExpandSquare(squareBinomial);
    Require(expanded.IsAdmitted&&expanded.ProofRule==
        SymbolicExactTransformations.ExpandRule&&
        expanded.Expression is IrApply {Operation:IrOperation.Add} add&&
        add.Arguments.Length==3,"closed exact binomial expansion");
});
Probe("canonical expanded square factors under admitted rule",()=>
{
    var expanded=SymbolicExactTransformations.ExpandSquare(squareBinomial);
    var factored=SymbolicExactTransformations.FactorSquare(expanded.Expression!);
    Require(factored.IsAdmitted&&
        factored.ProofRule==SymbolicExactTransformations.FactorRule,
        "inverse closed binomial factorization");
});
Probe("real sqrt of square is abs without nonnegative assumption",()=>
{
    var radical=IrApply.Create(IrOperation.Sqrt,[squared]);
    var r=SymbolicExactTransformations.NormalizeRealSqrtSquare(
        radical,IrAssumptionSet.Empty);
    Require(r.IsAdmitted&&r.Expression is IrApply {Operation:IrOperation.Abs},
        "real sqrt(x²) is |x|");
});
Probe("real sqrt square admits x only when nonnegative is asserted",()=>
{
    var radical=IrApply.Create(IrOperation.Sqrt,[squared]);
    var assumptions=IrAssumptionSet.Create([
        new IrRelation(x,IrRelationKind.GreaterOrEqual,N(0))]);
    var r=SymbolicExactTransformations.NormalizeRealSqrtSquare(
        radical,assumptions);
    Require(r.IsAdmitted&&
        r.Expression is IrRestrictedExpression restriction &&
        restriction.Expression is IrSymbol s&&s.Identity==x.Identity&&
        restriction.Assumptions.Conditions.Length==1,
        "real nonnegative branch and its retained assumption are necessary");
});
Probe("free-symbol substitution rejects no meaning but changes exact IR",()=>
{
    var r=SymbolicExactTransformations.SubstituteFree(
        Add(x,N(1)),x,N(2));
    Require(r.IsAdmitted&&r.ProofRule==
        SymbolicExactTransformations.SubstitutionRule,
        "capture-free bounded substitution admission");
});
Probe("bounded execution admits a previously qualified rule",()=>
{
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,
        squareBinomial,new SymbolicWorkBudget());
    Require(r.IsAdmitted&&r.ProofRule==SymbolicExactTransformations.ExpandRule,
        "bounded local symbolic execution");
});
Probe("cancelled bounded execution has no expression or proof",()=>
{
    using var source=new CancellationTokenSource();
    source.Cancel();
    var r=SymbolicBoundedExecution.Execute(
        SymbolicBoundedOperation.ExpandSquare,
        squareBinomial,new SymbolicWorkBudget(),cancellationToken:source.Token);
    Require(r.Status==SymbolicBoundedStatus.Cancelled&&!r.IsAdmitted&&
        r.Expression is null&&r.ProofRule is null,
        "managed cancellation is not a mathematical success");
});
Probe("formal derivative of cubic is independently admitted",()=>
{
    var r=SymbolicExactDifferentiation.TryDifferentiate(Power(x,3),x);
    Require(r.IsAdmitted&&r.ProofRule==SymbolicExactDifferentiation.Rule,
        "formal cubic derivative rule");
});
Probe("derivative against an independent free real target is zero",()=>
{
    var r=SymbolicExactDifferentiation.TryDifferentiate(Power(x,3),y);
    Require(r.IsAdmitted&&r.Derivative is IrExactScalar zero&&
        zero.Value==ExactRational.Zero,"derivative of independent variable");
});
Probe("pointwise cubic derivative at two is twelve",()=>
{
    var r=SymbolicDerivativePointAnalysis.Analyze(Power(x,3),x,R(2));
    Require(r.HasExactValue&&r.Value==R(12),"exact pointwise derivative");
});
Probe("real absolute value kink is non-differentiable",()=>
{
    var abs=IrApply.Create(IrOperation.Abs,[x]);
    var r=SymbolicDerivativePointAnalysis.Analyze(abs,x,R(0));
    Require(r.Status==SymbolicDerivativePointStatus.NonDifferentiable&&
        !r.HasExactValue,"no invented derivative at kink");
});
Probe("sqrt at a negative real point is outside real domain",()=>
{
    var radical=IrApply.Create(IrOperation.Sqrt,[x]);
    var r=SymbolicDerivativePointAnalysis.Analyze(radical,x,R(-1));
    Require(r.Status==SymbolicDerivativePointStatus.OutsideRealDomain,
        "real radicand domain restriction");
});
Probe("real square equals one has two exact roots",()=>
{
    var r=SymbolicExactRealSolving.Solve(squared,IrRelationKind.Equal,N(1),x);
    Require(r.IsComplete&&r.Status==SymbolicRealSolvingStatus.Finite&&
        r.Roots.Length==2&&r.Roots[0]==R(-1)&&r.Roots[1]==R(1),
        "ordered complete exact rational solution set");
});
Probe("negative square target is empty over the real domain",()=>
{
    var r=SymbolicExactRealSolving.Solve(squared,IrRelationKind.Equal,N(-1),x);
    Require(r.IsComplete&&r.Status==SymbolicRealSolvingStatus.Empty&&
        r.Roots.IsEmpty,"negative square has no real roots");
});
Probe("irrational real roots are unknown not empty",()=>
{
    var r=SymbolicExactRealSolving.Solve(squared,IrRelationKind.Equal,N(2),x);
    Require(!r.IsComplete&&r.Status==SymbolicRealSolvingStatus.Unknown,
        "unknown is distinct from mathematically empty");
});
Probe("real x>=2 has one qualified closed lower ray",()=>
{
    var r=SymbolicExactRealSolving.Solve(x,IrRelationKind.GreaterOrEqual,N(2),x);
    Require(r.IsComplete&&r.Status==SymbolicRealSolvingStatus.LowerClosedRay&&
        r.ClosedBound==R(2),"closed ray instead of arbitrary numerical sampling");
});
Probe("root audit validates a complete exact quadratic candidate list",()=>
{
    var r=SymbolicRootAudit.Verify(squared,N(1),x,[R(-1),R(1)]);
    Require(r.IsComplete&&r.VerifiedRoots.Length==2&&
        r.VerifiedRoots.All(root=>root.Multiplicity==1),
        "original equality candidate completeness and multiplicity");
});
Probe("missing one quadratic candidate is only a verified subset",()=>
{
    var r=SymbolicRootAudit.Verify(squared,N(1),x,[R(1)]);
    Require(r.Status==SymbolicRootAuditStatus.VerifiedSubset&&!r.IsComplete,
        "no guessed missing root");
});
Probe("an original rational cancellation hole rejects candidate one",()=>
{
    var r=SymbolicRootAudit.Verify(rational,N(0),x,[R(-1),R(1)]);
    Require(r.Status==SymbolicRootAuditStatus.VerifiedSubset&&
        r.ExcludedFromDomain.Length==1&&r.ExcludedFromDomain[0]==R(1)&&
        r.VerifiedRoots.Length==1&&r.VerifiedRoots[0].Value==R(-1),
        "original domain and exact root audit");
});
Probe("integral of x on zero one is exactly one half",()=>
{
    var r=SymbolicExactPolynomialCalculus.Integrate(x,x,R(0),R(1));
    Require(r.IsExact&&r.DefiniteValue==R(1,2)&&
        r.ConstantConvention==
        SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
        "proper exact rational integral preserves +C family");
});
Probe("integral of square over zero one is exactly one third",()=>
{
    var r=SymbolicExactPolynomialCalculus.Integrate(squared,x,R(0),R(1));
    Require(r.IsExact&&r.DefiniteValue==R(1,3),
        "exact quadratic integral");
});
Probe("constructed primitive retains an admitted derivative",()=>
{
    var primitive=SymbolicExactPolynomialCalculus.Integrate(x,x,R(0),R(1));
    var r=SymbolicExactDifferentiation.TryDifferentiate(
        primitive.ZeroConstantPrimitive!,x);
    Require(primitive.IsExact&&r.IsAdmitted&&
        primitive.DerivativeRuleId==r.ProofRule,
        "exact derivative/primitive roundtrip");
});
Probe("polynomial finite limit at two is exactly four",()=>
{
    var r=SymbolicExactPolynomialCalculus.FiniteLimit(squared,x,R(2));
    Require(r.IsExact&&r.Value==R(4),
        "polynomial continuity exact point limit");
});
Probe("simple rational pole admits no improper integral claim",()=>
{
    var simple=Divide(N(1),Sub(x,N(1)));
    var r=SymbolicRealCalculusBoundaries.AnalyzeIntegral(simple,x,R(0),R(2));
    Require(r.Status==SymbolicCalculusBoundaryStatus.SimplePole&&
        r.HasQualifiedBoundary&&!r.HasExactValue&&r.ExactValue is null,
        "unproved improper integration must not return finite value");
});
Probe("removable original hole has finite limit but undefined value",()=>
{
    var r=SymbolicRealCalculusBoundaries.AnalyzeLimit(rational,x,R(1));
    Require(r.Status==SymbolicCalculusBoundaryStatus.ExactRemovableLimit&&
        r.HasExactValue&&r.HasQualifiedBoundary&&
        r.ExactValue==R(2)&&r.ExceptionalPoint==R(1),
        "original function value remains undefined at the removable hole");
});
Probe("cancellation quotient integrates only over a hole-free interval",()=>
{
    var r=SymbolicRealCalculusBoundaries.AnalyzeIntegral(rational,x,R(2),R(3));
    Require(r.HasExactValue&&r.ExactValue==R(7,2)&&
        r.ConstantConvention==
        SymbolicExactPolynomialCalculus.AdditiveConstantConvention,
        "no unguarded cancellation across excluded original points");
});
Probe("unknown radical integral is refused without a proof",()=>
{
    var sqrt=IrApply.Create(IrOperation.Sqrt,[x]);
    var r=SymbolicRealCalculusBoundaries.AnalyzeIntegral(sqrt,x,R(0),R(1));
    Require(r.Status==SymbolicCalculusBoundaryStatus.UnknownConvergence&&
        !r.HasExactValue&&r.RuleId is null,
        "no implicit numeric quadrature or general CAS");
});
Probe("real dimensionful symbolic differentiation has typed refusal",()=>
{
    var q=new IrQuantityLiteral(N(1),"imperial.inch",
        UnitSystem.BritishImperial,DimensionVector.LengthDimension);
    var r=SymbolicExactDifferentiation.TryDifferentiate(q,x);
    Require(r.Status==SymbolicDifferentiationStatus.UnsupportedUnits&&!r.IsAdmitted,
        "no derived unit claims");
});
Probe("packaged Math assemblies have no foreign CAS or agent broker dependency",()=>
{
    var assemblies=new[]{typeof(SymbolicExactTransformations).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly};
    var deps=assemblies.SelectMany(a=>a.GetReferencedAssemblies())
        .Select(x=>x.Name??string.Empty).ToArray();
    Require(!deps.Any(n=>
        n.Contains("Angouri",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MathNet",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Aura",StringComparison.OrdinalIgnoreCase)||
        n.Contains("Decision",StringComparison.OrdinalIgnoreCase)||
        n.Contains("MCDM",StringComparison.OrdinalIgnoreCase)),
        "no external CAS, AURA, Decision or MCDM runtime dependency");
});
if(passed!=32)
    throw new InvalidOperationException("M4 scientific release gate requires 32 independent NuGet-only oracles.");
Console.WriteLine("dw.tools.math/m4-nuget-gate/0.5 qualified; probes="+passed);
