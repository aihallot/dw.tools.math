#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

const string Contract = "docs/planning/proposals/m8-admission-contract.json";
const string Roadmap = "docs/planning/proposals/math-next-roadmap-m8-m12.json";
const string Backlog = "docs/planning/backlog.json";
const string Native = ".aura/workflow/plan/project.json";
using var d = JsonDocument.Parse(File.ReadAllBytes(Contract));
var root = d.RootElement;
using var b = JsonDocument.Parse(File.ReadAllBytes(Backlog));
using var n = JsonDocument.Parse(File.ReadAllBytes(Native));
using var roadmap = JsonDocument.Parse(File.ReadAllBytes(Roadmap));
Need(root.GetProperty("id").GetString() == "M8-ADMISSION-CONTRACT", "identity");
Need(root.GetProperty("status").GetString() == "qualified-design-pending-canonical-admission", "unadmitted state");
Need(b.RootElement.GetProperty("plan_version").GetString() == "0.1.122", "baseline backlog");
Need(b.RootElement.GetProperty("releases").GetArrayLength() == 8, "8 accepted releases");
Need(n.RootElement.GetProperty("milestones").GetArrayLength() == 8, "8 existing native milestones");
Need(roadmap.RootElement.GetProperty("new_milestones_not_yet_inserted").GetBoolean(), "proposal unchanged");
var a = root.GetProperty("admission");
Need(a.GetProperty("expectedAddedNodes").GetInt32() == 39 &&
     a.GetProperty("expectedTotalNativeNodes").GetInt32() == 580, "node arithmetic");
Need(a.GetProperty("initialDwfState").GetString() == "not-ready", "initial state");
var nodes = root.GetProperty("nodes").EnumerateArray().ToArray();
Need(nodes.Length == 39, "exact node count");
var indexed = nodes.ToDictionary(x => x.GetProperty("id").GetString() ?? throw new InvalidDataException("id"));
Need(indexed.Count == 39 && indexed.ContainsKey("M8"), "unique node ids");
var kindCount = nodes.GroupBy(x => x.GetProperty("kind").GetString()).ToDictionary(x => x.Key ?? "", x => x.Count());
Need(kindCount.GetValueOrDefault("milestone") == 1 &&
     kindCount.GetValueOrDefault("workPackage") == 2 &&
     kindCount.GetValueOrDefault("phase") == 4 &&
     kindCount.GetValueOrDefault("task") == 8 &&
     kindCount.GetValueOrDefault("subtask") == 24, "node distribution");
foreach (var node in nodes)
{
    var id = node.GetProperty("id").GetString() ?? "";
    var parent = node.GetProperty("parent");
    Need(node.GetProperty("state").GetString() == "not-ready", "node state " + id);
    if (id == "M8") Need(parent.ValueKind == JsonValueKind.Null, "root parent");
    else Need(parent.ValueKind == JsonValueKind.String &&
        indexed.ContainsKey(parent.GetString() ?? ""), "node parent " + id);
    foreach (var dep in node.GetProperty("dependsOn").EnumerateArray())
    {
        var depId = dep.GetString() ?? "";
        Need(depId == "M7" || indexed.ContainsKey(depId), "dependency " + id);
    }
}
Need(Directory.Exists("docs/planning/proposals"), "proposal folder");
Console.WriteLine("MATH-M8-ADMISSION|design=qualified|new=39|target=580|native=unchanged");
static void Need(bool ok, string error) { if (!ok) throw new InvalidDataException("M8 design: " + error); }
