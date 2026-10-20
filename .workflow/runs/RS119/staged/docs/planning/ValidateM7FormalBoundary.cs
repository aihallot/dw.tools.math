#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string Previous="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Fixture="docs/planning/fixtures/m7-polynomial-identity-certificate.json";
const string PriorChecker="docs/planning/ValidateM7FormalCertificate.cs";
const string PriorDecision="docs/planning/decisions/m7-w02-c01-t1.md";
const string PriorScreen="docs/distribution/m7-formal-provider-screen.md";
const string Decision="docs/planning/decisions/m7-w02-c01-t2.md";
const string Distribution="docs/distribution/m7-formal-toolchain-deferral.md";
const string Evidence="docs/planning/evidence/M7-W02-C01-T2-qualified.json";
const string Phase="M7-W02-C01";
const string Task="M7-W02-C01-T2";
const string Status="m7-w02-c01-t2-formal-trusted-base-provider-deferral-qualified";
const string Marker="dw.tools.math/m7-w02-c01-t2-formal-provider-deferral/1.2 qualified; fixture=replayed; statuses=verified,invalid,unproved; triggers=3; prover=none; c01=done";
const string Witness="M7-FORMAL-BOUNDARY:POLYNOMIAL=-1,0,1;CERTIFICATE=verified-finite-identity;TRUSTED-BASE=local-bcl-only";
const string Refusal="M7-FORMAL-BOUNDARY:UNPROVED:HOLE=true;TIMEOUT=true;PROVIDER-FAILURE=true;MISSING=true;ALTERED=invalid-certificate";

static void Need(bool okay,string message)
{
    if(!okay)throw new InvalidOperationException("Independent M7 formal boundary: "+message);
}
static string Local(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||path==Native,
        "Only Math planning and local research files may be accessed.");
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
         !path.Contains('\\'),"Unsafe local evidence path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static JsonDocument Open(string path)=>JsonDocument.Parse(File.ReadAllText(Local(path)));
