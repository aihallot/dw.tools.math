using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Milestone = "M6", Work = "M6-W02", Phase = "M6-W02-C03-S01";
const string Task = "M6-W02-C03-S01-T1";
const string Red = "M6-W02-C03-S01-T1-R", Green = "M6-W02-C03-S01-T1-G";
const string Verify = "M6-W02-C03-S01-T1-V";
const string State = ".aura/workflow/state.json", Backlog = "docs/planning/backlog.json";
const string Prior = "docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json";
const string Split = "docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json";
const string RedProof = "docs/planning/evidence/M6-W02-C03-S01-T1-contract-red.json";
const string Qualified = "docs/planning/evidence/M6-W02-C03-S01-T1-qualified.json";
const string QualifiedStatus = "bounded-finite-2d-3d-euclidean-geometry-local-nuget-qualified";
const string Version = "1.1.0-preview.5", Solution = "dw.tools.math.slnx";
const string Product = "src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj";
const string Source = "src/projects/dw.tools.math.scientific/FiniteGeometry64.cs";
const string Test = "tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj";
const string RedTest = "tests/projects/dw.tools.math.scientific.tests/M6W02C03S01T1RedTests.cs";
const string GreenTests = "tests/projects/dw.tools.math.scientific.tests/M6W02C03S01T1Tests.cs";
const string PriorConsumer = "tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj";
const string PriorConsumerSource = "tests/consumers/m6-w02-c02-t2/Program.cs";
const string PriorConsumerConfig = "tests/consumers/m6-w02-c02-t2/NuGet.Config";
const string Consumer = "tests/consumers/m6-w02-c03-s01-t1/m6-finite-geometry-consumer.csproj";
const string ConsumerSource = "tests/consumers/m6-w02-c03-s01-t1/Program.cs";
const string ConsumerConfig = "tests/consumers/m6-w02-c03-s01-t1/NuGet.Config";
const string Decision = "docs/planning/decisions/m6-w02-c03-s01-t1-finite-euclidean.md";
const string Recipe = "docs/distribution/m6-finite-euclidean-t1.md";
const string PackageHelper = ".workflow/runs/RS093/staged/checks/m6-w02-c03-s01-t1-package-sha.cs";
const string RedMarker = "M6-W02-C03-S01-T1 RED: finite 2D/3D Euclidean geometry public contract is absent.";
const string ConsumerMarker = "dw.tools.math/m6-finite-geometry-consumer/1.1 qualified; probes=8";
const string PriorMarker = "dw.tools.math/m6-special-function-boundary-consumer/1.1 qualified; probes=8";
const int GreenCount = 24, TotalTargeted = 25, C02T2 = 88, C02T1 = 80;
const int C01T2 = 88, C01T1 = 70;

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var target = File.Exists(Path.Combine(root, Qualified));
p.Workflow.RequireCurrentRun("RS093", Milestone, Work, target ? Phase : null, null, null);
p.Workflow.RequirePriorSuccess("RS092", 1,
    "docs: split M6 W02 C03 scientific scope into three native DWF phases",
    "0.1.59", 1, 1, 0, false);

p.Require.JsonValueEquals(Prior, "/status",
    JsonValue.Create("bounded-managed-special-angle-gamma-branch-risk-audit-qualified"));
p.Require.JsonValueEquals(Prior, "/local_product_version", JsonValue.Create("1.1.0-preview.4"));
p.Require.JsonValueEquals(Prior, "/m6_w02_c02_phase_done", JsonValue.Create(true));
p.Require.JsonValueEquals(Split, "/status",
    JsonValue.Create("m6-w02-c03-size-l-split-native-topology-qualified-no-product"));
p.Require.JsonValueEquals(Split, "/native_node_count", JsonValue.Create(523));
p.Require.JsonValueEquals(Split, "/product_qualified", JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog, "/plan_version",
    JsonValue.Create("0.1.91"), JsonValue.Create("0.1.92"));
p.Require.JsonValueEquals(Backlog, "/plan_version",
    JsonValue.Create(target ? "0.1.92" : "0.1.91"));

