using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.3";
if(args.Length!=1)throw new ArgumentException("Exactly one source-free local package path required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w01-c02-t1-local-feed")
    throw new InvalidOperationException("Only the qualified local univariate NuGet feed is admitted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("No actual net10 analysis assembly in archive.");
    var nuspec=zip.Entries.Single(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase));
    using var input=nuspec.Open();
    var meta=XDocument.Load(input).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Unqualified package identity or dependencies.");
}
static void Require(bool condition,string why)
{
    if(!condition)throw new InvalidOperationException(why);
}
static void Near(double? value,double expected,double tolerance=1e-8)
{
    Require(value is { } v&&double.IsFinite(v)&&global::System.Math.Abs(v-expected)<=tolerance,
        "Real NuGet scientific result differs from independent hand oracle.");
}
var probes=0;
void Probe(string label,Action action)
{
    try{action();probes++;}
    catch(Exception ex){throw new InvalidOperationException("Univariate NuGet probe: "+label,ex);}
}
Probe("sqrt2 root",()=>{
    var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x*x-2,1,2);
    Require(r.Succeeded&&!r.GuaranteedRootError&&r.Iterations>0,"No bounded root observation.");
    Near(r.Root,global::System.Math.Sqrt(2),1e-8);
});
Probe("endpoint root",()=>{
    var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x-2,1,2);
    Require(r.Succeeded&&r.Iterations==0,"Endpoint root not observed.");
    Near(r.Root,2);
});
Probe("missing sign bracket refusal",()=>{
    var r=BoundedUnivariateAnalysis64.SolveBracketed(x=>x*x+1,1,2);
    Require(!r.Succeeded&&r.Status==UnivariateStatus64.MissingBracket&&r.Root is null,
        "No-bracket input must not produce a partial result.");
});
Probe("quadratic Simpson integral",()=>{
    var q=BoundedUnivariateAnalysis64.IntegrateSimpson(x=>x*x,0,1);
    Require(q.Succeeded&&!q.ErrorBoundGuaranteed,"No honest integration diagnostic.");
    Near(q.Integral,1d/3d,1e-9);
});
Probe("sine Simpson integral",()=>{
    var q=BoundedUnivariateAnalysis64.IntegrateSimpson(global::System.Math.Sin,0,global::System.Math.PI,new(1e-6,128,512));
    Require(q.Succeeded,"Smooth sine reference must be admitted.");
    Near(q.Integral,2d,1e-5);
});
Probe("invalid callback value refusal",()=>{
    var q=BoundedUnivariateAnalysis64.IntegrateSimpson(_=>double.NaN,0,1);
    Require(!q.Succeeded&&q.Status==UnivariateStatus64.NonFiniteEvaluation&&q.Integral is null,
        "Nonfinite callback must not return partial estimate.");
});
Probe("budget exhaustion refusal",()=>{
    var q=BoundedUnivariateAnalysis64.IntegrateSimpson(global::System.Math.Exp,0,1,new(1e-20,128,8));
    Require(!q.Succeeded&&q.Status==UnivariateStatus64.BudgetExceeded,
        "Finite resource budget must be typed.");
});
Probe("T1/T2 curve compatibility",()=>{
    var r=BoundedCurveAnalysis64.InterpolateNaturalSpline([new(0,0),new(1,1),new(2,0)],0.5);
    Require(r.Succeeded,"Prior spline must stay compatible.");
    Near(r.Value,0.6875);
    var q=CurveBoundaryAudit64.EvaluateLinear([new(0,1),new(1,3)],0.5,new("s","m"),"s");
    Require(q.Admitted&&!q.PredictiveGuarantee,"Prior unit boundary must stay compatible.");
});
Require(probes==8,"Exactly eight independent NuGet scientific probes required.");
var digest=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C02-T1-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+digest);
Console.WriteLine("dw.tools.math/m6-univariate-consumer/1.1 qualified; probes="+probes);
