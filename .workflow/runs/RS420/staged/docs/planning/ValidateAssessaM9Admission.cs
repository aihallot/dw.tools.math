#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json.Nodes;

var backlog = JsonNode.Parse(File.ReadAllText("docs/planning/backlog.json"))!.AsObject();
var plan = JsonNode.Parse(File.ReadAllText(".aura/workflow/plan/project.json"))!.AsObject();
void Need(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
JsonObject Record(JsonArray source, string id) =>
    source.Select(x => x!.AsObject()).Single(x => x["id"]!.GetValue<string>() == id);
Need(backlog["plan_version"]!.GetValue<string>() == "0.1.187", "Unexpected backlog version.");
var releases = backlog["releases"]!.AsArray();
var release = Record(releases, "M9");
Need(release["status"]!.GetValue<string>() == "done" && release["evidence"]!.AsArray().Count == 1 &&
    (string?)release["evidence"]![0] == ".aura/workflow/reports/RS364/attempt-001.json", "M9 canonical milestone closure missing.");
Need(release["depends_on"]!.AsArray().Single()!.GetValue<string>() == "M3", "M9 must depend on completed M3.");
Need(Record(releases, "M3")["status"]!.GetValue<string>() == "done", "Do not rewrite M3 history.");
var wp = Record(backlog["work_packages"]!.AsArray(), "M9-W01");
Need(wp["status"]!.GetValue<string>() == "done" && wp["evidence"]!.AsArray().Count == 1 &&
    (string?)wp["evidence"]![0] == ".aura/workflow/reports/RS363/attempt-001.json", "M9 canonical work package closure missing.");
var chunk = Record(backlog["chunks"]!.AsArray(), "M9-W01-C01");
Need(chunk["status"]!.GetValue<string>() == "done" && chunk["evidence"]!.AsArray().Count == 1 &&
    (string?)chunk["evidence"]![0] == ".aura/workflow/reports/RS362/attempt-001.json", "M9 canonical phase closure missing.");
Need(chunk["depends_on"]!.AsArray().Single()!.GetValue<string>() == "M3-W02-C03",
    "Sampling must depend on qualified existing SplitMix64.");
Need(chunk["tasks"]!.AsArray().Count == 2, "Expected source and package qualification tasks.");
Need(chunk["requirements"]!.AsArray().Count == 2, "Expected bounded sampling and consumer requirements.");
Need(chunk["acceptance"]!.GetValue<string>().Contains("rejection", StringComparison.OrdinalIgnoreCase),
    "Unbiased rejection contract missing.");
Need(chunk["boundary_acceptance"]!.GetValue<string>().Contains("Assessa", StringComparison.Ordinal),
    "External ownership boundary missing.");
Need(Record(backlog["chunks"]!.AsArray(), "M3-W02-C03")["status"]!.GetValue<string>() == "done",
    "Historical PRNG chunk must remain done.");
var milestone = Record(plan["milestones"]!.AsArray(), "M9");
Need(milestone["state"]!.GetValue<string>() == "done", "M9 native gate must be closed.");
var nativeWp = Record(milestone["workPackages"]!.AsArray(), "M9-W01");
var phase = Record(nativeWp["phases"]!.AsArray(), "M9-W01-C01");
Need(nativeWp["state"]!.GetValue<string>() == "done" && phase["state"]!.GetValue<string>() == "done", "M9 work package and phase must be done.");
Need(phase["tasks"]!.AsArray().Count == 2, "Native plan must contain both tasks.");
foreach (var task in phase["tasks"]!.AsArray().Select(x => x!.AsObject()))
{
    Need(task["state"]!.GetValue<string>() == (task["id"]!.GetValue<string>() is "M9-W01-C01-T1" or "M9-W01-C01-T2" ? "done" : "not-ready"), "Unexpected task progress.");
    Need(task["subtasks"]!.AsArray().Count == 3, "Expected RED/GREEN/verify subtasks.");
    Need(task["subtasks"]!.AsArray().All(x => x!["state"]!.GetValue<string>() == (x["id"]!.GetValue<string>() switch { "M9-W01-C01-T1-R" or "M9-W01-C01-T1-G" or "M9-W01-C01-T1-V" => "done", "M9-W01-C01-T2-R" => "done", "M9-W01-C01-T2-G" => "done", "M9-W01-C01-T2-V" => "done", _ => "not-ready" })),
        "Subtasks must not claim progress.");
}
Need(Record(releases, "M8")["status"]!.GetValue<string>() == "done", "M8 canonical reconciliation absent.");
var m8 = Record(plan["milestones"]!.AsArray(), "M8");
Need(m8["state"]!.GetValue<string>() == "done", "Native M8 closure absent.");
var evidence = new (string Id, int Prior, string Validation, string Scope)[]
{
    ("RS267", 2, "assessa-m9-admission-contract", "M8"),
    ("RS268", 1, "m9-bounded-integer-exact-regressions", "M9"),
    ("RS269", 1, "m9-unpublished-statistics-candidate-exact-package", "M9"),
    ("RS270", 1, "m9-net11-locked-bounded-candidate-package-consumer", "M9"),
    ("RS271", 0, "m9-statistics-private-readiness-no-publish", "M9"),
    ("RS272", 1, "m9-private-publisher-disposable-rehearsal-and-net11-readback", "M9"),
    ("RS273", 0, "m9-physical-private-feed-independent-net11-readback", "M9"),
    ("RS274", 1, "m9-assessa-private-package-handoff-durable-identities", "M9"),
    ("RS275", 0, "m9-reconcile-delivered-assessa-request", "M9"),
    ("RS277", 0, "m9-published-bounded-statistics-linux-net11-independent-binary", "M9"),
    ("RS278", 0, "m9-assessa-handoff-rs277-linux-locked-binary-evidence", "M9")
};
foreach (var item in evidence)
{
    var r = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/" + item.Id + "/attempt-001.json"))!.AsObject();
    Need((string?)r["runId"] == item.Id && (string?)r["outcome"] == "succeeded" &&
        (int?)r["attempt"] == 1 && (string?)r["executor"]?["toolVersion"] == "0.1.66" &&
        (int?)r["priorInvocationFailures"]?["count"] == item.Prior &&
        (bool?)r["priorInvocationFailures"]?["firstPassSucceeded"] == (item.Prior == 0) &&
        r["boundaryViolations"]!.AsArray().Count == 0 &&
        (bool?)r["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
        r["projectNodes"]!.AsArray().Count == 1 && (string?)r["projectNodes"]![0] == item.Scope &&
        r["progress"]!["completions"]!.AsArray().Count == 0 &&
        r["contribution"]!["observed"]!["validations"]!.AsArray().Any(v => (string?)v?["id"] == item.Validation),
        "Historical evidence or scope differs: " + item.Id);
}
var redReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS350/attempt-001.json"))!.AsObject();
var redProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS350/preparation-proof.json"))!.AsObject();
var validationIds = redReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
Need((string?)redReport["runId"] == "RS350" && (string?)redReport["outcome"] == "succeeded" &&
    (int?)redReport["attempt"] == 1 && (string?)redReport["executor"]?["toolVersion"] == "0.1.76" &&
    (int?)redReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)redReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    redReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)redReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    redReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    redReport["progress"]!["transitions"]!.AsArray().Count == 5 &&
    redReport["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T1-R" && (string?)x?["afterState"] == "in-progress") &&
    validationIds.Length == 8 && validationIds.Contains("m9-rs350-source-oracle-and-three-compiled-mutants", StringComparer.Ordinal) &&
    (string?)redProof["runId"] == "RS350" && (bool?)redProof["payloadCompiled"] == true &&
    (int?)redProof["rehearsedValidationCount"] == 8 &&
    redProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(validationIds, StringComparer.Ordinal),
    "RS350 durable RED qualification evidence differs.");
