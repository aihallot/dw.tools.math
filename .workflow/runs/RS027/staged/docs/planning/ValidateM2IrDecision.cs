#:property PublishAot=false
#:property NuGetAudit=false

using System.Text.Json.Nodes;

try
{
    Require(args.Length == 1 && args[0] == "--check",
        "M2 IR subset validator is read-only; supply --check.");
    var root = Directory.GetCurrentDirectory();
    JsonObject Load(string relative) => JsonNode.Parse(
        File.ReadAllText(Path.Combine(root, relative)))?.AsObject()
        ?? throw new InvalidOperationException("Invalid or missing JSON: " + relative);
    string S(JsonObject node, string property) =>
        node[property]?.GetValue<string>()
        ?? throw new InvalidOperationException("Missing string: " + property);
    JsonArray A(JsonObject node, string property) =>
        node[property]?.AsArray()
        ?? throw new InvalidOperationException("Missing array: " + property);

    var product = Load("docs/planning/backlog.json");
    var native = Load(".aura/workflow/plan/project.json");
    var matrix = Load("docs/planning/evidence/M2-W01-C01-mapping.json");
    var qualified = Load("docs/planning/evidence/M2-W01-C01-qualified.json");
    var priorGate = Load("docs/planning/evidence/M1-W03-C02-gate.json");
    var adoption = Load("docs/coordination/requests/MATH-XR-002-aura-adoption.json");
    var decisionPath = "docs/planning/decisions/m2-w01-c01.md";
    var decision = File.ReadAllText(Path.Combine(root, decisionPath));

    Require(S(product, "plan_version") == "0.1.26",
        "Expected M2 IR decision plan version 0.1.26.");
    var releases = A(product, "releases").Select(x => x!.AsObject()).ToArray();
    Require(S(releases.Single(x => S(x,"id") == "M1"),"status") == "done" &&
            S(releases.Single(x => S(x,"id") == "M2"),"status") == "in_progress",
        "M1 must stay qualified and M2 is only in progress.");
    var wp = A(product,"work_packages").Select(x=>x!.AsObject())
        .Single(x=>S(x,"id")=="M2-W01");
    Require(S(wp,"status") == "in_progress", "M2-W01 mathematical model must remain in progress.");
    var phases = A(product,"chunks").Select(x=>x!.AsObject()).ToArray();
    var c = phases.Single(x=>S(x,"id")=="M2-W01-C01");
    var c2 = phases.Single(x=>S(x,"id")=="M2-W01-C02");
    Require(S(c,"status")=="done" && S(c2,"status")=="planned",
        "An ADR must not pretend to have delivered the IR implementation.");
    var tasks = A(c,"tasks").Select(x=>x!.AsObject()).ToArray();
    Require(tasks.Length==2 && tasks.All(x=>S(x,"status")=="done" &&
        A(x,"subtasks").Count==3 &&
        A(x,"subtasks").All(s=>S(s!.AsObject(),"status")=="done")),
        "Both study/ADR tasks and their six question-observation-decision stages must be completed.");
    var proof = A(c,"evidence").Select(x=>x!.GetValue<string>()).ToArray();
    Require(proof.Contains("docs/planning/evidence/M2-W01-C01-mapping.json",
        StringComparer.Ordinal) &&
        proof.Contains("docs/planning/evidence/M2-W01-C01-qualified.json",
        StringComparer.Ordinal), "The decision chunk has insufficient independent proof.");
    var nativeMilestone=A(native,"milestones").Select(x=>x!.AsObject())
        .Single(x=>S(x,"id")=="M2");
    var nativeWp=A(nativeMilestone,"workPackages").Select(x=>x!.AsObject())
        .Single(x=>S(x,"id")=="M2-W01");
    var nativePhase=A(nativeWp,"phases").Select(x=>x!.AsObject())
        .Single(x=>S(x,"id")=="M2-W01-C01");
    Require(S(nativeMilestone,"state")=="in-progress" &&
        S(nativeWp,"state")=="in-progress" &&
        S(nativePhase,"state")=="done" &&
        A(nativePhase,"tasks").All(x=>S(x!.AsObject(),"state")=="done"),
        "Native plan incorrectly projects the M2 architecture decision.");
    Require(S(matrix,"status")=="proposed-not-implemented" &&
        S(matrix,"selected_model")=="immutable-typed-provider-neutral-ast" &&
        S(matrix,"selected_exact_primitive")=="dw.quantities.ExactRational" &&
        matrix["new_bigrational_implementation"]?.GetValue<bool>()==false &&
        matrix["codec_implemented"]?.GetValue<bool>()==false &&
        matrix["parser_changed"]?.GetValue<bool>()==false,
        "IR decision must preserve the existing exact primitive and not claim implemented code.");
    var rows=A(matrix,"mapping").Select(x=>x!.AsObject()).ToArray();
    var accepted=new HashSet<string>(StringComparer.Ordinal)
        {"rational","quantity","matrix","function","exclusion"};
    Require(rows.Length==5 &&
        rows.Select(x=>S(x,"id")).ToHashSet(StringComparer.Ordinal).SetEquals(accepted) &&
        rows.All(x=>S(x,"canonical_node").Length>0 &&
            S(x,"openmath").Length>0 && S(x,"strict_content_mathml").Length>0 &&
            S(x,"condition").Length>0 &&
            A(x,"preserved").Count>0 && A(x,"unsupported").Count>0),
        "Mapping/loss ledger must include all five admitted cases and explicit guards.");
    var rationals=rows.Single(x=>S(x,"id")=="rational");
    Require(S(rationals["value"]!.AsObject(),"numerator")=="1" &&
        S(rationals["value"]!.AsObject(),"denominator")=="3",
        "Exact rational mapping is altered.");
    var quantity=rows.Single(x=>S(x,"id")=="quantity");
    Require(S(quantity["value"]!.AsObject(),"unit_id")=="imperial.inch" &&
        A(quantity["value"]!.AsObject(),"dimension").Count==8,
        "Quantity mapping has lost the canonical ID or information dimension.");
    var exclusion=rows.Single(x=>S(x,"id")=="exclusion");
    Require(A(exclusion["value"]!.AsObject(),"assumptions").Count>0 &&
        S(exclusion,"condition").Contains("restriction",StringComparison.Ordinal),
        "The x != 1 exclusion cannot be discarded or treated as presentation.");
    Require(S(matrix["adjacent_symbolic_guard"]!.AsObject(),"must_remain")=="abs(x)",
        "Real-domain sqrt(x²) must not be confused with x absent a positivity assumption.");
    var policies=matrix["admission_policy"]!.AsObject();
    Require(S(policies,"complex")=="deferred" &&
        S(policies,"interval_arithmetic").StartsWith("deferred",StringComparison.Ordinal) &&
        S(policies,"nan_infinity")=="rejected" &&
        S(policies,"binding").Contains("stable symbol IDs",StringComparison.Ordinal) &&
        S(policies,"hashing").Contains("presentation",StringComparison.Ordinal),
        "The architecture lacks required domain, binding or hashing boundaries.");
    var limits=policies["limits_proposed_only"]!.AsObject();
    Require(limits["maximum_nodes"]?.GetValue<int>()==1024 &&
        limits["maximum_depth"]?.GetValue<int>()==32 &&
        limits["maximum_matrix_elements"]?.GetValue<int>()==4096,
        "Proposed, not implemented IR resource limits have drifted.");

    Require(S(qualified,"status")=="typed-ir-adr-qualified-no-product-implementation" &&
        qualified["product_ir_compiled"]?.GetValue<bool>()==false &&
        qualified["adoption_transmitted"]?.GetValue<bool>()==false,
        "Evidence must not claim an implemented IR or sent external request.");
    Require(S(priorGate,"status")=="local-exact-m1-gate-qualified-external-adoption-not-started",
        "M1 extraction authority must remain qualified.");
    Require(S(adoption,"status")=="draft" && S(adoption,"transmission")=="none",
        "M2 architecture work cannot transmit the AURA adoption request.");

    foreach(var token in new[]{
        "OpenMath 2.0 revision 2", "Content MathML",
        "ExactRational", "BigRational",
        "sqrt(x²)", "x != 1", "canonical JSON",
        "binding", "capture-free", "NaN", "infinities",
        "MaximumNodes=1024", "MaximumDepth=32",
        "not an implemented IR library"})
        Require(decision.Contains(token,StringComparison.OrdinalIgnoreCase),
            "The IR ADR lacks mandatory boundary decision: " + token);
    Require(File.Exists(Path.Combine(root,
        "docs/planning/evidence/M1-W03-C02-gate.json")) &&
        !File.Exists(Path.Combine(root,"src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj")),
        "This ADR gate must not create an empty or unqualified IR project.");

    Console.WriteLine("M2 IR ADR valid: five fidelity rows, bound symbols and exclusions, no IR library and no external adoption claimed.");
    return 0;
}
catch(Exception e)
{
    Console.Error.WriteLine("M2 IR ADR invalid: " + e.Message);
    return 1;
}
static void Require(bool value, string message)
{
    if(!value) throw new InvalidOperationException(message);
}
