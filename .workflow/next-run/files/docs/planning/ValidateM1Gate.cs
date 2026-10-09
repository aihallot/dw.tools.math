#:property PublishAot=false
#:property NuGetAudit=false

using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

try
{
    var mode = args.Length == 0 ? "--check" : args.Single();
    Require(mode == "--check", "ValidateM1Gate is read-only: use --check.");
    var root = Directory.GetCurrentDirectory();
    JsonObject Read(string relative) =>
        JsonNode.Parse(File.ReadAllText(Path.Combine(root, relative)))?.AsObject()
        ?? throw new InvalidOperationException("Missing or invalid JSON: " + relative);
    string S(JsonObject node, string key) =>
        node[key]?.GetValue<string>()
        ?? throw new InvalidOperationException("Missing string: " + key);
    JsonArray A(JsonObject node, string key) =>
        node[key]?.AsArray()
        ?? throw new InvalidOperationException("Missing array: " + key);

    var plan = Read("docs/planning/backlog.json");
    var native = Read(".aura/workflow/plan/project.json");
    var adoption = Read("docs/coordination/requests/MATH-XR-002-aura-adoption.json");
    var matrix = Read("docs/coordination/matrices/m1-aura-adoption-consumers.json");
    var gate = Read("docs/planning/evidence/M1-W03-C02-gate.json");
    var rs025 = Read(".aura/workflow/reports/RS025/attempt-001.json");
    var packageReceipt = Read("docs/planning/evidence/M1-W03-C01-qualified.json");
    var rights = Read("docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json");

    Require(S(plan,"plan_version") == "0.1.25", "M1 product plan not at expected release-gate version.");
    var releases = A(plan,"releases").Select(x=>x!.AsObject()).ToArray();
    var m1 = releases.Single(x=>S(x,"id") == "M1");
    Require(S(m1,"status") == "done" && S(m1,"gate") == "G-M1", "M1 local release gate not done.");
    Require(releases.Single(x=>S(x,"id")=="M0")["status"]?.GetValue<string>()=="done",
        "Prerequisite M0 gate not complete.");

    var chunks = A(plan,"chunks").Select(x=>x!.AsObject()).ToArray();
    var workPackages = A(plan,"work_packages").Select(x=>x!.AsObject()).ToArray();
    var m1Chunks = chunks.Where(x=>S(x,"release")=="M1").ToArray();
    Require(m1Chunks.Length==9 && m1Chunks.All(x=>S(x,"status")=="done"),
        "All nine M1 chunks must be done before a local release gate.");
    var m1Works = workPackages.Where(x=>S(x,"release")=="M1").ToArray();
    Require(m1Works.Length==3 && m1Works.All(x=>S(x,"status")=="done"),
        "All three M1 work packages must be done before the gate.");
    foreach(var node in m1Chunks.Cast<JsonObject>().Concat(m1Works).Append(m1))
    {
        var proof = A(node,"evidence");
        Require(proof.Count>0,"No durable evidence for "+S(node,"id"));
        foreach(var evidencePath in proof)
        {
            var relative=evidencePath?.GetValue<string>()
                ?? throw new InvalidOperationException("Null evidence path.");
            Require(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal)
                && File.Exists(Path.Combine(root,relative)), "Missing or unsafe evidence: "+relative);
        }
    }

    var milestone = A(native,"milestones").Select(x=>x!.AsObject()).Single(x=>S(x,"id")=="M1");
    Require(S(milestone,"state")=="done","Native M1 milestone is not done.");
    var nativeWorks=A(milestone,"workPackages").Select(x=>x!.AsObject()).ToArray();
    Require(nativeWorks.Length==3 && nativeWorks.All(x=>S(x,"state")=="done"),
        "Native M1 work packages incomplete.");
    Require(nativeWorks.All(w=>A(w,"phases").All(p=>p!.AsObject()["state"]?.GetValue<string>()=="done")),
        "Native M1 phases incomplete.");

    Require(S(rs025,"outcome")=="succeeded" && rs025["attempt"]?.GetValue<int>()==1,
        "Prior RS025 package qualification not durably succeeded.");
    Require(S(packageReceipt,"status")=="isolated-local-package-consumer-and-provenance-qualified",
        "Qualified RS025 package receipt missing.");
    var receiptPackages=A(packageReceipt,"package_artifacts").Select(x=>x!.AsObject()).ToArray();
    var expectedIds=new HashSet<string>(StringComparer.Ordinal)
        {"dw.quantities","dw.quantities.expression","dw.quantities.standard"};
    Require(receiptPackages.Length==3 &&
        receiptPackages.Select(x=>S(x,"package_id")).ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds),
        "RS025 package receipt does not qualify all three Math identities.");
    Require(receiptPackages.All(x=>S(x,"version")=="0.2.0-preview.1" &&
        Regex.IsMatch(S(x,"sha256"),"^[0-9a-f]{64}$",RegexOptions.CultureInvariant)),
        "RS025 receipt has unexpected package version or SHA.");

    Require(S(rights,"source_revision")=="82a6b435a387a7e116b47a6b2c433ae9e067bf21",
        "Owner authority revision drifted.");
    Require(S(rights["license_and_notices"]!.AsObject(),"model")=="owner-authorized proprietary code",
        "No owner-authorized provenance.");
    Require(rights["authorization"]?["package_from_dw_tools_math"]?.GetValue<bool>()==true &&
        rights["authorization"]?["redistribute_from_dw_tools_math"]?.GetValue<bool>()==true,
        "Package rights have not been owner-attested.");

    Require(S(adoption,"status")=="draft" && S(adoption,"transmission")=="none",
        "Math must not assert external adoption, transmission or acknowledgement.");
    Require(S(adoption,"recipient")=="AURA owning agent", "Recipient ownership changed.");
    Require(S(matrix,"external_adoption_state")=="not_adopted" &&
        S(matrix,"transmission")=="none", "The external consumer matrix overstates adoption.");
    var rows=A(matrix,"rows").Select(x=>x!.AsObject()).ToArray();
    var expectedConsumers = new HashSet<string>(StringComparer.Ordinal)
        {"core_math_actions","nutrition","data_transforms","sqlite","json","kernel","worker_cli"};
    Require(rows.Length==7 &&
        rows.Select(x=>S(x,"consumer_id")).ToHashSet(StringComparer.Ordinal).SetEquals(expectedConsumers),
        "Required AURA transitive consumers are missing from the adoption matrix.");
    Require(rows.All(x=>S(x,"adoption_state")=="not_adopted" &&
        S(x,"test_state")=="not_run_in_aura"), "External consumer row invents AURA qualification.");

    Require(S(gate,"status")=="local-exact-m1-gate-qualified-external-adoption-not-started",
        "M1 gate evidence status is not the accepted local-only state.");
    Require(gate["local_consumer_execution_passed"]?.GetValue<bool>()==true &&
        gate["locked_solution_and_full_regressions_passed"]?.GetValue<bool>()==true &&
        gate["packages_verified"]?.GetValue<bool>()==true,
        "M1 gate lacks observed local qualification evidence.");
    var built=A(gate,"newly_built_packages").Select(x=>x!.AsObject()).ToArray();
    Require(built.Length==3 &&
        built.Select(x=>S(x,"package_id")).ToHashSet(StringComparer.Ordinal).SetEquals(expectedIds) &&
        built.All(x=>Regex.IsMatch(S(x,"sha256"),"^[0-9a-f]{64}$",RegexOptions.CultureInvariant)),
        "M1 gate must record real SHA-256 for all three newly built packages.");

    foreach (var required in new[] {
        "docs/planning/decisions/m1-w03-c02.md",
        "docs/distribution/m1-release-gate.md",
        "docs/distribution/exact-math-packages.md",
        "tests/consumers/exact-math-smoke/exact-math-smoke.csproj",
        "tests/consumers/exact-math-smoke/Program.cs",
        "tests/consumers/exact-math-smoke/NuGet.Config" })
        Require(File.Exists(Path.Combine(root,required)), "Required M1 release recipe missing: "+required);

    foreach(var proj in new[]{
        "src/projects/dw.quantities/dw.quantities.csproj",
        "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
        "src/projects/dw.quantities.standard/dw.quantities.standard.csproj"})
    {
        var content=File.ReadAllText(Path.Combine(root,proj));
        Require(!content.Contains("dw.localization",StringComparison.OrdinalIgnoreCase) &&
            !content.Contains("aura",StringComparison.OrdinalIgnoreCase) &&
            !content.Contains("../..",StringComparison.Ordinal),
            "Unexpected host or sibling dependency in "+proj);
    }
    var consumer=File.ReadAllText(Path.Combine(root,
        "tests/consumers/exact-math-smoke/exact-math-smoke.csproj"));
    Require(!consumer.Contains("ProjectReference",StringComparison.Ordinal) &&
        !consumer.Contains("aura.",StringComparison.OrdinalIgnoreCase),
        "M1 consumer is not package-only or references AURA.");

    Console.WriteLine("M1 gate valid: 9 chunks, 3 work packages, 3 local Math packages, 7 pending AURA consumer domains; no external adoption claimed.");
    return 0;
}
catch(Exception e)
{
    Console.Error.WriteLine("M1 gate invalid: "+e.Message);
    return 1;
}

static void Require(bool condition, string message)
{
    if(!condition) throw new InvalidOperationException(message);
}
