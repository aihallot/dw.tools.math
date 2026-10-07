using System.Xml.Linq;

var root = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
var failures = new List<string>();

static bool HasUpper(string value) => value.Any(char.IsUpper);
static string Rel(string root, string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

foreach (var area in new[] { "src", "tests" })
{
    var areaRoot = Path.Combine(root, area);
    if (!Directory.Exists(areaRoot)) continue;

    foreach (var project in Directory.EnumerateFiles(areaRoot, "*.csproj", SearchOption.AllDirectories))
    {
        var relative = Rel(root, project);
        var segments = relative.Split('/');
        if (segments.Any(HasUpper))
            failures.Add($"project-path-not-lowercase: {relative}");
    }

    foreach (var directoryName in new[] { "bin", "obj" })
    {
        foreach (var directory in Directory.EnumerateDirectories(areaRoot, directoryName, SearchOption.AllDirectories))
            failures.Add($"compiled-output-under-source-tree: {Rel(root, directory)}");
    }
}

var hasProjectsUnderSourceTrees =
    new[] { "src", "tests" }
        .Select(area => Path.Combine(root, area))
        .Where(Directory.Exists)
        .Any(area => Directory.EnumerateFiles(area, "*.csproj", SearchOption.AllDirectories).Any());

if (hasProjectsUnderSourceTrees)
{
    var propsPath = Path.Combine(root, "Directory.Build.props");
    var relocates = false;
    if (File.Exists(propsPath))
    {
        var text = File.ReadAllText(propsPath);
        relocates =
            text.Contains("<BaseOutputPath>", StringComparison.OrdinalIgnoreCase)
            && text.Contains("<BaseIntermediateOutputPath>", StringComparison.OrdinalIgnoreCase);
    }

    if (!relocates)
        failures.Add("build-output-relocation-missing: Directory.Build.props must set BaseOutputPath and BaseIntermediateOutputPath for projects under src/tests.");
}

if (failures.Count == 0)
{
    Console.WriteLine("PASS dotnet-current structural conventions");
    return 0;
}

foreach (var failure in failures.OrderBy(value => value, StringComparer.Ordinal))
    Console.Error.WriteLine(failure);
return 1;
