using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetCandidate;

internal static class Program
{
    private const string QuantitiesId = "dw.quantities";
    private const string HistoricalVersion = "0.2.0-preview.1";
    private const string SourceProject = "src/projects/dw.quantities/dw.quantities.csproj";
    private const string F1Project =
        "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
    private const string Fixture =
        "tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs";
    private const string NuGetCoreSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";

    private static readonly JsonSerializerOptions Indented = new()
    {
        WriteIndented = true
    };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 2 && args[0] == "--qualify-quantities")
            {
                QualifyQuantities(args[1]);
                return 0;
            }
            Console.Error.WriteLine("Expected --qualify-quantities <repository-root>.");
            return 2;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUGET-F2-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void QualifyQuantities(string repositoryRoot)
    {
        var root = Path.GetFullPath(repositoryRoot);
        var source = Path.Combine(root, SourceProject.Replace('/', Path.DirectorySeparatorChar));
        var fixture = Path.Combine(root, Fixture.Replace('/', Path.DirectorySeparatorChar));
        Need(File.Exists(source) && File.Exists(fixture),
            "qualified Math source project or complete package-only consumer fixture missing");

        var xml = XDocument.Load(source, LoadOptions.None);
        string? Property(string name) => xml.Descendants()
            .Where(x => x.Name.LocalName == name).Select(x => x.Value.Trim())
            .FirstOrDefault();
        Need(Property("PackageId") == QuantitiesId &&
             Property("Version") == HistoricalVersion &&
             Property("IsPackable") == "true" &&
             !xml.Descendants().Any(e => e.Name.LocalName is
                 "ProjectReference" or "PackageReference"),
             "dw.quantities is not the same standalone source package qualified by F1");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must be provided");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute local feed");

        // Reuse the RS134 consumer's complete owned-inventory/version proposal
        // instead of copying its feed parser or implementing a second allocator.
        var commandReceipts = new List<object>();
        var initialF1 = RunChecked(root, "f1-inventory-before", "dotnet", [
            "run", "--project", F1Project, "-c", "Release", "--no-build", "--no-restore",
            "--", "--inspect", "."
        ]);
        commandReceipts.Add(initialF1.Receipt);
        using var initialJson = JsonDocument.Parse(initialF1.Output.Trim());
        var initial = initialJson.RootElement;
        Need(Text(initial, "status") ==
                 "f1-source-and-feed-inventory-qualified-not-published" &&
             Text(initial, "repositoryTool") == "Dw.Tools.NuGet" &&
             Text(initial, "repositoryPackageVersion") == "20260.20261026.1010" &&
             initial.GetProperty("packageCount").GetInt32() == 13 &&
             !initial.GetProperty("feedPublished").GetBoolean() &&
             !initial.GetProperty("versionsReserved").GetBoolean(),
             "the exact RS134 F1 inventory and shared version contract did not pass");
        var candidates = initial.GetProperty("packages").EnumerateArray().ToArray();
        var quantities = candidates.Single(item => Text(item, "packageId") == QuantitiesId);
        var proposedVersion = Text(quantities, "candidateVersionPreview");
        Need(Text(quantities, "historicalSourceVersion") == HistoricalVersion &&
             Text(quantities, "effectivePackageVersion") == HistoricalVersion &&
             Text(quantities, "effectivePackable") == "true" &&
             Text(quantities, "effectiveTargetFramework") == "net10.0" &&
             quantities.GetProperty("projectReferences").GetArrayLength() == 0 &&
             quantities.GetProperty("externalPackageReferences").GetArrayLength() == 0 &&
             Regex.IsMatch(proposedVersion, @"^20260\.2026[0-9]{4}\.1[0-9]{3}$",
                RegexOptions.CultureInvariant),
             "proposed candidate version or source dependency surface invalid");
        Need(DateOnly.TryParseExact(Text(initial, "observedDate"), "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var observedDate) &&
             observedDate == DateOnly.FromDateTime(DateTime.Now) &&
             proposedVersion.Split('.')[1] ==
                observedDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
             "candidate preview and observed local date differ");

        var core = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(core.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             ShaFile(core.Location) == NuGetCoreSha,
             "the actually loaded shared NuGet core has a different binary identity");

        var scratch = Path.Combine(root, "build", "artifacts", "rs135-quantities-candidate");
        var candidateFeed = Path.Combine(scratch, "candidate");
        var consumerDirectory = Path.Combine(scratch, "consumer");
        var archivePath = Path.Combine(candidateFeed,
            QuantitiesId + "." + proposedVersion + ".nupkg");
        Need(!File.Exists(archivePath) && !Directory.Exists(consumerDirectory),
            "run-owned candidate/consumer output already exists; no overwrite permitted");
        Directory.CreateDirectory(candidateFeed);
        Directory.CreateDirectory(consumerDirectory);

        commandReceipts.Add(RunChecked(root, "source-locked-restore", "dotnet", [
            "restore", SourceProject, "--locked-mode", "--source", feed,
            "-p:NuGetAudit=false"
        ]).Receipt);
        commandReceipts.Add(RunChecked(root, "source-release-build", "dotnet", [
            "build", SourceProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        commandReceipts.Add(RunChecked(root, "candidate-pack", "dotnet", [
            "pack", SourceProject, "-c", "Release", "--no-build", "--no-restore",
            "-p:PackageVersion=" + proposedVersion, "-p:NuGetAudit=false",
            "--output", candidateFeed
        ]).Receipt);
        Need(File.Exists(archivePath), "requested candidate archive is absent");
        var candidateRepository = new DotNetLocalPackageRepository(candidateFeed,
            new DotNetLocalRepositoryLimits(MaximumItems: 16, MaximumPackageBytes: 67_108_864,
                MaximumTotalBytes: 67_108_864));
        var archiveObservation = candidateRepository.List(QuantitiesId);
        Need(archiveObservation.InspectedPackages == 1 &&
             archiveObservation.MatchedPackages == 1,
             "shared NuGet package reader did not accept exactly one candidate");
        var package = archiveObservation.Packages.Single();
        var archiveSha = ShaFile(archivePath);
        Need(package.PackageId == QuantitiesId &&
             package.Version == proposedVersion &&
             package.Sha256 == archiveSha,
             "archive PackageId, version or SHA-256 differs from shared NuGet inspection");
        var dllSha = InspectCandidateDll(archivePath, proposedVersion);

        // Complete C# source is checked in. Only the test-owned project XML
        // gets the exact on-the-day NuGet PackageReference version.
        File.Copy(fixture, Path.Combine(consumerDirectory, "Program.cs"), overwrite: false);
        var consumerProject = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <IsPackable>false</IsPackable>
                <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
                <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
              </PropertyGroup>
              <ItemGroup>
                <Compile Include="Program.cs" />
                <PackageReference Include="dw.quantities" Version="[__VERSION__]" />
              </ItemGroup>
            </Project>
            """.Replace("__VERSION__", proposedVersion, StringComparison.Ordinal);
        var consumerProjectPath = Path.Combine(consumerDirectory, "rs135-quantities-consumer.csproj");
        File.WriteAllText(consumerProjectPath, consumerProject + "\n");
        var config = Path.Combine(consumerDirectory, "NuGet.Config");
        File.WriteAllText(config,
            "<configuration><packageSources><clear /></packageSources></configuration>\n");
        var cache = Path.Combine(root, ".workflow", "local", "nuget-f2-rs135", "packages");
        commandReceipts.Add(RunChecked(root, "consumer-restore", "dotnet", [
            "restore", consumerProjectPath, "--configfile", config,
            "--source", candidateFeed, "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commandReceipts.Add(RunChecked(root, "consumer-release-build", "dotnet", [
            "build", consumerProjectPath, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var consumer = RunChecked(root, "consumer-binary-execution", "dotnet", [
            "run", "--project", consumerProjectPath, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        commandReceipts.Add(consumer.Receipt);
        Need(consumer.Output.Contains("MATH-QUANTITIES-BINARY|PASS|4",
             StringComparison.Ordinal), "the compiled package-only consumer did not pass four controls");

        // Re-run the original immutable F1 tool to prove that the scoped
        // shared feed and its calendar preview were not changed by the test.
        var finalF1 = RunChecked(root, "f1-inventory-after", "dotnet", [
            "run", "--project", F1Project, "-c", "Release", "--no-build", "--no-restore",
            "--", "--inspect", "."
        ]);
        commandReceipts.Add(finalF1.Receipt);
        using var finalJson = JsonDocument.Parse(finalF1.Output.Trim());
        var finalInventory = finalJson.RootElement;
        var finalQuantities = finalInventory.GetProperty("packages").EnumerateArray()
            .Single(item => Text(item, "packageId") == QuantitiesId);
        Need(Text(initial, "inventorySha256") == Text(finalInventory, "inventorySha256") &&
             Text(finalQuantities, "candidateVersionPreview") == proposedVersion &&
             initial.GetProperty("inspectedArchives").GetInt32() ==
                finalInventory.GetProperty("inspectedArchives").GetInt32(),
             "Math packages in NUGET_LOCAL drifted during candidate qualification");
        Need(ShaFile(core.Location) == NuGetCoreSha, "shared NuGet core DLL changed");
        var sdk = RunChecked(root, "sdk", "dotnet", ["--version"]);
        commandReceipts.Add(sdk.Receipt);
        Need(commandReceipts.Count == 9, "missing a required as-run candidate or consumer command");

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.nuget-quantities-candidate",
            status = "qualified-local-candidate-not-published",
            sourcePackageId = QuantitiesId,
            sourceVersion = HistoricalVersion,
            packageVersion = proposedVersion,
            versionReserved = false,
            sharedCore = "dw.tools.nuget/[20260.20261026.1010]",
            sharedCoreDllSha256 = NuGetCoreSha,
            candidateArchivePath = Path.GetRelativePath(root, archivePath).Replace('\\', '/'),
            candidateArchiveSha256 = archiveSha,
            candidateArchiveBytes = new FileInfo(archivePath).Length,
            candidateDllSha256 = dllSha,
            packageDependencies = 0,
            localFeedSelector = "NUGET_LOCAL",
            sourceFeedUnchanged = true,
            published = false,
            oldSourcePackageVersionRetained = true,
            compiledSourceFreeConsumerPassed = true,
            consumerChecks = 4,
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            sdkVersion = sdk.Output.Trim(),
            commands = commandReceipts
        }, Indented));
    }

    private static string InspectCandidateDll(string archivePath, string version)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        Need(archive.Entries.Count <= 32, "candidate archive has unexpected file count");
        var entry = archive.GetEntry("dw.quantities.nuspec")
            ?? throw new InvalidDataException("candidate package nuspec is absent");
        using (var stream = entry.Open())
        {
            var metadata = XDocument.Load(stream, LoadOptions.None);
            Need(metadata.Descendants().Any(e =>
                 e.Name.LocalName == "id" && e.Value == QuantitiesId) &&
                 metadata.Descendants().Any(e =>
                 e.Name.LocalName == "version" && e.Value == version) &&
                 !metadata.Descendants().Any(e => e.Name.LocalName == "dependency"),
                 "candidate nuspec version, id or package closure changed");
        }
        var dll = archive.GetEntry("lib/net10.0/dw.quantities.dll")
            ?? throw new InvalidDataException("candidate net10.0 DLL is missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static string Text(JsonElement item, string name) =>
        item.GetProperty(name).GetString() ??
        throw new InvalidDataException("Missing candidate identity " + name);

    private static string ShaFile(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool valid, string reason)
    {
        if (!valid) throw new InvalidDataException(reason);
    }

    private static (string Output, object Receipt) RunChecked(
        string root, string id, string executable, string[] args)
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
        process.StartInfo.Environment["NUGET_PACKAGES"] =
            Path.Combine(root, ".workflow", "local", "nuget-f2-rs135", "packages");
        foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not start candidate qualification command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("candidate command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var error = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var all = output + "\n" + error;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + all[..System.Math.Min(all.Length, 3500)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }
}
