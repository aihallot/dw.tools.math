using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.6";
if(args.Length!=1)throw new ArgumentException("One local special-function boundary package required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w02-c03-s01-t2-local-feed")
    throw new InvalidOperationException("Only exact bounded local scientific T2 feed admitted.");
var bytes=File.ReadAllBytes(path);
using(var buffer=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(buffer,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not {Length: > 0})
        throw new InvalidOperationException("Expected a real net10 scientific assembly.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous NuGet metadata.");
    using var item=specs[0].Open();
    var meta=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(meta.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       meta.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       meta.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Package identity or foreign dependencies drifted.");
}
static void Require(bool ok,string why)
{
    if(!ok)throw new InvalidOperationException(why);
}
static void Near(SpecialRealBoundaryResult64 r,double expected,double tolerance=1e-10)
{
    Require(r.Succeeded&&r.Value is { } value&&double.IsFinite(value)&&
        global::System.Math.Abs(value-expected)<=tolerance&&!r.ErrorBoundGuaranteed,
        "A real local NuGet boundary oracle did not match the expected finite result.");
}
static void ComplexNear(SpecialComplexBoundaryResult64 r,Complex expected,double tolerance=1e-10)
{
    Require(r.Succeeded&&r.Value is { } value&&
        global::System.Math.Abs(value.Real-expected.Real)<=tolerance&&
        global::System.Math.Abs(value.Imaginary-expected.Imaginary)<=tolerance&&
        !r.ErrorBoundGuaranteed,"A complex principal-value NuGet oracle failed.");
}
static void RealRefused(SpecialRealBoundaryResult64 r,SpecialBoundaryStatus64 expected)
{
    Require(!r.Succeeded&&r.Status==expected&&r.Value is null&&r.EffectiveRadians is null,
        "A typed real numerical boundary refusal exposed a partial result.");
}
static void ComplexRefused(SpecialComplexBoundaryResult64 r,SpecialBoundaryStatus64 expected)
{
    Require(!r.Succeeded&&r.Status==expected&&r.Value is null,
        "A typed complex numerical boundary refusal exposed a partial result.");
}
var total=0;
void Probe(string name,Action verify)
{
    try{verify();total++;}
    catch(Exception error){throw new InvalidOperationException("NuGet special-function boundary: "+name,error);}
}
Probe("explicit ninety-degree sine",()=>{
    Near(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Sine,90,SpecialAngleInput64.Degrees),1);
});
Probe("explicit one-hundred-eighty-degree cosine",()=>{
    Near(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Cosine,180,SpecialAngleInput64.Degrees),-1);
});
Probe("missing angular unit is typed refusal",()=>{
    RealRefused(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Sine,1),
        SpecialBoundaryStatus64.AngleUnitRequired);
});
Probe("real Gamma pole is explicit",()=>{
    RealRefused(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Gamma,0),
        SpecialBoundaryStatus64.GammaPole);
});
Probe("exponential overflow and underflow risks preempted",()=>{
    RealRefused(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Exponential,900),
        SpecialBoundaryStatus64.OverflowRisk);
    RealRefused(SpecialFunctionBoundaryAudit64.EvaluateReal(SpecialFunctionKind64.Exponential,-900),
        SpecialBoundaryStatus64.UnderflowRisk);
});
Probe("complex negative real logarithm needs cut approval",()=>{
    ComplexRefused(SpecialFunctionBoundaryAudit64.EvaluateComplex(
        SpecialFunctionKind64.NaturalLogarithm,new Complex(-1,0)),
        SpecialBoundaryStatus64.BranchCutRequiresOptIn);
});
Probe("caller-approved complex logarithm uses principal pi",()=>{
    ComplexNear(SpecialFunctionBoundaryAudit64.EvaluateComplex(
        SpecialFunctionKind64.NaturalLogarithm,new Complex(-1,0),
        ComplexLogCutPolicy64.CallerAcceptsPrincipalCut),
        new Complex(0,global::System.Math.PI));
});
Probe("complex Gamma remains refused while T1 is retained",()=>{
    ComplexRefused(SpecialFunctionBoundaryAudit64.EvaluateComplex(
        SpecialFunctionKind64.Gamma,Complex.One),
        SpecialBoundaryStatus64.ComplexGammaUnsupported);
    var old=BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5);
    Require(old.Succeeded&&old.Value is { } value&&
        global::System.Math.Abs(value-24d)<1e-10,
        "Qualified T1 positive Gamma must remain callable.");
    var exact=BoundedExactScientific.Binomial(5,2);
    Require(exact.Succeeded&&exact.Value is { } number&&number==10,
        "Exact scientific C01 must remain callable.");
    var dependencies=typeof(SpecialFunctionBoundaryAudit64).Assembly
        .GetReferencedAssemblies().Select(x=>x.Name??"");
    Require(!dependencies.Any(x=>new[]{"MathNet","Meta.Numerics","Aura","Decision","MCDM","Angouri"}
        .Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase))),
        "An unadmitted foreign provider or agent dependency appeared.");
});
Require(total==8,"Eight independent scientific NuGet boundary probes required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C02-T2-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-special-function-boundary-consumer/1.1 qualified; probes="+total);
