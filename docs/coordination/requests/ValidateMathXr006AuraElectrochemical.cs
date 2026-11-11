#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

const string JsonPath = "docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.json";
const string MarkdownPath = "docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md";
using var document = JsonDocument.Parse(File.ReadAllBytes(JsonPath));
var root = document.RootElement;
Need(Text(root, "id") == "MATH-XR-006-AURA-ELECTROCHEM", "request id");
Need(Text(root, "status") == "draft" && Text(root, "transmission") == "none", "request must remain unsent draft");
Need(Text(root, "gate") == "MATH-ELECTROCHEM-UNIT-PROFILE" && Text(root, "consumer_need") == "A2-W02-C01-U-N", "gate identity");
Need(Text(root, "math_result_commit") == "1d0923c7baecd79948d49b82380d789ad8fe3c10", "Math result commit");
var packages = root.GetProperty("package_closure").EnumerateArray().ToArray();
Need(packages.Length == 3, "three-package closure");
Need(packages.Single(x => Text(x, "id") == "dw.quantities.standard").GetProperty("version").GetString() == "20260.20261110.1001", "Standard version");
Need(Text(root.GetProperty("profile"), "id") == "math.si-electrochemical", "profile id");
Need(root.GetProperty("profile").GetProperty("unit_ids").GetArrayLength() == 6, "six opt-in units");
Need(root.GetProperty("profile").GetProperty("inherited_catalogue").GetProperty("unit_count").GetInt32() == 58, "inherited catalogue");
Need(root.GetProperty("profile").GetProperty("standard_catalogue").GetProperty("unit_count").GetInt32() == 14, "standard catalogue");
var boundaries = root.GetProperty("boundaries");
Need(!boundaries.GetProperty("aura_integration_verified").GetBoolean() &&
     !boundaries.GetProperty("aura_historical_reuse_rights_established").GetBoolean() &&
     !boundaries.GetProperty("public_release_authorized").GetBoolean() &&
     boundaries.GetProperty("math_must_not_modify_aura").GetBoolean(), "ownership boundaries");
var markdown = File.ReadAllText(MarkdownPath);
Need(markdown.Contains("Status: draft. Recipient: AURA owning agent. Transmission: none.", StringComparison.Ordinal), "markdown transmission state");
Need(markdown.Contains("A2-W02-C01-U-N", StringComparison.Ordinal) &&
     markdown.Contains("docs/distribution/electrochemical-published-readback.json", StringComparison.Ordinal), "markdown evidence");
Console.WriteLine("MATH-XR-006|draft=valid|packages=3|units=6|transmission=none");

static string Text(JsonElement element, string key) => element.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing string " + key);
static void Need(bool value, string reason)
{
    if (!value) throw new InvalidDataException("MATH-XR-006: " + reason);
}
