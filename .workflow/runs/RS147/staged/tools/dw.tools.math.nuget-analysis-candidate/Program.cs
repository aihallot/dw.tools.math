using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Dw.Tools.Math.NuGetAnalysisCandidate;

internal static class Program
{
    private const string Id = "dw.tools.math.analysis";
    private const string Version = "20260.20261104.1001";
    private const string SourceVersion = "1.1.0-preview.6";
    private const string Project = "src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj";
    private const string Fixture = "tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs";
    private const string ExistingInventory = "docs/distribution/nuget-federation-f1-qualified.json";
    private const string Prerequisite = "docs/distribution/nuget-federation-f13-ir-published-qualified.json";
    private const string Work = "build/artifacts/rs147-analysis-candidate";
    private const string FileName = Id + "." + Version + ".nupkg";
    private static readonly string[] SourceNames =
    [
        "BoundedDifferentialAnalysis64.cs", "BoundedUnivariateAnalysis64.cs",
        "CurveBoundaryAudit64.cs", "CurvePoint64.cs", "DifferentialBoundaryAudit64.cs",
        "M6W01C01Contract.cs", "UnivariateBoundaryAudit64.cs"
    ];
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] arguments)
    {
        try
        {
            Need(arguments.Length == 2 && arguments[0] == "--qualify-analysis",
                "expected --qualify-analysis <repository-root>");
            Qualify(Path.GetFullPath(arguments[1]));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-ANALYSIS-RS147-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void Qualify(string root)
    {
        using var inventory = JsonDocument.Parse(File.ReadAllText(PathFor(root, ExistingInventory)));
        using var predecessor = JsonDocument.Parse(File.ReadAllText(PathFor(root, Prerequisite)));
        var prior = predecessor.RootElement;
        Need(Text(prior, "status") ==
                 "rs146-ir-published-local-two-binaries-independently-verified" &&
             prior.GetProperty("physically_published_package_verified").GetBoolean() &&
             !prior.GetProperty("package_published_by_this_run").GetBoolean(),
             "RS146 exact private IR readback authority required");
        var rows = inventory.RootElement.GetProperty("observed").GetProperty("packages")
            .EnumerateArray().ToArray();
        Need(rows.Length == 13 && inventory.RootElement.GetProperty("package_count").GetInt32() == 13,
            "source project inventory must remain an exact thirteen-package snapshot");
        var selected = rows.Single(x => Text(x, "packageId") == Id);
        Need(Text(selected, "historicalSourceVersion") == SourceVersion &&
             Text(selected, "sourceProject") == Project &&
             Text(selected, "effectiveTargetFramework") == "net10.0" &&
             selected.GetProperty("effectivePackable").GetString() == "true" &&
             selected.GetProperty("projectReferences").GetArrayLength() == 0 &&
             selected.GetProperty("externalPackageReferences").GetArrayLength() == 0,
             "analysis must be an unchanged independent Math source project");
        var projectPath = PathFor(root, Project);
        var projectXml = XDocument.Load(projectPath, LoadOptions.None);
        string? Property(string name) => projectXml.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == Id &&
             Property("AssemblyName") == Id &&
             Property("Version") == SourceVersion &&
             Property("IsPackable") == "true" &&
             !projectXml.Descendants().Any(x =>
                 x.Name.LocalName is "ProjectReference" or "PackageReference"),
             "original analysis project identity, source version or dependencies drifted");
        var sources = Directory.EnumerateFiles(
                PathFor(root, "src/projects/dw.tools.math.analysis"), "*.cs",
                SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(sources.Select(Path.GetFileName).SequenceEqual(SourceNames,
                 StringComparer.Ordinal),
            "analysis source set changed from the seven qualified owner source files");
        var sourceHashes = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            [Project] = Sha(projectPath)
        };
        foreach (var source in sources)
        {
            var relative = Path.GetRelativePath(root, source).Replace('\\', '/');
            sourceHashes[relative] = Sha(source);
        }
        Need(File.Exists(PathFor(root, Fixture)), "thirteen-case independent fixture absent");
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("existing NUGET_LOCAL must be selected");
        Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
            "existing absolute NUGET_LOCAL required");
        Need(!File.Exists(Path.Combine(feed, FileName)),
            "original analysis candidate identity already appears in private feed");

        var work = PathFor(root, Work);
        var output = Path.Combine(work, "candidate");
        var scratch = Path.Combine(work, "offline-restore-source");
        var cache = Path.Combine(work, "private-package-cache");
        Need(!File.Exists(Path.Combine(output, FileName)),
            "an existing analysis candidate may not be overwritten or repacked");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(scratch);
        var commands = new List<object>();
        commands.Add(Run(root, cache, "source-restore", "dotnet", [
            "restore", Project, "--source", scratch, "--packages", cache,
            "--locked-mode", "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "source-build", "dotnet", [
            "build", Project, "-c", "Release", "--no-restore",
            "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "source-pack", "dotnet", [
            "pack", Project, "-c", "Release", "--no-build", "--no-restore",
            "--output", output,
            "-p:PackageVersion=" + Version, "-p:NuGetAudit=false"
        ]).Receipt);
        var candidate = Path.Combine(output, FileName);
        Need(File.Exists(candidate) &&
             (File.GetAttributes(candidate) & FileAttributes.ReparsePoint) == 0,
             "exact original analysis candidate not physically created");
        var archiveSha = Sha(candidate);
        var archiveBytes = new FileInfo(candidate).Length;
        Need(archiveBytes is > 2000 and < 67108864,
            "candidate archive length outside bounded real NuGet range");
        using var archive = ZipFile.OpenRead(candidate);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(x => x.FullName).Distinct(StringComparer.Ordinal).Count() ==
                 archive.Entries.Count &&
             archive.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "candidate NuGet ZIP members violate bounded canonical contract");
        var spec = archive.GetEntry(Id + ".nuspec") ??
            throw new InvalidDataException("candidate nuspec missing");
        using (var stream = spec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == Id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" && x.Value == Version) &&
                 !xml.Descendants().Any(x => x.Name.LocalName == "dependency"),
                 "real analysis package must have exact calendar identity and zero dependencies");
        }
        var dll = archive.GetEntry("lib/net10.0/" + Id + ".dll") ??
            throw new InvalidDataException("real net10.0 analysis assembly missing");
        string dllSha;
        using (var stream = dll.Open()) dllSha = Convert.ToHexStringLower(SHA256.HashData(stream));

        var consumer = Path.Combine(work, "independent-consumer");
        Directory.CreateDirectory(consumer);
        File.Copy(PathFor(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: true);
        var csproj = Path.Combine(consumer, "rs147-analysis-consumer.csproj");
        var content = new XDocument(new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",
                new XElement("OutputType", "Exe"),
                new XElement("TargetFramework", "net10.0"),
                new XElement("ManagePackageVersionsCentrally", "false"),
                new XElement("RestorePackagesWithLockFile", "false"),
                new XElement("EnableDefaultCompileItems", "false"),
                new XElement("IsPackable", "false")),
            new XElement("ItemGroup",
                new XElement("Compile", new XAttribute("Include", "Program.cs")),
                new XElement("PackageReference", new XAttribute("Include", Id),
                    new XAttribute("Version", "[" + Version + "]")))));
        File.WriteAllText(csproj,
            content.ToString(SaveOptions.DisableFormatting) + "\n", new UTF8Encoding(false));
        var config = new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                new XElement("add", new XAttribute("key", "candidate"),
                    new XAttribute("value", output)))));
        var configPath = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(configPath,
            config.ToString(SaveOptions.DisableFormatting) + "\n", new UTF8Encoding(false));
        commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", csproj, "--configfile", configPath,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", csproj, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var consumerRun = Run(root, cache, "consumer-binary", "dotnet", [
            "run", "--project", csproj, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        commands.Add(consumerRun.Receipt);
        Need(consumerRun.Output.Contains("MATH-ANALYSIS-BINARY|PASS|13",
                 StringComparison.Ordinal) &&
             consumerRun.Output.Split("MATH-ANALYSIS-CHECK|").Length - 1 == 13,
             "fresh source-free package consumer did not pass all thirteen oracles");
        commands.Add(Run(root, cache, "sdk", "dotnet", ["--version"]).Receipt);
        Need(commands.Count == 7 && Sha(candidate) == archiveSha,
            "seven real command receipts and unchanged candidate bytes required");
        var sdk = ((JsonElement)JsonSerializer.SerializeToElement(commands[^1]))
            .GetProperty("output_excerpt").GetString()?.Trim() ?? "";
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            kind = "dw.tools.math.analysis-immutable-local-nuget-candidate",
            status = "qualified-independent-analysis-calendar-candidate-not-published",
            packageId = Id,
            packageVersion = Version,
            sourceVersion = SourceVersion,
            originalProjectSha256 = sourceHashes[Project],
            originalSourcesSha256 = sourceHashes,
            candidateArchivePath = Work + "/candidate/" + FileName,
            candidateArchiveBytes = archiveBytes,
            candidateArchiveSha256 = archiveSha,
            candidateDllSha256 = dllSha,
            packageDependencies = 0,
            independentPackageReference = true,
            originalProjectReferencesUsed = false,
            compiledBinaryConsumerChecks = 13,
            sourceProjectUnmodified = true,
            sourceFeedUnchanged = !File.Exists(Path.Combine(feed, FileName)),
            packagePublished = false,
            versionReserved = false,
            publicLicenseSelected = false,
            installedPackageRollbackQualified = false,
            sdkVersion = sdk,
            runtimeVersion = RuntimeInformation.FrameworkDescription,
            osDescription = RuntimeInformation.OSDescription,
            processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            commandReceipts = commands
        }, Pretty));
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
        process.StartInfo.Environment["NUGET_PACKAGES"] = cache;
        process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
        process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
        var timer = Stopwatch.StartNew();
        Need(process.Start(), "could not start " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(360000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("analysis qualification timed out in " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        timer.Stop();
        if (process.ExitCode != 0)
        {
            var details = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + details[..System.Math.Min(details.Length, 3800)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = timer.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }

    private static string PathFor(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static string Text(JsonElement node, string property) =>
        node.GetProperty(property).GetString() ??
        throw new InvalidDataException("missing source fact " + property);

    private static void Need(bool okay, string problem)
    {
        if (!okay) throw new InvalidDataException(problem);
    }
}
