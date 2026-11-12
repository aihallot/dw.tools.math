using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Report = "docs/planning/evidence/RS180-dwf-native-topology-capability.json";
var p = PayloadContext.Create();
p.Workflow.RequireCurrentRun("RS180", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS179", 1,
    "docs: qualify proposed M8-M12 Math roadmap and DWF topology gate",
    "0.1.66", 0, 0, 0, true);
foreach (var id in new[] { "M0", "M1", "M2", "M3", "M4", "M5", "M6", "M7" })
    p.ProjectPlan.RequireNodeState(id, "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status", JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version", JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/proposals/math-next-roadmap-m8-m12.json", "/new_milestones_not_yet_inserted", JsonValue.Create(true));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion", JsonValue.Create("0.26"));

var assembly = typeof(PayloadContext).Assembly;
// A statically known SDK type makes the public-method reflection trimming-safe.
const string SpecName = "Dw.Tools.Workflow.Payloads.PayloadProjectPlanNodeSpec";
var methods = typeof(PayloadProjectPlan).GetMethods(BindingFlags.Instance | BindingFlags.Public);
var milestone = methods.SingleOrDefault(m => m.Name == "EnsureMilestoneSubtree" &&
    m.GetParameters() is [{ ParameterType: var argument }] &&
    argument.FullName == SpecName);
var package = methods.SingleOrDefault(m => m.Name == "EnsureWorkPackageSubtree" &&
    m.GetParameters() is [{ ParameterType: var owner }, { ParameterType: var argument }] &&
    owner == typeof(string) && argument.FullName == SpecName);
var specAvailable = milestone is not null || package is not null;
var qualified = milestone is not null && package is not null;
var report = new JsonObject
{
    ["schemaVersion"] = 1,
    ["kind"] = "math.dwf-native-topology-capability",
    ["runId"] = "RS180",
    ["status"] = qualified ? "typed-milestone-authoring-observed" : "typed-milestone-authoring-unavailable",
    ["executorAssembly"] = assembly.GetName().Name,
    ["executorAssemblyVersion"] = assembly.GetName().Version?.ToString() ?? "unreported",
    ["milestoneOperationAvailable"] = milestone is not null,
    ["workPackageOperationAvailable"] = package is not null,
    ["nodeSpecAvailable"] = specAvailable,
    ["supportedForNewMathMilestones"] = qualified,
    ["sourceOnlyClaimExcluded"] = true,
    ["canonicalBacklogUnchanged"] = true,
    ["nativePlanUnchanged"] = true,
    ["proposal"] = "docs/planning/proposals/math-next-roadmap-m8-m12.json",
    ["nextGate"] = qualified
        ? "Admit M8 with a typed complete subtree and reconcile canonical backlog, plan and validators in one subsequent run."
        : "Qualify an updated DWF executable that provides typed milestone authoring before adding M8."
};
p.Files.WriteComplete(Report, report.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
p.Require.JsonValueEquals(Report, "/runId", JsonValue.Create("RS180"));
p.Require.JsonValueEquals(Report, "/canonicalBacklogUnchanged", JsonValue.Create(true));
p.Require.JsonValueEquals(Report, "/nativePlanUnchanged", JsonValue.Create(true));
return p.Complete();
