#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

const string PathToProposal = "docs/planning/proposals/math-next-roadmap-m8-m12.json";
const string MarkdownPath = "docs/planning/proposals/math-next-roadmap-m8-m12.md";
using var document = JsonDocument.Parse(File.ReadAllBytes(PathToProposal));
var root = document.RootElement;
Require(root.GetProperty("schema_version").GetInt32() == 1, "schema");
Require(Value(root, "id") == "MATH-NEXT-M8-M12", "identity");
Require(root.GetProperty("accepted_direction").GetBoolean() &&
    root.GetProperty("canonical_backlog_unchanged").GetBoolean() &&
    root.GetProperty("new_milestones_not_yet_inserted").GetBoolean(), "planning nonclaims");
Require(Value(root, "canonical_backlog_version") == "0.1.122", "baseline backlog");
string[] order = ["M8", "M9", "M10", "M11", "M12"];
Require(root.GetProperty("order").EnumerateArray().Select(x => x.GetString()).SequenceEqual(order), "approved order");
var milestones = root.GetProperty("milestones").EnumerateArray().ToArray();
Require(milestones.Length == 5 && milestones.Select(x => Value(x, "id")).SequenceEqual(order), "milestone scope");
Require(milestones.All(x => x.GetProperty("work_streams").GetArrayLength() == 2), "bounded work streams");
Require(milestones.All(x => x.GetProperty("work_streams").EnumerateArray().All(y =>
    y.GetProperty("acceptance").GetArrayLength() >= 2)), "admitted acceptance descriptions");
var gate = root.GetProperty("initial_gate");
Require(gate.GetProperty("discovery_fixture_count").GetInt32() == 13, "pinned conformance");
Require(gate.GetProperty("workflow_source_observation").GetProperty("methods").GetArrayLength() == 2, "typed source candidates");
Require(File.ReadAllText(MarkdownPath).Contains("not yet admitted to canonical backlog", StringComparison.Ordinal), "unadmitted roadmap");
Console.WriteLine("MATH-ROADMAP-RS179|milestones=5|streams=10|prior=preserved|native-change=false");
static string Value(JsonElement node, string key) => node.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing field " + key);
static void Require(bool valid, string message)
{
    if (!valid) throw new InvalidDataException("RS179 roadmap: " + message);
}
