#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Report129=".aura/workflow/reports/RS129/attempt-001.json";
const string Receipt129="docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json";
const string Receipt124="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
const string M7="docs/planning/evidence/M7-internal-gate-qualified.json";
const string Readme="README.md";
const string Guide="docs/distribution/current-local-consumption.md";
const string Decision="docs/planning/decisions/post-m7-readme-reconciliation.md";
const string Validator="docs/planning/ValidatePostM7ReadmeStatus.cs";
const string Evidence="docs/planning/evidence/RS130-post-m7-readme-qualified.json";
const string Status="rs130-current-math-readme-and-immutable-historical-proof-reconciled-unpublished";
const string Counts="MATH-RS130:PACKAGES=13;REPORTS=5;CONSUMERS=7;PROBES=95;EXACT-SMOKE=PASS;NODES=541";
const string Scope="MATH-RS130:DOCS=CURRENT;LEGACY-ANCHOR=ARCHIVED;SOURCE-PINS=UNCHANGED;PUBLICATION=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs130-readme-reconciliation/1.0 qualified; current=2026-10-21; archive=2026-10-07; public=false";
const string OldStatus="Status on 2026-10-07: initial planning; no mathematical engine has been implemented in this repository yet.";
const string CurrentStatus="As of 2026-10-21: implemented libraries and local delivery qualification completed; no public NuGet release.";

static void Need(bool ok,string reason)
{
    if(!ok) throw new InvalidOperationException("RS130 Math README reconciliation: "+reason);
}
static string Local(string relative)
{
    Need(!string.IsNullOrWhiteSpace(relative)&&!Path.IsPathRooted(relative)&&
         !relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\')&&!relative.Contains(':'),
         "Unsafe canonical source path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Local(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Sha(string relative)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(relative)))).ToLowerInvariant();
static string Text(JsonElement x,string key)=>x.GetProperty(key).GetString()??
    throw new InvalidOperationException("Missing text field "+key);
static bool Flag(JsonElement x,string key)=>x.GetProperty(key).GetBoolean();
static void Has(string source,string marker,string label)=>
    Need(source.Contains(marker,StringComparison.Ordinal),
         label+" is missing "+marker);
static string Property(XDocument xml,string name)=>
    xml.Descendants().FirstOrDefault(x=>x.Name.LocalName==name)?.Value.Trim()??
    throw new InvalidOperationException("Missing source property "+name);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int Incomplete(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Incomplete);
    return n;
}
Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "This verifier is read-only: only --check is supported.");

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var reportDoc=Open(Report129);
using var priorDoc=Open(Receipt129);
using var m7Doc=Open(M7);
using var oldDoc=Open(Receipt124);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var report=reportDoc.RootElement;
var prior=priorDoc.RootElement;
var m7=m7Doc.RootElement;
var old=oldDoc.RootElement;

Need(Text(backlog,"plan_version")=="0.1.122"&&Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("releases").EnumerateArray().All(x=>
         Text(x,"status")=="done"),
     "Accepted product roadmap may not be rewritten by README work.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Incomplete)==0,
     "Original 541-node DWF roadmap must remain completely done.");
