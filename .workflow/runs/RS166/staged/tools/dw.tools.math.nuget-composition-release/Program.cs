using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetCompositionRelease;

internal static class Program
{
    private const string PackageId = "dw.tools.math.composition";
    private const string Version = "20260.20261107.1001";
    private const string SourceVersion = "0.3.0-preview.1";
    private const string CandidateSha =
        "b0624cb1835f66fef41ff763da065a400f1709f65808315e28e3247aa2a4228f";
    private const string CandidateDllSha =
        "6e50d10639ee45020fd53dfa6eb81be4e1e38eefcdff235a0731be8e17a2a252";
    private const string BaseId = "dw.tools.math.ir";
    private const string QuantitiesId = "dw.quantities";
    private const string QuantitiesVersion = "20260.20261102.1001";
    private const string QuantitiesArchiveSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string QuantitiesDllSha = "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string BaseVersion = "20260.20261104.1001";
    private const string BaseArchiveSha =
        "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
    private const string BaseDllSha =
        "5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af";
    private const string SharedNuGetDllSha =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";

    private const string CandidateReceipt =
        "docs/distribution/nuget-federation-f31-composition-candidate-qualified.json";
    private const string BaseReceipt =
        "docs/distribution/nuget-federation-f13-ir-published-qualified.json";
    private const string SourceProject =
        "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
    private const string SourceDirectory = "src/projects/dw.tools.math.composition";
    private const string ConsumerFixture =
        "tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs";
    private const string CandidatePath =
        "build/artifacts/rs164-composition-candidate/candidate/dw.tools.math.composition.20260.20261107.1001.nupkg";
    private const string Rs165Report = ".aura/workflow/reports/RS165/attempt-001.json";
    private static readonly (string Id, string Version)[] Dependencies =
        [(QuantitiesId, "[" + QuantitiesVersion + "]"),
         (BaseId, "[" + BaseVersion + "]")];
    private static readonly string[] OriginalSourceNames =
    [
        "ExactQuantityPipeline.cs",
        "ExactReplayContracts.cs",
        "MathCapabilityCatalog.cs",
        "OperationContracts.cs",
    ];
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 ||
                args[0] is not ("--rehearse" or "--publish" or "--verify"))
            {
                Console.Error.WriteLine("Expected --rehearse, --publish or --verify.");
                return 2;
            }
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            var feed = RequireExistingFeed();
            ValidateSourceAuthority(root, feed);
            switch (args[0])
            {
                case "--rehearse": Rehearse(root, feed); break;
                case "--publish": Publish(root, feed); break;
                case "--verify": VerifyPublished(root, feed); break;
            }
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("MATH-COMPOSITION-RELEASE-REFUSED: " + exception.Message);
            return 1;
        }
    }

    private static void ValidateSourceAuthority(string root, string feed)
    {
        using var owner = Load(RootFile(root, CandidateReceipt));
        using var prerequisite = Load(RootFile(root, BaseReceipt));
        var result = owner.RootElement;
        var qualified = result.GetProperty("candidate");
        Need(Value(result, "status") ==
                 "rs164-composition-dependent-calendar-candidate-qualified-unpublished" &&
             !result.GetProperty("package_published").GetBoolean() &&
             Value(qualified, "status") ==
                 "qualified-composition-dependent-calendar-candidate-not-published" &&
             Value(qualified, "packageId") == PackageId &&
             Value(qualified, "packageVersion") == Version &&
             Value(qualified, "historicalSourceVersion") == SourceVersion &&
             Value(qualified, "candidateArchivePath") == CandidatePath &&
             qualified.GetProperty("candidateArchiveBytes").GetInt32() == 17101 &&
             Value(qualified, "candidateArchiveSha256") == CandidateSha &&
             Value(qualified, "candidateDllSha256") == CandidateDllSha &&
             qualified.GetProperty("packageDependencies").GetInt32() == 2 &&
             qualified.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
             qualified.GetProperty("independentPackageReference").GetBoolean() &&
             !qualified.GetProperty("published").GetBoolean(),
             "RS164 original Composition candidate identity, source or binary qualification drifted");
        Need(Value(qualified, "dependencyIrId") == BaseId &&
             Value(qualified, "dependencyIrVersion") == BaseVersion &&
             Value(qualified, "dependencyIrArchiveSha256") == BaseArchiveSha &&
             Value(qualified, "dependencyIrDllSha256") == BaseDllSha &&
             Value(qualified, "dependencyQuantitiesId") == QuantitiesId &&
             Value(qualified, "dependencyQuantitiesVersion") == QuantitiesVersion &&
             Value(qualified, "dependencyQuantitiesArchiveSha256") == QuantitiesArchiveSha &&
             Value(qualified, "dependencyQuantitiesDllSha256") == QuantitiesDllSha &&
              qualified.GetProperty("originalSourceProjectReferences").GetInt32() == 2 &&
              qualified.GetProperty("originalFourSourceFilesLinked").GetBoolean(),
             "exact published IR and directly referenced Quantities dependencies differ from F31");
        var dependencyReceipt = prerequisite.RootElement;
        var dependencyObservation = dependencyReceipt.GetProperty("observation");
        Need(Value(dependencyReceipt, "status") ==
                 "rs146-ir-published-local-two-binaries-independently-verified" &&
             dependencyReceipt.GetProperty("physically_published_package_verified").GetBoolean() &&
             Value(dependencyObservation, "packageId") == BaseId &&
             Value(dependencyObservation, "packageVersion") == BaseVersion &&
             Value(dependencyObservation, "archiveSha256") == BaseArchiveSha &&
             Value(dependencyObservation, "packagedDllSha256") == BaseDllSha &&
             dependencyObservation.GetProperty("declaredDependencyCount").GetInt32() == 1 &&
             dependencyObservation.GetProperty("exactDependencies").EnumerateArray()
                 .Any(x => Value(x, "id") == QuantitiesId &&
                     Value(x, "version") == QuantitiesVersion &&
                     Value(x, "archiveSha256") == QuantitiesArchiveSha &&
                     Value(x, "packagedDllSha256") == QuantitiesDllSha),
             "F13 IR independent archive and transitive Quantities evidence drifted");

        var originalProject = RootFile(root, SourceProject);
        var xml = XDocument.Load(originalProject, LoadOptions.None);
        string? Property(string key) => xml.Descendants()
            .Where(x => x.Name.LocalName == key)
            .Select(x => x.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == PackageId &&
             Property("AssemblyName") == PackageId &&
             Property("RootNamespace") == "Dw.Tools.Math.Composition" &&
             Property("Version") == SourceVersion &&
             Property("IsPackable") == "true" &&
             xml.Descendants().Where(x => x.Name.LocalName == "ProjectReference")
                 .Select(x => (string?)x.Attribute("Include"))
                 .SequenceEqual(["../dw.quantities/dw.quantities.csproj",
                      "../dw.tools.math.ir/dw.tools.math.ir.csproj"]) &&
             !xml.Descendants().Any(x => x.Name.LocalName == "PackageReference") &&
             Sha(originalProject) == Value(qualified, "originalProjectSha256"),
             "the original Composition project SHA or two ProjectReferences changed");

        var originalFiles = Directory.EnumerateFiles(
                RootFile(root, SourceDirectory), "*.cs", SearchOption.TopDirectoryOnly)
            .Order(StringComparer.Ordinal).ToArray();
        Need(originalFiles.Select(Path.GetFileName)
                 .SequenceEqual(OriginalSourceNames, StringComparer.Ordinal),
             "Composition source set must contain precisely the same four C# files");
        var sourceHashes = qualified.GetProperty("originalSourcesSha256");
        foreach (var file in originalFiles)
        {
            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
            Need(Sha(file) == Value(sourceHashes, relative),
                 "RS164 pinned Composition source SHA differs: " + relative);
        }
        var lockPath = RootFile(root, SourceDirectory + "/packages.lock.json");
        using (var lockDocument = Load(lockPath))
        {
            var locked = lockDocument.RootElement;
            var dependencies = locked.GetProperty("dependencies").GetProperty("net10.0")
                .EnumerateObject().ToArray();
            Need(locked.GetProperty("version").GetInt32() == 2 &&
                 dependencies.Length == 2 &&
                 dependencies.Any(x => x.Name == BaseId &&
                     Value(x.Value, "type") == "Project") &&
                 dependencies.Any(x => x.Name == QuantitiesId &&
                     Value(x.Value, "type") == "Project"),
                 "original Composition lockfile retains both direct Quantities and IR projects");
        }
        Need(sourceHashes.EnumerateObject().Count() == 5 &&
             Sha(lockPath) == Value(qualified, "originalSourceLockSha256") &&
             Sha(lockPath) == Value(sourceHashes,
                 SourceDirectory + "/packages.lock.json") &&
             qualified.GetProperty("originalFourSourceFilesLinked").GetBoolean() &&
             qualified.GetProperty("originalSourceProjectReferences").GetInt32() == 2,
             "F31 four original Composition files and lockfile fingerprint drifted");
        Need(File.Exists(RootFile(root, ConsumerFixture)),
             "RS164 original source-free binary fixture is unavailable");

        var baseArchive = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
        Need(IsPhysical(baseArchive, 21926, BaseArchiveSha) &&
             PackageDllSha(baseArchive, BaseId, BaseVersion, [(QuantitiesId, "[" + QuantitiesVersion + "]")]) == BaseDllSha,
             "immutable published base package/dll differs");
        var transitiveArchive = Path.Combine(feed, QuantitiesId + "." + QuantitiesVersion + ".nupkg");
        Need(IsPhysical(transitiveArchive, 14747, QuantitiesArchiveSha) &&
             PackageDllSha(transitiveArchive, QuantitiesId, QuantitiesVersion, []) ==
                 QuantitiesDllSha,
             "immutable published transitive Quantities package differs");
        var library = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(library.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             Sha(library.Location) == SharedNuGetDllSha,
             "shared NuGet DLL differs from its qualified 20260.20261026.1010 identity");
    }

    private static void Rehearse(string root, string feed)
    {
        var work = RootFile(root, "build/artifacts/rs165-composition-rehearsal-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "rehearsal workspace already exists");
        var producer = Path.Combine(work, "producer");
        var packageDirectory = Path.Combine(work, "candidate");
        var disposableFeed = Path.Combine(work, "disposable-feed");
        Directory.CreateDirectory(producer);
        Directory.CreateDirectory(packageDirectory);
        Directory.CreateDirectory(disposableFeed);
        try
        {
            var cache = Path.Combine(work, "private-nuget-cache");
            var project = Path.Combine(producer, "composition-release-rehearsal.csproj");
            File.WriteAllText(project, CreateProducerProject(root), new UTF8Encoding(false));
            var config = Path.Combine(producer, "NuGet.Config");
            File.WriteAllText(config, Config(("published-base", feed)), new UTF8Encoding(false));
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
                "--output", packageDirectory, "-p:NuGetAudit=false"
            ]).Receipt);

            var builtPackage = Path.Combine(
                packageDirectory, PackageId + "." + Version + ".nupkg");
            Need(File.Exists(builtPackage), "actual compiled Composition archive was not created");
            var rehearsalArchiveSha = Sha(builtPackage);
            var rehearsalDllSha = PackageDllSha(
                builtPackage, PackageId, Version, Dependencies);
            var repository = new DotNetLocalPackageRepository(disposableFeed,
                new DotNetLocalRepositoryLimits(256, 67_108_864, 67_108_864));
            var prepared = repository.PreparePublication(
                builtPackage, rehearsalArchiveSha, CancellationToken.None,
                ignoreSubdirectories: true);
            Need(!File.Exists(prepared.Receipt.Path),
                 "shared PreparePublication unexpectedly wrote an archive");
            var published = repository.Publish(prepared, CancellationToken.None);
            Need(published.PackageId == PackageId &&
                 published.Version == Version &&
                 published.Sha256 == rehearsalArchiveSha &&
                 Sha(published.Path) == rehearsalArchiveSha &&
                 PackageDllSha(published.Path, PackageId, Version, Dependencies) ==
                     rehearsalDllSha,
                 "shared published immutable package bytes or declared dependency differ");
            var inventory = repository.List(PackageId);
            Need(inventory.InspectedPackages == 1 &&
                 inventory.MatchedPackages == 1 &&
                 inventory.Packages.Single().Sha256 == rehearsalArchiveSha,
                 "shared NuGet reader rejected the disposable published Composition archive");
            ExpectRefusal(() => repository.PreparePublication(
                builtPackage, rehearsalArchiveSha, CancellationToken.None,
                ignoreSubdirectories: true), "collision");
            ExpectRefusal(() => repository.PreparePublication(
                builtPackage, new string('0', 64), CancellationToken.None,
                ignoreSubdirectories: true), "digest_mismatch");
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancellationRefused = false;
            try
            {
                _ = repository.PreparePublication(
                    builtPackage, rehearsalArchiveSha, cancelled.Token,
                    ignoreSubdirectories: true);
            }
            catch (OperationCanceledException)
            {
                cancellationRefused = true;
            }
            Need(cancellationRefused && Sha(published.Path) == rehearsalArchiveSha,
                 "cancellation did not refuse or altered the disposable Composition archive");

            commands.AddRange(RunConsumer(
                root, work, disposableFeed, feed, rehearsalDllSha));
            var publishedBase = Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg");
            Need(IsPhysical(publishedBase, 21926, BaseArchiveSha) &&
                 PackageDllSha(publishedBase, BaseId, BaseVersion, [(QuantitiesId, "[" + QuantitiesVersion + "]")]) == BaseDllSha &&
                 IsPhysical(Path.Combine(feed, QuantitiesId + "." + QuantitiesVersion + ".nupkg"),
                     14747, QuantitiesArchiveSha) &&
                 Sha(published.Path) == rehearsalArchiveSha,
                 "physical base archive or disposable Composition archive drifted during rehearsal");
            var sdk = Run(root, cache, "sdk", "dotnet", ["--version"]);
            commands.Add(sdk.Receipt);
            Need(commands.Count == 7, "exact seven inner command receipts required");

            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.composition-immutable-local-publisher-rehearsal",
                status = "composition-dependent-package-publisher-rehearsal-qualified",
                packageId = PackageId,
                version = Version,
                historicalSourceVersion = SourceVersion,
                rs164CandidateArchiveSha256 = CandidateSha,
                rs164CandidateDllSha256 = CandidateDllSha,
                rehearsalArchiveSha256 = rehearsalArchiveSha,
                rehearsalDllSha256 = rehearsalDllSha,
                exactDependencyId = BaseId,
                directQuantitiesId = QuantitiesId,
                directQuantitiesVersion = QuantitiesVersion,
                directQuantitiesArchiveSha256 = QuantitiesArchiveSha,
                directQuantitiesDllSha256 = QuantitiesDllSha,
                exactDependencyVersion = BaseVersion,
                exactDependencyArchiveSha256 = BaseArchiveSha,
                exactDependencyDllSha256 = BaseDllSha,
                transitiveQuantityId = QuantitiesId,
                transitiveQuantityVersion = QuantitiesVersion,
                transitiveQuantityArchiveSha256 = QuantitiesArchiveSha,
                transitiveQuantityDllSha256 = QuantitiesDllSha,
                dependencyCount = 2,
                publisher = "Dw.Tools.NuGet/[20260.20261026.1010]",
                publisherDllSha256 = SharedNuGetDllSha,
                transitiveDependencyUnchanged = true,
                controls = 7,
                consumerChecks = 15,
                disposableFeedPublished = true,
                publishedToNugetLocal = false,
                collisionRefused = true,
                alteredDigestRefused = true,
                cancelledRefused = cancellationRefused,
                sourceFilesUnchanged = true,
                sourceProjectVersionUnchanged = true,
                sourceProjectReferencePreserved = true,
                publishedDependencyUnchanged = true,
                sourceReleaseAuthorization = false,
                publicLicenseSelected = false,
                installedPackageRollbackQualified = false,
                commandReceipts = commands
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
        Need(IsPhysical(candidate, 17101, CandidateSha) &&
             PackageDllSha(candidate, PackageId, Version, Dependencies) ==
                 CandidateDllSha,
             "only the original 17,101-byte RS164 Composition candidate may be published");
        Need(DateOnly.FromDateTime(DateTime.Now) >= new DateOnly(2026, 11, 7),
             "physical publication date cannot predate original package creation date");
        var records = RootFile(root, "build/artifacts/rs165-composition-release/receipts");
        var receiptFile = Path.Combine(records, PackageId + "." + Version + ".json");
        Need(!File.Exists(receiptFile),
             "a previous private Composition publication receipt cannot be replaced");
        var reader = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = reader.PreparePublication(
            candidate, CandidateSha, CancellationToken.None,
            ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == PackageId &&
             prepared.Receipt.Version == Version &&
             prepared.Receipt.Sha256 == CandidateSha,
             "shared publisher prepared a different Composition archive");
        var delivered = reader.Publish(prepared, CancellationToken.None);
        Need(Sha(delivered.Path) == CandidateSha &&
             PackageDllSha(delivered.Path, PackageId, Version, Dependencies) ==
                 CandidateDllSha,
             "physically delivered Composition archive differs from exact RS164 candidate");

        Directory.CreateDirectory(records);
        using (var stream = new FileStream(
            receiptFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.composition-private-nuget-local-publication",
                status = "published-local-awaiting-independent-dwf-verification",
                packageId = PackageId,
                version = Version,
                archiveSha256 = CandidateSha,
                compositionDllSha256 = CandidateDllSha,
                directDependencyCount = Dependencies.Length,
                directDependencies = new[]
                {
                    new
                    {
                        id = QuantitiesId,
                        version = QuantitiesVersion,
                        archiveSha256 = QuantitiesArchiveSha,
                        packagedDllSha256 = QuantitiesDllSha
                    },
                    new
                    {
                        id = BaseId,
                        version = BaseVersion,
                        archiveSha256 = BaseArchiveSha,
                        packagedDllSha256 = BaseDllSha
                    }
                },
                feedSelector = "NUGET_LOCAL",
                independentlyReadFromPublishedFeed = false,
                remoteRegistryPublished = false,
                installedPackageRollbackQualified = false
            }, Pretty);
            stream.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-COMPOSITION-PUBLISHED|id=" + PackageId +
            "|version=" + Version + "|sha256=" + CandidateSha +
            "|dependencies=" + QuantitiesId + "@" + QuantitiesVersion +
            "," + BaseId + "@" + BaseVersion +
            "|feed=NUGET_LOCAL|verification=pending");
    }

    private static void VerifyPublished(string root, string feed)
    {
        RequireQualifiedRunAndCleanCheckout(root);
        var published = Path.Combine(feed, PackageId + "." + Version + ".nupkg");
        Need(IsPhysical(published, 17101, CandidateSha) &&
             PackageDllSha(published, PackageId, Version, Dependencies) ==
                 CandidateDllSha,
             "shared-feed Composition package must match the original RS164 candidate exactly");
        var work = RootFile(root, "build/artifacts/rs165-composition-verify-" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "verification workspace already exists");
        Directory.CreateDirectory(work);
        try
        {
            var commands = RunConsumer(root, work, feed, feed, CandidateDllSha);
            Need(commands.Count == 3 && Sha(published) == CandidateSha,
                 "fresh published Composition package-only consumer failed or archive changed");
            Console.WriteLine("MATH-COMPOSITION-VERIFY|id=" + PackageId +
                "|version=" + Version + "|binary=15|archive=" + CandidateSha +
                "|dependencies=" + QuantitiesId + "@" + QuantitiesVersion +
                "," + BaseId + "@" + BaseVersion +
                "|feed=NUGET_LOCAL|PASS");
        }
        finally
        {
            if (Directory.Exists(work))
                Directory.Delete(work, recursive: true);
        }
    }

    private static List<object> RunConsumer(
        string root, string work, string irFeed, string dependencyFeed, string dllSha)
    {
        var consumer = Path.Combine(work, "binary-consumer");
        Directory.CreateDirectory(consumer);
        File.Copy(RootFile(root, ConsumerFixture),
            Path.Combine(consumer, "Program.cs"), overwrite: false);
        var project = Path.Combine(consumer, "composition-consumer.csproj");
        var xml = new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
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
                    new XAttribute("Version", "[" + Version + "]")))));
        File.WriteAllText(project, xml.ToString(SaveOptions.DisableFormatting) + "\n",
            new UTF8Encoding(false));
        var config = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(config,
            string.Equals(irFeed, dependencyFeed, StringComparison.OrdinalIgnoreCase)
                ? Config(("published", irFeed))
                : Config(("disposable", irFeed), ("published", dependencyFeed)),
            new UTF8Encoding(false));
        var cache = Path.Combine(work, "private-nuget-cache");
        var commands = new List<object>();
        commands.Add(Run(root, cache, "consumer-restore", "dotnet", [
            "restore", project, "--configfile", config, "--packages", cache,
            "-p:NuGetAudit=false"
        ]).Receipt);
        commands.Add(Run(root, cache, "consumer-build", "dotnet", [
            "build", project, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"
        ]).Receipt);
        var executed = Run(root, cache, "consumer-binary", "dotnet", [
            "run", "--project", project, "-c", "Release",
            "--no-build", "--no-restore", "--", dllSha
        ]);
        commands.Add(executed.Receipt);
        Need(executed.Output.Contains("MATH-COMPOSITION-BINARY|PASS|15",
                 StringComparison.Ordinal) &&
             executed.Output.Split("MATH-COMPOSITION-CHECK|").Length - 1 == 15 &&
             executed.Output.Contains("MATH-COMPOSITION-CHECK|loaded-composition-dll|PASS",
                 StringComparison.Ordinal) &&
             executed.Output.Contains("MATH-COMPOSITION-CHECK|replay-cache-bounds|PASS",
                 StringComparison.Ordinal),
             "fresh package-only Composition consumer must pass all 15 independent controls");
        return commands;
    }

    private static string CreateProducerProject(string root)
    {
        var project = new XDocument(
            new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("AssemblyName", PackageId),
                    new XElement("RootNamespace", "Dw.Tools.Math.Composition"),
                    new XElement("PackageId", PackageId),
                    new XElement("Version", SourceVersion),
                    new XElement("PackageVersion", Version),
                    new XElement("Authors", "dw.tools.math"),
                    new XElement("Description",
                        "Inspectable mathematical operation results and capabilities, provider-neutral."),
                    new XElement("IsPackable", "true"),
                    new XElement("ManagePackageVersionsCentrally", "false"),
                    new XElement("RestorePackagesWithLockFile", "false"),
                    new XElement("EnableDefaultCompileItems", "false")),
                new XElement("ItemGroup",
                    OriginalSourceNames.Select(name => new XElement("Compile",
                        new XAttribute("Include", RootFile(root, SourceDirectory + "/" + name)),
                        new XAttribute("Link", name))),
                    new XElement("PackageReference",
                        new XAttribute("Include", QuantitiesId),
                        new XAttribute("Version", "[" + QuantitiesVersion + "]")),
                    new XElement("PackageReference",
                        new XAttribute("Include", BaseId),
                        new XAttribute("Version", "[" + BaseVersion + "]")))));
        return project.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static string Config(params (string Key, string Path)[] feeds)
    {
        var xml = new XDocument(new XElement("configuration",
            new XElement("packageSources", new XElement("clear"),
                feeds.Select(f => new XElement("add",
                    new XAttribute("key", f.Key),
                    new XAttribute("value", f.Path))))));
        return xml.ToString(SaveOptions.DisableFormatting) + "\n";
    }

    private static void RequireQualifiedRunAndCleanCheckout(string root)
    {
        using var report = Load(RootFile(root, Rs165Report));
        var run = report.RootElement;
        Need(Value(run, "runId") == "RS165" &&
             Value(run, "outcome") == "succeeded" &&
             run.GetProperty("attempt").GetInt32() == 1 &&
             Value(run.GetProperty("git"), "commitMessage") ==
                 "test: qualify Composition immutable NuGet publisher rehearsal with two dependencies",
             "successful exact RS165 DWF publisher rehearsal required");
        using var attestation = Load(RootFile(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        var rights = attestation.RootElement.GetProperty("authorization");
        Need(rights.GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "private Composition package publication requires source-owner rights attestation");
        var cache = RootFile(root, "build/artifacts/rs165-composition-operator-cache");
        var branch = Run(root, cache, "git-branch", "git",
            ["branch", "--show-current"]).Output.Trim();
        var status = Run(root, cache, "git-status", "git",
            ["status", "--porcelain", "--untracked-files=normal"]).Output.Trim();
        Need(branch == "main" && status.Length == 0,
             "explicit private Composition publication requires clean main checkout");
    }

    private static string RequireExistingFeed()
    {
        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL") ??
            throw new InvalidDataException("NUGET_LOCAL must select the existing private feed");
        Need(!string.IsNullOrWhiteSpace(feed) &&
             Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
             "NUGET_LOCAL must be an existing absolute directory");
        return feed;
    }

    private static bool IsPhysical(string path, int bytes, string expectedSha) =>
        File.Exists(path) &&
        (File.GetAttributes(path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
        new FileInfo(path).Length == bytes &&
        Sha(path) == expectedSha;

    private static string PackageDllSha(
        string archivePath, string id, string version,
        IReadOnlyList<(string Id, string Version)> dependencies)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(e => e.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(e => !e.FullName.Contains("..", StringComparison.Ordinal) &&
                 !e.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !e.FullName.Contains('\\')),
             "real NuGet archive ZIP entries are not bounded and canonical");
        var nuspec = archive.GetEntry(id + ".nuspec") ??
            throw new InvalidDataException("exact Composition NuGet manifest missing");
        using (var stream = nuspec.Open())
        {
            var manifest = XDocument.Load(stream, LoadOptions.None);
            Need(manifest.Descendants().Any(e => e.Name.LocalName == "id" &&
                    e.Value == id) &&
                 manifest.Descendants().Any(e => e.Name.LocalName == "version" &&
                    e.Value == version),
                 "package manifest ID or version is not the pinned qualified value");
            var actual = manifest.Descendants()
                .Where(e => e.Name.LocalName == "dependency")
                .Select(e => (Id: (string?)e.Attribute("id") ?? "",
                    Version: (string?)e.Attribute("version") ?? "")).ToArray();
            Need(actual.SequenceEqual(dependencies),
                 "exact Composition package dependency closure must be unchanged");
        }
        var dll = archive.GetEntry("lib/net10.0/" + id + ".dll") ??
            throw new InvalidDataException("net10.0 Composition package DLL is missing");
        using var input = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(input));
    }

    private static void ExpectRefusal(Action action, string code)
    {
        var refused = false;
        try { action(); }
        catch (DotNetLocalRepositoryException error) when (error.Code == code)
        {
            refused = true;
        }
        Need(refused, "shared publisher did not refuse " + code);
    }

    private static JsonDocument Load(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Value(JsonElement source, string name) =>
        source.GetProperty(name).GetString() ??
        throw new InvalidDataException("Missing exact original Composition identity " + name);

    private static string RootFile(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool okay, string message)
    {
        if (!okay) throw new InvalidDataException(message);
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
        Need(process.Start(), "cannot launch Composition publisher command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(240000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Composition publisher command timed out: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var diagnostics = output + "\n" + errors;
            throw new InvalidDataException(id + " exit=" + process.ExitCode +
                "\n" + diagnostics[..System.Math.Min(diagnostics.Length, 4200)]);
        }
        return (output, new
        {
            id, exit_code = process.ExitCode,
            command = executable + " " + string.Join(" ", args),
            elapsed_ms = watch.ElapsedMilliseconds,
            output_excerpt = output[..System.Math.Min(output.Length, 1100)]
        });
    }
}
