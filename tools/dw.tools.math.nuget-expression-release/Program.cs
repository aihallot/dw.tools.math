using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetExpressionRelease;

internal static class Program
{
    private const string PackageId = "dw.quantities.expression";
    private const string Version = "20260.20261102.1001";
    private const string HistoricalSourceVersion = "0.2.0-preview.1";
    private const string ExpressionArchiveSha =
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
    private const string ExpressionDllSha =
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
    private const string BaseId = "dw.quantities";
    private const string BaseVersion = "20260.20261102.1001";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string NuGetCoreSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string Receipt =
        "docs/distribution/nuget-federation-f5-expression-candidate-qualified.json";
    private const string BaseReceipt =
        "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
    private const string CandidateRelativePath =
        "build/artifacts/rs138-expression-candidate/candidate/dw.quantities.expression.20260.20261102.1001.nupkg";
    private const string SourceProject =
        "src/projects/dw.quantities.expression/dw.quantities.expression.csproj";
    private const string SourceDir = "src/projects/dw.quantities.expression";
    private const string Fixture =
        "tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs";
    private const string ReleaseDwfReport = ".aura/workflow/reports/RS139/attempt-001.json";
    private static readonly string[] SourceNames =
        ["ExactSelection.cs", "ExpressionDiagnostics.cs", "ExpressionParser.cs"];
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || args[0] is not ("--rehearse" or "--publish" or "--verify"))
            {
                Console.Error.WriteLine("Expected --rehearse, --publish or --verify.");
                return 2;
            }
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            var feed = RequiredFeed();
            VerifyOwner(root, feed);
            switch (args[0])
            {
                case "--rehearse": Rehearse(root, feed); break;
                case "--publish": Publish(root, feed); break;
                case "--verify": VerifyPublished(root, feed); break;
            }
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-EXPRESSION-RELEASE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void VerifyOwner(string root, string feed)
    {
        using var receipt = ReadJson(Absolute(root, Receipt));
        using var baseReceipt = ReadJson(Absolute(root, BaseReceipt));
        var owner = receipt.RootElement;
        var candidate = owner.GetProperty("candidate");
        Need(Read(owner, "status") ==
             "rs138-expression-calendar-dependent-nuget-candidate-qualified-unpublished" &&
             !owner.GetProperty("package_published").GetBoolean() &&
             Read(candidate, "status") == "qualified-local-dependent-candidate-not-published" &&
             Read(candidate, "packageId") == PackageId &&
             Read(candidate, "packageVersion") == Version &&
             Read(candidate, "historicalSourceVersion") == HistoricalSourceVersion &&
             Read(candidate, "candidateArchiveSha256") == ExpressionArchiveSha &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13951 &&
             Read(candidate, "candidateDllSha256") == ExpressionDllSha &&
             Read(candidate, "exactBasePackageId") == BaseId &&
             Read(candidate, "exactBasePackageVersion") == BaseVersion &&
             Read(candidate, "exactBasePackageSha256") == BaseArchiveSha &&
             Read(candidate, "exactBaseDllSha256") == BaseDllSha &&
             candidate.GetProperty("packagedDependencies").GetInt32() == 1 &&
             candidate.GetProperty("consumerChecks").GetInt32() == 6 &&
             candidate.GetProperty("actualSourceFreeConsumerPassed").GetBoolean(),
             "RS138 real package and independent consumer provenance changed");
        Need(Read(baseReceipt.RootElement, "status") ==
             "rs137-published-local-math-nuget-binary-independently-verified" &&
             baseReceipt.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean(),
             "the prerequisite published base package lacks independent DWF evidence");

        var sourceProject = Absolute(root, SourceProject);
        var xml = XDocument.Load(sourceProject, LoadOptions.None);
        string? Property(string name) => xml.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == PackageId &&
             Property("AssemblyName") == PackageId &&
             Property("RootNamespace") == PackageId &&
             Property("Version") == HistoricalSourceVersion &&
             Property("IsPackable") == "true" &&
             xml.Descendants().Where(x => x.Name.LocalName == "ProjectReference")
                 .Select(x => (string?)x.Attribute("Include"))
                 .SequenceEqual(["../dw.quantities/dw.quantities.csproj"]) &&
             !xml.Descendants().Any(x => x.Name.LocalName == "PackageReference") &&
             Sha(sourceProject) == Read(candidate, "originalProjectSha256"),
             "original Expression source package or ProjectReference changed");
        foreach (var name in SourceNames)
        {
            var path = SourceDir + "/" + name;
            Need(Sha(Absolute(root, path)) ==
                 Read(candidate.GetProperty("originalSourceSha256"), path),
                 "RS138 source SHA mismatch for " + name);
        }
        Need(File.Exists(Absolute(root, Fixture)), "RS138 binary-only fixture is missing");

        var basePath = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
        Need(File.Exists(basePath) && Sha(basePath) == BaseArchiveSha &&
             CheckArchive(basePath, BaseId, BaseVersion, []) == BaseDllSha,
             "immutable published base is unavailable or changed");
        var core = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(core.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             Sha(core.Location) == NuGetCoreSha,
             "the loaded Dw.Tools.NuGet shared publisher differs from the qualified DLL");
    }

    private static void Rehearse(string root, string feed)
    {
        var workspace = Absolute(root, "build/artifacts/rs139-expression-rehearsal-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(workspace), "rehearsal workspace already exists");
        var producer = Path.Combine(workspace, "producer");
        var packageFolder = Path.Combine(workspace, "candidate");
        var disposable = Path.Combine(workspace, "disposable-feed");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(packageFolder);
        Directory.CreateDirectory(disposable);
        try
        {
            var cache = Path.Combine(workspace, "private-nuget-cache");
            var project = Path.Combine(producer, "expression-release-rehearsal.csproj");
            File.WriteAllText(project, ProducerProject(root), new UTF8Encoding(false));
            var config = Path.Combine(producer, "NuGet.Config");
            File.WriteAllText(config, Config(("published", feed)), new UTF8Encoding(false));
            var commands = new List<object>();
            commands.Add(Run(root, cache, "source-restore", "dotnet", [
                "restore", project, "--configfile", config, "--packages", cache,
                "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "source-build", "dotnet", [
                "build", project, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ]).Receipt);
            commands.Add(Run(root, cache, "source-pack", "dotnet", [
                "pack", project, "-c", "Release", "--no-build", "--no-restore",
                "--output", packageFolder, "-p:NuGetAudit=false"
            ]).Receipt);
            var archivePath = Path.Combine(packageFolder, PackageId + "." + Version + ".nupkg");
            Need(File.Exists(archivePath), "real linked-source expression package absent");
            var archiveSha = Sha(archivePath);
            var dllSha = CheckArchive(archivePath, PackageId, Version, [(BaseId, "[" + BaseVersion + "]")]);
            var packageRepository = new DotNetLocalPackageRepository(disposable,
                new DotNetLocalRepositoryLimits(256, 67_108_864, 67_108_864));
            var prepared = packageRepository.PreparePublication(
                archivePath, archiveSha, CancellationToken.None, ignoreSubdirectories: true);
            Need(!File.Exists(prepared.Receipt.Path), "shared publisher's prepare wrote package bytes");
            var delivered = packageRepository.Publish(prepared, CancellationToken.None);
            Need(delivered.PackageId == PackageId &&
                 delivered.Version == Version &&
                 delivered.Sha256 == archiveSha &&
                 Sha(delivered.Path) == archiveSha &&
                 CheckArchive(delivered.Path, PackageId, Version,
                     [(BaseId, "[" + BaseVersion + "]")]) == dllSha,
                 "real Expression dependency package did not publish identical bytes");
            var inspection = packageRepository.List(PackageId);
            Need(inspection.InspectedPackages == 1 && inspection.MatchedPackages == 1 &&
                 inspection.Packages.Single().Sha256 == archiveSha,
                 "the shared package reader disagrees with disposable published identity");
            ExpectRefusal(() => packageRepository.PreparePublication(
                archivePath, archiveSha, CancellationToken.None, ignoreSubdirectories: true),
                "collision");
            ExpectRefusal(() => packageRepository.PreparePublication(
                archivePath, new string('0', 64), CancellationToken.None,
                ignoreSubdirectories: true), "digest_mismatch");
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancellationRefused = false;
            try
            {
                _ = packageRepository.PreparePublication(
                    archivePath, archiveSha, cancelled.Token, ignoreSubdirectories: true);
            }
            catch (OperationCanceledException)
            {
                cancellationRefused = true;
            }
            Need(cancellationRefused &&
                 Sha(delivered.Path) == archiveSha,
                 "cancelled publisher did not refuse or changed disposable bytes");

            commands.AddRange(Consume(root, workspace, disposable, feed, dllSha));
            var basePath = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
            Need(Sha(basePath) == BaseArchiveSha &&
                 Sha(delivered.Path) == archiveSha,
                 "a producer dependency or disposable publication changed");
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 7, "seven as-run source/consumer/SDK receipts expected");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.expression-disposable-publisher-rehearsal",
                status = "expression-real-dependent-package-publisher-qualified",
                packageId = PackageId,
                version = Version,
                historicalVersion = HistoricalSourceVersion,
                rs138CandidateArchiveSha256 = ExpressionArchiveSha,
                rs138CandidateDllSha256 = ExpressionDllSha,
                rehearsalArchiveSha256 = archiveSha,
                rehearsalDllSha256 = dllSha,
                exactDependencyId = BaseId,
                exactDependencyVersion = BaseVersion,
                dependencyArchiveSha256 = BaseArchiveSha,
                dependencyDllSha256 = BaseDllSha,
                publisher = "Dw.Tools.NuGet/[20260.20261026.1010]",
                publisherDllSha256 = NuGetCoreSha,
                checks = 7,
                consumerChecks = 6,
                disposableFeedPublished = true,
                publishedToNugetLocal = false,
                invalidHashRefused = true,
                duplicateIdentityRefused = true,
                cancelledRefused = cancellationRefused,
                packagedSourceFilesUnchanged = true,
                historicalProjectVersionUnchanged = true,
                sourceFeedBaseArchiveUnchanged = true,
                installedPackageRollbackQualified = false,
                sourceReleaseAuthorization = false,
                commandReceipts = commands
            }, Pretty));
        }
        finally
        {
            if (Directory.Exists(workspace))
                Directory.Delete(workspace, recursive: true);
        }
    }

    private static void Publish(string root, string feed)
    {
        RequireQualifiedRunAndCleanCheckout(root);
        var archivePath = Absolute(root, CandidateRelativePath);
        Need(File.Exists(archivePath) && new FileInfo(archivePath).Length == 13951 &&
             Sha(archivePath) == ExpressionArchiveSha &&
             CheckArchive(archivePath, PackageId, Version,
                 [(BaseId, "[" + BaseVersion + "]")]) == ExpressionDllSha,
             "only the original exact RS138 candidate bytes may be published");
        var creation = new DateOnly(2026, 11, 2);
        Need(DateOnly.FromDateTime(DateTime.Now) >= creation,
             "version creation date cannot be in the future");
        var publisher = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = publisher.PreparePublication(
            archivePath, ExpressionArchiveSha, CancellationToken.None,
            ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == PackageId &&
             prepared.Receipt.Version == Version &&
             prepared.Receipt.Sha256 == ExpressionArchiveSha,
             "shared publication prepared a different package");
        var delivered = publisher.Publish(prepared, CancellationToken.None);
        Need(Sha(delivered.Path) == ExpressionArchiveSha &&
             CheckArchive(delivered.Path, PackageId, Version,
                 [(BaseId, "[" + BaseVersion + "]")]) == ExpressionDllSha,
             "shared feed did not retain the exact qualified Expression bytes");
        var records = Absolute(root, "build/artifacts/rs139-expression-release/receipts");
        Directory.CreateDirectory(records);
        var receiptPath = Path.Combine(records, PackageId + "." + Version + ".json");
        using (var stream = new FileStream(receiptPath,
                   FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.expression-private-local-nuget-publication",
                status = "published-local-awaiting-independent-dwf-receipt",
                packageId = PackageId,
                version = Version,
                archiveSha256 = ExpressionArchiveSha,
                expressionDllSha256 = ExpressionDllSha,
                exactDependencyVersion = BaseVersion,
                feed = "NUGET_LOCAL",
                independentlyReadFromPublishedFeed = false,
                remotePublished = false,
                physicalInstalledRollbackQualified = false
            }, Pretty);
            stream.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-EXPRESSION-PUBLISHED|id=" + PackageId + "|version=" +
            Version + "|sha256=" + ExpressionArchiveSha + "|feed=NUGET_LOCAL|verification=pending");
    }

    private static void VerifyPublished(string root, string feed)
    {
        RequireQualifiedRunAndCleanCheckout(root);
        var published = Path.Combine(feed, PackageId + "." + Version + ".nupkg");
        Need(File.Exists(published) && new FileInfo(published).Length == 13951 &&
             Sha(published) == ExpressionArchiveSha &&
             CheckArchive(published, PackageId, Version,
                 [(BaseId, "[" + BaseVersion + "]")]) == ExpressionDllSha,
             "Expression local publication is missing or has changed bytes");
        var temporary = Absolute(root, "build/artifacts/rs139-expression-verify-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(temporary), "verification workspace occupied");
        Directory.CreateDirectory(temporary);
        try
        {
            var receipts = Consume(root, temporary, feed, feed, ExpressionDllSha);
            Need(receipts.Count == 3 &&
                 Sha(published) == ExpressionArchiveSha,
                 "published dependent NuGet consumer or source package changed");
            Console.WriteLine("MATH-EXPRESSION-VERIFY|id=" + PackageId +
                "|version=" + Version + "|binary=6|archive=" +
                ExpressionArchiveSha + "|dependency=" + BaseVersion +
                "|feed=NUGET_LOCAL|PASS");
        }
        finally
        {
            if (Directory.Exists(temporary))
                Directory.Delete(temporary, recursive: true);
        }
    }

    private static List<object> Consume(
        string root, string workspace, string expressionFeed, string baseFeed, string dllSha)
    {
        var consumerRoot = Path.Combine(workspace, "consumer");
        Directory.CreateDirectory(consumerRoot);
        File.Copy(Absolute(root, Fixture), Path.Combine(consumerRoot, "Program.cs"),
            overwrite: false);
        var consumerProject = Path.Combine(consumerRoot, "consumer.csproj");
        var consumerXml = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Exe"),
                    new XElement("IsPackable", "false"),
                    new XElement("EnableDefaultCompileItems", "false"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false")),
                new XElement("ItemGroup",
                    new XElement("Compile", new XAttribute("Include", "Program.cs")),
                    new XElement("PackageReference", new XAttribute("Include", PackageId),
                        new XAttribute("Version", "[" + Version + "]")))));
        File.WriteAllText(consumerProject,
            consumerXml.ToString(SaveOptions.DisableFormatting) + "\n",
            new UTF8Encoding(false));
        var config = Path.Combine(consumerRoot, "NuGet.Config");
        File.WriteAllText(config,
            string.Equals(expressionFeed, baseFeed, StringComparison.OrdinalIgnoreCase)
                ? Config(("published", expressionFeed))
                : Config(("expression", expressionFeed), ("base", baseFeed)),
            new UTF8Encoding(false));
        var cache = Path.Combine(workspace, "private-nuget-cache");
        var result = new List<object>();
        result.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", consumerProject, "--configfile", config, "--packages", cache,
            "-p:NuGetAudit=false"
        ]).Receipt);
        result.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", consumerProject, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var executed = Run(root, cache, "consumer-binary", "dotnet", [
            "run", "--project", consumerProject, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha, BaseDllSha
        ]);
        result.Add(executed.Receipt);
        Need(executed.Output.Contains("MATH-EXPRESSION-BINARY|PASS|6",
             StringComparison.Ordinal) &&
             executed.Output.Contains("MATH-EXPRESSION-CHECK|loaded-binaries|PASS",
             StringComparison.Ordinal),
             "published Expression and base NuGet DLL consumer did not pass all six cases");
        return result;
    }

    private static string ProducerProject(string root)
    {
        var project = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", PackageId),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", HistoricalSourceVersion),
                    new XElement("PackageVersion", Version),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    SourceNames.Select(name => new XElement("Compile",
                        new XAttribute("Include", Absolute(root, SourceDir + "/" + name)),
                        new XAttribute("Link", name))),
                    new XElement("PackageReference",
                        new XAttribute("Include", BaseId),
                        new XAttribute("Version", "[" + BaseVersion + "]")))));
        return project.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static string Config(params (string Key, string Path)[] sources)
    {
        var config = new XDocument(new XElement("configuration",
            new XElement("packageSources",
                new XElement("clear"),
                sources.Select(s => new XElement("add",
                    new XAttribute("key", s.Key),
                    new XAttribute("value", s.Path))))));
        return config.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static void RequireQualifiedRunAndCleanCheckout(string root)
    {
        using var report = ReadJson(Absolute(root, ReleaseDwfReport));
        var actual = report.RootElement;
        Need(Read(actual, "runId") == "RS139" &&
             Read(actual, "outcome") == "succeeded" &&
             actual.GetProperty("attempt").GetInt32() >= 1 &&
             Read(actual.GetProperty("git"), "commitMessage") ==
                 "test: qualify Expression immutable local NuGet publisher rehearsal",
             "RS139 successful DWF publisher rehearsal report is required");
        using var attest = ReadJson(Absolute(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        var rights = attest.RootElement.GetProperty("authorization");
        Need(rights.GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "source owner does not attest private package and redistribution rights");
        var branch = Run(root, Absolute(root, "build/artifacts"), "git-branch", "git",
            ["branch", "--show-current"]).Output.Trim();
        var status = Run(root, Absolute(root, "build/artifacts"), "git-clean", "git",
            ["status", "--porcelain", "--untracked-files=normal"]).Output.Trim();
        Need(branch == "main" && status.Length == 0,
             "private package release requires a clean main checkout");
    }

    private static string CheckArchive(
        string path, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var archive = ZipFile.OpenRead(path);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal)
                 .Count() == archive.Entries.Count &&
             archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                !e.FullName.Contains('\\')),
             "NuGet candidate ZIP layout is not bounded and canonical");
        var nuspec = archive.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("required NuGet nuspec missing");
        using (var stream = nuspec.Open())
        {
            var xml = XDocument.Load(stream, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" && x.Value == version),
                 "package id or version mismatch");
            var actual = xml.Descendants().Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? "")).ToArray();
            Need(actual.Length == dependencies.Count && actual.SequenceEqual(dependencies),
                 "package dependency closure must match the exact published base version");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("net10.0 Expression DLL missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static void ExpectRefusal(Action operation, string expected)
    {
        var refused = false;
        try { operation(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == expected)
        {
            refused = true;
        }
        Need(refused, "shared NuGet publisher did not refuse " + expected);
    }

    private static string RequiredFeed()
    {
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing local repository");
        Need(!string.IsNullOrWhiteSpace(feed) &&
             Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
             "NUGET_LOCAL must be an existing absolute directory");
        return feed;
    }

    private static JsonDocument ReadJson(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Read(JsonElement element, string field) =>
        element.GetProperty(field).GetString() ??
        throw new InvalidDataException("Missing exact owner field: " + field);

    private static string Absolute(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool valid, string message)
    {
        if (!valid) throw new InvalidDataException(message);
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
        foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not start bounded command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Expression publisher command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var error = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + error;
            throw new InvalidDataException(id + " failed exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3200)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1000)]
        });
    }
}
