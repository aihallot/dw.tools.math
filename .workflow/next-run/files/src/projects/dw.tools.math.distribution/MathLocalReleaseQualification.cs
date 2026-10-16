using System.Collections.Immutable;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace Dw.Tools.Math.Distribution;

/// <summary>Declared package and dependencies, not a certification receipt.</summary>
public sealed record MathPackageIdentity(
    string Id, string Version, ImmutableArray<string> DirectDependencies);

public enum MathPackageQualificationStatus
{
    Qualified, InvalidArchive, UnsupportedIdentity, DependencyMismatch,
    MissingNet10Assembly, MissingRequiredDocuments, BudgetExceeded
}

/// <summary>Admitted only after inspecting actual local NuGet archive bytes.</summary>
public sealed record MathPackageQualificationResult(
    MathPackageQualificationStatus Status, MathPackageIdentity? Package,
    string? ArchiveSha256, int ArchiveBytes, bool DistributionDocumentsPackaged,
    bool RuntimeIdentifierCertified, string? RuleId)
{
    public bool IsQualified =>
        Status == MathPackageQualificationStatus.Qualified &&
        Package is not null && ArchiveSha256 is not null &&
        ArchiveBytes > 0 && !RuntimeIdentifierCertified && RuleId is not null;
}

/// <summary>
/// Closed seven-package local Math graph and read-only BCL archive audit.
/// No archive extraction, network, release publication or RID certification.
/// </summary>
public static class MathLocalReleaseQualification
{
    public const string Rule = "dw-math-local-package-archive-net10/1";
    public const string TargetFramework = "net10.0";
    public const int MaximumArchiveBytes = 4_194_304;
    public const int MaximumEntries = 256;
    public const long MaximumUncompressedBytes = 16_777_216;

    private static readonly ImmutableArray<MathPackageIdentity> Packages =
    [
        new("dw.quantities","0.2.0-preview.1",[]),
        new("dw.tools.math.ir","0.3.0-preview.1",["dw.quantities"]),
        new("dw.tools.math.codecs","1.0.0-preview.8",["dw.tools.math.ir"]),
        new("dw.tools.math.symbolics","0.5.0-preview.10",["dw.tools.math.ir"]),
        new("dw.tools.math.numerics","0.4.0-preview.4",["dw.tools.math.ir"]),
        new("dw.tools.math.composition","0.3.0-preview.1",
            ["dw.quantities","dw.tools.math.ir"]),
        new("dw.tools.math.distribution","1.0.0-preview.4",
            ["dw.quantities","dw.tools.math.ir","dw.tools.math.codecs",
             "dw.tools.math.symbolics","dw.tools.math.numerics",
             "dw.tools.math.composition"])
    ];

    public static ImmutableArray<MathPackageIdentity> ExpectedPackages => Packages;

    public static MathPackageIdentity? Find(string? packageId) =>
        Packages.FirstOrDefault(item =>
            string.Equals(item.Id,packageId,StringComparison.Ordinal));

