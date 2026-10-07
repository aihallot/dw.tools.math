#:property PublishAot=false
#:property NuGetAudit=false

using System.Text.Json.Nodes;

try
{
    var root = Directory.GetCurrentDirectory();
    var backlog = LoadObject(Path.Combine(root, "docs", "planning", "backlog.json"));
    var plan = LoadObject(Path.Combine(root, ".aura", "workflow", "plan", "project.json"));
    var rs001 = LoadObject(Path.Combine(root, ".aura", "workflow", "reports", "RS001", "attempt-003.json"));

    Require(S(rs001, "outcome") == "succeeded", "RS001 durable report is not succeeded.");
    Require(rs001["attempt"]?.GetValue<int>() == 3, "RS001 durable report is not attempt 3.");
    Require(S(plan["project"]!.AsObject(), "id") == "dw.tools.math", "Unexpected native DWF project id.");

    var expected = BuildExpected(backlog);
    var actual = BuildActual(plan);

    Require(expected.Count == 496, $"Expected product projection count changed: {expected.Count}.");
    Require(actual.Count == expected.Count, $"Native DWF node count {actual.Count} differs from expected {expected.Count}.");

    foreach (var pair in expected)
    {
        Require(actual.TryGetValue(pair.Key, out var candidate), "Missing native DWF node " + pair.Key + ".");
        var node = candidate ?? throw new InvalidOperationException("Missing native DWF node " + pair.Key + ".");
        Require(node.Kind == pair.Value.Kind, $"Kind mismatch for {pair.Key}: {node.Kind} != {pair.Value.Kind}.");
        Require(node.ParentId == pair.Value.ParentId, $"Parent mismatch for {pair.Key}: {node.ParentId ?? "<root>"} != {pair.Value.ParentId ?? "<root>"}.");
        EqualSet(node.DependsOn, pair.Value.DependsOn, "Dependency mismatch for " + pair.Key + ".");
        Require(!string.IsNullOrWhiteSpace(node.Title), "Empty native DWF title for " + pair.Key + ".");
        Require(node.Title.All(ch => ch <= 127), "Native DWF title is not canonical English/ASCII for " + pair.Key + ": " + node.Title);
    }

    foreach (var id in actual.Keys)
        Require(expected.ContainsKey(id), "Unexpected native DWF node " + id + ".");

    var historicalOmissions = HistoricalOmissions();
    foreach (var id in historicalOmissions)
        Require(!actual.ContainsKey(id), "Historical C01 subtask must not be backfilled: " + id + ".");

    Require(State(actual, "M0") is "in-progress" or "done", "M0 must be in-progress or done after native DWF adoption.");
    Require(State(actual, "M0-W01") is "in-progress" or "done", "M0-W01 must be in-progress before RS002 or done after RS002.");
    RequireState(actual, "M0-W01-C01", "done");
    RequireState(actual, "M0-W01-C01-T1", "done");
    RequireState(actual, "M0-W01-C01-T2", "done");
    RequireEvidenceChecklist(actual["M0-W01-C01"].Object, "M0-W01-C01");
    RequireEvidenceChecklist(actual["M0-W01-C01-T1"].Object, "M0-W01-C01-T1");
    RequireEvidenceChecklist(actual["M0-W01-C01-T2"].Object, "M0-W01-C01-T2");

    var c02Ids = new[]
    {
        "M0-W01-C02",
        "M0-W01-C02-T1",
        "M0-W01-C02-T1-A",
        "M0-W01-C02-T1-B",
        "M0-W01-C02-T1-C",
        "M0-W01-C02-T2",
        "M0-W01-C02-T2-A",
        "M0-W01-C02-T2-B",
        "M0-W01-C02-T2-C"
    };

    var baseline =
        State(actual, "M0-W01-C02") == "ready" &&
        State(actual, "M0-W01-C02-T1") == "ready" &&
        State(actual, "M0-W01-C02-T1-A") == "ready" &&
        State(actual, "M0-W01-C02-T1-B") == "not-ready" &&
        State(actual, "M0-W01-C02-T1-C") == "not-ready" &&
        State(actual, "M0-W01-C02-T2") == "not-ready" &&
        State(actual, "M0-W01-C02-T2-A") == "not-ready" &&
        State(actual, "M0-W01-C02-T2-B") == "not-ready" &&
        State(actual, "M0-W01-C02-T2-C") == "not-ready";

    var target = State(actual, "M0-W01") == "done" && c02Ids.All(id => State(actual, id) == "done");
    baseline = State(actual, "M0-W01") == "in-progress" && baseline;
    Require(baseline || target, "M0-W01-C02 is neither the accepted RS002 baseline nor exact completed target.");

    Require(File.Exists(Path.Combine(root, "docs", "planning", "decisions", "native-dwf-roadmap-reconciliation.md")),
        "Missing native DWF roadmap reconciliation decision.");
    if (target)
    {
        Require(File.Exists(Path.Combine(root, "docs", "planning", "decisions", "m0-w01-c02.md")),
            "Completed M0-W01-C02 target is missing its qualification decision.");
    }

    Console.WriteLine($"native DWF adoption valid: nodes={actual.Count} milestones=8 workPackages=17 phases=53 tasks=106 subtasks=312 c02={(target ? "done" : "ready")}");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("native DWF adoption invalid: " + ex.Message);
    return 1;
}

