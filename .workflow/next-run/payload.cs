using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var payload = PayloadContext.Create();

payload.Files.ReplaceFromStaged(
    "staged/docs/planning/ValidateNativeDwfAdoption.cs",
    "docs/planning/ValidateNativeDwfAdoption.cs");
payload.Files.ReplaceFromStaged(
    "staged/docs/planning/decisions/m0-w01-c02.md",
    "docs/planning/decisions/m0-w01-c02.md");

ConvergeC02(payload);

return payload.Complete();

static void ConvergeC02(PayloadContext payload)
{
    var ids = new[]
    {
        "M0",
        "M0-W01",
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
    var states = ReadStates(payload.RepositoryRoot, ids);

    var baseline =
        states["M0"] == "in-progress" &&
        states["M0-W01"] == "in-progress" &&
        states["M0-W01-C02"] == "ready" &&
        states["M0-W01-C02-T1"] == "ready" &&
        states["M0-W01-C02-T1-A"] == "ready" &&
        states["M0-W01-C02-T1-B"] == "not-ready" &&
        states["M0-W01-C02-T1-C"] == "not-ready" &&
        states["M0-W01-C02-T2"] == "not-ready" &&
        states["M0-W01-C02-T2-A"] == "not-ready" &&
        states["M0-W01-C02-T2-B"] == "not-ready" &&
        states["M0-W01-C02-T2-C"] == "not-ready";

    var target =
        states["M0"] == "in-progress" &&
        states["M0-W01"] == "done" &&
        ids.Skip(2).All(id => states[id] == "done");

    if (target)
    {
        payload.ProjectPlan.RequireNodeState("M0", "in-progress");
        payload.ProjectPlan.RequireNodeState("M0-W01", "done");
        foreach (var id in ids.Skip(2))
            payload.ProjectPlan.RequireNodeState(id, "done");
        return;
    }

    if (!baseline)
        throw new InvalidOperationException("M0-W01-C02 is neither the declared RS002 baseline nor exact completed target.");

    payload.ProjectPlan.ActivateReadyContinuation("M0-W01-C02-T1-A");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01-C02-T1-A");

    CompleteReadySibling(payload, "M0-W01-C02-T1-B");
    CompleteReadySibling(payload, "M0-W01-C02-T1-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01-C02-T1");

    payload.ProjectPlan.TransitionNode("M0-W01-C02-T2", "not-ready", "ready");
    payload.ProjectPlan.TransitionNode("M0-W01-C02-T2-A", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation("M0-W01-C02-T2-A");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01-C02-T2-A");

    CompleteReadySibling(payload, "M0-W01-C02-T2-B");
    CompleteReadySibling(payload, "M0-W01-C02-T2-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01-C02-T2");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01-C02");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W01");
}

static void CompleteReadySibling(PayloadContext payload, string id)
{
    payload.ProjectPlan.TransitionNode(id, "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation(id);
    payload.ProjectPlan.ConvergeNodeToDone(id);
}

static Dictionary<string, string> ReadStates(string repositoryRoot, IEnumerable<string> ids)
{
    var wanted = ids.ToHashSet(StringComparer.Ordinal);
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    var path = Path.Combine(repositoryRoot, ".aura", "workflow", "plan", "project.json");
    var root = JsonNode.Parse(File.ReadAllText(path))
        ?? throw new InvalidOperationException("Native DWF project plan is empty.");
    Visit(root, wanted, result);
    foreach (var id in wanted)
        if (!result.ContainsKey(id))
            throw new InvalidOperationException("Native DWF project node is missing: " + id + ".");
    return result;
}

static void Visit(JsonNode? node, IReadOnlySet<string> wanted, IDictionary<string, string> result)
{
    if (node is JsonObject obj)
    {
        var id = obj["id"]?.GetValue<string>();
        if (id is not null && wanted.Contains(id))
            result[id] = obj["state"]?.GetValue<string>()
                ?? throw new InvalidOperationException("Native DWF node has no state: " + id + ".");
        foreach (var property in obj)
            Visit(property.Value, wanted, result);
        return;
    }

    if (node is JsonArray array)
        foreach (var item in array)
            Visit(item, wanted, result);
}
