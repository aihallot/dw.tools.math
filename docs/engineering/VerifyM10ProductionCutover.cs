#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;

if (args.Length != 1 || args[0] is not ("--red" or "--target")) throw new ArgumentException("Expected --red or --target.");
var red = args[0] == "--red";
static void Need(bool ok, string message) { if (!ok) throw new InvalidDataException("M10 production cutover: " + message); }
static string Git(params string[] args)
{
    using var p = new Process { StartInfo = new ProcessStartInfo("git") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true } };
    foreach (var arg in args) p.StartInfo.ArgumentList.Add(arg);
    Need(p.Start(), "Cannot start Git.");
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    if (!p.WaitForExit(30000)) { p.Kill(entireProcessTree: true); throw new TimeoutException("Git timed out."); }
    Need(p.ExitCode == 0, stderr.GetAwaiter().GetResult());
    return stdout.GetAwaiter().GetResult();
}
static string Normalize(string s) => s.Replace("\r\n", "\n", StringComparison.Ordinal);
static string AdaptInspector(string source)
{
    const string anchor = "var sources = LoadSources(root);";
    const string oldTarget = "properties[\"TargetFramework\"] == \"net10.0\"";
    const string targetPolicy = """
        var central = XDocument.Load(Path.Combine(root, "Directory.Build.props"));
        var framework = central.Descendants("TargetFramework").Single().Value;
        var language = central.Descendants("LangVersion").Single().Value;
        Need((framework, language) is ("net10.0", "14.0") or ("net11.0", "15.0"),
            "unqualified central framework/language pair");
        var sources = LoadSources(root);
    """;
    Need(source.Split(anchor, StringSplitOptions.None).Length == 2 &&
        source.Split(oldTarget, StringSplitOptions.None).Length == 2, "Inspector source anchors differ.");
    return source.Replace(anchor, targetPolicy, StringComparison.Ordinal)
        .Replace(oldTarget, "properties[\"TargetFramework\"] == framework", StringComparison.Ordinal);
}
const string baseline = "e4bbaa004ae643f2090c6c0010a6587b41e455eb";
const string discovery = "tools/dw.tools.math.discovery-bindings/dw.tools.math.discovery-bindings.csproj";
const string inspector = "tools/dw.tools.math.nuget-preflight/Program.cs";
var ledger = JsonNode.Parse(File.ReadAllText("docs/engineering/m10-cutover-readiness-verification-rs409.json"))!;
var paths = ledger["mutationInputs"]!.AsArray().Select(x => (string)x!).ToArray();
var retained = ledger["retainedInputs"]!.AsArray().Select(x => (string)x!).ToArray();
var historical = ledger["historicalProjects"]!.AsArray().Select(x => (string)x!["path"]!).ToArray();
var solution = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => ((string)x.Attribute("Path")!).Replace('\\','/')).ToArray();
var locks = solution.Select(x => x[..x.LastIndexOf('/')] + "/packages.lock.json").ToHashSet(StringComparer.Ordinal);
var exact = locks.Concat(new[] { "global.json", "Directory.Build.props", discovery, inspector }).ToHashSet(StringComparer.Ordinal);
Need(solution.Length == 24 && locks.Count == 24 && paths.Length == 28 && exact.SetEquals(paths) &&
    retained.Length == 3 && retained.ToHashSet(StringComparer.Ordinal).SetEquals(new[] { "Directory.Packages.props", "dw.tools.math.slnx", "NuGet.config" }) &&
    historical.Length == 97 && historical.Distinct(StringComparer.Ordinal).Count() == 97, "Scope mismatch.");
