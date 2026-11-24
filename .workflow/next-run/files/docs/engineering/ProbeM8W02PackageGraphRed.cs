#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

const string Inspector = "docs/engineering/InspectM8Net11PackageGraph.cs";
var projectFiles = Directory.EnumerateFiles("src/projects", "*.csproj",
        SearchOption.AllDirectories)
    .Where(path => !path.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                   !path.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Select(path => path.Replace('\\', '/'))
    .OrderBy(path => path, StringComparer.Ordinal)
    .ToArray();
Need(projectFiles.Length == 13,
    "Expected thirteen existing packable Math project declarations.");
var originals = projectFiles.Append(Inspector).ToArray();
Need(originals.Length == 14 && originals.All(File.Exists),
    "Existing Math source graph validator or project declaration is missing.");
var originalHashes = originals.ToDictionary(path => path, Sha256, StringComparer.Ordinal);

// Use a short, repository-owned disposable root because DWF's Windows clone path is long.
var workspace = Path.GetFullPath(Path.Combine(
    "build", "artifacts", "g320-" + Guid.NewGuid().ToString("N")[..10]));
Directory.CreateDirectory(workspace);
foreach (var relative in originals)
{
    var dest = Path.Combine(workspace,
        relative.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
    File.Copy(relative, dest);
    Need(Sha256(dest) == originalHashes[relative],
        "Copied project or inspector bytes differ: " + relative);
}

ProcessResult Verify() => Run(workspace, "run", "--file", Inspector, "--", "--check");
void Clean()
{
    var result = Verify();
    Need(result.ExitCode == 0 &&
        result.Output.Contains(
            "MATH-RS217-GRAPH|projects=13|edges=16|cycles=0|externalProjectRefs=0|",
            StringComparison.Ordinal) &&
        result.Output.Contains("mutation=false|PASS", StringComparison.Ordinal),
        "Existing package-graph inspector refused a restored copy: " + result.Output);
}
Clean();

const string Composition =
    "src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj";
const string Ir = "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj";
var refusalCount = 0;
void Reject(string label, string relativePath, Action<XDocument> mutate,
    string expectedReason)
{
    var target = Path.Combine(workspace,
        relativePath.Replace('/', Path.DirectorySeparatorChar));
    var pristineBytes = File.ReadAllBytes(target);
    try
    {
        var project = XDocument.Load(target);
        mutate(project);
        project.Save(target);
        var result = Verify();
        Need(result.ExitCode != 0 &&
            result.Output.Contains("RS217: " + expectedReason,
                StringComparison.Ordinal),
            "Graph change escaped existing inspector: " + label + " " + result.Output);
        refusalCount++;
        Console.WriteLine("MATH-RS320-GRAPH-REFUSAL|" + label + "|PASS");
    }
    finally
    {
        File.WriteAllBytes(target, pristineBytes);
    }
    Need(Sha256(target) == originalHashes[relativePath],
        "Copied graph declaration was not restored: " + relativePath);
    Clean();
}

XElement CompositionQuantitiesReference(XDocument doc) =>
    doc.Descendants().Single(x => x.Name.LocalName == "ProjectReference" &&
        x.Attribute("Include")?.Value.Contains(
            "../dw.quantities/dw.quantities.csproj", StringComparison.Ordinal) == true);

Reject("out-of-roster-reference", Composition, doc =>
    CompositionQuantitiesReference(doc).SetAttributeValue(
        "Include", "../dw.tools.math.forged/dw.tools.math.forged.csproj"),
    "ProjectReference leaves the qualified packable set: dw.tools.math.composition");

Reject("duplicate-dependency", Composition, doc =>
{
    var itemGroup = doc.Descendants().Single(x => x.Name.LocalName == "ItemGroup" &&
        x.Elements().Any(e => e.Name.LocalName == "ProjectReference"));
    itemGroup.Add(new XElement("ProjectReference",
        new XAttribute("Include",
            "../dw.tools.math.ir/dw.tools.math.ir.csproj")));
}, "Duplicate dependency: dw.tools.math.composition");

Reject("removed-edge", Composition, doc =>
    CompositionQuantitiesReference(doc).Remove(),
    "Expected sixteen observed direct dependency edges.");

Reject("cycle", Ir, doc =>
{
    var reference = doc.Descendants().Single(x =>
        x.Name.LocalName == "ProjectReference");
    reference.SetAttributeValue("Include",
        "../dw.tools.math.composition/dw.tools.math.composition.csproj");
}, "Cycle detected:");

Need(refusalCount == 4,
    "Expected four independent graph risk refusals.");
foreach (var relative in originals)
{
    var copiedPath = Path.Combine(workspace,
        relative.Replace('/', Path.DirectorySeparatorChar));
    Need(Sha256(relative) == originalHashes[relative] &&
        Sha256(copiedPath) == originalHashes[relative],
        "Original or independently copied Math source changed: " + relative);
}
Console.WriteLine("MATH-RS320-PACKAGE-GRAPH-RED|copies=14|sources=13|" +
    "edges=16|outOfRoster=refused|duplicate=refused|missingEdge=refused|" +
    "cycle=refused|cleanRestored=4|publishedPackagesUnchanged=true|" +
    "net11Migration=false|PASS");

static string Sha256(string path)
{
    using var source = File.OpenRead(path);
    return Convert.ToHexStringLower(SHA256.HashData(source));
}

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS320: " + reason);
}

static ProcessResult Run(string workingDirectory, params string[] args)
{
    var info = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = workingDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var arg in args) info.ArgumentList.Add(arg);
    using var process = Process.Start(info) ??
        throw new InvalidOperationException("RS320: Cannot start copied graph inspector.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(120000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS320: Copied graph inspector timed out.");
    }
    return new ProcessResult(process.ExitCode,
        stdout.GetAwaiter().GetResult() + "\n" +
        stderr.GetAwaiter().GetResult());
}

sealed record ProcessResult(int ExitCode, string Output);
