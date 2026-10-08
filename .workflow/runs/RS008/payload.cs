using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string SourceRelativePath = "lib/dw.quantities/ExactRational.cs";
const string TargetRelativePath = "src/projects/dw.quantities/ExactRational.cs";
const string ExpectedSha256 = "3c8d3beaa3b04884ca8424b5bbc6ee5f2d87b4adf1f6e329a7047bd974b1405a";

var payload = PayloadContext.Create();

InstallAuthorizedSource(payload);
payload.Files.ReplaceFromStaged("staged/dw.tools.math.slnx", "dw.tools.math.slnx");
payload.Files.ReplaceFromStaged("staged/tests/projects/dw.quantities.tests/M1W01C01Tests.cs", "tests/projects/dw.quantities.tests/M1W01C01Tests.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/ValidateM1W01C01Green.cs", "docs/planning/ValidateM1W01C01Green.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/backlog.json", "docs/planning/backlog.json");

RunDotNet(payload.RepositoryRoot, "run", "--file", "docs/planning/ValidateM1W01C01Green.cs", "--", "--capture");
RunDotNet(payload.RepositoryRoot, "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
ConvergeNativeProgress(payload);

return payload.Complete();

static void InstallAuthorizedSource(PayloadContext payload)
{
    var repositoryRoot = payload.RepositoryRoot;
    var baselinePath = Path.Combine(repositoryRoot, "docs", "planning", "source-baseline.json");
    var baseline = JsonNode.Parse(File.ReadAllText(baselinePath))?.AsObject()
        ?? throw new InvalidOperationException("Source baseline is empty.");

    var aura = (baseline["repositories"] as JsonArray
        ?? throw new InvalidOperationException("Source baseline has no repositories."))
        .Select(x => x?.AsObject() ?? throw new InvalidOperationException("Null source repository."))
        .Single(x => x["repository"]?.GetValue<string>() == "aura");

    var sourceRoot = aura["observed_root"]?.GetValue<string>();
    if (string.IsNullOrWhiteSpace(sourceRoot))
        throw new InvalidOperationException("AURA observed_root is missing.");

    var sourceEntry = (aura["files"] as JsonArray
        ?? throw new InvalidOperationException("AURA baseline has no files."))
        .Select(x => x?.AsObject() ?? throw new InvalidOperationException("Null AURA source file."))
        .Single(x => x["path"]?.GetValue<string>() == SourceRelativePath);

    var expected = sourceEntry["sha256"]?.GetValue<string>();
    if (!string.Equals(expected, ExpectedSha256, StringComparison.Ordinal))
        throw new InvalidOperationException("Pinned ExactRational source hash changed.");

    var sourcePath = Path.Combine(sourceRoot, SourceRelativePath.Replace('/', Path.DirectorySeparatorChar));
    if (!File.Exists(sourcePath))
        throw new InvalidOperationException("Authorized ExactRational source is not available at the pinned observed root: " + sourcePath);

    var bytes = File.ReadAllBytes(sourcePath);
    var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if (!string.Equals(actual, ExpectedSha256, StringComparison.Ordinal))
        throw new InvalidOperationException("Authorized ExactRational source bytes drifted from the pinned SHA-256.");

    var targetPath = Path.Combine(repositoryRoot, TargetRelativePath.Replace('/', Path.DirectorySeparatorChar));
    if (File.Exists(targetPath))
    {
        var existing = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(targetPath))).ToLowerInvariant();
        if (!string.Equals(existing, ExpectedSha256, StringComparison.Ordinal))
            throw new InvalidOperationException("Math ExactRational.cs exists with undeclared third-state bytes.");
        payload.Files.WriteComplete(TargetRelativePath, new System.Text.UTF8Encoding(false, true).GetString(bytes));
        return;
    }

    var content = new System.Text.UTF8Encoding(false, true).GetString(bytes);
    payload.Files.WriteComplete(TargetRelativePath, content);
    var installed = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(targetPath))).ToLowerInvariant();
    if (!string.Equals(installed, ExpectedSha256, StringComparison.Ordinal))
        throw new InvalidOperationException("Math ExactRational.cs did not preserve the pinned source bytes.");
}

static void ConvergeNativeProgress(PayloadContext payload)
{
    var ids = new[]
    {
        "M1", "M1-W01", "M1-W01-C01",
        "M1-W01-C01-T1", "M1-W01-C01-T1-R", "M1-W01-C01-T1-G", "M1-W01-C01-T1-V",
        "M1-W01-C01-T2", "M1-W01-C01-T2-R"
    };
    var states = ReadStates(payload.RepositoryRoot, ids);

    var baseline =
        states["M1"] == "in-progress" &&
        states["M1-W01"] == "in-progress" &&
        states["M1-W01-C01"] == "in-progress" &&
        states["M1-W01-C01-T1"] == "in-progress" &&
        states["M1-W01-C01-T1-R"] == "done" &&
        states["M1-W01-C01-T1-G"] == "ready" &&
        states["M1-W01-C01-T1-V"] == "not-ready" &&
        states["M1-W01-C01-T2"] == "not-ready" &&
        states["M1-W01-C01-T2-R"] == "not-ready";

    var target =
        states["M1"] == "in-progress" &&
        states["M1-W01"] == "in-progress" &&
        states["M1-W01-C01"] == "in-progress" &&
        states["M1-W01-C01-T1"] == "done" &&
        states["M1-W01-C01-T1-R"] == "done" &&
        states["M1-W01-C01-T1-G"] == "done" &&
        states["M1-W01-C01-T1-V"] == "done" &&
        states["M1-W01-C01-T2"] == "ready" &&
        states["M1-W01-C01-T2-R"] == "ready";

    if (target)
    {
        foreach (var id in ids) payload.ProjectPlan.RequireNodeState(id, states[id]);
        return;
    }

    if (!baseline)
        throw new InvalidOperationException("M1-W01-C01 GREEN is neither the declared RS008 baseline nor exact target.");

    payload.ProjectPlan.ActivateReadyContinuation("M1-W01-C01-T1-G");
    payload.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T1-G");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1-V", "not-ready", "ready");
    payload.ProjectPlan.ActivateReadyContinuation("M1-W01-C01-T1-V");
    payload.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T1-V");
    payload.ProjectPlan.ConvergeNodeToDone("M1-W01-C01-T1");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T2", "not-ready", "ready");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T2-R", "not-ready", "ready");
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
    if (!p.WaitForExit(420_000))
    {
        p.Kill(entireProcessTree: true);
        throw new TimeoutException("dotnet command exceeded 420 seconds.");
    }
    var so = stdout.GetAwaiter().GetResult();
    var se = stderr.GetAwaiter().GetResult();
    if (!string.IsNullOrWhiteSpace(so)) Console.Write(so);
    if (!string.IsNullOrWhiteSpace(se)) Console.Error.Write(se);
    if (p.ExitCode != 0)
        throw new InvalidOperationException("dotnet command failed with exit code " + p.ExitCode + ".");
}
