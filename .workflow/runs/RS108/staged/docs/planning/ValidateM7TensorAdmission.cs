#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Decision="docs/planning/decisions/m7-w01-c02.md";
const string Screen="docs/planning/decisions/m7-w01-c02-provider-screen.md";
const string Evidence="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string Prior="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string Marker="dw.tools.math/m7-w01-c02-t1-independent-tensor-screen/1.2 qualified; dense=2x2x2; sum=36; mismatch-refused=true; cap=64; provider=none";
const long MaxElements=64;
const long MaxBytes=512;

static void Need(bool yes,string why)
{
    if(!yes)throw new InvalidOperationException("Independent M7 tensor research: "+why);
}
static string PathFor(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||path==Native,
        "Read-only research checker accepts local planning authorities only.");
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
        !path.Contains('\\'),"Unsafe research reference path: "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(PathFor(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static JsonElement ById(JsonElement items,string id)=>
    items.EnumerateArray().Single(value=>value.GetProperty("id").GetString()==id);
static void State(JsonElement node,string expected,string property="status")=>
    Need(node.GetProperty(property).GetString()==expected,
        "State mismatch "+node.GetProperty("id").GetString()+" => "+expected);
static bool TryElementCount(int[] shape,long maxElements,out long elements)
{
    elements=0;
    if(shape.Length==0 || shape.Length>3 || maxElements<1)return false;
    long count=1;
    foreach(var extent in shape)
    {
        if(extent<=0 || count>maxElements/extent)return false;
        count*=extent;
    }
    elements=count;
    return true;
}
static bool SameShape(int[] a,int[] b)=>
    a.Length==b.Length && a.SequenceEqual(b);
static long At(long[] data,int[] shape,int i,int j,int k)
{
    Need(shape.Length==3,"Only an explicit rank-three reference is admitted.");
    Need(TryElementCount(shape,MaxElements,out var elements)&&
         data.LongLength==elements,"Reference tensor data/shape mismatch.");
    Need(i>=0&&i<shape[0]&&j>=0&&j<shape[1]&&k>=0&&k<shape[2],
        "Index exceeds explicit shape.");
    var position=checked((i*shape[1]+j)*shape[2]+k);
    return data[position];
}
static void Contains(string body,string token,string label)=>
    Need(body.Contains(token,StringComparison.OrdinalIgnoreCase),
        label+" lacks: "+token);

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
var decision=Read(Decision);
var screen=Read(Screen);
foreach(var x in new[]{"## Question","## Observation","## Decision","[2,2,2]",
    "sum = 36","[1,0,1] = 6","64-element","512-byte",
    "T2","no product","consumer","no implicit"})
    Contains(decision,x,"Tensor research decision");
foreach(var x in new[]{"## Independent shape and resource oracle",
    "## Candidate approach and ownership matrix","[4,4,5]","[2,4]",
    "512 bytes","source-free","Unqualified","AURA","trigger"})
    Contains(screen,x,"Tensor provider/owner screen");

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var target=File.Exists(PathFor(Evidence));
Need(backlog.GetProperty("plan_version").GetString()==
    (target?"0.1.107":"0.1.106"),
    "Unexpected research baseline or target plan version.");
State(ById(backlog.GetProperty("releases"),"M6"),"done");
State(ById(native.GetProperty("milestones"),"M6"),"done","state");
State(ById(backlog.GetProperty("releases"),"M7"),"in_progress");
State(ById(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
var c01=ById(backlog.GetProperty("chunks"),"M7-W01-C01");
var c02=ById(backlog.GetProperty("chunks"),"M7-W01-C02");
State(c01,"done");
State(c02,target?"in_progress":"planned");
Need(c02.GetProperty("kind").GetString()=="spike",
    "Completed research cannot be silently reclassified as a tensor implementation.");
var productT1=ById(c02.GetProperty("tasks"),"M7-W01-C02-T1");
var productT2=ById(c02.GetProperty("tasks"),"M7-W01-C02-T2");
State(productT1,target?"done":"planned");
State(productT2,"planned");
var m7=ById(native.GetProperty("milestones"),"M7");
var wp=ById(m7.GetProperty("workPackages"),"M7-W01");
var p01=ById(wp.GetProperty("phases"),"M7-W01-C01");
var p02=ById(wp.GetProperty("phases"),"M7-W01-C02");
State(m7,"in-progress","state");
State(wp,"in-progress","state");
State(p01,"done","state");
State(p02,target?"in-progress":"not-ready","state");
var nativeT1=ById(p02.GetProperty("tasks"),"M7-W01-C02-T1");
var nativeT2=ById(p02.GetProperty("tasks"),"M7-W01-C02-T2");
State(nativeT1,target?"done":"not-ready","state");
State(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var t1Id="M7-W01-C02-T1-"+(char)('A'+i);
    var t2Id="M7-W01-C02-T2-"+(char)('A'+i);
    State(ById(productT1.GetProperty("subtasks"),t1Id),target?"done":"planned");
    State(ById(nativeT1.GetProperty("subtasks"),t1Id),
        target?"done":"not-ready","state");
    State(ById(productT2.GetProperty("subtasks"),t2Id),"planned");
    State(ById(nativeT2.GetProperty("subtasks"),t2Id),"not-ready","state");
}
State(ById(wp.GetProperty("phases"),"M7-W01-C03"),"not-ready","state");
Need(prior.GetProperty("status").GetString()==
        "m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified" &&
     prior.GetProperty("m7_w01_c01_parent_done").GetBoolean() &&
     !prior.GetProperty("m7_w01_work_package_done").GetBoolean() &&
     !prior.GetProperty("external_provider_adopted").GetBoolean() &&
     !prior.GetProperty("public_precision_api_delivered").GetBoolean(),
     "The admitted C01 precision deferral no longer supports this research gate.");

// Reference-only dense rank-three sample: no implementation of a product tensor API.
int[] shape=[2,2,2];
long[] data=[1,2,3,4,5,6,7,8];
Need(TryElementCount(shape,MaxElements,out var n)&&n==8,
    "Dense shape must contain exactly eight bounded scalar cells.");
Need(n*sizeof(long)==64 && n*sizeof(long)<=MaxBytes,
    "Declared byte budget must be checked without a hidden copy.");
Need(data.Sum()==36 && At(data,shape,1,0,1)==6 &&
     At(data,shape,0,0,0)==1 && At(data,shape,1,1,1)==8,
    "Independent row-major tensor layout, sum or indexing oracle failed.");
Console.WriteLine("M7-TENSOR-ORACLE:DENSE:SHAPE=2x2x2:SUM=36:INDEX[1,0,1]=6");

int[] incompatible=[2,4];
int[] atBudget=[4,4,4];
int[] aboveBudget=[4,4,5];
int[] emptyExtent=[2,0,2];
int[] huge=[int.MaxValue,int.MaxValue,int.MaxValue];
Need(TryElementCount(incompatible,MaxElements,out var eight)&&eight==8 &&
     !SameShape(shape,incompatible),"Same-sized but different tensor shapes cannot match.");
Need(TryElementCount(atBudget,MaxElements,out var exactly64)&&exactly64==64 &&
     exactly64*sizeof(long)==MaxBytes,
    "Maximum allowed count and bytes must be admitted exactly.");
Need(!TryElementCount(aboveBudget,MaxElements,out _) &&
     !TryElementCount(emptyExtent,MaxElements,out _) &&
     !TryElementCount(Array.Empty<int>(),MaxElements,out _) &&
     !TryElementCount(huge,MaxElements,out _),
    "Invalid rank, zero extent, 80-cell or huge request must refuse before allocation.");
Console.WriteLine("M7-TENSOR-ORACLE:REFUSALS:SHAPE-MISMATCH=true;BUDGET-OVER=64;ZERO-EXTENT=true");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var e=evidenceDoc.RootElement;
    Need(e.GetProperty("status").GetString()==
         "m7-c02-t1-dense-shape-oracles-and-provider-screen-qualified" &&
         e.GetProperty("independent_oracle_groups").GetInt32()==2 &&
         e.GetProperty("reference_elements").GetInt32()==8 &&
         e.GetProperty("reference_sum").GetInt32()==36 &&
         e.GetProperty("reference_index_value").GetInt32()==6 &&
         e.GetProperty("research_element_budget").GetInt32()==64 &&
         e.GetProperty("research_byte_budget").GetInt32()==512 &&
         e.GetProperty("t1_done").GetBoolean() &&
         !e.GetProperty("t2_done").GetBoolean() &&
         !e.GetProperty("advanced_tensor_api_delivered").GetBoolean() &&
         !e.GetProperty("external_tensor_provider_adopted").GetBoolean() &&
         !e.GetProperty("m7_w01_c02_parent_done").GetBoolean(),
         "Target evidence must retain the exact research-only tensor disposition.");
}
Console.WriteLine(Marker);