    public static MathPackageQualificationResult Audit(string packageId,byte[] archive)
    {
        ArgumentNullException.ThrowIfNull(packageId);
        ArgumentNullException.ThrowIfNull(archive);
        var expected=Find(packageId);
        if(expected is null)
            return Refuse(MathPackageQualificationStatus.UnsupportedIdentity);
        if(archive.Length==0)
            return Refuse(MathPackageQualificationStatus.InvalidArchive);
        if(archive.Length>MaximumArchiveBytes)
            return Refuse(MathPackageQualificationStatus.BudgetExceeded);

        try
        {
            using var input=new MemoryStream(archive,writable:false);
            using var zip=new ZipArchive(input,ZipArchiveMode.Read,leaveOpen:false);
            var entries=zip.Entries;
            if(entries.Count>MaximumEntries ||
                entries.Sum(e=>e.Length)>MaximumUncompressedBytes ||
                entries.Any(e=>e.Length>MaximumUncompressedBytes))
                return Refuse(MathPackageQualificationStatus.BudgetExceeded);
            if(entries.GroupBy(e=>e.FullName,StringComparer.Ordinal)
                    .Any(group=>group.Count()>1) ||
                entries.Any(e=>e.FullName.StartsWith("/",StringComparison.Ordinal) ||
                    e.FullName.Contains('\\') ||
                    e.FullName.Split('/').Contains("..",StringComparer.Ordinal)))
                return Refuse(MathPackageQualificationStatus.InvalidArchive);

            var assembly=zip.GetEntry("lib/net10.0/"+expected.Id+".dll");
            if(assembly is null || assembly.Length<=0)
                return Refuse(MathPackageQualificationStatus.MissingNet10Assembly);
            var specEntries=entries.Where(e=>e.FullName.EndsWith(".nuspec",
                StringComparison.OrdinalIgnoreCase)).ToArray();
            if(specEntries.Length!=1 || specEntries[0].Length<=0 ||
                specEntries[0].Length>65_536)
                return Refuse(MathPackageQualificationStatus.InvalidArchive);

            using var stream=specEntries[0].Open();
            using var reader=XmlReader.Create(stream,new XmlReaderSettings
            {
                DtdProcessing=DtdProcessing.Prohibit,
                XmlResolver=null,
                MaxCharactersFromEntities=0,
                MaxCharactersInDocument=65_536
            });
            var root=XDocument.Load(reader);
            var metadata=root.Descendants().Single(e=>e.Name.LocalName=="metadata");
            var actualId=metadata.Elements()
                .Single(e=>e.Name.LocalName=="id").Value;
            var actualVersion=metadata.Elements()
                .Single(e=>e.Name.LocalName=="version").Value;
            if(!string.Equals(actualId,expected.Id,StringComparison.Ordinal) ||
               !string.Equals(actualVersion,expected.Version,StringComparison.Ordinal))
                return Refuse(MathPackageQualificationStatus.UnsupportedIdentity);

            var dependencies=metadata.Descendants().Where(e=>
                e.Name.LocalName=="dependency").ToArray();
            var actualIds=dependencies.Select(e=>(string?)e.Attribute("id")??"")
                .OrderBy(value=>value,StringComparer.Ordinal).ToArray();
            var expectedIds=expected.DirectDependencies
                .OrderBy(value=>value,StringComparer.Ordinal).ToArray();
            if(!actualIds.SequenceEqual(expectedIds,StringComparer.Ordinal))
                return Refuse(MathPackageQualificationStatus.DependencyMismatch);
            foreach(var dependency in dependencies)
            {
                var id=(string?)dependency.Attribute("id")??"";
                var required=Find(id);
                var range=(string?)dependency.Attribute("version");
                if(required is null || range is null ||
                    !range.Contains(required.Version,StringComparison.Ordinal))
                    return Refuse(MathPackageQualificationStatus.DependencyMismatch);
            }

            var distribution=expected.Id=="dw.tools.math.distribution";
            var documents=distribution &&
                zip.GetEntry("readme.md") is { Length: > 0 } &&
                zip.GetEntry("NOTICE.md") is { Length: > 0 } &&
                metadata.Elements().Any(e=>e.Name.LocalName=="readme" &&
                    e.Value=="readme.md");
            if(distribution && !documents)
                return Refuse(MathPackageQualificationStatus.MissingRequiredDocuments);

            var digest=Convert.ToHexString(SHA256.HashData(archive))
                .ToLowerInvariant();
            return new(MathPackageQualificationStatus.Qualified,expected,
                digest,archive.Length,documents,false,Rule);
        }
        catch(Exception error) when (error is InvalidDataException or
            XmlException or IOException or InvalidOperationException or
            ArgumentException or OverflowException)
        {
            return Refuse(MathPackageQualificationStatus.InvalidArchive);
        }
    }

    private static MathPackageQualificationResult Refuse(
        MathPackageQualificationStatus status) =>
        new(status,null,null,0,false,false,null);
}