foreach (var (pointer, value) in new (string, string)[]
{
    ("/releases/5/status", "done"), ("/releases/6/status", "in_progress"),
    ("/work_packages/13/status", "done"), ("/work_packages/14/status", "in_progress"),
    ("/chunks/43/status", "done"), ("/chunks/44/status", "done"),
    ("/chunks/45/id", "M6-W02-C03"), ("/chunks/45/status", "planned"),
    ("/chunks/46/id", Phase),
    ("/chunks/46/status", target ? "in_progress" : "planned"),
    ("/chunks/46/tasks/0/status", target ? "done" : "planned"),
    ("/chunks/46/tasks/1/status", "planned"),
    ("/chunks/47/id", "M6-W02-C03-S02"),
    ("/chunks/47/status", "planned"),
    ("/chunks/48/id", "M6-W02-C03-S03"),
    ("/chunks/48/status", "planned")
}) p.Require.JsonValueEquals(Backlog, pointer, JsonValue.Create(value));
foreach (var index in new[] { 0, 1, 2 })
    p.Require.JsonValueEquals(Backlog, "/chunks/46/tasks/0/subtasks/" + index + "/status",
        JsonValue.Create(target ? "done" : "planned"));
p.Require.TextContains(Product, target ?
    "<Version>1.1.0-preview.5</Version>" : "<Version>1.1.0-preview.4</Version>");
p.Require.TextContains(PriorConsumer, target ?
    "Version=\"[1.1.0-preview.5]\"" : "Version=\"[1.1.0-preview.4]\"");
p.Require.TextContains(PriorConsumerSource, target ?
    "Version=\"1.1.0-preview.5\"" : "Version=\"1.1.0-preview.4\"");

p.ProjectPlan.RequireNodeState("M5", "done");
p.ProjectPlan.RequireNodeState(Milestone, "in-progress");
p.ProjectPlan.RequireNodeState("M6-W01", "done");
p.ProjectPlan.RequireNodeState(Work, "in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01", "done");
p.ProjectPlan.RequireNodeState("M6-W02-C02", "done");
p.ProjectPlan.RequireNodeState("M6-W02-C03", "not-ready");
p.ProjectPlan.RequireNodeState(Phase, target ? "in-progress" : "not-ready");
foreach (var id in new[] { Task, Red, Green, Verify })
    p.ProjectPlan.RequireNodeState(id, target ? "done" : "not-ready");
p.ProjectPlan.RequireNodeState(Phase + "-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03-S02", "not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03-S03", "not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C04", "not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C05", "not-ready");
if (!target)
{
    foreach (var path in new[]
    {
        Source, RedTest, GreenTests, Consumer, ConsumerSource,
        ConsumerConfig, Decision, Recipe, RedProof, Qualified
    }) p.Require.FileMissing(path);
}
else
{
    p.Require.JsonValueEquals(RedProof, "/status",
        JsonValue.Create("compiled-finite-geometry-public-api-red-observed"));
    p.Require.JsonValueEquals(Qualified, "/status", JsonValue.Create(QualifiedStatus));
    foreach (var (pointer, value) in new (string, JsonNode?)[]
    {
        ("/independent_green_tests_passed", JsonValue.Create(GreenCount)),
        ("/red_recovered_and_green_tests_passed", JsonValue.Create(TotalTargeted)),
        ("/retained_c02_t2_tests_passed", JsonValue.Create(C02T2)),
        ("/retained_c02_t1_tests_passed", JsonValue.Create(C02T1)),
        ("/retained_c01_t2_tests_passed", JsonValue.Create(C01T2)),
        ("/retained_c01_t1_tests_passed", JsonValue.Create(C01T1)),
        ("/geometry_consumer_probes_passed", JsonValue.Create(8)),
        ("/retained_special_consumer_probes_passed", JsonValue.Create(8)),
        ("/local_product_version", JsonValue.Create(Version)),
        ("/m6_w02_c03_s01_t1_done", JsonValue.Create(true)),
        ("/m6_w02_c03_s01_phase_done", JsonValue.Create(false)),
        ("/m6_w02_c03_parent_done", JsonValue.Create(false)),
        ("/m6_w02_work_package_done", JsonValue.Create(false)),
        ("/publicly_published", JsonValue.Create(false))
    }) p.Require.JsonValueEquals(Qualified, pointer, value);
}

