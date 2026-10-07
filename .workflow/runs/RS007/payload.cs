using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

var payload = PayloadContext.Create();

payload.Files.ReplaceFromStaged("staged/src/projects/dw.quantities/dw.quantities.csproj", "src/projects/dw.quantities/dw.quantities.csproj");
payload.Files.ReplaceFromStaged("staged/src/projects/dw.quantities/packages.lock.json", "src/projects/dw.quantities/packages.lock.json");
payload.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/dw.quantities.tests.csproj", "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj");
payload.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/packages.lock.json", "tests/projects/dw.quantities.tests/packages.lock.json");
payload.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/M1W01C01Tests.cs", "tests/projects/dw.quantities.tests/M1W01C01Tests.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/ValidateM1W01C01Red.cs", "docs/planning/ValidateM1W01C01Red.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/backlog.json", "docs/planning/backlog.json");

RunDotNet(payload.RepositoryRoot, "run", "--file", "docs/planning/ValidateM1W01C01Red.cs", "--", "--capture");
RunDotNet(payload.RepositoryRoot, "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
ConvergeNativeProgress(payload);

return payload.Complete();

static void ConvergeNativeProgress(PayloadContext payload)
{
    var ids = new[] { "M1", "M1-W01", "M1-W01-C01", "M1-W01-C01-T1", "M1-W01-C01-T1-R", "M1-W01-C01-T1-G" };
    var states = ReadStates(payload.RepositoryRoot, ids);

    var baseline =
        states["M1"] == "ready" &&
        states["M1-W01"] == "ready" &&
        states["M1-W01-C01"] == "ready" &&
        states["M1-W01-C01-T1"] == "ready" &&
        states["M1-W01-C01-T1-R"] == "ready" &&
        states["M1-W01-C01-T1-G"] == "not-ready";

    var target =
        states["M1"] == "in-progress" &&
        states["M1-W01"] == "in-progress" &&
        states["M1-W01-C01"] == "in-progress" &&
        states["M1-W01-C01-T1"] == "in-progress" &&
        states["M1-W01-C01-T1-R"] == "done" &&
        states["M1-W01-C01-T1-G"] == "ready";

    if (target)
    {
        foreach (var id in ids) payload.ProjectPlan.RequireNodeState(id, states[id]);
        return;
    }

    if (!baseline)
        throw new InvalidOperationException("M1-W01-C01 RED is neither the declared RS007 baseline nor exact target.");

    payload.ProjectPlan.TransitionNode("M1", "ready", "in-progress");
    payload.ProjectPlan.TransitionNode("M1-W01", "ready", "in-progress");
    payload.ProjectPlan.TransitionNode("M1-W01-C01", "ready", "in-progress");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1", "ready", "in-progress");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1-R", "ready", "in-progress");
    payload.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T1-R");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1-G", "not-ready", "ready");
}

static Dictionary<string, string> ReadStates(string root, IEnumerable<string> ids)
{
    var wanted = ids.ToHashSet(StringComparer.Ordinal);
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    var node = JsonNode.Parse(File.ReadAllText(Path.Combine(root, ".aura", "workflow", "plan", "project.json")))
        ?? throw new InvalidOperationException("Native DWF project plan is empty.");
    Visit(node, wanted, result);
    foreach (var id in wanted)
        if (!result.ContainsKey(id))
            throw new InvalidOperationException("Missing native node: " + id);
    return result;
}

static void Visit(JsonNode? node, IReadOnlySet<string> wanted, IDictionary<string, string> result)
{
    if (node is JsonObject o)
    {
        var id = o["id"]?.GetValue<string>();
        if (id is not null && wanted.Contains(id))
            result[id] = o["state"]?.GetValue<string>() ?? throw new InvalidOperationException("Node has no state: " + id);
        foreach (var p in o) Visit(p.Value, wanted, result);
        return;
    }
    if (node is JsonArray a)
        foreach (var x in a) Visit(x, wanted, result);
}

static void RunDotNet(string root, params string[] args)
{
    using var p = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    p.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    p.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    foreach (var a in args) p.StartInfo.ArgumentList.Add(a);
    if (!p.Start()) throw new InvalidOperationException("Failed to start dotnet.");
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    if (!p.WaitForExit(240_000))
    {
        p.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet command exceeded 240 seconds.");
    }
    var so = stdout.GetAwaiter().GetResult();
    var se = stderr.GetAwaiter().GetResult();
    if (!string.IsNullOrWhiteSpace(so)) Console.Write(so);
    if (!string.IsNullOrWhiteSpace(se)) Console.Error.Write(se);
    if (p.ExitCode != 0)
        throw new InvalidOperationException("dotnet command failed with exit code " + p.ExitCode + ".");
}
