using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Dw.Tools.Math.NuGetNumericsOriginalConsumer;

internal static class Program
{
    private const string Id = "dw.tools.math.numerics";
    private const string Version = "20260.20261109.1001";
    private const string OriginalArchiveSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
    private const string OriginalDllSha = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";
    private const string IrId = "dw.tools.math.ir";
    private const string IrVersion = "20260.20261104.1001";
    private const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
    private const string QuantityId = "dw.quantities";
    private const string QuantityVersion = "20260.20261102.1001";
    private const string QuantitySha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string F35 = "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json";
    private const string F37 = "docs/distribution/nuget-federation-f37-numerics-release-qualified.json";
    private const string Fixture = "tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs";
    private const string SourceProject = "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 2 && args[0] == "--verify-original",
                "Expected only --verify-original <repository-root>.");
            Console.WriteLine(JsonSerializer.Serialize(Verify(Path.GetFullPath(args[1])), Pretty));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUMERICS-F38-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static object Verify(string root)
    {
        string Local(string path) =>
            Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
        using var f35 = Read(Local(F35));
        using var f37 = Read(Local(F37));
        using var report = Read(Local(".aura/workflow/reports/RS170/attempt-001.json"));
        var candidate = f35.RootElement.GetProperty("candidate");
        var release = f37.RootElement.GetProperty("controller");
        var previous = report.RootElement;
        Need(Text(previous, "runId") == "RS170" &&
             previous.GetProperty("attempt").GetInt32() == 1 &&
             Text(previous, "outcome") == "succeeded" &&
             Text(previous.GetProperty("git"), "commitMessage") ==
                 "test: qualify Numerics immutable private release controller" &&
             Text(f35.RootElement, "status") ==
                 "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
             !f35.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(f37.RootElement, "status") ==
                 "rs170-numerics-immutable-release-controller-qualified-unpublished" &&
             !f37.RootElement.GetProperty("package_published").GetBoolean() &&
             !f37.RootElement.GetProperty("release_authorized").GetBoolean() &&
             release.GetProperty("originalCandidatePhysicallyVerified").GetBoolean() &&
             Text(release, "originalF35ArchiveSha256") == OriginalArchiveSha &&
             Text(candidate, "packageId") == Id &&
             Text(candidate, "packageVersion") == Version &&
             Text(candidate, "candidateArchiveSha256") == OriginalArchiveSha &&
             Text(candidate, "candidateDllSha256") == OriginalDllSha &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == 18313 &&
             candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15,
             "F35 original and RS170 controller authority changed");
        Need(Sha(Local(SourceProject)) == Text(candidate, "originalProjectSha256") &&
             Sha(Local("src/projects/dw.tools.math.numerics/packages.lock.json")) ==
                 Text(candidate, "originalSourceLockSha256"),
             "original project or lockfile fingerprint changed");
        var sourceHashes = candidate.GetProperty("originalSourcesSha256").EnumerateObject().ToArray();
        Need(sourceHashes.Length == 4 &&
             sourceHashes.All(item => File.Exists(Local(item.Name)) &&
                 Sha(Local(item.Name)) == item.Value.GetString()),
             "three original Numerics source SHA-256 identities changed");

        var relative = Text(candidate, "candidateArchivePath").Replace('\\', '/');
        Need(Regex.IsMatch(relative,
                @"\Abuild/artifacts/rs168-numerics-candidate/scratch/[0-9a-f]{32}/candidate/" +
                Regex.Escape(Id) + @"\." + Regex.Escape(Version) + @"\.nupkg\z",
                RegexOptions.CultureInvariant),
             "original immutable F35 archive path does not match recorded source");
        var original = Local(relative);
        Need(File.Exists(original) &&
             (File.GetAttributes(original) &
                 (FileAttributes.ReparsePoint | FileAttributes.Directory)) == 0 &&
             new FileInfo(original).Length == 18313 &&
             Sha(original) == OriginalArchiveSha &&
             PackageDllSha(original) == OriginalDllSha,
             "original immutable F35 archive, Nuspec or packaged DLL is not exact");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("read-only published dependency feed is missing");
        Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
             "private IR/Quantities feed must be a valid absolute directory");
        var irArchive = Path.Combine(feed, IrId + "." + IrVersion + ".nupkg");
        var quantitiesArchive = Path.Combine(feed, QuantityId + "." + QuantityVersion + ".nupkg");
        Need(File.Exists(irArchive) && Sha(irArchive) == IrSha &&
             File.Exists(quantitiesArchive) && Sha(quantitiesArchive) == QuantitySha &&
             !File.Exists(Path.Combine(feed, Id + "." + Version + ".nupkg")),
             "published dependency packages changed or Numerics already released");

        var scratch = Local("build/artifacts/rs171-numerics-original/" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(scratch), "original verification scratch already exists");
        var disposable = Path.Combine(scratch, "feed");
        var consumer = Path.Combine(scratch, "consumer");
        var cache = Path.Combine(scratch, "cache");
        Directory.CreateDirectory(disposable);
        Directory.CreateDirectory(consumer);
        Directory.CreateDirectory(cache);
        Need(!Directory.EnumerateFileSystemEntries(cache).Any(),
             "original-consumer NuGet cache must be empty before restore");
        var copy = Path.Combine(disposable, Id + "." + Version + ".nupkg");
        Need(!File.Exists(copy), "disposable original archive cannot be overwritten");
        File.Copy(original, copy, overwrite: false);
        Need(Sha(copy) == OriginalArchiveSha &&
             new FileInfo(copy).Length == 18313 &&
             PackageDllSha(copy) == OriginalDllSha,
             "disposable original package bytes do not match F35 source");
        Need(File.Exists(Local(Fixture)), "qualified Numerics compiled binary fixture missing");
        File.Copy(Local(Fixture), Path.Combine(consumer, "Program.cs"), overwrite: false);
        var project = Path.Combine(consumer, "numerics-original-binary.csproj");
        File.WriteAllText(project,
            new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Exe"),
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("IsPackable", "false"),
                    new XElement("EnableDefaultCompileItems", "false"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false")),
                new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Include", "Program.cs")),
                    new XElement("PackageReference",
                        new XAttribute("Include", Id),
                        new XAttribute("Version", "[" + Version + "]")))))
            .ToString(SaveOptions.DisableFormatting) + "\n");
        var config = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(config,
            new XDocument(new XElement("configuration",
                new XElement("packageSources", new XElement("clear"),
                    new XElement("add", new XAttribute("key", "original"),
                        new XAttribute("value", disposable)),
                    new XElement("add", new XAttribute("key", "dependencies"),
                        new XAttribute("value", feed)))))
            .ToString(SaveOptions.DisableFormatting) + "\n");

        var commands = new List<object>();
        commands.Add(Run(root, cache, "consumer-restore", [
            "restore", project, "--configfile", config,
            "--packages", cache, "-p:NuGetAudit=false"]).Receipt);
        commands.Add(Run(root, cache, "consumer-build", [
            "build", project, "-c", "Release",
            "--no-restore", "-p:NuGetAudit=false"]).Receipt);
        var test = Run(root, cache, "consumer-binary", [
            "run", "--project", project, "-c", "Release",
            "--no-build", "--no-restore", "--", OriginalDllSha]);
        commands.Add(test.Receipt);
        Need(test.Output.Contains("MATH-NUMERICS-BINARY|PASS|15", StringComparison.Ordinal) &&
             test.Output.Split("MATH-NUMERICS-CHECK|").Length - 1 == 15 &&
             test.Output.Contains("MATH-NUMERICS-CHECK|loaded-numerics-dll|PASS",
                 StringComparison.Ordinal) &&
             test.Output.Contains("MATH-NUMERICS-CHECK|lu-and-solve|PASS",
                 StringComparison.Ordinal) &&
             test.Output.Contains("MATH-NUMERICS-CHECK|approximate-ir-roundtrip|PASS",
                 StringComparison.Ordinal),
             "15 compiled package-only probes must load original F35 Numerics bytes");
        var sdk = Run(root, cache, "sdk", ["--version"]);
        commands.Add(sdk.Receipt);
        Need(commands.Count == 4 &&
             Sha(original) == OriginalArchiveSha &&
             Sha(copy) == OriginalArchiveSha &&
             Sha(irArchive) == IrSha &&
             Sha(quantitiesArchive) == QuantitySha &&
             !File.Exists(Path.Combine(feed, Id + "." + Version + ".nupkg")),
             "original, private dependency or shared-feed identity changed");

        return new
        {
            schemaVersion = 1,
            status = "qualified-original-f35-numerics-consumer-unpublished",
            packageId = Id,
            packageVersion = Version,
            originalArchiveSha256 = OriginalArchiveSha,
            originalArchiveBytes = 18313,
            originalDllSha256 = OriginalDllSha,
            disposableArchiveSha256 = Sha(copy),
            originalArchivePhysicallyVerified = true,
            archiveRebuilt = false,
            rehearsalArchiveSubstituted = false,
            freshNuGetPackagesCache = true,
            compiledBinaryChecks = 15,
            privateFeedPublished = false,
            releaseAuthorized = false,
            directDependencyId = IrId,
            directDependencyVersion = IrVersion,
            directDependencyArchiveSha256 = IrSha,
            transitiveDependencyId = QuantityId,
            transitiveDependencyVersion = QuantityVersion,
            transitiveDependencyArchiveSha256 = QuantitySha,
            commandReceipts = commands,
            sdkVersion = sdk.Output.Trim(),
            nextAction = "Only an explicit owner authorization may allow publication of the exact original F35 archive to NUGET_LOCAL; follow with independent physical readback."
        };
    }

    private static string PackageDllSha(string path)
    {
        using var archive = ZipFile.OpenRead(path);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "F35 archive ZIP members must be bounded and canonical");
        var nuspec = archive.GetEntry(Id + ".nuspec")
            ?? throw new InvalidDataException("original Numerics Nuspec missing");
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == Id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" &&
                     x.Value == Version),
                 "original NuGet ID/version does not match F35");
            var deps = xml.Descendants().Where(x => x.Name.LocalName == "dependency")
                .Select(x => (
                    Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? "")).ToArray();
            Need(deps.SequenceEqual(
                    new[] { (Id: IrId, Version: "[" + IrVersion + "]") }),
                 "original NuGet package must have one exact IR dependency");
        }
        var dll = archive.GetEntry("lib/net10.0/" + Id + ".dll")
            ?? throw new InvalidDataException("original net10.0 Numerics DLL missing");
        using var bytes = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(bytes));
    }

    private static JsonDocument Read(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));
    private static string Text(JsonElement element, string key) =>
        element.GetProperty(key).GetString()
            ?? throw new InvalidDataException("required metadata missing: " + key);
    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    private static void Need(bool result, string why)
    {
        if (!result) throw new InvalidDataException(why);
    }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
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
        Need(process.Start(), "failed to start original Numerics command " + id);
        var clock = Stopwatch.StartNew();
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(360000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("original Numerics consumer command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        clock.Stop();
        if (process.ExitCode != 0)
        {
            var message = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
                message[..System.Math.Min(message.Length, 3500)]);
        }
        return (output, new
        {
            id,
            exit_code = process.ExitCode,
            command = "dotnet " + string.Join(" ", args),
            elapsed_ms = clock.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1100)]
        });
    }
}
