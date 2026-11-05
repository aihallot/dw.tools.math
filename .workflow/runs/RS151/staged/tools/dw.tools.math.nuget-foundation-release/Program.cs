using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetFoundationRelease;

internal static class Program
{
    private const string Id = "dw.tools.math.foundation";
    private const string Version = "20260.20261105.1001";
    private const string HistoricalVersion = "0.1.0-preview.1";
    private const string CandidateSha = "0fc5499e40593d2e2c01b5d8228c3f731d7df9d54050200906e53faa33569f94";
    private const string CandidateDllSha = "8dd3af5cba6a277703307a972552e177feb22f3da5f42e220a01ce1ed64964d2";
    private const string PublisherDllSha = "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string OriginalProject = "src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj";
    private const string OriginalDirectory = "src/projects/dw.tools.math.foundation";
    private const string F17 = "docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json";
    private const string F16 = "docs/distribution/nuget-federation-f16-analysis-published-qualified.json";
    private const string Fixture = "tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs";
    private const string CandidatePath =
        "build/artifacts/rs150-foundation-candidate/candidate/dw.tools.math.foundation.20260.20261105.1001.nupkg";
    private const string Rs151 = ".aura/workflow/reports/RS151/attempt-001.json";
    private static readonly string[] SourceFiles = ["FoundationContract.cs"];
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 1 &&
                 args[0] is "--rehearse" or "--publish" or "--verify",
                 "Expected --rehearse, --publish or --verify.");
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            var feed = ExistingFeed();
            ValidateSource(root);
            var publisher = typeof(DotNetLocalPackageRepository).Assembly;
            Need(publisher.GetName().Name?.Equals("dw.tools.nuget",
                     StringComparison.OrdinalIgnoreCase) == true &&
                 Hash(publisher.Location) == PublisherDllSha,
                 "pinned shared publisher DLL identity changed");
            switch (args[0])
            {
                case "--rehearse": Rehearse(root, feed); break;
                case "--publish": PublishOriginal(root, feed); break;
                case "--verify": VerifyOriginal(root, feed); break;
            }
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-FOUNDATION-RELEASE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void ValidateSource(string root)
    {
        using var evidence = Load(PathFor(root, F17));
        using var prior = Load(PathFor(root, F16));
        var owner = evidence.RootElement;
        var candidate = owner.GetProperty("candidate");
        Need(Text(owner, "status") ==
                 "rs150-independent-foundation-calendar-nuget-candidate-qualified-unpublished" &&
             !owner.GetProperty("package_published").GetBoolean() &&
             Text(candidate, "status") ==
                 "qualified-independent-foundation-calendar-candidate-not-published" &&
             Text(candidate, "packageId") == Id &&
             Text(candidate, "packageVersion") == Version &&
             Text(candidate, "historicalSourceVersion") == HistoricalVersion &&
             Text(candidate, "candidateArchivePath") == CandidatePath &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == 3863 &&
             Text(candidate, "candidateArchiveSha256") == CandidateSha &&
             Text(candidate, "candidateDllSha256") == CandidateDllSha &&
             candidate.GetProperty("packageDependencies").GetInt32() == 0 &&
             candidate.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 5 &&
             candidate.GetProperty("independentPackageReference").GetBoolean() &&
             !candidate.GetProperty("packagePublished").GetBoolean(),
             "RS150 source-free candidate authority has drifted");
        Need(Text(prior.RootElement, "status") ==
                 "rs149-analysis-published-local-binary-independently-verified" &&
             prior.RootElement.GetProperty("physically_published_package_verified").GetBoolean(),
             "previous physical publication readback was not accepted");
        var originalProjectPath = PathFor(root, OriginalProject);
        var project = XDocument.Load(originalProjectPath, LoadOptions.None);
        string? Property(string name) => project.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == Id && Property("AssemblyName") == Id &&
             Property("Version") == HistoricalVersion &&
             Property("IsPackable") == "true" &&
             !project.Descendants().Any(e =>
                 e.Name.LocalName is "ProjectReference" or "PackageReference"),
             "original Foundation csproj identity or independent dependency closure changed");
        var expectedSources = candidate.GetProperty("originalSourcesSha256");
        Need(expectedSources.EnumerateObject().Count() == 2 &&
             Hash(originalProjectPath) == Text(expectedSources, OriginalProject),
             "original csproj SHA or two-source fingerprint set changed");
        var actualSources = Directory.EnumerateFiles(
                PathFor(root, OriginalDirectory), "*.cs", SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(actualSources.Select(Path.GetFileName)
                 .SequenceEqual(SourceFiles, StringComparer.Ordinal),
             "exact one original Foundation C# file must be preserved");
        foreach (var file in actualSources)
        {
            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
            Need(Hash(file) == Text(expectedSources, relative),
                 "original Foundation source fingerprint drifted: " + relative);
        }
        Need(File.Exists(PathFor(root, Fixture)),
             "original five-oracle fixture unavailable");
    }

    private static void Rehearse(string root, string feed)
    {
        var work = PathFor(root, "build/artifacts/rs151-foundation-rehearsal-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "private rehearsal workspace collision");
        var producer = Path.Combine(work, "producer");
        var archiveDirectory = Path.Combine(work, "candidate");
        var disposableFeed = Path.Combine(work, "disposable-feed");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(archiveDirectory);
        Directory.CreateDirectory(disposableFeed);
        try
        {
            var cache = Path.Combine(work, "private-package-cache");
            var project = Path.Combine(producer, "foundation-release-rehearsal.csproj");
            var configPath = Path.Combine(producer, "NuGet.Config");
            File.WriteAllText(project, ProducerProject(root), new UTF8Encoding(false));
            File.WriteAllText(configPath, FeedConfig(), new UTF8Encoding(false));
            var commands = new List<object>();
            commands.Add(Run(root, cache, "source-restore", "dotnet", [
                "restore", project, "--configfile", configPath,
                "--packages", cache, "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "source-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "source-pack", "dotnet", [
                "pack", project, "-c", "Release", "--no-build",
                "--no-restore", "--output", archiveDirectory,
                "-p:NuGetAudit=false"
            ]).Receipt);
            var built = Path.Combine(archiveDirectory, ArchiveName());
            Need(File.Exists(built), "compiled real rehearsal archive absent");
            var archiveSha = Hash(built);
            var dllSha = PackageDllHash(built);
            var repository = new DotNetLocalPackageRepository(disposableFeed,
                new DotNetLocalRepositoryLimits(256, 67_108_864, 67_108_864));
            var prepared = repository.PreparePublication(
                built, archiveSha, CancellationToken.None, ignoreSubdirectories: true);
            Need(!File.Exists(prepared.Receipt.Path),
                "publication preparation must not write the archive");
            var published = repository.Publish(prepared, CancellationToken.None);
            Need(published.PackageId == Id && published.Version == Version &&
                 published.Sha256 == archiveSha &&
                 Hash(published.Path) == archiveSha &&
                 PackageDllHash(published.Path) == dllSha,
                 "immutable disposable publication or readback bytes differ");
            var inventory = repository.List(Id);
            Need(inventory.InspectedPackages == 1 &&
                 inventory.MatchedPackages == 1 &&
                 inventory.Packages.Single().Sha256 == archiveSha,
                 "real shared NuGet reader rejected the disposable package");
            ExpectRefusal(() => repository.PreparePublication(
                built, archiveSha, CancellationToken.None, ignoreSubdirectories: true),
                "collision");
            ExpectRefusal(() => repository.PreparePublication(
                built, new string('0', 64), CancellationToken.None,
                ignoreSubdirectories: true), "digest_mismatch");
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            var cancellationRefused = false;
            try
            {
                _ = repository.PreparePublication(
                    built, archiveSha, cancellation.Token, ignoreSubdirectories: true);
            }
            catch (OperationCanceledException) { cancellationRefused = true; }
            Need(cancellationRefused && Hash(published.Path) == archiveSha,
                "cancellation must refuse without modifying published bytes");
            commands.AddRange(RunConsumer(root, work, disposableFeed, dllSha));
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 7 && Hash(published.Path) == archiveSha &&
                 !File.Exists(Path.Combine(feed, ArchiveName())),
                 "seven command receipts, immutable disposable bytes and no shared publication required");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.foundation-private-immutable-publisher-rehearsal",
                status = "foundation-immutable-disposable-publisher-qualified",
                packageId = Id,
                packageVersion = Version,
                historicalSourceVersion = HistoricalVersion,
                originalCandidateArchiveSha256 = CandidateSha,
                originalCandidateDllSha256 = CandidateDllSha,
                rehearsalArchiveSha256 = archiveSha,
                rehearsalDllSha256 = dllSha,
                packageDependencies = 0,
                publisher = "Dw.Tools.NuGet/[20260.20261026.1010]",
                publisherDllSha256 = PublisherDllSha,
                disposableFeedPublished = true,
                publishedToNugetLocal = false,
                collisionRefused = true,
                alteredDigestRefused = true,
                cancelledRefused = cancellationRefused,
                consumerChecks = 5,
                originalSourcesUnchanged = true,
                originalSourceProjectVersionUnchanged = true,
                sharedFeedUnchanged = true,
                licenseSelected = false,
                rollbackQualified = false,
                sdkVersion = sdk.Output.Trim(),
                commandReceipts = commands
            }, JsonOptions));
        }
        finally
        {
            if (Directory.Exists(work)) Directory.Delete(work, recursive: true);
        }
    }

    private static void PublishOriginal(string root, string feed)
    {
        RequireCleanOwnerAndRs151(root);
        var original = PathFor(root, CandidatePath);
        Need(IsExactPhysical(original, 3863, CandidateSha) &&
             PackageDllHash(original) == CandidateDllSha,
             "only the exact original 3,863-byte RS150 Foundation candidate may be published");
        Need(DateOnly.FromDateTime(DateTime.Now) >= new DateOnly(2026, 11, 5),
             "original candidate creation date cannot be in the future");
        var receiptDirectory = PathFor(root, "build/artifacts/rs151-foundation-release/receipts");
        var receiptPath = Path.Combine(receiptDirectory, ArchiveName() + ".json");
        Need(!File.Exists(receiptPath), "a prior operator publication receipt cannot be overwritten");
        var repository = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = repository.PreparePublication(
            original, CandidateSha, CancellationToken.None, ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == Id &&
             prepared.Receipt.Version == Version &&
             prepared.Receipt.Sha256 == CandidateSha,
             "shared publisher selected different Foundation archive bytes");
        var published = repository.Publish(prepared, CancellationToken.None);
        Need(IsExactPhysical(published.Path, 3863, CandidateSha) &&
             PackageDllHash(published.Path) == CandidateDllSha,
             "physical shared-feed published Foundation archive differs");
        Directory.CreateDirectory(receiptDirectory);
        using (var stream = new FileStream(receiptPath, FileMode.CreateNew,
                   FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.foundation-private-nuget-local-publication",
                status = "published-local-awaiting-independent-dwf-verification",
                packageId = Id,
                version = Version,
                archiveSha256 = CandidateSha,
                packagedDllSha256 = CandidateDllSha,
                dependencyCount = 0,
                feedSelector = "NUGET_LOCAL",
                independentReadbackPending = true,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false
            }, JsonOptions);
            stream.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-FOUNDATION-PUBLISHED|id=" + Id + "|version=" + Version +
            "|sha256=" + CandidateSha +
            "|dependencies=0|feed=NUGET_LOCAL|verification=pending");
    }

    private static void VerifyOriginal(string root, string feed)
    {
        RequireCleanOwnerAndRs151(root);
        var archive = Path.Combine(feed, ArchiveName());
        Need(IsExactPhysical(archive, 3863, CandidateSha) &&
             PackageDllHash(archive) == CandidateDllSha,
             "physical shared-feed Foundation bytes differ from original RS150 candidate");
        var work = PathFor(root, "build/artifacts/rs151-foundation-verify-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "verification workspace collision");
        Directory.CreateDirectory(work);
        try
        {
            var commands = RunConsumer(root, work, feed, CandidateDllSha);
            Need(commands.Count == 3 &&
                 IsExactPhysical(archive, 3863, CandidateSha),
                 "original published archive drifted during fresh five-oracle consumer");
            Console.WriteLine("MATH-FOUNDATION-VERIFY|id=" + Id +
                "|version=" + Version + "|binary=5|archive=" + CandidateSha +
                "|dependencies=0|feed=NUGET_LOCAL|PASS");
        }
        finally
        {
            if (Directory.Exists(work)) Directory.Delete(work, recursive: true);
        }
    }

    private static void RequireCleanOwnerAndRs151(string root)
    {
        using var report = Load(PathFor(root, Rs151));
        var result = report.RootElement;
        Need(Text(result, "runId") == "RS151" &&
             Text(result, "outcome") == "succeeded" &&
             result.GetProperty("attempt").GetInt32() == 1 &&
             Text(result.GetProperty("git"), "commitMessage") ==
                 "test: qualify Foundation immutable NuGet publisher rehearsal",
             "successful exact RS151 publisher rehearsal required for publication");
        using var evidence = Load(PathFor(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        var rights = evidence.RootElement.GetProperty("authorization");
        Need(rights.GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "exact private source-owner rights required");
        var cache = PathFor(root, "build/artifacts/rs151-foundation-operator-check");
        var branch = Run(root, cache, "git-branch", "git",
            ["branch", "--show-current"]).Output.Trim();
        var status = Run(root, cache, "git-status", "git",
            ["status", "--porcelain", "--untracked-files=normal"]).Output.Trim();
        Need(branch == "main" && status.Length == 0,
             "explicit owner publication requires a clean main checkout");
    }

    private static string ProducerProject(string root)
    {
        var xml = new XDocument(new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",
                new XElement("AssemblyName", Id),
                new XElement("RootNamespace", "Dw.Tools.Math.Foundation"),
                new XElement("PackageId", Id),
                new XElement("Version", HistoricalVersion),
                new XElement("PackageVersion", Version),
                new XElement("Authors", "dw.tools.math"),
                new XElement("Description", "Minimal verified Math Foundation distribution contract."),
                new XElement("TargetFramework", "net10.0"),
                new XElement("ManagePackageVersionsCentrally", "false"),
                new XElement("RestorePackagesWithLockFile", "false"),
                new XElement("EnableDefaultCompileItems", "false"),
                new XElement("IsPackable", "true")),
            new XElement("ItemGroup", SourceFiles.Select(name =>
                new XElement("Compile", new XAttribute("Include",
                    PathFor(root, OriginalDirectory + "/" + name)),
                    new XAttribute("Link", name))))));
        return xml.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static List<object> RunConsumer(
        string root, string work, string feed, string expectedDllSha)
    {
        var consumer = Path.Combine(work, "isolated-consumer");
        Directory.CreateDirectory(consumer);
        File.Copy(PathFor(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var projectPath = Path.Combine(consumer, "foundation-consumer.csproj");
        var projectXml = new XDocument(new XElement("Project",
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
        File.WriteAllText(projectPath,
            projectXml.ToString(SaveOptions.DisableFormatting) + "\n",
            new UTF8Encoding(false));
        var configPath = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(configPath, FeedConfig(feed), new UTF8Encoding(false));
        var cache = Path.Combine(work, "nuget-cache");
        var commands = new List<object>();
        commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", projectPath, "--configfile", configPath,
            "--packages", cache, "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", projectPath, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var tested = Run(root, cache, "consumer-binary", "dotnet", [
            "run", "--project", projectPath, "-c", "Release",
            "--no-build", "--no-restore", "--", expectedDllSha
        ]);
        commands.Add(tested.Receipt);
        Need(tested.Output.Contains("MATH-FOUNDATION-BINARY|PASS|5",
                 StringComparison.Ordinal) &&
             tested.Output.Split("MATH-FOUNDATION-CHECK|").Length - 1 == 5,
             "fresh source-free Foundation consumer failed original five oracles");
        return commands;
    }

    private static string FeedConfig(string? source = null)
    {
        var xml = new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                source is null ? null :
                    new XElement("add", new XAttribute("key", "only"),
                        new XAttribute("value", source)))));
        return xml.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static string ExistingFeed()
    {
        var path = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(path) && Path.IsPathFullyQualified(path) &&
             Directory.Exists(path), "NUGET_LOCAL must be an existing absolute directory");
        return path;
    }

    private static bool IsExactPhysical(string path, int bytes, string sha) =>
        File.Exists(path) &&
        (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
        new FileInfo(path).Length == bytes && Hash(path) == sha;

    private static string PackageDllHash(string file)
    {
        using var archive = ZipFile.OpenRead(file);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(e => e.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                 !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !e.FullName.Contains('\\')),
             "NuGet ZIP contents not bounded");
        var nuspec = archive.GetEntry(Id + ".nuspec") ??
            throw new InvalidDataException("real Foundation nuspec missing");
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == Id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" &&
                     x.Value == Version) &&
                 !xml.Descendants().Any(x => x.Name.LocalName == "dependency"),
                 "Foundation package identity or zero-dependency closure drifted");
        }
        var dll = archive.GetEntry("lib/net10.0/" + Id + ".dll") ??
            throw new InvalidDataException("net10.0 Foundation DLL missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static void ExpectRefusal(Action action, string code)
    {
        var refused = false;
        try { action(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == code)
        { refused = true; }
        Need(refused, "shared NuGet publisher did not refuse " + code);
    }

    private static string ArchiveName() => Id + "." + Version + ".nupkg";
    private static JsonDocument Load(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));
    private static string Text(JsonElement node, string name) =>
        node.GetProperty(name).GetString() ??
        throw new InvalidDataException("missing original qualified field " + name);
    private static string PathFor(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
    private static string Hash(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    private static void Need(bool okay, string reason)
    { if (!okay) throw new InvalidDataException(reason); }

    private static (string Output, object Receipt) Run(
        string root, string cache, string id, string program, string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = program, WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true
            }
        };
        process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
        process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        process.StartInfo.Environment["NUGET_PACKAGES"] = cache;
        foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
        var stopwatch = Stopwatch.StartNew();
        Need(process.Start(), "cannot launch " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Foundation publisher command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        stopwatch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4100)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = program + " " + string.Join(" ", args),
            elapsed_ms = stopwatch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }
}
