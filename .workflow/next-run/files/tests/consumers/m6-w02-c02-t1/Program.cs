using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.3";
if(args.Length!=1)throw new ArgumentException("Exactly one local scientific-functions NuGet archive required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path))!="m6-w02-c02-t1-local-feed")
    throw new InvalidOperationException("Only qualified local C02 T1 special-function feed accepted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Actual net10 scientific provider assembly missing.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous NuGet metadata.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Package identity or dependency drift.");
}
static void Require(bool ok,string reason)
{
    if(!ok)throw new InvalidOperationException(reason);
}
static void Real(SpecialRealResult64 value,double expected,double tol=1e-10)
{
    Require(value.Succeeded&&value.Value is { } actual&&
        double.IsFinite(actual)&&global::System.Math.Abs(actual-expected)<=tol,
        "Independent real scientific oracle not satisfied.");
}
static void ComplexNear(SpecialComplexResult64 value,Complex expected,double tol=1e-10)
{
    Require(value.Succeeded&&value.Value is { } actual&&
        global::System.Math.Abs(actual.Real-expected.Real)<=tol&&
        global::System.Math.Abs(actual.Imaginary-expected.Imaginary)<=tol,
        "Independent complex principal-value oracle not satisfied.");
}
var probes=0;
void Probe(string label,Action action)
{
    try{action();probes++;}
    catch(Exception e){throw new InvalidOperationException("Local scientific-function probe: "+label,e);}
}
Probe("real sin of zero",()=>Real(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Sine,0),0));
Probe("real cos pi",()=>Real(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Cosine,global::System.Math.PI),-1));
Probe("natural log of one",()=>Real(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.NaturalLogarithm,1),0));
Probe("Gamma five via managed Lanczos",()=>Real(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,5),24,1e-10));
Probe("positive half Gamma reference",()=>Real(BoundedSpecialFunctions64.EvaluateReal(SpecialFunctionKind64.Gamma,0.5),global::System.Math.Sqrt(global::System.Math.PI),1e-11));
Probe("complex exp i pi principal result",()=>ComplexNear(BoundedSpecialFunctions64.EvaluateComplex(SpecialFunctionKind64.Exponential,new Complex(0,global::System.Math.PI)),new Complex(-1,0)));
Probe("complex log minus one principal phase",()=>ComplexNear(BoundedSpecialFunctions64.EvaluateComplex(SpecialFunctionKind64.NaturalLogarithm,new Complex(-1,0)),new Complex(0,global::System.Math.PI)));
Probe("unsupported complex Gamma and retained exact integers",()=>{
    var refused=BoundedSpecialFunctions64.EvaluateComplex(SpecialFunctionKind64.Gamma,Complex.One);
    Require(!refused.Succeeded&&refused.Value is null&&
        refused.Status==SpecialFunctionStatus64.UnsupportedDomain,
        "Complex Gamma is out of the admitted T1 subset.");
    var old=BoundedExactScientific.Binomial(5,2);
    Require(old.Succeeded&&old.Value is { } result&&result==10,
        "Retained exact combinatorics must remain callable.");
    var refs=typeof(BoundedSpecialFunctions64).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");
    Require(!refs.Any(x=>new[]{"MathNet","Meta.Numerics","Aura","Decision","MCDM","Angouri"}
        .Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase))),
        "No unqualified foreign provider or agent dependency.");
});
Require(probes==8,"Exactly eight independently asserted package-function probes required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C02-T1-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-special-functions-consumer/1.1 qualified; probes="+probes);
