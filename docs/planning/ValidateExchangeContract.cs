#:property PublishAot=false
#:property NuGetAudit=false
using System.Text.Json.Nodes;
try
{
    var root=Directory.GetCurrentDirectory();
    var exchange=Load(Path.Combine(root,"docs","planning","evidence","M0-W02-C03-exchange-contract.json"));
    var request=Load(Path.Combine(root,"docs","coordination","requests","MATH-XR-001-aura-baseline.json"));
    var backlog=Load(Path.Combine(root,"docs","planning","backlog.json"));

    Require(S(exchange,"authority").Contains("human owner",StringComparison.OrdinalIgnoreCase),"Exchange authority must remain owner-mediated.");
    var states=Strings(exchange,"states");
    Require(states.SequenceEqual(new[]{"draft","ready_for_owner","sent","acknowledged","accepted","rejected","implemented","verified"},StringComparer.Ordinal),"Exchange state order changed.");
    var transition=exchange["transition_evidence"]?.AsObject()??throw new InvalidOperationException("Missing transition_evidence.");
    foreach(var state in new[]{"sent","acknowledged","accepted","rejected","implemented","verified"})
        Require(!string.IsNullOrWhiteSpace(S(transition,state)),"Missing evidence rule for "+state+".");

    Require(S(request,"id")=="MATH-XR-001-BASELINE","Unexpected request id.");
    Require(S(request,"status")=="draft","Baseline request must remain draft until owner transmission.");
    Require(S(request,"transmission")=="none","Baseline request must not claim transmission.");
    Require(S(request,"source_revision")=="82a6b435a387a7e116b47a6b2c433ae9e067bf21","Baseline request source revision mismatch.");
    Require(S(request,"source_manifest")=="docs/planning/source-baseline.json","Baseline request source manifest mismatch.");
    Require(S(request,"transfer_manifest")=="docs/planning/evidence/M0-W02-C02-transfer-manifest.json","Baseline request transfer manifest mismatch.");
    Require(Strings(request,"package_identities").SequenceEqual(new[]{"dw.quantities","dw.quantities.expression","dw.quantities.standard"},StringComparer.Ordinal),"Baseline package identities changed.");
    Require(A(request,"requested_actions").Count>=5,"Baseline request is missing requested actions.");
    Require(A(request,"non_goals").Count>=4,"Baseline request is missing non-goals.");
    Require(A(request,"host_validation").Count>=4,"Baseline request is missing host validation criteria.");

    var recipient=request["indicative_recipient_scope"]?.AsObject()??throw new InvalidOperationException("Missing indicative_recipient_scope.");
    Require(A(recipient,"direct_transfer_sources").Count==20,"Baseline request direct-transfer source count mismatch.");
    Require(A(recipient,"behavioral_adaptation_sources").Count==1,"Baseline request adaptation count mismatch.");
    Require(A(recipient,"retained_host_sources").Count==8,"Baseline request retained-host source count mismatch.");

    var response=request["expected_response"]?.AsObject()??throw new InvalidOperationException("Missing expected_response.");
    Require(Strings(response,"decision").SequenceEqual(new[]{"accepted","rejected","needs_change"},StringComparer.Ordinal),"Expected response decision contract changed.");
    Require(A(response,"fields").Count>=6,"Expected response fields are incomplete.");

    var requestMarkdown=File.ReadAllText(Path.Combine(root,"docs","coordination","requests","MATH-XR-001-aura.md"));
    Require(requestMarkdown.Contains("Status: **draft**",StringComparison.Ordinal),"Human-readable request must remain draft.");
    Require(requestMarkdown.Contains("Transmission: **none**",StringComparison.Ordinal),"Human-readable request must state no transmission.");

    Require(S(FindById(backlog,"chunks","M0-W02-C03"),"status")=="done","M0-W02-C03 must be done.");
    Require(S(FindById(backlog,"work_packages","M0-W02"),"status")=="done","M0-W02 must be done.");
    Require(S(FindById(backlog,"releases","M0"),"status")=="done","M0 must be done.");
    Require(S(FindById(backlog,"releases","M1"),"status")=="ready","M1 must be ready.");
    Require(S(FindById(backlog,"chunks","M1-W01-C01"),"status")=="ready","M1-W01-C01 must be ready.");

    Console.WriteLine("exchange contract valid: MATH-XR-001 draft/untransmitted; M0 closed; M1 exact-rational slice ready");
    return 0;
}
catch(Exception ex){Console.Error.WriteLine("exchange contract invalid: "+ex.Message);return 1;}
static JsonObject Load(string path)=>JsonNode.Parse(File.ReadAllText(path))?.AsObject()??throw new InvalidOperationException("Invalid or empty JSON: "+path);
static JsonArray A(JsonObject o,string key)=>o[key] as JsonArray??throw new InvalidOperationException("Missing array "+key);
static string S(JsonObject o,string key)=>o[key]?.GetValue<string>() is {Length:>0} value?value:throw new InvalidOperationException("Missing string "+key);
static string[] Strings(JsonObject o,string key)=>A(o,key).Select(x=>x?.GetValue<string>()??throw new InvalidOperationException("Null string "+key)).ToArray();
static JsonObject FindById(JsonObject root,string arrayName,string id)=>A(root,arrayName).Select(x=>x!.AsObject()).Single(x=>S(x,"id")==id);
static void Require(bool value,string message){if(!value)throw new InvalidOperationException(message);}
