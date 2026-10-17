using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Analysis;

const string Id="dw.tools.math.analysis",Version="1.1.0-preview.1";
if(args.Length!=1)throw new ArgumentException("Exactly one isolated local nupkg path is required.");
var archivePath=Path.GetFullPath(args[0]);
if(Path.GetFileName(archivePath)!=Id+"."+Version+".nupkg" ||
   Path.GetFileName(Path.GetDirectoryName(archivePath))!="m6-w01-c01-t1-local-feed")
    throw new InvalidOperationException("Unexpected local archive version or feed.");
var archive=File.ReadAllBytes(archivePath);
using(var stream=new MemoryStream(archive,writable:false))
using(var zip=new ZipArchive(stream,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("No admitted net10 Math analysis assembly in actual nupkg.");
    var spec=zip.Entries.Where(e=>e.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(spec.Length!=1)throw new InvalidOperationException("Ambiguous package metadata.");
    using var input=spec[0].Open();
    var xml=XDocument.Load(input);
    var metadata=xml.Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id ||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version ||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign NuGet dependency or package identity.");
}
var passed=0;
static void Require(bool okay,string description)
{
    if(!okay)throw new InvalidOperationException(description);
}
static void Near(double? actual,double expected)
{
    Require(actual is not null&&double.IsFinite(actual.Value)&&
        global::System.Math.Abs(actual.Value-expected)<1e-10,
        "Math curve result differs from independently calculated hand oracle.");
}
void Probe(string label,Action verify)
{
    try{verify();passed++;}
    catch(Exception exception)
    {
        throw new InvalidOperationException("Independent net10 NuGet curve probe: "+label,exception);
    }
}
CurvePoint64[] straight=[new(0,1),new(1,3),new(2,5)];
Probe("finite interpolation at 0.5",()=>
{
    var answer=BoundedCurveAnalysis64.InterpolateLinear(straight,0.5);
    Require(answer.Succeeded,"Linear interpolation requires admitted success.");
    Near(answer.Value,2);
});
Probe("natural spline reproduces nontrivial curvature",()=>
{
    var answer=BoundedCurveAnalysis64.InterpolateNaturalSpline(
        [new(0,0),new(1,1),new(2,0)],0.5);
    Require(answer.Succeeded,"Curved spline must be admitted.");
    Near(answer.Value,0.6875);
});
Probe("three-point least squares fit exactly y=2x+1",()=>
{
    var fit=BoundedCurveAnalysis64.FitLeastSquares(straight);
    Require(fit.Succeeded,"OLS must be admitted for three independent abscissae.");
    Near(fit.Slope,2);
    Near(fit.Intercept,1);
    Near(fit.SumSquaredResiduals,0);
});
Probe("noisy symmetric sample has independently checked residual",()=>
{
    var fit=BoundedCurveAnalysis64.FitLeastSquares(
        [new(0,0),new(1,1),new(2,0)]);
    Require(fit.Succeeded,"Unweighted linear OLS must return finite fit.");
    Near(fit.Slope,0);
    Near(fit.Intercept,1d/3d);
    Near(fit.SumSquaredResiduals,2d/3d);
});
Probe("duplicate independent abscissae refuse fit",()=>
{
    var refused=BoundedCurveAnalysis64.FitLeastSquares([new(0,1),new(0,2)]);
    Require(refused.Status==CurveAnalysisStatus64.DuplicateAbscissa &&
        refused.Slope is null && refused.Intercept is null,
        "A duplicate x is not a reliable independent observation.");
});
Probe("extrapolation is a typed refusal",()=>
{
    var refused=BoundedCurveAnalysis64.InterpolateLinear(straight,3);
    Require(refused.Status==CurveAnalysisStatus64.OutsideDomain &&
        refused.Value is null,"No silent forecast outside trained interval.");
});
Probe("finite budget rejects oversized data",()=>
{
    var points=Enumerable.Range(0,33).Select(i=>new CurvePoint64(i,2*i+1)).ToArray();
    var refused=BoundedCurveAnalysis64.InterpolateNaturalSpline(points,0.5);
    Require(refused.Status==CurveAnalysisStatus64.BudgetExceeded &&
        refused.Value is null,"No unbounded spline allocation.");
});
Probe("versioned rule and no foreign agent runtime",()=>
{
    var observed=BoundedCurveAnalysis64.InterpolateLinear(straight,0.5);
    var deps=typeof(BoundedCurveAnalysis64).Assembly.GetReferencedAssemblies()
        .Select(x=>x.Name??"");
    Require(observed.RuleId==BoundedCurveAnalysis64.RuleId &&
        !deps.Any(x=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
            .Any(name=>x.Contains(name,StringComparison.OrdinalIgnoreCase))),
        "Math numerical analysis must be independently versioned without a broker.");
});
Require(passed==8,"Exactly eight source-free NuGet scientific oracles required.");
var digest=Convert.ToHexString(SHA256.HashData(archive)).ToLowerInvariant();
Console.WriteLine("M6-W01-C01-T1-CONSUMER-SHA256:"+Id+":"+Version+":"+
    archive.Length+":"+digest);
Console.WriteLine("dw.tools.math/m6-curve-analysis-consumer/1.1 qualified; probes="+passed);