Need(!redReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "RED modified production source.");
var closure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS351/attempt-001.json"))!.AsObject();
Need((string?)closure["runId"] == "RS351" && (string?)closure["outcome"] == "succeeded" &&
    (int?)closure["attempt"] == 1 && (string?)closure["executor"]?["toolVersion"] == "0.1.76" &&
    (int?)closure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)closure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    closure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)closure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    closure["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)closure["progress"]!["completions"]![0]!["id"] == "M9-W01-C01-T1-R" &&
    (string?)closure["progress"]!["completions"]![0]!["afterState"] == "done" &&
    closure["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T1-G" && (string?)x?["afterState"] == "ready"),
    "RS351 durable RED closure or GREEN readiness differs.");
var productTask = Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T1");
var productRed = Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-R");
var productGreen = Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-G");
Need((string?)productRed["status"] == "done" &&
    productRed["evidence"]!.AsArray().Count == 1 &&
    (string?)productRed["evidence"]![0] == ".aura/workflow/reports/RS351/attempt-001.json" &&
    (string?)productGreen["status"] == "done" && productGreen["evidence"]!.AsArray().Count == 1 &&
    (string?)productGreen["evidence"]![0] == ".aura/workflow/reports/RS353/attempt-001.json" &&
    (string?)Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-V")["status"] == "done" &&
    (string?)Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-V")["evidence"]![0] == ".aura/workflow/reports/RS355/attempt-001.json" &&
    (string?)productTask["status"] == "done" &&
    (string?)productTask["evidence"]![0] == ".aura/workflow/reports/RS355/attempt-001.json" &&
    (string?)Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["status"] == "done" &&
    (string?)Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["evidence"]![0] == ".aura/workflow/reports/RS361/attempt-001.json" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-R")["status"] == "done" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-R")["evidence"]![0] == ".aura/workflow/reports/RS357/attempt-001.json" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-G")["status"] == "done" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-G")["evidence"]![0] == ".aura/workflow/reports/RS359/attempt-001.json" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-V")["status"] == "done" &&
    (string?)Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-V")["evidence"]![0] == ".aura/workflow/reports/RS361/attempt-001.json", "Canonical durable package task closure projection differs.");
var greenReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS352/attempt-001.json"))!.AsObject();
var greenProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS352/preparation-proof.json"))!.AsObject();
var greenIds = greenReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedGreenIds = [
    "m9-rs352-authentic-source-independent-oracle", "m9-rs352-statistics-locked-regression",
    "m9-rs352-red-closure-evidence", "m9-rs352-native-green-active", "m9-rs352-published-handoff",
    "m9-rs352-request-reconciliation", "m9-rs352-m8-evidence-frontier", "m9-rs352-planning-render-check", "diff-check"
];
var greenTransitions = greenReport["progress"]!["transitions"]!.AsArray();
Need((string?)greenReport["runId"] == "RS352" && (string?)greenReport["outcome"] == "succeeded" &&
    (int?)greenReport["attempt"] == 1 && (string?)greenReport["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)greenReport["git"]?["commitMessage"] == "test: qualify existing M9 bounded sampling GREEN" &&
    (int?)greenReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)greenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    greenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)greenReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    greenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    greenTransitions.Count == 1 && (string?)greenTransitions[0]?["id"] == "M9-W01-C01-T1-G" &&
    (string?)greenTransitions[0]?["beforeState"] == "ready" && (string?)greenTransitions[0]?["afterState"] == "in-progress" &&
    greenIds.SequenceEqual(expectedGreenIds, StringComparer.Ordinal) &&
    (string?)greenProof["runId"] == "RS352" && (bool?)greenProof["payloadCompiled"] == true &&
    (int?)greenProof["rehearsedValidationCount"] == 9 &&
    greenProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(greenIds, StringComparer.Ordinal),
    "RS352 durable GREEN qualification or compiled preparation proof differs.");
Need(!greenReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "GREEN modified production source.");
var verificationReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS354/attempt-001.json"))!.AsObject();
var verificationProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS354/preparation-proof.json"))!.AsObject();
var verificationIds = verificationReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedVerificationIds = [
    "m9-rs354-independent-source-report-and-blob-audit", "m9-rs354-authentic-source-oracle-replay",
    "m9-rs354-targeted-bounded-sampling-replay", "m9-rs354-historical-admission",
    "m9-rs354-native-verification-active", "m9-rs354-published-handoff",
    "m9-rs354-request-reconciliation", "m9-rs354-m8-evidence-frontier",
    "m9-rs354-planning-render-check", "diff-check"
];
var verificationTransitions = verificationReport["progress"]!["transitions"]!.AsArray();
Need((string?)verificationReport["runId"] == "RS354" && (string?)verificationReport["outcome"] == "succeeded" &&
    (int?)verificationReport["attempt"] == 1 && (string?)verificationReport["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)verificationReport["git"]?["commitMessage"] == "test: independently verify M9 bounded sampling source and replay" &&
    (int?)verificationReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)verificationReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    verificationReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)verificationReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    verificationReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    verificationTransitions.Count == 1 && (string?)verificationTransitions[0]?["id"] == "M9-W01-C01-T1-V" &&
    (string?)verificationTransitions[0]?["beforeState"] == "ready" && (string?)verificationTransitions[0]?["afterState"] == "in-progress" &&
    verificationIds.SequenceEqual(expectedVerificationIds, StringComparer.Ordinal) &&
    (string?)verificationProof["runId"] == "RS354" &&
    (string?)verificationProof["sourceCommit"] == "51ce6b3984d926aefd137942367ea8d318754cd4" &&
    (bool?)verificationProof["payloadCompiled"] == true && (int?)verificationProof["rehearsedValidationCount"] == 10 &&
    verificationProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(verificationIds, StringComparer.Ordinal),
    "RS354 durable Verification or compiled preparation proof differs.");
