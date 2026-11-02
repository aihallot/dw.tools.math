using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetFeedVerify;

internal static class Program
{
    private const string PackageId = "dw.quantities";
    private const string PackageVersion = "20260.20261102.1001";
    private const string ArchiveSha256 =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string DllSha256 =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string NuGetCoreSha256 =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string OwnerCandidate =
        "docs/distribution/nuget-federation-f2-quantities-qualified.json";
    private const string OwnerRehearsal =
        "docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json";
    private const string ConsumerFixture =
        "tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs";
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    private static int Main(string[] arguments)
    {
        try
        {
            if (arguments.Length != 2 || arguments[0] != "--verify-published")
            {
                Console.Error.WriteLine("Expected --verify-published <repository-root>.");
                return 2;
            }
            Verify(Path.GetFullPath(arguments[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUGET-RS137-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Verify(string root)
    {
        using var previous = JsonDocument.Parse(File.ReadAllText(
            RepositoryFile(root, OwnerCandidate)));
        using var ownerRehearsal = JsonDocument.Parse(File.ReadAllText(
            RepositoryFile(root, OwnerRehearsal)));
        var candidate = previous.RootElement.GetProperty("candidate");
        Need(candidate.GetProperty("sourcePackageId").GetString() == PackageId &&
             candidate.GetProperty("packageVersion").GetString() == PackageVersion &&
             candidate.GetProperty("candidateArchiveSha256").GetString() == ArchiveSha256 &&
             candidate.GetProperty("candidateDllSha256").GetString() == DllSha256 &&
             candidate.GetProperty("candidateArchiveBytes").GetInt64() == 14747 &&
             candidate.GetProperty("compiledSourceFreeConsumerPassed").GetBoolean() &&
             candidate.GetProperty("consumerChecks").GetInt32() == 4 &&
             !candidate.GetProperty("published").GetBoolean(),
             "RS135 immutable archive and consumer provenance changed");
        Need(ownerRehearsal.RootElement.GetProperty("status").GetString() ==
             "rs136-math-immutable-local-nuget-publisher-rehearsal-qualified-unpublished" &&
             !ownerRehearsal.RootElement.GetProperty("package_published").GetBoolean(),
             "RS136 may only have approved a disposable-feed rehearsal");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must identify the existing shared feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
        var bounds = new DotNetLocalRepositoryLimits(
            MaximumItems: 64, MaximumPackageBytes: 67_108_864,
            MaximumTotalBytes: 67_108_864);
        // Read-only directory validation uses the existing federator's path policy.
        _ = new DotNetLocalPackageRepository(feed, bounds);
        var archivePath = Path.Combine(feed, PackageId + "." + PackageVersion + ".nupkg");
        Need(File.Exists(archivePath), "the exact qualified package is absent from NUGET_LOCAL");
        Need((File.GetAttributes(archivePath) & FileAttributes.ReparsePoint) == 0,
             "the published package cannot be a redirected file");
        Need(new FileInfo(archivePath).Length == 14747 &&
             ShaFile(archivePath) == ArchiveSha256,
             "the published archive bytes do not equal the RS135 candidate");

        var library = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(library.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             ShaFile(library.Location) == NuGetCoreSha256,
             "the loaded shared NuGet reader does not match the approved binary");
        Need(PackagedDllSha(archivePath) == DllSha256,
             "the physical published package contains an unexpected Math DLL");

        var workspace = Path.Combine(root, "build", "artifacts",
            "rs137-published-quantities-" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(workspace), "private verification workspace already exists");
        Directory.CreateDirectory(workspace);
        try
        {
            var snapshot = Path.Combine(workspace, "package-snapshot");
            var consumerRoot = Path.Combine(workspace, "binary-consumer");
            Directory.CreateDirectory(snapshot);
            Directory.CreateDirectory(consumerRoot);
            var copiedArchive = Path.Combine(snapshot, Path.GetFileName(archivePath));
            File.Copy(archivePath, copiedArchive, overwrite: false);
            Need(ShaFile(copiedArchive) == ArchiveSha256,
                 "exact shared-feed archive differs from its isolated snapshot");
            // Let the existing published NuGet reader, not a new package parser,
            // independently validate the copied archive's nuspec and its identity.
            var repository = new DotNetLocalPackageRepository(snapshot, bounds);
            var inventory = repository.List(PackageId);
            Need(inventory.InspectedPackages == 1 && inventory.MatchedPackages == 1,
                 "one and only one independent Math package must be inspected");
            var observed = inventory.Packages.Single();
            Need(observed.PackageId == PackageId &&
                 observed.Version == PackageVersion &&
                 observed.Sha256 == ArchiveSha256 &&
                 observed.Bytes == 14747,
                 "the shared NuGet package reader rejected the archived identity");

            var fixture = RepositoryFile(root, ConsumerFixture);
            Need(File.Exists(fixture), "the original RS135 package-only oracle is missing");
            File.Copy(fixture, Path.Combine(consumerRoot, "Program.cs"), overwrite: false);
            var consumerProject = Path.Combine(consumerRoot,
                "rs137-shared-feed-consumer.csproj");
            File.WriteAllText(consumerProject, """
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
                    <PackageReference Include="dw.quantities" Version="[20260.20261102.1001]" />
                  </ItemGroup>
                </Project>
                """, new UTF8Encoding(false));
            var config = Path.Combine(consumerRoot, "NuGet.Config");
            File.WriteAllText(config,
                "<configuration><packageSources><clear /></packageSources></configuration>\n",
                new UTF8Encoding(false));
            var cache = Path.Combine(workspace, "private-package-cache");
            var commands = new List<object>();
            commands.Add(Run(root, workspace, "consumer-restore", "dotnet", [
                "restore", consumerProject, "--configfile", config,
                "--source", feed, "--packages", cache, "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, workspace, "consumer-build", "dotnet", [
                "build", consumerProject, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            var executed = Run(root, workspace, "consumer-execute", "dotnet", [
                "run", "--project", consumerProject, "-c", "Release",
                "--no-build", "--no-restore", "--", DllSha256
            ]);
            commands.Add(executed.Receipt);
            Need(executed.Output.Contains("MATH-QUANTITIES-BINARY|PASS|4",
                StringComparison.Ordinal) &&
                 executed.Output.Contains("MATH-QUANTITIES-CHECK|loaded-dll|PASS",
                StringComparison.Ordinal),
                 "the fresh published-PackageReference consumer did not load the expected DLL");
            Need(new FileInfo(archivePath).Length == 14747 &&
                 ShaFile(archivePath) == ArchiveSha256 &&
                 PackagedDllSha(archivePath) == DllSha256 &&
                 ShaFile(library.Location) == NuGetCoreSha256,
                 "shared feed package or published NuGet reader changed during verification");

            var sdk = Run(root, workspace, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 4, "the independent published consumer commands were incomplete");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.nuget-local-feed-independent-observation",
                status = "published-local-binary-verified-read-only",
                packageId = PackageId,
                packageVersion = PackageVersion,
                archiveSha256 = ArchiveSha256,
                archiveBytes = 14747,
                packagedDllSha256 = DllSha256,
                loadedDllSha256 = DllSha256,
                sharedNugetPackageVersion = "20260.20261026.1010",
                sharedNugetDllSha256 = NuGetCoreSha256,
                sourceFeed = "NUGET_LOCAL",
                originalFixture = ConsumerFixture,
                privateSourceOnly = true,
                independentPackageReference = true,
                compiledConsumerChecks = 4,
                physicalArchiveInspected = true,
                publishedArchiveUnchanged = true,
                feedWideInventoryPerformed = false,
                publicationInvoked = false,
                remoteRegistryPublished = false,
                physicalInstalledPackageRollbackQualified = false,
                otherOsQualified = false,
                discoveryMigrationQualified = false,
                sdkVersion = sdk.Output.Trim(),
                commands
            }, Indented));
        }
        finally
        {
            if (Directory.Exists(workspace))
                Directory.Delete(workspace, recursive: true);
        }
    }

    private static string PackagedDllSha(string archive)
    {
        using var zip = ZipFile.OpenRead(archive);
        Need(zip.Entries.Count <= 32 &&
             zip.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal)
                .Count() == zip.Entries.Count &&
             zip.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !e.FullName.Contains('\\')),
             "unsafe or divergent NuGet ZIP entry paths");
        var nuspec = zip.GetEntry("dw.quantities.nuspec") ??
            throw new InvalidDataException("the canonical dw.quantities nuspec is missing");
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(e => e.Name.LocalName == "id" &&
                 e.Value == PackageId) &&
                 xml.Descendants().Any(e => e.Name.LocalName == "version" &&
                 e.Value == PackageVersion) &&
                 !xml.Descendants().Any(e => e.Name.LocalName == "dependency"),
                 "published package has a different package or dependency graph");
        }
        var dll = zip.GetEntry("lib/net10.0/dw.quantities.dll") ??
            throw new InvalidDataException("the published net10.0 Math binary is missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static string ShaFile(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static string RepositoryFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static void Need(bool okay, string reason)
    {
        if (!okay) throw new InvalidDataException(reason);
    }

    private static (string Output, object Receipt) Run(
        string root, string workspace, string id, string executable, string[] arguments)
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
            Path.Combine(workspace, "private-package-cache");
        foreach (var arg in arguments) process.StartInfo.ArgumentList.Add(arg);
        var clock = Stopwatch.StartNew();
        Need(process.Start(), "could not launch bounded verification " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("published feed check timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var error = stderr.GetAwaiter().GetResult();
        clock.Stop();
        if (process.ExitCode != 0)
        {
            var combined = output + "\n" + error;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + combined[..System.Math.Min(combined.Length, 3400)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = clock.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }
}
