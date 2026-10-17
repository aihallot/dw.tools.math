using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("Exactly one local numerical-audit nupkg is required.");
var archivePath=Path.GetFullPath(args[0]);
if(Path.GetFileName(archivePath)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(archivePath))!="m6-w01-c03-t2-local-feed")
    throw new InvalidOperationException("Only the pinned C03-T2 isolated feed is admissible.");
var bytes=File.ReadAllBytes(archivePath);
using(var input=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(input,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Actual compiled net10 Math package assembly absent.");
    var specs=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous package metadata.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign package dependency or identity drift.");
}
static void Require(bool okay,string explanation)
{
    if(!okay)throw new InvalidOperationException(explanation);
}
static void Near(double? value,double expected,double tolerance=1e-7)
{
    Require(value is { } actual&&double.IsFinite(actual)&&
        global::System.Math.Abs(actual-expected)<=tolerance,
        "NuGet-only numerical result differs from independent scientific oracle.");
}
var count=0;
void Probe(string name,Action action)
{
    try{action();count++;}
    catch(Exception issue){throw new InvalidOperationException("C03 boundary consumer: "+name,issue);}
}
Probe("caller-assumed smooth square derivative",()=>{
    var r=DifferentialBoundaryAudit64.Differentiate(x=>x*x,2,DerivativeRegularity64.CallerAssumesSmooth);
    Require(r.Succeeded&&!r.SmoothnessIndependentlyVerified&&
        !r.LocalErrorBoundGuaranteed,"Admitted central-difference assumptions cannot become proven guarantees.");
    Near(r.Value,4,1e-9);
});
Probe("unspecified cusp regularity refuses",()=>{
    var r=DifferentialBoundaryAudit64.Differentiate(global::System.Math.Abs,0);
    Require(r.Status==DifferentialAuditStatus64.RegularityUnspecified&&
        r.Value is null&&!r.Succeeded,"An unproved differentiability claim is inadmissible.");
});
Probe("unrepresentable numerical derivative step refuses",()=>{
    var r=DifferentialBoundaryAudit64.Differentiate(x=>x*x,1e20,
        DerivativeRegularity64.CallerAssumesSmooth);
    Require(r.Status==DifferentialAuditStatus64.StepNotRepresentable&&
        r.Value is null,"Sub-ULP coordinate changes may not report derivatives.");
});
Probe("unspecified ODE regime refuses",()=>{
    var r=DifferentialBoundaryAudit64.IntegrateOde((t,y)=>y,0,1,1);
    Require(r.Status==DifferentialAuditStatus64.OdeRegimeUnspecified&&
        r.FinalValue is null,"The caller must declare a nonstiff event-free scalar IVP.");
});
Probe("stiff, event-driven and PDE regimes refuse",()=>{
    foreach(var regime in new[]{
        OdeRegime64.KnownStiff,OdeRegime64.EventDriven,
        OdeRegime64.PartialDifferentialEquation})
    {
        var r=DifferentialBoundaryAudit64.IntegrateOde((t,y)=>y,0,1,1,regime);
        Require(r.Status==DifferentialAuditStatus64.UnsupportedRegime&&
            r.FinalValue is null,"Unqualified solver regime was accepted.");
    }
});
Probe("assumed nonstiff exponential IVP",()=>{
    var r=DifferentialBoundaryAudit64.IntegrateOde((t,y)=>y,0,1,1,
        OdeRegime64.CallerAssumesNonStiffNoEvents);
    Require(r.Succeeded&&!r.NonStiffRegimeIndependentlyVerified&&
        !r.EventAbsenceIndependentlyVerified&&!r.GlobalErrorBoundGuaranteed,
        "A caller assumption is not a stability theorem or error bound.");
    Near(r.FinalValue,global::System.Math.E,1e-7);
});
Probe("resource-limited ODE refuses nonpartially",()=>{
    var r=DifferentialBoundaryAudit64.IntegrateOde((t,y)=>y,0,1,1,
        OdeRegime64.CallerAssumesNonStiffNoEvents,new(32,1e-6,383));
    Require(r.Status==DifferentialAuditStatus64.BudgetExceeded&&
        r.FinalValue is null&&!r.Succeeded,
        "An insufficient evaluation budget must not emit a partial IVP result.");
});
Probe("retained T1 differential and numerical consumers",()=>{
    var prior=BoundedDifferentialAnalysis64.Differentiate(x=>x*x,2);
    Require(prior.Succeeded,"Previously qualified derivative is unavailable.");
    Near(prior.Value,4,1e-9);
    var curve=BoundedCurveAnalysis64.InterpolateLinear([new(0,1),new(1,3)],0.5);
    Require(curve.Succeeded,"Previously qualified curve is unavailable.");
    Near(curve.Value,2);
});
Require(count==8,"Exactly eight independently asserted NuGet-only boundary probes required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C03-T2-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-differential-boundary-consumer/1.1 qualified; probes="+count);
