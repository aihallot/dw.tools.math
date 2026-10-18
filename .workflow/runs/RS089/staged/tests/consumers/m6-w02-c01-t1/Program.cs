using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.2";
if(args.Length!=1)throw new ArgumentException("Exactly one local exact-science package required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg" ||
   Path.GetFileName(Path.GetDirectoryName(path)) is not
       ("m6-w02-c01-t1-local-feed" or "m6-w02-c01-t2-local-feed"))
    throw new InvalidOperationException("Only the pinned local scientific feed is admitted.");
var bytes=File.ReadAllBytes(path);
using(var memory=new MemoryStream(bytes,writable:false))
using(var archive=new ZipArchive(memory,ZipArchiveMode.Read))
{
    if(archive.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Missing actual net10 scientific assembly.");
    var specs=archive.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous scientific nuspec.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id ||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version ||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Foreign package dependency or identity.");
}
static void Require(bool condition,string explanation)
{
    if(!condition)throw new InvalidOperationException(explanation);
}
static void Exact(ExactScientificResult result,BigInteger expected)
{
    Require(result.Succeeded&&result.Value is { } number&&number==expected,
        "A source-free BigInteger result differs from the independent exact oracle.");
}
static void Refused(ExactScientificResult result,ExactScientificStatus status)
{
    Require(!result.Succeeded&&result.Value is null&&result.Status==status,
        "The typed, nonpartial exact-science refusal was not observed.");
}
var probes=0;
void Probe(string name,Action action)
{
    try{action();probes++;}
    catch(Exception issue){throw new InvalidOperationException("Local NuGet exact scientific oracle: "+name,issue);}
}
Probe("euclidean gcd 48 18",()=>Exact(BoundedExactScientific.GreatestCommonDivisor(48,18),6));
Probe("least common multiple 6 8",()=>Exact(BoundedExactScientific.LeastCommonMultiple(6,8),24));
Probe("zero factorial exact one",()=>Exact(BoundedExactScientific.Factorial(0),1));
Probe("factorial ten",()=>Exact(BoundedExactScientific.Factorial(10),3628800));
Probe("five choose two",()=>Exact(BoundedExactScientific.Binomial(5,2),10));
Probe("arithmetic odd sequence exact sum",()=>Exact(BoundedExactScientific.SumArithmeticSequence(1,2,5),25));
Probe("signed finite arithmetic series",()=>Exact(BoundedExactScientific.SumArithmeticSequence(-3,1,5),-5));
Probe("invalid domain and output budget refuse",()=>{
    Refused(BoundedExactScientific.Binomial(-1,0),ExactScientificStatus.InvalidInput);
    Refused(BoundedExactScientific.Factorial(10,new(128,16,4096)),ExactScientificStatus.ResultTooLarge);
    var refs=typeof(BoundedExactScientific).Assembly.GetReferencedAssemblies().Select(x=>x.Name??"");
    Require(!refs.Any(x=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}
        .Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase))),
        "Exact-scientific package depends on an unqualified agent or provider.");
});
Require(probes==8,"Exactly eight source-free NuGet scientific oracles required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C01-T1-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-exact-science-consumer/1.1 qualified; probes="+probes);
