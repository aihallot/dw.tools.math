using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Dw.Tools.NuGet;

namespace Dw.Tools.Math.NuGetPreflight;

internal static partial class Program
{
    private sealed record SourceProject(
        string Id, string Version, string RelativePath, string[] References,
        string? DeclaredLicense, string? DeclaredPackageVersion);
    private static readonly Regex CalendarVersion = new(
        @"^[0-9]{5}\.[0-9]{8}\.[0-9]{4}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions Indented = new()
    {
        WriteIndented = true
    };

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "--self-test")
            {
                SelfTest();
                return 0;
            }
            if (args.Length == 2 && args[0] == "--inspect")
            {
                Inspect(args[1]);
                return 0;
            }
            Console.Error.WriteLine("Expected --self-test or --inspect <repository-root>.");
            return 2;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("MATH-NUGET-F1-REFUSED: " + error.Message);
            return 1;
        }
    }

    private static void SelfTest()
    {
        var date = new DateOnly(2026, 11, 2);
        const string package = "dw.quantities";
        var initial = DotNetCalendarPackageVersion.Calculate(
            new DotNetCalendarVersionRequest(package, date), []);
        Need(initial is DotNetCalendarVersionOutcome.Success start &&
             start.Result.Version == "20260.20261102.1001",
             "first calendar sequence");

        var next = DotNetCalendarPackageVersion.Calculate(
            new DotNetCalendarVersionRequest(package, date),
            [new DotNetPackageVersionIdentity(package, "20260.20261102.1001")]);
        Need(next is DotNetCalendarVersionOutcome.Success successor &&
             successor.Result.Version == "20260.20261102.1002",
             "second calendar sequence");

        var legacy = DotNetCalendarPackageVersion.Calculate(
            new DotNetCalendarVersionRequest(package, date),
            [new DotNetPackageVersionIdentity(package, "0.2.0-preview.1")]);
        Need(legacy is DotNetCalendarVersionOutcome.Failure
             { Kind: DotNetCalendarVersionFailureKind.InvalidHistory },
             "legacy version must not be reinterpreted as calendar history");

        var future = DotNetCalendarPackageVersion.Calculate(
            new DotNetCalendarVersionRequest(package, date),
            [new DotNetPackageVersionIdentity(package, "20260.20261103.1001")]);
        Need(future is DotNetCalendarVersionOutcome.Failure
             { Kind: DotNetCalendarVersionFailureKind.ClockRegression },
             "future-date refusal");

        var different = DotNetCalendarPackageVersion.Calculate(
            new DotNetCalendarVersionRequest("dw.quantities.standard", date),
            [new DotNetPackageVersionIdentity(package, "20260.20261102.1001")]);
        Need(different is DotNetCalendarVersionOutcome.Success independent &&
             independent.Result.Version == "20260.20261102.1001",
             "version sequence must be independent by package id");

        Console.WriteLine("MATH-NUGET-F1-SELFTEST|PASS|5");
    }

    private static void Inspect(string repositoryRoot)
    {
        var root = Path.GetFullPath(repositoryRoot);
        Need(Directory.Exists(Path.Combine(root, "src", "projects")), "missing source project root");
        var sources = LoadSources(root);
        Need(sources.Length == 13, "exactly thirteen qualified Math libraries are expected");
        var ids = sources.Select(p => p.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        Need(ids.Count == 13, "duplicate Math package id");
        foreach (var project in sources)
            foreach (var reference in project.References)
                Need(ids.Contains(reference), "unresolved package dependency: " + project.Id + " -> " + reference);

        var feed = Environment.GetEnvironmentVariable("NUGET_LOCAL")
            ?? throw new InvalidDataException(
                "NUGET_LOCAL must name one existing absolute local feed");
        Need(!string.IsNullOrWhiteSpace(feed) && Path.IsPathFullyQualified(feed) &&
             Directory.Exists(feed), "NUGET_LOCAL must name one existing absolute local feed");
        var packagesCache = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
        if (packagesCache is not null && !string.IsNullOrWhiteSpace(packagesCache))
        {
            Need(!string.Equals(Path.GetFullPath(feed),
                     Path.GetFullPath(packagesCache), StringComparison.OrdinalIgnoreCase),
                 "NUGET_PACKAGES is a cache, not the shared publication feed");
        }

        const string sharedPackageName = "dw.tools.nuget.20260.20261026.1010.nupkg";
        const string sharedPackageSha =
            "60836cc1feb3b468f8e9c991059458d79f3f575789da5a7a81042327427972bf";
        var sharedPackagePath = Path.Combine(feed, sharedPackageName);
        Need(File.Exists(sharedPackagePath), "published shared NuGet package is unavailable");
        Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(sharedPackagePath))) ==
             sharedPackageSha, "shared NuGet package archive identity drifted");
        var assembly = typeof(DotNetCalendarPackageVersion).Assembly;
        Need(string.Equals(assembly.GetName().Name, "dw.tools.nuget",
             StringComparison.OrdinalIgnoreCase), "unexpected shared package assembly");
        var loadedCoreSha = Convert.ToHexStringLower(
            SHA256.HashData(File.ReadAllBytes(assembly.Location)));
        Need(loadedCoreSha ==
             "b16d2e96f45862fbb0feb11d85b58d4a3a452555cd06444e3d6d53285de9bb61",
             "actual loaded published core DLL SHA-256 drifted");

        var limits = new DotNetLocalRepositoryLimits(
            MaximumItems: 4096, MaximumPackageBytes: 67_108_864,
            MaximumTotalBytes: 1_073_741_824);
        _ = new DotNetLocalPackageRepository(feed, limits);
        var inventory = ScopePackages(feed, root, ids, limits);
        var date = DateOnly.FromDateTime(DateTime.Now);
        Need(date.Year == 2026, "future-year calendar generation needs a separate owner decision");

        var rows = new List<object>();
        foreach (var project in sources)
        {
            var properties = EffectiveProperties(root, project.RelativePath);
            Need(properties["PackageId"] == project.Id &&
                 properties["Version"] == project.Version &&
                 properties["IsPackable"].Equals("true", StringComparison.OrdinalIgnoreCase) &&
                 properties["TargetFramework"] == "net10.0",
                 "evaluated Math package identity changed: " + project.Id);
            Need(properties["PackageVersion"] == (project.DeclaredPackageVersion ?? project.Version),
                 "evaluated PackageVersion is not the qualified preview baseline: " + project.Id);

            var matching = inventory.Packages.Where(p =>
                p.PackageId.Equals(project.Id, StringComparison.OrdinalIgnoreCase)).ToArray();
            var dated = matching.Where(p => CalendarVersion.IsMatch(p.Version)).ToArray();
            var history = dated.Select(p => new DotNetPackageVersionIdentity(
                p.PackageId, p.Version)).ToArray();
            var proposal = DotNetCalendarPackageVersion.Calculate(
                new DotNetCalendarVersionRequest(project.Id, date), history);
            Need(proposal is DotNetCalendarVersionOutcome.Success,
                 "calendar version refused by shared library for " + project.Id);
            var selected = ((DotNetCalendarVersionOutcome.Success)proposal).Result;

            rows.Add(new
            {
                packageId = project.Id,
                sourceProject = project.RelativePath,
                historicalSourceVersion = project.Version,
                effectivePackageVersion = properties["PackageVersion"],
                effectiveTargetFramework = properties["TargetFramework"],
                effectivePackable = properties["IsPackable"],
                effectiveLicenseExpression = properties["PackageLicenseExpression"],
                candidateVersionPreview = selected.Version,
                candidateIsReserved = false,
                projectReferences = project.References,
                externalPackageReferences = Array.Empty<string>(),
                historicalArchives = matching.Where(p => !CalendarVersion.IsMatch(p.Version))
                    .Select(p => new { p.Version, p.Sha256, p.Bytes }).ToArray(),
                calendarArchives = dated.Select(p => new { p.Version, p.Sha256, p.Bytes }).ToArray()
            });
        }

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            schemaVersion = 1,
            status = "f1-source-and-feed-inventory-qualified-not-published",
            feedSource = "NUGET_LOCAL",
            inspectionScope = "thirteen-Math-package-id-prefixes-top-level-only",
            repositoryTool = "Dw.Tools.NuGet",
            repositoryPackageVersion = "20260.20261026.1010",
            repositoryPackageSha256 = sharedPackageSha,
            loadedLibrarySha256 = loadedCoreSha,
            inspectedArchives = inventory.InspectedPackages,
            inventorySha256 = inventory.InventorySha256,
            observedDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            msbuildPropertyQueries = sources.Length,
            packageCount = rows.Count,
            versionsReserved = false,
            feedPublished = false,
            packages = rows
        }, Indented));
    }

    private static SourceProject[] LoadSources(string root)
    {
        var projects = Directory.EnumerateFiles(
                Path.Combine(root, "src", "projects"), "*.csproj",
                SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal).ToArray();
        Need(projects.Length == 13, "source project count changed");
        var result = new List<SourceProject>();
        foreach (var path in projects)
        {
            var xml = XDocument.Load(path, LoadOptions.None);
            string? Property(string name) =>
                xml.Descendants().Where(x => x.Name.LocalName == name)
                   .Select(x => x.Value.Trim()).FirstOrDefault();
            var id = Property("PackageId");
            var version = Property("Version");
            Need(id is { Length: > 0 } && version is { Length: > 0 } &&
                 Property("IsPackable") == "true",
                 "non-packable or unspecified source package: " + path);
            var references = xml.Descendants()
                .Where(e => e.Name.LocalName == "ProjectReference")
                .Select(e => Path.GetFileNameWithoutExtension(
                    (string?)e.Attribute("Include") ??
                    throw new InvalidDataException("Missing project reference identity")))
                .OrderBy(x => x, StringComparer.Ordinal).ToArray();
            Need(!xml.Descendants().Any(e => e.Name.LocalName == "PackageReference"),
                 "runtime package dependency requires separate F1 qualification: " + id);
            result.Add(new SourceProject(id!, version!,
                Path.GetRelativePath(root, path).Replace('\\', '/'),
                references, Property("PackageLicenseExpression"),
                Property("PackageVersion")));
        }
        return result.OrderBy(p => p.Id, StringComparer.Ordinal).ToArray();
    }

    private static Dictionary<string, string> EffectiveProperties(string root, string relativeProject)
    {
        var output = RunProcess(root, "dotnet", ["msbuild", relativeProject,
            "-nologo",
            "-getProperty:PackageId,PackageVersion,Version,IsPackable,TargetFramework,PackageLicenseExpression"]);
        var first = output.IndexOf('{');
        var last = output.LastIndexOf('}');
        Need(first >= 0 && last >= first, "MSBuild effective package properties were not JSON");
        using var document = JsonDocument.Parse(output[first..(last + 1)]);
        var json = document.RootElement;
        var properties = json.TryGetProperty("Properties", out var nested) ? nested : json;
        var names = new[]
        {
            "PackageId", "PackageVersion", "Version", "IsPackable",
            "TargetFramework", "PackageLicenseExpression"
        };
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var name in names)
            values[name] = properties.TryGetProperty(name, out var value) &&
                           value.ValueKind == JsonValueKind.String ? value.GetString()! : "";
        return values;
    }

    private static DotNetLocalPackageInventory ScopePackages(
        string feed, string root, HashSet<string> ids, DotNetLocalRepositoryLimits limits)
    {
        var owned = Path.Combine(root, ".workflow", "local", "nuget-f1-rs134");
        Directory.CreateDirectory(owned);
        var scratch = Path.Combine(owned, "scope-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scratch);
        var reader = new DotNetLocalPackageRepository(scratch, limits);
        try
        {
            int visited = 0, selected = 0;
            long bytes = 0;
            foreach (var path in Directory.EnumerateFileSystemEntries(feed))
            {
                Need(++visited <= limits.MaximumItems, "shared feed top-level inventory limit");
                var name = Path.GetFileName(path);
                if (!name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase) ||
                    !ids.Any(id => name.StartsWith(id + ".", StringComparison.OrdinalIgnoreCase)))
                    continue;
                var attributes = File.GetAttributes(path);
                Need((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) == 0,
                     "selected package is not a regular archive: " + name);
                var length = new FileInfo(path).Length;
                Need(length <= limits.MaximumPackageBytes &&
                     length <= limits.MaximumTotalBytes - bytes,
                     "selected package exceeds bounded snapshot limits");
                bytes += length;
                File.Copy(path, Path.Combine(scratch, name), overwrite: false);
                selected++;
            }
            var inventory = reader.List(ignoreSubdirectories: true);
            Need(inventory.InspectedPackages == selected, "scoped package count drift");
            foreach (var package in inventory.Packages)
            {
                Need(ids.Contains(package.PackageId), "NuGet package ID outside Math scope");
                Need(Path.GetFileName(package.Path).Equals(
                    package.PackageId + "." + package.Version + ".nupkg",
                    StringComparison.OrdinalIgnoreCase), "archive name and nuspec identity mismatch");
            }
            return inventory;
        }
        finally
        {
            Directory.Delete(scratch, recursive: true);
        }
    }

    private static string RunProcess(string directory, string executable, string[] arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executable, WorkingDirectory = directory,
                UseShellExecute = false, RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        foreach (var argument in arguments)
            process.StartInfo.ArgumentList.Add(argument);
        Need(process.Start(), "unable to start dotnet msbuild property inspection");
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(120000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("MSBuild property inspection timed out");
        }
        var output = stdout.GetAwaiter().GetResult();
        var errors = stderr.GetAwaiter().GetResult();
        Need(process.ExitCode == 0, "MSBuild property inspection failed: " +
             errors[..System.Math.Min(errors.Length, 1500)]);
        return output;
    }

    private static void Need(bool value, string message)
    {
        if (!value) throw new InvalidDataException(message);
    }
}
