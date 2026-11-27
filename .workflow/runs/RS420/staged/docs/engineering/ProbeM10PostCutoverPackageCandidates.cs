#:property PublishAot=false
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args is not ["--check"]) throw new ArgumentException("Expected --check.");
const string Version = "20260.20261127.1001-rs420";
var repository = Path.GetFullPath(".");
Need(Run(repository, "--version").Trim() == "11.0.100", "Actual SDK differs.");
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\', '/')).ToArray();
var projectPaths = solution.Where(x => x.StartsWith("src/projects/", StringComparison.Ordinal)).ToArray();
Need(solution.Length == 24 && projectPaths.Length == 13, "Actual source roster differs.");
var projects = projectPaths.ToDictionary(x => Path.GetFullPath(x), x => XDocument.Load(x), StringComparer.OrdinalIgnoreCase);
string Field(XDocument doc, string name) => doc.Descendants(name).Single().Value;
var idsByPath = projects.ToDictionary(x => x.Key, x => Field(x.Value, "PackageId"), StringComparer.OrdinalIgnoreCase);
var graph = new SortedDictionary<string, string[]>(StringComparer.Ordinal);
foreach (var (path, doc) in projects)
{
    var id = idsByPath[path];
    Need(id == Path.GetFileNameWithoutExtension(path) && Field(doc, "AssemblyName") == id &&
        Field(doc, "IsPackable") == "true" && !doc.Descendants("PackageReference").Any() &&
        !doc.Descendants("PackageVersion").Any(), "Unexpected source metadata: " + id);
    var deps = doc.Descendants("ProjectReference").Select(x => idsByPath[Path.GetFullPath(
        Path.Combine(Path.GetDirectoryName(path)!, (string)x.Attribute("Include")!))])
        .OrderBy(x => x, StringComparer.Ordinal).ToArray();
    Need(deps.Distinct(StringComparer.Ordinal).Count() == deps.Length, "Duplicate dependency.");
    graph.Add(id, deps);
}
Need(graph.Count == 13 && graph.Values.Sum(x => x.Length) == 16 &&
    graph["dw.tools.math.distribution"].Length == 6, "Package graph differs.");
var ordered = new List<string>();
var visiting = new HashSet<string>(StringComparer.Ordinal);
void Visit(string id)
{
    if (ordered.Contains(id, StringComparer.Ordinal)) return;
    Need(visiting.Add(id), "Cyclic dependency.");
    foreach (var dep in graph[id]) Visit(dep);
    visiting.Remove(id);
    ordered.Add(id);
}
foreach (var id in graph.Keys) Visit(id);
var shared = Environment.GetEnvironmentVariable("NUGET_LOCAL");
Need(!string.IsNullOrWhiteSpace(shared) && Path.IsPathFullyQualified(shared) && Directory.Exists(shared), "Shared feed missing.");
string Archive(string dir, string id) => Path.Combine(dir, id + "." + Version + ".nupkg");
Dictionary<string, string> Snapshot()
{
    var archives = Directory.EnumerateFiles(shared!, "*.nupkg", SearchOption.AllDirectories)
        .Where(x => graph.Keys.Any(id => Path.GetFileName(x).StartsWith(id + ".", StringComparison.OrdinalIgnoreCase)))
        .ToArray();
    Need(!archives.Any(x => graph.Keys.Any(id => string.Equals(Path.GetFileName(x), id + "." + Version + ".nupkg", StringComparison.OrdinalIgnoreCase))),
        "Candidate identity already exists in shared feed.");
    return archives.ToDictionary(x => x, Hash, StringComparer.OrdinalIgnoreCase);
}
var before = Snapshot();
Need(before.TryGetValue(Path.Combine(shared!, "dw.quantities.20260.20261102.1001.nupkg"), out var oldQ) &&
    oldQ == "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d" &&
    before.TryGetValue(Path.Combine(shared!, "dw.tools.math.statistics.20260.20261120.1001.nupkg"), out var oldS) &&
    oldS == "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1", "Historical archive identity differs.");
