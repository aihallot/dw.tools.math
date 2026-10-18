using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;
using Dw.Tools.Math.Scientific;

const string Id="dw.tools.math.scientific",Version="1.1.0-preview.3";
if(args.Length!=1)throw new ArgumentException("One local scientific boundary package is required.");
var path=Path.GetFullPath(args[0]);
if(Path.GetFileName(path)!=Id+"."+Version+".nupkg"||
   Path.GetFileName(Path.GetDirectoryName(path)) is not
       ("m6-w02-c01-t2-local-feed" or "m6-w02-c02-t1-local-feed"))
    throw new InvalidOperationException("Only the exact T2 local NuGet feed is admitted.");
var bytes=File.ReadAllBytes(path);
using(var stream=new MemoryStream(bytes,writable:false))
using(var zip=new ZipArchive(stream,ZipArchiveMode.Read))
{
    if(zip.GetEntry("lib/net10.0/"+Id+".dll") is not { Length: > 0 })
        throw new InvalidOperationException("Real net10 scientific assembly absent.");
    var specs=zip.Entries.Where(x=>x.FullName.EndsWith(".nuspec",
        StringComparison.OrdinalIgnoreCase)).ToArray();
    if(specs.Length!=1)throw new InvalidOperationException("Ambiguous scientific package.");
    using var item=specs[0].Open();
    var metadata=XDocument.Load(item).Descendants().Single(x=>x.Name.LocalName=="metadata");
    if(metadata.Elements().Single(x=>x.Name.LocalName=="id").Value!=Id||
       metadata.Elements().Single(x=>x.Name.LocalName=="version").Value!=Version||
       metadata.Descendants().Any(x=>x.Name.LocalName=="dependency"))
        throw new InvalidOperationException("Invalid isolated scientific archive identity/dependencies.");
}
static void Require(bool ok,string description)
{
    if(!ok)throw new InvalidOperationException(description);
}
static void Exact(ExactScientificAuditResult r,BigInteger expected,bool normalized=false)
{
    Require(r.Succeeded&&r.Value is { } value&&value==expected&&
        r.NegativeOperandsNormalized==normalized,"Exact boundary result differs from oracle.");
}
static void Refused(ExactScientificAuditResult r,ExactScientificStatus status)
{
    Require(!r.Succeeded&&r.Value is null&&r.Status==status&&
        r.Steps==0&&r.MaximumIntermediateBitsObserved is null,
        "Typed scientific refusal exposes a partial result.");
}
var count=0;
void Probe(string label,Action action)
{
    try{action();count++;}
    catch(Exception failure){throw new InvalidOperationException("NuGet-only scientific boundary: "+label,failure);}
}
Probe("strict negative gcd stays refused",()=>{
    Refused(ExactScientificBoundaryAudit.GreatestCommonDivisor(-48,18),
        ExactScientificStatus.InvalidInput);
});
Probe("signed magnitude gcd requires opt-in",()=>{
    Exact(ExactScientificBoundaryAudit.GreatestCommonDivisor(-48,18,
        SignedOperandPolicy.NormalizeMagnitude),6,true);
});
Probe("signed magnitude lcm requires opt-in",()=>{
    Exact(ExactScientificBoundaryAudit.LeastCommonMultiple(-6,8,
        SignedOperandPolicy.NormalizeMagnitude),24,true);
});
Probe("factorial and binomial remain exact",()=>{
    Exact(ExactScientificBoundaryAudit.Factorial(0),1);
    Exact(ExactScientificBoundaryAudit.Binomial(5,2),10);
});
Probe("signed cancellation is exactly zero",()=>{
    var r=ExactScientificBoundaryAudit.SumArithmeticSequence(1000,-2000,2);
    Exact(r,0);
    Require(r.MaximumIntermediateBitsObserved is 11,
        "An 11-bit intermediate observation is required.");
});
Probe("intermediate budget cannot be fooled by cancellation",()=>{
    Refused(ExactScientificBoundaryAudit.SumArithmeticSequence(
        1000,-2000,2,maximumIntermediateBits:8),
        ExactScientificStatus.ResultTooLarge);
});
Probe("independent operand/result/step budgets",()=>{
    Refused(ExactScientificBoundaryAudit.GreatestCommonDivisor(
        -(BigInteger.One<<129),1,SignedOperandPolicy.NormalizeMagnitude),
        ExactScientificStatus.BudgetExceeded);
    Refused(ExactScientificBoundaryAudit.Binomial(5,2,new(128,3,4096)),
        ExactScientificStatus.ResultTooLarge);
    Refused(ExactScientificBoundaryAudit.SumArithmeticSequence(
        1,1,5,budget:new(128,4096,4)),
        ExactScientificStatus.BudgetExceeded);
});
Probe("finite exactness and no agent or primality dependencies",()=>{
    Exact(ExactScientificBoundaryAudit.SumArithmeticSequence(-3,1,5),-5);
    var methods=typeof(ExactScientificBoundaryAudit).GetMethods(
        System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static);
    Require(!methods.Any(x=>x.Name.Contains("Prime",StringComparison.OrdinalIgnoreCase)),
        "No probabilistic primality claim is admitted.");
    var deps=typeof(ExactScientificBoundaryAudit).Assembly.GetReferencedAssemblies()
        .Select(x=>x.Name??"");
    Require(!deps.Any(x=>new[]{"Aura","Decision","MCDM","MathNet","Angouri"}
        .Any(y=>x.Contains(y,StringComparison.OrdinalIgnoreCase))),
        "Scientific package cannot require unqualified foreign providers.");
});
Require(count==8,"Exactly eight independently asserted scientific boundary oracles required.");
var sha=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
Console.WriteLine("M6-W02-C01-T2-CONSUMER-SHA256:"+Id+":"+Version+":"+bytes.Length+":"+sha);
Console.WriteLine("dw.tools.math/m6-exact-boundary-consumer/1.1 qualified; probes="+count);
