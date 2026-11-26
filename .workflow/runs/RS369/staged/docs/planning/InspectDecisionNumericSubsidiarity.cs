#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

const string Target = "docs/planning/evidence/decision-numeric-source-inventory-rs369.md";
if (args.Length != 1 || args[0] is not ("--write" or "--check"))
    throw new ArgumentException("Expected --write or --check.");

var start = new ProcessStartInfo("git") {
    UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true
};
foreach (var argument in new[] { "ls-files", "-z", "--", "src/projects" })
    start.ArgumentList.Add(argument);
using var process = Process.Start(start) ?? throw new InvalidOperationException("Git unavailable.");
var stdout = process.StandardOutput.ReadToEndAsync();
var stderr = process.StandardError.ReadToEndAsync();
if (!process.WaitForExit(30000)) {
    process.Kill(entireProcessTree: true);
    throw new TimeoutException("Tracked-source inventory timed out.");
}
var output = stdout.GetAwaiter().GetResult();
var error = stderr.GetAwaiter().GetResult();
Need(process.ExitCode == 0, "git ls-files failed: " + error);
var paths = output.Split('\0', StringSplitOptions.RemoveEmptyEntries)
    .Where(x => x.EndsWith(".cs", StringComparison.Ordinal) || x.EndsWith(".csproj", StringComparison.Ordinal))
    .Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
var projects = paths.Where(x => x.EndsWith(".csproj", StringComparison.Ordinal)).ToArray();
var solutionProjects = XDocument.Load("dw.tools.math.slnx").Descendants("Project")
    .Select(x => (string?)x.Attribute("Path") ?? "").Where(x => x.StartsWith("src/projects/", StringComparison.Ordinal))
    .OrderBy(x => x, StringComparer.Ordinal).ToArray();
Need(solutionProjects.Length == 13 && solutionProjects.All(x => projects.Contains(x, StringComparer.Ordinal)),
    "A production solution project is absent from the tracked project roster.");
Need(paths.All(File.Exists), "A tracked source path is missing.");
var sources = paths.Where(x => x.EndsWith(".cs", StringComparison.Ordinal)).ToArray();
Need(sources.Length > 0, "No tracked C# sources.");
var known = new[] {
    "src/projects/dw.tools.math.statistics/SplitMix64Random.cs",
    "src/projects/dw.tools.math.statistics/SplitMix64PositionCodec.cs",
    "src/projects/dw.tools.math.statistics/BernoulliDistribution64.cs",
    "src/projects/dw.tools.math.statistics/ContinuousUniformDistribution64.cs",
    "src/projects/dw.tools.math.statistics/QuantileStatistics64.cs",
    "src/projects/dw.tools.math.statistics/DescriptiveStatistics64.cs",
    "src/projects/dw.quantities/ExactRational.cs"
};
Need(known.All(x => sources.Contains(x, StringComparer.Ordinal)), "Known public primitive source absent.");
var words = new Regex(
    @"quantil|percentil|median|variance|moment|welford|accumulat|running.?stat|streaming|online.?stat|rational|int64|long\.MaxValue|long\.MinValue|scal(e|ing)|splitmix|bernoulli|uniform|checkpoint|snapshot",
    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
var publicToken = new Regex(@"\bpublic\b", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
var b = new StringBuilder();
var fence = new string((char)96, 3);
void Line(string line = "") => b.Append(line).Append('\n');
string Normalize(string value) => value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
string Hash(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
Line("# RS369 - Decision numeric subsidiarity source inventory");
Line();
Line("Scope: all Git-tracked C# and project files under src/projects, checked against the production solution roster. Hashes identify LF-normalized text, not binaries.");
Line("This is a lexical review aid, not a compiler API inventory or proof of absence. Review all candidate declarations and alternate implementations before admitting new APIs.");
Line("The report does not establish performance, package compatibility, publication or adoption.");
Line();
Line("## Tracked project files (includes all thirteen production projects)");
foreach (var path in projects) Line("- " + path);
Line();
Line("## Complete tracked source roster");
foreach (var path in paths) Line("- " + path + " | " + Hash(Normalize(File.ReadAllText(path))));
Line();
Line("## Candidate lines across all production C# sources");
int hits = 0, candidateFiles = 0;
foreach (var path in sources) {
    var lines = Normalize(File.ReadAllText(path)).Split('\n');
    // In candidate files include all public declaration lines, as well as every keyword hit.
    if (!known.Contains(path, StringComparer.Ordinal) && !lines.Any(x => words.IsMatch(x))) continue;
    candidateFiles++;
    Line(); Line("### " + path); Line(); Line(fence + "text");
    for (var i = 0; i < lines.Length; i++) {
        if (!words.IsMatch(lines[i]) && !publicToken.IsMatch(lines[i])) continue;
        Line((i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture) + ": " + lines[i].Trim());
        hits++;
    }
    Line(fence);
}
Need(candidateFiles >= known.Length && hits > 0, "No usable candidate inventory.");
var result = b.ToString();
if (args[0] == "--write") {
    if (!File.Exists(Target) || File.ReadAllText(Target) != result)
        File.WriteAllText(Target, result, new UTF8Encoding(false));
} else Need(File.Exists(Target) && File.ReadAllText(Target) == result, "Inventory differs from tracked source.");
Console.WriteLine($"MATH-RS369-NUMERIC-INVENTORY|projects={projects.Length}|sources={sources.Length}|candidateFiles={candidateFiles}|lines={hits}|lexicalOnly=true|PASS");

static void Need(bool condition, string message) {
    if (!condition) throw new InvalidDataException("RS369: " + message);
}
