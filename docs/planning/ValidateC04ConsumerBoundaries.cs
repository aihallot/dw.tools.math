using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Math-owned, read-only boundary validator. It does not contact external
// repositories, calculate workflow authority hashes or claim adoption.
const string Prior = "docs/planning/evidence/M3-W02-C04-consumer-qualified.json";
const string Matrix = "docs/coordination/matrices/m3-decision-mcdm-primitive-boundaries.json";
const string DecisionRequest = "docs/coordination/requests/MATH-XR-002-decision.md";
const string McdmRequest = "docs/coordination/requests/MATH-XR-003-mcdm.md";
const string DecisionNote = "docs/planning/decisions/m3-w02-c04-t2-boundary-decisions.md";
const string Handoff = "docs/distribution/m3-w02-consumer-handoff.md";
const string ExpectedCommit = "9bbbdde4f0c8a2df30f585f5b1a20e5f6d514cc3";
const string ExpectedStatus = "isolated-math-packages-and-primitive-proposals-qualified";

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException("C04 owner boundary validation: " + message);
}
static string Read(string relative)
{
    var root = Directory.GetCurrentDirectory();
    return File.ReadAllText(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar)));
}
static string Str(JsonElement element, string property) => element.GetProperty(property).GetString() ??
    throw new InvalidOperationException("Required nonnull JSON string: " + property);
static bool IsLowerSha256(string value) =>
    value.Length == 64 &&
    value.All(ch => ch is >= '0' and <= '9' or >= 'a' and <= 'f');

using var priorDoc = JsonDocument.Parse(Read(Prior));
using var matrixDoc = JsonDocument.Parse(Read(Matrix));
var prior = priorDoc.RootElement;
var matrix = matrixDoc.RootElement;

Require(Str(prior, "status") == ExpectedStatus, "RS050 consumer evidence must already be qualified.");
Require(prior.GetProperty("consumer_oracles_passed").GetInt32() == 31, "31 RS050 independent consumer probes are required.");
Require(prior.GetProperty("package_only_consumer").GetBoolean() &&
        prior.GetProperty("no_project_references").GetBoolean() &&
        prior.GetProperty("external_nuget_sources_disabled").GetBoolean(),
    "RS050 must prove local-only package independence.");
Require(!prior.GetProperty("external_adoption_observed").GetBoolean() &&
        !prior.GetProperty("requests_transmitted").GetBoolean(),
    "Do not reinterpret the earlier qualified Math consumer as recipient adoption.");
Require(Str(prior,"dotnet_sdk") == "10.0.402" &&
        Str(prior,"observed_runtime_identifier") == "win-x64",
    "Pin only the actually observed SDK and runtime.");
Require(Str(prior,"consumer_exact_output") ==
        "dw.tools.math/c04-isolated-consumer/0.4 qualified; probes=31",
    "Independent consumer marker must match the durable T1 evidence.");

Require(matrix.GetProperty("schema_version").GetInt32() == 1 &&
        Str(matrix,"id") == "M3-W02-C04-RECIPIENT-BOUNDARIES" &&
        Str(matrix,"math_source_commit") == ExpectedCommit &&
        Str(matrix,"math_evidence") == Prior &&
        Str(matrix,"math_qualification") == ExpectedStatus,
    "Matrix must identify the immutable observed Math qualification.");
Require(matrix.GetProperty("qualified_local_consumer_oracles").GetInt32() == 31 &&
        Str(matrix,"math_sdk_observed") == "10.0.402" &&
        Str(matrix,"math_runtime_observed") == "win-x64",
    "Do not invent tests or platforms.");
Require(Str(matrix,"status") == "draft_for_owner_review" &&
        Str(matrix,"transmission") == "none" &&
        !matrix.GetProperty("external_adoption_observed").GetBoolean() &&
        !matrix.GetProperty("published_public_feed").GetBoolean(),
    "Matrix must keep recipient adoption, distribution and transmission false.");

var packages = matrix.GetProperty("local_artifacts").EnumerateArray().ToArray();
Require(packages.Length == 2, "Exactly two C04 package artifacts were verified.");
foreach (var id in new[] { "dw.quantities", "dw.tools.math.statistics" })
{
    var values = packages.Where(v => Str(v,"package_id") == id).ToArray();
    Require(values.Length == 1, "One observed package identity per local artifact: " + id);
    var artifact = values[0];
    var prefix = id == "dw.quantities" ? "package_quantities_" : "package_statistics_";
    var expectedId = Str(prior, prefix+"id");
    var expectedVersion = Str(prior, prefix+"version");
    var expectedSha = Str(prior, prefix+"sha256");
    Require(Str(artifact,"package_id") == expectedId &&
            Str(artifact,"version") == expectedVersion &&
            Str(artifact,"sha256") == expectedSha &&
            IsLowerSha256(expectedSha),
        "Artifact identity and SHA-256 must be copied from observed local RS050 evidence.");
    Require(Str(artifact,"mode") == "local-only" &&
            Str(artifact,"transfer") == "not_performed",
        "No public or recipient transfer has occurred.");
}

