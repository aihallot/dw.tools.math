#:property NuGetAudit=false
#:property PublishAot=false

using System.Xml.Linq;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");

var expectedIds = new HashSet<string>(StringComparer.Ordinal)
{
    "dw.tools.math.foundation", "dw.quantities", "dw.quantities.standard",
    "dw.quantities.expression", "dw.tools.math.ir", "dw.tools.math.composition",
    "dw.tools.math.numerics", "dw.tools.math.statistics",
    "dw.tools.math.symbolics", "dw.tools.math.codecs",
    "dw.tools.math.distribution", "dw.tools.math.analysis",
    "dw.tools.math.scientific"
};
var root = Path.GetFullPath(Path.Combine("src", "projects"));
var projects = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
    .Where(p => !p.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal)
        && !p.Replace('\\', '/').Contains("/bin/", StringComparison.Ordinal))
    .Select(path => (Path: Path.GetFullPath(path), Xml: XDocument.Load(path)))
    .Where(item => item.Xml.Descendants().Any(x =>
        x.Name.LocalName == "IsPackable" && x.Value.Trim() == "true"))
    .ToArray();
Require(projects.Length == 13, "Expected thirteen packable projects.");
var idsByPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
foreach (var (path, xml) in projects)
{
    var id = xml.Descendants().Single(x => x.Name.LocalName == "PackageId").Value.Trim();
    Require(expectedIds.Contains(id) && idsByPath.TryAdd(path, id),
        "Unexpected or duplicate project ID: " + id);
}
Require(idsByPath.Values.ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds),
    "Packable roster differs from RS214 baseline.");
var graph = new SortedDictionary<string, string[]>(StringComparer.Ordinal);
foreach (var (path, xml) in projects)
{
    var id = idsByPath[path];
    var references = xml.Descendants()
        .Where(x => x.Name.LocalName == "ProjectReference")
        .Select(x => x.Attribute("Include")?.Value ??
            throw new InvalidDataException("Missing ProjectReference path."))
        .Select(relative => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, relative)))
        .Select(reference =>
        {
            Require(idsByPath.TryGetValue(reference, out _),
                "ProjectReference leaves the qualified packable set: " + id);
            return idsByPath[reference];
        })
        .ToArray();
    Require(references.Distinct(StringComparer.Ordinal).Count() == references.Length,
        "Duplicate dependency: " + id);
    graph.Add(id, references.OrderBy(x => x, StringComparer.Ordinal).ToArray());
}
Require(graph.Values.Sum(x => x.Length) == 16, "Expected sixteen observed direct dependency edges.");
Require(graph["dw.tools.math.distribution"].Length == 6,
    "Distribution composition graph changed.");
var colors = new Dictionary<string, int>(StringComparer.Ordinal);
var levels = new Dictionary<string, int>(StringComparer.Ordinal);
int Visit(string id)
{
    if (colors.TryGetValue(id, out var color))
    {
        Require(color != 1, "Cycle detected: " + id);
        if (color == 2) return levels[id];
    }
    colors[id] = 1;
    var level = graph[id].Length == 0 ? 0 : graph[id].Max(Visit) + 1;
    colors[id] = 2;
    levels[id] = level;
    return level;
}
foreach (var id in graph.Keys) Visit(id);
Require(levels.Count == 13 && levels.Values.Max() <= 5,
    "Unexpected dependency depth.");
foreach (var (id, references) in graph)
    Console.WriteLine("MATH-RS217-PACKAGE-GRAPH|id=" + id +
        "|level=" + levels[id] + "|directDependencies=" +
        (references.Length == 0 ? "none" : string.Join(",", references)));
Console.WriteLine("MATH-RS217-GRAPH|projects=13|edges=" + graph.Values.Sum(x => x.Length) + "|cycles=0|externalProjectRefs=0|maxLevel=" +
    levels.Values.Max() + "|net11BuildQualified=false|mutation=false|PASS");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS217: " + message);
}
