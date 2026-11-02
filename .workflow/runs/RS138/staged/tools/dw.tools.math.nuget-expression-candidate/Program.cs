using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetExpressionCandidate;

internal static class Program
{
    private const string ExpressionId = "dw.quantities.expression";
    private const string OriginalVersion = "0.2.0-preview.1";
    private const string BaseId = "dw.quantities";
    private const string BaseVersion = "20260.20261102.1001";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string CoreDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string SourceProject =
        "src/projects/dw.quantities.expression/dw.quantities.expression.csproj";
    private const string SourceDirectory = "src/projects/dw.quantities.expression";
    private const string F1Tool =
        "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
    private const string Fixture =
        "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs";

    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    private static int Main(string[] arguments)
    {
        try
        {
            if (arguments.Length != 2 || arguments[0] != "--qualify-expression")
            {
                Console.Error.WriteLine("Expected --qualify-expression <repository-root>.");
                return 2;
            }
            Qualify(Path.GetFullPath(arguments[1]));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("MATH-EXPRESSION-CANDIDATE-REFUSED: " + exception.Message);
            return 1;
        }
    }

    private static void Qualify(string root)
    {
        var rootProject = Path.Combine(root, SourceProject.Replace('/', Path.DirectorySeparatorChar));
        var originalDirectory = Path.Combine(root,
            SourceDirectory.Replace('/', Path.DirectorySeparatorChar));
        Need(File.Exists(rootProject) && Directory.Exists(originalDirectory),
            "the original expression source and project are required");
        var original = XDocument.Load(rootProject, LoadOptions.None);
        string? Property(string name) => original.Descendants()
            .Where(x => x.Name.LocalName == name).Select(x => x.Value.Trim()).FirstOrDefault();
        var referenced = original.Descendants()
            .Where(x => x.Name.LocalName == "ProjectReference").ToArray();
        Need(Property("PackageId") == ExpressionId &&
             Property("Version") == OriginalVersion &&
             Property("AssemblyName") == ExpressionId &&
             Property("RootNamespace") == ExpressionId &&
             Property("IsPackable") == "true" &&
             referenced.Length == 1 &&
             (string?)referenced[0].Attribute("Include") ==
                 "../dw.quantities/dw.quantities.csproj" &&
             !original.Descendants().Any(x => x.Name.LocalName == "PackageReference"),
             "original source's exact historical one-project dependency changed");
        var sourceFiles = Directory.EnumerateFiles(originalDirectory, "*.cs",
            SearchOption.TopDirectoryOnly).Order(StringComparer.Ordinal).ToArray();
        var expectedNames = new[] { "ExactSelection.cs", "ExpressionDiagnostics.cs", "ExpressionParser.cs" };
        Need(sourceFiles.Select(Path.GetFileName).SequenceEqual(expectedNames, StringComparer.Ordinal),
             "the original source set changed or includes an unexpected file");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must name the physical local feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute directory");
        var publishedBase = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
        Need(File.Exists(publishedBase) &&
             (File.GetAttributes(publishedBase) & FileAttributes.ReparsePoint) == 0 &&
             new FileInfo(publishedBase).Length == 14747 &&
             ShaFile(publishedBase) == BaseArchiveSha &&
             InspectDll(publishedBase, BaseId, BaseVersion, Array.Empty<(string, string)>()) ==
                 BaseDllSha,
             "the RS137 published dw.quantities package is missing or differs");
        var nugetAssembly = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(nugetAssembly.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             ShaFile(nugetAssembly.Location) == CoreDllSha,
             "the NuGet core must match its qualified installed binary");

        var rootScratch = Path.Combine(root, "build", "artifacts",
            "rs138-expression-candidate");
        var packDirectory = Path.Combine(rootScratch, "candidate");
        var producerDirectory = Path.Combine(rootScratch, "producer");
        var consumerDirectory = Path.Combine(rootScratch, "consumer");
        var candidateConfig = Path.Combine(producerDirectory, "NuGet.Config");
        var archivePrefix = ExpressionId + ".";
        Need(!Directory.Exists(producerDirectory) && !Directory.Exists(consumerDirectory),
             "run-owned candidate output already exists; no accidental replacement");
        Directory.CreateDirectory(packDirectory);
        Directory.CreateDirectory(producerDirectory);
        Directory.CreateDirectory(consumerDirectory);

        var cache = Path.Combine(root, ".workflow", "local", "nuget-rs138", "packages");
        var commands = new List<object>();
        var before = Run(root, cache, "f1-feed-inspection-before", "dotnet", [
            "run", "--project", F1Tool, "-c", "Release", "--no-build",
            "--no-restore", "--", "--inspect", "."
        ]);
        commands.Add(before.Receipt);
        using var beforeJson = JsonDocument.Parse(before.Output.Trim());
        var f1 = beforeJson.RootElement;
        Need(Get(f1, "status") == "f1-source-and-feed-inventory-qualified-not-published" &&
             Get(f1, "repositoryTool") == "Dw.Tools.NuGet" &&
             Get(f1, "repositoryPackageVersion") == "20260.20261026.1010" &&
             Get(f1, "loadedLibrarySha256") == CoreDllSha &&
             f1.GetProperty("packageCount").GetInt32() == 13 &&
             !f1.GetProperty("feedPublished").GetBoolean(),
             "unchanged RS134 feed/version proposal authority is missing");
        var expressionEntry = f1.GetProperty("packages").EnumerateArray()
            .Single(x => Get(x, "packageId") == ExpressionId);
        Need(Get(expressionEntry, "historicalSourceVersion") == OriginalVersion &&
             Get(expressionEntry, "effectivePackageVersion") == OriginalVersion &&
             expressionEntry.GetProperty("projectReferences").EnumerateArray()
                 .Select(x => x.GetString()).SequenceEqual(new[] { BaseId }) &&
             expressionEntry.GetProperty("externalPackageReferences").GetArrayLength() == 0,
             "the original expression's evaluated package dependency changed");
        var proposed = Get(expressionEntry, "candidateVersionPreview");
        var date = DateOnly.FromDateTime(DateTime.Now);
        Need(date.Year == 2026 &&
             proposed.StartsWith("20260." +
                 date.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + ".",
                 StringComparison.Ordinal) &&
             proposed.Split('.').Length == 3 &&
             int.TryParse(proposed.Split('.')[2], out var counter) &&
             counter is >= 1001 and <= 1999,
             "the existing F1 calendar allocator did not propose today's 2026 version");

        var sourceHashes = sourceFiles.ToDictionary(
            path => Path.GetRelativePath(root, path).Replace('\\', '/'),
            ShaFile, StringComparer.Ordinal);
        var sourceProjectSha = ShaFile(rootProject);
        var generatedProject = Path.Combine(producerDirectory, "expression-candidate.csproj");
        // This explicit source-linked packaging projection replaces the historical
        // ProjectReference ONLY in a disposable build project, never in the source.
        var projection = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", ExpressionId),
                    new XElement("RootNamespace", ExpressionId),
                    new XElement("PackageId", ExpressionId),
                    new XElement("Version", OriginalVersion),
                    new XElement("PackageVersion", proposed),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("Description", Property("Description")),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    sourceFiles.Select(source => new XElement("Compile",
                        new XAttribute("Include", source),
                        new XAttribute("Link", Path.GetFileName(source)))),
                    new XElement("PackageReference",
                        new XAttribute("Include", BaseId),
                        new XAttribute("Version", "[" + BaseVersion + "]")))));
        File.WriteAllText(generatedProject,
            projection.ToString(SaveOptions.DisableFormatting) + "\n");
        WriteSourcesConfig(candidateConfig,
            ("published", feed));
        commands.Add(Run(root, cache, "linked-source-restore", "dotnet", [
            "restore", generatedProject, "--configfile", candidateConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "linked-source-build", "dotnet", [
            "build", generatedProject, "-c", "Release",
            "--no-restore", "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "expression-candidate-pack", "dotnet", [
            "pack", generatedProject, "-c", "Release",
            "--no-build", "--no-restore",
            "--output", packDirectory, "-p:NuGetAudit=false"
        ]).Receipt);
        var candidateFile = Path.Combine(packDirectory,
            archivePrefix + proposed + ".nupkg");
        Need(File.Exists(candidateFile),
            "the generated expression candidate has no expected archive");
        var declaredDependency = (BaseId, "[" + BaseVersion + "]");
        var expressionDll = InspectDll(candidateFile, ExpressionId, proposed,
            [declaredDependency]);
        var archiveSha = ShaFile(candidateFile);
        var candidateRepo = new DotNetLocalPackageRepository(packDirectory,
            new DotNetLocalRepositoryLimits(64, 67_108_864, 67_108_864));
        var candidateListing = candidateRepo.List(ExpressionId);
        Need(candidateListing.InspectedPackages == 1 &&
             candidateListing.MatchedPackages == 1,
             "shared NuGet reader did not validate exactly one generated expression");
        var item = candidateListing.Packages.Single();
        Need(item.PackageId == ExpressionId && item.Version == proposed &&
             item.Sha256 == archiveSha,
             "shared NuGet reader produced a different package identity or hash");

        var originalConsumer = Path.Combine(root,
            Fixture.Replace('/', Path.DirectorySeparatorChar));
        Need(File.Exists(originalConsumer),
            "the checked-in exact package-only consumer fixture is missing");
        File.Copy(originalConsumer,
            Path.Combine(consumerDirectory, "Program.cs"), overwrite: false);
        var consumerProject = Path.Combine(consumerDirectory, "expression-consumer.csproj");
        var consumerXml = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Exe"),
                    new XElement("IsPackable", "false"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Include", "Program.cs")),
                    new XElement("PackageReference",
                        new XAttribute("Include", ExpressionId),
                        new XAttribute("Version", "[" + proposed + "]")))));
        File.WriteAllText(consumerProject,
            consumerXml.ToString(SaveOptions.DisableFormatting) + "\n");
        var consumerConfig = Path.Combine(consumerDirectory, "NuGet.Config");
        WriteSourcesConfig(consumerConfig, ("candidate", packDirectory), ("published", feed));
        commands.Add(Run(root, cache, "consumer-local-only-restore", "dotnet", [
            "restore", consumerProject, "--configfile", consumerConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "consumer-release-build", "dotnet", [
            "build", consumerProject, "-c", "Release",
            "--no-restore", "-p:NuGetAudit=false"
        ]).Receipt);
        var smoke = Run(root, cache, "consumer-binary-run", "dotnet", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", expressionDll, BaseDllSha
        ]);
        commands.Add(smoke.Receipt);
        Need(smoke.Output.Contains("MATH-EXPRESSION-BINARY|PASS|6",
             StringComparison.Ordinal) &&
             smoke.Output.Contains("MATH-EXPRESSION-CHECK|loaded-binaries|PASS",
             StringComparison.Ordinal),
             "the source-free expression+base binary consumer failed");

        var after = Run(root, cache, "f1-feed-inspection-after", "dotnet", [
            "run", "--project", F1Tool, "-c", "Release", "--no-build",
            "--no-restore", "--", "--inspect", "."
        ]);
        commands.Add(after.Receipt);
        using var afterJson = JsonDocument.Parse(after.Output.Trim());
        var current = afterJson.RootElement;
        Need(Get(f1, "inventorySha256") == Get(current, "inventorySha256") &&
             f1.GetProperty("inspectedArchives").GetInt32() ==
                 current.GetProperty("inspectedArchives").GetInt32() &&
             Get(current.GetProperty("packages").EnumerateArray()
                 .Single(x => Get(x, "packageId") == ExpressionId),
                 "candidateVersionPreview") == proposed &&
             ShaFile(publishedBase) == BaseArchiveSha &&
             ShaFile(candidateFile) == archiveSha &&
             ShaFile(nugetAssembly.Location) == CoreDllSha,
             "shared feed, original published base, or candidate changed");
        var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
        commands.Add(sdk.Receipt);
        Need(commands.Count == 9, "nine as-run source, consumer and feed commands expected");

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.nuget-expression-calendar-candidate",
            status = "qualified-local-dependent-candidate-not-published",
            packageId = ExpressionId,
            packageVersion = proposed,
            historicalSourceVersion = OriginalVersion,
            originalProjectSha256 = sourceProjectSha,
            originalSourceSha256 = sourceHashes,
            temporaryPackagingProjection = true,
            sourceProjectUnmodified = true,
            sourceDllCompilationFromUnchangedSources = true,
            sourceProjectReferencesRetained = 1,
            packagedDependencies = 1,
            exactBasePackageId = BaseId,
            exactBasePackageVersion = BaseVersion,
            exactBasePackageSha256 = BaseArchiveSha,
            exactBaseDllSha256 = BaseDllSha,
            candidateArchivePath = Path.GetRelativePath(root, candidateFile).Replace('\\', '/'),
            candidateArchiveSha256 = archiveSha,
            candidateArchiveBytes = new FileInfo(candidateFile).Length,
            candidateDllSha256 = expressionDll,
            sourceFeed = "NUGET_LOCAL",
            sourceFeedInventoryUnchanged = true,
            sharedNugetPackageVersion = "20260.20261026.1010",
            sharedNugetDllSha256 = CoreDllSha,
            published = false,
            versionReserved = false,
            publicLicenseSelected = false,
            installedRollbackQualified = false,
            consumerChecks = 6,
            actualSourceFreeConsumerPassed = true,
            privateCandidateFeedOnly = true,
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            sdkVersion = sdk.Output.Trim(),
            commands
        }, Json));
    }

    private static string InspectDll(string archivePath, string id, string version,
        IReadOnlyList<(string Id, string Version)> expectedDependencies)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(e => e.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !e.FullName.Contains('\\')),
             "unqualified NuGet ZIP structure");
        var manifest = archive.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("expected NuGet package manifest missing");
        using (var stream = manifest.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(e => e.Name.LocalName == "id" &&
                 e.Value == id) &&
                 xml.Descendants().Any(e => e.Name.LocalName == "version" &&
                 e.Value == version), "candidate package identity or version mismatch");
            var actualDependencies = xml.Descendants()
                .Where(e => e.Name.LocalName == "dependency")
                .Select(e => (Id: (string?)e.Attribute("id") ?? "",
                    Version: (string?)e.Attribute("version") ?? "")).ToArray();
            Need(actualDependencies.Length == expectedDependencies.Count &&
                 actualDependencies.SequenceEqual(expectedDependencies),
                 "NuGet package must declare exactly the published base version");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("candidate package net10 DLL absent");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static void WriteSourcesConfig(string path,
        params (string Key, string Directory)[] sources)
    {
        var xml = new XDocument(
            new XElement("configuration",
                new XElement("packageSources",
                    new XElement("clear"),
                    sources.Select(x => new XElement("add",
                        new XAttribute("key", x.Key),
                        new XAttribute("value", x.Directory))))));
        File.WriteAllText(path, xml.ToString(SaveOptions.DisableFormatting) + "\n");
    }

    private static string Get(JsonElement element, string key) =>
        element.GetProperty(key).GetString() ??
        throw new InvalidDataException("missing qualified NuGet identity " + key);

    private static string ShaFile(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool valid, string message)
    {
        if (!valid) throw new InvalidDataException(message);
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
        foreach (var argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);
        var clock = Stopwatch.StartNew();
        Need(process.Start(), "could not start bounded expression candidate command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("expression candidate command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        clock.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostic = output + "\n" + errors;
            throw new InvalidDataException(id + " returned exit " + process.ExitCode +
                "\n" + diagnostic[..System.Math.Min(diagnostic.Length, 4000)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = clock.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1250)]
        });
    }
}