var decisionRequest = Read(DecisionRequest);
var mcdmRequest = Read(McdmRequest);
Require(decisionRequest.Contains("Status: draft. Recipient: Decision agent. Not sent.",StringComparison.Ordinal) &&
        decisionRequest.Contains("Transmission: none.",StringComparison.Ordinal),
    "Decision request must remain unsent.");
Require(mcdmRequest.Contains("Status: draft. Recipient: MCDM agent. Not sent.",StringComparison.Ordinal) &&
        mcdmRequest.Contains("Transmission: none.",StringComparison.Ordinal),
    "MCDM request must remain unsent.");

foreach (var artifact in packages)
{
    var identity = Str(artifact,"package_id");
    var digest = Str(artifact,"sha256");
    Require(decisionRequest.Contains(identity,StringComparison.Ordinal) &&
            decisionRequest.Contains(digest,StringComparison.Ordinal) &&
            mcdmRequest.Contains(identity,StringComparison.Ordinal) &&
            mcdmRequest.Contains(digest,StringComparison.Ordinal),
        "Both drafts must identify the actual observed packages and SHA-256s.");
}
Require(decisionRequest.Contains("VaR/CVaR",StringComparison.Ordinal) &&
        decisionRequest.Contains("EVPI",StringComparison.Ordinal) &&
        decisionRequest.Contains("MDP",StringComparison.Ordinal) &&
        decisionRequest.Contains("NOT cryptographically",StringComparison.Ordinal),
    "Decision's risk, solver and PRNG restrictions must remain explicit.");
Require(mcdmRequest.Contains("PROMETHEE",StringComparison.Ordinal) &&
        mcdmRequest.Contains("FlowQuantization",StringComparison.Ordinal) &&
        mcdmRequest.Contains("ties",StringComparison.Ordinal) &&
        mcdmRequest.Contains("ranking",StringComparison.Ordinal),
    "MCDM's tie, flow and preference policy must remain explicit.");

var rows = matrix.GetProperty("rows").EnumerateArray().ToArray();
Require(rows.Length == 8, "Exact accepted Math proposal/hold/exclusion matrix has eight cases.");
var distinct = new HashSet<string>(StringComparer.Ordinal);
var candidates = 0;
var holds = 0;
var retained = 0;
foreach (var row in rows)
{
    var id = Str(row,"id");
    var disposition = Str(row,"disposition");
    var owner = Str(row,"recipient");
    Require(distinct.Add(id), "Duplicate candidate identity: " + id);
    Require(owner is "Decision" or "MCDM" or "AURA", "Unknown recipient " + owner);
    Require(Str(row,"status") == "not_started" &&
            Str(row,"recipient_revision") == "unverified" &&
            Str(row,"recipient_tests") == "not_executed" &&
            Str(row,"owner_decision") == "pending" &&
            Str(row,"transmission") == "none",
        "Every recipient case must remain unverified, untransmitted, and not adopted.");
    Require(!string.IsNullOrWhiteSpace(Str(row,"policy_boundary")) &&
            !string.IsNullOrWhiteSpace(Str(row,"required_owner_tests")) &&
            !string.IsNullOrWhiteSpace(Str(row,"rollback")),
        "Each owner proposal needs explicit scientific boundaries, tests and rollback.");

    switch (disposition)
    {
        case "candidate":
            candidates++;
            Require(owner is "Decision" or "MCDM" &&
                    packages.Any(p => Str(p,"package_id") == Str(row,"math_package")),
                "Candidate must be supported by one observed local Math package.");
            break;
        case "hold":
            holds++;
            Require(Str(row,"math_package") == "none", "Held scope has no qualified C04 artifact.");
            break;
        case "retained_external":
            retained++;
            Require(Str(row,"math_package") == "none", "External business policy must not become a Math package.");
            break;
        default:
            throw new InvalidOperationException("Unrecognized candidate disposition: " + disposition);
    }
}
Require(candidates == 4 && holds == 1 && retained == 3,
    "Require four candidates, one held matrix proposal, and three protected external domains.");
var expectedRows = new[]
{
    "decision.exact-scale", "decision.quantile", "decision.randomness",
    "mcdm.compensated-sum", "mcdm.generic-matrix",
    "decision.policy-and-solvers", "mcdm.preferences-and-ranking",
    "aura-and-discovery-broker"
};
Require(expectedRows.All(distinct.Contains), "No accepted boundary may disappear.");

var decisionNote = Read(DecisionNote);
var handoff = Read(Handoff);
foreach (var keyword in new[] { "PROMETHEE", "FlowQuantization", "EVPI", "MDP",
    "VaR/CVaR", "dw.discovery", "G-M3" })
{
    Require(decisionNote.Contains(keyword,StringComparison.Ordinal),
        "Scientific decision must explicitly preserve " + keyword);
}
Require(handoff.Contains("not an adoption receipt", StringComparison.OrdinalIgnoreCase) &&
        handoff.Contains("31",StringComparison.Ordinal) &&
        handoff.Contains("4 candidate",StringComparison.Ordinal) &&
        handoff.Contains("transmitted",StringComparison.Ordinal),
    "Math handoff must distinguish package consumer proof from external adoption.");

Console.WriteLine(
    "dw.tools.math/c04-boundaries/0.4 qualified; rows=8; candidate=4; hold=1; external=3; transmitted=0");
