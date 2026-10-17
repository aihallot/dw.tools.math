using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)
    throw new ArgumentException("Exactly one isolated Math numerical audit nupkg required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path)) is not
       ("m6-w01-c02-t2-local-feed" or "m6-w01-c03-t1-local-feed" or "m6-w01-gate-local-feed"))
    throw new InvalidOperationException("Only the qualified local T2 or C03 T1 boundary-audit feed is admitted.");
var archiveBytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(archiveBytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("Expected a real analysis net10 binary.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("One package identity required.");
    using var input=specs[0].Open();
    var meta=XDocument.Load(input).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Unadmitted NuGet dependency or identity.");
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}
static void Near(double? actual,double expected,double tolerance=1e-8)
{
    Require(actual is { } v&&double.IsFinite(v)&&
        global::System.Math.Abs(v-expected)<=tolerance,
        "Isolated NuGet mathematical result diverged from independent oracle.");
}
var checkedCount=0;
void Probe(string label,Action assertion)
{
    try{assertion();checkedCount++;}
    catch(Exception reason){throw new InvalidOperationException("Audit client: "+label,reason);}
}
Probe("sqrt2 audited root and residual",()=>{
    var r=UnivariateBoundaryAudit64.AuditRoot(x=>x*x-2,1,2);
    Require(r.Succeeded&&!r.ContinuityProven&&!r.ErrorBoundGuaranteed&&
        r.AbsoluteResidual<=1e-10,"Residual-bound root audit not admitted.");
    Near(r.Root,global::System.Math.Sqrt(2));
});
Probe("sign discontinuity cannot pass a residual-only gate",()=>{
    var r=UnivariateBoundaryAudit64.AuditRoot(x=>x<0?-1e200:1e200,-1,1);
    Require(r.Status==UnivariateAuditStatus64.ResidualNotMet&&
        r.Root is null&&!r.Succeeded,"Jump must not masquerade as small-residual root.");
});
Probe("root without a sign-changing bracket refuses",()=>{
    var r=UnivariateBoundaryAudit64.AuditRoot(x=>x*x+1,1,2);
    Require(r.Status==UnivariateAuditStatus64.MissingBracket&&r.Root is null,
        "Missing bracket cannot produce a partial observation.");
});
Probe("coarse position tolerance cannot waive residual",()=>{
    var r=UnivariateBoundaryAudit64.AuditRoot(x=>x*x-2,1,2,new(0.1,1e-20,128));
    Require(r.Status==UnivariateAuditStatus64.ResidualNotMet&&r.Root is null,
        "Width-only convergence is insufficient.");
});
Probe("unknown quadrature regularity refuses",()=>{
    var r=UnivariateBoundaryAudit64.AuditQuadrature(x=>x<0.5?0:1,0,1);
    Require(r.Status==UnivariateAuditStatus64.RegularityUnspecified&&
        r.Integral is null,"Undeclared smoothness cannot validate Simpson convergence.");
});
Probe("caller-assumed smooth square integrates to third",()=>{
    var r=UnivariateBoundaryAudit64.AuditQuadrature(x=>x*x,0,1,
        QuadratureRegularity64.CallerAssumesSmooth);
    Require(r.Succeeded&&r.SmoothnessAssumedByCaller&&
        !r.ErrorBoundGuaranteed,"Assumed smoothness cannot be certified.");
    Near(r.Integral,1d/3d,1e-9);
});
Probe("smooth sine case approximates two",()=>{
    var r=UnivariateBoundaryAudit64.AuditQuadrature(global::System.Math.Sin,0,
        global::System.Math.PI,QuadratureRegularity64.CallerAssumesSmooth,
        new(1e-6,128,512));
    Require(r.Succeeded,"Sine reference should remain admitted.");
    Near(r.Integral,2,1e-5);
});
Probe("strict budget refuses while original curves remain",()=>{
    var refused=UnivariateBoundaryAudit64.AuditQuadrature(global::System.Math.Exp,0,1,
        QuadratureRegularity64.CallerAssumesSmooth,new(1e-20,128,8));
    Require(refused.Status==UnivariateAuditStatus64.BudgetExceeded&&refused.Integral is null,
        "Over-budget estimate cannot expose partial values.");
    var old=BoundedCurveAnalysis64.InterpolateLinear([new(0,1),new(1,3)],0.5);
    Require(old.Succeeded,"Prior T1 analysis must remain available.");
    Near(old.Value,2);
});
Require(checkedCount==8,"Exactly eight independent isolated numerical audit probes required.");
var digest=Convert.ToHexString(SHA256.HashData(archiveBytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C02-T2-CONSUMER-SHA256:"+Id+":"+Version+":"+
    archiveBytes.Length+":"+digest);
Console.WriteLine("dw.tools.math/m6-univariate-boundary-consumer/1.1 qualified; probes="+checkedCount);
