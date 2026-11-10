using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetElectrochemicalPublication;

internal static class Program
{
    private const string OperatorRoot = @"D:\data.dev\github\ai@hallot.net\dw.tools.math";
    private const string QualifiedFeed = @"I:\nuget.local";
    private const string PackageId = "dw.quantities.standard";
    private const string Version = "20260.20261110.1001";
    private const string ArchiveSha = "766e756e402ced1303a3b86d3faef39cfb1f40d00e34121288cc97b372968e6d";
    private const long ArchiveBytes = 14269;
    private const string PublisherSha = "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string Leaf = PackageId + "." + Version + ".nupkg";
    private const string CandidateDirectory = "build/artifacts/rs174-electrochemical-candidate/ae18522f6d6d4ea4a827439af4a9d300/candidate";
    private const string Intent = "RS176\n947988c69bbacda5ddf8fd97c0eee65377385a7f\n" + QualifiedFeed + "\n" + Leaf + "\n" + ArchiveSha + "\n";

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || args[0] is not ("--inspect" or "--publish"))
                throw new ArgumentException("Expected --inspect or --publish.");
            Run(publish: args[0] == "--publish");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("RS176 publication refused: " + error.Message);
            return 1;
        }
    }

    private static void Run(bool publish)
    {
        var root = Directory.GetCurrentDirectory();
        var configured = Environment.GetEnvironmentVariable("NUGET_LOCAL");
        Require(OperatingSystem.IsWindows() && !string.IsNullOrEmpty(configured) &&
            Path.IsPathFullyQualified(configured) && SamePath(configured, QualifiedFeed) &&
            Directory.Exists(QualifiedFeed), "expected the RS174/RS175 private NUGET_LOCAL directory");
        if (publish)
            Require(SamePath(root, OperatorRoot), "publication requires the operator checkout");
        using var candidateRecord = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(root,
            "docs/distribution/electrochemical-nuget-candidate-qualified.json")));
        var candidate = candidateRecord.RootElement.GetProperty("candidate");
        var source = Path.Combine(OperatorRoot, CandidateDirectory, Leaf);
        var target = Path.Combine(QualifiedFeed, Leaf);
        Require(candidate.GetProperty("candidateArchivePath").GetString() == CandidateDirectory + "/" + Leaf &&
            candidate.GetProperty("candidateArchiveSha256").GetString() == ArchiveSha &&
            candidate.GetProperty("candidateArchiveBytes").GetInt64() == ArchiveBytes,
            "candidate differs from the qualified RS174 archive");
        VerifyFile(typeof(DotNetLocalPackageRepository).Assembly.Location, PublisherSha);
        // The shared constructor checks repository roots and their ancestors. PreparePublication
        // inspects the candidate and the flat directory snapshot without opening unrelated archives.
        _ = new DotNetLocalPackageRepository(Path.GetDirectoryName(source)!);
        var repository = new DotNetLocalPackageRepository(QualifiedFeed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        VerifyFile(source, ArchiveSha, ArchiveBytes);
        VerifyPreserved(candidate);

        var intentPath = Path.Combine(OperatorRoot, "build", "artifacts", "rs176-electrochemical-publication.intent");
        bool hasIntent = HasMatchingIntent(intentPath);
        string observation;
        if (File.Exists(target))
        {
            Require(hasIntent, "the target already exists without an RS176 publication intent");
            VerifyFile(target, ArchiveSha, ArchiveBytes);
            observation = "existing-exact-after-intent";
        }
        else
        {
            var prepared = repository.PreparePublication(source, ArchiveSha,
                CancellationToken.None, ignoreSubdirectories: true);
            var plan = prepared.Receipt;
            Require(SamePath(plan.Path, target) && plan.PackageId == PackageId &&
                plan.Version == Version && plan.Sha256 == ArchiveSha && plan.Bytes == ArchiveBytes,
                "unexpected destination or prepared identity");
            observation = "ready";
            if (publish)
            {
                // Persist intent before the external write. Never adopt a pre-existing identity
                // on first entry or infer who wrote the file during an interrupted invocation.
                if (!hasIntent) RecordIntent(intentPath);
                var result = repository.Publish(prepared, CancellationToken.None);
                Require(SamePath(result.Path, target) && result.Sha256 == ArchiveSha &&
                    result.Bytes == ArchiveBytes && result.PackageId == PackageId &&
                    result.Version == Version, "shared publication returned an unexpected identity");
                VerifyFile(target, ArchiveSha, ArchiveBytes);
                observation = "created";
            }
        }
        if (publish)
        {
            VerifyPreserved(candidate);
            VerifyFile(source, ArchiveSha, ArchiveBytes);
        }
        Console.WriteLine($"RS176|mode={(publish ? "publication" : "inspection")}|observation={observation}" +
            $"|feed={QualifiedFeed}|package={PackageId}|version={Version}|bytes={ArchiveBytes}|sha256={ArchiveSha}" +
            "|preserved=3|independent-readback=pending");
    }

    private static void VerifyPreserved(JsonElement candidate)
    {
        VerifyFile(Path.Combine(QualifiedFeed, "dw.quantities.20260.20261102.1001.nupkg"),
            candidate.GetProperty("dependencyQuantitiesArchiveSha256").GetString()!, 14747);
        VerifyFile(Path.Combine(QualifiedFeed, "dw.quantities.expression.20260.20261102.1001.nupkg"),
            candidate.GetProperty("dependencyExpressionArchiveSha256").GetString()!, 13951);
        VerifyFile(Path.Combine(QualifiedFeed, "dw.quantities.standard.20260.20261103.1001.nupkg"),
            candidate.GetProperty("originalStandardArchiveSha256").GetString()!, 13604);
    }

    private static void VerifyFile(string path, string sha, long? bytes = null)
    {
        Require((File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0,
            "expected a regular file: " + path);
        using var input = File.OpenRead(path);
        Require((bytes is null || input.Length == bytes) && Convert.ToHexStringLower(SHA256.HashData(input)) == sha,
            "file size or SHA-256 differs: " + path);
    }

    private static bool HasMatchingIntent(string path)
    {
        if (!File.Exists(path)) return false;
        var expected = Encoding.UTF8.GetBytes(Intent);
        VerifyFile(path, Convert.ToHexStringLower(SHA256.HashData(expected)), expected.Length);
        return true;
    }

    private static void RecordIntent(string path)
    {
        // The existing archive is below build/artifacts, whose ancestors were checked above.
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        var ownsTemporary = false;
        try
        {
            using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                ownsTemporary = true;
                file.Write(Encoding.UTF8.GetBytes(Intent));
                file.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite: false);
            ownsTemporary = false;
        }
        finally
        {
            if (ownsTemporary) File.Delete(temporary);
        }
    }

    private static bool SamePath(string left, string right) =>
        string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(left)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(right)), StringComparison.OrdinalIgnoreCase);

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidDataException(message);
    }
}
