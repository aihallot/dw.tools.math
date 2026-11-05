using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Dw.Tools.Math.NuGetScientificCandidate;

internal static class Program
{
    private const string Id = "dw.tools.math.scientific";
    private const string Version = "20260.20261105.1001";
    private const string SourceVersion = "1.1.0-preview.14";
    private const string Project = "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
    private const string SourceFolder = "src/projects/dw.tools.math.scientific";
    private static readonly string[] SourceNames =
    [
        "BoundedExactScientific.cs",
        "ExactScientificBoundaryAudit.cs",
        "ExactScientificContracts.cs",
        "FiniteGeometry64.cs",
        "FiniteGeometryBoundaryAudit64.cs",
        "FiniteSignal64.cs",
        "FiniteSignalBoundaryAudit64.cs",
        "FiniteUniformSeries64.cs",
        "FiniteUniformSeriesBoundaryAudit64.cs",
        "M6W02C03BoundaryAggregate64.cs",
        "M6W02C03Contract.cs",
        "M6W02C04Boundary64.cs",
        "M6W02C04Contract.cs",
        "ManagedSpecialFunctions64.cs",
        "SpecialFunctionBoundaryAudit64.cs",
        "SpecialFunctionContracts64.cs",
    ];
    private const string F1 = "docs/distribution/nuget-federation-f1-qualified.json";
    private const string F19 = "docs/distribution/nuget-federation-f19-foundation-published-qualified.json";
    private const string Fixture = "tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs";
    private const string Artifact = "build/artifacts/rs153-scientific-candidate";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 2 && args[0] == "--qualify-scientific",
                "Expected --qualify-scientific <repository-root>.");
            Qualify(Path.GetFullPath(args[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-SCIENTIFIC-RS153-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Qualify(string root)
    {
        using var inventory = Load(PathFor(root, F1));
        using var previous = Load(PathFor(root, F19));
        Need(Text(previous.RootElement, "status") ==
                 "rs152-foundation-published-local-binary-independently-verified" &&
             previous.RootElement.GetProperty("physically_published_package_verified").GetBoolean() &&
             !previous.RootElement.GetProperty("package_published_by_this_run").GetBoolean(),
             "the RS149 Analysis physical feed readback must already be accepted");
        var rows = inventory.RootElement.GetProperty("observed")
            .GetProperty("packages").EnumerateArray().ToArray();
        Need(rows.Length == 13 && inventory.RootElement.GetProperty("package_count").GetInt32() == 13,
            "the original F1 source inventory must contain exactly thirteen Math packages");
        var selected = rows.Single(x => Text(x, "packageId") == Id);
        Need(Text(selected, "sourceProject") == Project &&
             Text(selected, "historicalSourceVersion") == SourceVersion &&
             Text(selected, "effectiveTargetFramework") == "net10.0" &&
             Text(selected, "effectivePackable") == "true" &&
             selected.GetProperty("projectReferences").GetArrayLength() == 0 &&
             selected.GetProperty("externalPackageReferences").GetArrayLength() == 0,
             "the selected Scientific source must have an independent dependency closure");

        var projectFile = PathFor(root, Project);
        var xml = XDocument.Load(projectFile, LoadOptions.None);
        string? Property(string name) => xml.Descendants().Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == Id &&
             Property("AssemblyName") == Id &&
             Property("RootNamespace") == "Dw.Tools.Math.Scientific" &&
             Property("Version") == SourceVersion &&
             Property("IsPackable") == "true" &&
             !xml.Descendants().Any(x =>
                 x.Name.LocalName is "ProjectReference" or "PackageReference"),
             "Scientific source project metadata, historical version or dependencies drifted");
        var sourceFiles = Directory.EnumerateFiles(PathFor(root, SourceFolder),
                "*.cs", SearchOption.TopDirectoryOnly).Order(StringComparer.Ordinal).ToArray();
        Need(sourceFiles.Select(Path.GetFileName)
                 .SequenceEqual(SourceNames, StringComparer.Ordinal),
             "the original Scientific source set must contain precisely sixteen C# files");
        var lockFile = PathFor(root, SourceFolder + "/packages.lock.json");
        using (var lockJson = Load(lockFile))
        {
            var locked = lockJson.RootElement;
            Need(locked.GetProperty("version").GetInt32() == 2 &&
                 locked.GetProperty("dependencies").GetProperty("net10.0")
                     .EnumerateObject().Count() == 0,
                 "original Scientific lockfile must contain zero dependencies");
        }
        Need(File.Exists(PathFor(root, Fixture)),
             "the original fifteen-case independent fixture is absent");
        var hashes = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            [Project] = Hash(projectFile),
            [SourceFolder + "/packages.lock.json"] = Hash(lockFile)
        };
        foreach (var file in sourceFiles)
        {
            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
            hashes.Add(relative, Hash(file));
        }

        var sharedFeed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(sharedFeed) &&
             Path.IsPathFullyQualified(sharedFeed) && Directory.Exists(sharedFeed),
             "NUGET_LOCAL must be an existing absolute directory");
        var filename = Id + "." + Version + ".nupkg";
        var sharedPackage = Path.Combine(sharedFeed, filename);
        Need(!File.Exists(sharedPackage),
             "the selected Scientific package ID/version already exists in NUGET_LOCAL");

        var work = PathFor(root, Artifact);
        var candidateFolder = Path.Combine(work, "candidate");
        var emptySource = Path.Combine(work, "offline-restore-source");
        var privateCache = Path.Combine(work, "private-package-cache");
        var candidatePath = Path.Combine(candidateFolder, filename);
        Need(!File.Exists(candidatePath),
             "an original Scientific candidate cannot be overwritten or rebuilt");
        Directory.CreateDirectory(candidateFolder);
        Directory.CreateDirectory(emptySource);

        var commands = new List<object>();
        commands.Add(Run(root, privateCache, "source-restore", "dotnet", [
            "restore", Project, "--source", emptySource,
            "--packages", privateCache, "--locked-mode",
            "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, privateCache, "source-build", "dotnet", [
            "build", Project, "-c", "Release", "--no-restore",
            "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, privateCache, "source-pack", "dotnet", [
            "pack", Project, "-c", "Release", "--no-build", "--no-restore",
            "--output", candidateFolder,
            "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);

        Need(File.Exists(candidatePath) &&
             (File.GetAttributes(candidatePath) &
                 (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0,
             "the exact compiled Scientific candidate archive is missing or redirected");
        var bytes = new FileInfo(candidatePath).Length;
        Need(bytes is > 2000 and < 67_108_864, "real nupkg length outside bounded archive limits");
        var archiveSha = Hash(candidatePath);
        string dllSha;
        using (var archive = ZipFile.OpenRead(candidatePath))
        {
            Need(archive.Entries.Count is >= 3 and <= 48 &&
                 archive.Entries.Select(e => e.FullName)
                     .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
                 archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                     !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                     !e.FullName.Contains('\\')),
                 "the candidate ZIP entries must be unique, safe and bounded");
            var nuspec = archive.GetEntry(Id + ".nuspec") ??
                throw new InvalidDataException("the original Scientific nuspec is missing");
            using (var input = nuspec.Open())
            {
                var manifest = XDocument.Load(input, LoadOptions.None);
                Need(manifest.Descendants().Any(e => e.Name.LocalName == "id" && e.Value == Id) &&
                     manifest.Descendants().Any(e => e.Name.LocalName == "version" && e.Value == Version) &&
                     !manifest.Descendants().Any(e => e.Name.LocalName == "dependency"),
                     "the generated candidate must have the exact ID/version and zero dependencies");
            }
            var dll = archive.GetEntry("lib/net10.0/" + Id + ".dll") ??
                throw new InvalidDataException("the actual net10.0 package DLL is absent");
            using var dllStream = dll.Open();
            dllSha = Convert.ToHexStringLower(SHA256.HashData(dllStream));
        }

        var consumer = Path.Combine(work, "source-free-consumer");
        Directory.CreateDirectory(consumer);
        File.Copy(PathFor(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var consumerProject = Path.Combine(consumer, "scientific-consumer.csproj");
        var consumerXml = new XDocument(new XElement("Project",
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
        File.WriteAllText(consumerProject,
            consumerXml.ToString(SaveOptions.DisableFormatting) + "\n",
            new UTF8Encoding(false));
        var configPath = Path.Combine(consumer, "NuGet.Config");
        var config = new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                new XElement("add", new XAttribute("key", "candidate"),
                    new XAttribute("value", candidateFolder)))));
        File.WriteAllText(configPath,
            config.ToString(SaveOptions.DisableFormatting) + "\n", new UTF8Encoding(false));
        commands.Add(Run(root, privateCache, "consumer-restore", "dotnet", [
            "restore", consumerProject, "--configfile", configPath,
            "--packages", privateCache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, privateCache, "consumer-build", "dotnet", [
            "build", consumerProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var executed = Run(root, privateCache, "consumer-binary", "dotnet", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        commands.Add(executed.Receipt);
        Need(executed.Output.Contains("MATH-SCIENTIFIC-BINARY|PASS|15",
                 StringComparison.Ordinal) &&
             executed.Output.Split("MATH-SCIENTIFIC-CHECK|").Length - 1 == 15,
             "the fresh source-free Scientific package consumer did not pass fifteen cases");
        var sdk = Run(root, privateCache, "sdk", "dotnet", ["--version"]);
        commands.Add(sdk.Receipt);
        Need(commands.Count == 7 && Hash(candidatePath) == archiveSha &&
             !File.Exists(sharedPackage),
             "seven real receipts and unchanged private feed/candidate required");
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.scientific-immutable-local-nuget-candidate",
            status = "qualified-independent-scientific-calendar-candidate-not-published",
            packageId = Id,
            packageVersion = Version,
            historicalSourceVersion = SourceVersion,
            sourceProjectPath = Project,
            originalSourcesSha256 = hashes,
            candidateArchivePath = Artifact + "/candidate/" + filename,
            candidateArchiveBytes = bytes,
            candidateArchiveSha256 = archiveSha,
            candidateDllSha256 = dllSha,
            packageDependencies = 0,
            independentPackageReference = true,
            originalProjectReferencesUsed = false,
            compiledBinaryConsumerChecks = 15,
            sourceProjectUnmodified = true,
            sourceFeedUnchanged = true,
            packagePublished = false,
            versionReserved = false,
            publicLicenseSelected = false,
            installedPackageRollbackQualified = false,
            sdkVersion = sdk.Output.Trim(),
            runtimeVersion = RuntimeInformation.FrameworkDescription,
            osDescription = RuntimeInformation.OSDescription,
            processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            commandReceipts = commands
        }, Pretty));
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
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "cannot start Scientific command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(360000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Scientific candidate command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostic = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostic[..System.Math.Min(diagnostic.Length, 3800)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", arguments),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }

    private static JsonDocument Load(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Text(JsonElement source, string property) =>
        source.GetProperty(property).GetString() ??
        throw new InvalidDataException("source identity field absent: " + property);

    private static string PathFor(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Hash(string file) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));

    private static void Need(bool okay, string reason)
    {
        if (!okay) throw new InvalidDataException(reason);
    }
}
