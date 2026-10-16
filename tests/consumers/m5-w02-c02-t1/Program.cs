using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using dw.quantities;
using Dw.Tools.Math.Distribution;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;

var manifest=MathLocalReleaseQualification.ExpectedPackages;
if(args.Length!=manifest.Length)
    throw new ArgumentException("Exactly seven generated local product archive paths are required.");
var bytes=new byte[manifest.Length][];
for(var index=0;index<manifest.Length;index++)
{
    var expected=manifest[index];
    var archivePath=Path.GetFullPath(args[index]);
    if(Path.GetFileName(archivePath)!=expected.Id+"."+expected.Version+".nupkg"||
       Path.GetFileName(Path.GetDirectoryName(archivePath))!="m5-w02-c02-t1-local-feed")
        throw new InvalidOperationException("Supplied archive must be in the exact selected local feed.");
    bytes[index]=File.ReadAllBytes(archivePath);
}
var results=manifest.Select((package,index)=>
    MathLocalReleaseQualification.Audit(package.Id,bytes[index])).ToArray();

var count=0;
void Probe(string label,Action check)
{
    try {check();count++;}
    catch(Exception error)
    {
        throw new InvalidOperationException(
            "M5 independent Math archive and source-free NuGet consumer: "+label,error);
    }
}
static void Require(bool success,string reason)
{
    if(!success)throw new InvalidOperationException(reason);
}
static ExactRational R(long n,long d=1)=>new(n,d);
Probe("seven versioned Math packages in an entirely local graph",()=>
{
    Require(manifest.Length==7&&
        manifest.Select(x=>x.Id).Distinct(StringComparer.Ordinal).Count()==7&&
        manifest.All(p=>p.DirectDependencies.All(dep=>manifest.Any(x=>x.Id==dep))),
        "all versioned dependencies must be declared without foreign source");
});
Probe("actual seven archives qualify with correct identities and net10 assemblies",()=>
{
    Require(results.Length==manifest.Length&&
        results.Select((r,i)=>r.IsQualified&&r.Status==MathPackageQualificationStatus.Qualified&&
            r.Package?.Id==manifest[i].Id&&r.Package?.Version==manifest[i].Version)
        .All(x=>x),"a declared package graph alone cannot qualify broken archive bytes");
});
Probe("all actual package hashes and byte lengths match independently",()=>
{
    Require(results.Select((r,i)=>
        r.ArchiveBytes==bytes[i].Length &&
        r.ArchiveSha256==
            Convert.ToHexString(SHA256.HashData(bytes[i])).ToLowerInvariant())
        .All(x=>x),"each archive needs an actual content digest");
});
Probe("distribution embeds its own README and source-rights NOTICE",()=>
{
    var dist=results.Single(x=>x.Package?.Id=="dw.tools.math.distribution");
    Require(dist.DistributionDocumentsPackaged&&dist.IsQualified,
        "the consumer must obtain real package documentation");
});
Probe("TFM support is distinct from runtime-identifier certification",()=>
{
    Require(results.All(x=>!x.RuntimeIdentifierCertified)&&
        RuntimeInformation.RuntimeIdentifier.Length>0&&
        results.All(x=>x.RuleId==MathLocalReleaseQualification.Rule),
        "a local Windows/Linux/macOS run does not certify every RID");
});
Probe("existing exact numerical and symbolic vertical remains usable",()=>
{
    var output=ExactMathVerticalRunner.Run(
        "where(sub(square(x),1),ne(x,1))",R(2));
    Require(output.IsAdmitted&&output.ExactValueAtPoint==R(3)&&
        output.CanonicalDerivative=="mul(2,x)"&&
        output.OriginalRootAudit!.VerifiedRoots.Single().Value==R(-1)&&
        output.OriginalRootAudit.ExcludedFromDomain.Single()==R(1)&&
        !output.OriginalRootAudit.IsComplete&&
        output.Binary64Projection?.Status==RationalBinary64Status.Exact,
        "the release artifact must preserve the qualified mathematical recipe");
});
Probe("bad local archive is refused without mathematical or package proof",()=>
{
    var result=MathLocalReleaseQualification.Audit(
        "dw.tools.math.distribution",[1,2,3,4]);
    Require(result.Status==MathPackageQualificationStatus.InvalidArchive&&
        !result.IsQualified&&result.Package is null&&result.ArchiveSha256 is null,
        "a broken ZIP cannot be a package qualification receipt");
});
Probe("no AURA, foreign CAS or sibling-source dependency leaked",()=>
{
    var names=new[]{typeof(MathLocalReleaseQualification).Assembly,
        typeof(IrNode).Assembly,typeof(ExactRational).Assembly}
        .SelectMany(a=>a.GetReferencedAssemblies())
        .Select(a=>a.Name??string.Empty);
    Require(!names.Any(name=>
        new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
            .Any(term=>name.Contains(term,StringComparison.OrdinalIgnoreCase)))&&
        MathLocalReleaseQualification.Find("Aura.Kernel") is null,
        "local packages must remain independent of brokers and external CAS");
});
if(count!=8)
    throw new InvalidOperationException("Eight independent clean NuGet release oracles required.");

for(var index=0;index<manifest.Length;index++)
{
    var package=manifest[index];
    var receipt=results[index];
    Console.WriteLine("M5-W02-C02-T1-CONSUMER-SHA256:"+package.Id+":"+
        package.Version+":"+receipt.ArchiveBytes+":"+receipt.ArchiveSha256);
}
Console.WriteLine("dw.tools.math/m5-clean-package-consumer/1.0 qualified; probes="+count);
Console.WriteLine("OBSERVED-RUNTIME:"+RuntimeInformation.RuntimeIdentifier);
