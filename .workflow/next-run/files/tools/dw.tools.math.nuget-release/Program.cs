using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetRelease;

internal static class Program
{
    private const string PackageId = "dw.quantities";
    private const string QualifiedVersion = "20260.20261102.1001";
    private const string SourceVersion = "0.2.0-preview.1";
    private const string QualifiedArchiveSha256 =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string QualifiedDllSha256 =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string NuGetCoreSha256 =
        "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61";
    private const string CandidateRelativePath =
        "build/artifacts/rs135-quantities-candidate/candidate/dw.quantities.20260.20261102.1001.nupkg";
    private const string ConsumerFixture =
        "tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs";
    private const string SourceProject =
        "src/projects/dw.quantities/dw.quantities.csproj";
    private const string OwnerReceipt =
        "docs/distribution/nuget-federation-f2-quantities-qualified.json";
    private const string ReleaseReport =
        ".aura/workflow/reports/RS136/attempt-001.json";
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || args[0] is not ("--rehearse" or "--publish" or "--verify"))
            {
                Console.Error.WriteLine("Use --rehearse, --publish or --verify.");
                return 2;
            }
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            VerifyCommonAuthority(root);
            switch (args[0])
            {
                case "--rehearse":
                    Rehearse(root);
                    break;
                case "--publish":
                    Publish(root);
                    break;
                case "--verify":
                    Verify(root);
                    break;
            }
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUGET-RELEASE-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void VerifyCommonAuthority(string root)
    {
        var projectPath = RootPath(root, SourceProject);
        Need(File.Exists(projectPath) && File.Exists(RootPath(root, ConsumerFixture)),
            "qualified Math source and independent consumer fixture are required");
        var project = XDocument.Load(projectPath, LoadOptions.None);
        string? Property(string name) => project.Descendants()
            .Where(e => e.Name.LocalName == name)
            .Select(e => e.Value.Trim()).FirstOrDefault();
        Need(Property("PackageId") == PackageId &&
             Property("Version") == SourceVersion &&
             Property("IsPackable") == "true" &&
             !project.Descendants().Any(e => e.Name.LocalName is
                 "ProjectReference" or "PackageReference"),
             "owner source package identity or dependency scope drifted");
        using var evidence = ReadJson(RootPath(root, OwnerReceipt));
        var owner = evidence.RootElement;
        var candidate = owner.GetProperty("candidate");
        Need(Text(owner, "status") ==
             "rs135-quantities-calendar-candidate-qualified-unpublished" &&
             Text(candidate, "sourcePackageId") == PackageId &&
             Text(candidate, "packageVersion") == QualifiedVersion &&
             Text(candidate, "candidateArchivePath") == CandidateRelativePath &&
             Text(candidate, "candidateArchiveSha256") == QualifiedArchiveSha256 &&
             candidate.GetProperty("candidateArchiveBytes").GetInt64() == 14747 &&
             Text(candidate, "candidateDllSha256") == QualifiedDllSha256 &&
             candidate.GetProperty("consumerChecks").GetInt32() == 4 &&
             candidate.GetProperty("compiledSourceFreeConsumerPassed").GetBoolean() &&
             !owner.GetProperty("package_published").GetBoolean(),
             "immutable RS135 owner receipt is not the qualified binary candidate");
        var loaded = typeof(DotNetLocalPackageRepository).Assembly;
        Need(loaded.GetName().Name?.Equals("dw.tools.nuget",
                 StringComparison.OrdinalIgnoreCase) == true &&
             Sha(loaded.Location) == NuGetCoreSha256,
             "loaded NuGet core assembly is not the exact qualified publisher");
    }

    private static void Rehearse(string root)
    {
        // The test NEVER passes NUGET_LOCAL to the publication API.
        // A genuine Math package is packed and published only into a fresh
        // disposable feed whose path is derived from a newly created GUID.
        var localFeed = ApprovedFeed();
        var work = Path.Combine(root, "build", "artifacts",
            "rs136-rehearsal-" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(work), "rehearsal target already exists");
        var packageFeed = Path.Combine(work, "candidate");
        var disposable = Path.Combine(work, "disposable-feed");
        Directory.CreateDirectory(packageFeed);
        Directory.CreateDirectory(disposable);
        try
        {
            var commands = new List<object>();
            commands.Add(Run(root, "source-restore", "dotnet", [
                "restore", SourceProject, "--locked-mode", "--source", localFeed,
                "-p:NuGetAudit=false"
            ], work).Receipt);
            commands.Add(Run(root, "source-build", "dotnet", [
                "build", SourceProject, "-c", "Release", "--no-restore",
                "-p:NuGetAudit=false"
            ], work).Receipt);
            commands.Add(Run(root, "source-pack", "dotnet", [
                "pack", SourceProject, "-c", "Release", "--no-build", "--no-restore",
                "-p:PackageVersion=" + QualifiedVersion,
                "--output", packageFeed, "-p:NuGetAudit=false"
            ], work).Receipt);

            var archive = Path.Combine(packageFeed, PackageId + "." + QualifiedVersion + ".nupkg");
            Need(File.Exists(archive), "real source package rehearsal archive missing");
            var rehearsalSha = Sha(archive);
            var rehearsalDll = InspectArchive(archive, QualifiedVersion);
            var pub = new DotNetLocalPackageRepository(disposable,
                new DotNetLocalRepositoryLimits(256, 67_108_864, 67_108_864));
            var planned = pub.PreparePublication(archive, rehearsalSha,
                CancellationToken.None, ignoreSubdirectories: true);
            Need(!File.Exists(planned.Receipt.Path), "rehearsal prepare unexpectedly wrote an archive");
            var published = pub.Publish(planned, CancellationToken.None);
            Need(published.PackageId == PackageId &&
                 published.Version == QualifiedVersion &&
                 published.Sha256 == rehearsalSha &&
                 Sha(published.Path) == rehearsalSha &&
                 InspectArchive(published.Path, QualifiedVersion) == rehearsalDll,
                 "shared NuGet publisher failed exact-byte readback");
            ExpectRefusal(() => pub.PreparePublication(archive, rehearsalSha,
                CancellationToken.None, ignoreSubdirectories: true), "collision");
            ExpectRefusal(() => pub.PreparePublication(archive, new string('0', 64),
                CancellationToken.None, ignoreSubdirectories: true), "digest_mismatch");
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancellationRefused = false;
            try
            {
                _ = pub.PreparePublication(archive, rehearsalSha, cancelled.Token,
                    ignoreSubdirectories: true);
            }
            catch (OperationCanceledException)
            {
                cancellationRefused = true;
            }
            Need(cancellationRefused, "shared publisher did not refuse pre-cancelled preparation");
            Need(Sha(published.Path) == rehearsalSha, "rehearsal refusal changed published bytes");
            var inspected = pub.List(PackageId);
            Need(inspected.MatchedPackages == 1 && inspected.InspectedPackages == 1,
                 "rehearsal published inventory must contain exactly one archive");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.nuget-release-controller-rehearsal",
                status = "real-math-package-disposable-publisher-qualified",
                packageId = PackageId,
                packageVersion = QualifiedVersion,
                sourceVersion = SourceVersion,
                sourceArchiveSha256 = QualifiedArchiveSha256,
                rehearsalArchiveSha256 = rehearsalSha,
                rehearsalDllSha256 = rehearsalDll,
                publisher = "Dw.Tools.NuGet/[20260.20261026.1010]",
                publisherDllSha256 = NuGetCoreSha256,
                controls = 6,
                candidateIsPublishedToSharedFeed = false,
                sharedFeedTouched = false,
                publicationModeEnabledByDwf = false,
                collisionRefused = true,
                alteredHashRefused = true,
                cancellationRefused,
                publishedInDisposableFeed = true,
                physicalRollbackQualified = false,
                commands
            }, Indented));
        }
        finally
        {
            // Only this GUID-owned disposable workspace is deleted.
            if (Directory.Exists(work))
                Directory.Delete(work, recursive: true);
        }
    }

    private static void Publish(string root)
    {
        // This explicit operator action is never invoked by DWF run next.
        VerifyAuthorizedRelease(root);
        Need(DateOnly.FromDateTime(DateTime.Now).ToString("yyyyMMdd",
             System.Globalization.CultureInfo.InvariantCulture) ==
             QualifiedVersion.Split('.')[1],
             "candidate version date is stale: requalify a new calendar package");
        var candidate = RootPath(root, CandidateRelativePath);
        Need(File.Exists(candidate) && new FileInfo(candidate).Length == 14747 &&
             Sha(candidate) == QualifiedArchiveSha256 &&
             InspectArchive(candidate, QualifiedVersion) == QualifiedDllSha256,
             "the exact RS135 archive and DLL must still physically exist unchanged");

        var feed = ApprovedFeed();
        var releaseFolder = Path.Combine(root, "build", "artifacts", "rs136-publisher-receipts");
        var receiptPath = Path.Combine(releaseFolder,
            PackageId + "." + QualifiedVersion + ".json");
        Need(!File.Exists(receiptPath), "this release already has a local receipt");
        var publisher = new DotNetLocalPackageRepository(feed,
            new DotNetLocalRepositoryLimits(4096, 67_108_864, 1_073_741_824));
        var prepared = publisher.PreparePublication(candidate, QualifiedArchiveSha256,
            CancellationToken.None, ignoreSubdirectories: true);
        Need(prepared.Receipt.PackageId == PackageId &&
             prepared.Receipt.Version == QualifiedVersion &&
             prepared.Receipt.Sha256 == QualifiedArchiveSha256,
             "publication prepared a different immutable package identity");
        var delivered = publisher.Publish(prepared, CancellationToken.None);
        Need(Sha(delivered.Path) == QualifiedArchiveSha256 &&
             InspectArchive(delivered.Path, QualifiedVersion) == QualifiedDllSha256,
             "physical shared-feed publication must rehash to the qualified bytes");
        Directory.CreateDirectory(releaseFolder);
        using (var output = new FileStream(receiptPath, FileMode.CreateNew,
                   FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(output, new
            {
                schemaVersion = 1,
                kind = "dw.tools.math.immutable-local-nuget-publication",
                status = "published-local-awaiting-independent-dwf-verification",
                packageId = PackageId,
                version = QualifiedVersion,
                sha256 = QualifiedArchiveSha256,
                dllSha256 = QualifiedDllSha256,
                feedSelector = "NUGET_LOCAL",
                physicallyPublished = true,
                physicallyVerifiedBytes = true,
                remotelyPublished = false,
                ownerRightsBasis = "M0-W02-C01-owner-rights-attestation",
                installedPackageRollbackQualified = false
            }, Indented);
            output.WriteByte((byte)'\n');
        }
        Console.WriteLine("MATH-NUGET-PUBLISHED|id=" + PackageId +
            "|version=" + QualifiedVersion + "|sha256=" + QualifiedArchiveSha256 +
            "|feed=NUGET_LOCAL|verification=pending");
    }

    private static void Verify(string root)
    {
        VerifyAuthorizedRelease(root);
        var feed = ApprovedFeed();
        var archive = Path.Combine(feed, PackageId + "." + QualifiedVersion + ".nupkg");
        Need(File.Exists(archive) && Sha(archive) == QualifiedArchiveSha256 &&
             InspectArchive(archive, QualifiedVersion) == QualifiedDllSha256,
             "published Math archive or DLL does not match the qualified candidate");
        var temp = Path.Combine(root, "build", "artifacts",
            "rs136-verify-" + Guid.NewGuid().ToString("N"));
        Need(!Directory.Exists(temp), "verification workspace already exists");
        Directory.CreateDirectory(temp);
        try
        {
            var project = Path.Combine(temp, "consumer.csproj");
            var source = Path.Combine(temp, "Program.cs");
            var config = Path.Combine(temp, "NuGet.Config");
            File.Copy(RootPath(root, ConsumerFixture), source, overwrite: false);
            File.WriteAllText(project, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <IsPackable>false</IsPackable>
                    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                  </PropertyGroup>
                  <ItemGroup>
                    <Compile Include="Program.cs" />
                    <PackageReference Include="dw.quantities" Version="[20260.20261102.1001]" />
                  </ItemGroup>
                </Project>
                """, new UTF8Encoding(false));
            File.WriteAllText(config,
                "<configuration><packageSources><clear /></packageSources></configuration>\n",
                new UTF8Encoding(false));
            var operations = new[]
            {
                Run(root, "consumer-restore", "dotnet", [
                    "restore", project, "--configfile", config, "--source", feed,
                    "-p:NuGetAudit=false"
                ], temp),
                Run(root, "consumer-build", "dotnet", [
                    "build", project, "-c", "Release", "--no-restore",
                    "-p:NuGetAudit=false"
                ], temp),
                Run(root, "consumer-run", "dotnet", [
                    "run", "--project", project, "-c", "Release", "--no-build",
                    "--no-restore", "--", QualifiedDllSha256
                ], temp)
            };
            Need(operations[2].Output.Contains("MATH-QUANTITIES-BINARY|PASS|4",
                 StringComparison.Ordinal), "published package-only consumer failed");
            Console.WriteLine("MATH-NUGET-VERIFY|id=" + PackageId +
                "|version=" + QualifiedVersion + "|binary=4|archive=" +
                QualifiedArchiveSha256 + "|feed=NUGET_LOCAL|PASS");
        }
        finally
        {
            if (Directory.Exists(temp))
                Directory.Delete(temp, recursive: true);
        }
    }

    private static void VerifyAuthorizedRelease(string root)
    {
        using var report = ReadJson(RootPath(root, ReleaseReport));
        var result = report.RootElement;
        Need(Text(result, "runId") == "RS136" &&
             Text(result, "outcome") == "succeeded" &&
             result.GetProperty("attempt").GetInt32() == 1 &&
             Text(result.GetProperty("git"), "commitMessage") ==
                 "test: qualify Math immutable local NuGet publisher rehearsal",
             "successful RS136 DWF result is required before physical release");
        using var owner = ReadJson(RootPath(root,
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json"));
        var rights = owner.RootElement.GetProperty("authorization");
        Need(rights.GetProperty("package_from_dw_tools_math").GetBoolean() &&
             rights.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
             "owner package and redistribution rights are not attested");

        var branch = Run(root, "git-branch", "git", ["branch", "--show-current"],
            Path.Combine(root, "build", "artifacts")).Output.Trim();
        var dirty = Run(root, "git-clean", "git", [
            "status", "--porcelain", "--untracked-files=normal"
        ], Path.Combine(root, "build", "artifacts")).Output.Trim();
        Need(branch == "main" && dirty.Length == 0,
             "explicit publication requires clean Math main checkout");
    }

    private static string ApprovedFeed()
    {
        var value = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException("NUGET_LOCAL must name an approved local feed");
        Need(!string.IsNullOrWhiteSpace(value) && Path.IsPathFullyQualified(value) &&
             Directory.Exists(value), "NUGET_LOCAL is not an existing absolute directory");
        return value;
    }

    private static JsonDocument ReadJson(string path) => JsonDocument.Parse(File.ReadAllText(path));

    private static string Text(JsonElement root, string key) =>
        root.GetProperty(key).GetString() ??
        throw new InvalidDataException("Required owner identity missing: " + key);

    private static string RootPath(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    private static void Need(bool ok, string message)
    {
        if (!ok) throw new InvalidDataException(message);
    }

    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

    private static string InspectArchive(string packagePath, string version)
    {
        using var archive = ZipFile.OpenRead(packagePath);
        Need(archive.Entries.Count is > 2 and <= 32 &&
             archive.Entries.Select(e => e.FullName).Distinct(StringComparer.Ordinal)
                 .Count() == archive.Entries.Count &&
             !archive.Entries.Any(e => e.FullName.Contains("..", StringComparison.Ordinal) ||
                e.FullName.StartsWith("/", StringComparison.Ordinal) ||
                e.FullName.Contains('\\')),
             "unsafe ZIP entry layout");
        var metadata = archive.GetEntry("dw.quantities.nuspec")
            ?? throw new InvalidDataException("NuGet manifest missing");
        using (var input = metadata.Open())
        {
            var xml = XDocument.Load(input, LoadOptions.None);
            Need(xml.Descendants().Any(e => e.Name.LocalName == "id" &&
                 e.Value == PackageId) &&
                 xml.Descendants().Any(e => e.Name.LocalName == "version" &&
                 e.Value == version) &&
                 !xml.Descendants().Any(e => e.Name.LocalName == "dependency"),
                 "package metadata identity or zero-dependency boundary changed");
        }
        var dll = archive.GetEntry("lib/net10.0/dw.quantities.dll")
            ?? throw new InvalidDataException("net10.0 Math library missing");
        using var dllStream = dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(dllStream));
    }

    private static void ExpectRefusal(Action action, string code)
    {
        var refused = false;
        try
        {
            action();
        }
        catch (DotNetLocalRepositoryException error) when (error.Code == code)
        {
            refused = true;
        }
        Need(refused, "publisher did not refuse " + code);
    }

    private static (string Output, object Receipt) Run(
        string root, string id, string command, string[] args, string workspace)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command, WorkingDirectory = root,
                UseShellExecute = false, RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
        process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        process.StartInfo.Environment["NUGET_PACKAGES"] =
            Path.Combine(workspace, "private-packages");
        foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
        var watch = Stopwatch.StartNew();
        Need(process.Start(), "could not start bounded command " + id);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(180000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("bounded command timed out: " + id);
        }
        var outText = stdout.GetAwaiter().GetResult();
        var errorText = stderr.GetAwaiter().GetResult();
        watch.Stop();
        if (process.ExitCode != 0)
        {
            var output = outText + "\n" + errorText;
            throw new InvalidOperationException(id + " failed exit=" +
                process.ExitCode + "\n" +
                output[..System.Math.Min(output.Length, 3000)]);
        }
        return (outText, new
        {
            id, exit_code = process.ExitCode,
            elapsed_ms = watch.ElapsedMilliseconds,
            command = command + " " + string.Join(" ", args),
            output_excerpt = outText[..System.Math.Min(outText.Length, 900)]
        });
    }
}
