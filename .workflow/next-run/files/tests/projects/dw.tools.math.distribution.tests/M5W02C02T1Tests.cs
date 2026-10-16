using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using dw.quantities;
using Dw.Tools.Math.Distribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Distribution.Tests;

[TestClass]
public sealed class M5W02C02T1Tests
{
    private sealed record Fixture
    {
        public string Id { get; init; } = "dw.tools.math.distribution";
        public string? ActualId { get; init; }
        public string? Version { get; init; }
        public string? Xml { get; init; }
        public string? ExtraDependency { get; init; }
        public string? WrongDependencyVersion { get; init; }
        public string? ExtraEntry { get; init; }
        public bool Assembly { get; init; } = true;
        public bool Readme { get; init; } = true;
        public bool Notice { get; init; } = true;
        public bool ReadmeMetadata { get; init; } = true;
        public bool IncludeSpec { get; init; } = true;
        public bool DuplicateSpec { get; init; }
        public bool OmitDependencies { get; init; }
        public int PaddingEntries { get; init; }
    }

    private static void Write(ZipArchive zip,string path,string contents)
    {
        using var writer=new StreamWriter(zip.CreateEntry(path).Open(),
            new UTF8Encoding(false));
        writer.Write(contents);
    }
    private static byte[] Archive(Fixture? settings=null)
    {
        var config=settings ?? new Fixture();
        var expected=MathLocalReleaseQualification.Find(config.Id) ??
            throw new InvalidOperationException("Unexpected test manifest id.");
        using var destination=new MemoryStream();
        using(var zip=new ZipArchive(destination,ZipArchiveMode.Create,leaveOpen:true))
        {
            var direct=config.OmitDependencies ? [] :
                expected.DirectDependencies.ToArray();
            var dependencyMarkup=string.Concat(direct.Select(name=>
                "<dependency id=\""+name+"\" version=\"["+
                (config.WrongDependencyVersion ?? MathLocalReleaseQualification.Find(name)!.Version)+
                ", )\" />"));
            if(config.ExtraDependency is not null)
                dependencyMarkup+="<dependency id=\""+config.ExtraDependency+
                    "\" version=\"[1.0.0, )\" />";
            var xml=config.Xml ??
                "<package><metadata><id>"+(config.ActualId??config.Id)+
                "</id><version>"+(config.Version??expected.Version)+
                "</version>"+(config.ReadmeMetadata&&config.Id=="dw.tools.math.distribution"
                    ? "<readme>readme.md</readme>" : "")+
                "<dependencies>"+dependencyMarkup+"</dependencies></metadata></package>";
            if(config.IncludeSpec)Write(zip,config.Id+".nuspec",xml);
            if(config.DuplicateSpec)Write(zip,"another.nuspec",xml);
            if(config.Assembly)Write(zip,"lib/net10.0/"+config.Id+".dll",
                "mock-metadata-only-unit-fixture");
            if(config.Id=="dw.tools.math.distribution" && config.Readme)
                Write(zip,"readme.md","Local distribution documentation.");
            if(config.Id=="dw.tools.math.distribution" && config.Notice)
                Write(zip,"NOTICE.md","Local ownership notices.");
            if(config.ExtraEntry is not null)
                Write(zip,config.ExtraEntry,"test");
            for(var index=0;index<config.PaddingEntries;index++)
                Write(zip,"padding/"+index+".txt","x");
        }
        return destination.ToArray();
    }
    private static MathPackageQualificationResult Qualified(Fixture? settings=null)
    {
        var config=settings??new Fixture();
        var zip=Archive(config);
        var result=MathLocalReleaseQualification.Audit(config.Id,zip);
        Assert.IsTrue(result.IsQualified,"Expected admitted ZIP: "+result.Status);
        Assert.AreEqual(MathPackageQualificationStatus.Qualified,result.Status);
        Assert.AreEqual(zip.Length,result.ArchiveBytes);
        Assert.AreEqual(Convert.ToHexString(SHA256.HashData(zip)).ToLowerInvariant(),
            result.ArchiveSha256);
        Assert.AreEqual(config.Id,result.Package!.Id);
        Assert.AreEqual(MathLocalReleaseQualification.Rule,result.RuleId);
        Assert.IsFalse(result.RuntimeIdentifierCertified);
        return result;
    }
    private static MathPackageQualificationResult Refused(
        MathPackageQualificationStatus status,Fixture? settings=null)
    {
        var config=settings??new Fixture();
        var result=MathLocalReleaseQualification.Audit(config.Id,Archive(config));
        Assert.AreEqual(status,result.Status);
        Assert.IsFalse(result.IsQualified);
        Assert.IsNull(result.Package);
        Assert.IsNull(result.ArchiveSha256);
        Assert.IsNull(result.RuleId);
        Assert.AreEqual(0,result.ArchiveBytes);
        Assert.IsFalse(result.DistributionDocumentsPackaged);
        Assert.IsFalse(result.RuntimeIdentifierCertified);
        return result;
    }