Need(!verificationReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "Verification modified production source.");
var sourceClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS355/attempt-001.json"))!.AsObject();
var sourceClosureProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS355/preparation-proof.json"))!.AsObject();
var sourceClosureIds = sourceClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
var completedSourceNodes = sourceClosure["progress"]!["completions"]!.AsArray().Select(x => (string?)x?["id"]).OrderBy(x => x, StringComparer.Ordinal);
Need((string?)sourceClosure["runId"] == "RS355" && (string?)sourceClosure["outcome"] == "succeeded" &&
    (int?)sourceClosure["attempt"] == 1 && (string?)sourceClosure["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)sourceClosure["git"]?["commitMessage"] == "chore: close M9 sampling source and ready package RED" &&
    (int?)sourceClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)sourceClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    sourceClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)sourceClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    completedSourceNodes.SequenceEqual(new[] { "M9-W01-C01-T1", "M9-W01-C01-T1-V" }, StringComparer.Ordinal) &&
    sourceClosure["progress"]!["transitions"]!.AsArray().Count == 4 &&
    sourceClosure["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T2" && (string?)x?["afterState"] == "ready") &&
    sourceClosure["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T2-R" && (string?)x?["afterState"] == "ready") &&
    sourceClosureIds.Length == 8 && sourceClosureIds.Contains("m9-rs355-verification-closure-evidence", StringComparer.Ordinal) &&
    (string?)sourceClosureProof["runId"] == "RS355" && (bool?)sourceClosureProof["payloadCompiled"] == true &&
    (int?)sourceClosureProof["rehearsedValidationCount"] == 8 &&
    sourceClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(sourceClosureIds, StringComparer.Ordinal),
    "RS355 durable source closure or compiled proof differs.");
var packageRedReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS356/attempt-001.json"))!.AsObject();
var packageRedProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS356/preparation-proof.json"))!.AsObject();
var packageRedIds = packageRedReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedPackageRedIds = [
    "m9-rs356-actual-exact-locked-package-refusals", "m9-rs356-source-closure-evidence",
    "m9-rs356-native-package-red-active", "m9-rs356-published-handoff",
    "m9-rs356-request-reconciliation", "m9-rs356-m8-evidence-frontier",
    "m9-rs356-planning-render-check", "diff-check"
];
var packageRedTransitions = packageRedReport["progress"]!["transitions"]!.AsArray();
Need((string?)packageRedReport["runId"] == "RS356" && (string?)packageRedReport["outcome"] == "succeeded" &&
    (int?)packageRedReport["attempt"] == 1 && (string?)packageRedReport["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)packageRedReport["git"]?["commitMessage"] == "test: activate M9 package RED and falsify locked consumer boundary" &&
    (int?)packageRedReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)packageRedReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    packageRedReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)packageRedReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    packageRedReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    packageRedTransitions.Count == 2 &&
    packageRedTransitions.Select(x => (string?)x?["id"]).OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "M9-W01-C01-T2", "M9-W01-C01-T2-R" }, StringComparer.Ordinal) &&
    packageRedTransitions.All(x => (string?)x?["beforeState"] == "ready" && (string?)x?["afterState"] == "in-progress") &&
    packageRedIds.SequenceEqual(expectedPackageRedIds, StringComparer.Ordinal) &&
    (string?)packageRedProof["runId"] == "RS356" &&
    (string?)packageRedProof["sourceCommit"] == "78b7500554384d4148c305d6d4a0a1292284f342" &&
    (bool?)packageRedProof["payloadCompiled"] == true && (int?)packageRedProof["rehearsedValidationCount"] == 8 &&
    packageRedProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(packageRedIds, StringComparer.Ordinal),
    "RS356 durable exact package RED or compiled preparation proof differs.");
Need(!packageRedReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "Package RED modified production source.");
var packageRedClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS357/attempt-001.json"))!.AsObject();
var packageRedClosureProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS357/preparation-proof.json"))!.AsObject();
var packageRedClosureIds = packageRedClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
Need((string?)packageRedClosure["runId"] == "RS357" && (string?)packageRedClosure["outcome"] == "succeeded" &&
    (int?)packageRedClosure["attempt"] == 1 && (string?)packageRedClosure["executor"]?["toolVersion"] == "0.1.76" &&
    (int?)packageRedClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)packageRedClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    packageRedClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)packageRedClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    packageRedClosure["progress"]!["completions"]!.AsArray().Count == 1 &&
    (string?)packageRedClosure["progress"]!["completions"]![0]!["id"] == "M9-W01-C01-T2-R" &&
    (string?)packageRedClosure["progress"]!["completions"]![0]!["afterState"] == "done" &&
    packageRedClosure["progress"]!["transitions"]!.AsArray().Any(x => (string?)x?["id"] == "M9-W01-C01-T2-G" && (string?)x?["afterState"] == "ready") &&
    packageRedClosureIds.Length == 7 && packageRedClosureIds.Contains("m9-rs357-package-red-closure-evidence", StringComparer.Ordinal) &&
    (string?)packageRedClosureProof["runId"] == "RS357" && (bool?)packageRedClosureProof["payloadCompiled"] == true &&
    (int?)packageRedClosureProof["rehearsedValidationCount"] == 7 &&
    packageRedClosureProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(packageRedClosureIds, StringComparer.Ordinal),
    "RS357 durable package RED closure differs.");
