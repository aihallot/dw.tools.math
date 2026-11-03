using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetExpressionFeedVerify;

internal static class Program
{
    private const string ExpressionId = "dw.quantities.expression";
    private const string BaseId = "dw.quantities";
    private const string Version = "20260.20261102.1001";
    private const string ExpressionArchiveSha =
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
    private const string ExpressionDllSha =
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string SharedNuGetDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string ExpressionCandidateReceipt =
        "docs/distribution/nuget-federation-f5-expression-candidate-qualified.json";
    private const string ExpressionRehearsalReceipt =
        "docs/distribution/nuget-federation-f6-expression-publisher-qualified.json";
    private const string PublishedBaseReceipt =
        "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
    private const string ConsumerFixture =
        "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs";

    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 || args[0] != "--verify-published-expression")
            {
                Console.Error.WriteLine(
                    "Expected --verify-published-expression <repository-root>.");
                return 2;
            }
            Verify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-EXPRESSION-RS140-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Verify(string root)
    {
        using var owner = ReadJson(RepoFile(root, ExpressionCandidateReceipt));
        using var rehearsal = ReadJson(RepoFile(root, ExpressionRehearsalReceipt));
        using var baseProof = ReadJson(RepoFile(root, PublishedBaseReceipt));
        var expression = owner.RootElement.GetProperty("candidate");
        Need(Read(owner.RootElement, "status") ==
                 "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished" &&
             !owner.RootElement.GetProperty("package_published").GetBoolean() &&
             Read(expression, "packageId") == ExpressionId &&
             Read(expression, "packageVersion") == Version &&
             Read(expression, "candidateArchiveSha256") == ExpressionArchiveSha &&
             Read(expression, "candidateDllSha256") == ExpressionDllSha &&
             expression.GetProperty("candidateArchiveBytes").GetInt64() == 13951 &&
             Read(expression, "exactBasePackageId") == BaseId &&
             Read(expression, "exactBasePackageVersion") == Version &&
             Read(expression, "exactBasePackageSha256") == BaseArchiveSha &&
             Read(expression, "exactBaseDllSha256") == BaseDllSha &&
             expression.GetProperty("packagedDependencies").GetInt32() == 1 &&
             expression.GetProperty("consumerChecks").GetInt32() == 6 &&
             expression.GetProperty("actualSourceFreeConsumerPassed").GetBoolean(),
             "RS138 original candidate and exact published dependency are not unchanged");
        Need(Read(rehearsal.RootElement, "status") ==
                 "rs139-expression-immutable-local-publisher-rehearsal-qualified-unpublished" &&
             !rehearsal.RootElement.GetProperty("package_published").GetBoolean() &&
             rehearsal.RootElement.GetProperty("rehearsal")
                 .GetProperty("disposableFeedPublished").GetBoolean(),
             "RS139 qualified only a disposable publisher rehearsal");
        Need(Read(baseProof.RootElement, "status") ==
                 "rs137-published-local-math-nuget-binary-independently-verified" &&
             baseProof.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean(),
             "prerequisite base package lacks DWF-qualified publication");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must name the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
        var bounds = new DotNetLocalRepositoryLimits(
            MaximumItems: 16, MaximumPackageBytes: 67_108_864,
            MaximumTotalBytes: 67_108_864);
        // A constructor check validates path ancestry; inspect only the two exact files.
        _ = new DotNetLocalPackageRepository(feed, bounds);
        var expressionFile = Path.Combine(feed, ExpressionId + "." + Version + ".nupkg");
        var baseFile = Path.Combine(feed, BaseId + "." + Version + ".nupkg");
        CheckPhysical(expressionFile, ExpressionArchiveSha, 13951);
        CheckPhysical(baseFile, BaseArchiveSha, 14747);
        Need(PackageDllSha(expressionFile, ExpressionId, Version,
                 [(BaseId, "[" + Version + "]")]) == ExpressionDllSha &&
             PackageDllSha(baseFile, BaseId, Version, []) == BaseDllSha,
             "physical published package DLL or dependency graph differs from qualification");

        var shared = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(shared.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             FileSha(shared.Location) == SharedNuGetDllSha,
             "the loaded federation reader is not the qualified published DLL");

        var workspace = Path.Combine(root, "build", "artifacts",
            "rs140-published-expression-" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(workspace), "a run-owned verification directory is occupied");
        Directory.CreateDirectory(workspace);
        try
        {
            var snapshot = Path.Combine(workspace, "bounded-package-snapshot");
            var consumer = Path.Combine(workspace, "binary-consumer");
            Directory.CreateDirectory(snapshot);
            Directory.CreateDirectory(consumer);
            var expressionCopy = Path.Combine(snapshot, Path.GetFileName(expressionFile));
            var baseCopy = Path.Combine(snapshot, Path.GetFileName(baseFile));
            File.Copy(expressionFile, expressionCopy, overwrite: false);
            File.Copy(baseFile, baseCopy, overwrite: false);
            CheckPhysical(expressionCopy, ExpressionArchiveSha, 13951);
            CheckPhysical(baseCopy, BaseArchiveSha, 14747);

            var packageReader = new DotNetLocalPackageRepository(snapshot, bounds);
            var inventory = packageReader.List();
            Need(inventory.InspectedPackages == 2 && inventory.MatchedPackages == 2 &&
                 inventory.Packages.Count == 2 &&
                 inventory.Packages.Any(x =>
                     x.PackageId == ExpressionId && x.Version == Version &&
                     x.Sha256 == ExpressionArchiveSha && x.Bytes == 13951) &&
                 inventory.Packages.Any(x =>
                     x.PackageId == BaseId && x.Version == Version &&
                     x.Sha256 == BaseArchiveSha && x.Bytes == 14747),
                 "the existing NuGet reader rejected the exact two-package snapshot");

            var fixture = RepoFile(root, ConsumerFixture);
            Need(File.Exists(fixture), "RS138 source-free six-case C# fixture is missing");
            File.Copy(fixture, Path.Combine(consumer, "Program.cs"), overwrite: false);
            var project = Path.Combine(consumer, "rs140-expression-consumer.csproj");
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
                    <PackageReference Include="dw.quantities.expression" Version="[20260.20261102.1001]" />
                  </ItemGroup>
                </Project>
                """, new UTF8Encoding(false));
            var config = Path.Combine(consumer, "NuGet.Config");
            File.WriteAllText(config,
                "<configuration><packageSources><clear /></packageSources></configuration>\n",
                new UTF8Encoding(false));
            var cache = Path.Combine(workspace, "private-nuget-packages");
            var commands = new List<object>();
            commands.Add(Run(root, workspace, "consumer-restore", "dotnet", [
                "restore", project, "--configfile", config,
                "--source", feed, "--packages", cache, "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, workspace, "consumer-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            var consumerExecution = Run(root, workspace, "consumer-binary", "dotnet", [
                "run", "--project", project, "-c", "Release",
                "--no-build", "--no-restore", "--", ExpressionDllSha, BaseDllSha
            ]);
            commands.Add(consumerExecution.Receipt);
            Need(consumerExecution.Output.Contains("MATH-EXPRESSION-BINARY|PASS|6",
                     StringComparison.Ordinal) &&
                 consumerExecution.Output.Contains(
                     "MATH-EXPRESSION-CHECK|loaded-binaries|PASS",
                     StringComparison.Ordinal),
                 "independent exact-package binary consumer did not pass six controls");
            CheckPhysical(expressionFile, ExpressionArchiveSha, 13951);
            CheckPhysical(baseFile, BaseArchiveSha, 14747);
            Need(FileSha(shared.Location) == SharedNuGetDllSha,
                 "shared NuGet library changed during independent observation");
            var sdk = Run(root, workspace, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 4, "the four exact consumer/SDK command receipts are required");

            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.expression-shared-feed-binary-observation",
                status = "expression-published-local-binary-verified-read-only",
                packageId = ExpressionId,
                packageVersion = Version,
                archiveSha256 = ExpressionArchiveSha,
                archiveBytes = 13951,
                packagedDllSha256 = ExpressionDllSha,
                loadedDllSha256 = ExpressionDllSha,
                exactDependencyPackageId = BaseId,
                exactDependencyPackageVersion = Version,
                exactDependencyArchiveSha256 = BaseArchiveSha,
                exactDependencyArchiveBytes = 14747,
                packagedDependencyDllSha256 = BaseDllSha,
                loadedDependencyDllSha256 = BaseDllSha,
                packageDependencyCount = 1,
                sharedNugetVersion = "20260.20261026.1010",
                sharedNugetDllSha256 = SharedNuGetDllSha,
                feedSelector = "NUGET_LOCAL",
                packageReaderSnapshotCount = inventory.InspectedPackages,
                actualPhysicalArchiveVerified = true,
                independentPackageReference = true,
                publishedArchiveUnchanged = true,
                feedWideInventoryPerformed = false,
                sourceProjectReferencesUsed = false,
                compiledConsumerChecks = 6,
                publicationInvoked = false,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false,
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

    private static void CheckPhysical(string path, string sha, int length)
    {
        Need(File.Exists(path) &&
             (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
             new FileInfo(path).Length == length &&
             FileSha(path) == sha,
             "the local package is absent, redirected, or differs from its immutable SHA");
    }

    private static string PackageDllSha(string file, string packageId, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var zip = ZipFile.OpenRead(file);
        Need(zip.Entries.Count is >= 3 and <= 48 &&
             zip.Entries.Select(z => z.FullName).Distinct(StringComparer.Ordinal)
                 .Count() == zip.Entries.Count &&
             zip.Entries.All(z => !z.FullName.Contains("..", StringComparison.Ordinal) &&
                !z.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !z.FullName.Contains('\\')),
             "package ZIP paths must remain bounded and canonical");
        var nuspec = zip.GetEntry(packageId + ".nuspec") ??
            throw new InvalidDataException("expected exact package nuspec missing");
        using (var metadata = nuspec.Open())
        {
            var doc = XDocument.Load(metadata, LoadOptions.None);
            Need(doc.Descendants().Any(x => x.Name.LocalName == "id" &&
                    x.Value == packageId) &&
                 doc.Descendants().Any(x => x.Name.LocalName == "version" &&
                    x.Value == version),
                 "package ID and version differ from exact pinned dependency closure");
            var actual = doc.Descendants().Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? "")).ToArray();
            Need(actual.SequenceEqual(dependencies),
                "package contains a different NuGet dependency closure");
        }
        var dll = zip.GetEntry("lib/net10.0/" + packageId + ".dll") ??
            throw new InvalidDataException("the expected exact net10.0 DLL is missing");
        using var stream = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }

    private static JsonDocument ReadJson(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string RepoFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string FileSha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool valid, string reason)
    {
        if (!valid) throw new InvalidDataException(reason);
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
            Path.Combine(workspace, "private-nuget-packages");
        foreach (var argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not run exact NuGet verification " + id);
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("bounded NuGet verification timed out: " + id);
        }
        var output = standardOutput.GetAwaiter().GetResult();
        var errors = standardError.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var details = output + "\n" + errors;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + details[..System.Math.Min(details.Length, 3500)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }
}
