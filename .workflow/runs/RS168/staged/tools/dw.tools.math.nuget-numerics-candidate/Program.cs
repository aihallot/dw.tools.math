using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetNumericsCandidate;

internal static class Program
{
    private const string PackageId = "dw.tools.math.numerics";
    private const string SourceVersion = "0.4.0-preview.4";
    private const string SourceProject = "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
    private const string SourceFolder = "src/projects/dw.tools.math.numerics";
    private const string BaseId = "dw.tools.math.ir";
    private const string BaseVersion = "20260.20261104.1001";
    private const string BaseArchiveSha =
        "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
    private const string BaseDllSha =
        "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af";
    private const string QuantitiesId = "dw.quantities";
    private const string QuantitiesVersion = "20260.20261102.1001";
    private const string QuantitiesArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string QuantitiesDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string CoreDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string F1Project =
        "tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj";
    private const string Fixture =
        "tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs";
    private static readonly string[] SourceNames =
    [
        "BoundedLinearAlgebra64.cs",
        "NumericalMatrices.cs",
        "RationalBinary64Conversion.cs",
    ];
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 || args[0] != "--qualify-numerics")
            {
                Console.Error.WriteLine("Expected --qualify-numerics <repository-root>.");
                return 2;
            }
            Qualify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUMERICS-CANDIDATE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Qualify(string root)
    {
        var projectPath = RepoFile(root, SourceProject);
        var sourceDir = RepoFile(root, SourceFolder);
        using var predecessor = JsonDocument.Parse(File.ReadAllText(RepoFile(root,
            "docs/distribution/nuget-federation-f34-composition-published-qualified.json")));
        Need(Text(predecessor.RootElement, "status") ==
                 "rs167-composition-published-local-three-binaries-independently-verified" &&
             predecessor.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             !predecessor.RootElement.GetProperty("package_published_by_this_run").GetBoolean() &&
              Text(predecessor.RootElement.GetProperty("observation"), "packageId") ==
                  "dw.tools.math.composition" &&
              predecessor.RootElement.GetProperty("observation")
                  .GetProperty("compiledConsumerChecks").GetInt32() == 15,
             "RS167 Composition original physical archive and dependencies must already be qualified");
        Need(File.Exists(projectPath) && Directory.Exists(sourceDir),
             "original Numerics project must exist");
        var projectXml = XDocument.Load(projectPath, LoadOptions.None);
        string? Property(string name) => projectXml.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        var oldReferences = projectXml.Descendants()
            .Where(x => x.Name.LocalName == "ProjectReference")
            .Select(x => (string?)x.Attribute("Include")).ToArray();
        Need(Property("AssemblyName") == PackageId &&
             Property("RootNamespace") == "Dw.Tools.Math.Numerics" &&
             Property("PackageId") == PackageId &&
             Property("Version") == SourceVersion &&
             Property("IsPackable") == "true" &&
             oldReferences.SequenceEqual(["../dw.tools.math.ir/dw.tools.math.ir.csproj"]) &&
             !projectXml.Descendants().Any(x => x.Name.LocalName == "PackageReference"),
             "original Numerics project package, version or its single source dependency drifted");

        var sources = Directory.EnumerateFiles(sourceDir, "*.cs", SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(sources.Select(Path.GetFileName).SequenceEqual(SourceNames, StringComparer.Ordinal),
             "the three original Numerics sources changed");
        var lockPath = RepoFile(root, SourceFolder + "/packages.lock.json");
        using (var lockDocument = JsonDocument.Parse(File.ReadAllText(lockPath)))
        {
            var lockRoot = lockDocument.RootElement;
            var lockTargets = lockRoot.GetProperty("dependencies")
                .GetProperty("net10.0").EnumerateObject().ToArray();
            Need(lockRoot.GetProperty("version").GetInt32() == 2 &&
                 lockTargets.Length == 2 &&
                 lockTargets.Any(x => x.Name == BaseId && Text(x.Value, "type") == "Project") &&
                 lockTargets.Any(x => x.Name == QuantitiesId && Text(x.Value, "type") == "Project"),
                 "original Numerics lockfile must preserve exactly IR and its transitive Quantities project dependencies");
        }
        var sourceProjectSha = ShaFile(projectPath);
        var sourceFingerprints = sources.ToDictionary(
            s => Path.GetRelativePath(root, s).Replace('\\', '/'),
            ShaFile, StringComparer.Ordinal);
        sourceFingerprints.Add(SourceFolder + "/packages.lock.json", ShaFile(lockPath));
        Need(File.Exists(RepoFile(root, Fixture)),
             "the separate source-free Numerics consumer oracle is missing");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must identify the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must be an absolute existing directory");
        using var publishedBaseEvidence = JsonDocument.Parse(File.ReadAllText(RepoFile(
            root, "docs/distribution/nuget-federation-f13-ir-published-qualified.json")));
        var baseObservation = publishedBaseEvidence.RootElement.GetProperty("observation");
        Need(Text(publishedBaseEvidence.RootElement, "status") ==
                 "rs146-ir-published-local-two-binaries-independently-verified" &&
             publishedBaseEvidence.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             Text(baseObservation, "packageId") == BaseId &&
             Text(baseObservation, "packageVersion") == BaseVersion &&
             Text(baseObservation, "archiveSha256") == BaseArchiveSha &&
             Text(baseObservation, "packagedDllSha256") == BaseDllSha &&
             baseObservation.GetProperty("archiveBytes").GetInt32() == 21926 &&
             baseObservation.GetProperty("declaredDependencyCount").GetInt32() == 1 &&
             baseObservation.GetProperty("exactDependencies").EnumerateArray()
                 .Any(x => Text(x, "id") == QuantitiesId &&
                     Text(x, "version") == QuantitiesVersion &&
                     Text(x, "archiveSha256") == QuantitiesArchiveSha &&
                     Text(x, "packagedDllSha256") == QuantitiesDllSha),
             "F13 independently published IR dependency evidence is not exact");
        var baseArchive = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
        Need(CheckPhysical(baseArchive, BaseArchiveSha, 21926) &&
             PackagedDllSha(baseArchive, BaseId, BaseVersion,
                 [(QuantitiesId, "[" + QuantitiesVersion + "]")]) == BaseDllSha,
             "published direct IR dependency differs from F13 physical readback");
        var quantitiesArchive = Path.Combine(feed, QuantitiesId + "." + QuantitiesVersion + ".nupkg");
        Need(CheckPhysical(quantitiesArchive, QuantitiesArchiveSha, 14747) &&
             PackagedDllSha(quantitiesArchive, QuantitiesId, QuantitiesVersion, []) ==
                 QuantitiesDllSha,
             "published transitive Quantities package differs from exact F13 dependency");
        var nugetLibrary = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(nugetLibrary.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             ShaFile(nugetLibrary.Location) == CoreDllSha,
             "the shared NuGet reader is not the qualified immutable DLL");

        var cache = RepoFile(root, ".workflow/local/nuget-rs168/packages");
        var commands = new List<object>();
        // A disposable DWF checkout has no prebuilt F1 executable.
        // Restore/build the existing F1 tool from the exact private feed once;
        // both subsequent inventory observations then run that same binary.
        commands.Add(Run(root, cache, "f1-tool-restore", "dotnet", [
            "restore", F1Project, "--source", feed, "--packages", cache,
            "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "f1-tool-build", "dotnet", [
            "build", F1Project, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var before = Run(root, cache, "f1-inventory-before", "dotnet", [
            "run", "--project", F1Project, "-c", "Release",
            "--no-build", "--no-restore", "--", "--inspect", "."
        ]);
        commands.Add(before.Receipt);
        using var beforeDoc = JsonDocument.Parse(before.Output.Trim());
        var inspected = beforeDoc.RootElement;
        Need(Text(inspected, "status") == "f1-source-and-feed-inventory-qualified-not-published" &&
             Text(inspected, "repositoryTool") == "Dw.Tools.NuGet" &&
             Text(inspected, "repositoryPackageVersion") == "20260.20261026.1010" &&
             Text(inspected, "loadedLibrarySha256") == CoreDllSha &&
             inspected.GetProperty("packageCount").GetInt32() == 13 &&
             !inspected.GetProperty("feedPublished").GetBoolean(),
             "the prior F1 bounded inventory and shared library authority drifted");
        var numerics = inspected.GetProperty("packages").EnumerateArray()
            .Single(x => Text(x, "packageId") == PackageId);
        Need(Text(numerics, "historicalSourceVersion") == SourceVersion &&
             Text(numerics, "effectivePackageVersion") == SourceVersion &&
             Text(numerics, "effectiveTargetFramework") == "net10.0" &&
             Text(numerics, "effectivePackable") == "true" &&
             numerics.GetProperty("projectReferences").EnumerateArray()
                 .Select(x => x.GetString()).SequenceEqual([BaseId]) &&
             numerics.GetProperty("externalPackageReferences").GetArrayLength() == 0,
             "F1 evaluated Numerics source package or its exact graph changed");
        var newVersion = Text(numerics, "candidateVersionPreview");
        var versionParts = newVersion.Split('.');
        var today = DateOnly.FromDateTime(DateTime.Now);
        Need(today.Year == 2026 && versionParts.Length == 3 &&
             versionParts[0] == "20260" &&
             versionParts[1] == today.ToString(
                 "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) &&
             int.TryParse(versionParts[2], out var counter) &&
             counter is >= 1001 and <= 1999 &&
             !numerics.GetProperty("candidateIsReserved").GetBoolean(),
             "F1 must propose an unreserved current-day calendar NuGet version");

        var work = RepoFile(root, "build/artifacts/rs168-numerics-candidate");
        var producer = Path.Combine(work, "producer");
        var packageFolder = Path.Combine(work, "candidate");
        var consumer = Path.Combine(work, "consumer");
        Need(!Directory.Exists(producer) && !Directory.Exists(consumer),
             "the run-owned Numerics candidate workspace already exists");
        Directory.CreateDirectory(producer);
        Need(!File.Exists(Path.Combine(work, "candidate", PackageId + "." + newVersion + ".nupkg")),
             "an original Numerics candidate may never be overwritten");
        Directory.CreateDirectory(packageFolder);
        Directory.CreateDirectory(consumer);

        var generated = Path.Combine(producer, "numerics-candidate.csproj");
        var producerConfig = Path.Combine(producer, "NuGet.Config");
        var projection = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", Property("RootNamespace")),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", SourceVersion),
                    new XElement("PackageVersion", newVersion),
                    new XElement("Authors", Property("Authors")),
                    new XElement("Description", Property("Description")),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    sources.Select(source => new XElement("Compile",
                        new XAttribute("Include", source),
                        new XAttribute("Link", Path.GetFileName(source)))),
                    new XElement("PackageReference",
                        new XAttribute("Include", BaseId),
                        new XAttribute("Version", "[" + BaseVersion + "]")))));
        File.WriteAllText(generated, projection.ToString(SaveOptions.DisableFormatting) + "\n");
        File.WriteAllText(producerConfig, Sources(("published", feed)));
        commands.Add(Run(root, cache, "numerics-linked-restore", "dotnet", [
            "restore", generated, "--configfile", producerConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "numerics-linked-build", "dotnet", [
            "build", generated, "-c", "Release", "--no-restore", "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "numerics-candidate-pack", "dotnet", [
            "pack", generated, "-c", "Release", "--no-build", "--no-restore",
            "--output", packageFolder, "-p:NuGetAudit=false"
        ]).Receipt);

        var package = Path.Combine(packageFolder, PackageId + "." + newVersion + ".nupkg");
        Need(!File.Exists(Path.Combine(feed, PackageId + "." + newVersion + ".nupkg")),
             "candidate Numerics package identity already exists in shared private feed");
        Need(File.Exists(package), "the Numerics calendar candidate archive was not created");
        var archiveSha = ShaFile(package);
        var dllSha = PackagedDllSha(package, PackageId, newVersion,
            [(BaseId, "[" + BaseVersion + "]")]);
        var reader = new DotNetLocalPackageRepository(packageFolder,
            new DotNetLocalRepositoryLimits(64, 67_108_864, 67_108_864));
        var listed = reader.List(PackageId);
        Need(listed.InspectedPackages == 1 && listed.MatchedPackages == 1 &&
             listed.Packages.Count == 1 &&
             listed.Packages[0].PackageId == PackageId &&
             listed.Packages[0].Version == newVersion &&
             listed.Packages[0].Sha256 == archiveSha,
             "the shared NuGet repository reader refused the exact Numerics candidate");

        File.Copy(RepoFile(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var consumerProject = Path.Combine(consumer, "numerics-binary-consumer.csproj");
        var consumerConfig = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(consumerProject, new XDocument(
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
                        new XAttribute("Version", "[" + newVersion + "]")))))
            .ToString(SaveOptions.DisableFormatting) + "\n");
        File.WriteAllText(consumerConfig,
            Sources(("candidate", packageFolder), ("published", feed)));
        commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", consumerProject, "--configfile", consumerConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", consumerProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var consumerTest = Run(root, cache, "consumer-binary-run", "dotnet", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        commands.Add(consumerTest.Receipt);
        Need(consumerTest.Output.Contains("MATH-NUMERICS-BINARY|PASS|15",
                 StringComparison.Ordinal) &&
             consumerTest.Output.Split("MATH-NUMERICS-CHECK|").Length - 1 == 15 &&
             consumerTest.Output.Contains("MATH-NUMERICS-CHECK|loaded-numerics-dll|PASS",
                 StringComparison.Ordinal) &&
             consumerTest.Output.Contains("MATH-NUMERICS-CHECK|lu-and-solve|PASS",
                 StringComparison.Ordinal) &&
             consumerTest.Output.Contains("MATH-NUMERICS-CHECK|approximate-ir-roundtrip|PASS",
                 StringComparison.Ordinal),
             "new source-free Numerics consumer must satisfy fifteen binary oracles");

        var after = Run(root, cache, "f1-inventory-after", "dotnet", [
            "run", "--project", F1Project, "-c", "Release",
            "--no-build", "--no-restore", "--", "--inspect", "."
        ]);
        commands.Add(after.Receipt);
        using var afterDoc = JsonDocument.Parse(after.Output.Trim());
        var observed = afterDoc.RootElement;
        Need(Text(observed, "inventorySha256") == Text(inspected, "inventorySha256") &&
             observed.GetProperty("inspectedArchives").GetInt32() ==
                 inspected.GetProperty("inspectedArchives").GetInt32() &&
             Text(observed.GetProperty("packages").EnumerateArray()
                 .Single(x => Text(x, "packageId") == PackageId),
                 "candidateVersionPreview") == newVersion &&
             CheckPhysical(baseArchive, BaseArchiveSha, 21926) &&
             CheckPhysical(quantitiesArchive, QuantitiesArchiveSha, 14747) &&
             ShaFile(package) == archiveSha &&
             ShaFile(projectPath) == sourceProjectSha &&
             sourceFingerprints.All(x => ShaFile(RepoFile(root, x.Key)) == x.Value) &&
             !File.Exists(Path.Combine(feed, PackageId + "." + newVersion + ".nupkg")) &&
             ShaFile(nugetLibrary.Location) == CoreDllSha,
             "original Numerics code, physical published base, candidate or shared feed changed");
        var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
        commands.Add(sdk.Receipt);
        Need(commands.Count == 11,
             "eleven actual F1 tool, source/consumer/feed and SDK command results must be captured");

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.numerics-dependent-nuget-candidate",
            status = "qualified-numerics-dependent-calendar-candidate-not-published",
            packageId = PackageId,
            packageVersion = newVersion,
            historicalSourceVersion = SourceVersion,
            originalProjectSha256 = sourceProjectSha,
            originalSourcesSha256 = sourceFingerprints,
            originalSourceProjectReferences = 1,
            originalThreeSourceFilesLinked = true,
            originalSourceLockSha256 = ShaFile(lockPath),
            sourceProjectUnmodified = true,
            temporaryPackagingProjection = true,
            packageDependencies = 1,
            dependencyIrId = BaseId,
            dependencyIrVersion = BaseVersion,
            dependencyIrArchiveSha256 = BaseArchiveSha,
            dependencyIrDllSha256 = BaseDllSha,
            dependencyQuantitiesId = QuantitiesId,
            dependencyQuantitiesVersion = QuantitiesVersion,
            dependencyQuantitiesArchiveSha256 = QuantitiesArchiveSha,
            dependencyQuantitiesDllSha256 = QuantitiesDllSha,
            candidateArchivePath = Path.GetRelativePath(root, package).Replace('\\', '/'),
            candidateArchiveSha256 = archiveSha,
            candidateArchiveBytes = new FileInfo(package).Length,
            candidateDllSha256 = dllSha,
            sharedNugetPackageVersion = "20260.20261026.1010",
            sharedNugetDllSha256 = CoreDllSha,
            feedSelector = "NUGET_LOCAL",
            sourceFeedInventoryUnchanged = true,
            independentPackageReference = true,
            compiledBinaryConsumerChecks = 15,
            published = false,
            versionReserved = false,
            publicLicenseSelected = false,
            installedRollbackQualified = false,
            targetFramework = "net10.0",
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            sdkVersion = sdk.Output.Trim(),
            commands
        }, Json));
    }

    private static bool CheckPhysical(string file, string sha, int bytes) =>
        File.Exists(file) &&
        (File.GetAttributes(file) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
        new FileInfo(file).Length == bytes &&
        ShaFile(file) == sha;

    private static string PackagedDllSha(
        string file, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var archive = ZipFile.OpenRead(file);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !x.FullName.Contains('\\')),
             "candidate ZIP paths must be safe and bounded");
        var nuspec = archive.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("required exact package nuspec is missing");
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" && x.Value == version),
                 "candidate id/version does not match the approved calendar identity");
            var current = xml.Descendants()
                .Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? "")).ToArray();
            Need(current.SequenceEqual(dependencies),
                 "candidate nuspec must have exactly one pinned IR dependency");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("candidate net10.0 Numerics assembly is missing");
        using var contents = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(contents));
    }

    private static string Text(JsonElement node, string name) =>
        node.GetProperty(name).GetString() ??
        throw new InvalidDataException("missing required read-only NuGet metadata " + name);

    private static string Sources(params (string Key, string Directory)[] sources) =>
        new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                sources.Select(x => new XElement("add",
                    new XAttribute("key", x.Key),
                    new XAttribute("value", x.Directory))))))
        .ToString(SaveOptions.DisableFormatting) + "\n";

    private static string RepoFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string ShaFile(string file) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));

    private static void Need(bool yes, string reason)
    {
        if (!yes) throw new InvalidDataException(reason);
    }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string executable, string[] args)
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
        foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not start bounded Numerics candidate command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Numerics candidate command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4600)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1200)]
        });
    }
}
