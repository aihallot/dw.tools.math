using System;
using System.Linq;
using dw.quantities;
using Dw.Tools.Math.Distribution;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;

const string source="where(sub(square(x),1),ne(x,1))";
static ExactRational R(long n,long d=1)=>new(n,d);
var count=0;
void Probe(string label,Action oracle)
{
    try {oracle();count++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 independent exact vertical NuGet consumer: "+label,error);
    }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}
Probe("independent original typed mathematical IR replay",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(2));
    Require(audited.IsAdmitted&&audited.OriginalIrReplayed&&
        audited.SourceSha256?.Length==64&&
        audited.OriginalSemanticSha256==audited.Vertical!.OriginalSemanticSha256&&
        !audited.SourceFingerprintIsMathematicalProof,
        "a source-byte fingerprint is never semantic proof");
});
Probe("derivative remains distinct from original equation",()=>
{
    var original=ExactVerticalBoundaryAudit.Audit(source,R(2));
    var refused=ExactVerticalBoundaryAudit.Audit(
        source,R(2),original.Vertical!.FormalBodyDerivative);
    Require(original.IsAdmitted&&original.FormalDerivativeSeparated&&
        original.Vertical.CanonicalDerivative=="mul(2,x)"&&
        refused.Status==ExactVerticalBoundaryStatus.IncompatiblePipeline&&
        refused.LossKind==ExactVerticalBoundaryLossKind.WrongRootEquationOrDerivative&&
        refused.Vertical is null,
        "formal differentiation cannot substitute for original solving");
});
Probe("only admitted original root is minus one",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(2));
    var roots=audited.Vertical!.OriginalRootAudit!;
    Require(audited.UnrestrictedCandidatesComplete&&
        !audited.OriginalRootAuditComplete&&
        roots.VerifiedRoots.Single().Value==R(-1)&&
        roots.ExcludedFromDomain.Single()==R(1),
        "unrestricted candidates do not certify the complete original audit");
});
Probe("domain hole refuses value and proof",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(1));
    Require(audited.Status==ExactVerticalBoundaryStatus.ExcludedOriginalPoint&&
        audited.LossKind==ExactVerticalBoundaryLossKind.OriginalDomainExclusion&&
        audited.Vertical is null&&audited.AuditRuleId is null&&
        audited.OriginalSemanticSha256 is null,
        "a point excluded from original x!=1 cannot be filled");
});
Probe("unapproved binary64 rounding refuses partial answers",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(1,3));
    Require(audited.Status==ExactVerticalBoundaryStatus.UnauthorizedRounding&&
        audited.LossKind==ExactVerticalBoundaryLossKind.DisallowedNumericLoss&&
        audited.Vertical is null,
        "precision policy must not be assumed");
});
Probe("consented rounding retains exact error",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(1,3),
        numericPolicy:RationalBinary64Policy.AllowRounded);
    Require(audited.IsAdmitted&&audited.Vertical!.ExactValueAtPoint==R(-8,9)&&
        audited.Vertical!.Binary64Projection?.Status==RationalBinary64Status.Rounded&&
        audited.Vertical!.Binary64Projection!.AbsoluteError is ExactRational error &&
        error>R(0)&&
        audited.NumericLoss=="binary64-rounded-exact-error-retained-original-ir-unchanged",
        "approximate binary64 cannot replace exact rational meaning");
});
Probe("alternate exact quadratic is not hardcoded to one fixture",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(
        "where(sub(square(x),4),ne(x,1))",R(3));
    Require(audited.IsAdmitted&&audited.Vertical!.ExactValueAtPoint==R(5)&&
        audited.Vertical!.OriginalRootAudit!.VerifiedRoots.Length==2&&
        audited.Vertical!.OriginalRootAudit!.ExcludedFromDomain.Length==0,
        "different rational roots must retain original-domain evidence");
});
Probe("pinned Math-only package needs no AURA or external CAS",()=>
{
    var audited=ExactVerticalBoundaryAudit.Audit(source,R(2));
    var names=new[]{typeof(ExactVerticalBoundaryAudit).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(assembly=>assembly.GetReferencedAssemblies())
        .Select(assembly=>assembly.Name??string.Empty);
    Require(audited.IsAdmitted&&!names.Any(n=>
        new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
            .Any(keyword=>n.Contains(keyword,StringComparison.OrdinalIgnoreCase))),
        "independent Math consumer cannot depend on any foreign CAS or broker");
});
if(count!=8)
    throw new InvalidOperationException("Eight exact boundary NuGet consumer probes required.");
Console.WriteLine("dw.tools.math/m5-exact-vertical-boundary-consumer/1.0 qualified; probes="+count);
