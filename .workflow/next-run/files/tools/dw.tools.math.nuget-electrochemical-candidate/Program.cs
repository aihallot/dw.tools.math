using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetElectrochemicalCandidate;

internal static class Program
{
    private const string PackageId = "dw.quantities.standard";
    private const string HistoricalVersion = "0.2.0-preview.1";
    private const string BaseId = "dw.quantities";
    private const string ExpressionId = "dw.quantities.expression";
    private const string PublishedVersion = "20260.20261102.1001";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string ExpressionArchiveSha =
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
    private const string ExpressionDllSha =
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
    private const string SharedNuGetDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string SourceProject =
        "src/projects/dw.quantities.standard/dw.quantities.standard.csproj";
    private const string SourceFolder = "src/projects/dw.quantities.standard";
    private const string OriginalFixture =
        "tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs";
    private const string F8 =
        "docs/distribution/nuget-federation-f8-standard-candidate-qualified.json";
    private const string F10 =
        "docs/distribution/nuget-federation-f10-standard-published-qualified.json";
    private const string F39 =
        "docs/distribution/electrochemical-unit-profile-source-qualified.json";
    private const string OldVersion = "20260.20261103.1001";
    private const string OldSha =
        "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75";
    private static readonly string[] Names =
    [
        "ElectrochemicalUnitProfile.cs", "ExplicitProfileExpressionUnitResolver.cs",
        "InheritedStandardUnitCatalog.cs",
        "PureUnitConverter.cs", "StandardExpressionUnitResolver.cs", "StandardUnitCatalog.cs"
    ];
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 || args[0] != "--qualify-candidate")
            {
                Console.Error.WriteLine("Expected --qualify-candidate <repository-root>.");
                return 2;
            }
            Qualify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-ELECTROCHEM-CANDIDATE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Qualify(string root)
    {
        var sourceProject = RepoFile(root, SourceProject);
        var folder = RepoFile(root, SourceFolder);
        Need(File.Exists(sourceProject) && Directory.Exists(folder),
             "original Standard source project is missing");
        var xml = XDocument.Load(sourceProject, LoadOptions.None);
        string? Property(string name) => xml.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        var references = xml.Descendants()
            .Where(x => x.Name.LocalName == "ProjectReference")
            .Select(x => (string?)x.Attribute("Include")).ToArray();
        Need(Property("PackageId") == PackageId &&
             Property("AssemblyName") == PackageId &&
             Property("Version") == HistoricalVersion &&
             Property("RootNamespace") == "Dw.Quantities.Standard" &&
             Property("IsPackable") == "true" &&
             references.SequenceEqual(new[]
             {
                 "../dw.quantities/dw.quantities.csproj",
                 "../dw.quantities.expression/dw.quantities.expression.csproj"
             }) &&
             !xml.Descendants().Any(x => x.Name.LocalName == "PackageReference"),
             "original Standard project identity or its two source dependencies changed");
        var sourceFiles = Directory.EnumerateFiles(folder, "*.cs",
            SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(sourceFiles.Select(Path.GetFileName).SequenceEqual(Names, StringComparer.Ordinal),
             "the exact six Math Standard C# sources must be unchanged");
        var originalProjectSha = ShaFile(sourceProject);
        var originalSources = sourceFiles.ToDictionary(
            x => Path.GetRelativePath(root, x).Replace('\\', '/'),
            ShaFile, StringComparer.Ordinal);
        using var oldCandidate = JsonDocument.Parse(File.ReadAllText(RepoFile(root, F8)));
        using var published = JsonDocument.Parse(File.ReadAllText(RepoFile(root, F10)));
        using var sourceQualification = JsonDocument.Parse(File.ReadAllText(RepoFile(root, F39)));
        using var priorRun = JsonDocument.Parse(File.ReadAllText(RepoFile(root,
            ".aura/workflow/reports/RS173/attempt-001.json")));
        var baseline = oldCandidate.RootElement.GetProperty("candidate");
        var sourceProof = sourceQualification.RootElement;
        var prior = priorRun.RootElement;
        Need(Get(prior, "runId") == "RS173" &&
             prior.GetProperty("attempt").GetInt32() == 1 &&
             Get(prior, "outcome") == "succeeded" &&
             Get(prior.GetProperty("git"), "commitMessage") ==
                "feat: add explicit Math electrochemical SI unit profile with exact dimensions" &&
             Get(sourceProof, "status") ==
                "rs173-math-electrochemical-six-unit-source-qualified-not-packaged" &&
             sourceProof.GetProperty("tests").GetProperty("passed").GetInt32() == 12 &&
             !sourceProof.GetProperty("package_published").GetBoolean() &&
             !sourceProof.GetProperty("aura_code_reuse_rights_established").GetBoolean() &&
             Get(oldCandidate.RootElement, "status") ==
                "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished" &&
             Get(published.RootElement, "status") ==
                "rs143-standard-published-local-three-binaries-independently-verified",
             "original source and previous immutable Standard proofs must agree");
        Need(ShaFile(sourceProject) == Get(baseline, "originalProjectSha256"),
             "original Standard csproj SHA differs from published baseline");
        var fiveOriginalHashes = baseline.GetProperty("originalSourcesSha256");
        var newProfileHashes = sourceProof.GetProperty("source_files");
        foreach (var item in originalSources)
        {
            var wanted = item.Key.EndsWith("/ElectrochemicalUnitProfile.cs",
                StringComparison.Ordinal) ? Get(newProfileHashes, item.Key) :
                Get(fiveOriginalHashes, item.Key);
            Need(item.Value == wanted, "six original source SHA identities changed " + item.Key);
        }
        var fixture = RepoFile(root, OriginalFixture);
        Need(File.Exists(fixture), "the independent Standard C# consumer fixture is missing");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL is required");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an existing absolute local feed");
        var baseArchive = Path.Combine(feed, BaseId + "." + PublishedVersion + ".nupkg");
        var expressionArchive = Path.Combine(feed,
            ExpressionId + "." + PublishedVersion + ".nupkg");
        Need(CheckPhysical(baseArchive, 14747, BaseArchiveSha) &&
             CheckPhysical(expressionArchive, 13951, ExpressionArchiveSha) &&
             CheckDll(baseArchive, BaseId, PublishedVersion, []) == BaseDllSha &&
             CheckDll(expressionArchive, ExpressionId, PublishedVersion,
                 [(BaseId, "[" + PublishedVersion + "]")]) == ExpressionDllSha,
             "the published exact dependency packages are missing or changed");

        var oldPackage = Path.Combine(feed, PackageId + "." + OldVersion + ".nupkg");
        Need(CheckPhysical(oldPackage, 13604, OldSha),
             "old physically published Standard archive must remain identical");
        var loadedCore = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(loadedCore.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             ShaFile(loadedCore.Location) == SharedNuGetDllSha,
             "loaded shared NuGet reader is not the pinned qualified DLL");
        // The private flat feed may contain unrelated child directories.
        // The shared reader explicitly skips them without traversing descendants;
        // every top-level package remains inspected under the original limits.
        var existing = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824))
            .List(PackageId, ignoreSubdirectories: true);
        Need(existing.DirectoryPolicy == "ignore" &&
             existing.ScannedEntries >= existing.InspectedPackages &&
             existing.MatchedPackages >= 1,
             "private feed top-level inventory must be complete and explicit");
        Need(existing.Packages.Any(p => p.Version == OldVersion && p.Sha256 == OldSha),
             "shared NuGet reader did not confirm old immutable package");
        var date = DateOnly.FromDateTime(DateTime.Now);
        Need(date.Year == 2026, "calendar candidate is outside 2026");
        var used = existing.Packages.Select(p => p.Version).ToHashSet(StringComparer.Ordinal);
        var version = Enumerable.Range(1001, 999)
            .Select(n => "20260." + date.ToString("yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture) + "." +
                n.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .FirstOrDefault(v => !used.Contains(v)) ??
            throw new InvalidDataException("all on-date Standard versions occupied");
        Need(!File.Exists(Path.Combine(feed, PackageId + "." + version + ".nupkg")),
             "candidate must not overwrite a published identity");
        var scratch = RepoFile(root, "build/artifacts/rs174-electrochemical-candidate/" +
            Guid.NewGuid().ToString("N"));
        var cache = Path.Combine(scratch, "producer-cache");
        var consumerCache = Path.Combine(scratch, "consumer-cache");
        var commands = new List<object>();
        var producer = Path.Combine(scratch, "producer");
        var candidate = Path.Combine(scratch, "candidate");
        var consumer = Path.Combine(scratch, "consumer");
        Need(!Directory.Exists(scratch), "candidate workspace must be new");
        Need(!Directory.Exists(producer) && !Directory.Exists(consumer),
             "run-owned candidate/consumer workspace is already occupied");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(candidate);
        Directory.CreateDirectory(consumer);
        var producerProject = Path.Combine(producer, "electrochemical-candidate.csproj");
        var producerConfig = Path.Combine(producer, "NuGet.Config");

        // A disposable packaging projection links source bytes unchanged and
        // replaces ONLY temporary ProjectReferences with two exact PackageReferences.
        var projection = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", Property("RootNamespace")),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", HistoricalVersion),
                    new XElement("PackageVersion", version),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("Description", Property("Description")),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    sourceFiles.Select(path => new XElement("Compile",
                        new XAttribute("Include", path),
                        new XAttribute("Link", Path.GetFileName(path)))),
                    new XElement("PackageReference",
                        new XAttribute("Include", BaseId),
                        new XAttribute("Version", "[" + PublishedVersion + "]")),
                    new XElement("PackageReference",
                        new XAttribute("Include", ExpressionId),
                        new XAttribute("Version", "[" + PublishedVersion + "]")))));
        File.WriteAllText(producerProject,
            projection.ToString(SaveOptions.DisableFormatting) + "\n");
        File.WriteAllText(producerConfig, Sources(("published", feed)));

        commands.Add(Run(root, cache, "standard-linked-restore", "dotnet", [
            "restore", producerProject, "--configfile", producerConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "standard-linked-build", "dotnet", [
            "build", producerProject, "-c", "Release",
            "--no-restore", "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "standard-candidate-pack", "dotnet", [
            "pack", producerProject, "-c", "Release", "--no-build", "--no-restore",
            "--output", candidate, "-p:NuGetAudit=false"
        ]).Receipt);
        var archive = Path.Combine(candidate, PackageId + "." + version + ".nupkg");
        Need(File.Exists(archive), "on-date Standard candidate archive is missing");
        var archiveSha = ShaFile(archive);
        var dllSha = CheckDll(archive, PackageId, version,
        [
            (BaseId, "[" + PublishedVersion + "]"),
            (ExpressionId, "[" + PublishedVersion + "]")
        ]);
        var packageReader = new DotNetLocalPackageRepository(candidate,
            new DotNetLocalRepositoryLimits(32, 67_108_864, 67_108_864));
        var observed = packageReader.List(PackageId);
        Need(observed.MatchedPackages == 1 && observed.InspectedPackages == 1 &&
             observed.Packages.Single().PackageId == PackageId &&
             observed.Packages.Single().Version == version &&
             observed.Packages.Single().Sha256 == archiveSha,
             "published NuGet reader rejected candidate archive identity or hash");

        File.Copy(fixture, Path.Combine(consumer, "Program.cs"), overwrite: false);
        var consumerProject = Path.Combine(consumer, "electrochemical-consumer.csproj");
        var consumerConfig = Path.Combine(consumer, "NuGet.Config");
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
                        new XAttribute("Include", PackageId),
                        new XAttribute("Version", "[" + version + "]")))));
        File.WriteAllText(consumerProject,
            consumerXml.ToString(SaveOptions.DisableFormatting) + "\n");
        File.WriteAllText(consumerConfig,
            Sources(("candidate", candidate), ("published", feed)));
        commands.Add(Run(root, consumerCache, "consumer-local-only-restore", "dotnet", [
            "restore", consumerProject, "--configfile", consumerConfig,
            "--packages", consumerCache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, consumerCache, "consumer-release-build", "dotnet", [
            "build", consumerProject, "-c", "Release",
            "--no-restore", "-p:NuGetAudit=false"
        ]).Receipt);
        var consumerTest = Run(root, consumerCache, "consumer-binary-run", "dotnet", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha, ExpressionDllSha, BaseDllSha
        ]);
        commands.Add(consumerTest.Receipt);
        Need(consumerTest.Output.Contains("MATH-ELECTROCHEM-BINARY|PASS|12",
             StringComparison.Ordinal) &&
             consumerTest.Output.Split("MATH-ELECTROCHEM-CHECK|").Length - 1 == 12 &&
             consumerTest.Output.Contains(
                 "MATH-ELECTROCHEM-CHECK|three-loaded-dlls-and-catalogue-identity|PASS",
                 StringComparison.Ordinal),
             "fresh independent package-only consumer did not pass twelve checks");
        var producerSources = ActualSources(root, cache, producerProject, commands);
        var consumerSources = ActualSources(root, consumerCache, consumerProject, commands);
        Need(producerSources.Length >= 1 && consumerSources.Length >= 2 &&
             producerSources.All(x => !x.Contains("://", StringComparison.Ordinal)) &&
             consumerSources.All(x => !x.Contains("://", StringComparison.Ordinal)),
             "actual restore sources must be local and recorded");
        var loadedClrIdentities = consumerTest.Output
            .Split(new[] { (char)13, (char)10 }, StringSplitOptions.RemoveEmptyEntries)
            .Single(x => x.StartsWith("MATH-ELECTROCHEM-CLR|standard=",
                StringComparison.Ordinal));
        var archiveEntries = Inventory(archive);
        Need(CheckPhysical(baseArchive, 14747, BaseArchiveSha) &&
             CheckPhysical(expressionArchive, 13951, ExpressionArchiveSha) &&
             CheckPhysical(oldPackage, 13604, OldSha) &&
             ShaFile(archive) == archiveSha &&
             ShaFile(sourceProject) == originalProjectSha &&
             originalSources.All(x => ShaFile(RepoFile(root, x.Key)) == x.Value) &&
             ShaFile(loadedCore.Location) == SharedNuGetDllSha &&
             !File.Exists(Path.Combine(feed, PackageId + "." + version + ".nupkg")),
             "original five, new sixth source, shared archives or candidate changed");
        var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
        commands.Add(sdk.Receipt);
        Need(commands.Count == 9,
             "nine candidate, consumer, source discovery and SDK commands required");

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.nuget-electrochemical-dependent-candidate",
            status = "qualified-electrochemical-standard-calendar-candidate-unpublished",
            packageId = PackageId,
            packageVersion = version,
            historicalSourceVersion = HistoricalVersion,
            originalProjectSha256 = originalProjectSha,
            originalSourcesSha256 = originalSources,
            originalSourceProjectReferences = 2,
            sourceProjectUnmodified = true,
            temporaryPackagingProjection = true,
            originalSixSourceFilesLinked = true,
            profileId = "math.si-electrochemical",
            profileVersion = "2026-11-electrochemical-exact-v1",
            profileUnitCount = 6,
            auraCodeReuseRightsEstablished = false,
            packageDependencies = 2,
            dependencyQuantitiesId = BaseId,
            dependencyQuantitiesVersion = PublishedVersion,
            dependencyQuantitiesArchiveSha256 = BaseArchiveSha,
            dependencyQuantitiesDllSha256 = BaseDllSha,
            dependencyExpressionId = ExpressionId,
            dependencyExpressionVersion = PublishedVersion,
            dependencyExpressionArchiveSha256 = ExpressionArchiveSha,
            dependencyExpressionDllSha256 = ExpressionDllSha,
            candidateArchivePath = Path.GetRelativePath(root, archive).Replace('\\', '/'),
            candidateArchiveSha256 = archiveSha,
            candidateArchiveBytes = new FileInfo(archive).Length,
            candidateDllSha256 = dllSha,
            sharedNugetPackageVersion = "20260.20261026.1010",
            sharedNugetDllSha256 = SharedNuGetDllSha,
            feedSelector = "NUGET_LOCAL",
            sourceFeedInventoryUnchanged = true,
            independentlyCompiledBinaryConsumer = true,
            consumerChecks = 12,
            loadedClrAssemblyIdentities = loadedClrIdentities,
            packagedFileInventory = archiveEntries,
            packagedDllPath = "lib/net10.0/" + PackageId + ".dll",
            producerEffectiveSources = producerSources,
            consumerEffectiveSources = consumerSources,
            loadedThreeDllHashesVerified = true,
            originalStandardArchiveSha256 = OldSha,
            oldStandardVersion = OldVersion,
            sourceFilesUnchangedAfter = true,
            legacyCataloguesPreserved = true,
            publishedToNugetLocal = false,
            published = false,
            versionReserved = false,
            publicLicenseSelected = false,
            installedRollbackQualified = false,
            targetFramework = "net10.0",
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            osDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            processArchitecture =
                System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(),
            clrAssemblyName = PackageId,
            clrSourceVersion = HistoricalVersion,
            sdkVersion = sdk.Output.Trim(),
            commands
        }, Indented));
    }

    private static string[] ActualSources(string root, string cache,
        string project, List<object> receipts)
    {
        var observed = Run(root, cache, "assets-" + Path.GetFileNameWithoutExtension(project),
            "dotnet", ["msbuild", project, "-getProperty:ProjectAssetsFile"]);
        receipts.Add(observed.Receipt);
        var path = observed.Output.Trim();
        Need(Path.IsPathFullyQualified(path) && File.Exists(path),
             "effective NuGet asset file missing");
        using var assets = JsonDocument.Parse(File.ReadAllText(path));
        return assets.RootElement.GetProperty("project").GetProperty("restore")
            .GetProperty("sources").EnumerateObject().Select(x => x.Name)
            .Order(StringComparer.Ordinal).ToArray();
    }

    private static object[] Inventory(string package)
    {
        using var zip = ZipFile.OpenRead(package);
        Need(zip.Entries.Count is >= 3 and <= 48, "bounded full package ZIP required");
        var entries = new List<object>();
        foreach (var entry in zip.Entries.OrderBy(x => x.FullName, StringComparer.Ordinal))
        {
            using var data = entry.Open();
            entries.Add(new { path = entry.FullName, bytes = entry.Length,
                compressedBytes = entry.CompressedLength,
                sha256 = Convert.ToHexStringLower(SHA256.HashData(data)) });
        }
        return entries.ToArray();
    }

    private static bool CheckPhysical(string file, int size, string sha) =>
        File.Exists(file) &&
        (File.GetAttributes(file) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
        new FileInfo(file).Length == size &&
        ShaFile(file) == sha;

    private static string CheckDll(string path, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var archive = ZipFile.OpenRead(path);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal)
                 .Count() == archive.Entries.Count &&
             archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !e.FullName.Contains('\\')),
             "NuGet archive structure is not bounded");
        var manifest = archive.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("expected package nuspec missing");
        using (var stream = manifest.Open())
        {
            var doc = XDocument.Load(stream, LoadOptions.None);
            Need(doc.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == id) &&
                 doc.Descendants().Any(x => x.Name.LocalName == "version" &&
                     x.Value == version), "NuGet archive ID/version differs");
            var declared = doc.Descendants().Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? ""))
                .OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            var expected = dependencies.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            Need(declared.SequenceEqual(expected),
                 "NuGet package must retain exactly its two exact dependencies");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("net10.0 Standard DLL is absent");
        using var binary = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(binary));
    }

    private static string Sources(params (string Key, string Path)[] values)
    {
        var config = new XDocument(
            new XElement("configuration",
                new XElement("packageSources",
                    new XElement("clear"),
                    values.Select(v => new XElement("add",
                        new XAttribute("key", v.Key),
                        new XAttribute("value", v.Path))))));
        return config.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static string RepoFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Get(JsonElement node, string key) =>
        node.GetProperty(key).GetString() ??
        throw new InvalidDataException("missing exact NuGet identity " + key);

    private static string ShaFile(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool ok, string message)
    {
        if (!ok) throw new InvalidDataException(message);
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
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "cannot start Standard candidate step " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Standard candidate command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var error = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + error;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4200)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1200)]
        });
    }
}