Need(Text(report,"runId")=="RS129"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&
     !Flag(report,"correctionMode")&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     Text(report.GetProperty("git"),"commitMessage")==
         "docs: qualify post-M7 Math delivery evidence portfolio without publication"&&
     report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(report.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "RS129 first-pass successful predecessor evidence differs.");
Need(Text(prior,"status")==
         "rs129-source-pinned-post-m7-local-delivery-portfolio-qualified-unpublished"&&
     prior.GetProperty("verified_run_reports").GetInt32()==5&&
     prior.GetProperty("source_projects").GetInt32()==13&&
     prior.GetProperty("test_projects").GetInt32()==11&&
     prior.GetProperty("historical_consumer_directories").GetInt32()==47&&
     prior.GetProperty("freshly_packed_package_roster").GetInt32()==13&&
     prior.GetProperty("fresh_consumer_executions").GetInt32()==7&&
     prior.GetProperty("numbered_consumer_probes").GetInt32()==95&&
     Flag(prior,"exact_quantity_smoke_passed")&&
     prior.GetProperty("run_first_pass_yes").GetInt32()==3&&
     prior.GetProperty("run_first_pass_no").GetInt32()==2&&
     prior.GetProperty("preparation_refusals_total").GetInt32()==3&&
     prior.GetProperty("native_nodes_retained").GetInt32()==541&&
     !Flag(prior,"readme_current")&&
     !Flag(prior,"publicly_published")&&
     !Flag(prior,"public_release_authorized")&&
     !Flag(prior,"external_requests_transmitted")&&
     !Flag(prior,"external_approvals_received")&&
     !Flag(prior,"cross_version_abi_certified")&&
     !Flag(prior,"discovery_skill_generated")&&
     !Flag(prior,"future_program_approved"),
     "Historic RS129 local-delivery evidence or publication limits changed.");
foreach(var (field,document) in new[]{
    ("decision_sha256","docs/planning/decisions/post-m7-delivery-portfolio.md"),
    ("distribution_sha256","docs/distribution/post-m7-delivery-portfolio-matrix.md"),
    ("validator_sha256","docs/planning/ValidatePostM7DeliveryPortfolio.cs")
}) Need(Text(prior,field)==Sha(document),
    "Source-pinned RS129 document or checker SHA drifted: "+document);
Need(Text(old,"status")==
         "rs124-current-source-build-test-and-three-local-packages-qualified-unpublished"&&
     Text(old,"validator_sha256")==Sha(
         "docs/planning/ValidatePostM7DeliveryInventory.cs")&&
     !Flag(old,"publicly_published"),
     "Historic RS124 inventory validator must remain byte-identical.");
Need(Text(m7,"status")==
         "m7-internal-advanced-research-and-scope-gate-qualified-publication-withheld"&&
     Flag(m7,"m7_internal_scope_done")&&
     !Flag(m7,"public_release_authorized")&&
     !Flag(m7,"publicly_published")&&
     !Flag(m7,"external_requests_transmitted")&&
     !Flag(m7,"future_program_approved"),
     "M7 done is not a new public Math delivery scope.");
Has(Read("docs/planning/ValidatePostM7DeliveryPortfolio.cs"),
    "Status on 2026-10-07: initial planning",
    "Immutable older source-pinned portfolio validator");
Has(Read("docs/planning/ValidatePostM7DeliveryInventory.cs"),
    "Status on 2026-10-07",
    "Immutable older source-pinned inventory validator");

var readme=Read(Readme);
Need(readme.StartsWith("# dw.tools.math\n\n",
         StringComparison.Ordinal)&&
     readme.IndexOf("## Current status — 2026-10-21",
         StringComparison.Ordinal) is > 0 and < 600,
     "README must lead with present-day verified status.");
Has(readme,CurrentStatus,"Current README status");
const string ArchivedHeading=
    "## Historical baseline (2026-10-07) — archived statement, not current status";
var section=readme.IndexOf(ArchivedHeading,StringComparison.Ordinal);
var historic=readme.IndexOf(OldStatus,StringComparison.Ordinal);
Need(section>0&&historic>section&&
     readme.LastIndexOf(OldStatus,StringComparison.Ordinal)==historic&&
     !readme[..section].Contains(OldStatus,StringComparison.Ordinal),
     "Exactly one old status must be retained only in the archived section.");
Need(readme.IndexOf(CurrentStatus,StringComparison.Ordinal)<section&&
     readme.IndexOf("## What is actually available?",StringComparison.Ordinal)<section,
     "The historical planning snapshot cannot be mistaken for current scope.");
foreach(var token in new[]{
    "13 packable .NET 10 source libraries",
    "11 solution test projects",
    "7 independent source-free consumer executions",
    "95 numbered functional probes",
    "unnumbered exact-quantity smoke",
    "No public NuGet release",
    "docs/distribution/current-local-consumption.md",
    "docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json",
    "docs/discovery/README.md",
    "docs/coordination/requests/README.md",
    "M7 precision",
    "dotnet restore dw.tools.math.slnx --locked-mode",
    "dotnet build dw.tools.math.slnx -c Release --no-restore",
    "dotnet test dw.tools.math.slnx -c Release --no-build --no-restore"
}) Has(readme,token,"Reconciled current README");

var guide=Read(Guide);
foreach(var token in new[]{
    "# Current local Math delivery and consumption",
    "## Verified package roster (13 packable projects)",
    "## Verified evidence by run",
    "## Local developer workflow",
    "## Open release and agent gates",
    "RS124","RS125","RS126","RS127","RS128","RS129",
    "7 independent","95 numbered probes",
    "No public NuGet packages",
    "AURA",
    "Discovery"
}) Has(guide,token,"Local delivery guide");
var table=guide.Split("## Verified evidence by run",2,
    StringSplitOptions.None)[0].Split('\n')
    .Where(x=>x.StartsWith("| `dw",StringComparison.Ordinal))
    .ToArray();
var expected=new (string Id,string Version)[]
{
    ("dw.tools.math.foundation","0.1.0-preview.1"),
    ("dw.quantities","0.2.0-preview.1"),
    ("dw.quantities.standard","0.2.0-preview.1"),
    ("dw.quantities.expression","0.2.0-preview.1"),
    ("dw.tools.math.ir","0.3.0-preview.1"),
    ("dw.tools.math.composition","0.3.0-preview.1"),
    ("dw.tools.math.numerics","0.4.0-preview.4"),
    ("dw.tools.math.statistics","0.4.0-preview.6"),
    ("dw.tools.math.symbolics","0.5.0-preview.10"),
    ("dw.tools.math.codecs","1.0.0-preview.8"),
    ("dw.tools.math.distribution","1.0.0-preview.4"),
    ("dw.tools.math.analysis","1.1.0-preview.6"),
    ("dw.tools.math.scientific","1.1.0-preview.14")
};
Need(table.Length==expected.Length,
    "The local delivery guide must name exactly thirteen packages.");
foreach(var (id,version) in expected)
{
    var entry=table.Where(x=>x.StartsWith("| `"+id+"` |",
        StringComparison.Ordinal)).ToArray();
    Need(entry.Length==1&&
         entry[0].Contains("| `"+version+"` |",StringComparison.Ordinal),
         "Wrong, duplicate or missing guide package identity: "+id);
    var doc=XDocument.Parse(Read("src/projects/"+id+"/"+id+".csproj"));
    Need(Property(doc,"PackageId")==id&&
         Property(doc,"Version")==version&&
         Property(doc,"IsPackable")=="true",
         "Documented package differs from current packable source: "+id);
}
Need(Property(XDocument.Parse(Read("Directory.Build.props")),
         "TargetFramework")=="net10.0",
     "Current README SDK target must match the source project policy.");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","Unsent downstream requests");
Has(Read("docs/discovery/README.md"),
    "No generated Math skill","Candidate Math Discovery boundary");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "An unqualified generated agent skill is outside README reconciliation.");