var inputs = paths.Concat(retained).ToArray();
var original = inputs.ToDictionary(p => p, p => Git("show", baseline + ":" + p), StringComparer.Ordinal);
var expected = new Dictionary<string,string>(original, StringComparer.Ordinal);
var sdk = JsonNode.Parse(original["global.json"])!;
Need((string?)sdk["sdk"]?["version"] == "10.0.401" && (string?)sdk["sdk"]?["rollForward"] == "latestPatch", "Original SDK differs.");
sdk["sdk"]!["version"] = "11.0.100";
sdk["sdk"]!["rollForward"] = "disable";
expected["global.json"] = sdk.ToJsonString();
var props = XDocument.Parse(original["Directory.Build.props"]);
Need(props.Descendants("TargetFramework").Single().Value == "net10.0" && props.Descendants("LangVersion").Single().Value == "14.0", "Original props differ.");
props.Descendants("TargetFramework").Single().Value = "net11.0";
props.Descendants("LangVersion").Single().Value = "15.0";
expected["Directory.Build.props"] = props.ToString();
var tool = XDocument.Parse(original[discovery]);
Need(tool.Descendants("TargetFramework").Single().Value == "net10.0", "Original discovery target differs.");
tool.Descendants("TargetFramework").Single().Value = "net11.0";
expected[discovery] = tool.ToString();
expected[inspector] = AdaptInspector(original[inspector]);
foreach (var path in locks)
{
    var doc = JsonNode.Parse(original[path])!;
    var deps = doc["dependencies"]!.AsObject();
    Need(deps.Count == 1 && deps["net10.0"] is JsonObject, "Original lock differs: " + path);
    var graph = deps["net10.0"]!.DeepClone();
    deps.Remove("net10.0");
    deps["net11.0"] = graph;
    expected[path] = doc.ToJsonString();
}
bool Equivalent(string path, string a, string b)
{
    if (path == "global.json" || locks.Contains(path)) return JsonNode.DeepEquals(JsonNode.Parse(a), JsonNode.Parse(b));
    if (path == "Directory.Build.props" || path == discovery) return XNode.DeepEquals(XDocument.Parse(a).Root, XDocument.Parse(b).Root);
    return Normalize(a) == Normalize(b);
}
string[] Mismatches(Dictionary<string,string> actual, Dictionary<string,string> target) =>
    inputs.Where(p => !Equivalent(p, actual[p], target[p])).OrderBy(x => x, StringComparer.Ordinal).ToArray();
var actual = inputs.ToDictionary(p => p, File.ReadAllText, StringComparer.Ordinal);
var changed = Git("diff", "--name-only", "--no-renames", baseline, "--").Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(x => x.TrimEnd('\r')).Where(p => exact.Contains(p) || retained.Contains(p) || historical.Contains(p) ||
        p.StartsWith("src/", StringComparison.Ordinal) || p.StartsWith("tests/", StringComparison.Ordinal) ||
        p.StartsWith("tools/", StringComparison.Ordinal) || p.StartsWith("samples/", StringComparison.Ordinal)).ToHashSet(StringComparer.Ordinal);
foreach (var path in historical)
    Need(Git("hash-object", "--path=" + path, path).Trim() == Git("rev-parse", baseline + ":" + path).Trim(), "Historical project drift: " + path);
if (red)
{
    Need(changed.Count == 0 && Mismatches(actual, original).Length == 0, "Production baseline differs.");
    Need(Mismatches(actual, expected).ToHashSet(StringComparer.Ordinal).SetEquals(exact), "Target RED does not reject all 28 stale inputs.");
    var digest = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(AdaptInspector(actual[inspector]))));
    Need(digest == (string?)ledger["inspectorCandidateSha256"], "Candidate source digest differs from RS406.");
    void Negative(string path, string value)
    {
        var copy = new Dictionary<string,string>(expected, StringComparer.Ordinal) { [path] = value };
        Need(Mismatches(copy, expected).SequenceEqual(new[] { path }), "Negative control did not reject " + path);
        Console.WriteLine("MATH-RS411-NEGATIVE|path=" + path + "|PASS");
    }
    Negative("global.json", original["global.json"]);
    Negative("Directory.Build.props", original["Directory.Build.props"]);
    Negative(locks.OrderBy(x => x, StringComparer.Ordinal).First(), original[locks.OrderBy(x => x, StringComparer.Ordinal).First()]);
    Negative(discovery, original[discovery]);
    Negative(inspector, original[inspector]);
    var altered = XDocument.Parse(expected["Directory.Build.props"]);
    altered.Descendants("TreatWarningsAsErrors").Single().Value = "false";
    Negative("Directory.Build.props", altered.ToString());
    Negative("NuGet.config", original["NuGet.config"] + "\n<!-- drift -->");
    Console.WriteLine("MATH-RS411-PRODUCTION-RED|baseline=PASS|targetRefused=28|historical=97|candidate=PASS|negative=7|production=false|PASS");
}
else
{
    Need(changed.SetEquals(exact), "Actual production mutation roster differs.");
    Need(Mismatches(actual, expected).Length == 0, "Target mismatch: " + string.Join(", ", Mismatches(actual, expected)));
    Console.WriteLine("MATH-M10-PRODUCTION-TARGET|inputs=28|historical=97|retained=3|PASS");
}
