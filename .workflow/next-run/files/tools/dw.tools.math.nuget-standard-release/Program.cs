using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetStandardRelease;

internal static class Program
{
    private const string PackageId = "dw.quantities.standard";
    private const string Version = "20260.20261103.1001";
    private const string HistoricalVersion = "0.2.0-preview.1";
    private const string ArchiveSha = "c600644b960b164f0f48c4088924d9d31b9b73c9e3d973e86d088c080a809b75";
    private const string DllSha = "5936468ecbcd8d8c27c04fe07ac8ca1e69dbebc9b7aa5145684f4ddf747fae0d";
    private const string BaseId = "dw.quantities";
    private const string ExpressionId = "dw.quantities.expression";
    private const string BaseVersion = "20260.20261102.1001";
    private const string ExpressionVersion = "20260.20261102.1001";
    private const string BaseArchiveSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string ExpressionArchiveSha =
        "d4eda8b7704093cc908056c3d998662931060e7ebf8e4a98b8057e6dab2cecae";
    private const string ExpressionDllSha =
        "f19c71d1313388f8cf3f05661ef78aaac8687d5230ed5673207f8fc0c9ff03e1";
    private const string CoreDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string CandidateReceipt =
        "docs/distribution/nuget-federation-f8-standard-candidate-qualified.json";
    private const string BaseReceipt =
        "docs/distribution/nuget-federation-f4-quantities-published-qualified.json";
    private const string ExpressionReceipt =
        "docs/distribution/nuget-federation-f7-expression-published-qualified.json";
    private const string CandidatePath =
        "build/artifacts/rs141-standard-candidate/candidate/dw.quantities.standard.20260.20261103.1001.nupkg";
    private const string SourceProject = "src/projects/dw.quantities.standard/dw.quantities.standard.csproj";
    private const string SourceDirectory = "src/projects/dw.quantities.standard";
    private const string Fixture =
        "tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs";
    private const string Report = ".aura/workflow/reports/RS142/attempt-001.json";
    private static readonly string[] SourceNames =
    [
        "ExplicitProfileExpressionUnitResolver.cs", "InheritedStandardUnitCatalog.cs",
        "PureUnitConverter.cs", "StandardExpressionUnitResolver.cs", "StandardUnitCatalog.cs"
    ];
    private static readonly (string Id, string Version)[] DirectDependencies =
    [
        (BaseId, "[" + BaseVersion + "]"),
        (ExpressionId, "[" + ExpressionVersion + "]")
    ];
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
            VerifySourceAuthority(root, feed);
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
            Console.Error.WriteLine("MATH-STANDARD-RELEASE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void VerifySourceAuthority(string root, string feed)
    {
        using var source = ReadJson(RootFile(root, CandidateReceipt));
        using var baseSource = ReadJson(RootFile(root, BaseReceipt));
        using var expressionSource = ReadJson(RootFile(root, ExpressionReceipt));
        var owner = source.RootElement;
        var candidate = owner.GetProperty("candidate");
        Need(Get(owner, "status") ==
                 "rs141-standard-two-dependency-calendar-candidate-qualified-unpublished" &&
             !owner.GetProperty("package_published").GetBoolean() &&
             Get(candidate, "status") == "qualified-standard-dependent-candidate-not-published" &&
             Get(candidate, "packageId") == PackageId &&
             Get(candidate, "packageVersion") == Version &&
             Get(candidate, "historicalSourceVersion") == HistoricalVersion &&
             Get(candidate, "candidateArchiveSha256") == ArchiveSha &&
             Get(candidate, "candidateDllSha256") == DllSha &&
             candidate.GetProperty("candidateArchiveBytes").GetInt32() == 13604 &&
             candidate.GetProperty("packageDependencies").GetInt32() == 2 &&
             candidate.GetProperty("consumerChecks").GetInt32() == 10 &&
             candidate.GetProperty("independentlyCompiledBinaryConsumer").GetBoolean() &&
             !candidate.GetProperty("published").GetBoolean() &&
             Get(candidate, "candidateArchivePath") == CandidatePath,
             "RS141 exact archive, candidate status or independent consumer changed");
        Need(Get(candidate, "dependencyQuantitiesId") == BaseId &&
             Get(candidate, "dependencyQuantitiesVersion") == BaseVersion &&
             Get(candidate, "dependencyQuantitiesArchiveSha256") == BaseArchiveSha &&
             Get(candidate, "dependencyQuantitiesDllSha256") == BaseDllSha &&
             Get(candidate, "dependencyExpressionId") == ExpressionId &&
             Get(candidate, "dependencyExpressionVersion") == ExpressionVersion &&
             Get(candidate, "dependencyExpressionArchiveSha256") == ExpressionArchiveSha &&
             Get(candidate, "dependencyExpressionDllSha256") == ExpressionDllSha,
             "both RS141 exact package dependencies must remain pinned");
        Need(Get(baseSource.RootElement, "status") ==
                 "rs137-published-local-math-nuget-binary-independently-verified" &&
             baseSource.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             Get(expressionSource.RootElement, "status") ==
                 "rs140-expression-published-local-binary-independently-verified" &&
             expressionSource.RootElement.GetProperty("physically_published_package_verified")
                 .GetBoolean() &&
             expressionSource.RootElement.GetProperty("dependency_package_verified")
                 .GetBoolean(),
             "two prerequisites need their prior independent DWF-published receipts");

        var original = RootFile(root, SourceProject);
        var originalXml = XDocument.Load(original, LoadOptions.None);
        string? Property(string name) => originalXml.Descendants()
            .Where(x => x.Name.LocalName == name)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        var references = originalXml.Descendants()
            .Where(x => x.Name.LocalName == "ProjectReference")
            .Select(x => (string?)x.Attribute("Include")).ToArray();
        Need(Property("PackageId") == PackageId &&
             Property("AssemblyName") == PackageId &&
             Property("RootNamespace") == "Dw.Quantities.Standard" &&
             Property("Version") == HistoricalVersion &&
             Property("IsPackable") == "true" &&
             references.SequenceEqual(
             [
                 "../dw.quantities/dw.quantities.csproj",
                 "../dw.quantities.expression/dw.quantities.expression.csproj"
             ]) &&
             !originalXml.Descendants().Any(x => x.Name.LocalName == "PackageReference") &&
             Sha(original) == Get(candidate, "originalProjectSha256"),
             "historical Standard project source graph or bytes changed");
        var sourceFingerprint = candidate.GetProperty("originalSourcesSha256");
        var sourceFiles = Directory.EnumerateFiles(
            RootFile(root, SourceDirectory), "*.cs", SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(sourceFiles.Select(Path.GetFileName).SequenceEqual(SourceNames),
             "historical Standard set must contain exactly five sources");
        foreach (var file in sourceFiles)
        {
            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
            Need(Sha(file) == Get(sourceFingerprint, relative),
                 "pinned RS141 Standard source was replaced: " + relative);
        }
        Need(File.Exists(RootFile(root, Fixture)), "RS141 source-free binary fixture missing");

        Need(CheckOriginalDependency(feed, BaseId, BaseVersion, 14747,
                 BaseArchiveSha, BaseDllSha, []) &&
             CheckOriginalDependency(feed, ExpressionId, ExpressionVersion, 13951,
                 ExpressionArchiveSha, ExpressionDllSha,
                 [(BaseId, "[" + BaseVersion + "]")]),
             "published base or Expression archive/dependency changed");
        var core = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(core.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             Sha(core.Location) == CoreDllSha,
             "the loaded shared NuGet publisher must match its qualified binary");
    }

    private static void Rehearse(string root, string feed)
    {
        // The ONLY mode permitted during DWF RS142. No write into NUGET_LOCAL.
        var work = RootFile(root, "build/artifacts/rs142-standard-rehearsal-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "fresh run-owned disposable workspace required");
        var producer = Path.Combine(work, "producer");
        var generated = Path.Combine(work, "candidate");
        var disposable = Path.Combine(work, "disposable-feed");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(generated);
        Directory.CreateDirectory(disposable);
        try
        {
            var cache = Path.Combine(work, "private-nuget-cache");
            var project = Path.Combine(producer, "standard-release-rehearsal.csproj");
            File.WriteAllText(project, ProducerProject(root), new UTF8Encoding(false));
            var config = Path.Combine(producer, "NuGet.Config");
            File.WriteAllText(config, Config(("published-dependencies", feed)), new UTF8Encoding(false));

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
                "--output", generated, "-p:NuGetAudit=false"
            ]).Receipt);
            var trialFile = Path.Combine(generated, PackageId + "." + Version + ".nupkg");
            Need(File.Exists(trialFile), "real Standard dependent package was not created");
            var archive = Sha(trialFile);
            var dll = CheckPackage(trialFile, PackageId, Version, DirectDependencies);

            var repository = new DotNetLocalPackageRepository(disposable,
                new DotNetLocalRepositoryLimits(256, 67_108_864, 67_108_864));
            var prepared = repository.PreparePublication(
                trialFile, archive, CancellationToken.None, ignoreSubdirectories: true);
            Need(!File.Exists(prepared.Receipt.Path),
                 "shared PreparePublication must not write an archive");
            var published = repository.Publish(prepared, CancellationToken.None);
            Need(published.PackageId == PackageId &&
                 published.Version == Version &&
                 published.Sha256 == archive &&
                 Sha(published.Path) == archive &&
                 CheckPackage(published.Path, PackageId, Version, DirectDependencies) == dll,
                 "shared publisher did not preserve exact dependent package bytes");
            var readback = repository.List(PackageId);
            Need(readback.InspectedPackages == 1 &&
                 readback.MatchedPackages == 1 &&
                 readback.Packages.Single().Sha256 == archive,
                 "shared NuGet reader rejected the disposable released archive");
            ExpectRefusal(() => repository.PreparePublication(
                trialFile, archive, CancellationToken.None, ignoreSubdirectories: true),
                "collision");
            ExpectRefusal(() => repository.PreparePublication(
                trialFile, new string('0', 64), CancellationToken.None,
                ignoreSubdirectories: true), "digest_mismatch");
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancelledRefused = false;
            try
            {
                _ = repository.PreparePublication(
                    trialFile, archive, cancelled.Token, ignoreSubdirectories: true);
            }
            catch (OperationCanceledException)
            {
                cancelledRefused = true;
            }
            Need(cancelledRefused && Sha(published.Path) == archive,
                 "cancellation was not refused or altered the disposable archive");

            commands.AddRange(RunConsumer(root, work, disposable, feed, dll));
            Need(CheckOriginalDependency(feed, BaseId, BaseVersion, 14747,
                     BaseArchiveSha, BaseDllSha, []) &&
                 CheckOriginalDependency(feed, ExpressionId, ExpressionVersion, 13951,
                     ExpressionArchiveSha, ExpressionDllSha,
                     [(BaseId, "[" + BaseVersion + "]")]) &&
                 Sha(published.Path) == archive,
                 "published prerequisites or disposable Standard bytes changed during trial");
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 7, "rehearsal must record seven real command results");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.standard-dependent-nuget-publisher-rehearsal",
                status = "standard-real-two-dependency-publisher-qualified",
                packageId = PackageId,
                version = Version,
                historicalVersion = HistoricalVersion,
                rs141ArchiveSha256 = ArchiveSha,
                rs141DllSha256 = DllSha,
                rehearsalArchiveSha256 = archive,
                rehearsalDllSha256 = dll,
                dependencies = new[]
                {
                    new { id = BaseId, version = BaseVersion, archiveSha256 = BaseArchiveSha,
                          dllSha256 = BaseDllSha },
                    new { id = ExpressionId, version = ExpressionVersion,
                          archiveSha256 = ExpressionArchiveSha, dllSha256 = ExpressionDllSha }
                },
                dependencyCount = 2,
                publisher = "Dw.Tools.NuGet/[20260.20261026.1010]",
                publisherDllSha256 = CoreDllSha,
                controls = 7,
                consumerChecks = 10,
                collisionRefused = true,
                alteredDigestRefused = true,
                cancelledRefused,
                disposableFeedPublished = true,
                publishedToNugetLocal = false,
                sourceFilesUnchanged = true,
                originalProjectVersionUnchanged = true,
                historicalProjectReferencesPreserved = true,
                publishedDependencyArchivesUnchanged = true,
                sourceReleaseAuthorization = false,
                installedPackageRollbackQualified = false,
                commands
            }, Pretty));
        }
        finally
        {
            if (Directory.Exists(work))
                Directory.Delete(work, recursive: true);
        }
    }

    private static void Publish(string root, string feed)
    {
        RequireQualifiedRunAndCleanCheckout(root);
        var candidate = RootFile(root, CandidatePath);
        Need(File.Exists(candidate) && new FileInfo(candidate).Length == 13604 &&
             Sha(candidate) == ArchiveSha &&
             CheckPackage(candidate, PackageId, Version, DirectDependencies) == DllSha,
             "only the original RS141 13,604-byte Standard archive is eligible");
        Need(DateOnly.FromDateTime(DateTime.Now) >= new DateOnly(2026, 11, 3),
             "publication must not predate the candidate's creation date");
        var records = RootFile(root, "build/artifacts/rs142-standard-release/receipts");
        var receiptFile = Path.Combine(records, PackageId + "." + Version + ".json");
        Need(!File.Exists(receiptFile),
             "an existing local publication receipt must not be replaced");
        var repo = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = repo.PreparePublication(candidate, ArchiveSha,
            CancellationToken.None, ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == PackageId &&
             prepared.Receipt.Version == Version &&
             prepared.Receipt.Sha256 == ArchiveSha,
             "shared publisher prepared the wrong immutable package");
        var published = repo.Publish(prepared, CancellationToken.None);
        Need(Sha(published.Path) == ArchiveSha &&
             CheckPackage(published.Path, PackageId, Version, DirectDependencies) == DllSha,
             "published archive or exact dependencies differ after readback");

        Directory.CreateDirectory(records);
        using (var stream = new FileStream(receiptFile,
                   FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.standard-private-local-nuget-publication",
                status = "published-local-awaiting-independent-dwf-readback",
                packageId = PackageId,
                version = Version,
                archiveSha256 = ArchiveSha,
                standardDllSha256 = DllSha,
                baseVersion = BaseVersion,
                expressionVersion = ExpressionVersion,
                feedSelector = "NUGET_LOCAL",
                independentlyReadFromPublishedFeed = false,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false
            }, Pretty);
            stream.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-STANDARD-PUBLISHED|id=" + PackageId +
            "|version=" + Version + "|sha256=" + ArchiveSha +
            "|dependencies=2|feed=NUGET_LOCAL|verification=pending");
    }

    private static void VerifyPublished(string root, string feed)
    {
        RequireQualifiedRunAndCleanCheckout(root);
        var published = Path.Combine(feed, PackageId + "." + Version + ".nupkg");
        Need(File.Exists(published) && new FileInfo(published).Length == 13604 &&
             Sha(published) == ArchiveSha &&
             CheckPackage(published, PackageId, Version, DirectDependencies) == DllSha,
             "actual shared-feed Standard archive or DLL differs from RS141");
        var work = RootFile(root, "build/artifacts/rs142-standard-verify-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "standard verifier workspace occupied");
        Directory.CreateDirectory(work);
        try
        {
            var commands = RunConsumer(root, work, feed, feed, DllSha);
            Need(commands.Count == 3 &&
                 Sha(published) == ArchiveSha,
                 "fresh published Standard package-only consumer failed or package changed");
            Console.WriteLine("MATH-STANDARD-VERIFY|id=" + PackageId +
                "|version=" + Version + "|binary=10|archive=" + ArchiveSha +
                "|dependencies=2|feed=NUGET_LOCAL|PASS");
        }
        finally
        {
            if (Directory.Exists(work))
                Directory.Delete(work, recursive: true);
        }
    }

    private static List<object> RunConsumer(
        string root, string workspace, string standardFeed, string dependencyFeed,
        string standardDll)
    {
        var consumer = Path.Combine(workspace, "binary-consumer");
        Directory.CreateDirectory(consumer);
        File.Copy(RootFile(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var project = Path.Combine(consumer, "standard-consumer.csproj");
        File.WriteAllText(project, new XDocument(
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
                        new XAttribute("Version", "[" + Version + "]")))))
            .ToString(SaveOptions.DisableFormatting) + "\n", new UTF8Encoding(false));
        var config = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(config,
            string.Equals(standardFeed, dependencyFeed, StringComparison.OrdinalIgnoreCase)
                ? Config(("published", standardFeed))
                : Config(("candidate", standardFeed), ("published", dependencyFeed)),
            new UTF8Encoding(false));
        var cache = Path.Combine(workspace, "private-nuget-cache");
        var operations = new List<object>();
        operations.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", project, "--configfile", config, "--packages", cache,
            "-p:NuGetAudit=false"
        ]).Receipt);
        operations.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", project, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var executed = Run(root, cache, "consumer-binary", "dotnet", [
            "run", "--project", project, "-c", "Release",
            "--no-build", "--no-restore", "--", standardDll, ExpressionDllSha, BaseDllSha
        ]);
        operations.Add(executed.Receipt);
        Need(executed.Output.Contains("MATH-STANDARD-BINARY|PASS|10",
             StringComparison.Ordinal) &&
             executed.Output.Contains("MATH-STANDARD-CHECK|250g-equals-quarter-kg|PASS",
             StringComparison.Ordinal) &&
             executed.Output.Contains("MATH-STANDARD-CHECK|loaded-dll-hashes|PASS",
             StringComparison.Ordinal),
             "real source-free Standard consumer did not verify three DLLs and 10 cases");
        return operations;
    }

    private static string ProducerProject(string root) =>
        new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", "Dw.Quantities.Standard"),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", HistoricalVersion),
                    new XElement("PackageVersion", Version),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("Description",
                        "Standalone exact standard-unit catalogue with explicit cultural profiles and passive metadata."),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    SourceNames.Select(name => new XElement("Compile",
                        new XAttribute("Include", RootFile(root, SourceDirectory + "/" + name)),
                        new XAttribute("Link", name))),
                    DirectDependencies.Select(reference => new XElement("PackageReference",
                        new XAttribute("Include", reference.Id),
                        new XAttribute("Version", reference.Version))))))
        .ToString(SaveOptions.DisableFormatting) + "\n";

    private static string Config(params (string Key, string Path)[] sources) =>
        new XDocument(new XElement("configuration",
            new XElement("packageSources",
                new XElement("clear"),
                sources.Select(x => new XElement("add",
                    new XAttribute("key", x.Key),
                    new XAttribute("value", x.Path))))))
        .ToString(SaveOptions.DisableFormatting) + "\n";

    private static void RequireQualifiedRunAndCleanCheckout(string root)
    {
        using var report = ReadJson(RootFile(root, Report));
        var run = report.RootElement;
        Need(Get(run, "runId") == "RS142" &&
             Get(run, "outcome") == "succeeded" &&
             run.GetProperty("attempt").GetInt32() >= 1 &&
             Get(run.GetProperty("git"), "commitMessage") ==
                 "test: qualify Standard immutable local NuGet publisher rehearsal",
             "RS142 successful DWF report is required for explicit operator publication");
        using var rights = ReadJson(RootFile(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        Need(rights.RootElement.GetProperty("authorization")
                 .GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.RootElement.GetProperty("authorization")
                 .GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "private Math packaging and redistribution require the existing owner rights");
        var cache = RootFile(root, "build/artifacts/rs142-operator-cache");
        var branch = Run(root, cache, "git-branch", "git",
            ["branch", "--show-current"]).Output.Trim();
        var status = Run(root, cache, "git-clean", "git",
            ["status", "--porcelain", "--untracked-files=normal"]).Output.Trim();
        Need(branch == "main" && status.Length == 0,
             "explicit release requires a clean main branch checkout");
    }

    private static bool CheckOriginalDependency(
        string feed, string id, string version, int bytes,
        string archiveSha, string dllSha, IReadOnlyList<(string Id, string Version)> dependencies)
    {
        var path = Path.Combine(feed, id + "." + version + ".nupkg");
        return File.Exists(path) &&
            (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0 &&
            new FileInfo(path).Length == bytes &&
            Sha(path) == archiveSha &&
            CheckPackage(path, id, version, dependencies) == dllSha;
    }

    private static string CheckPackage(
        string file, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var package = ZipFile.OpenRead(file);
        Need(package.Entries.Count is >= 3 and <= 48 &&
             package.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == package.Entries.Count &&
             package.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "real NuGet archive ZIP layout must be bounded");
        var manifest = package.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("exact package nuspec missing");
        using (var contents = manifest.Open())
        {
            var doc = XDocument.Load(contents, LoadOptions.None);
            Need(doc.Descendants().Any(x => x.Name.LocalName == "id" && x.Value == id) &&
                 doc.Descendants().Any(x => x.Name.LocalName == "version" && x.Value == version),
                 "package ID or version differs");
            var actual = doc.Descendants().Where(x => x.Name.LocalName == "dependency")
                .Select(x => (Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? ""))
                .OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            var expected = dependencies.OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            Need(actual.SequenceEqual(expected),
                 "package must declare exactly its pinned NuGet dependency closure");
        }
        var library = package.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("expected net10.0 package DLL is missing");
        using var stream = library.Open();
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }

    private static void ExpectRefusal(Action operation, string reason)
    {
        var refused = false;
        try { operation(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == reason)
        {
            refused = true;
        }
        Need(refused, "shared publisher failed to refuse " + reason);
    }

    private static string RequiredFeed()
    {
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must point to the private local package feed");
        Need(!string.IsNullOrWhiteSpace(feed) &&
             Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed),
             "NUGET_LOCAL must be an existing absolute directory");
        return feed;
    }

    private static JsonDocument ReadJson(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Get(JsonElement element, string field) =>
        element.GetProperty(field).GetString() ??
        throw new InvalidDataException("Missing source package field " + field);

    private static string RootFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool condition, string detail)
    {
        if (!condition) throw new InvalidDataException(detail);
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
        var stopwatch = Stopwatch.StartNew();
        Need(process.Start(), "could not launch bounded Standard command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Standard command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        stopwatch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 3700)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = stopwatch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1050)]
        });
    }
}
