#:property PublishAot=false
#:property NuGetAudit=false

using System.Text.Json.Nodes;

try
{
    var root = Directory.GetCurrentDirectory();
    var baseline = Load(Path.Combine(root, "docs", "planning", "source-baseline.json"));
    var backlog = Load(Path.Combine(root, "docs", "planning", "backlog.json"));
    var audit = Load(Path.Combine(root, "docs", "planning", "evidence", "M0-W02-C01-provenance-audit.json"));
    var boundary = Load(Path.Combine(root, "docs", "planning", "evidence", "M0-W02-C01-transfer-boundary.json"));

    var repos = A(baseline, "repositories").Select(x => x!.AsObject()).ToArray();
    Require(repos.Length == 4, "Expected four observed source repositories.");
    var counts = repos.ToDictionary(x => S(x, "repository"), x => A(x, "files").Count, StringComparer.Ordinal);
    Require(counts["aura"] == 43 && counts["decision"] == 6 && counts["mcdm"] == 4 && counts["brainstorming"] == 1,
        "Source-baseline repository file counts drifted.");
    Require(counts.Values.Sum() == 54, "Source-baseline selected file count drifted from 54.");
    Require(S(baseline, "scope").Contains("no source imported", StringComparison.OrdinalIgnoreCase),
        "Source baseline no longer states that no source was imported.");

    Require(S(audit, "status") == "qualified-owner-rights-satisfied", "Unexpected provenance audit status.");
    Require(audit["selected_file_count"]?.GetValue<int>() == 54, "Provenance audit file count mismatch.");
    var rights = audit["rights_decision"]?.AsObject() ?? throw new InvalidOperationException("Missing rights_decision.");
    Require(S(rights, "state") == "satisfied", "Rights decision must be satisfied.");
    Require(rights["authorized_to_copy_distributable_source"]?.GetValue<bool>() == true,
        "Audit must record owner authorization for distributable source copying.");
    Require(S(rights, "ownership_model") == "owner-authorized proprietary code", "Unexpected ownership model.");

    Require(S(boundary, "status") == "qualified", "Transfer boundary is not qualified.");
    Require(A(boundary, "excluded_from_transfer").Count >= 5, "Transfer boundary exclusion list is incomplete.");

    var w02 = FindById(backlog, "work_packages", "M0-W02");
    var c01 = FindById(backlog, "chunks", "M0-W02-C01");
    Require(S(w02, "status") is "in_progress" or "done", "M0-W02 must be in_progress or done after provenance qualification.");
    Require(S(c01, "status") == "done", "M0-W02-C01 must be done after owner-rights closure.");
    var tasks = A(c01, "tasks").Select(x => x!.AsObject()).ToDictionary(x => S(x, "id"), StringComparer.Ordinal);
    Require(S(tasks["M0-W02-C01-T1"], "status") == "done", "T1 must be done after owner authorization.");
    Require(S(tasks["M0-W02-C01-T2"], "status") == "done", "T2 transfer boundary must remain done.");

    var ext = FindById(backlog, "external_dependencies", "EXT-SOURCE-RIGHTS");
    Require(S(ext, "status") == "satisfied", "EXT-SOURCE-RIGHTS must be satisfied.");
    Require(A(ext, "evidence").Any(x => string.Equals(x?.GetValue<string>(), "docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json", StringComparison.Ordinal)),
        "EXT-SOURCE-RIGHTS is missing the owner attestation evidence.");

    var attestationPath = Path.Combine(root, "docs", "planning", "evidence", "M0-W02-C01-owner-rights-attestation.json");
    Require(File.Exists(attestationPath), "Missing owner-rights attestation.");
    var attestation = Load(attestationPath);
    Require(S(attestation, "source_revision") == "82a6b435a387a7e116b47a6b2c433ae9e067bf21", "Owner attestation source revision mismatch.");
    var authorization = attestation["authorization"]?.AsObject() ?? throw new InvalidOperationException("Missing authorization object.");
    foreach (var key in new[] { "copy_into_dw_tools_math", "modify_in_dw_tools_math", "package_from_dw_tools_math", "redistribute_from_dw_tools_math" })
        Require(authorization[key]?.GetValue<bool>() == true, "Owner attestation does not authorize " + key + ".");

    var decisionPath = Path.Combine(root, "docs", "planning", "decisions", "m0-w02-c01-source-rights.md");
    Require(File.Exists(decisionPath), "Missing source-rights decision record.");
    var decisionText = File.ReadAllText(decisionPath);
    Require(decisionText.Contains("Satisfied by owner attestation", StringComparison.Ordinal),
        "Source-rights decision does not record satisfaction.");

    var c02 = FindById(backlog, "chunks", "M0-W02-C02");
    Require(S(c02, "status") is "ready" or "done", "M0-W02-C02 must be ready or done after C01 closure.");

    Console.WriteLine("transfer provenance valid: 54 selected files; owner rights satisfied; M0-W02-C01 closed");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("transfer provenance invalid: " + ex.Message);
    return 1;
}

static JsonObject Load(string path) =>
    JsonNode.Parse(File.ReadAllText(path))?.AsObject()
    ?? throw new InvalidOperationException("Invalid or empty JSON: " + path);

static JsonArray A(JsonObject o, string key) =>
    o[key] as JsonArray ?? throw new InvalidOperationException("Missing array " + key);

static string S(JsonObject o, string key) =>
    o[key]?.GetValue<string>() is { Length: > 0 } value
        ? value
        : throw new InvalidOperationException("Missing string " + key);

static JsonObject FindById(JsonObject root, string arrayName, string id) =>
    A(root, arrayName).Select(x => x!.AsObject()).Single(x => S(x, "id") == id);

static void Require(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}
