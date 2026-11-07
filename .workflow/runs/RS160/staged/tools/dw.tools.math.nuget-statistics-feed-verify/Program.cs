using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Dw.Tools.Math.NuGetStatisticsFeedVerify;

internal static class Program
{
    private const string StatisticsId = "dw.tools.math.statistics";
    private const string StatisticsVersion = "20260.20261106.1001";
    private const string StatisticsArchiveSha = "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
    private const string StatisticsDllSha = "75749d668a6bc6216ac91bba55c9b66edb52ddc628d08c3b929227e752b79a96";
    private const string BaseId = "dw.quantities";
    private const string BaseVersion = "20260.20261102.1001";
    private const string BaseArchiveSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string Fixture = "tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs";
    private const string Candidate = "docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json";
    private const string Rehearsal = "docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json";
    private const string Correction = "docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json";
    private const string BaseReceipt = "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";

    private sealed record Package(string Id, string Version, int Bytes,
        string ArchiveSha, string DllSha, (string Id, string Version)[] Dependencies);
    private sealed record Inspection(string ArchiveSha, string DllSha, int Bytes);
    private static readonly Package Statistics = new(StatisticsId, StatisticsVersion, 17451,
        StatisticsArchiveSha, StatisticsDllSha, [(BaseId, "[" + BaseVersion + "]")]);
    private static readonly Package Base = new(BaseId, BaseVersion, 14747,
        BaseArchiveSha, BaseDllSha, []);
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    private static int Main(string[] arguments)
    {
        try
        {
            Need(arguments.Length == 2 && arguments[0] == "--verify-published-statistics",
                "expected --verify-published-statistics <repository-root>");
            Verify(Path.GetFullPath(arguments[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-STATISTICS-RS160-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Verify(string root)
    {
        using var source = JsonDocument.Parse(File.ReadAllText(PathFor(root, Candidate)));
        using var trial = JsonDocument.Parse(File.ReadAllText(PathFor(root, Rehearsal)));
        using var correction = JsonDocument.Parse(File.ReadAllText(PathFor(root, Correction)));
        using var baseEvidence = JsonDocument.Parse(File.ReadAllText(PathFor(root, BaseReceipt)));
        var original = source.RootElement.GetProperty("candidate");
        var rehearsal = trial.RootElement.GetProperty("rehearsal");
        Need(Text(source.RootElement, "status") ==
                 "rs157-statistics-dependent-calendar-candidate-qualified-unpublished" &&
             !source.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(original, "packageId") == StatisticsId &&
             Text(original, "packageVersion") == StatisticsVersion &&
             original.GetProperty("candidateArchiveBytes").GetInt32() == Statistics.Bytes &&
             Text(original, "candidateArchiveSha256") == StatisticsArchiveSha &&
             Text(original, "candidateDllSha256") == StatisticsDllSha &&
             original.GetProperty("packageDependencies").GetInt32() == 1 &&
             original.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 18 &&
             original.GetProperty("independentPackageReference").GetBoolean() &&
             Text(original, "dependencyQuantitiesVersion") == BaseVersion &&
             Text(original, "dependencyQuantitiesArchiveSha256") == BaseArchiveSha &&
             Text(original, "dependencyQuantitiesDllSha256") == BaseDllSha,
             "original qualified Statistics candidate or dependency evidence drifted");
        Need(Text(trial.RootElement, "status") ==
                 "rs158-statistics-immutable-local-publisher-rehearsal-qualified-unpublished" &&
             !trial.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(rehearsal, "rs157CandidateArchiveSha256") == StatisticsArchiveSha &&
             rehearsal.GetProperty("disposableFeedPublished").GetBoolean() &&
             !rehearsal.GetProperty("publishedToNugetLocal").GetBoolean(),
             "RS158 rehearsal must not be mistaken for real publication");
        Need(Text(baseEvidence.RootElement, "status") ==
                 "rs137-published-local-math-nuget-binary-independently-verified" &&
             baseEvidence.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean(), "exact Quantities prerequisite lacks DWF proof");

        var updated = correction.RootElement;
        Need(Text(updated, "status") ==
                 "rs159-statistics-private-publication-receipt-corrected-unpublished" &&
             updated.GetProperty("release_controller_compiled").GetBoolean() &&
             !updated.GetProperty("release_modes_invoked").GetBoolean() &&
             !updated.GetProperty("package_published_by_this_run").GetBoolean() &&
             Text(updated, "original_archive_sha256") == StatisticsArchiveSha &&
             Text(updated, "original_dll_sha256") == StatisticsDllSha &&
             Text(updated, "historical_controller_source_sha256") ==
                 Text(trial.RootElement.GetProperty("source_files"),
                     "tools/dw.tools.math.nuget-statistics-release/Program.cs"),
             "RS159 must prove exact F25 controller provenance without any publication");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
        var statisticsPath = Path.Combine(feed, FileName(Statistics));
        var basePath = Path.Combine(feed, FileName(Base));
        var firstStatistics = Inspect(statisticsPath, Statistics);
        var firstBase = Inspect(basePath, Base);
        var work = PathFor(root, "build/artifacts/rs160-statistics-published-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "verification workspace collision");
        Directory.CreateDirectory(work);
        try
        {
            var snapshot = Path.Combine(work, "physical-feed-snapshot");
            var consumer = Path.Combine(work, "isolated-consumer");
            Directory.CreateDirectory(snapshot);
            Directory.CreateDirectory(consumer);
            foreach (var package in new[] { Statistics, Base })
            {
                var physical = Path.Combine(feed, FileName(package));
                var copied = Path.Combine(snapshot, FileName(package));
                File.Copy(physical, copied, overwrite: false);
                _ = Inspect(copied, package);
            }
            var program = Path.Combine(consumer, "Program.cs");
            File.Copy(PathFor(root, Fixture), program, overwrite: false);
            var project = Path.Combine(consumer, "rs160-statistics-consumer.csproj");
            var projectXml = new XDocument(new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Exe"),
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("IsPackable", "false"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Include", "Program.cs")),
                    new XElement("PackageReference", new XAttribute("Include", StatisticsId),
                        new XAttribute("Version", "[" + StatisticsVersion + "]")))));
            File.WriteAllText(project, projectXml.ToString(SaveOptions.DisableFormatting) + "\n",
                new UTF8Encoding(false));
            var configPath = Path.Combine(consumer, "NuGet.Config");
            var config = new XDocument(new XElement("configuration",
                new XElement("packageSources", new XElement("clear"),
                    new XElement("add", new XAttribute("key", "physical-snapshot"),
                        new XAttribute("value", snapshot)))));
            File.WriteAllText(configPath, config.ToString(SaveOptions.DisableFormatting) + "\n",
                new UTF8Encoding(false));
            var cache = Path.Combine(work, "private-package-cache");
            var commands = new List<object>();
            commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
                "restore", project, "--configfile", configPath,
                "--packages", cache, "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "consumer-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            var executed = Run(root, cache, "consumer-binary", "dotnet", [
                "run", "--project", project, "-c", "Release",
                "--no-build", "--no-restore", "--", StatisticsDllSha
            ]);
            commands.Add(executed.Receipt);
            Need(executed.Output.Contains("MATH-STATISTICS-BINARY|PASS|18", StringComparison.Ordinal) &&
                 executed.Output.Split("MATH-STATISTICS-CHECK|").Length - 1 == 18 &&
                 executed.Output.Contains("MATH-STATISTICS-CHECK|loaded-package-sha256|PASS", StringComparison.Ordinal) &&
                 executed.Output.Contains("MATH-STATISTICS-CHECK|splitmix-noncanonical-refusal|PASS", StringComparison.Ordinal),
                 "source-free Statistics consumer must pass the original eighteen independent oracles");
            var lastStatistics = Inspect(statisticsPath, Statistics);
            var lastBase = Inspect(basePath, Base);
            Need(firstStatistics == lastStatistics && firstBase == lastBase,
                "source feed changed during isolated consumer validation");
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 4, "four independent command receipts required");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.statistics-private-feed-independent-readback",
                status = "statistics-published-local-two-binaries-independently-verified",
                packageId = StatisticsId,
                packageVersion = StatisticsVersion,
                archiveBytes = firstStatistics.Bytes,
                archiveSha256 = firstStatistics.ArchiveSha,
                packagedDllSha256 = firstStatistics.DllSha,
                loadedDllSha256 = StatisticsDllSha,
                exactDependencies = new[] { new
                {
                    id = BaseId, version = BaseVersion,
                    archiveBytes = firstBase.Bytes,
                    archiveSha256 = firstBase.ArchiveSha,
                    packagedDllSha256 = firstBase.DllSha,
                    loadedDllSha256 = BaseDllSha
                } },
                declaredDependencyCount = 1,
                publishedPackageSnapshotCount = 2,
                consumerSource = Fixture,
                independentPackageReference = true,
                originalMathProjectReferencesUsed = false,
                compiledConsumerChecks = 18,
                physicalArchivesInspected = true,
                publishedArchivesUnchanged = true,
                feedWideInventoryPerformed = false,
                publicationInvoked = false,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false,
                otherOsQualified = false,
                discoveryMigrationQualified = false,
                sourceFeed = "NUGET_LOCAL",
                sdkVersion = sdk.Output.Trim(),
                runtimeVersion = RuntimeInformation.FrameworkDescription,
                osDescription = RuntimeInformation.OSDescription,
                processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
                commands
            }, Indented));
        }
        finally
        {
            if (Directory.Exists(work)) Directory.Delete(work, recursive: true);
        }
    }

    private static Inspection Inspect(string path, Package package)
    {
        Need(File.Exists(path) &&
             (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
             new FileInfo(path).Length == package.Bytes,
             "physical immutable package missing, redirected or wrong length: " + package.Id);
        var archiveHash = Sha(path);
        Need(archiveHash == package.ArchiveSha,
             "physical archive SHA-256 mismatch: " + package.Id);
        using var zip = ZipFile.OpenRead(path);
        Need(zip.Entries.Count is >= 3 and <= 48 &&
             zip.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal).Count() ==
                 zip.Entries.Count &&
             zip.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                 !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !e.FullName.Contains('\\')),
             "invalid bounded immutable NuGet ZIP structure: " + package.Id);
        var nuspec = zip.GetEntry(package.Id + ".nuspec") ??
            throw new InvalidDataException("package nuspec missing: " + package.Id);
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(e => e.Name.LocalName == "id" &&
                     e.Value == package.Id) &&
                 xml.Descendants().Any(e => e.Name.LocalName == "version" &&
                     e.Value == package.Version),
                 "package identity drifted: " + package.Id);
            var actual = xml.Descendants().Where(e => e.Name.LocalName == "dependency")
                .Select(e => (Id: (string?)e.Attribute("id") ?? "",
                    Version: (string?)e.Attribute("version") ?? "")).ToArray();
            Need(actual.SequenceEqual(package.Dependencies),
                 "exact dependency manifest mismatch: " + package.Id);
        }
        var dll = zip.GetEntry("lib/net10.0/" + package.Id + ".dll") ??
            throw new InvalidDataException("net10.0 package DLL missing: " + package.Id);
        using var input = dll.Open();
        var dllHash = Convert.ToHexStringLower(SHA256.HashData(input));
        Need(dllHash == package.DllSha, "compiled DLL SHA-256 mismatch: " + package.Id);
        return new Inspection(archiveHash, dllHash, package.Bytes);
    }

    private static string FileName(Package package) =>
        package.Id + "." + package.Version + ".nupkg";

    private static string PathFor(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static string Text(JsonElement node, string field) =>
        node.GetProperty(field).GetString() ??
        throw new InvalidDataException("missing exact evidence field " + field);

    private static void Need(bool okay, string description)
    {
        if (!okay) throw new InvalidDataException(description);
    }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string executable, string[] arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executable,
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
        process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        process.StartInfo.Environment["NUGET_PACKAGES"] = cache;
        foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
        var stopwatch = Stopwatch.StartNew();
        Need(process.Start(), "cannot start independent command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("independent consumer timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        stopwatch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + errors;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3800)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = stopwatch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1100)]
        });
    }
}
