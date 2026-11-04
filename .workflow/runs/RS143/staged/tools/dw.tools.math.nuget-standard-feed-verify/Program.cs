using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetStandardFeedVerify;

internal static class Program
{
    private const string StandardId = "dw.quantities.standard";
    private const string StandardVersion = "20260.20261103.1001";
    private const string StandardArchiveSha =
        "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75";
    private const string StandardDllSha =
        "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d";
    private const string ExpressionId = "dw.quantities.expression";
    private const string ExpressionVersion = "20260.20261102.1001";
    private const string ExpressionArchiveSha =
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
    private const string ExpressionDllSha =
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
    private const string BaseId = "dw.quantities";
    private const string BaseVersion = "20260.20261102.1001";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string SharedNuGetDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";

    private const string CandidateReceipt =
        "docs/distribution/nuget-federation-f8-standard-candidate-qualified.json";
    private const string RehearsalReceipt =
        "docs/distribution/nuget-federation-f9-standard-publisher-qualified.json";
    private const string BaseReceipt =
        "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
    private const string ExpressionReceipt =
        "docs/distribution/nuget-federation-f7-expression-published-qualified.json";
    private const string ConsumerFixture =
        "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };
    private static readonly (string Id, string Version, string ArchiveSha, string DllSha, int Bytes)[] Packages =
    [
        (StandardId, StandardVersion, StandardArchiveSha, StandardDllSha, 13604),
        (ExpressionId, ExpressionVersion, ExpressionArchiveSha, ExpressionDllSha, 13951),
        (BaseId, BaseVersion, BaseArchiveSha, BaseDllSha, 14747)
    ];

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 || args[0] != "--verify-published-standard")
            {
                Console.Error.WriteLine("Expected --verify-published-standard <repository-root>.");
                return 2;
            }
            Verify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-STANDARD-RS143-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Verify(string root)
    {
        using var source = ReadJson(RepositoryFile(root, CandidateReceipt));
        using var rehearsal = ReadJson(RepositoryFile(root, RehearsalReceipt));
        using var baseEvidence = ReadJson(RepositoryFile(root, BaseReceipt));
        using var expressionEvidence = ReadJson(RepositoryFile(root, ExpressionReceipt));
        var candidate = source.RootElement.GetProperty("candidate");
        var trial = rehearsal.RootElement.GetProperty("rehearsal");
        Need(Text(source.RootElement, "status") ==
                 "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished" &&
             !source.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(candidate, "packageId") == StandardId &&
             Text(candidate, "packageVersion") == StandardVersion &&
             Text(candidate, "candidateArchiveSha256") == StandardArchiveSha &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13604 &&
             Text(candidate, "candidateDllSha256") == StandardDllSha &&
             candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
             candidate.GetProperty("consumerChecks").GetInt32() == 10 &&
             candidate.GetProperty("independentlyCompiledBinaryConsumer").GetBoolean() &&
             Text(candidate, "dependencyQuantitiesVersion") == BaseVersion &&
             Text(candidate, "dependencyQuantitiesArchiveSha256") == BaseArchiveSha &&
             Text(candidate, "dependencyQuantitiesDllSha256") == BaseDllSha &&
             Text(candidate, "dependencyExpressionVersion") == ExpressionVersion &&
             Text(candidate, "dependencyExpressionArchiveSha256") == ExpressionArchiveSha &&
             Text(candidate, "dependencyExpressionDllSha256") == ExpressionDllSha,
             "the exact RS141 dependent NuGet package and consumer are not retained");
        Need(Text(rehearsal.RootElement, "status") ==
                 "rs142-standard-immutable-local-publisher-rehearsal-qualified-unpublished" &&
             !rehearsal.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(trial, "rs141ArchiveSha256") == StandardArchiveSha &&
             trial.GetProperty("disposableFeedPublished").GetBoolean() &&
             !trial.GetProperty("publishedToNugetLocal").GetBoolean(),
             "the RS142 publisher rehearsal cannot claim a shared feed publication");
        Need(Text(baseEvidence.RootElement, "status") ==
                 "rs137-published-local-math-nuget-binary-independently-verified" &&
             baseEvidence.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             Text(expressionEvidence.RootElement, "status") ==
                 "rs140-expression-published-local-binary-independently-verified" &&
             expressionEvidence.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             expressionEvidence.RootElement.GetProperty("dependency_package_verified")
                 .GetBoolean(),
             "both immutable prerequisite archives require independent DWF acceptance");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
        var bounds = new DotNetLocalRepositoryLimits(
            MaximumItems: 16, MaximumPackageBytes: 67_108_864,
            MaximumTotalBytes: 67_108_864);
        // Validate the feed directory, but never enumerate unrelated producer archives.
        _ = new DotNetLocalPackageRepository(feed, bounds);
        foreach (var item in Packages)
        {
            var published = Path.Combine(feed, item.Id + "." + item.Version + ".nupkg");
            CheckPhysical(published, item.Bytes, item.ArchiveSha);
            Need(PackageDllSha(published, item.Id, item.Version,
                Dependencies(item.Id)) == item.DllSha,
                "published dependency graph or DLL differs: " + item.Id);
        }
        var shared = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(shared.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             FileSha(shared.Location) == SharedNuGetDllSha,
             "the loaded shared NuGet reader has an unexpected assembly SHA-256");

        var work = Path.Combine(root, "build", "artifacts",
            "rs143-standard-published-" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "a private verification workspace already exists");
        Directory.CreateDirectory(work);
        try
        {
            var snapshot = Path.Combine(work, "qualified-package-snapshot");
            var consumer = Path.Combine(work, "source-free-consumer");
            Directory.CreateDirectory(snapshot);
            Directory.CreateDirectory(consumer);
            foreach (var item in Packages)
            {
                var original = Path.Combine(feed, item.Id + "." + item.Version + ".nupkg");
                var copy = Path.Combine(snapshot, Path.GetFileName(original));
                File.Copy(original, copy, overwrite: false);
                CheckPhysical(copy, item.Bytes, item.ArchiveSha);
            }
            var snapshotReader = new DotNetLocalPackageRepository(snapshot, bounds);
            var readback = snapshotReader.List();
            Need(readback.InspectedPackages == 3 && readback.MatchedPackages == 3 &&
                 readback.Packages.Count == 3 &&
                 Packages.All(expected => readback.Packages.Any(actual =>
                     actual.PackageId == expected.Id &&
                     actual.Version == expected.Version &&
                     actual.Sha256 == expected.ArchiveSha &&
                     actual.Bytes == expected.Bytes)),
                 "the published NuGet repository reader did not accept the exact three-package snapshot");

            var fixture = RepositoryFile(root, ConsumerFixture);
            Need(File.Exists(fixture), "the original RS141 ten-case source-free fixture is absent");
            File.Copy(fixture, Path.Combine(consumer, "Program.cs"), overwrite: false);
            var project = Path.Combine(consumer, "rs143-standard-consumer.csproj");
            File.WriteAllText(project, """
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
                    <PackageReference Include="dw.quantities.standard" Version="[20260.20261103.1001]" />
                  </ItemGroup>
                </Project>
                """, new UTF8Encoding(false));
            var config = Path.Combine(consumer, "NuGet.Config");
            File.WriteAllText(config,
                "<configuration><packageSources><clear /></packageSources></configuration>\n",
                new UTF8Encoding(false));
            var cache = Path.Combine(work, "private-package-cache");
            var commands = new List<object>();
            commands.Add(Run(root, work, "consumer-restore", "dotnet", [
                "restore", project, "--configfile", config,
                "--source", feed, "--packages", cache, "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, work, "consumer-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
            ]).Receipt);
            var executed = Run(root, work, "consumer-binary", "dotnet", [
                "run", "--project", project, "-c", "Release", "--no-build",
                "--no-restore", "--", StandardDllSha, ExpressionDllSha, BaseDllSha
            ]);
            commands.Add(executed.Receipt);
            Need(executed.Output.Contains("MATH-STANDARD-BINARY|PASS|10",
                     StringComparison.Ordinal) &&
                 executed.Output.Contains("MATH-STANDARD-CHECK|loaded-dll-hashes|PASS",
                     StringComparison.Ordinal) &&
                 executed.Output.Contains("MATH-STANDARD-CHECK|250g-equals-quarter-kg|PASS",
                     StringComparison.Ordinal),
                 "fresh exact NuGet consumer failed ten source-free Standard tests");

            foreach (var item in Packages)
            {
                var published = Path.Combine(feed, item.Id + "." + item.Version + ".nupkg");
                CheckPhysical(published, item.Bytes, item.ArchiveSha);
                Need(PackageDllSha(published, item.Id, item.Version,
                    Dependencies(item.Id)) == item.DllSha,
                    "published package changed during binary observation " + item.Id);
            }
            Need(FileSha(shared.Location) == SharedNuGetDllSha,
                 "loaded NuGet provider changed during published consumer verification");
            var sdk = Run(root, work, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 4, "four real independent consumer and SDK observations required");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.standard-private-feed-readback",
                status = "standard-published-local-three-binaries-independently-verified",
                packageId = StandardId,
                packageVersion = StandardVersion,
                archiveSha256 = StandardArchiveSha,
                archiveBytes = 13604,
                packagedDllSha256 = StandardDllSha,
                loadedDllSha256 = StandardDllSha,
                exactDependencies = new[]
                {
                    new { id = BaseId, version = BaseVersion, archiveSha256 = BaseArchiveSha,
                          archiveBytes = 14747, packagedDllSha256 = BaseDllSha,
                          loadedDllSha256 = BaseDllSha },
                    new { id = ExpressionId, version = ExpressionVersion,
                          archiveSha256 = ExpressionArchiveSha, archiveBytes = 13951,
                          packagedDllSha256 = ExpressionDllSha,
                          loadedDllSha256 = ExpressionDllSha }
                },
                declaredDependencyCount = 2,
                publishedPackageSnapshotCount = readback.InspectedPackages,
                sharedNugetPackageVersion = "20260.20261026.1010",
                sharedNugetDllSha256 = SharedNuGetDllSha,
                sourceFeed = "NUGET_LOCAL",
                consumerSource = ConsumerFixture,
                independentPackageReference = true,
                originalMathProjectReferencesUsed = false,
                compiledConsumerChecks = 10,
                physicalArchivesInspected = true,
                publishedArchivesUnchanged = true,
                feedWideInventoryPerformed = false,
                publicationInvoked = false,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false,
                otherOsQualified = false,
                discoveryMigrationQualified = false,
                sdkVersion = sdk.Output.Trim(),
                commands
            }, Pretty));
        }
        finally
        {
            if (Directory.Exists(work))
                Directory.Delete(work, recursive: true);
        }
    }

    private static IReadOnlyList<(string Id, string Version)> Dependencies(string id) =>
        id switch
        {
            StandardId =>
            [
                (BaseId, "[" + BaseVersion + "]"),
                (ExpressionId, "[" + ExpressionVersion + "]")
            ],
            ExpressionId => [(BaseId, "[" + BaseVersion + "]")],
            BaseId => [],
            _ => throw new InvalidDataException("unrecognized Math package in bounded readback")
        };

    private static void CheckPhysical(string path, int size, string sha)
    {
        Need(File.Exists(path) &&
             (File.GetAttributes(path) &
                 (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
             new FileInfo(path).Length == size &&
             FileSha(path) == sha,
             "private feed archive is missing, redirected or has changed bytes");
    }

    private static string PackageDllSha(
        string path, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var zip = ZipFile.OpenRead(path);
        Need(zip.Entries.Count is >= 3 and <= 48 &&
             zip.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == zip.Entries.Count &&
             zip.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "package ZIP entry set is not bounded");
        var nuspec = zip.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("required NuGet package nuspec missing");
        using (var source = nuspec.Open())
        {
            var metadata = XDocument.Load(source, LoadOptions.None);
            Need(metadata.Descendants().Any(x =>
                     x.Name.LocalName == "id" && x.Value == id) &&
                 metadata.Descendants().Any(x =>
                     x.Name.LocalName == "version" && x.Value == version),
                 "package ID/version differs from immutable qualification");
            var actual = metadata.Descendants()
                .Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? ""))
                .OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            var expected = dependencies.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            Need(actual.SequenceEqual(expected),
                 "package dependency closure differs from exact published identities");
        }
        var dll = zip.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("net10.0 NuGet DLL is missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static JsonDocument ReadJson(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Text(JsonElement node, string field) =>
        node.GetProperty(field).GetString() ??
        throw new InvalidDataException("missing owner-sourced field: " + field);

    private static string RepositoryFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string FileSha(string file) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));

    private static void Need(bool good, string why)
    {
        if (!good) throw new InvalidDataException(why);
    }

    private static (string Output, object Receipt) Run(
        string root, string workspace, string id, string program, string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = program,
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
        foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not launch exact package check " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("published Standard check timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var error = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + error;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3700)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = program + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1200)]
        });
    }
}
