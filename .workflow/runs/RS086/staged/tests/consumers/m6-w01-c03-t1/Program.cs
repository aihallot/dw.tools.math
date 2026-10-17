using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("One local qualified differential analysis archive required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path)) is not
       ("m6-w01-c03-t1-local-feed" or "m6-w01-c03-t2-local-feed"))
    throw new InvalidOperationException("Unexpected numerical differentiation/ODE archive or feed.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var archive=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(archive.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No real net10 numerical analysis assembly.");
    var specs=archive.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous archive metadata.");
    using var content=specs[0].Open();
    var metadata=XDocument.Load(content).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign NuGet dependencies or version drift.");
}
static void Require(bool okay,string reason)
{
    if(!okay)throw new InvalidOperationException(reason);
}
static void Near(double? actual,double expected,double tolerance=1e-7)
{
    Require(actual is { } value&&double.IsFinite(value)&&
        global::System.Math.Abs(value-expected)<=tolerance,
        "Independent source-free numerical calculation did not match known oracle.");
}
var passed=0;
void Probe(string name,Action check)
{
    try{check();passed++;}
    catch(Exception error){throw new InvalidOperationException("NuGet-only derivative/ODE: "+name,error);}
}
Probe("square derivative at two",()=>{
    var r=BoundedDifferentialAnalysis64.Differentiate(x=>x*x,2);
    Require(r.Succeeded&&!r.ErrorBoundGuaranteed,"A bounded central-difference observation is required.");
    Near(r.Value,4,1e-9);
});
Probe("sine derivative at zero",()=>{
    var r=BoundedDifferentialAnalysis64.Differentiate(global::System.Math.Sin,0);
    Require(r.Succeeded,"Expected bounded trigonometric derivative.");
    Near(r.Value,1,1e-6);
});
Probe("invalid derivative resolution is refused",()=>{
    var r=BoundedDifferentialAnalysis64.Differentiate(x=>x*x,1e20);
    Require(r.Status==DifferentialStatus64.StepNotRepresentable&&
        !r.Succeeded&&r.Value is null,"Unrepresentable step must not claim a derivative.");
});
Probe("finite derivative budget refusal",()=>{
    var r=BoundedDifferentialAnalysis64.Differentiate(x=>x*x,2,new(1e-3,1e-6,3));
    Require(r.Status==DifferentialStatus64.BudgetExceeded&&r.Value is null,
        "Finite evaluation budgets are required.");
});
Probe("simple exponential-growth ODE",()=>{
    var r=BoundedDifferentialAnalysis64.IntegrateOde((t,y)=>y,0,1,1);
    Require(r.Succeeded&&!r.GlobalErrorBoundGuaranteed&&r.Steps==64,
        "A bounded RK4 growth observation is required.");
    Near(r.FinalValue,global::System.Math.E);
});
Probe("exponential-decay ODE",()=>{
    var r=BoundedDifferentialAnalysis64.IntegrateOde((t,y)=>-y,0,1,1);
    Require(r.Succeeded,"Expected bounded RK4 decay.");
    Near(r.FinalValue,1/global::System.Math.E);
});
Probe("invalid ODE callback refuses",()=>{
    var r=BoundedDifferentialAnalysis64.IntegrateOde((t,y)=>double.NaN,0,1,1);
    Require(r.Status==DifferentialStatus64.NonFiniteEvaluation&&
        r.FinalValue is null,"Nonfinite callback must not produce partial trajectory.");
});
Probe("prior independent numerical analyses retained",()=>{
    var q=BoundedUnivariateAnalysis64.IntegrateSimpson(x=>x*x,0,1);
    Require(q.Succeeded,"Previously qualified quadrature must remain callable.");
    Near(q.Integral,1d/3d);
    var x=CurveBoundaryAudit64.EvaluateLinear([new(0,1),new(1,3)],0.5,
        new("s","m"),"s");
    Require(x.Admitted&&!x.PredictiveGuarantee,
        "Prior curve unit/forecast refusal must remain callable.");
    Near(x.Value,2);
});
Require(passed==8,"Exactly eight distinct source-free numerical probes required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W01-C03-T1-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-differential-consumer/1.1 qualified; probes="+passed);
