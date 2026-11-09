using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetNumericsRehearsal;

internal static class Program
{
    private const string PackageId = "dw.tools.math.numerics";
    private const string CandidateProject = "tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj";
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

        var cache = Local(".workflow/local/nuget-rs169/packages");
        var steps = new List<object>();
        steps.Add(Run(root, cache, "candidate-tool-restore", [
            "restore", CandidateProject, "--source", feed, "--packages", cache,
            "-p:NuGetAudit=false"
        ]).Receipt);
        steps.Add(Run(root, cache, "candidate-tool-build", [
            "build", CandidateProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var candidateRun = Run(root, cache, "source-rebuild", [
            "run", "--project", CandidateProject, "-c", "Release",
            "--no-build", "--no-restore", "--", "--qualify-numerics", "."
        ]);
        steps.Add(candidateRun.Receipt);
        using var generated = JsonDocument.Parse(candidateRun.Output.Trim());
        var candidate = generated.RootElement;
        var version = Text(candidate, "packageVersion");
        var candidatePath = Text(candidate, "candidateArchivePath");
        Need(Text(candidate, "status") ==
                 "qualified-numerics-dependent-calendar-candidate-not-published" &&
             Text(candidate, "packageId") == PackageId &&
             Text(candidate, "historicalSourceVersion") == "0.4.0-preview.4" &&
             candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
             candidate.GetProperty("packageDependencies").GetInt32() == 1 &&
             Text(candidate, "dependencyIrArchiveSha256") == IrSha &&
             Text(candidate, "dependencyQuantitiesArchiveSha256") == QuantitiesSha &&
             !candidate.GetProperty("published").GetBoolean() &&
             !candidate.GetProperty("versionReserved").GetBoolean() &&
             Regex.IsMatch(candidatePath,
                 @"\Abuild/artifacts/rs168-numerics-candidate/scratch/[a-f0-9]{32}/candidate/" +
                 Regex.Escape(PackageId) + @"\." + Regex.Escape(version) + @"\.nupkg\z",
                 RegexOptions.CultureInvariant),
             "reconstructed unpublished candidate or exact binary/dependency checks refused");
        var source = Local(candidatePath);
        var generatedSha = Text(candidate, "candidateArchiveSha256");
        var dllSha = Text(candidate, "candidateDllSha256");
        Need(File.Exists(source) && Sha(source) == generatedSha &&
             new FileInfo(source).Length > 2000 &&
             Regex.IsMatch(generatedSha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant) &&
             Regex.IsMatch(dllSha, "^[0-9a-f]{64}$", RegexOptions.CultureInvariant),
             "source-built candidate archive not observed with exact SHA-256");

        var rehearsalRoot = Local("build/artifacts/rs169-numerics-rehearsal/" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(rehearsalRoot), "rehearsal directory collision");
        var disposableFeed = Path.Combine(rehearsalRoot, "feed");
        var consumer = Path.Combine(rehearsalRoot, "consumer");
        Directory.CreateDirectory(disposableFeed);
        Directory.CreateDirectory(consumer);
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
        File.WriteAllText(config, new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                new XElement("add", new XAttribute("key", "rehearsal"),
                    new XAttribute("value", disposableFeed)),
                new XElement("add", new XAttribute("key", "dependencies"),
                    new XAttribute("value", feed)))))
            .ToString(SaveOptions.DisableFormatting) + "\n");
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
        Need(steps.Count == 7 && candidate.GetProperty("commands").GetArrayLength() == 11 &&
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
            candidateInnerCommands = 11,
            commands = steps,
            sharedNugetReader = "20260.20261026.1010",
            targetFramework = "net10.0",
            sdkVersion = sdk.Output.Trim(),
            originalCandidateSourcePackagePublished = false,
            nextAction = "Only the exact original RS168 F35 archive may be considered for owner-authorized private publication after a separate immutable release gate."
        };
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