var decision=Read(Decision);
foreach(var token in new[]{
    "## Owner approval and one-run outcome",
    "## The exact historical-anchor problem and resolution",
    "## Proof and process",
    "## What follows only after durable success",
    "A (documentation) → B (Discovery/AURA qualification) → D (interproject coordination)",
    "READMe" // case-insensitive handling below
}) Need(decision.Contains(token,StringComparison.OrdinalIgnoreCase),
    "Decision record lacks "+token);

if(File.Exists(Local(Evidence)))
{
    using var receiptDoc=Open(Evidence);
    var done=receiptDoc.RootElement;
    Need(Text(done,"status")==Status&&
         Flag(done,"readme_current")&&
         Flag(done,"archived_baseline_preserved")&&
         Flag(done,"historical_source_pins_preserved")&&
         done.GetProperty("source_projects").GetInt32()==13&&
         done.GetProperty("independent_consumer_executions").GetInt32()==7&&
         done.GetProperty("numbered_probes_historical").GetInt32()==95&&
         Flag(done,"exact_quantity_smoke_passed")&&
         done.GetProperty("native_nodes_retained").GetInt32()==541&&
         Text(done,"approved_sequence")=="A-B-D"&&
         !Flag(done,"m0_m7_plan_mutated")&&
         !Flag(done,"publicly_published")&&
         !Flag(done,"external_requests_transmitted")&&
         !Flag(done,"external_approvals_received")&&
         !Flag(done,"cross_version_abi_certified")&&
         !Flag(done,"other_os_rids_certified")&&
         !Flag(done,"discovery_skill_generated")&&
         !Flag(done,"future_program_approved")&&
         Text(done,"readme_sha256")==Sha(Readme)&&
         Text(done,"guide_sha256")==Sha(Guide)&&
         Text(done,"decision_sha256")==Sha(Decision)&&
         Text(done,"validator_sha256")==Sha(Validator),
         "Documentation target receipt is not source-pinned or overclaims delivery.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