static string Read(string path)=>File.ReadAllText(Local(path));
static string Hash(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static string Get(JsonElement obj,string field)=>obj.GetProperty(field).GetString() ??
    throw new InvalidOperationException("Missing text field: "+field);
static bool Flag(JsonElement obj,string field)=>obj.GetProperty(field).GetBoolean();
static JsonElement ById(JsonElement array,string id)=>
    array.EnumerateArray().Single(x=>Get(x,"id")==id);
static void State(JsonElement item,string state,string field="status")=>
    Need(Get(item,field)==state,
        "Incorrect "+field+" for "+Get(item,"id")+" expected "+state);
static void Contains(string source,string fragment,string what)=>
    Need(source.Contains(fragment,StringComparison.OrdinalIgnoreCase),
        what+" lacks '"+fragment+"'");
static BigInteger[] Integers(JsonElement vector)
{
    Need(vector.ValueKind==JsonValueKind.Array,
        "Coefficient witness must be an explicit integer vector.");
    var data=vector.EnumerateArray().ToArray();
    Need(data.Length>0 && data.Length<=3,
        "Only bounded degree-two integer coefficient vectors are accepted.");
    return data.Select(cell=>
    {
        var value=cell.GetString()??"";
        Need(BigInteger.TryParse(value,NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,out _),
            "Certificate contains a non-integer coefficient.");
        return BigInteger.Parse(value,CultureInfo.InvariantCulture);
    }).ToArray();
}
static string Verdict(bool complete,bool hole,bool timeout,bool providerFailed,
    BigInteger[]? calculated,BigInteger[]? rhs,BigInteger[]? certificate)
{
    if(!complete||hole||timeout||providerFailed||
        calculated is null||rhs is null||certificate is null)
        return "unproved";
    if(calculated.Length!=3||rhs.Length!=3||certificate.Length!=3||
       !calculated.SequenceEqual(rhs)||!calculated.SequenceEqual(certificate))
        return "invalid-certificate";
    return "verified-finite-identity";
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
using var priorDoc=Open(Prior);
using var previousDoc=Open(Previous);
using var fixtureDoc=Open(Fixture);
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
var prior=priorDoc.RootElement;
var previous=previousDoc.RootElement;
var fixture=fixtureDoc.RootElement;
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Get(backlog,"plan_version")==(target?"0.1.118":"0.1.117"),
    "Expected exact RS118 formal baseline or RS119 target.");
Need(Get(prior,"status")==
       "m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified" &&
     Get(prior,"certificate_verdict")=="verified-finite-identity" &&
     Get(prior,"certificate_kind")=="finite-integer-polynomial-coefficients" &&
     Get(prior,"certificate_domain")=="Z[x]" &&
     Get(prior,"certificate_coefficients")=="-1,0,1" &&
     prior.GetProperty("reference_degree").GetInt32()==2 &&
     prior.GetProperty("independent_oracle_groups").GetInt32()==2 &&
     Flag(prior,"altered_certificate_rejected") &&
     Flag(prior,"truncated_certificate_rejected") &&
     Flag(prior,"hole_unproved")&&Flag(prior,"timeout_unproved") &&
     Flag(prior,"provider_failure_unproved") &&
     Flag(prior,"t1_done")&&!Flag(prior,"t2_done") &&
     !Flag(prior,"c01_phase_done")&&!Flag(prior,"external_prover_adopted") &&
     !Flag(prior,"general_proof_kernel_certified") &&
     !Flag(prior,"source_free_consumer_qualified") &&
     !Flag(prior,"public_formal_api_delivered"),
     "Previously accepted T1 finite certificate has drifted.");
foreach(var (field,path) in new[]{
    ("fixture_sha256",Fixture),("validator_sha256",PriorChecker),
    ("decision_sha256",PriorDecision),("screen_sha256",PriorScreen)
})
    Need(Get(prior,field)==Hash(path),
        "Historical formal T1 source/document SHA drifted: "+path);
Need(Get(previous,"status")==
     "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished" &&
     Flag(previous,"m7_w01_work_package_done") &&
     !Flag(previous,"m7_release_done") &&
     !Flag(previous,"new_advanced_product_delivered"),
     "Completed W01 predecessor is not a public advanced product.");
Need(Get(fixture,"certificate_status")=="untrusted-until-local-replay" &&
     Get(fixture,"claim")=="(x-1)(x+1)=x^2-1" &&
     Get(fixture,"domain")=="Z[x]" &&
     Get(fixture,"proof_assistant")=="none" &&
     Get(fixture,"external_smt_solver")=="none" &&
     !Flag(fixture,"general_proof_kernel_qualified"),
     "The source certificate cannot be a purported external-kernel theorem.");

State(ById(backlog.GetProperty("releases"),"M6"),"done");
State(ById(backlog.GetProperty("releases"),"M7"),"in_progress");
State(ById(backlog.GetProperty("work_packages"),"M7-W01"),"done");
State(ById(backlog.GetProperty("work_packages"),"M7-W02"),"in_progress");
var phaseProduct=ById(backlog.GetProperty("chunks"),Phase);
State(phaseProduct,target?"done":"in_progress");
State(ById(backlog.GetProperty("chunks"),"M7-W02-C02"),"planned");
var t1=ById(phaseProduct.GetProperty("tasks"),"M7-W02-C01-T1");
var t2=ById(phaseProduct.GetProperty("tasks"),Task);
State(t1,"done");
State(t2,target?"done":"planned");
Need(Get(phaseProduct,"kind")=="spike"&&Get(phaseProduct,"size")=="M",
    "Formal proof candidate decision must remain size-M research.");
var m7=ById(native.GetProperty("milestones"),"M7");
var work=ById(m7.GetProperty("workPackages"),"M7-W02");
var phaseNative=ById(work.GetProperty("phases"),Phase);
State(m7,"in-progress","state");
State(ById(m7.GetProperty("workPackages"),"M7-W01"),"done","state");
State(work,"in-progress","state");
State(phaseNative,target?"done":"in-progress","state");
State(ById(work.GetProperty("phases"),"M7-W02-C02"),"not-ready","state");
var nativeT1=ById(phaseNative.GetProperty("tasks"),"M7-W02-C01-T1");
var nativeT2=ById(phaseNative.GetProperty("tasks"),Task);
State(nativeT1,"done","state");
State(nativeT2,target?"done":"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(ById(t1.GetProperty("subtasks"),"M7-W02-C01-T1-"+suffix),"done");
    State(ById(nativeT1.GetProperty("subtasks"),"M7-W02-C01-T1-"+suffix),"done","state");
    State(ById(t2.GetProperty("subtasks"),Task+"-"+suffix),target?"done":"planned");
    State(ById(nativeT2.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"not-ready","state");
}

var factors=fixture.GetProperty("factors");
Need(factors.GetArrayLength()==2,"Only two source-owned linear factors are admitted.");
var a=Integers(factors[0]);
var b=Integers(factors[1]);
var rhs=Integers(fixture.GetProperty("rhs_coefficients"));
var cert=Integers(fixture.GetProperty("certificate_product_coefficients"));
Need(a.Length==2&&b.Length==2,"Only one finite degree-two identity is admitted.");
BigInteger[] product=[
    a[0]*b[0],a[0]*b[1]+a[1]*b[0],a[1]*b[1]
];
Need(product.SequenceEqual(new BigInteger[]{-1,0,1}) &&
     Verdict(true,false,false,false,product,rhs,cert)=="verified-finite-identity",
    "Independent finite coefficient certificate was not verified.");
Console.WriteLine(Witness);
Need(Verdict(true,false,false,false,product,rhs,[-1,1,1])=="invalid-certificate" &&
     Verdict(true,false,false,false,product,rhs,[-1,0])=="invalid-certificate" &&
     Verdict(false,false,false,false,product,rhs,cert)=="unproved" &&
     Verdict(true,true,false,false,product,rhs,cert)=="unproved" &&
     Verdict(true,false,true,false,product,rhs,cert)=="unproved" &&
     Verdict(true,false,false,true,product,rhs,cert)=="unproved" &&
     Verdict(true,false,false,false,product,rhs,null)=="unproved",
    "Hole, failed provider or unverified incomplete certificate cannot be proved.");
Console.WriteLine(Refusal);

var decision=Read(Decision);
var screen=Read(Distribution);
foreach(var token in new[]{
    "## Question","## Observation","## Decision","DEFER",
    "verified-finite-identity","invalid-certificate","unproved",
    "Three mandatory","All three review triggers are necessary",
    "Lean","Rocq","SMT","trusted base","source-free","no public",
    "M7-W02-C02"
})
    Contains(decision,token,"Formal owner-gated T2 Q/O/D");
foreach(var token in new[]{
    "## Proof/certificate/derivation separation",
    "## Candidate toolchain / legality / platform matrix",
    "## Three re-entry triggers","Lean","Rocq","SMT",
    "If any trigger is missing, remain deferred",
    "source-free","AURA","invalid-certificate","unproved"
})
    Contains(screen,token,"Formal T2 provider deferral");
Need(screen.Split('\n').Count(line=>line.Contains("| **Defer** |",
    StringComparison.Ordinal))==3,
    "Exactly Lean/Rocq/SMT candidate rows must remain deferred.");

if(target)
{
    using var proofDoc=Open(Evidence);
    var proof=proofDoc.RootElement;
    Need(Get(proof,"status")==Status &&
        Get(proof,"disposition")=="defer-unqualified-formal-proof-providers-and-public-api" &&
        proof.GetProperty("review_trigger_count").GetInt32()==3 &&
        proof.GetProperty("deferred_external_provider_categories").GetInt32()==3 &&
        Get(proof,"certificate_verdict")=="verified-finite-identity" &&
        Flag(proof,"t1_done")&&Flag(proof,"t2_done") &&
        Flag(proof,"c01_phase_done") &&
        !Flag(proof,"m7_w02_work_package_done") &&
        !Flag(proof,"m7_release_done") &&
        !Flag(proof,"lean_kernel_qualified") &&
        !Flag(proof,"rocq_kernel_qualified") &&
        !Flag(proof,"smt_certificate_checker_qualified") &&
        !Flag(proof,"external_prover_adopted") &&
        !Flag(proof,"general_proof_kernel_certified") &&
        !Flag(proof,"public_formal_api_delivered") &&
        !Flag(proof,"source_free_consumer_qualified") &&
        !Flag(proof,"automatic_provider_cutover") &&
        !Flag(proof,"publicly_published") &&
        Get(proof,"decision_sha256")==Hash(Decision) &&
        Get(proof,"distribution_sha256")==Hash(Distribution) &&
        Get(proof,"boundary_validator_sha256")==Hash(
            "docs/planning/ValidateM7FormalBoundary.cs"),
        "Completed formal T2 source or deferred provider gate differs.");
}
Console.WriteLine(Marker);
return 0;
