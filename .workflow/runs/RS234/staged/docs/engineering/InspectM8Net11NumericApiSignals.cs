#:property NuGetAudit=false
#:property PublishAot=false

using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

var projects = Directory.EnumerateFiles("src/projects", "*.csproj", SearchOption.AllDirectories)
    .Where(p => !p.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                !p.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Select(p => (Path: p, Xml: XDocument.Load(p)))
    .Where(p => p.Xml.Descendants().Any(x =>
        x.Name.LocalName == "IsPackable" && x.Value.Trim() == "true"))
    .OrderBy(p => p.Path, StringComparer.Ordinal).ToArray();
Require(projects.Length == 13, "The packable project roster changed.");
string[] signals =
[
    "Math.Round(", "MathF.Round(", "System.Math.Round(", "System.MathF.Round(",
    "BigInteger", "System.Numerics.Complex", "new Complex(", "Complex.",
    "Convert.ToInt16(", "Convert.ToInt32(", "Convert.ToInt64(",
    "checked(", "unchecked("
];
var totals = new SortedDictionary<string, int>(StringComparer.Ordinal);
var sourceCount = 0;
var signaledCount = 0;
foreach (var (projectPath, xml) in projects)
{
    var id = xml.Descendants().Single(x => x.Name.LocalName == "PackageId").Value.Trim();
    var root = Path.GetDirectoryName(projectPath) ??
        throw new InvalidDataException("Missing project directory: " + projectPath);
    var files = Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
        .Where(p => !p.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) &&
                    !p.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
        .OrderBy(p => p, StringComparer.Ordinal).ToArray();
    Require(files.Length > 0, "Empty source scope: " + id);
    sourceCount += files.Length;
    foreach (var file in files)
    {
        var source = File.ReadAllText(file);
        var matched = false;
        foreach (var signal in signals)
        {
            var count = 0;
            for (var offset = 0; (offset = source.IndexOf(signal, offset,
                StringComparison.Ordinal)) >= 0; offset += signal.Length)
                count++;
            if (count == 0) continue;
            totals.TryGetValue(signal, out var previous);
            totals[signal] = previous + count;
            matched = true;
            Console.WriteLine("MATH-RS234-API-SIGNAL|project=" + id + "|file=" +
                Path.GetRelativePath(".", file).Replace('\\', '/') +
                "|pattern=" + signal + "|matches=" + count);
        }
        if (matched) signaledCount++;
    }
}
Require(sourceCount >= 13, "Unexpected source file inventory.");
foreach (var signal in signals)
    Console.WriteLine("MATH-RS234-API-TOTAL|pattern=" + signal + "|matches=" +
        (totals.TryGetValue(signal, out var count) ? count : 0));
Console.WriteLine("MATH-RS234-API-AUDIT|packableProjects=13|sourceFiles=" + sourceCount +
    "|signaledFiles=" + signaledCount + "|lexicalOnly=true|" +
    "apiCompatibilityQualified=false|packageCompatibilityQualified=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS234: " + message);
}