var root = Path.GetFullPath(Path.Combine("build", "artifacts", "m10-rs420-" + Guid.NewGuid().ToString("N")[..12]));
var producer = Path.Combine(root, "producer");
var feed = Path.Combine(root, "feed");
Directory.CreateDirectory(producer);
Directory.CreateDirectory(feed);
string Git(params string[] arguments)
{
    var info = new ProcessStartInfo("git") { WorkingDirectory = repository, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var arg in arguments) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("git unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(60000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("git timeout."); }
    var output = stdout.GetAwaiter().GetResult();
    Need(process.ExitCode == 0, stderr.GetAwaiter().GetResult());
    return output;
}
var tracked = Git("ls-files", "--", "src/projects", "docs/distribution/m5-package-readme.md", "docs/distribution/m5-package-notices.md")
    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
    .Where(x => !x.EndsWith("/packages.lock.json", StringComparison.Ordinal)).ToArray();
Need(projectPaths.All(x => tracked.Contains(x, StringComparer.Ordinal)), "Incomplete tracked source.");
var protectedFiles = tracked.Concat(projectPaths.Select(x => Path.Combine(Path.GetDirectoryName(x)!, "packages.lock.json")))
    .Concat(new[] { "global.json", "Directory.Build.props", "Directory.Packages.props", "NuGet.config" })
    .Distinct(StringComparer.Ordinal).ToDictionary(x => x, Hash, StringComparer.Ordinal);
foreach (var path in tracked)
{
    var dest = Path.Combine(producer, path);
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(path, dest);
    Need(Hash(dest) == protectedFiles[path], "Source copy differs: " + path);
}
File.Copy("global.json", Path.Combine(producer, "global.json"));
File.Copy("Directory.Build.props", Path.Combine(producer, "Directory.Build.props"));
File.WriteAllText(Path.Combine(producer, "Directory.Build.targets"), "<Project />");
File.WriteAllText(Path.Combine(producer, "Directory.Packages.props"),
    "<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>");
var config = Path.Combine(producer, "NuGet.config");
File.WriteAllText(config, "<configuration><packageSources><clear /></packageSources></configuration>");
Need(Run(producer, "--version").Trim() == "11.0.100", "Isolated SDK differs.");
var archiveHashes = new Dictionary<string, string>(StringComparer.Ordinal);
var dllHashes = new Dictionary<string, string>(StringComparer.Ordinal);
foreach (var id in ordered)
{
    var original = idsByPath.Single(x => x.Value == id).Key;
    var relative = Path.GetRelativePath(repository, original);
    var project = Path.Combine(producer, relative);
    var doc = XDocument.Load(project);
    doc.Descendants("Version").Single().AddAfterSelf(new XElement("PackageVersion", Version));
    foreach (var reference in doc.Descendants("ProjectReference").ToArray())
    {
        var dependency = idsByPath[Path.GetFullPath(Path.Combine(Path.GetDirectoryName(original)!, (string)reference.Attribute("Include")!))];
        reference.ReplaceWith(new XElement("PackageReference", new XAttribute("Include", dependency),
            new XAttribute("Version", "[" + Version + "]")));
    }
    Need(!doc.Descendants("ProjectReference").Any(), "Unconverted source dependency.");
    doc.Save(project);
    var dir = Path.GetDirectoryName(project)!;
    var cache = Path.Combine(root, "cache", id);
    string[] restore = ["restore", project, "--configfile", config, "--source", feed, "--packages", cache, "--use-lock-file"];
    Run(dir, restore);
    var lockPath = Path.Combine(dir, "packages.lock.json");
    var lockHash = Hash(lockPath);
    Run(dir, [.. restore, "--locked-mode"]);
    Need(Hash(lockPath) == lockHash, "Lock changed: " + id);
    var closure = new HashSet<string>(StringComparer.Ordinal);
    void AddDeps(string node) { foreach (var dep in graph[node]) if (closure.Add(dep)) AddDeps(dep); }
    AddDeps(id);
    using (var lockDoc = JsonDocument.Parse(File.ReadAllText(lockPath)))
    {
        var targets = lockDoc.RootElement.GetProperty("dependencies");
        Need(targets.EnumerateObject().Count() == 1, "Unexpected lock target.");
        var rows = targets.GetProperty("net11.0").EnumerateObject().ToArray();
        Need(closure.SetEquals(rows.Select(x => x.Name)), "Locked closure differs: " + id);
        foreach (var row in rows)
        {
            Need(row.Value.GetProperty("resolved").GetString() == Version &&
                row.Value.GetProperty("type").GetString() == (graph[id].Contains(row.Name, StringComparer.Ordinal) ? "Direct" : "Transitive"),
                "Locked dependency identity differs: " + row.Name);
            Need(Hash(Path.Combine(cache, row.Name, Version, "lib", "net11.0", row.Name + ".dll")) == dllHashes[row.Name],
                "Restored dependency binary differs: " + row.Name);
        }
    }
    var archive = Archive(feed, id);
    Need(!File.Exists(archive), "Candidate overwrite refused.");
    Run(dir, "pack", project, "-c", "Release", "--no-restore", "--output", feed, "-warnaserror");
    using var zip = ZipFile.OpenRead(archive);
    using var descriptor = zip.Entries.Single(e => e.FullName == id + ".nuspec").Open();
    var xml = XDocument.Load(descriptor).Root!;
    var ns = xml.Name.Namespace;
    var meta = xml.Element(ns + "metadata")!;
    Need(meta.Element(ns + "id")?.Value == id && meta.Element(ns + "version")?.Value == Version, "Nuspec identity differs.");
    var deps = meta.Descendants(ns + "dependency").ToArray();
    Need(graph[id].SequenceEqual(deps.Select(x => (string)x.Attribute("id")!).OrderBy(x => x, StringComparer.Ordinal)) &&
        deps.All(x => (string?)x.Attribute("version") == "[" + Version + "]"), "Nuspec exact dependency graph differs.");
    var binary = zip.Entries.Where(e => e.FullName.StartsWith("lib/", StringComparison.Ordinal) && e.FullName.EndsWith(".dll", StringComparison.Ordinal)).ToArray();
    Need(binary.Length == 1 && binary[0].FullName == "lib/net11.0/" + id + ".dll", "Archive TFM differs.");
    using var dll = binary[0].Open();
    var dllHash = Convert.ToHexStringLower(SHA256.HashData(dll));
    Need(dllHash == Hash(Path.Combine(producer, "build", "artifacts", "bin", id, "Release", "net11.0", id + ".dll")), "Packed assembly differs from producer.");
    dllHashes.Add(id, dllHash);
    archiveHashes.Add(id, Hash(archive));
    Console.WriteLine("MATH-RS420-ARCHIVE|id=" + id + "|version=" + Version + "|tfm=net11.0|archiveSha256=" + archiveHashes[id] +
        "|dllSha256=" + dllHash + "|lockSha256=" + lockHash + "|deps=" + string.Join(",", graph[id]) + "|PASS");
}
Need(Directory.GetFiles(feed, "*.nupkg").Length == 13 && archiveHashes.All(x => Hash(Archive(feed, x.Key)) == x.Value), "Candidate archive roster or bytes differ.");
Need(protectedFiles.All(x => Hash(x.Key) == x.Value), "Production input changed.");
var after = Snapshot();
Need(before.Count == after.Count && before.All(x => after.TryGetValue(x.Key, out var value) && value == x.Value), "Shared archives changed.");
Console.WriteLine("MATH-RS420-CANDIDATES|root=" + root + "|sdk=11.0.100|tfm=net11.0|packages=13|edges=16|version=" + Version +
    "|sharedArchives=" + before.Count + "|lockedRestore=PASS|publication=false|reservation=false|PASS");

static string Hash(string path)
{
    using var stream = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(stream));
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS420: " + message);
}
static string Run(string dir, params string[] arguments)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = dir, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var argument in arguments) info.ArgumentList.Add(argument);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("dotnet unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(480000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet command timed out.");
    }
    var output = stdout.GetAwaiter().GetResult();
    var errors = stderr.GetAwaiter().GetResult();
    if (process.ExitCode != 0) throw new InvalidDataException("dotnet failed: " + string.Join(" ", arguments) + "\n" + output + "\n" + errors);
    if (arguments[0] == "run") Console.WriteLine(output);
    if (!string.IsNullOrWhiteSpace(errors)) Console.WriteLine(errors);
    return output;
}
