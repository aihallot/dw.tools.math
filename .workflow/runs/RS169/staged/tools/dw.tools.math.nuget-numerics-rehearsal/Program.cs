using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetNumericsRehearsal;

internal static class Program
{
    private const string PackageId = "dw.tools.math.numerics";
    private const string BinaryFixture = "tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs";
    private const string CandidateEvidence = "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json";
    private const string IrPackage = "dw.tools.math.ir.20260.20261104.1001.nupkg";
    private const string QuantitiesPackage = "dw.quantities.20260.20261102.1001.nupkg";
    private const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
    private const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string OriginalCandidateSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
    private const string OriginalCandidateDll = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";
    private const string OriginalCandidateVersion = "20260.20261109.1001";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static int Main(string[] arguments)
    {
        try
        {
            Need(arguments.Length == 2 && arguments[0] == "--rehearse",
                "Expected --rehearse <repository-root>.");
            var root = Path.GetFullPath(arguments[1]);
            Console.WriteLine(JsonSerializer.Serialize(Rehearse(root), JsonOptions));
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUMERICS-F36-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static object Rehearse(string root)
    {
        string Local(string relative) =>
            Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        Need(Directory.Exists(root) && File.Exists(Local(CandidateEvidence)),
            "F35 predecessor evidence is required");
        using var f35 = JsonDocument.Parse(File.ReadAllText(Local(CandidateEvidence)));
        using var prior = JsonDocument.Parse(File.ReadAllText(
            Local(".aura/workflow/reports/RS168/attempt-003.json")));
        var report = prior.RootElement;
        var priorCandidate = f35.RootElement.GetProperty("candidate");
        Need(Text(report, "runId") == "RS168" &&
             report.GetProperty("attempt").GetInt32() == 3 &&
             Text(report, "outcome") == "succeeded" &&
             Text(f35.RootElement, "status") ==
                 "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
             !f35.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(priorCandidate, "packageId") == PackageId &&
             Text(priorCandidate, "packageVersion") == OriginalCandidateVersion &&
             Text(priorCandidate, "candidateArchiveSha256") == OriginalCandidateSha &&
             Text(priorCandidate, "candidateDllSha256") == OriginalCandidateDll &&
             priorCandidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
             !priorCandidate.GetProperty("published").GetBoolean(),
             "original F35 qualified candidate identity or success lineage changed");

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL feed selector is missing");
        Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
            "private dependency feed must be an existing absolute directory");
        var irArchive = Path.Combine(feed, IrPackage);
        var quantitiesArchive = Path.Combine(feed, QuantitiesPackage);
        Need(File.Exists(irArchive) && Sha(irArchive) == IrSha &&
             File.Exists(quantitiesArchive) && Sha(quantitiesArchive) == QuantitiesSha,
             "exact original published IR/Quantities dependency packages changed");
        var sourceHashes = priorCandidate.GetProperty("originalSourcesSha256");
        Need(sourceHashes.EnumerateObject().Count() == 4 &&
             sourceHashes.EnumerateObject().All(item =>
                 File.Exists(Local(item.Name)) && Sha(Local(item.Name)) == item.Value.GetString()),
             "original three Numerics sources or exact lockfile changed");
        Need(File.Exists(Local(BinaryFixture)), "existing source-free binary consumer missing");
        Need(Text(priorCandidate, "originalProjectSha256") ==
                 Sha(Local("src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj")) &&
             Text(priorCandidate, "originalSourceLockSha256") ==
                 Sha(Local("src/projects/dw.tools.math.numerics/packages.lock.json")) &&
             priorCandidate.GetProperty("originalThreeSourceFilesLinked").GetBoolean() &&
             priorCandidate.GetProperty("originalSourceProjectReferences").GetInt32() == 1,
             "the qualified original Numerics source project and lockfile changed");

        // Keep the original F35 evidence authoritative. Rebuild the unchanged three
        // source files in an independent disposable producer rather than calling F35
        // again with its already-populated RS168 NuGet package cache.
        var version = OriginalCandidateVersion;
        var rehearsalRoot = Local("build/artifacts/rs169-numerics-rehearsal/" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(rehearsalRoot), "rehearsal directory collision");
        var producer = Path.Combine(rehearsalRoot, "producer");
        var candidateFolder = Path.Combine(rehearsalRoot, "candidate");
        var disposableFeed = Path.Combine(rehearsalRoot, "feed");
        var consumer = Path.Combine(rehearsalRoot, "consumer");
        var cache = Path.Combine(rehearsalRoot, "packages");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(candidateFolder);
        Directory.CreateDirectory(disposableFeed);
        Directory.CreateDirectory(consumer);
        Directory.CreateDirectory(cache);
        Need(!Directory.EnumerateFileSystemEntries(cache).Any(),
             "rehearsal NuGet package cache must start empty");
        var originalSources = sourceHashes.EnumerateObject()
            .Where(item => item.Name.EndsWith(".cs", StringComparison.Ordinal))
            .Select(item => Local(item.Name))
            .OrderBy(path => path, StringComparer.Ordinal).ToArray();
        Need(originalSources.Length == 3 &&
             originalSources.Select(Path.GetFileName).Order(StringComparer.Ordinal)
                 .SequenceEqual(new[]
                 {
                     "BoundedLinearAlgebra64.cs",
                     "NumericalMatrices.cs",
                     "RationalBinary64Conversion.cs"
                 }, StringComparer.Ordinal),
             "exact three F35 Numerics sources are required");
        var producerProject = Path.Combine(producer, "numerics-rehearsal.csproj");
        File.WriteAllText(producerProject,
            new XDocument(new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", "Dw.Tools.Math.Numerics"),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", "0.4.0-preview.4"),
                    new XElement("PackageVersion", version),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("Description",
                        "Provider-neutral finite binary64 matrix, vector and sparse contracts with explicit budgets."),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    originalSources.Select(file => new XElement("Compile",
                        new XAttribute("Include", file),
                        new XAttribute("Link", Path.GetFileName(file)))),
                    new XElement("PackageReference",
                        new XAttribute("Include", "dw.tools.math.ir"),
                        new XAttribute("Version", "[20260.20261104.1001]")))))
                .ToString(SaveOptions.DisableFormatting) + "\n");
        var producerConfig = Path.Combine(producer, "NuGet.Config");
        File.WriteAllText(producerConfig, Sources(("dependencies", feed)));
        var steps = new List<object>();
        steps.Add(Run(root, cache, "producer-restore", [
            "restore", producerProject, "--configfile", producerConfig,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        steps.Add(Run(root, cache, "producer-build", [
            "build", producerProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        steps.Add(Run(root, cache, "candidate-pack", [
            "pack", producerProject, "-c", "Release",
            "--no-build", "--no-restore", "--output", candidateFolder,
            "-p:NuGetAudit=false"
        ]).Receipt);
        var source = Path.Combine(candidateFolder,
            PackageId + "." + version + ".nupkg");
        Need(File.Exists(source), "newly built Numerics archive is missing");
        var generatedSha = Sha(source);
        var dllSha = PackagedDllSha(source, PackageId, version);
        Need(new FileInfo(source).Length > 2000 &&
             Regex.IsMatch(generatedSha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) &&
             Regex.IsMatch(dllSha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) &&
             !File.Exists(Path.Combine(feed, PackageId + "." + version + ".nupkg")),
             "rehearsal archive identity, metadata or original shared feed changed");
        var repository = new DotNetLocalPackageRepository(disposableFeed,
            new DotNetLocalRepositoryLimits(32, 67_108_864, 67_108_864));
        var prepared = repository.PreparePublication(source, generatedSha);
        Need(prepared.Receipt.PackageId == PackageId &&
             prepared.Receipt.Version == version &&
             prepared.Receipt.Sha256 == generatedSha,
             "disposable publication preflight changed candidate identity");
        var published = repository.Publish(prepared);
        var inventory = repository.List(PackageId);
        Need(published.Sha256 == generatedSha &&
             File.Exists(published.Path) && Sha(published.Path) == generatedSha &&
             inventory.InspectedPackages == 1 &&
             inventory.MatchedPackages == 1 &&
             inventory.Packages.Count == 1 &&
             inventory.Packages[0].PackageId == PackageId &&
             inventory.Packages[0].Version == version &&
             inventory.Packages[0].Sha256 == generatedSha,
             "disposable feed readback differs from exact candidate archive");

        RequireRefusal("collision", () =>
            repository.PreparePublication(source, generatedSha));
        RequireRefusal("digest_mismatch", () =>
            repository.PreparePublication(source, new string('0', 64)));
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            var refused = false;
            try { repository.List(token: cancellation.Token); }
            catch (OperationCanceledException) { refused = true; }
            Need(refused, "cancelled repository inspection must refuse");
        }

        File.Copy(Local(BinaryFixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var consumerProject = Path.Combine(consumer, "numerics-rehearsal-consumer.csproj");
        File.WriteAllText(consumerProject,
            new XDocument(new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Exe"),
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("EnableDefaultCompileItems", "false"),
                    new XElement("RestorePackagesWithLockFile", "false")),
                new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Include", "Program.cs")),
                    new XElement("PackageReference",
                        new XAttribute("Include", PackageId),
                        new XAttribute("Version", "[" + version + "]")))))
                .ToString(SaveOptions.DisableFormatting) + "\n");
        var config = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(config,
            Sources(("rehearsal", disposableFeed), ("dependencies", feed)));
        steps.Add(Run(root, cache, "consumer-restore", [
            "restore", consumerProject, "--configfile", config,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        steps.Add(Run(root, cache, "consumer-build", [
            "build", consumerProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var test = Run(root, cache, "consumer-binary", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        steps.Add(test.Receipt);
        Need(test.Output.Contains("MATH-NUMERICS-BINARY|PASS|15",
                 StringComparison.Ordinal) &&
             test.Output.Split("MATH-NUMERICS-CHECK|").Length - 1 == 15 &&
             test.Output.Contains("MATH-NUMERICS-CHECK|loaded-numerics-dll|PASS",
                 StringComparison.Ordinal) &&
             test.Output.Contains("MATH-NUMERICS-CHECK|lu-and-solve|PASS",
                 StringComparison.Ordinal) &&
             test.Output.Contains("MATH-NUMERICS-CHECK|approximate-ir-roundtrip|PASS",
                 StringComparison.Ordinal),
             "disposable-feed binary consumer failed fifteen independent probes");
        var sdk = Run(root, cache, "sdk", ["--version"]);
        steps.Add(sdk.Receipt);
        Need(steps.Count == 7 && priorCandidate.GetProperty("commands").GetArrayLength() == 11 &&
             Sha(irArchive) == IrSha && Sha(quantitiesArchive) == QuantitiesSha &&
             Sha(source) == generatedSha && Sha(published.Path) == generatedSha &&
             !File.Exists(Path.Combine(feed, PackageId + "." + version + ".nupkg")),
             "eleven candidate receipts, seven rehearsal commands or unchanged feeds required");

        return new
        {
            schemaVersion = 1,
            status = "qualified-numerics-immutable-disposable-rehearsal-unpublished",
            packageId = PackageId,
            packageVersion = version,
            sourceVersion = "0.4.0-preview.4",
            originalF35Version = OriginalCandidateVersion,
            originalF35ArchiveSha256 = OriginalCandidateSha,
            originalF35DllSha256 = OriginalCandidateDll,
            rehearsalArchiveSha256 = generatedSha,
            rehearsalDllSha256 = dllSha,
            rehearsalArchiveBytes = new FileInfo(published.Path).Length,
            reconstructionIsOriginalArchiveProof = generatedSha == OriginalCandidateSha,
            sourceReconstructedFromOriginalFiles = true,
            disposableFeedPublished = true,
            sharedFeedPublished = false,
            releaseAuthorized = false,
            immutableCollisionRefused = true,
            falsifiedDigestRefused = true,
            cancellationRefused = true,
            independentCompiledBinaryChecks = 15,
            directDependencyCount = 1,
            directDependencyId = "dw.tools.math.ir",
            directDependencyVersion = "20260.20261104.1001",
            directDependencyArchiveSha256 = IrSha,
            transitiveDependencyId = "dw.quantities",
            transitiveDependencyVersion = "20260.20261102.1001",
            transitiveDependencyArchiveSha256 = QuantitiesSha,
            candidateInnerCommands = priorCandidate.GetProperty("commands").GetArrayLength(),
            commands = steps,
            sharedNugetReader = "20260.20261026.1010",
            targetFramework = "net10.0",
            sdkVersion = sdk.Output.Trim(),
            originalCandidateSourcePackagePublished = false,
            nextAction = "Only the exact original RS168 F35 archive may be considered for owner-authorized private publication after a separate immutable release gate."
        };
    }


    private static string Sources(params (string Key, string Directory)[] feeds) =>
        new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                feeds.Select(feed => new XElement("add",
                    new XAttribute("key", feed.Key),
                    new XAttribute("value", feed.Directory))))))
        .ToString(SaveOptions.DisableFormatting) + "\n";

    private static string PackagedDllSha(string archivePath, string id, string version)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "Numerics candidate archive entry paths must be bounded and safe");
        var nuspec = archive.GetEntry(id + ".nuspec")
            ?? throw new InvalidDataException("candidate nuspec missing");
        using (var input = nuspec.Open())
        {
            var manifest = XDocument.Load(input, LoadOptions.None);
            Need(manifest.Descendants().Any(x => x.Name.LocalName == "id" &&
                     x.Value == id) &&
                 manifest.Descendants().Any(x => x.Name.LocalName == "version" &&
                     x.Value == version),
                 "rehearsal nuspec package identity differs");
            var dependencies = manifest.Descendants()
                .Where(x => x.Name.LocalName == "dependency")
                .Select(x => (
                    Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? "")).ToArray();
            Need(dependencies.SequenceEqual(
                     new[] { (Id: "dw.tools.math.ir",
                             Version: "[20260.20261104.1001]") }),
                 "rehearsal nuspec must declare only exact published IR");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll")
            ?? throw new InvalidDataException("Numerics net10.0 DLL missing");
        using var stream = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }

    private static void RequireRefusal(string code, Action action)
    {
        try { action(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == code) { return; }
        throw new InvalidDataException("Expected shared NuGet repository refusal: " + code);
    }

    private static string Text(JsonElement element, string name) =>
        element.GetProperty(name).GetString()
        ?? throw new InvalidDataException("Missing exact JSON string: " + name);

    private static string Sha(string file) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));

    private static void Need(bool success, string reason)
    {
        if (!success) throw new InvalidDataException(reason);
    }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string[] arguments)
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
        foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
        Need(process.Start(), "unable to start real NuGet rehearsal command " + id);
        var clock = Stopwatch.StartNew();
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(540000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("NuGet rehearsal timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        clock.Stop();
        if (process.ExitCode != 0)
        {
            var error = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode + "\n" +
                error[..System.Math.Min(error.Length, 3000)]);
        }
        return (output, new
        {
            id,
            exit_code = process.ExitCode,
            command = "dotnet " + string.Join(" ", arguments),
            elapsed_ms = clock.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 900)]
        });
    }
}