static Dictionary<string, ExpectedNode> BuildExpected(JsonObject backlog)
{
    var result = new Dictionary<string, ExpectedNode>(StringComparer.Ordinal);
    var historical = HistoricalOmissions();

    foreach (var node in Objects(backlog, "releases"))
        Add(result, S(node, "id"), "milestone", null, Strings(node, "depends_on"));

    foreach (var node in Objects(backlog, "work_packages"))
        Add(result, S(node, "id"), "work-package", S(node, "release"), []);

    foreach (var chunk in Objects(backlog, "chunks"))
    {
        var chunkId = S(chunk, "id");
        Add(result, chunkId, "phase", S(chunk, "work_package"), Strings(chunk, "depends_on"));
        foreach (var taskNode in Objects(chunk, "tasks"))
        {
            var taskId = S(taskNode, "id");
            Add(result, taskId, "task", chunkId, []);
            foreach (var subtask in Objects(taskNode, "subtasks"))
            {
                var subtaskId = S(subtask, "id");
                if (!historical.Contains(subtaskId))
                    Add(result, subtaskId, "subtask", taskId, []);
            }
        }
    }

    return result;
}

static Dictionary<string, ActualNode> BuildActual(JsonObject plan)
{
    var result = new Dictionary<string, ActualNode>(StringComparer.Ordinal);
    foreach (var milestone in Objects(plan, "milestones"))
        Visit(milestone, "milestone", null, result);
    return result;
}

static void Visit(JsonObject node, string kind, string? parentId, IDictionary<string, ActualNode> result)
{
    var id = S(node, "id");
    Require(result.TryAdd(id, new ActualNode(
        id,
        S(node, "title"),
        kind,
        parentId,
        S(node, "state"),
        StringsOptional(node, "dependsOn"),
        node)), "Duplicate native DWF node " + id + ".");

    var childKey = kind switch
    {
        "milestone" => "workPackages",
        "work-package" => "phases",
        "phase" => "tasks",
        "task" => "subtasks",
        _ => null
    };
    if (childKey is null) return;

    var nextKind = kind switch
    {
        "milestone" => "work-package",
        "work-package" => "phase",
        "phase" => "task",
        "task" => "subtask",
        _ => throw new InvalidOperationException()
    };

    foreach (var child in ObjectsOptional(node, childKey))
        Visit(child, nextKind, id, result);
}

static void RequireEvidenceChecklist(JsonObject node, string id)
{
    var checklist = ObjectsOptional(node, "checklist").ToArray();
    Require(checklist.Length == 1, id + " must have exactly one historical RS001 evidence checklist item.");
    var item = checklist[0];
    Require(S(item, "id") == "rs001-evidence", id + " checklist id mismatch.");
    Require(S(item, "state") == "satisfied", id + " RS001 evidence checklist is not satisfied.");
    var evidence = Strings(item, "evidence");
    Require(evidence.SequenceEqual(new[] { ".aura/workflow/reports/RS001/attempt-003.json" }, StringComparer.Ordinal),
        id + " RS001 evidence path mismatch.");
}

static HashSet<string> HistoricalOmissions() => new(StringComparer.Ordinal)
{
    "M0-W01-C01-T1-R",
    "M0-W01-C01-T1-G",
    "M0-W01-C01-T1-V",
    "M0-W01-C01-T2-R",
    "M0-W01-C01-T2-G",
    "M0-W01-C01-T2-V"
};

static void Add(IDictionary<string, ExpectedNode> target, string id, string kind, string? parentId, string[] dependsOn)
{
    Require(target.TryAdd(id, new ExpectedNode(kind, parentId, dependsOn)), "Duplicate expected product id " + id + ".");
}

static void RequireState(IReadOnlyDictionary<string, ActualNode> nodes, string id, string state) =>
    RequireExactState(nodes[id], state);

static void RequireExactState(ActualNode node, string state) =>
    Require(node.State == state, $"State mismatch for {node.Id}: {node.State} != {state}.");

static string State(IReadOnlyDictionary<string, ActualNode> nodes, string id) => nodes[id].State;

static void EqualSet(IEnumerable<string> actual, IEnumerable<string> expected, string message)
{
    var a = actual.OrderBy(x => x, StringComparer.Ordinal).ToArray();
    var e = expected.OrderBy(x => x, StringComparer.Ordinal).ToArray();
    Require(a.SequenceEqual(e, StringComparer.Ordinal), message + $" actual=[{string.Join(",", a)}] expected=[{string.Join(",", e)}]");
}

static JsonObject LoadObject(string path) =>
    JsonNode.Parse(File.ReadAllText(path))?.AsObject()
    ?? throw new InvalidOperationException("Invalid or empty JSON: " + path);

static IEnumerable<JsonObject> Objects(JsonObject root, string key) =>
    (root[key] as JsonArray ?? throw new InvalidOperationException("Missing array " + key))
    .Select(x => x?.AsObject() ?? throw new InvalidOperationException("Null object in " + key));

static IEnumerable<JsonObject> ObjectsOptional(JsonObject root, string key) =>
    root[key] is JsonArray array
        ? array.Select(x => x?.AsObject() ?? throw new InvalidOperationException("Null object in " + key))
        : Enumerable.Empty<JsonObject>();

static string S(JsonObject root, string key) =>
    root[key]?.GetValue<string>() is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException("Missing string " + key);

static string[] Strings(JsonObject root, string key) =>
    (root[key] as JsonArray ?? throw new InvalidOperationException("Missing array " + key))
    .Select(x => x?.GetValue<string>() ?? throw new InvalidOperationException("Null string in " + key))
    .ToArray();

static string[] StringsOptional(JsonObject root, string key) =>
    root[key] is JsonArray array
        ? array.Select(x => x?.GetValue<string>() ?? throw new InvalidOperationException("Null string in " + key)).ToArray()
        : [];

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed record ExpectedNode(string Kind, string? ParentId, string[] DependsOn);
sealed record ActualNode(string Id, string Title, string Kind, string? ParentId, string State, string[] DependsOn, JsonObject Object);
