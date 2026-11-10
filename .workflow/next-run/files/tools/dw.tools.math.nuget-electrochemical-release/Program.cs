using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetElectrochemicalRelease;

internal static class Program
{
    private const string PackageId = "dw.quantities.standard";
    private const string Version = "20260.20261110.1001";
    private const string ArchiveSha = "766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d";
    private const string DllSha = "9020646c909cc1a5408c19f51919681a8e21d43024ec8530a2119b328e8dd17a";
    private const string PublisherSha = "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string CandidateReceipt = "docs/distribution/electrochemical-nuget-candidate-qualified.json";
    private const string CandidatePath = "build/artifacts/rs174-electrochemical-candidate/ae18522f6d6d4ea4a827439af4a9d300/candidate/dw.quantities.standard.20260.20261110.1001.nupkg";
    private const string OperatorRoot = @"D:\data.dev\github\ai@hallot.net\dw.tools.math";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };
    private static readonly (string Leaf, long Bytes, string Sha)[] PreservedArchives =
    [
        ("dw.quantities.20260.20261102.1001.nupkg", 14747,
            "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d"),
        ("dw.quantities.expression.20260.20261102.1001.nupkg", 13951,
            "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae"),
        ("dw.quantities.standard.20260.20261103.1001.nupkg", 13604,
            "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75")
    ];

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 2 && args[0] == "--rehearse-publisher",
                "Expected --rehearse-publisher <repository-root>; shared-feed publication is not supported by this gate.");
            Rehearse(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-ELECTROCHEM-PUBLISHER-REFUSED: " +
                error.GetType().Name + ": " + error.Message);
            return 1;
        }
    }

    private static void Rehearse(string root)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Repo(root, CandidateReceipt)));
        var receipt = document.RootElement;
        var candidate = receipt.GetProperty("candidate");
        Need(Text(receipt, "status") ==
                "rs174-electrochemical-standard-calendar-candidate-qualified-unpublished" &&
             !receipt.GetProperty("package_published").GetBoolean() &&
             !receipt.GetProperty("aura_code_reuse_rights_established").GetBoolean() &&
             Text(candidate, "packageId") == PackageId &&
             Text(candidate, "packageVersion") == Version &&
             Text(candidate, "candidateArchivePath") == CandidatePath &&
             Text(candidate, "candidateArchiveSha256") == ArchiveSha &&
             Text(candidate, "candidateDllSha256") == DllSha &&
             candidate.GetProperty("candidateArchiveBytes").GetInt64() == 14269 &&
             candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
             candidate.GetProperty("consumerChecks").GetInt32() == 12 &&
             candidate.GetProperty("loadedThreeDllHashesVerified").GetBoolean() &&
             candidate.GetProperty("legacyCataloguesPreserved").GetBoolean() &&
             !candidate.GetProperty("published").GetBoolean(),
             "RS174 original candidate identity or acquired qualification changed");
        VerifySources(root, candidate);
        var publisher = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(publisher.GetName().Name, "dw.tools.nuget", StringComparison.OrdinalIgnoreCase) &&
             Sha(publisher.Location) == PublisherSha,
             "loaded shared NuGet publisher differs from the exact qualified DLL");
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL is required");
        // Constructor retains the shared path/ancestor safety checks, but no List
        // scans unrelated packages in the operator's shared flat feed.
        _ = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        VerifyPreservedArchives(feed);
        var targetLeaf = PackageId + "." + Version + ".nupkg";
        Need(!File.Exists(Path.Combine(feed, targetLeaf)),
             "RS175 requires the candidate to remain unpublished in NUGET_LOCAL");

        // Disposable DWF preparation has no ignored build artifacts. Its only
        // external source read is this exact approved operator archive, never
        // a directory search or a substitute checkout used for execution.
        var localSource = Repo(root, CandidatePath);
        var borrowedOperatorArchive = !File.Exists(localSource);
        var source = borrowedOperatorArchive ? Repo(OperatorRoot, CandidatePath) : localSource;
        Need(File.Exists(source), "original RS174 archive missing; rebuilding or substitution is forbidden");
        _ = new DotNetLocalPackageRepository(Path.GetDirectoryName(source) ??
            throw new InvalidDataException("original archive directory missing"));
        var inventory = candidate.GetProperty("packagedFileInventory");
        VerifyArchive(source, inventory);

        var scratch = Repo(root, "build/artifacts/rs175-electrochemical-publisher/" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(scratch), "publisher rehearsal needs a fresh owned workspace");
        Directory.CreateDirectory(scratch);
        var controls = new List<string>();
        try
        {
            var disposable = Path.Combine(scratch, "disposable-feed");
            Directory.CreateDirectory(disposable);
            Directory.CreateDirectory(Path.Combine(disposable, "ignored-child"));
            var repository = new DotNetLocalPackageRepository(disposable,
                new DotNetLocalRepositoryLimits(32, 67_108_864, 67_108_864));
            var prepared = repository.PreparePublication(source, ArchiveSha,
                CancellationToken.None, ignoreSubdirectories: true);
            Need(Path.GetFullPath(prepared.Receipt.Path) == Path.Combine(disposable, targetLeaf) &&
                 !File.Exists(prepared.Receipt.Path) &&
                 !Directory.EnumerateFiles(disposable, "*", SearchOption.TopDirectoryOnly).Any(),
                 "preparation must select only the disposable target and write no file");
            controls.Add("prepare-no-write");

            var published = repository.Publish(prepared, CancellationToken.None);
            Need(published.Path == prepared.Receipt.Path && published.PackageId == PackageId &&
                 published.Version == Version && published.Sha256 == ArchiveSha && published.Bytes == 14269,
                 "publisher changed original RS174 archive identity");
            VerifyArchive(published.Path, inventory);
            controls.Add("exact-archive-publish");

            // A separate repository reader inspects the physically written copy.
            var readback = new DotNetLocalPackageRepository(disposable,
                new DotNetLocalRepositoryLimits(32, 67_108_864, 67_108_864))
                .List(PackageId, ignoreSubdirectories: true);
            Need(readback.Complete && readback.InspectedPackages == 1 &&
                 readback.MatchedPackages == 1 && readback.DirectoryPolicy == "ignore" &&
                 readback.SkippedDirectories == 1 &&
                 readback.Packages.Single().Version == Version &&
                 readback.Packages.Single().Sha256 == ArchiveSha &&
                 readback.Packages.Single().Bytes == 14269,
                 "independent disposable-feed readback did not observe exact original bytes");
            controls.Add("independent-archive-readback");

            Refuses(() => repository.PreparePublication(source, ArchiveSha,
                CancellationToken.None, ignoreSubdirectories: true), "collision");
            controls.Add("collision-refused");
            Refuses(() => repository.PreparePublication(source, new string('0', 64),
                CancellationToken.None, ignoreSubdirectories: true), "digest_mismatch");
            controls.Add("digest-mismatch-refused");
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancelledRefused = false;
            try
            {
                _ = repository.PreparePublication(source, ArchiveSha,
                    cancelled.Token, ignoreSubdirectories: true);
            }
            catch (OperationCanceledException) { cancelledRefused = true; }
            Need(cancelledRefused, "cancelled publication preparation must refuse");
            controls.Add("cancellation-refused");

            VerifyArchive(source, inventory);
            VerifyArchive(published.Path, inventory);
            VerifySources(root, candidate);
            VerifyPreservedArchives(feed);
            Need(!File.Exists(Path.Combine(feed, targetLeaf)) &&
                 Directory.EnumerateFiles(disposable, "*", SearchOption.TopDirectoryOnly).Count() == 1,
                 "rehearsal altered shared publication state or left an extra temporary file");
            controls.Add("original-inputs-preserved");
            Need(controls.Count == 7 && controls.Distinct(StringComparer.Ordinal).Count() == 7,
                 "seven distinct publisher controls required");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                status = "qualified-original-electrochemical-publisher-unpublished",
                packageId = PackageId,
                packageVersion = Version,
                originalArchiveSha256 = ArchiveSha,
                originalArchiveBytes = 14269,
                originalDllSha256 = DllSha,
                originalArchiveReadFrom = source,
                borrowedOperatorArchive,
                publisherPackageVersion = "20260.20261026.1010",
                publisherDllSha256 = PublisherSha,
                controlsPassed = controls.Count,
                controls,
                archiveInventoryEntriesVerified = inventory.GetArrayLength(),
                disposableFeedPublished = true,
                independentArchiveReadback = true,
                packageRebuilt = false,
                sourceCampaignReplayed = false,
                binaryCampaignReplayed = false,
                publishedToNugetLocal = false,
                originalCandidatePreserved = true,
                sourceFilesPreserved = true,
                requiredPublishedArchivesPreserved = PreservedArchives.Length,
                auraCodeReuseRightsEstablished = false,
                installedRollbackQualified = false
            }, Pretty));
        }
        finally
        {
            Directory.Delete(scratch, recursive: true);
        }
    }

    private static void VerifySources(string root, JsonElement candidate)
    {
        var hashes = candidate.GetProperty("originalSourcesSha256");
        Need(hashes.EnumerateObject().Count() == 6 &&
             Sha(Repo(root, "src/projects/dw.quantities.standard/dw.quantities.standard.csproj")) ==
                 Text(candidate, "originalProjectSha256"), "original Standard project identity changed");
        foreach (var property in hashes.EnumerateObject())
        {
            Need(property.Name.StartsWith("src/projects/dw.quantities.standard/", StringComparison.Ordinal) &&
                 !property.Name.Contains("..", StringComparison.Ordinal) &&
                 Sha(Repo(root, property.Name)) == property.Value.GetString(),
                 "an original Math Standard source changed: " + property.Name);
        }
    }

    private static void VerifyPreservedArchives(string feed)
    {
        foreach (var archive in PreservedArchives)
        {
            var path = Path.Combine(feed, archive.Leaf);
            Need(File.Exists(path) &&
                 (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
                 new FileInfo(path).Length == archive.Bytes && Sha(path) == archive.Sha,
                 "immutable published prerequisite changed: " + archive.Leaf);
        }
    }

    private static void VerifyArchive(string path, JsonElement inventory)
    {
        Need(File.Exists(path) &&
             (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
             new FileInfo(path).Length == 14269 && Sha(path) == ArchiveSha,
             "exact original RS174 archive bytes are required");
        using var zip = ZipFile.OpenRead(path);
        Need(inventory.GetArrayLength() == 5 && zip.Entries.Count == 5 &&
             zip.Entries.Select(x => x.FullName).Distinct(StringComparer.Ordinal).Count() == 5,
             "RS174 complete five-entry package inventory changed");
        foreach (var expected in inventory.EnumerateArray())
        {
            var entry = zip.GetEntry(Text(expected, "path")) ??
                throw new InvalidDataException("original archive entry missing");
            using var stream = entry.Open();
            var digest = Convert.ToHexStringLower(SHA256.HashData(stream));
            Need(entry.Length == expected.GetProperty("bytes").GetInt64() &&
                 entry.CompressedLength == expected.GetProperty("compressedBytes").GetInt64() &&
                 digest == Text(expected, "sha256"), "packaged entry bytes differ from RS174");
            if (entry.FullName == "lib/net10.0/dw.quantities.standard.dll")
                Need(digest == DllSha, "compiled Standard DLL changed");
        }
    }

    private static void Refuses(Action action, string code)
    {
        try { action(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == code) { return; }
        throw new InvalidDataException("expected exact shared publisher refusal: " + code);
    }
    private static string Repo(string root, string path) =>
        Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
    private static string Text(JsonElement value, string key) =>
        value.GetProperty(key).GetString() ?? throw new InvalidDataException("missing field " + key);
    private static string Sha(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }
    private static void Need(bool valid, string message)
    {
        if (!valid) throw new InvalidDataException(message);
    }
}
