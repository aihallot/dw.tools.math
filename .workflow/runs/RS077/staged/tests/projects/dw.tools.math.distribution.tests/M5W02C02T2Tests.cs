using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Codecs;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Distribution;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Numerics;
using Dw.Tools.Math.Symbolics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C02T2Tests
{
    private const string PackageId="dw.tools.math.distribution";

    private sealed record Fixture(
        string Id=PackageId,
        bool NativeRuntime=false,
        bool NativeRoot=false,
        string? NativeExtension=null,
        bool MissingAssembly=false,
        bool BrokenManagedBytes=false,
        bool ForeignManagedIdentity=false,
        bool MissingReadme=false,
        bool MissingNotice=false,
        bool WrongVersion=false,
        bool MissingDependencies=false,
        bool CorruptNuspec=false,
        string? ExtraEntry=null);

    private static Assembly AssemblyFor(string id) => id switch
    {
        "dw.quantities" => typeof(ExactRational).Assembly,
        "dw.tools.math.ir" => typeof(IrNode).Assembly,
        "dw.tools.math.codecs" => typeof(CanonicalMathNotation).Assembly,
        "dw.tools.math.symbolics" => typeof(SymbolicExactDifferentiation).Assembly,
        "dw.tools.math.numerics" => typeof(RationalBinary64Conversion).Assembly,
        "dw.tools.math.composition" => typeof(MathOperationResult).Assembly,
        PackageId => typeof(MathPackageRuntimeSurfaceAudit).Assembly,
        _ => throw new ArgumentException("Unknown local package fixture.")
    };

    private static void Put(ZipArchive zip,string path,string text)
    {
        using var writer=new StreamWriter(zip.CreateEntry(path).Open(),new UTF8Encoding(false));
        writer.Write(text);
    }

    private static byte[] Archive(Fixture? input=null)
    {
        var fixture=input??new Fixture();
        var declared=MathLocalReleaseQualification.Find(fixture.Id)!;
        using var output=new MemoryStream();
        using(var zip=new ZipArchive(output,ZipArchiveMode.Create,leaveOpen:true))
        {
            var deps=fixture.MissingDependencies ? Array.Empty<string>() :
                declared.DirectDependencies.ToArray();
            var xml=fixture.CorruptNuspec?"<package><metadata>":
                "<package><metadata><id>"+fixture.Id+"</id><version>"+
                (fixture.WrongVersion?"0.0.0":declared.Version)+"</version>"+
                (fixture.Id==PackageId?"<readme>readme.md</readme>":"")+
                "<dependencies>"+string.Concat(deps.Select(name=>
                    "<dependency id=\""+name+"\" version=\"["+
                    MathLocalReleaseQualification.Find(name)!.Version+", )\" />"))+
                "</dependencies></metadata></package>";
            Put(zip,fixture.Id+".nuspec",xml);
            if(!fixture.MissingAssembly)
            {
                var code=fixture.BrokenManagedBytes?new byte[]{1,2,3,4,5,6,7,8}:
                    File.ReadAllBytes(AssemblyFor(fixture.ForeignManagedIdentity?
                        "dw.quantities":fixture.Id).Location);
                using var entry=zip.CreateEntry("lib/net10.0/"+fixture.Id+".dll").Open();
                entry.Write(code);
            }
            if(fixture.Id==PackageId)
            {
                if(!fixture.MissingReadme)Put(zip,"readme.md","Local Math README.");
                if(!fixture.MissingNotice)Put(zip,"NOTICE.md","Math notices.");
            }
            if(fixture.NativeRuntime)
                Put(zip,"runtimes/linux-x64/native/libnative.so","native");
            if(fixture.NativeRoot)Put(zip,"native/module.bin","native");
            if(fixture.NativeExtension is not null)
                Put(zip,"content/lib"+fixture.NativeExtension,"native");
            if(fixture.ExtraEntry is not null)
                Put(zip,fixture.ExtraEntry,"source documentation");
        }
        return output.ToArray();
    }

    private static MathPackageSurfaceResult Admit(string id=PackageId,Fixture? input=null)
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit(id,Archive(input??new Fixture(Id:id)));
        Assert.IsTrue(result.IsQualified,"Expected managed package qualification: "+result.Status);
        Assert.AreEqual(MathPackageSurfaceStatus.Qualified,result.Status);
        Assert.AreEqual(id,result.Package!.Id);
        Assert.AreEqual(64,result.ManagedAssemblySha256!.Length);
        Assert.AreEqual(64,result.PublicApiSha256!.Length);
        Assert.AreEqual(MathPackageRuntimeSurfaceAudit.Rule,result.RuleId);
        Assert.IsTrue(result.PublicTypes>0);
        Assert.IsTrue(result.PublicMembers>0);
        Assert.IsTrue(result.IlOnlyMetadataVerified);
        Assert.IsFalse(result.NativeAssetsPresent);
        Assert.IsFalse(result.OtherRidsCertified);
        Assert.IsFalse(result.CrossVersionSourceCompatibilityCertified);
        Assert.IsFalse(result.CrossVersionBinaryCompatibilityCertified);
        Assert.IsFalse(result.PerformanceSlaCertified);
        return result;
    }

    private static void Refuse(MathPackageSurfaceStatus expected,Fixture fixture)
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit(fixture.Id,Archive(fixture));
        Assert.AreEqual(expected,result.Status);
        Assert.IsFalse(result.IsQualified);
        Assert.IsNull(result.Package);
        Assert.IsNull(result.ManagedAssemblySha256);
        Assert.IsNull(result.PublicApiSha256);
        Assert.IsNull(result.RuleId);
    }

    [TestMethod]
    public void DistributionManagedAssemblyQualified()
    {
        var result=Admit(); Assert.IsTrue(result.PublicTypes>3);
    }

    [TestMethod]
    public void QuantitiesRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.quantities").IsQualified);
    }

    [TestMethod]
    public void IrRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.tools.math.ir").IsQualified);
    }

    [TestMethod]
    public void CodecsRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.tools.math.codecs").IsQualified);
    }

    [TestMethod]
    public void SymbolicsRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.tools.math.symbolics").IsQualified);
    }

    [TestMethod]
    public void NumericsRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.tools.math.numerics").IsQualified);
    }

    [TestMethod]
    public void CompositionRealDllMetadataIsQualified()
    {
        Assert.IsTrue(Admit("dw.tools.math.composition").IsQualified);
    }

    [TestMethod]
    public void ManifestStillPinsExactlySevenPackages()
    {
        var expected=MathLocalReleaseQualification.ExpectedPackages; Assert.AreEqual(7,expected.Length); Assert.AreEqual("1.0.0-preview.4",expected.Single(x=>x.Id==PackageId).Version);
    }

    [TestMethod]
    public void SameBytesProduceIdenticalPublicApiDigest()
    {
        var bytes=Archive();var first=MathPackageRuntimeSurfaceAudit.Audit(PackageId,bytes);var second=MathPackageRuntimeSurfaceAudit.Audit(PackageId,bytes);Assert.IsTrue(first.IsQualified);Assert.AreEqual(first.PublicApiSha256,second.PublicApiSha256);
    }

    [TestMethod]
    public void ActualManagedDllBytesAreShaBound()
    {
        var result=Admit();var bytes=File.ReadAllBytes(typeof(MathPackageRuntimeSurfaceAudit).Assembly.Location);Assert.AreEqual(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),result.ManagedAssemblySha256);
    }

    [TestMethod]
    public void ObservedHostRidIsNotAnotherPlatformCertificate()
    {
        var result=Admit();Assert.AreEqual(System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,result.ObservedHostRid);Assert.IsFalse(result.OtherRidsCertified);
    }

    [TestMethod]
    public void CurrentPublicSurfaceIsNotCrossVersionSourceProof()
    {
        Assert.IsFalse(Admit().CrossVersionSourceCompatibilityCertified);
    }

    [TestMethod]
    public void CurrentPublicSurfaceIsNotCrossVersionBinaryProof()
    {
        Assert.IsFalse(Admit().CrossVersionBinaryCompatibilityCertified);
    }

    [TestMethod]
    public void NoPortablePerformanceSlaIsConjectured()
    {
        Assert.IsFalse(Admit().PerformanceSlaCertified);
    }

    [TestMethod]
    public void RealManagedDllHasPublicMembers()
    {
        var result=Admit();Assert.IsTrue(result.PublicMembers>5);
    }

    [TestMethod]
    public void NativeLinuxRuntimeAssetsExplicitlyRefused()
    {
        Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,new(NativeRuntime:true));
    }

    [TestMethod]
    public void NativeRootAssetsExplicitlyRefused()
    {
        Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,new(NativeRoot:true));
    }

    [TestMethod]
    public void DylibExtensionCannotBecomeManagedOnly()
    {
        Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,new(NativeExtension:".dylib"));
    }

    [TestMethod]
    public void StaticArchiveExtensionCannotBecomeManagedOnly()
    {
        Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,new(NativeExtension:".a"));
    }

    [TestMethod]
    public void WindowsNativeLinkLibraryRefused()
    {
        Refuse(MathPackageSurfaceStatus.NativeAssetsNotQualified,new(NativeExtension:".lib"));
    }

    [TestMethod]
    public void HarmlessExtraMarkdownIsNotNative()
    {
        Assert.IsTrue(Admit(input:new(ExtraEntry:"docs/api.md")).IsQualified);
    }

    [TestMethod]
    public void IncorrectManagedAssemblyIdentityRefused()
    {
        Refuse(MathPackageSurfaceStatus.InvalidMetadata,new(ForeignManagedIdentity:true));
    }

    [TestMethod]
    public void BrokenPeDoesNotGetPublicProof()
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit(PackageId,Archive(new(BrokenManagedBytes:true)));Assert.IsFalse(result.IsQualified);Assert.IsNull(result.PublicApiSha256);
    }

    [TestMethod]
    public void MissingDllRefusedBeforeRuntimeInspection()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(MissingAssembly:true));
    }

    [TestMethod]
    public void MissingReadmeDoesNotGetNativeCertificate()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(MissingReadme:true));
    }

    [TestMethod]
    public void MissingNoticesDoNotGetApiCertificate()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(MissingNotice:true));
    }

    [TestMethod]
    public void WrongNuspecVersionCannotBePromoted()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(WrongVersion:true));
    }

    [TestMethod]
    public void MissingDeclaredDependenciesAreRefused()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(MissingDependencies:true));
    }

    [TestMethod]
    public void MalformedNuspecIsRefused()
    {
        Refuse(MathPackageSurfaceStatus.InvalidPackage,new(CorruptNuspec:true));
    }

    [TestMethod]
    public void ForeignPackageIdHasNoPublicSurface()
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit("foreign.agent",Archive());Assert.AreEqual(MathPackageSurfaceStatus.InvalidPackage,result.Status);Assert.IsNull(result.Package);
    }

    [TestMethod]
    public void NullIdIsCallerError()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>MathPackageRuntimeSurfaceAudit.Audit(null!,Archive()));
    }

    [TestMethod]
    public void NullArchiveIsCallerError()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>MathPackageRuntimeSurfaceAudit.Audit(PackageId,null!));
    }

    [TestMethod]
    public void CorruptZipHasNoApiDigest()
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit(PackageId,[1,2,3]);Assert.AreEqual(MathPackageSurfaceStatus.InvalidPackage,result.Status);Assert.IsNull(result.PublicApiSha256);
    }

    [TestMethod]
    public void OverlongArchiveNeverComputesApiDigest()
    {
        var result=MathPackageRuntimeSurfaceAudit.Audit(PackageId,new byte[MathLocalReleaseQualification.MaximumArchiveBytes+1]);Assert.AreEqual(MathPackageSurfaceStatus.BudgetExceeded,result.Status);Assert.IsNull(result.PublicApiSha256);
    }

    [TestMethod]
    public void ExistingExactMathVerticalStillWorks()
    {
        var result=ExactMathVerticalRunner.Run("where(sub(square(x),1),ne(x,1))",new ExactRational(2,1));Assert.IsTrue(result.IsAdmitted);Assert.AreEqual(new ExactRational(3,1),result.ExactValueAtPoint!.Value);
    }

    [TestMethod]
    public void NoForeignAgentRuntimeAssemblyNeeded()
    {
        var names=typeof(MathPackageRuntimeSurfaceAudit).Assembly.GetReferencedAssemblies().Select(a=>a.Name??"");Assert.IsFalse(names.Any(n=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}.Any(token=>n.Contains(token,StringComparison.OrdinalIgnoreCase))));
    }
}
