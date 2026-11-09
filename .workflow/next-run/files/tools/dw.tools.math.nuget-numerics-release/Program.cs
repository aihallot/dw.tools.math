using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetNumericsRelease;

internal static class Program
{
    private const string Id = "dw.tools.math.numerics";
    private const string Version = "20260.20261109.1001";
    private const string ArchiveSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
    private const string DllSha = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";
    private const int ArchiveBytes = 18313;
    private const string IrId = "dw.tools.math.ir";
    private const string IrVersion = "20260.20261104.1001";
    private const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
    private const string QuantitiesId = "dw.quantities";
    private const string QuantitiesVersion = "20260.20261102.1001";
    private const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string NugetDllSha = "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string F35 = "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json";
    private const string F36 = "docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json";
    private const string Fixture = "tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs";
    private const string SourceProject = "src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj";
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            Need(args.Length == 1 && args[0] is ("--qualify" or "--publish" or "--verify"),
                "Expected one of --qualify, --publish or --verify.");
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
                ?? throw new InvalidDataException("NUGET_LOCAL is required");
            Need(Path.IsPathFullyQualified(feed) && Directory.Exists(feed),
                "NUGET_LOCAL must identify an existing absolute private feed");
            var candidate = ValidateAuthority(root, feed);
            switch (args[0])
            {
                case "--qualify": Qualify(root, feed, candidate); break;
                case "--publish": Publish(root, feed, candidate); break;
                case "--verify": Verify(root, feed); break;
            }
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUMERICS-F37-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static string ValidateAuthority(string root, string feed)
    {
        using var f35 = Read(Root(root, F35));
        using var f36 = Read(Root(root, F36));
        using var prior = Read(Root(root, ".aura/workflow/reports/RS169/attempt-002.json"));
        var qualified = f35.RootElement.GetProperty("candidate");
        var rehearsal = f36.RootElement.GetProperty("rehearsal");
        var history = prior.RootElement;
        Need(Text(history, "runId") == "RS169" &&
             history.GetProperty("attempt").GetInt32() == 2 &&
             Text(history, "outcome") == "succeeded" &&
             history.GetProperty("correctionMode").GetBoolean() &&
             history.GetProperty("correctionSourceAttempt").GetInt32() == 1 &&
             Text(history.GetProperty("executor"), "toolVersion") == "0.1.66" &&
             Text(history.GetProperty("git"), "commitMessage") ==
                 "test: rehearse Numerics immutable NuGet publisher with IR and Quantities" &&
             history.GetProperty("priorInvocationFailures")
                 .GetProperty("count").GetInt32() == 1 &&
             !history.GetProperty("priorInvocationFailures")
                 .GetProperty("firstPassSucceeded").GetBoolean(),
             "RS169 corrected success identity must be preserved");
        Need(Text(f35.RootElement, "status") ==
                 "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
             !f35.RootElement.GetProperty("package_published").GetBoolean() &&
             Text(qualified, "packageId") == Id &&
             Text(qualified, "packageVersion") == Version &&
             Text(qualified, "historicalSourceVersion") == "0.4.0-preview.4" &&
             qualified.GetProperty("candidateArchiveBytes").GetInt32() == ArchiveBytes &&
             Text(qualified, "candidateArchiveSha256") == ArchiveSha &&
             Text(qualified, "candidateDllSha256") == DllSha &&
             qualified.GetProperty("compiledBinaryConsumerChecks").GetInt32() == 15 &&
             qualified.GetProperty("packageDependencies").GetInt32() == 1 &&
             !qualified.GetProperty("published").GetBoolean() &&
             Text(qualified, "dependencyIrId") == IrId &&
             Text(qualified, "dependencyIrVersion") == IrVersion &&
             Text(qualified, "dependencyIrArchiveSha256") == IrSha &&
             Text(qualified, "dependencyQuantitiesId") == QuantitiesId &&
             Text(qualified, "dependencyQuantitiesVersion") == QuantitiesVersion &&
             Text(qualified, "dependencyQuantitiesArchiveSha256") == QuantitiesSha,
             "immutable F35 original Numerics candidate identity changed");
        Need(Text(f36.RootElement, "status") ==
                 "rs169-numerics-immutable-disposable-rehearsal-qualified-unpublished" &&
             !f36.RootElement.GetProperty("shared_package_published").GetBoolean() &&
             !f36.RootElement.GetProperty("owner_release_authorized").GetBoolean() &&
             Text(rehearsal, "originalF35ArchiveSha256") == ArchiveSha &&
             Text(rehearsal, "originalF35DllSha256") == DllSha &&
             Text(rehearsal, "rehearsalArchiveSha256") != ArchiveSha &&
             !rehearsal.GetProperty("reconstructionIsOriginalArchiveProof").GetBoolean() &&
             rehearsal.GetProperty("disposableFeedPublished").GetBoolean() &&
             !rehearsal.GetProperty("sharedFeedPublished").GetBoolean() &&
             rehearsal.GetProperty("independentCompiledBinaryChecks").GetInt32() == 15,
             "F36 rehearsal must not be substituted for immutable F35 original");
        Need(Sha(Root(root, SourceProject)) == Text(qualified, "originalProjectSha256") &&
             Sha(Root(root, "src/projects/dw.tools.math.numerics/packages.lock.json")) ==
                 Text(qualified, "originalSourceLockSha256"),
             "Numerics original project/lockfile changed");
        var sources = qualified.GetProperty("originalSourcesSha256").EnumerateObject().ToArray();
        Need(sources.Length == 4 &&
             sources.All(s => File.Exists(Root(root, s.Name)) &&
                 Sha(Root(root, s.Name)) == s.Value.GetString()) &&
             File.Exists(Root(root, Fixture)),
             "three original Numerics C# sources or source-free consumer changed");
        Need(Sha(Path.Combine(feed, IrId + "." + IrVersion + ".nupkg")) == IrSha &&
             Sha(Path.Combine(feed, QuantitiesId + "." + QuantitiesVersion + ".nupkg")) == QuantitiesSha,
             "exact published IR and Quantities dependency archives changed");
        var readerAssembly = typeof(DotNetLocalPackageRepository).Assembly;
        Need(string.Equals(readerAssembly.GetName().Name, "dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) &&
             Sha(readerAssembly.Location) == NugetDllSha,
             "pinned shared NuGet repository library changed");
        var relative = Text(qualified, "candidateArchivePath").Replace('\\', '/');
        var prefix = "build/artifacts/rs168-numerics-candidate/scratch/";
        var suffix = "/candidate/" + Id + "." + Version + ".nupkg";
        Need(relative.StartsWith(prefix, StringComparison.Ordinal) &&
             relative.EndsWith(suffix, StringComparison.Ordinal) &&
             relative.Length == prefix.Length + 32 + suffix.Length &&
             relative.Substring(prefix.Length, 32).All(c =>
                 c is >= '0' and <= '9' or >= 'a' and <= 'f'),
             "original F35 archive location is not its qualified scratch identity");
        return Root(root, relative);
    }

    private static void Qualify(string root, string feed, string original)
    {
        var scratch = Root(root, "build/artifacts/rs170-numerics-controller/" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(scratch), "qualification workspace must be fresh");
        Directory.CreateDirectory(scratch);
        var invalid = Path.Combine(scratch, "not-original.nupkg");
        File.WriteAllText(invalid, "not-an-original-F35-NuGet-archive\n");
        Need(!IsOriginal(invalid),
             "altered candidate must be refused before any publication");
        var originalPresent = File.Exists(original);
        var originalVerified = originalPresent && IsOriginal(original);
        var ir = Path.Combine(feed, IrId + "." + IrVersion + ".nupkg");
        var quantities = Path.Combine(feed, QuantitiesId + "." + QuantitiesVersion + ".nupkg");
        Need(Sha(ir) == IrSha && Sha(quantities) == QuantitiesSha,
             "qualification cannot mutate previously published dependency bytes");
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            status = "qualified-numerics-immutable-release-controller-unpublished",
            packageId = Id,
            version = Version,
            originalF35ArchiveSha256 = ArchiveSha,
            originalF35DllSha256 = DllSha,
            originalF35ArchiveBytes = ArchiveBytes,
            sourceProjectPreserved = true,
            f36RehearsalNotOriginal = true,
            malformedCandidateRefused = true,
            originalCandidatePhysicallyObserved = originalPresent,
            originalCandidatePhysicallyVerified = originalVerified,
            sharedFeedPublishedByThisRun = false,
            sharedPublicationAuthorized = false,
            privatePublishGateEnabled = true,
            privateVerifyGateEnabled = true,
            noOverwritePublisherRequired = true,
            directDependencyId = IrId,
            directDependencyVersion = IrVersion,
            directDependencyArchiveSha256 = IrSha,
            transitiveDependencyId = QuantitiesId,
            transitiveDependencyVersion = QuantitiesVersion,
            transitiveDependencyArchiveSha256 = QuantitiesSha,
            ownerReleaseReadiness = originalVerified
                ? "original-archive-observed-not-authorized"
                : "original-archive-not-verified",
            nextAction = "Owner must explicitly authorize publication of only the exact F35 original archive; F36 rehearsal bytes are not substitutes."
        }, Pretty));
    }

    private static void Publish(string root, string feed, string original)
    {
        RequireQualifiedRunAndCleanMain(root);
        Need(IsOriginal(original),
             "only exact F35 original Numerics archive may be privately published");
        var receipts = Root(root, "build/artifacts/rs170-numerics-controller/receipts");
        var receipt = Path.Combine(receipts, Id + "." + Version + ".json");
        Need(!File.Exists(receipt), "prior Numerics publication receipt cannot be overwritten");
        var repo = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = repo.PreparePublication(original, ArchiveSha,
            ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == Id &&
             prepared.Receipt.Version == Version &&
             prepared.Receipt.Sha256 == ArchiveSha,
             "shared publisher has not selected exact original F35 candidate");
        var published = repo.Publish(prepared);
        Need(IsOriginal(published.Path),
             "published private archive differs from original F35 bytes");
        Directory.CreateDirectory(receipts);
        using (var stream = new FileStream(receipt,
            FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(stream, new
            {
                schemaVersion = 1,
                status = "published-local-awaiting-independent-dwf-verification",
                packageId = Id,
                version = Version,
                archiveSha256 = ArchiveSha,
                numericsDllSha256 = DllSha,
                dependencyId = IrId,
                dependencyVersion = IrVersion,
                transitiveDependencyId = QuantitiesId,
                transitiveDependencyVersion = QuantitiesVersion,
                feedSelector = "NUGET_LOCAL",
                independentlyReadFromPublishedFeed = false,
                remoteRegistryPublished = false,
                installedRollbackQualified = false
            }, Pretty);
            stream.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-NUMERICS-PUBLISHED|id=" + Id + "|version=" +
            Version + "|sha256=" + ArchiveSha + "|dependency=" + IrVersion +
            "|feed=NUGET_LOCAL|verification=pending");
    }

    private static void Verify(string root, string feed)
    {
        RequireQualifiedRunAndCleanMain(root);
        var package = Path.Combine(feed, Id + "." + Version + ".nupkg");
        Need(IsOriginal(package),
             "published package must match exact original F35 Numerics archive");
        var work = Root(root, "build/artifacts/rs170-numerics-verify/" +
            Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "fresh verification workspace required");
        var consumer = Path.Combine(work, "consumer");
        var cache = Path.Combine(work, "cache");
        Directory.CreateDirectory(consumer);
        File.Copy(Root(root, Fixture), Path.Combine(consumer, "Program.cs"),
            overwrite: false);
        var project = Path.Combine(consumer, "numerics-binary-consumer.csproj");
        File.WriteAllText(project, new XDocument(new XElement("Project",
            new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",
                new XElement("OutputType", "Exe"),
                new XElement("TargetFramework", "net10.0"),
                new XElement("IsPackable", "false"),
                new XElement("ManagePackageVersionsCentrally", "false"),
                new XElement("EnableDefaultCompileItems", "false"),
                new XElement("RestorePackagesWithLockFile", "false")),
            new XElement("ItemGroup",
                new XElement("Compile", new XAttribute("Include", "Program.cs")),
                new XElement("PackageReference",
                    new XAttribute("Include", Id),
                    new XAttribute("Version", "[" + Version + "]")))))
            .ToString(SaveOptions.DisableFormatting) + "\n");
        var config = Path.Combine(consumer, "NuGet.Config");
        File.WriteAllText(config, new XDocument(
            new XElement("configuration",
                new XElement("packageSources", new XElement("clear"),
                    new XElement("add", new XAttribute("key", "private"),
                        new XAttribute("value", feed)))))
            .ToString(SaveOptions.DisableFormatting) + "\n");
        Run(root, cache, "consumer-restore", [
            "restore", project, "--configfile", config,
            "--packages", cache, "-p:NuGetAudit=false"]);
        Run(root, cache, "consumer-build", [
            "build", project, "-c", "Release", "--no-restore",
            "-p:NuGetAudit=false"]);
        var observation = Run(root, cache, "consumer-binary", [
            "run", "--project", project, "-c", "Release",
            "--no-build", "--no-restore", "--", DllSha]);
        Need(observation.Contains("MATH-NUMERICS-BINARY|PASS|15", StringComparison.Ordinal) &&
             observation.Split("MATH-NUMERICS-CHECK|").Length - 1 == 15 &&
             IsOriginal(package),
             "published Numerics binary-only consumer or physical readback failed");
        Console.WriteLine("MATH-NUMERICS-VERIFY|id=" + Id + "|version=" + Version +
            "|binary=15|archive=" + ArchiveSha + "|feed=NUGET_LOCAL|PASS");
    }

    private static void RequireQualifiedRunAndCleanMain(string root)
    {
        var reports = Root(root, ".aura/workflow/reports/RS170");
        Need(Directory.Exists(reports), "successful RS170 report required");
        var successes = Directory.EnumerateFiles(reports, "attempt-*.json")
            .Select(path => Read(Path.GetFullPath(path)))
            .ToArray();
        try
        {
            Need(successes.Count(doc =>
                Text(doc.RootElement, "runId") == "RS170" &&
                Text(doc.RootElement, "outcome") == "succeeded" &&
                Text(doc.RootElement.GetProperty("git"), "commitMessage") ==
                    "test: qualify Numerics immutable private release controller") == 1,
                "exact successful RS170 controller qualification required");
        }
        finally { foreach (var doc in successes) doc.Dispose(); }
        using var rights = Read(Root(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        Need(rights.RootElement.GetProperty("authorization")
                 .GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.RootElement.GetProperty("authorization")
                 .GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "source-owner rights must precede private release");
        var cache = Root(root, "build/artifacts/rs170-numerics-controller/git-cache");
        Need(Run(root, cache, "git-branch", ["branch", "--show-current"], "git")
                 .Trim() == "main" &&
             Run(root, cache, "git-status",
                 ["status", "--porcelain", "--untracked-files=normal"], "git")
                 .Trim().Length == 0,
             "release requires clean operator main");
    }

    private static bool IsOriginal(string archive) =>
        File.Exists(archive) &&
        (File.GetAttributes(archive) &
            (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0 &&
        new FileInfo(archive).Length == ArchiveBytes &&
        Sha(archive) == ArchiveSha &&
        PackagedDllSha(archive) == DllSha;

    private static string PackagedDllSha(string file)
    {
        using var archive = ZipFile.OpenRead(file);
        Need(archive.Entries.Count is >= 3 and <= 48 &&
             archive.Entries.Select(x => x.FullName)
                 .Distinct(StringComparer.Ordinal).Count() == archive.Entries.Count &&
             archive.Entries.All(x => !x.FullName.Contains("..", StringComparison.Ordinal) &&
                 !x.FullName.StartsWith("/", StringComparison.Ordinal) &&
                 !x.FullName.Contains('\\')),
             "candidate ZIP manifest paths refused");
        var nuspec = archive.GetEntry(Id + ".nuspec")
            ?? throw new InvalidDataException("exact Numerics nuspec missing");
        using (var input = nuspec.Open())
        {
            var xml = XDocument.Load(input, LoadOptions.None);
            Need(xml.Descendants().Any(x => x.Name.LocalName == "id" &&
                 x.Value == Id) &&
                 xml.Descendants().Any(x => x.Name.LocalName == "version" &&
                 x.Value == Version), "original Numerics nuspec identity mismatch");
            var dependencies = xml.Descendants()
                .Where(x => x.Name.LocalName == "dependency")
                .Select(x => (
                    Id: (string?)x.Attribute("id") ?? "",
                    Version: (string?)x.Attribute("version") ?? ""))
                .ToArray();
            Need(dependencies.SequenceEqual(new[] {
                 (Id: IrId, Version: "[" + IrVersion + "]") }),
                 "only exact original IR direct dependency is admissible");
        }
        var dll = archive.GetEntry("lib/net10.0/" + Id + ".dll")
            ?? throw new InvalidDataException("Numerics net10.0 assembly missing");
        using var content = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(content));
    }

    private static string Run(string root, string cache, string id,
        string[] args, string executable = "dotnet")
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
        Need(process.Start(), "could not launch NuGet release command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(300000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("release command timeout: " + id);
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        Need(process.ExitCode == 0, "release command " + id + " failed: " +
            (output + "\n" + errors)[..System.Math.Min((output + "\n" + errors).Length, 1600)]);
        return output;
    }

    private static JsonDocument Read(string path) =>
        JsonDocument.Parse(File.ReadAllText(path));

    private static string Text(JsonElement node, string name) =>
        node.GetProperty(name).GetString() ??
        throw new InvalidDataException("missing required metadata " + name);

    private static string Root(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static void Need(bool okay, string reason)
    {
        if (!okay) throw new InvalidDataException(reason);
    }
}
