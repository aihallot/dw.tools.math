using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Dw.Tools.Math.NuGetAnalysisFeedVerify;

internal static class Program
{
    private const string Id = "dw.tools.math.analysis";
    private const string Version = "20260.20261104.1001";
    private const string ArchiveSha = "d3e937efbe3fe9524a4146de655f867e965ac1322910cf45f2c060f0b915a3cf";
    private const string DllSha = "b2e5508f1e93c5f7968bed62e48c1760815431ce042c5172c3d261d5b7b123e7";
    private const int ArchiveBytes = 27831;
    private const string CandidateReceipt = "docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json";
    private const string PublisherReceipt = "docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json";
    private const string Fixture = "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 2 && args[0] == "--verify-published-analysis",
                "expected --verify-published-analysis <repository-root>");
            Verify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-ANALYSIS-RS149-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Verify(string root)
    {
        using var source = Load(PathFor(root, CandidateReceipt));
        using var rehearsal = Load(PathFor(root, PublisherReceipt));
        var candidate = source.RootElement.GetProperty("candidate");
        var trial = rehearsal.RootElement.GetProperty("rehearsal");
        Need(Text(source.RootElement, "status") ==
                 "rs147-independent-analysis-calendar-nuget-candidate-qualified-unpublished" &&
             !source.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(candidate, "packageId") == Id &&
             Text(candidate, "packageVersion") == Version &&
             Text(candidate, "sourceVersion") == "1.1.0-preview.6" &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == ArchiveBytes &&
             Text(candidate, "candidateArchiveSha256") == ArchiveSha &&
             Text(candidate, "candidateDllSha256") == DllSha &&
             candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
             candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 13 &&
             candidate.GetProperty("independentPackageReference").GetBoolean(),
             "original RS147 Analysis candidate evidence drifted");
        Need(Text(rehearsal.RootElement, "status") ==
                 "rs148-analysis-immutable-private-publisher-rehearsal-qualified-unpublished" &&
             !rehearsal.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(trial, "originalCandidateArchiveSha256") == ArchiveSha &&
             Text(trial, "originalCandidateDllSha256") == DllSha &&
             trial.GetProperty("disposableFeedPublished").GetBoolean() &&
             !trial.GetProperty("publishedToNugetLocal").GetBoolean(),
             "RS148 disposable-feed rehearsal cannot prove shared publication");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) &&
             Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
             "NUGET_LOCAL must be an existing absolute directory");
        var archive = Path.Combine(feed, Id + "." + Version + ".nupkg");
        var observedBefore = Inspect(archive);
        var work = PathFor(root, "build/artifacts/rs149-analysis-published-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "verification workspace collision");
        Directory.CreateDirectory(work);
        try
        {
            var snapshot = Path.Combine(work, "physical-feed-snapshot");
            var consumer = Path.Combine(work, "isolated-consumer");
            Directory.CreateDirectory(snapshot);
            Directory.CreateDirectory(consumer);
            var copy = Path.Combine(snapshot, Path.GetFileName(archive));
            File.Copy(archive, copy, overwrite: false);
            Need(Inspect(copy) == observedBefore, "independent physical snapshot differs");

            var fixture = PathFor(root, Fixture);
            Need(File.Exists(fixture), "original thirteen-case binary fixture absent");
            File.Copy(fixture, Path.Combine(consumer, "Program.cs"), overwrite: false);
            var project = Path.Combine(consumer, "rs149-analysis-consumer.csproj");
            var csproj = new XDocument(new XElement("Project",
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
                    new XElement("PackageReference", new XAttribute("Include", Id),
                        new XAttribute("Version", "[" + Version + "]")))));
            File.WriteAllText(project,
                csproj.ToString(SaveOptions.DisableFormatting) + "\n",
                new UTF8Encoding(false));
            var configPath = Path.Combine(consumer, "NuGet.Config");
            var config = new XDocument(new XElement("configuration",
                new XElement("packageSources", new XElement("clear"),
                    new XElement("add", new XAttribute("key", "physical-snapshot"),
                        new XAttribute("value", snapshot)))));
            File.WriteAllText(configPath,
                config.ToString(SaveOptions.DisableFormatting) + "\n",
                new UTF8Encoding(false));
            var cache = Path.Combine(work, "private-package-cache");
            var commands = new List<object>();
            commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
                "restore", project, "--configfile", configPath, "--packages", cache,
                "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "consumer-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            var tested = Run(root, cache, "consumer-binary", "dotnet", [
                "run", "--project", project, "-c", "Release",
                "--no-build", "--no-restore", "--", DllSha
            ]);
            commands.Add(tested.Receipt);
            Need(tested.Output.Contains("MATH-ANALYSIS-BINARY|PASS|13",
                     StringComparison.Ordinal) &&
                 tested.Output.Split("MATH-ANALYSIS-CHECK|").Length - 1 == 13,
                 "fresh source-free package consumer did not pass thirteen binary oracles");
            var observedAfter = Inspect(archive);
            Need(observedAfter == observedBefore &&
                 Inspect(copy) == observedBefore,
                 "published archive or isolated snapshot drifted during verification");
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 4, "four independent binary and SDK receipts required");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.analysis-private-feed-independent-readback",
                status = "analysis-published-local-binary-independently-verified",
                packageId = Id,
                packageVersion = Version,
                archiveBytes = observedBefore.Bytes,
                archiveSha256 = observedBefore.ArchiveSha,
                packagedDllSha256 = observedBefore.DllSha,
                loadedDllSha256 = DllSha,
                declaredDependencyCount = 0,
                publishedPackageSnapshotCount = 1,
                consumerSource = Fixture,
                independentPackageReference = true,
                originalMathProjectReferencesUsed = false,
                compiledConsumerChecks = 13,
                physicalArchiveInspected = true,
                publishedArchiveUnchanged = true,
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
            }, Pretty));
        }
        finally
        {
            if (Directory.Exists(work)) Directory.Delete(work, recursive: true);
        }
    }

    private sealed record Inspection(string ArchiveSha, string DllSha, long Bytes);

    private static Inspection Inspect(string path)
    {
        Need(File.Exists(path) &&
             (File.GetAttributes(path) &
                 (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
             new FileInfo(path).Length == ArchiveBytes,
             "original Analysis archive missing, redirected or unexpected length");
        var archiveSha = Hash(path);
        Need(archiveSha == ArchiveSha, "original published archive SHA-256 mismatch");
        using var zip = ZipFile.OpenRead(path);
        Need(zip.Entries.Count is >= 3 and <= 48 &&
             zip.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal).Count()
                 == zip.Entries.Count &&
             zip.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                 !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !e.FullName.Contains('\\')),
             "real package ZIP entry set not bounded or canonical");
        var nuspec = zip.GetEntry(Id + ".nuspec") ??
            throw new InvalidDataException("original package .nuspec missing");
        using (var input = nuspec.Open())
        {
            var manifest = XDocument.Load(input, LoadOptions.None);
            Need(manifest.Descendants().Any(x => x.Name.LocalName == "id" &&
                     x.Value == Id) &&
                 manifest.Descendants().Any(x => x.Name.LocalName == "version" &&
                     x.Value == Version) &&
                 !manifest.Descendants().Any(x => x.Name.LocalName == "dependency"),
                 "published package identity or zero-dependency closure drifted");
        }
        var dll = zip.GetEntry("lib/net10.0/" + Id + ".dll") ??
            throw new InvalidDataException("published net10.0 DLL absent");
        using var stream = dll.Open();
        var dllSha = Convert.ToHexStringLower(SHA256.HashData(stream));
        Need(dllSha == DllSha, "published Analysis DLL SHA-256 mismatch");
        return new Inspection(archiveSha, dllSha, new FileInfo(path).Length);
    }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string executable, string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executable, WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
        process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        process.StartInfo.Environment["NUGET_PACKAGES"] = cache;
        foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not execute " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("published Analysis consumer timed out in " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var details = output + "\n" + errors;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + details[..System.Math.Min(details.Length, 3800)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1100)]
        });
    }

    private static JsonDocument Load(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));
    private static string Text(JsonElement element, string name) =>
        element.GetProperty(name).GetString() ??
        throw new InvalidDataException("missing source identity: " + name);
    private static string PathFor(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
    private static string Hash(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    private static void Need(bool valid, string reason)
    {
        if (!valid) throw new InvalidDataException(reason);
    }
}