foreach (var item in new[]
{
    (Run: "RS269", Prior: 1, Commit: "f93f25cfb74cb14c91e25e60c64ef82776ca5c47", Id: "m9-unpublished-statistics-candidate-exact-package"),
    (Run: "RS270", Prior: 1, Commit: "f279a75b03320f82ae4d6e72e585206982c0dfcb", Id: "m9-net11-locked-bounded-candidate-package-consumer"),
    (Run: "RS273", Prior: 0, Commit: "6de70eddfc9bdf1d862c8de853fa978018d7d3c2", Id: "m9-physical-private-feed-independent-net11-readback"),
    (Run: "RS277", Prior: 0, Commit: "95e24db233795429156575f7236d006525148db4", Id: "m9-published-bounded-statistics-linux-net11-independent-binary")
})
{
    var report = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/" + item.Run + "/attempt-001.json"))!.AsObject();
    var proof = JsonNode.Parse(File.ReadAllText(".workflow/runs/" + item.Run + "/preparation-proof.json"))!.AsObject();
    var ids = report["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
    Need((string?)report["runId"] == item.Run && (string?)report["outcome"] == "succeeded" &&
        (int?)report["attempt"] == 1 && (int?)report["priorInvocationFailures"]?["preparationFailureCount"] == item.Prior &&
        (int?)report["priorInvocationFailures"]?["allocatedAttemptFailureCount"] == 0 &&
        ids.SequenceEqual(new[] { item.Id, "diff-check" }, StringComparer.Ordinal) &&
        (string?)proof["runId"] == item.Run && (string?)proof["sourceCommit"] == item.Commit &&
        (bool?)proof["payloadCompiled"] == true && (int?)proof["rehearsedValidationCount"] == 2 &&
        proof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(ids, StringComparer.Ordinal),
        "Historical candidate or consumer compiled proof differs: " + item.Run);
}
var packageGreenReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS358/attempt-001.json"))!.AsObject();
var packageGreenProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS358/preparation-proof.json"))!.AsObject();
var packageGreenIds = packageGreenReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedPackageGreenIds = [
    "m9-rs358-physical-published-windows-net11-readback", "m9-rs358-candidate-and-consumer-proof-reconciliation",
    "m9-rs358-native-package-green-active", "m9-rs358-published-handoff",
    "m9-rs358-request-reconciliation", "m9-rs358-m8-evidence-frontier",
    "m9-rs358-planning-render-check", "diff-check"
];
var packageGreenTransitions = packageGreenReport["progress"]!["transitions"]!.AsArray();
Need((string?)packageGreenReport["runId"] == "RS358" && (string?)packageGreenReport["outcome"] == "succeeded" &&
    (int?)packageGreenReport["attempt"] == 1 && (string?)packageGreenReport["executor"]?["toolVersion"] == "0.1.76" &&
    (string?)packageGreenReport["git"]?["commitMessage"] == "test: qualify existing M9 immutable package GREEN" &&
    (int?)packageGreenReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)packageGreenReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    packageGreenReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)packageGreenReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    packageGreenReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    packageGreenTransitions.Count == 1 && (string?)packageGreenTransitions[0]?["id"] == "M9-W01-C01-T2-G" &&
    (string?)packageGreenTransitions[0]?["beforeState"] == "ready" && (string?)packageGreenTransitions[0]?["afterState"] == "in-progress" &&
    packageGreenIds.SequenceEqual(expectedPackageGreenIds, StringComparer.Ordinal) &&
    (string?)packageGreenProof["runId"] == "RS358" &&
    (string?)packageGreenProof["sourceCommit"] == "1b71d972dc468f3206ba485b5a4cb7ec20d3e3ea" &&
    (bool?)packageGreenProof["payloadCompiled"] == true && (int?)packageGreenProof["rehearsedValidationCount"] == 8 &&
    packageGreenProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(packageGreenIds, StringComparer.Ordinal),
    "RS358 durable immutable package GREEN or compiled preparation proof differs.");
