using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using dw.quantities;
using Dw.Tools.Math.Distribution;

var expected=MathLocalReleaseQualification.ExpectedPackages;
if(args.Length!=expected.Length)
    throw new ArgumentException("Exactly seven local Math archive paths required.");
var archives=new byte[expected.Length][];
for(var i=0;i<expected.Length;i++)
{
    var item=expected[i];
    var path=Path.GetFullPath(args[i]);
    if(Path.GetFileName(path)!=item.Id+"."+item.Version+".nupkg" ||
       Path.GetFileName(Path.GetDirectoryName(path))!="m5-w02-c02-t2-local-feed")
        throw new InvalidOperationException("Unexpected archive path or local feed.");
    archives[i]=File.ReadAllBytes(path);
}
var qualified=expected.Select((item,i)=>
    MathPackageRuntimeSurfaceAudit.Audit(item.Id,archives[i])).ToArray();
var passed=0;
void Probe(string label,Action check)
{
    try {check();passed++;}
    catch(Exception error)
    {
        throw new InvalidOperationException("M5 package surface probe: "+label,error);
    }
}
static void Require(bool value,string reason)
{
    if(!value)throw new InvalidOperationException(reason);
}
Probe("seven actual net10 Math archives qualify independently",()=>
{
    Require(qualified.Length==7 && qualified.Select((result,i)=>
        result.IsQualified && result.Package?.Id==expected[i].Id &&
        result.Package?.Version==expected[i].Version).All(x=>x),
        "all versioned archived assemblies must pass public-surface inspection");
});
Probe("actual public API SHA and managed assembly SHA exist",()=>
{
    Require(qualified.All(result=>result.PublicApiSha256?.Length==64 &&
        result.ManagedAssemblySha256?.Length==64 && result.PublicTypes>0 &&
        result.PublicMembers>0 && result.IlOnlyMetadataVerified),
        "the declared package manifest is not a public API certificate");
});
Probe("assembly SHA matches an independent archive DLL extraction",()=>
{
    Require(qualified.Select((result,i)=>
    {
        using var file=new MemoryStream(archives[i],writable:false);
        using var zip=new ZipArchive(file,ZipArchiveMode.Read);
        var dll=zip.GetEntry("lib/net10.0/"+expected[i].Id+".dll");
        if(dll is null)return false;
        using var content=dll.Open();
        return result.ManagedAssemblySha256==
            Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
    }).All(x=>x),"managed assembly SHA must be from actual archive bytes");
});
Probe("runtime and foreign native assets are never silently certified",()=>
{
    Require(qualified.All(result=>!result.NativeAssetsPresent &&
        !result.OtherRidsCertified &&
        result.ObservedHostRid==RuntimeInformation.RuntimeIdentifier),
        "a net10 TFM plus one host is not cross-platform certification");
});
Probe("current API fingerprint cannot promise cross-version ABI",()=>
{
    Require(qualified.All(result=>!result.CrossVersionSourceCompatibilityCertified &&
        !result.CrossVersionBinaryCompatibilityCertified &&
        !result.PerformanceSlaCertified &&
        result.RuleId==MathPackageRuntimeSurfaceAudit.Rule),
        "public API bytes alone cannot prove unrelated release compatibility");
});
Probe("malformed ZIP has no reusable binary or public API proof",()=>
{
    var refused=MathPackageRuntimeSurfaceAudit.Audit(
        "dw.tools.math.distribution",[1,2,3]);
    Require(refused.Status==MathPackageSurfaceStatus.InvalidPackage &&
        refused.Package is null && refused.PublicApiSha256 is null &&
        refused.ManagedAssemblySha256 is null,
        "no success or partial proof on malformed archives");
});
Probe("independent exact symbolic numerical sample remains correct",()=>
{
    var r=ExactMathVerticalRunner.Run(
        "where(sub(square(x),1),ne(x,1))",new ExactRational(2,1));
    Require(r.IsAdmitted && r.CanonicalDerivative=="mul(2,x)" &&
        r.ExactValueAtPoint==new ExactRational(3,1) &&
        r.OriginalRootAudit!.VerifiedRoots.Single().Value==new ExactRational(-1,1) &&
        r.OriginalRootAudit.ExcludedFromDomain.Single()==new ExactRational(1,1),
        "packaging must not alter original mathematical restrictions");
});
Probe("package-only assembly graph has no foreign agent or CAS",()=>
{
    var names=new[]{typeof(MathPackageRuntimeSurfaceAudit).Assembly,
        typeof(ExactRational).Assembly}
        .SelectMany(a=>a.GetReferencedAssemblies()).Select(a=>a.Name??"");
    Require(!names.Any(name=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
        .Any(token=>name.Contains(token,StringComparison.OrdinalIgnoreCase))),
        "this Math package cannot depend on a sibling agent or foreign CAS");
});
if(passed!=8)throw new InvalidOperationException("Eight NuGet-only surface probes required.");
for(var i=0;i<expected.Length;i++)
{
    var item=expected[i];
    Console.WriteLine("M5-W02-C02-T2-SURFACE-SHA256:"+item.Id+":"+item.Version+":"+
        qualified[i].PublicApiSha256+":"+qualified[i].ManagedAssemblySha256);
}
Console.WriteLine("OBSERVED-RUNTIME:"+RuntimeInformation.RuntimeIdentifier);
Console.WriteLine("dw.tools.math/m5-managed-api-boundary-consumer/1.0 qualified; probes="+passed);
