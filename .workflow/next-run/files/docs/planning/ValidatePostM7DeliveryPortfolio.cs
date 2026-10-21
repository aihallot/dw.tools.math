#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Decision="docs/planning/decisions/post-m7-delivery-portfolio.md";
const string Matrix="docs/distribution/post-m7-delivery-portfolio-matrix.md";
const string Validator="docs/planning/ValidatePostM7DeliveryPortfolio.cs";
const string Evidence="docs/planning/evidence/RS129-post-m7-delivery-portfolio-qualified.json";
const string Status="rs129-source-pinned-post-m7-local-delivery-portfolio-qualified-unpublished";
const string Counts="MATH-RS129:RUNS=5;PACKABLE=13;CONSUMER-EXECUTIONS=7;NUMBERED-PROBES=95;EXACT-SMOKE=PASS;NATIVE=541";
const string Scope="MATH-RS129:PORTFOLIO=EVIDENCE-ONLY;FIRST-PASS=3-YES-2-NO;PUBLICATION=false;ABI=false;EXTERNAL=UNSENT";
const string Marker="dw.tools.math/rs129-delivery-portfolio/1.0 qualified; reports=5; packages=13; clients=7; probes=95; publication=false";

static void Need(bool yes,string why)
{
    if(!yes) throw new InvalidOperationException("Math RS129 delivery evidence: "+why);
}
static string Local(string path)
{
    Need(!string.IsNullOrWhiteSpace(path)&&
         !Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\')&&!path.Contains(':'),
         "Unexpected source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(path)))).ToLowerInvariant();
static string Text(JsonElement x,string key)=>x.GetProperty(key).GetString()??
    throw new InvalidOperationException("Missing text field "+key);
static bool Flag(JsonElement x,string key)=>x.GetProperty(key).GetBoolean();
static void Has(string source,string fragment,string label)=>
    Need(source.Contains(fragment,StringComparison.OrdinalIgnoreCase),
         label+" lacks "+fragment);
static int Count(JsonElement node)
{
    var n=1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int Undone(JsonElement node)
{
    var n=Text(node,"state")=="done"?0:1;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(key,out var children))
            n+=children.EnumerateArray().Sum(Undone);
    return n;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only the read-only --check contract is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
Need(Text(backlog,"plan_version")=="0.1.122"&&
     Text(backlog,"status")=="done"&&
     backlog.GetProperty("releases").GetArrayLength()==8&&
     backlog.GetProperty("work_packages").GetArrayLength()==17&&
     backlog.GetProperty("chunks").GetArrayLength()==58&&
     backlog.GetProperty("requirements").GetArrayLength()==106&&
     backlog.GetProperty("source_coverage").GetArrayLength()==20&&
     backlog.GetProperty("dispositions").GetArrayLength()==6&&
     backlog.GetProperty("releases").EnumerateArray()
        .All(x=>Text(x,"status")=="done")&&
     backlog.GetProperty("work_packages").EnumerateArray()
        .All(x=>Text(x,"status")=="done")&&
     backlog.GetProperty("chunks").EnumerateArray()
        .All(x=>Text(x,"status")=="done"),
     "Completed eight-milestone accepted product roadmap has changed.");
Need(native.GetProperty("milestones").GetArrayLength()==8&&
     native.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     native.GetProperty("milestones").EnumerateArray().Sum(Undone)==0,
     "All 541 accepted Math DWF nodes must remain done.");
var roster=new (string Run,string Message,string ReceiptStatus,int Refusals,bool FirstPass,int CommandCount)[]
{
    ("RS124","docs: qualify post-M7 local delivery inventory without publication",
     "rs124-current-source-build-test-and-three-local-packages-qualified-unpublished",1,false,10),
    ("RS125","docs: qualify fresh offline Math NuGet consumers without publication",
     "rs125-two-fresh-isolated-source-free-nuget-consumers-qualified-unpublished",0,true,9),
    ("RS126","docs: qualify offline Math distribution package graph and API surface",
     "rs126-fresh-seven-package-offline-distribution-surface-qualified-unpublished",0,true,12),
    ("RS127","docs: qualify full local Math package inventory and isolated clients",
     "rs127-thirteen-fresh-local-packages-two-independent-consumers-qualified-unpublished",2,false,20),
    ("RS128","docs: qualify fresh offline Math symbolics and codec consumers",
     "rs128-four-local-symbolic-codec-packages-two-source-free-consumers-qualified-unpublished",0,true,12)
};
var firstPass=0;
var firstPassNo=0;
var refusalCount=0;
foreach(var (run,commit,status,refusals,first,commandCount) in roster)
{
    var reportPath=".aura/workflow/reports/"+run+"/attempt-001.json";
    var receiptPath="docs/planning/evidence/"+run+"-post-m7-"+(
        run switch
        {
            "RS124"=>"delivery",
            "RS125"=>"nuget-consumers",
            "RS126"=>"distribution",
            "RS127"=>"complete-packaging",
            "RS128"=>"symbolic-codec",
            _=>throw new InvalidOperationException("Unknown run identity")
        })+"-qualified.json";
    // RS124 uses a distinct fully qualified filename.
    if(run=="RS124")
        receiptPath="docs/planning/evidence/RS124-post-m7-delivery-qualified.json";
    if(run=="RS125")
        receiptPath="docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json";
    if(run=="RS126")
        receiptPath="docs/planning/evidence/RS126-post-m7-distribution-qualified.json";
    if(run=="RS127")
        receiptPath="docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json";
    if(run=="RS128")
        receiptPath="docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json";
    using var reportDoc=Open(reportPath);
    using var receiptDoc=Open(receiptPath);
    var report=reportDoc.RootElement;
    var receipt=receiptDoc.RootElement;
    Need(Text(report,"runId")==run&&
         report.GetProperty("attempt").GetInt32()==1&&
         Text(report,"outcome")=="succeeded"&&
         !Flag(report,"correctionMode")&&
         Text(report.GetProperty("git"),"commitMessage")==commit&&
         Text(report.GetProperty("executor"),"toolVersion")=="0.1.59",
         "Historical run report or commit identity changed: "+run);
    var failures=report.GetProperty("priorInvocationFailures");
    Need(failures.GetProperty("count").GetInt32()==refusals&&
         failures.GetProperty("preparationFailureCount").GetInt32()==refusals&&
         failures.GetProperty("allocatedAttemptFailureCount").GetInt32()==0&&
         Flag(failures,"firstPassSucceeded")==first,
         "Prior refusal/first-pass accounting is untruthful: "+run);
    Need(Text(receipt,"status")==status&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         Text(receipt,"dotnet_sdk")=="10.0.402"&&
         Text(receipt,"observed_runtime_identifier")=="win-x64"&&
         receipt.GetProperty("command_receipts").GetArrayLength()==commandCount&&
         receipt.GetProperty("command_receipts").EnumerateArray()
             .All(x=>x.GetProperty("exit_code").GetInt32()==0)&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"external_approvals_received")&&
         !Flag(receipt,"cross_version_abi_certified")&&
         !Flag(receipt,"other_os_rids_certified"),
         "Observed post-M7 evidence was promoted beyond a local acceptance: "+run);
    foreach(var (field,path) in new[]{
        ("decision_sha256",Text(receipt,"decision_document")),
        ("distribution_sha256",Text(receipt,"distribution_document")),
        ("validator_sha256",Text(receipt,"validator"))
    })
        Need(Text(receipt,field)==Sha(path),
             "Previous source-pinned evidence SHA changed: "+run+" "+path);
    if(run=="RS124")
        Need(receipt.GetProperty("source_projects").GetInt32()==13&&
             receipt.GetProperty("test_projects").GetInt32()==11&&
             receipt.GetProperty("historical_consumer_directories").GetInt32()==47&&
             Flag(receipt,"full_solution_tests_passed")&&
             !Flag(receipt,"fresh_source_free_consumer_rerun"),
             "RS124 historical suite or no-client claim drifted.");
    if(run=="RS125")
        Need(receipt.GetProperty("source_free_consumers").GetInt32()==2&&
             receipt.GetProperty("fresh_consumer_probes").GetInt32()==16,
             "RS125 two consumers / sixteen probes drifted.");
    if(run=="RS126")
        Need(receipt.GetProperty("packages_packed").GetInt32()==7&&
             receipt.GetProperty("source_free_consumers").GetInt32()==1&&
             receipt.GetProperty("fresh_consumer_probes").GetInt32()==8&&
             receipt.GetProperty("package_results").GetArrayLength()==7,
             "RS126 local public surface evidence drifted.");
    if(run=="RS127")
        Need(receipt.GetProperty("packages_packed").GetInt32()==13&&
             receipt.GetProperty("source_free_consumers").GetInt32()==2&&
             receipt.GetProperty("m3_fresh_probes").GetInt32()==31&&
             Flag(receipt,"exact_quantity_smoke_passed")&&
             !Flag(receipt,"foundation_source_free_consumer_rerun")&&
             receipt.GetProperty("package_results").GetArrayLength()==13,
             "RS127 thirteen-package and separate smoke claims drifted.");
    if(run=="RS128")
        Need(receipt.GetProperty("packages_packed").GetInt32()==4&&
             receipt.GetProperty("source_free_consumers").GetInt32()==2&&
             receipt.GetProperty("fresh_consumer_probes").GetInt32()==40,
             "RS128 forty symbolic/codec probes drifted.");
    firstPass+=first?1:0;
    firstPassNo+=first?0:1;
    refusalCount+=refusals;
}
Need(firstPass==3&&firstPassNo==2&&refusalCount==3,
     "Aggregate first-pass and pre-attempt refusal counts changed.");
Has(Read("docs/coordination/requests/README.md"),
    "have not been sent","AURA/Decision/MCDM transmission boundary");
Has(Read("docs/discovery/README.md"),
    "No generated Math skill","Discovery product boundary");
Has(Read("README.md"),
    "Status on 2026-10-07: initial planning",
    "Root README documentation debt");
foreach(var term in new[]{
    "## Scope and decision","## Qualified observed evidence",
    "## Read-only acceptance and provenance",
    "## Future gates","## Engineering constraints",
    "95","seven","README.md","without publication"
}) Has(Read(Decision),term,"RS129 decision");
foreach(var term in new[]{
    "## Five qualified local runs","## Real delivery categories",
    "## No-progress DWF portfolio gate","95","47","13","first-pass"
}) Has(Read(Matrix),term,"RS129 distribution matrix");

if(File.Exists(Local(Evidence)))
{
    using var completedDoc=Open(Evidence);
    var receipt=completedDoc.RootElement;
    Need(Text(receipt,"status")==Status&&
         receipt.GetProperty("verified_run_reports").GetInt32()==5&&
         receipt.GetProperty("source_projects").GetInt32()==13&&
         receipt.GetProperty("test_projects").GetInt32()==11&&
         receipt.GetProperty("historical_consumer_directories").GetInt32()==47&&
         receipt.GetProperty("freshly_packed_package_roster").GetInt32()==13&&
         receipt.GetProperty("fresh_consumer_executions").GetInt32()==7&&
         receipt.GetProperty("numbered_consumer_probes").GetInt32()==95&&
         Flag(receipt,"exact_quantity_smoke_passed")&&
         receipt.GetProperty("run_first_pass_yes").GetInt32()==3&&
         receipt.GetProperty("run_first_pass_no").GetInt32()==2&&
         receipt.GetProperty("preparation_refusals_total").GetInt32()==3&&
         receipt.GetProperty("native_nodes_retained").GetInt32()==541&&
         receipt.GetProperty("lineage_receipts").GetArrayLength()==5&&
         !Flag(receipt,"readme_current")&&
         !Flag(receipt,"m0_m7_plan_mutated")&&
         !Flag(receipt,"publicly_published")&&
         !Flag(receipt,"external_requests_transmitted")&&
         !Flag(receipt,"cross_version_abi_certified")&&
         !Flag(receipt,"other_os_rids_certified")&&
         !Flag(receipt,"discovery_skill_generated")&&
         !Flag(receipt,"future_program_approved")&&
         !Flag(receipt,"new_public_product_delivered")&&
         Text(receipt,"decision_sha256")==Sha(Decision)&&
         Text(receipt,"distribution_sha256")==Sha(Matrix)&&
         Text(receipt,"validator_sha256")==Sha(Validator),
         "Portfolio target lacks bounded true facts or overclaims publication.");
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
