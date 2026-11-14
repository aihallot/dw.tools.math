#:property NuGetAudit=false
#:property PublishAot=false

using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--check" or "--check-sdk11"))
    throw new ArgumentException("Expected --check or --check-sdk11.");

var expected = new[] {
    "dw.tools.math.foundation",
    "dw.quantities",
    "dw.quantities.standard",
    "dw.quantities.expression",
    "dw.tools.math.ir",
    "dw.tools.math.composition",
    "dw.tools.math.numerics",
    "dw.tools.math.statistics",
    "dw.tools.math.symbolics",
    "dw.tools.math.codecs",
    "dw.tools.math.distribution",
    "dw.tools.math.analysis",
    "dw.tools.math.scientific"
};
using var global = JsonDocument.Parse(File.ReadAllText("global.json"));
var sdk = global.RootElement.GetProperty("sdk");
Require(sdk.GetProperty("version").GetString() == "10.0.401" &&
    sdk.GetProperty("rollForward").GetString() == "latestPatch" &&
    !sdk.GetProperty("allowPrerelease").GetBoolean(),
    "Observed pinned SDK policy must be recorded before an upgrade.");
var props = XDocument.Load("Directory.Build.props");
string? Prop(XDocument doc, string name) =>
    doc.Descendants().FirstOrDefault(x => x.Name.LocalName == name)?.Value.Trim();
Require(Prop(props, "TargetFramework") == "net10.0" &&
    Prop(props, "LangVersion") == "14.0" &&
    Prop(props, "TreatWarningsAsErrors") == "true" &&
    Prop(props, "BaseOutputPath")?.Contains("build/artifacts/", StringComparison.Ordinal) == true,
    "Central SDK/language/output baseline drift.");
var root = Path.Combine("src", "projects");
var all = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
    .Where(p => !p.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
        !p.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Select(path => (Path: path, Doc: XDocument.Load(path)))
    .Where(p => Prop(p.Doc, "IsPackable") == "true")
    .OrderBy(p => Prop(p.Doc, "PackageId"), StringComparer.Ordinal)
    .ToArray();
Require(all.Length == expected.Length, "Expected exactly 13 packable source projects.");
var ids = new HashSet<string>(StringComparer.Ordinal);
foreach (var item in all)
{
    var id = Prop(item.Doc, "PackageId") ?? "";
    var version = Prop(item.Doc, "Version") ?? "";
    Require(ids.Add(id) && expected.Contains(id, StringComparer.Ordinal) &&
        Path.GetFileNameWithoutExtension(item.Path) == id &&
        version.Contains("-preview.", StringComparison.Ordinal) &&
        Prop(item.Doc, "RestorePackagesWithLockFile") == "true",
        "Unexpected package or unpinned restore policy: " + item.Path);
    Require(Prop(item.Doc, "TargetFramework") is null &&
        Prop(item.Doc, "LangVersion") is null,
        "Packable project unexpectedly overrides central .NET/C# baseline: " + id);
    var projectRefs = item.Doc.Descendants()
        .Where(x => x.Name.LocalName == "ProjectReference")
        .Select(x => x.Attribute("Include")?.Value ?? "")
        .ToArray();
    Require(projectRefs.All(x => !string.IsNullOrWhiteSpace(x) &&
        !x.Contains("aura.", StringComparison.OrdinalIgnoreCase) &&
        !x.Contains("dw.tools.decision", StringComparison.OrdinalIgnoreCase)),
        "Sibling-project dependency or malformed reference: " + id);
    Console.WriteLine("MATH-RS214-PACKABLE|id=" + id + "|sourceVersion=" + version +
        "|inheritedTfm=net10.0|inheritedLang=CSharp14|projectRefs=" + projectRefs.Length);
}
Require(ids.SetEquals(expected), "Thirteen-package roster drift.");
Console.WriteLine("MATH-RS214-DOTNET-BASELINE|packable=13|sdkPin=10.0.401|rollForward=latestPatch|tfm=net10.0|lang=CSharp14|sdk11Qualified=false|noMutation=true|PASS");
if (args[0] == "--check-sdk11")
{
    var start = new ProcessStartInfo("dotnet")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    start.ArgumentList.Add("--list-sdks");
    using var process = Process.Start(start) ??
        throw new InvalidDataException("RS215: dotnet SDK listing could not start.");
    var stdout = process.StandardOutput.ReadToEnd();
    var stderr = process.StandardError.ReadToEnd();
    Require(process.WaitForExit(30000) && process.ExitCode == 0,
        "dotnet --list-sdks failed: " + stderr);
    var versions = stdout.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0])
        .ToArray();
    Require(versions.Contains("11.0.100", StringComparer.Ordinal),
        "The independently installed stable SDK 11.0.100 is not available.");
    Require(versions.All(v => !v.StartsWith("11.0.100-", StringComparison.Ordinal)),
        "Do not qualify an SDK 11 preview as the stable 11.0.100 baseline.");
    Console.WriteLine("MATH-RS215-SDK11-INVENTORY|sdk=11.0.100|installed=true|repoSdkPin=10.0.401|packable=13|net11BuildTested=false|mutation=false|PASS");
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS214: " + message);
}
