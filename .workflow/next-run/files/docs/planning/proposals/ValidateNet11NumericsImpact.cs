#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

const string Report = "docs/planning/proposals/net11-numerics-impact-matrix.json";
const string Narrative = "docs/planning/proposals/net11-numerics-impact.md";
using var d = JsonDocument.Parse(File.ReadAllBytes(Report));
var root = d.RootElement;
Need(root.GetProperty("id").GetString() == "MATH-NET11-NUMERICS-IMPACT", "identity");
Need(root.GetProperty("status").GetString() == "research-proposal-not-migrated", "non-delivery state");
Need(root.GetProperty("baseline").GetProperty("backlog").GetString() == "0.1.123", "backlog baseline");
Need(root.GetProperty("baseline").GetProperty("net10Preserved").GetBoolean(), "immutable baseline");
Need(root.GetProperty("sources").GetArrayLength() >= 4, "primary sources");
var rows = root.GetProperty("candidates").EnumerateArray().ToArray();
Need(rows.Length == 9, "nine independent candidates");
Need(rows.Select(x => x.GetProperty("id").GetString()).Distinct().Count() == rows.Length, "unique candidates");
Need(rows.All(x => x.GetProperty("feature").GetString()?.Length > 2 &&
                   x.GetProperty("action").GetString()?.Length > 10 &&
                   x.GetProperty("migration").GetString()?.Length > 10), "bounded proposed action");
Need(root.GetProperty("gates").GetArrayLength() >= 6, "qualification gates");
Need(File.ReadAllText(Narrative).Contains("not code migration", StringComparison.Ordinal), "document nonclaim");
Console.WriteLine("MATH-RS183-NUMERICS|candidates=9|net10=preserved|migration=none");
static void Need(bool valid, string message) { if (!valid) throw new InvalidDataException("RS183: " + message); }
