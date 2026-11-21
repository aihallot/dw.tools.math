using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetBoundedStatisticsRelease;

internal static class Program
{
    private const string PackageId = "dw.tools.math.statistics";
    private const string Version = "20260.20261120.1001";
    private const string BaseId = "dw.quantities";
    private const string BaseVersion = "20260.20261102.1001";
    private const string BaseSha =
        "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
    private const string BaseDllSha =
        "c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4";
    private const string OldStatisticsSha =
        "9232edaedf57f5f67530905398648fa240141146cb55eed14e7910d54717e4f8";
    private const string Approval =
        "docs/planning/decisions/m9-bounded-statistics-private-publish-authorization.md";
    private const string CandidateRecipe =
        "docs/engineering/ProbeM9StatisticsBoundedNugetCandidate.cs";
    private const string IndependentVerifier =
        "docs/engineering/VerifyM9BoundedStatisticsPrivateFeed.cs";
    private const string ReceiptPath =
        "build/artifacts/m9-bounded-statistics-publication/receipts/20260.20261120.1001.json";

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || args[0] is not ("--rehearse" or "--publish" or "--verify"))
                throw new ArgumentException("Expected --rehearse, --publish, or --verify.");
            var root = Path.GetFullPath(Directory.GetCurrentDirectory());
            var feed = RequirePrivateFeed();
            VerifyOldArchives(feed);
            switch (args[0])
            {
                case "--rehearse": Rehearse(root, feed); break;
                case "--publish": Publish(root, feed); break;
                case "--verify": Verify(root, feed); break;
            }
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("MATH-M9-PRIVATE-PUBLISH-REFUSED: " + ex.Message);
            return 1;
        }
    }

    private static void Rehearse(string root, string feed)
    {
        Need(!File.Exists(Target(feed)),
            "The private version has already been published; rehearsal cannot assume absent target.");
        var candidate = RecreateQualifiedCandidate(root, feed);
        var work = Path.Combine(root, "build/artifacts/m9-rs272-private-rehearsal-feed");
        // Disposable workspace only: no shared feed path is ever deleted or overwritten.
        if (Directory.Exists(work)) Directory.Delete(work, recursive: true);
        Directory.CreateDirectory(work);
        File.Copy(Path.Combine(feed, BaseId + "." + BaseVersion + ".nupkg"),
            Path.Combine(work, BaseId + "." + BaseVersion + ".nupkg"));
        var expectedArchive = Sha(candidate);
        var expectedDll = PackageDllSha(candidate);
        var repository = Repository(work);
        var prepared = repository.PreparePublication(
            candidate, expectedArchive, CancellationToken.None, ignoreSubdirectories: true);
        Need(!File.Exists(prepared.Receipt.Path), "PreparePublication changed disposable feed.");
        var published = repository.Publish(prepared, CancellationToken.None);
        Need(published.PackageId == PackageId && published.Version == Version &&
            published.Sha256 == expectedArchive && Sha(published.Path) == expectedArchive,
            "Shared NuGet publisher changed rehearsal archive.");
        IndependentVerify(root, work, expectedArchive, expectedDll);
        var collisionRefused = false;
        try
        {
            repository.PreparePublication(
                candidate, expectedArchive, CancellationToken.None, ignoreSubdirectories: true);
        }
        catch (DotNetLocalRepositoryException ex) when (ex.Code == "collision")
        {
            collisionRefused = true;
        }
        Need(collisionRefused, "Publisher did not refuse immutable-version collision.");
        Need(!File.Exists(Target(feed)), "Rehearsal wrote to shared NUGET_LOCAL.");
        VerifyOldArchives(feed);
        Console.WriteLine("MATH-RS272-PRIVATE-REHEARSAL|version=" + Version +
            "|sha256=" + expectedArchive + "|checks=12|collisionRefused=true|" +
            "sharedFeedUnchanged=true|published=false|PASS");
    }

    private static void Publish(string root, string feed)
    {
        RequireAuthorizedCleanMain(root);
        Need(!File.Exists(Target(feed)),
            "Target private NuGet version already exists; refuse overwrite even if hashes match.");
        var receiptPath = Path.Combine(root, ReceiptPath);
        Need(!File.Exists(receiptPath), "Existing publication receipt is immutable; use --verify.");
        var candidate = RecreateQualifiedCandidate(root, feed);
        var archiveSha = Sha(candidate);
        var dllSha = PackageDllSha(candidate);
        var repository = Repository(feed);
        var prepared = repository.PreparePublication(
            candidate, archiveSha, CancellationToken.None, ignoreSubdirectories: true);
        Need(!File.Exists(prepared.Receipt.Path), "PreparePublication must not write archive.");
        Directory.CreateDirectory(Path.GetDirectoryName(receiptPath)!);
        // A pending receipt permits recovery/verification without replaying a publication.
        using (var stream = new FileStream(receiptPath, FileMode.CreateNew,
            FileAccess.Write, FileShare.None))
            JsonSerializer.Serialize(stream, new PublicationReceipt(
                PackageId, Version, archiveSha, dllSha, BaseId, BaseVersion, BaseSha,
                "prepared-awaiting-publication-readback"));
        var published = repository.Publish(prepared, CancellationToken.None);
        Need(published.PackageId == PackageId && published.Version == Version &&
            published.Sha256 == archiveSha && Sha(published.Path) == archiveSha,
            "Published archive differs from exact qualified candidate.");
        IndependentVerify(root, feed, archiveSha, dllSha);
        VerifyOldArchives(feed);
        WriteVerifiedReceipt(receiptPath, archiveSha, dllSha);
        Console.WriteLine("MATH-M9-STATISTICS-PUBLISHED|id=" + PackageId +
            "|version=" + Version + "|sha256=" + archiveSha +
            "|dllSha256=" + dllSha + "|feed=NUGET_LOCAL|independentNet11=PASS");
    }

    private static void Verify(string root, string feed)
    {
        RequireAuthorizedCleanMain(root);
        var receiptPath = Path.Combine(root, ReceiptPath);
        Need(File.Exists(receiptPath),
            "Publication receipt absent; refuse to infer an authorized publication.");
        var receipt = ReadReceipt(receiptPath);
        Need(receipt.PackageId == PackageId && receipt.Version == Version &&
            receipt.DependencyId == BaseId && receipt.DependencyVersion == BaseVersion &&
            receipt.DependencySha256 == BaseSha,
            "Publication receipt identity differs.");
        Need(File.Exists(Target(feed)) && Sha(Target(feed)) == receipt.ArchiveSha256 &&
            PackageDllSha(Target(feed)) == receipt.DllSha256,
            "Published private archive differs from pending receipt.");
        IndependentVerify(root, feed, receipt.ArchiveSha256, receipt.DllSha256);
        VerifyOldArchives(feed);
        WriteVerifiedReceipt(receiptPath, receipt.ArchiveSha256, receipt.DllSha256);
        Console.WriteLine("MATH-M9-STATISTICS-VERIFY|version=" + Version +
            "|sha256=" + receipt.ArchiveSha256 + "|net11=PASS|feed=NUGET_LOCAL");
    }

    private static void RequireAuthorizedCleanMain(string root)
    {
        Need(File.Exists(Approval), "Explicit private publication authorization is absent.");
        var approval = File.ReadAllText(Approval);
        Need(approval.Contains("20260.20261120.1001", StringComparison.Ordinal) &&
            approval.Contains("controlled private publication", StringComparison.Ordinal),
            "Unexpected publication authorization.");
        Need(File.Exists(".aura/workflow/reports/RS272/attempt-001.json"),
            "DWF RS272 must succeed before any shared feed publication.");
        using (var document=JsonDocument.Parse(File.ReadAllText(
            ".aura/workflow/reports/RS272/attempt-001.json")))
        {
            var run = document.RootElement;
            Need(run.GetProperty("runId").GetString() == "RS272" &&
                run.GetProperty("outcome").GetString() == "succeeded" &&
                run.GetProperty("git").GetProperty("commitMessage").GetString() ==
                    "test: qualify guarded private bounded Statistics publisher",
                "Expected durable RS272 publication rehearsal missing.");
        }
        using (var document=JsonDocument.Parse(File.ReadAllText(
            "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json")))
        {
            var rights = document.RootElement.GetProperty("authorization");
            Need(rights.GetProperty("package_from_dw_tools_math").GetBoolean() &&
                rights.GetProperty("redistribute_from_dw_tools_math").GetBoolean(),
                "Owner package and redistribution rights have not been attested.");
        }
        Need(Run(root, "git", ["branch","--show-current"]).Trim() == "main" &&
            Run(root,"git",["status","--porcelain","--untracked-files=normal"]).Trim().Length==0,
            "Real private publish requires clean main checkout.");
    }

    private static string RecreateQualifiedCandidate(string root, string feed)
    {
        Need(!File.Exists(Target(feed)), "Existing target private version cannot be replaced.");
        Need(File.Exists(CandidateRecipe), "RS269 qualified candidate recipe missing.");
        var output = Run(root, "dotnet", [
            "run","--file",CandidateRecipe,"--","--check"]);
        Need(output.Contains("MATH-RS269-CANDIDATE|", StringComparison.Ordinal) &&
            output.Contains("published=false", StringComparison.Ordinal),
            "Qualified RS269 candidate recipe failed.");
        var candidate = Path.Combine(root,
            "build/artifacts/m9-rs269-bounded-statistics-candidate/candidate/" +
            PackageId + "." + Version + ".nupkg");
        Need(File.Exists(candidate), "Qualified private release candidate archive missing.");
        _ = PackageDllSha(candidate);
        return candidate;
    }

    private static string PackageDllSha(string path)
    {
        using var zip=ZipFile.OpenRead(path);
        Need(zip.Entries.Count is >= 3 and <= 48 &&
            zip.Entries.All(x => !x.FullName.Contains("..",StringComparison.Ordinal) &&
                !x.FullName.StartsWith("/",StringComparison.Ordinal) &&
                !x.FullName.Contains('\\')),
            "Noncanonical or oversized package manifest.");
        var dll=zip.Entries.Single(x=>x.FullName=="lib/net10.0/"+PackageId+".dll");
        var nuspec=zip.Entries.Single(x=>x.FullName==PackageId+".nuspec");
        using(var stream=nuspec.Open())
        {
            var xml=XDocument.Load(stream);
            var ns=xml.Root!.Name.Namespace;
            var m=xml.Root.Element(ns+"metadata")!;
            Need(m.Element(ns+"id")?.Value==PackageId &&
                m.Element(ns+"version")?.Value==Version,
                "Candidate identity does not match approved version.");
            var deps=m.Element(ns+"dependencies")!.Descendants(ns+"dependency").ToArray();
            Need(deps.Length==1 && deps[0].Attribute("id")?.Value==BaseId &&
                deps[0].Attribute("version")?.Value=="["+BaseVersion+"]",
                "Unexpected candidate dependency.");
        }
        using var data=dll.Open();
        return Convert.ToHexStringLower(SHA256.HashData(data));
    }

    private static void IndependentVerify(
        string root, string feed, string archiveSha, string dllSha)
    {
        var output=Run(root,"dotnet",[
            "run","--file",IndependentVerifier,"--","--check",feed,archiveSha,dllSha,BaseSha]);
        Need(output.Contains("MATH-RS272-PRIVATE-READBACK|",StringComparison.Ordinal) &&
            output.Contains("|checks=12|",StringComparison.Ordinal) &&
            output.Contains("|PASS",StringComparison.Ordinal),
            "Independent physical package readback or net11 consumer refused.");
    }

    private static void VerifyOldArchives(string feed)
    {
        Need(Sha(Path.Combine(feed,PackageId+".20260.20261106.1001.nupkg")) ==
            OldStatisticsSha, "Historical published Statistics archive changed.");
        var baseArchive=Path.Combine(feed,BaseId+"."+BaseVersion+".nupkg");
        Need(Sha(baseArchive)==BaseSha,"Published Quantities dependency archive differs.");
        using var zip=ZipFile.OpenRead(baseArchive);
        using var dll=zip.Entries.Single(x=>x.FullName=="lib/net10.0/dw.quantities.dll").Open();
        Need(Convert.ToHexStringLower(SHA256.HashData(dll))==BaseDllSha,
            "Published Quantities DLL differs.");
    }
    private static DotNetLocalPackageRepository Repository(string feed) =>
        new(feed,new DotNetLocalRepositoryLimits(4096,67_108_864,1_073_741_824));
    private static string Target(string feed) =>
        Path.Combine(feed,PackageId+"."+Version+".nupkg");
    private static string RequirePrivateFeed()
    {
        var feed=Environment.GetEnvironmentVariable("NUGET_LOCAL");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
            Directory.Exists(feed),"NUGET_LOCAL must identify an existing absolute private feed.");
        return feed!;
    }
    private static void WriteVerifiedReceipt(string path,string archive,string dll)
    {
        // A receipt is local evidence only; real DWF independent qualification remains separate.
        File.WriteAllText(path,JsonSerializer.Serialize(new PublicationReceipt(
            PackageId,Version,archive,dll,BaseId,BaseVersion,BaseSha,
            "published-private-independently-readback-passed"))+"\n");
    }
    private static PublicationReceipt ReadReceipt(string path) =>
        JsonSerializer.Deserialize<PublicationReceipt>(File.ReadAllText(path)) ??
            throw new InvalidDataException("Invalid publication receipt.");
    private static string Sha(string path) =>
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    private static void Need(bool yes,string why)
    {
        if(!yes)throw new InvalidDataException(why);
    }
    private static string Run(string root,string exe,IEnumerable<string> args)
    {
        var start=new ProcessStartInfo(exe)
        {
            WorkingDirectory=root,UseShellExecute=false,
            RedirectStandardOutput=true,RedirectStandardError=true
        };
        foreach(var arg in args)start.ArgumentList.Add(arg);
        using var process=Process.Start(start)??throw new InvalidOperationException("Cannot start "+exe);
        var stdout=process.StandardOutput.ReadToEnd();
        var stderr=process.StandardError.ReadToEnd();
        if(!process.WaitForExit(180000) || process.ExitCode!=0)
            throw new InvalidOperationException(exe+" failed:\n"+stdout+"\n"+stderr);
        return stdout.Trim();
    }
    private sealed record PublicationReceipt(
        string PackageId,string Version,string ArchiveSha256,string DllSha256,
        string DependencyId,string DependencyVersion,string DependencySha256,string Status);
}