Need(!packageGreenReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "Package GREEN modified production source.");
var packageVerificationReport = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS360/attempt-001.json"))!.AsObject();
var packageVerificationProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS360/preparation-proof.json"))!.AsObject();
var packageVerificationIds = packageVerificationReport["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedPackageVerificationIds = [
    "m9-rs360-independent-package-evidence-and-three-refusals", "m9-rs360-physical-published-linux-net11-replay",
    "m9-rs360-historical-admission", "m9-rs360-native-package-verification-active",
    "m9-rs360-published-handoff", "m9-rs360-request-reconciliation", "m9-rs360-m8-evidence-frontier",
    "m9-rs360-planning-render-check", "diff-check"
];
var packageVerificationTransitions = packageVerificationReport["progress"]!["transitions"]!.AsArray();
Need((string?)packageVerificationReport["runId"] == "RS360" && (string?)packageVerificationReport["outcome"] == "succeeded" &&
    (int?)packageVerificationReport["attempt"] == 1 && (string?)packageVerificationReport["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)packageVerificationReport["git"]?["commitMessage"] == "test: independently verify M9 immutable package and Linux replay" &&
    (int?)packageVerificationReport["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)packageVerificationReport["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    packageVerificationReport["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)packageVerificationReport["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    packageVerificationReport["progress"]!["completions"]!.AsArray().Count == 0 &&
    packageVerificationTransitions.Count == 1 && (string?)packageVerificationTransitions[0]?["id"] == "M9-W01-C01-T2-V" &&
    (string?)packageVerificationTransitions[0]?["beforeState"] == "ready" && (string?)packageVerificationTransitions[0]?["afterState"] == "in-progress" &&
    packageVerificationIds.SequenceEqual(expectedPackageVerificationIds, StringComparer.Ordinal) &&
    (string?)packageVerificationProof["runId"] == "RS360" &&
    (string?)packageVerificationProof["sourceCommit"] == "a63cd8dd863c99d4cbb6784ce22261c290e6f484" &&
    (bool?)packageVerificationProof["payloadCompiled"] == true && (int?)packageVerificationProof["rehearsedValidationCount"] == 9 &&
    packageVerificationProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(packageVerificationIds, StringComparer.Ordinal) &&
    packageVerificationReport["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(packageVerificationIds, StringComparer.Ordinal),
    "RS360 durable package Verification or compiled preparation proof differs.");
Need(!packageVerificationReport["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "Package Verification modified production source.");
var packageTaskClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS361/attempt-001.json"))!.AsObject();
var packageTaskProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS361/preparation-proof.json"))!.AsObject();
var packageTaskIds = packageTaskClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedPackageTaskIds = [
    "m9-rs361-independent-package-evidence-and-three-refusals", "m9-rs361-package-verification-closure-evidence",
    "m9-rs361-native-package-task-done", "m9-rs361-published-handoff",
    "m9-rs361-request-reconciliation", "m9-rs361-m8-evidence-frontier",
    "m9-rs361-planning-render-check", "diff-check"
];
var packageTaskMoves = packageTaskClosure["progress"]!["transitions"]!.AsArray();
var packageTaskDone = packageTaskClosure["progress"]!["completions"]!.AsArray();
Need((string?)packageTaskClosure["runId"] == "RS361" && (string?)packageTaskClosure["outcome"] == "succeeded" &&
    (int?)packageTaskClosure["attempt"] == 1 && (string?)packageTaskClosure["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)packageTaskClosure["git"]?["commitMessage"] == "chore: close M9 package Verification and task" &&
    (int?)packageTaskClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)packageTaskClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    packageTaskClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)packageTaskClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    packageTaskMoves.Count == 2 && packageTaskDone.Count == 2 &&
    packageTaskDone.Select(x => (string?)x?["id"]).OrderBy(x => x, StringComparer.Ordinal)
        .SequenceEqual(new[] { "M9-W01-C01-T2", "M9-W01-C01-T2-V" }, StringComparer.Ordinal) &&
    packageTaskMoves.All(x => (string?)x?["beforeState"] == "in-progress" && (string?)x?["afterState"] == "done") &&
    packageTaskIds.SequenceEqual(expectedPackageTaskIds, StringComparer.Ordinal) &&
    (string?)packageTaskProof["runId"] == "RS361" &&
    (string?)packageTaskProof["sourceCommit"] == "1b89d8bd975a5f63c27099b4a1a16a14b0a679b0" &&
    (bool?)packageTaskProof["payloadCompiled"] == true && (int?)packageTaskProof["rehearsedValidationCount"] == 8 &&
    packageTaskProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(packageTaskIds, StringComparer.Ordinal) &&
    packageTaskClosure["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(packageTaskIds, StringComparer.Ordinal),
    "RS361 durable package task closure differs.");
Need(!packageTaskClosure["contribution"]!["observed"]!["changedPaths"]!.AsArray().Any(x =>
    ((string?)x)?.StartsWith("src/", StringComparison.Ordinal) == true), "Package task closure modified production source.");
Need((string?)release["gate"] == "G-M9" &&
    release["required_chunks"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] { "M9-W01-C01" }, StringComparer.Ordinal) &&
    chunk["requirements"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] { "M9-W01-C01-R1", "M9-W01-C01-R2" }, StringComparer.Ordinal) &&
    chunk["external_dependencies"]!.AsArray().Count == 0 &&
    chunk["tasks"]!.AsArray().All(x => (string?)x?["status"] == "done" &&
        x!["subtasks"]!.AsArray().All(y => (string?)y?["status"] == "done")),
    "M9 gate or requirement coverage differs.");
var requirementRecords = backlog["requirements"]!.AsArray();
foreach (var id in new[] { "M9-W01-C01-R1", "M9-W01-C01-R2" })
{
    var requirement = Record(requirementRecords, id);
    Need((string?)requirement["release"] == "M9" && (string?)requirement["owner"] == "M9-W01-C01" &&
        !string.IsNullOrWhiteSpace((string?)requirement["acceptance"]), "Requirement authority differs: " + id);
}
var phaseClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS362/attempt-001.json"))!.AsObject();
var phaseProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS362/preparation-proof.json"))!.AsObject();
var phaseIds = phaseClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedPhaseIds = [
    "m9-rs362-phase-requirement-acceptance", "m9-rs362-native-phase-done", "m9-rs362-published-handoff",
    "m9-rs362-request-reconciliation", "m9-rs362-m8-evidence-frontier", "m9-rs362-planning-render-check", "diff-check"
];
var phaseMoves = phaseClosure["progress"]!["transitions"]!.AsArray();
var phaseDone = phaseClosure["progress"]!["completions"]!.AsArray();
Need((string?)phaseClosure["runId"] == "RS362" && (string?)phaseClosure["outcome"] == "succeeded" &&
    (int?)phaseClosure["attempt"] == 1 && (string?)phaseClosure["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)phaseClosure["git"]?["commitMessage"] == "chore: accept and close M9 bounded sampling phase" &&
    (int?)phaseClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)phaseClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    phaseClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)phaseClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    phaseMoves.Count == 1 && phaseDone.Count == 1 &&
    (string?)phaseDone[0]?["id"] == "M9-W01-C01" &&
    (string?)phaseMoves[0]?["id"] == "M9-W01-C01" &&
    (string?)phaseMoves[0]?["beforeState"] == "in-progress" && (string?)phaseMoves[0]?["afterState"] == "done" &&
    phaseIds.SequenceEqual(expectedPhaseIds, StringComparer.Ordinal) &&
    (string?)phaseProof["runId"] == "RS362" && (string?)phaseProof["sourceCommit"] == "817b8e6d39fc90dd469f6a9145234e34f8a433fb" &&
    (bool?)phaseProof["payloadCompiled"] == true && (int?)phaseProof["rehearsedValidationCount"] == 7 &&
    phaseProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(phaseIds, StringComparer.Ordinal) &&
    phaseClosure["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(phaseIds, StringComparer.Ordinal),
    "RS362 durable phase acceptance differs.");
Need(backlog["chunks"]!.AsArray().Where(x => (string?)x?["work_package"] == "M9-W01")
    .Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M9-W01-C01" }, StringComparer.Ordinal) &&
    nativeWp["phases"]!.AsArray().Count == 1, "Unexpected work package scope.");
var workClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS363/attempt-001.json"))!.AsObject();
var workProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS363/preparation-proof.json"))!.AsObject();
var workIds = workClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedWorkIds = [
    "m9-rs363-work-package-closure-evidence", "m9-rs363-native-work-package-done", "m9-rs363-published-handoff",
    "m9-rs363-request-reconciliation", "m9-rs363-m8-evidence-frontier", "m9-rs363-planning-render-check", "diff-check"
];
var workMoves = workClosure["progress"]!["transitions"]!.AsArray();
var workDone = workClosure["progress"]!["completions"]!.AsArray();
Need((string?)workClosure["runId"] == "RS363" && (string?)workClosure["outcome"] == "succeeded" &&
    (int?)workClosure["attempt"] == 1 && (string?)workClosure["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)workClosure["git"]?["commitMessage"] == "chore: close M9 bounded sampling work package" &&
    (int?)workClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)workClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    workClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)workClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    workMoves.Count == 1 && workDone.Count == 1 &&
    (string?)workDone[0]?["id"] == "M9-W01" && (string?)workMoves[0]?["id"] == "M9-W01" &&
    (string?)workMoves[0]?["beforeState"] == "in-progress" && (string?)workMoves[0]?["afterState"] == "done" &&
    workIds.SequenceEqual(expectedWorkIds, StringComparer.Ordinal) &&
    (string?)workProof["runId"] == "RS363" && (string?)workProof["sourceCommit"] == "fd8e32a9591dff88d8b1cd432c7b308e0d0737cd" &&
    (bool?)workProof["payloadCompiled"] == true && (int?)workProof["rehearsedValidationCount"] == 7 &&
    workProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(workIds, StringComparer.Ordinal) &&
    workClosure["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(workIds, StringComparer.Ordinal),
    "RS363 durable work package closure differs.");
Need(backlog["work_packages"]!.AsArray().Where(x => (string?)x?["release"] == "M9")
    .Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M9-W01" }, StringComparer.Ordinal) &&
    backlog["chunks"]!.AsArray().Where(x => (string?)x?["release"] == "M9")
    .Select(x => (string?)x?["id"]).SequenceEqual(new[] { "M9-W01-C01" }, StringComparer.Ordinal) &&
    milestone["workPackages"]!.AsArray().Count == 1 &&
    milestone["dependsOn"]!.AsArray().Select(x => (string?)x).SequenceEqual(new[] { "M3" }, StringComparer.Ordinal) &&
    (string?)Record(plan["milestones"]!.AsArray(), "M3")["state"] == "done", "G-M9 scope or prerequisite differs.");
var milestoneClosure = JsonNode.Parse(File.ReadAllText(".aura/workflow/reports/RS364/attempt-001.json"))!.AsObject();
var milestoneProof = JsonNode.Parse(File.ReadAllText(".workflow/runs/RS364/preparation-proof.json"))!.AsObject();
var milestoneIds = milestoneClosure["contribution"]!["observed"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).ToArray();
string[] expectedMilestoneIds = [
    "m9-rs364-milestone-closure-evidence", "m9-rs364-native-milestone-done", "m9-rs364-published-handoff",
    "m9-rs364-request-reconciliation", "m9-rs364-m8-evidence-frontier", "m9-rs364-planning-render-check", "diff-check"
];
var milestoneMoves = milestoneClosure["progress"]!["transitions"]!.AsArray();
var milestoneDone = milestoneClosure["progress"]!["completions"]!.AsArray();
Need((string?)milestoneClosure["runId"] == "RS364" && (string?)milestoneClosure["outcome"] == "succeeded" &&
    (int?)milestoneClosure["attempt"] == 1 && (string?)milestoneClosure["executor"]?["toolVersion"] == "0.1.78" &&
    (string?)milestoneClosure["git"]?["commitMessage"] == "chore: close M9 bounded sampling milestone" &&
    (int?)milestoneClosure["priorInvocationFailures"]?["count"] == 0 &&
    (bool?)milestoneClosure["priorInvocationFailures"]?["firstPassSucceeded"] == true &&
    milestoneClosure["boundaryViolations"]!.AsArray().Count == 0 &&
    (bool?)milestoneClosure["contribution"]?["observed"]?["mutationBoundaryClean"] == true &&
    milestoneMoves.Count == 1 && milestoneDone.Count == 1 &&
    (string?)milestoneDone[0]?["id"] == "M9" && (string?)milestoneMoves[0]?["id"] == "M9" &&
    (string?)milestoneMoves[0]?["beforeState"] == "in-progress" && (string?)milestoneMoves[0]?["afterState"] == "done" &&
    milestoneIds.SequenceEqual(expectedMilestoneIds, StringComparer.Ordinal) &&
    (string?)milestoneProof["runId"] == "RS364" && (string?)milestoneProof["sourceCommit"] == "b9e2cf4a74f2e4cee085367073ca14bdc0517413" &&
    (bool?)milestoneProof["payloadCompiled"] == true && (int?)milestoneProof["rehearsedValidationCount"] == 7 &&
    milestoneProof["preparationValidationIds"]!.AsArray().Select(x => (string?)x).SequenceEqual(milestoneIds, StringComparer.Ordinal) &&
    milestoneClosure["preparationValidationEvidence"]!["validations"]!.AsArray().Select(x => (string?)x?["id"]).SequenceEqual(milestoneIds, StringComparer.Ordinal),
    "RS364 durable milestone closure differs.");
var completionMap = new (JsonObject Node, string Run)[]
{
    (release, "RS364"), (wp, "RS363"), (chunk, "RS362"),
    (productTask, "RS355"), (Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-R"), "RS351"),
    (Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-G"), "RS353"),
    (Record(productTask["subtasks"]!.AsArray(), "M9-W01-C01-T1-V"), "RS355"),
    (Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2"), "RS361"),
    (Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-R"), "RS357"),
    (Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-G"), "RS359"),
    (Record(Record(chunk["tasks"]!.AsArray(), "M9-W01-C01-T2")["subtasks"]!.AsArray(), "M9-W01-C01-T2-V"), "RS361")
};
foreach (var (node, run) in completionMap)
{
    var path = ".aura/workflow/reports/" + run + "/attempt-001.json";
    var report = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
    var id = (string?)node["id"];
    Need((string?)node["status"] == "done" && node["evidence"]!.AsArray().Count == 1 && (string?)node["evidence"]![0] == path &&
        (string?)report["runId"] == run && (string?)report["outcome"] == "succeeded" &&
        report["boundaryViolations"]!.AsArray().Count == 0 &&
        report["progress"]!["completions"]!.AsArray().Count(x => (string?)x?["id"] == id &&
            (string?)x?["afterState"] == "done" && (bool?)x?["completed"] == true &&
            !string.IsNullOrWhiteSpace((string?)x?["achievement"])) == 1,
        "Canonical completion evidence differs: " + id);
}
Console.WriteLine("MATH-M9-RS365-CANONICAL-CLOSURE|RS364=first-pass|nodes=11|R1=qualified|R2=qualified|gate=G-M9|nativeM9=done|canonicalM9=done|assessaAdoption=pending|PASS");
