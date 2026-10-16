using System;
using System.Linq;
using System.Text.Json;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Distribution;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;

const string canonical="where(sub(square(x),1),ne(x,1))";
static ExactRational R(long n,long d=1)=>new(n,d);
var probes=0;
void Probe(string label,Action check)
{
    try {check();probes++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 independent numerical/symbolic console vertical: "+label,error);
    }
}
static void Require(bool condition,string reason)
{
    if(!condition)throw new InvalidOperationException(reason);
}
var accepted=ExactMathVerticalRunner.Run(canonical,R(2));
Probe("parse, typed restriction and exact source replay",()=>
{
    Require(accepted.IsAdmitted&&accepted.CanonicalSource==canonical&&
        accepted.OriginalAssumption=="ne(x,1)"&&accepted.Original is not null&&
        accepted.SourceCanonicalIrJson==
            IrCanonicalJsonCodec.Encode(accepted.Original)&&
        accepted.OriginalSemanticSha256==
            IrCanonicalHashes.SemanticStructuralSha256(accepted.Original),
        "source IR or its original exclusion was not retained");
});
Probe("formal derivative of body equals 2x without dropping restriction",()=>
{
    Require(accepted.CanonicalDerivative=="mul(2,x)"&&
        accepted.FormalDerivativeRule==SymbolicExactDifferentiation.Rule&&
        accepted.Original?.Assumptions.Conditions.Single().Kind==
            IrRelationKind.NotEqual,
        "a formal body derivative is not a solved restricted equation");
});
Probe("exact evaluation at two is three",()=>
{
    Require(accepted.ExactValueAtPoint==R(3)&&
        accepted.PointEvaluationRule==ExactMathVerticalRunner.PointRule&&
        accepted.ExactComposition?.Status==ComputationStatus.Exact,
        "polynomial continuity gives a value only at an original-domain point");
});
Probe("original roots are separately audited",()=>
{
    var roots=accepted.OriginalRootAudit;
    Require(accepted.UnrestrictedCandidates?.IsComplete==true&&
        roots?.VerifiedRoots.Single().Value==R(-1)&&
        roots.ExcludedFromDomain.Single()==R(1)&&
        !roots.IsComplete,"the original x!=1 excludes one algebraic candidate");
});
Probe("binary64 conversion is explicit and loss-free at three",()=>
{
    Require(accepted.Binary64Projection?.Status==RationalBinary64Status.Exact&&
        accepted.Binary64Projection.Value==3d&&
        accepted.Binary64Projection.AbsoluteError==R(0)&&
        accepted.Binary64Composition?.Status==ComputationStatus.Approximate,
        "binary64 is a separate numeric precision even when exactly representable");
});
Probe("excluded evaluation point and derivative-to-solver mix are refused",()=>
{
    var excluded=ExactMathVerticalRunner.Run(canonical,R(1));
    var mixed=ExactMathVerticalRunner.Run(
        canonical,R(2),accepted.FormalBodyDerivative);
    Require(excluded.Status==ExactVerticalStatus.ExcludedEvaluationPoint&&
        mixed.Status==ExactVerticalStatus.IncompatiblePipeline&&
        excluded.ExactValueAtPoint is null&&mixed.OriginalRootAudit is null,
        "cannot evaluate at excluded x or solve the derivative as original source");
});
Probe("rounding requires consent and retains exact error",()=>
{
    var withheld=ExactMathVerticalRunner.Run(canonical,R(1,3));
    var allowed=ExactMathVerticalRunner.Run(canonical,R(1,3),
        numericPolicy:RationalBinary64Policy.AllowRounded);
    Require(withheld.Status==ExactVerticalStatus.RoundingNotAuthorized&&
        allowed.IsAdmitted&&allowed.ExactValueAtPoint==R(-8,9)&&
        allowed.Binary64Projection?.Status==RationalBinary64Status.Rounded&&
        allowed.Binary64Projection.AbsoluteError>R(0),
        "there is no implicit approximate answer or swallowed precision loss");
});
Probe("NuGet-only dependency graph requires no AURA or external CAS",()=>
{
    var names=new[]{typeof(ExactMathVerticalRunner).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty).ToArray();
    Require(!names.Any(n=>
        new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
            .Any(mark=>n.Contains(mark,StringComparison.OrdinalIgnoreCase))),
        "a public standalone Math sample has no foreign broker or CAS dependency");
});
if(probes!=8)throw new InvalidOperationException("The Math vertical requires eight independent NuGet probes.");

var report=new
{
    schema_version=1,
    id="dw.tools.math/m5-exact-vertical/1",
    status=accepted.Status.ToString(),
    source=accepted.CanonicalSource,
    derivative=accepted.CanonicalDerivative,
    evaluated_at=2,
    exact_value=accepted.ExactValueAtPoint!.Value.ToFractionString(),
    binary64=accepted.Binary64Projection!.Value!.Value,
    numeric_loss=accepted.LossClassification,
    admitted_roots=accepted.OriginalRootAudit!.VerifiedRoots.Select(x=>x.Value.ToFractionString()).ToArray(),
    excluded_candidates=accepted.OriginalRootAudit.ExcludedFromDomain.Select(x=>x.ToFractionString()).ToArray(),
    original_root_audit_complete=accepted.OriginalRootAudit.IsComplete,
    provider=accepted.ProviderId,
    assumption=accepted.OriginalAssumption,
    source_sha256=accepted.OriginalSemanticSha256,
    replay_bytes=accepted.SourceCanonicalIrJson!.Length,
    oracles=probes
};
Console.WriteLine(JsonSerializer.Serialize(report));
Console.WriteLine("dw.tools.math/m5-exact-vertical-console/1.0 qualified; probes="+probes);
