using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("Exactly one local qualified nupkg path required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path)) is not
       ("m6-w01-c01-t2-local-feed" or "m6-w01-gate-local-feed"))
    throw new InvalidOperationException("Only the explicitly pinned local T2 NuGet feed is accepted.");
var bytes=File.ReadAllBytes(path);
using(var stream=new MemoryStream(bytes,writable:false))
using(var archive=new ZipArchive(stream,ZipArchiveMode.Read))
{
    if(archive.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No real net10 analysis assembly.");
    var metadataFiles=archive.Entries.Where(e=>e.FullName.EndsWith(
        ".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(metadataFiles.Length!=1)throw new InvalidOperationException("Ambiguous nuspec.");
    using var item=metadataFiles[0].Open();
    var metadata=XDocument.Load(item).Descendants()
        .Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Math analysis archive metadata is not the closed source-free contract.");
}
static void Require(bool okay,string description)
{
    if(!okay)throw new InvalidOperationException(description);
}
static void Near(double? actual,double expected)
{
    Require(actual is { } v&&double.IsFinite(v)&&
        global::System.Math.Abs(v-expected)<=1e-10,
        "Real NuGet numerical result differs from independent oracle.");
}
var count=0;
void Probe(string label,Action action)
{
    try{action();count++;}
    catch(Exception problem){throw new InvalidOperationException("NuGet-only Math boundary: "+label,problem);}
}
CurvePoint64[] points=[new(0,1),new(1,3),new(2,5)];
CurveAxisUnits64 units=new("s","m");
var opt=new CurveBoundaryPolicy64(1e-12,CurveExtrapolation64.LinearSegment);
Probe("observed units and interpolation",()=>
{
    var r=CurveBoundaryAudit64.EvaluateLinear(points,0.5,units,"s");
    Require(r.Admitted&&!r.Extrapolated&&!r.UnitsConverted&&r.SlopeUnit=="m/s",
        "Admitted unit metadata is explicit and never converted.");
    Near(r.Value,2);Near(r.LocalSlope,2);
});
Probe("default no extrapolation",()=>
{
    var r=CurveBoundaryAudit64.EvaluateLinear(points,3,units,"s");
    Require(r.Status==CurveBoundaryStatus64.OutsideDomain&&r.Value is null,
        "No extrapolation without explicit policy.");
});
Probe("right-hand opt-in extrapolation",()=>
{
    var r=CurveBoundaryAudit64.EvaluateLinear(points,3,units,"s",opt);
    Require(r.Admitted&&r.Extrapolated&&!r.PredictiveGuarantee,
        "Opt-in extrapolation is not a forecast guarantee.");
    Near(r.Value,7);
});
Probe("left-hand opt-in extrapolation",()=>
{
    var r=CurveBoundaryAudit64.EvaluateLinear(points,-1,units,"s",opt);
    Require(r.Admitted&&r.Extrapolated&&!r.PredictiveGuarantee,
        "Only explicit linear segment extrapolation is qualified.");
    Near(r.Value,-1);
});
Probe("mismatched physical unit rejected",()=>
{
    var r=CurveBoundaryAudit64.EvaluateLinear(points,0.5,units,"min");
    Require(r.Status==CurveBoundaryStatus64.IncompatibleUnit&&!r.Admitted&&
        r.Value is null&&r.SlopeUnit is null,"No silent unit conversion.");
});
Probe("relative conditioning rejected",()=>
{
    var r=CurveBoundaryAudit64.FitDescriptive(
        [new(1e12,0),new(1e12+0.01,1)],units);
    Require(r.Status==CurveBoundaryStatus64.NearRankDeficiency&&!r.Admitted&&
        r.Slope is null&&r.SumSquaredResiduals is null,
        "Near-rank-deficient regressions cannot be admitted.");
});
Probe("finite line fit is descriptive only",()=>
{
    var r=CurveBoundaryAudit64.FitDescriptive(points,units);
    Require(r.Admitted&&!r.CausalGuarantee&&r.SlopeUnit=="m/s",
        "Descriptive unweighted OLS cannot assert causality.");
    Near(r.Slope,2);Near(r.Intercept,1);Near(r.SumSquaredResiduals,0);
});
Probe("prior spline and no foreign agent dependencies",()=>
{
    var old=BoundedCurveAnalysis64.InterpolateNaturalSpline(
        [new(0,0),new(1,1),new(2,0)],0.5);
    Require(old.Succeeded,"Qualified T1 cubic spline must remain usable.");
    Near(old.Value,0.6875);
    var references=typeof(CurveBoundaryAudit64).Assembly.GetReferencedAssemblies()
        .Select(a=>a.Name??"");
    Require(!references.Any(name=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
        .Any(token=>name.Contains(token,StringComparison.OrdinalIgnoreCase))),
        "A NuGet-only Math curve client must remain agent and CAS independent.");
});
Require(count==8,"Eight independent current Math boundary probes required.");
var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C01-T2-CONSUMER-SHA256:"+Id+":"+Version+":"+
    bytes.Length+":"+digest);
Console.WriteLine("dw.tools.math/m6-curve-boundary-consumer/1.1 qualified; probes="+count);
