using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var payload = PayloadContext.Create();

payload.Files.ReplaceFromStaged("staged/docs/planning/backlog.json", "docs/planning/backlog.json");
payload.Files.ReplaceFromStaged("staged/docs/planning/ValidateTransferProvenance.cs", "docs/planning/ValidateTransferProvenance.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/evidence/M0-W02-C01-provenance-audit.json", "docs/planning/evidence/M0-W02-C01-provenance-audit.json");
payload.Files.ReplaceFromStaged("staged/docs/planning/decisions/m0-w02-c01-source-rights.md", "docs/planning/decisions/m0-w02-c01-source-rights.md");

RunDotNet(payload.RepositoryRoot, "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
ConvergeNativeProgress(payload);

return payload.Complete();

static void ConvergeNativeProgress(PayloadContext payload)
{
    var ids = new[]
    {
        "M0", "M0-W02", "M0-W02-C01",
        "M0-W02-C01-T1", "M0-W02-C01-T1-C",
        "M0-W02-C02", "M0-W02-C02-T1", "M0-W02-C02-T1-A"
    };
    var states = ReadStates(payload.RepositoryRoot, ids);

    var baseline =
        states["M0"] == "in-progress" &&
        states["M0-W02"] == "in-progress" &&
        states["M0-W02-C01"] == "in-progress" &&
        states["M0-W02-C01-T1"] == "blocked" &&
        states["M0-W02-C01-T1-C"] == "blocked" &&
        states["M0-W02-C02"] == "not-ready" &&
        states["M0-W02-C02-T1"] == "not-ready" &&
        states["M0-W02-C02-T1-A"] == "not-ready";

    var target =
        states["M0"] == "in-progress" &&
        states["M0-W02"] == "in-progress" &&
        states["M0-W02-C01"] == "done" &&
        states["M0-W02-C01-T1"] == "done" &&
        states["M0-W02-C01-T1-C"] == "done" &&
        states["M0-W02-C02"] == "ready" &&
        states["M0-W02-C02-T1"] == "ready" &&
        states["M0-W02-C02-T1-A"] == "ready";

    if (target)
    {
        foreach (var id in ids)
            payload.ProjectPlan.RequireNodeState(id, states[id]);
        return;
    }

    if (!baseline)
        throw new InvalidOperationException("M0-W02 rights closure is neither the declared RS004 baseline nor exact target.");

    payload.ProjectPlan.TransitionNode("M0-W02-C01-T1", "blocked", "ready");
    payload.ProjectPlan.TransitionNode("M0-W02-C01-T1-C", "blocked", "ready");
    payload.ProjectPlan.ActivateReadyContinuation("M0-W02-C01-T1-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C01-T1-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C01-T1");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C01");

    payload.ProjectPlan.TransitionNode("M0-W02-C02", "not-ready", "ready");
    payload.ProjectPlan.TransitionNode("M0-W02-C02-T1", "not-ready", "ready");
    payload.ProjectPlan.TransitionNode("M0-W02-C02-T1-A", "not-ready", "ready");
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

static void RunDotNet(string repositoryRoot, params string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
    if (!process.Start()) throw new InvalidOperationException("Failed to start dotnet planning generation.");
    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(180_000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet planning generation exceeded 180 seconds.");
    }
    var stdout = stdoutTask.GetAwaiter().GetResult();
    var stderr = stderrTask.GetAwaiter().GetResult();
    if (!string.IsNullOrWhiteSpace(stdout)) Console.Write(stdout);
    if (!string.IsNullOrWhiteSpace(stderr)) Console.Error.Write(stderr);
    if (process.ExitCode != 0)
        throw new InvalidOperationException("dotnet planning generation failed with exit code " + process.ExitCode + ".");
}
