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
const string Previous="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Fixture="docs/planning/fixtures/m7-polynomial-identity-certificate.json";
const string Decision="docs/planning/decisions/m7-w02-c01-t1.md";
const string Screen="docs/distribution/m7-formal-provider-screen.md";
const string Evidence="docs/planning/evidence/M7-W02-C01-T1-qualified.json";
const string Status="m7-w02-c01-t1-exact-polynomial-certificate-replayed-provider-screen-qualified";
const string Marker="dw.tools.math/m7-w02-c01-t1-independent-certificate/1.2 qualified; z-polynomial=-1,0,1; altered=invalid; hole=unproved; timeout=unproved; provider=none";
const string ExactMarker="M7-FORMAL-CERTIFICATE:Z-POLYNOMIAL:PRODUCT=-1,0,1;VERDICT=verified-finite-identity";
const string RefusalMarker="M7-FORMAL-CERTIFICATE:NEGATIVE:ALTERED=invalid-certificate;TRUNCATED=invalid-certificate;HOLE=unproved;TIMEOUT=unproved;PROVIDER-FAILURE=unproved";
const string Phase="M7-W02-C01";
const string Task="M7-W02-C01-T1";

static void Need(bool condition,string reason)
{
    if(!condition)throw new InvalidOperationException("Independent finite certificate: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal)||relative==Native,
        "Local read-only Math evidence and planning paths only.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\'),"Unsafe local reference "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static JsonDocument Open(string path)=>JsonDocument.Parse(File.ReadAllText(Local(path)));
static string Read(string path)=>File.ReadAllText(Local(path));
static string Hash(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static JsonElement ById(JsonElement list,string id)=>
    list.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static string Get(JsonElement obj,string field)=>
    obj.GetProperty(field).GetString() ??
        throw new InvalidOperationException("Missing text: "+field);
static bool Flag(JsonElement obj,string field)=>obj.GetProperty(field).GetBoolean();
static void State(JsonElement obj,string expected,string field="status")=>
    Need(Get(obj,field)==expected,"Unexpected "+field+" for "+Get(obj,"id"));
static void Contains(string text,string token,string where)=>
    Need(text.Contains(token,StringComparison.OrdinalIgnoreCase),
        where+" is missing: "+token);
static BigInteger[] Coefficients(JsonElement array)
{
    Need(array.ValueKind==JsonValueKind.Array,"Coefficient vector must be an array.");
    var cells=array.EnumerateArray().ToArray();
    Need(cells.Length>0&&cells.Length<=3,"This bounded fixture only admits up to three terms.");
    var output=new BigInteger[cells.Length];
    for(var i=0;i<cells.Length;i++)
    {
        var literal=cells[i].GetString();
        Need(!string.IsNullOrWhiteSpace(literal)&&
             BigInteger.TryParse(literal,NumberStyles.AllowLeadingSign,
                 CultureInfo.InvariantCulture,out var integer)&&
             integer.ToString(CultureInfo.InvariantCulture)==literal,
            "Coefficient must be a canonical signed base-10 integer.");
        output[i]=integer;
    }
    return output;
}
static BigInteger[] Multiply(BigInteger[] a,BigInteger[] b)
{
    Need(a.Length==2&&b.Length==2,"Only two degree-one factors are supported.");
    var answer=new BigInteger[3];
    for(var i=0;i<2;i++)
        for(var j=0;j<2;j++)
            answer[i+j]+=a[i]*b[j];
    return answer;
}
// One fixed-size certificate replay, NOT a general theorem prover or proof kernel.
static string Verdict(BigInteger[]? a,BigInteger[]? b,BigInteger[]? rhs,
    BigInteger[]? certificate,string claimState)
{
    if(claimState!="complete"||a is null||b is null||
        rhs is null||certificate is null)return "unproved";
    if(a.Length!=2||b.Length!=2||rhs.Length!=3||certificate.Length!=3)
        return "invalid-certificate";
    var exact=Multiply(a,b);
    return exact.SequenceEqual(certificate)&&exact.SequenceEqual(rhs)
        ?"verified-finite-identity":"invalid-certificate";
}
static BigInteger Evaluate(BigInteger[] coefficients,BigInteger x)
{
    BigInteger value=BigInteger.Zero;
    for(var i=coefficients.Length-1;i>=0;i--)
        value=(value*x)+coefficients[i];
    return value;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check may be invoked.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var previousDoc=Open(Previous);
using var fixtureDoc=Open(Fixture);
var b=backlogDoc.RootElement;
var n=nativeDoc.RootElement;
var previous=previousDoc.RootElement;
var fixture=fixtureDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Get(b,"plan_version")==(target?"0.1.117":"0.1.116"),
    "Incorrect exact RS117 baseline or RS118 target version.");
Need(Get(previous,"status")==
       "m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished" &&
    Flag(previous,"m7_w01_work_package_done") &&
    !Flag(previous,"m7_w02_work_package_done") &&
    !Flag(previous,"m7_release_done") &&
    !Flag(previous,"new_advanced_product_delivered") &&
    previous.GetProperty("native_nodes_retained").GetInt32()==541,
    "RS117 predecessor must remain a completed unpublished research portfolio.");
Need(Get(previous,"validator_sha256")==Hash(
        "docs/planning/ValidateM7AdvancedWorkPackage.cs") &&
     Get(previous,"decision_sha256")==Hash(
        "docs/planning/decisions/m7-w01-research-closure.md") &&
     Get(previous,"distribution_sha256")==Hash(
        "docs/distribution/m7-w01-advanced-research-matrix.md"),
     "RS117 exact immutable sources have changed.");
State(ById(b.GetProperty("releases"),"M7"),"in_progress");
State(ById(b.GetProperty("work_packages"),"M7-W01"),"done");
State(ById(b.GetProperty("work_packages"),"M7-W02"),
    target?"in_progress":"planned");
var c01=ById(b.GetProperty("chunks"),Phase);
var c02=ById(b.GetProperty("chunks"),"M7-W02-C02");
State(c01,target?"in_progress":"planned");
State(c02,"planned");
Need(Get(c01,"kind")=="spike"&&Get(c01,"size")=="M",
    "Original accepted formal interoperability chunk must remain size-M spike.");
var t1=ById(c01.GetProperty("tasks"),Task);
var t2=ById(c01.GetProperty("tasks"),"M7-W02-C01-T2");
State(t1,target?"done":"planned");
State(t2,"planned");
var m7=ById(n.GetProperty("milestones"),"M7");
var w01=ById(m7.GetProperty("workPackages"),"M7-W01");
var w02=ById(m7.GetProperty("workPackages"),"M7-W02");
State(m7,"in-progress","state");
State(w01,"done","state");
State(w02,target?"in-progress":"not-ready","state");
var phase=ById(w02.GetProperty("phases"),Phase);
State(phase,target?"in-progress":"not-ready","state");
State(ById(w02.GetProperty("phases"),"M7-W02-C02"),"not-ready","state");
var nativeT1=ById(phase.GetProperty("tasks"),Task);
var nativeT2=ById(phase.GetProperty("tasks"),"M7-W02-C01-T2");
State(nativeT1,target?"done":"not-ready","state");
State(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var ending=((char)('A'+i)).ToString();
    State(ById(t1.GetProperty("subtasks"),Task+"-"+ending),
        target?"done":"planned");
    State(ById(nativeT1.GetProperty("subtasks"),Task+"-"+ending),
        target?"done":"not-ready","state");
    State(ById(t2.GetProperty("subtasks"),"M7-W02-C01-T2-"+ending),"planned");
    State(ById(nativeT2.GetProperty("subtasks"),"M7-W02-C01-T2-"+ending),
        "not-ready","state");
}

Need(fixture.GetProperty("schema_version").GetInt32()==1 &&
     Get(fixture,"id")=="M7-W02-C01-T1-polynomial-identity-certificate" &&
     Get(fixture,"certificate_kind")=="finite-integer-polynomial-coefficients" &&
     Get(fixture,"claim")=="(x-1)(x+1)=x^2-1" &&
     Get(fixture,"domain")=="Z[x]" &&
     Get(fixture,"coefficient_order")=="ascending-degree" &&
     Get(fixture,"verification_requested")=="exact-coefficient-convolution" &&
     Get(fixture,"certificate_status")=="untrusted-until-local-replay" &&
     Get(fixture,"proof_assistant")=="none" &&
     Get(fixture,"external_smt_solver")=="none" &&
     !Flag(fixture,"public_proof_api_delivered") &&
     !Flag(fixture,"general_proof_kernel_qualified") &&
     !Flag(fixture,"source_free_consumer_qualified"),
    "The certificate must be exact, source-owned, untrusted until replay and provider-free.");

var factors=fixture.GetProperty("factors");
Need(factors.GetArrayLength()==2,
    "Exactly two explicit linear polynomial factors are declared.");
var first=Coefficients(factors[0]);
var second=Coefficients(factors[1]);
var rhs=Coefficients(fixture.GetProperty("rhs_coefficients"));
var certificate=Coefficients(fixture.GetProperty("certificate_product_coefficients"));
Need(first.SequenceEqual(new BigInteger[]{-1,1}) &&
     second.SequenceEqual(new BigInteger[]{1,1}) &&
     rhs.SequenceEqual(new BigInteger[]{-1,0,1}) &&
     certificate.SequenceEqual(new BigInteger[]{-1,0,1}) &&
     Verdict(first,second,rhs,certificate,"complete")=="verified-finite-identity",
     "Independent BCL replay did not match the fixed Z[x] polynomial certificate.");
var product=Multiply(first,second);
foreach(var x in new BigInteger[]{-2,0,2})
    Need(Evaluate(first,x)*Evaluate(second,x)==Evaluate(rhs,x),
        "Supplementary sample sanity diverged; samples do not replace the certificate.");
Console.WriteLine(ExactMarker);

// Falsification: wrong bytes/status never promote to a formal proof.
BigInteger[] altered=[-1,1,1];
BigInteger[] truncated=[-1,0];
Need(Verdict(first,second,rhs,altered,"complete")=="invalid-certificate" &&
     Verdict(first,second,rhs,truncated,"complete")=="invalid-certificate" &&
     Verdict(first,second,rhs,null,"complete")=="unproved" &&
     Verdict(first,second,rhs,certificate,"hole")=="unproved" &&
     Verdict(first,second,rhs,certificate,"timeout")=="unproved" &&
     Verdict(first,second,rhs,certificate,"provider-failure")=="unproved",
     "An invalid, partial, absent, timed-out or failed proof must never be proved.");
Console.WriteLine(RefusalMarker);
Need(product.SequenceEqual(certificate),"The replayed integer convolution must equal the certificate.");

var decision=Read(Decision);
var screen=Read(Screen);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "Z[x]","BigInteger","verified","hole","timeout",
    "Lean","Rocq","SMT","No new","source-free"})
    Contains(decision,term,"T1 formal Q/O/D decision");
foreach(var term in new[]{"## Machine-readable reference and falsifiable outcomes",
    "## Candidate formal technologies and provenance gate",
    "## Three admission triggers","Lean","Rocq","SMT",
    "unproved","source-free","trusted base"})
    Contains(screen,term,"T1 formal owner/provider screen");

if(target)
{
    using var eDoc=Open(Evidence);
    var e=eDoc.RootElement;
    Need(Get(e,"status")==Status &&
         Get(e,"certificate_verdict")=="verified-finite-identity" &&
         Get(e,"certificate_kind")=="finite-integer-polynomial-coefficients" &&
         e.GetProperty("reference_degree").GetInt32()==2 &&
         e.GetProperty("reference_coefficient_count").GetInt32()==3 &&
         e.GetProperty("independent_oracle_groups").GetInt32()==2 &&
         Flag(e,"altered_certificate_rejected") &&
         Flag(e,"hole_unproved")&&Flag(e,"timeout_unproved") &&
         Flag(e,"provider_failure_unproved") &&
         Flag(e,"t1_done") && !Flag(e,"t2_done") &&
         !Flag(e,"c01_phase_done")&&!Flag(e,"m7_w02_work_package_done") &&
         !Flag(e,"m7_release_done")&&!Flag(e,"external_prover_adopted") &&
         !Flag(e,"public_formal_api_delivered") &&
         !Flag(e,"general_proof_kernel_certified") &&
         Get(e,"fixture_sha256")==Hash(Fixture) &&
         Get(e,"decision_sha256")==Hash(Decision) &&
         Get(e,"screen_sha256")==Hash(Screen) &&
         Get(e,"validator_sha256")==Hash("docs/planning/ValidateM7FormalCertificate.cs"),
         "Completed formal T1 receipt is not the exact independently qualified target.");
}
Console.WriteLine(Marker);
return 0;