    [TestMethod] public void CurrentDistributionArchiveIdentityIsQualified()
    {
        var result=Qualified();
        Assert.AreEqual("1.0.0-preview.3",result.Package!.Version);
    }
    [TestMethod] public void CurrentDistributionEmbedsReadmeAndNotice()
    {
        var result=Qualified();
        Assert.IsTrue(result.DistributionDocumentsPackaged);
    }
    [TestMethod] public void LocalQuantitiesPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.quantities",Qualified(new(){Id="dw.quantities"}).Package!.Id);
    }
    [TestMethod] public void LocalMathIrPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.tools.math.ir",
            Qualified(new(){Id="dw.tools.math.ir"}).Package!.Id);
    }
    [TestMethod] public void LocalCodecsPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.tools.math.codecs",
            Qualified(new(){Id="dw.tools.math.codecs"}).Package!.Id);
    }
    [TestMethod] public void LocalSymbolicsPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.tools.math.symbolics",
            Qualified(new(){Id="dw.tools.math.symbolics"}).Package!.Id);
    }
    [TestMethod] public void LocalNumericsPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.tools.math.numerics",
            Qualified(new(){Id="dw.tools.math.numerics"}).Package!.Id);
    }
    [TestMethod] public void LocalCompositionPackageIsAdmittedIndependently()
    {
        Assert.AreEqual("dw.tools.math.composition",
            Qualified(new(){Id="dw.tools.math.composition"}).Package!.Id);
    }
    [TestMethod] public void ExactlySevenVersionedPackagesAreDeclared()
    {
        var list=MathLocalReleaseQualification.ExpectedPackages;
        Assert.AreEqual(7,list.Length);
        Assert.AreEqual(7,list.Select(p=>p.Id).Distinct(StringComparer.Ordinal).Count());
    }
    [TestMethod] public void EveryDependencyIsDeclaredInTheSameVersionedGraph()
    {
        var graph=MathLocalReleaseQualification.ExpectedPackages;
        Assert.IsTrue(graph.All(p=>p.DirectDependencies.All(d=>
            graph.Any(known=>known.Id==d))));
    }
    [TestMethod] public void NoDependencyCanPointIntoExternalCasOrAgentPackages()
    {
        var graph=MathLocalReleaseQualification.ExpectedPackages;
        var ids=graph.SelectMany(p=>p.DirectDependencies).ToArray();
        Assert.IsFalse(ids.Any(n=>new[]{"Aura","Decision","MCDM","Angouri","MathNet"}
            .Any(term=>n.Contains(term,StringComparison.OrdinalIgnoreCase))));
    }
    [TestMethod] public void UnknownPackageHasNoQualifiedIdentity()
    {
        var result=MathLocalReleaseQualification.Audit("foreign-agent",Archive());
        Assert.AreEqual(MathPackageQualificationStatus.UnsupportedIdentity,result.Status);
        Assert.IsNull(result.Package);
        Assert.IsNull(result.ArchiveSha256);
    }
    [TestMethod] public void NullPackageIdRequiresExplicitCallerInput()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            MathLocalReleaseQualification.Audit(null!,Archive()));
    }
    [TestMethod] public void NullArchiveRequiresExplicitCallerInput()
    {
        Assert.ThrowsExactly<ArgumentNullException>(()=>
            MathLocalReleaseQualification.Audit("dw.tools.math.distribution",null!));
    }
    [TestMethod] public void EmptyArchiveHasNoVerifiedResult()
    {
        var result=MathLocalReleaseQualification.Audit(
            "dw.tools.math.distribution",[]);
        Assert.AreEqual(MathPackageQualificationStatus.InvalidArchive,result.Status);
        Assert.IsNull(result.ArchiveSha256);
    }
    [TestMethod] public void CorruptZipCannotBecomeANuGetCertificate()
    {
        var result=MathLocalReleaseQualification.Audit(
            "dw.tools.math.distribution",Encoding.UTF8.GetBytes("not a zip"));
        Assert.AreEqual(MathPackageQualificationStatus.InvalidArchive,result.Status);
        Assert.IsFalse(result.IsQualified);
    }
    [TestMethod] public void OversizedCompressedBytesAreRefusedBeforeReadingZip()
    {
        var result=MathLocalReleaseQualification.Audit(
            "dw.tools.math.distribution",
            new byte[MathLocalReleaseQualification.MaximumArchiveBytes+1]);
        Assert.AreEqual(MathPackageQualificationStatus.BudgetExceeded,result.Status);
        Assert.IsNull(result.ArchiveSha256);
    }
    [TestMethod] public void WrongEmbeddedNuspecIdCannotMasqueradeAsExpectedProduct() =>
        Refused(MathPackageQualificationStatus.UnsupportedIdentity,
            new(){ActualId="dw.tools.math.symbolics"});
    [TestMethod] public void WrongEmbeddedNuspecVersionIsNotSilentlyResolved() =>
        Refused(MathPackageQualificationStatus.UnsupportedIdentity,
            new(){Version="1.0.0-preview.4"});
    [TestMethod] public void MissingNet10AssemblyRefusesArchive()
    {
        Refused(MathPackageQualificationStatus.MissingNet10Assembly,
            new(){Assembly=false});
    }
    [TestMethod] public void MissingNuspecCannotProvePackageIdentity() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){IncludeSpec=false});
    [TestMethod] public void DuplicateNuspecCannotQualifyAnAmbiguousPackage() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){DuplicateSpec=true});
    [TestMethod] public void MissingReadmeCannotQualifyDistributionArchive() =>
        Refused(MathPackageQualificationStatus.MissingRequiredDocuments,
            new(){Readme=false});
    [TestMethod] public void MissingNoticeCannotQualifyDistributionArchive() =>
        Refused(MathPackageQualificationStatus.MissingRequiredDocuments,
            new(){Notice=false});
    [TestMethod] public void MissingNuspecReadmeMetadataCannotQualifyDistribution() =>
        Refused(MathPackageQualificationStatus.MissingRequiredDocuments,
            new(){ReadmeMetadata=false});
    [TestMethod] public void LowerLevelPackagesDoNotInventBundledNotices()
    {
        var result=Qualified(new(){Id="dw.tools.math.ir"});
        Assert.IsFalse(result.DistributionDocumentsPackaged);
    }
    [TestMethod] public void RemovingDependenciesInvalidatesTheQualifiedGraph() =>
        Refused(MathPackageQualificationStatus.DependencyMismatch,
            new(){OmitDependencies=true});
    [TestMethod] public void ForeignNewDependencyCannotLeakIntoMathGraph() =>
        Refused(MathPackageQualificationStatus.DependencyMismatch,
            new(){ExtraDependency="Aura.Kernel"});
    [TestMethod] public void WrongDependencyVersionsAreRefused() =>
        Refused(MathPackageQualificationStatus.DependencyMismatch,
            new(){WrongDependencyVersion="9.9.9"});
    [TestMethod] public void EntityOrDtdEnabledNuspecCannotBeLoaded() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){Xml="<!DOCTYPE package [<!ENTITY p 'x'>]><package>&p;</package>"});
    [TestMethod] public void MalformedNuspecIsTypedInvalidArchive() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){Xml="<package><metadata>"});
    [TestMethod] public void EmptyNuspecIsTypedInvalidArchive() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){Xml=""});
    [TestMethod] public void ArchiveTraversalEntryCannotEscapeInspectionBoundary() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){ExtraEntry="../outside.txt"});
    [TestMethod] public void WindowsBackslashZipEntryIsNeverNormalized() =>
        Refused(MathPackageQualificationStatus.InvalidArchive,
            new(){ExtraEntry="dir\\foreign.txt"});
    [TestMethod] public void TooManyZipEntriesExceedBudget()
    {
        Refused(MathPackageQualificationStatus.BudgetExceeded,
            new(){PaddingEntries=MathLocalReleaseQualification.MaximumEntries+1});
    }
    [TestMethod] public void EqualSourceBytesHaveStableDigest()
    {
        var same=Archive();
        var first=MathLocalReleaseQualification.Audit("dw.tools.math.distribution",same);
        var second=MathLocalReleaseQualification.Audit("dw.tools.math.distribution",same);
        Assert.IsTrue(first.IsQualified);
        Assert.AreEqual(first.ArchiveSha256,second.ArchiveSha256);
    }
    [TestMethod] public void ChangingNoticesChangesVerifiedArchiveDigest()
    {
        var original=Qualified();
        var without=Refused(MathPackageQualificationStatus.MissingRequiredDocuments,
            new(){Notice=false});
        Assert.IsNull(without.ArchiveSha256);
        Assert.IsNotNull(original.ArchiveSha256);
    }
    [TestMethod] public void QualifiedArchiveDoesNotCertifyAnyOperatingSystemRid()
    {
        var result=Qualified();
        Assert.IsFalse(result.RuntimeIdentifierCertified);
        Assert.IsTrue(result.IsQualified);
    }
    [TestMethod] public void QualifiedArchiveRuleIsReportedFromRuntimeResult()
    {
        var result=Qualified();
        Assert.AreEqual(MathLocalReleaseQualification.Rule,result.RuleId);
    }
    [TestMethod] public void ExistingExactPublicVerticalStillRetainsOriginalDomain()
    {
        var result=ExactMathVerticalRunner.Run(
            "where(sub(square(x),1),ne(x,1))",new ExactRational(2,1));
        Assert.IsTrue(result.IsAdmitted);
        Assert.AreEqual(new ExactRational(3,1),result.ExactValueAtPoint!.Value);
        Assert.AreEqual(new ExactRational(-1,1),
            result.OriginalRootAudit!.VerifiedRoots.Single().Value);
    }
}