var hashes = new Dictionary<string, string>(StringComparer.Ordinal);
var stagedRed = p.Files.ReplaceFromStaged("staged/" + RedTest, RedTest);
hashes[RedTest] = stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing staged geometry RED source hash.");
if (!target)
{
    Required(root, "dotnet", "restore", Solution, "--locked-mode");
    Required(root, "dotnet", "build", Solution, "-c", "Release", "--no-restore");
    var redObservation = Run(root, "dotnet", "test", Test, "-c", "Release",
        "--no-build", "--no-restore",
        "--filter", "FullyQualifiedName~M6W02C03S01T1RedTests.PublicFiniteGeometryNormDistanceAndProjectionContractMustExist",
        "--logger", "console;verbosity=normal");
    if (redObservation.Code == 0 ||
        !redObservation.Output.Contains(RedMarker, StringComparison.Ordinal) ||
        !redObservation.Output.Contains("Failed", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "One compiled missing public API RED must be observed: " + Relevant(redObservation.Output));
    var redProof = new JsonObject
    {
        ["schema_version"] = 1, ["id"] = "M6-W02-C03-S01-T1-contract-red",
        ["status"] = "compiled-finite-geometry-public-api-red-observed",
        ["prior_c03_split"] = Split, ["red_test"] = RedTest,
        ["red_test_sha256"] = hashes[RedTest],
        ["expected_assertion"] = RedMarker,
        ["independent_compiled_red_tests"] = 1,
        ["compiler_failure_is_not_a_red_substitute"] = true
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
}
foreach (var path in new[]
{
    Product, Source, GreenTests, PriorConsumer, PriorConsumerSource,
    Consumer, ConsumerSource, ConsumerConfig, Decision, Recipe
})
{
    var staged = p.Files.ReplaceFromStaged("staged/" + path, path);
    hashes[path] = staged.AfterSha256 ??
        throw new InvalidOperationException("Missing staged finite geometry source SHA: " + path);
}
Required(root, "dotnet", "restore", Solution, "--locked-mode");
Required(root, "dotnet", "build", Solution, "-c", "Release", "--no-restore");
var newTests = Required(root, "dotnet", "test", Test, "-c", "Release",
    "--no-build", "--no-restore", "--filter", "FullyQualifiedName~M6W02C03S01T1",
    "--logger", "console;verbosity=normal");
MustPass(newTests, TotalTargeted, "new geometry RED/GREEN");
foreach (var (filter, count, reason) in new[]
{
    ("M6W02C02T2", C02T2, "retained special boundary"),
    ("M6W02C02T1", C02T1, "retained special base"),
    ("M6W02C01T2", C01T2, "retained exact boundary"),
    ("M6W02C01T1", C01T1, "retained exact integer")
})
{
    var regressionOutput = Required(root, "dotnet", "test", Test,
        "-c", "Release", "--no-build", "--no-restore",
        "--filter", "FullyQualifiedName~" + filter, "--logger", "console;verbosity=normal");
    MustPass(regressionOutput, count, reason);
}
Required(root, "dotnet", "test", Test, "-c", "Release",
    "--no-build", "--no-restore", "--logger", "console;verbosity=minimal");
foreach (var project in new[]
{
    "tests/projects/dw.tools.math.foundation.tests/dw.tools.math.foundation.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "tests/projects/dw.tools.math.numerics.tests/dw.tools.math.numerics.tests.csproj",
    "tests/projects/dw.tools.math.composition.tests/dw.tools.math.composition.tests.csproj"
})
{
    var regression = Required(root, "dotnet", "test", project,
        "-c", "Release", "--no-build", "--no-restore",
        "--logger", "console;verbosity=minimal");
    if (!regression.Contains("Passed", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Math regression not passed: " + project);
}
Required(root, "dotnet", "run", "--file", "docs/planning/ValidateTransferArchitecture.cs");

var feed = Path.Combine(root, "build", "artifacts", "m6-w02-c03-s01-t1-local-feed");
Required(root, "dotnet", "pack", Product, "-c", "Release",
    "--no-build", "--no-restore", "-o", feed);
var archive = Path.Combine(feed, "dw.tools.math.scientific." + Version + ".nupkg");
if (!File.Exists(archive))
    throw new InvalidOperationException("No real locally packed geometry preview.5 archive.");
var inspected = ReadReceipt(
    Required(root, "dotnet", "run", "--file", PackageHelper, "--", archive),
    "M6-W02-C03-S01-T1-PKG-SHA256:");

static void CheckIsolatedConsumer(string root, string project, string config, string version)
{
    var document = XDocument.Load(Path.Combine(root, project));
    var references = document.Descendants()
        .Where(x => x.Name.LocalName == "PackageReference").ToArray();
    if (references.Length != 1 ||
        (string?)references[0].Attribute("Include") != "dw.tools.math.scientific" ||
        (string?)references[0].Attribute("Version") != "[" + version + "]" ||
        document.Descendants().Any(x => x.Name.LocalName == "ProjectReference"))
        throw new InvalidOperationException("Expected one pinned source-free local package: " + project);
    var sources = XDocument.Load(Path.Combine(root, config))
        .Descendants().Single(x => x.Name.LocalName == "packageSources");
    if (!sources.Elements().Any(x => x.Name.LocalName == "clear") ||
        sources.Elements().Any(x => x.Name.LocalName == "add"))
        throw new InvalidOperationException("External NuGet source discovered: " + config);
}
var clients = new (string Project, string Config, string Marker, string Receipt, string Cache)[]
{
    (Consumer, ConsumerConfig, ConsumerMarker,
        "M6-W02-C03-S01-T1-CONSUMER-SHA256:", "m6-w02-c03-s01-t1-geometry-cache"),
    (PriorConsumer, PriorConsumerConfig, PriorMarker,
        "M6-W02-C02-T2-CONSUMER-SHA256:", "m6-w02-c03-s01-t1-retained-cache")
};
var receipts = new List<JsonNode>();
foreach (var client in clients)
{
    CheckIsolatedConsumer(root, client.Project, client.Config, Version);
    var cache = Path.Combine(root, "build", "artifacts", client.Cache);
    Required(root, "dotnet", "restore", client.Project,
        "--source", feed, "--configfile", client.Config, "--packages", cache);
    Required(root, "dotnet", "build", client.Project, "-c", "Release", "--no-restore");
    var output = Required(root, "dotnet", "run", "--project", client.Project,
        "-c", "Release", "--no-build", "--no-restore", "--", archive);
    if (!HasExactLine(output, client.Marker))
        throw new InvalidOperationException("Missing eight-probe source-free consumer receipt: " + client.Project);
    var receipt = ReadReceipt(output, client.Receipt);
    if (receipt.Bytes != inspected.Bytes || receipt.Sha != inspected.Sha)
        throw new InvalidOperationException("Local NuGet archive SHA mismatch across consumers.");
    receipts.Add(new JsonObject
    {
        ["project"] = client.Project, ["marker"] = client.Marker,
        ["package_sha256"] = receipt.Sha, ["package_bytes"] = receipt.Bytes,
        ["independent_probes"] = 8, ["project_references"] = 0
    });
}
if (!target)
{
    var proof = new JsonObject
    {
        ["schema_version"] = 1, ["id"] = "M6-W02-C03-S01-T1-qualified",
        ["status"] = QualifiedStatus, ["prior_c02_qualification"] = Prior,
        ["structural_split_qualification"] = Split,
        ["compiled_missing_public_api_red"] = RedProof,
        ["question"] = "Can a bounded finite Cartesian 2D/3D library return nonpartial approximate norms, distances and orthogonal vector projections with independently verified oracles?",
        ["observation"] = "One compiled missing-public-API RED recovered, 24 GREEN, 326 retained scientific tests, four Math regressions, one real local preview.5 archive and two SHA-equal eight-probe NuGet-only consumers.",
        ["decision"] = "Admit finite 2D/3D binary64 Euclidean norms, distances and projections with typed refusals and declared coordinate/axis budgets; reserve broader boundary and conditioning gate for T2.",
        ["source"] = Source, ["source_sha256"] = hashes[Source],
        ["product_project"] = Product, ["product_project_sha256"] = hashes[Product],
        ["red_test"] = RedTest, ["red_test_sha256"] = hashes[RedTest],
        ["green_tests"] = GreenTests, ["green_tests_sha256"] = hashes[GreenTests],
        ["retained_consumer_project"] = PriorConsumer,
        ["retained_consumer_project_sha256"] = hashes[PriorConsumer],
        ["retained_consumer_source"] = PriorConsumerSource,
        ["retained_consumer_source_sha256"] = hashes[PriorConsumerSource],
        ["geometry_consumer_project"] = Consumer,
        ["geometry_consumer_project_sha256"] = hashes[Consumer],
        ["geometry_consumer_source"] = ConsumerSource,
        ["geometry_consumer_source_sha256"] = hashes[ConsumerSource],
        ["geometry_consumer_config"] = ConsumerConfig,
        ["geometry_consumer_config_sha256"] = hashes[ConsumerConfig],
        ["decision_document"] = Decision, ["decision_document_sha256"] = hashes[Decision],
        ["distribution_document"] = Recipe, ["distribution_document_sha256"] = hashes[Recipe],
        ["compiled_red_observed"] = true,
        ["independent_green_tests_passed"] = GreenCount,
        ["red_recovered_and_green_tests_passed"] = TotalTargeted,
        ["retained_c02_t2_tests_passed"] = C02T2,
        ["retained_c02_t1_tests_passed"] = C02T1,
        ["retained_c01_t2_tests_passed"] = C01T2,
        ["retained_c01_t1_tests_passed"] = C01T1,
        ["regression_projects_passed"] = 4,
        ["geometry_consumer_probes_passed"] = 8,
        ["retained_special_consumer_probes_passed"] = 8,
        ["actual_inspected_package_bytes"] = inspected.Bytes,
        ["actual_inspected_package_sha256"] = inspected.Sha,
        ["isolated_consumer_receipts"] = p.Json.Array(receipts.ToArray()),
        ["local_product_version"] = Version,
        ["finite_dimensions"] = "Cartesian 2D or 3D",
        ["max_abs_coordinate"] = 1e100d,
        ["minimum_projection_axis_max"] = 1e-100d,
        ["certified_error_bounds"] = false,
        ["implicit_units_or_geodesy"] = false,
        ["publicly_published"] = false,
        ["m6_w02_c03_s01_t1_done"] = true,
        ["m6_w02_c03_s01_phase_done"] = false,
        ["m6_w02_c03_parent_done"] = false,
        ["m6_w02_work_package_done"] = false,
        ["m6_release_done"] = false,
        ["dotnet_sdk"] = Required(root, "dotnet", "--version").Trim(),
        ["observed_runtime_identifier"] = RuntimeInformation.RuntimeIdentifier,
        ["next_scope"] = "M6-W02-C03-S01-T2 independently qualify cancellation, dimension, near-zero axis, finite conditioning and consumer/refusal boundary audits.",
        ["nonclaims"] = p.Json.StringArray(
            "Binary64 observations have no established error-bound theorem and are not exact norms or projections.",
            "Inputs are caller-managed Cartesian coordinates with common linear units; no geodesic or unit-conversion semantics.",
            "Near-zero, cancellation, dimensional, source-free package and conditioning edge gates remain accepted S01 T2 work.",
            "The S01 phase, S02/S03 children, original C03, M6-W02 and M6 remain unfinished.",
            "No public package, untested target OS, AURA, Decision or MCDM adoption is certified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
    p.Json.EditObject(Backlog, doc =>
    {
        doc["plan_version"] = "0.1.92";
        var phase = doc["chunks"]!.AsArray().Select(x => x!.AsObject())
            .Single(x => (string?)x["id"] == Phase);
        var task = phase["tasks"]!.AsArray().Select(x => x!.AsObject())
            .Single(x => (string?)x["id"] == Task);
        if ((string?)phase["status"] != "planned" || (string?)task["status"] != "planned")
            throw new InvalidOperationException("Accepted geometry T1 status changed.");
        phase["status"] = "in_progress";
        phase["refinement"] = "RS092 split created this separate geometry child. RS093 T1 qualifies bounded binary64 finite 2D/3D norm, distance and projection, one compiled missing API RED, 24 GREEN, retained scientific contracts and two independently SHA-equal local preview.5 NuGet-only clients; T2 and aggregate gates remain open.";
        phase["readiness"] = "T1 qualified with limited finite Cartesian examples; T2 detailed boundary, conditioning and full integration remains planned. No parent C03 or release completion.";
        var files = phase["files"]!.AsArray().Select(x => x!.GetValue<string>());
        phase["files"] = p.Json.StringArray(files.Concat(new[]
        {
            Product, Source, RedTest, GreenTests, PriorConsumer, PriorConsumerSource,
            Consumer, ConsumerSource, ConsumerConfig, Decision, Recipe, RedProof, Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"] = p.Json.StringArray(
            "dotnet restore " + Solution + " --locked-mode",
            "dotnet build " + Solution + " -c Release --no-restore",
            "dotnet test " + Test + " -c Release --filter FullyQualifiedName~M6W02C03S01T1",
            "dotnet test " + Test + " -c Release",
            "dotnet run --project " + Consumer + " -c Release",
            "dotnet run --project " + PriorConsumer + " -c Release");
        var evidence = phase["evidence"]!.AsArray().Select(x => x!.GetValue<string>());
        phase["evidence"] = p.Json.StringArray(evidence.Concat(new[] { RedProof, Qualified })
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"] = "done";
        task["evidence"] = p.Json.StringArray(RedProof, Qualified);
        foreach (var item in task["subtasks"]!.AsArray().Select(x => x!.AsObject()))
        {
            item["status"] = "done";
            item["evidence"] = p.Json.StringArray(
                (string?)item["id"] == Red ? RedProof : Qualified);
        }
    });
    foreach (var id in new[] { Phase, Task, Red })
        p.ProjectPlan.TransitionNode(id, "not-ready", "ready");
    foreach (var id in new[] { Phase, Task, Red })
        p.ProjectPlan.TransitionNode(id, "ready", "in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach (var id in new[] { Green, Verify })
    {
        p.ProjectPlan.TransitionNode(id, "not-ready", "ready");
        p.ProjectPlan.TransitionNode(id, "ready", "in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState(Phase, "in-progress");
foreach (var id in new[] { Task, Red, Green, Verify })
    p.ProjectPlan.RequireNodeState(id, "done");
p.ProjectPlan.RequireNodeState(Phase + "-T2", "not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C03", "not-ready");
p.ProjectPlan.RequireNodeState(Work, "in-progress");
p.ProjectPlan.RequireNodeState(Milestone, "in-progress");
p.Json.EditObject(State, state =>
{
    if ((string?)state["pendingRun"] != "RS093" ||
        (string?)state["activeMilestone"] != Milestone ||
        (string?)state["activeWorkPackage"] != Work ||
        (string?)state["activePhase"] != (target ? Phase : null) ||
        state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException("Unexpected geometry T1 DWF cursor.");
    state["activePhase"] = Phase;
});
if (!target)
    Required(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
Required(root, "dotnet", "run", "--file", "docs/planning/ValidateNativeDwfAdoption.cs");
Required(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--check");
return p.Complete();

static void MustPass(string output, int expected, string title)
{
    if (!output.Contains("Total tests: " + expected, StringComparison.OrdinalIgnoreCase) ||
        !output.Contains("Passed: " + expected, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Expected " + expected + " passing " + title +
            " tests: " + Relevant(output));
}

static bool HasExactLine(string output, string marker) =>
    output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        .Any(x => x.Trim() == marker);

static (long Bytes, string Sha) ReadReceipt(string output, string marker)
{
    const string receiptVersion = "1.1.0-preview.5";
    var prefix = marker + "dw.tools.math.scientific:" + receiptVersion + ":";
    var matches = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        .Where(x => x.StartsWith(prefix, StringComparison.Ordinal)).ToArray();
    if (matches.Length != 1)
        throw new InvalidOperationException("Exactly one package byte/SHA-256 receipt required.");
    var parts = matches[0][prefix.Length..].Split(':');
    if (parts.Length != 2 ||
        !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var bytes) ||
        bytes <= 0 || parts[1].Length != 64 ||
        parts[1].Any(ch => !((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f'))))
        throw new InvalidOperationException("Malformed NuGet archive SHA-256 receipt.");
    return (bytes, parts[1]);
}

static string Relevant(string output)
{
    var index = output.IndexOf("Error Message:", StringComparison.Ordinal);
    if (index < 0) index = output.IndexOf("error ", StringComparison.OrdinalIgnoreCase);
    var relevant = index < 0 ? output : output[index..];
    return relevant.Length > 2300 ? relevant[..2300] : relevant;
}

static (int Code, string Output) Run(string root, string executable, params string[] args)
{
    using var process = new Process { StartInfo = new ProcessStartInfo
    {
        FileName = executable, WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var argument in args) process.StartInfo.ArgumentList.Add(argument);
    if (!process.Start()) throw new InvalidOperationException("Cannot start " + executable);
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("Scientific geometry qualification process exceeded timeout.");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode, output);
}

static string Required(string root, string executable, params string[] args)
{
    var result = Run(root, executable, args);
    if (result.Code != 0)
        throw new InvalidOperationException(executable + " " + string.Join(" ", args) +
            " exited " + result.Code + ": " + Relevant(result.Output));
    return result.Output;
}
