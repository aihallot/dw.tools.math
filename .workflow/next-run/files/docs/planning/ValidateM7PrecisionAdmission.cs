#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Decision="docs/planning/decisions/m7-w01-c01.md";
const string Screen="docs/planning/decisions/m7-w01-c01-provider-screen.md";
const string Evidence="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string Marker="dw.tools.math/m7-w01-c01-t1-independent-precision-screen/1.2 qualified; cancellation=1/0; tenth=enclosed; external-provider=false";

static void Need(bool truth,string reason)
{
    if(!truth) throw new InvalidOperationException("Independent M7 precision research: "+reason);
}
static string ReadText(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) ||
         relative==Native,"Read scope exceeds explicitly allowed Math planning authorities.");
    Need(!Path.IsPathRooted(relative) && !relative.Contains("..",StringComparison.Ordinal) &&
         !relative.Contains('\\'),"Invalid local reference path: "+relative);
    return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar)));
}
static JsonElement Find(JsonElement collection,string id)=>
    collection.EnumerateArray().Single(item=>item.GetProperty("id").GetString()==id);
static string State(JsonElement obj,string property="status")=>
    obj.GetProperty(property).GetString() ??
        throw new InvalidOperationException("Missing state string: "+property);
static void RequireState(JsonElement obj,string expected,string property="status")=>
    Need(State(obj,property)==expected,
        "Unexpected "+property+" for "+obj.GetProperty("id").GetString()+": "+State(obj,property));
static (BigInteger Numerator,BigInteger Denominator) ExactBinary(double value)
{
    Need(double.IsFinite(value),"Only finite IEEE 754 reference values are admitted.");
    ulong bits=unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
    int exponentBits=(int)((bits>>52)&0x7FFUL);
    ulong fraction=bits&((1UL<<52)-1UL);
    BigInteger mantissa=exponentBits==0
        ? new BigInteger(fraction)
        : new BigInteger(fraction|(1UL<<52));
    int shift=exponentBits==0?-1074:exponentBits-1075;
    BigInteger numerator=shift>=0?(mantissa<<shift):mantissa;
    BigInteger denominator=shift>=0?BigInteger.One:BigInteger.One<<(-shift);
    if((bits>>63)!=0)numerator=-numerator;
    return (numerator,denominator);
}

Need(args.Length==0 || (args.Length==1 && args[0]=="--check"),
    "Only the read-only --check mode is supported.");
var decision=ReadText(Decision);
var screen=ReadText(Screen);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "BigInteger","1/10","10000000000000000","T2",
    "no product capability","measurement","provider"})
    Need(decision.Contains(term,StringComparison.OrdinalIgnoreCase),
        "Required Q/O/D scientific boundary missing: "+term);
foreach(var term in new[]{"## Candidate strategy screen","ExactRational",
    "BigInteger","decimal","managed","native","license","not admitted",
    "source-free consumer","review trigger"})
    Need(screen.Contains(term,StringComparison.OrdinalIgnoreCase),
        "Provider screening/budget boundary missing: "+term);

using var backlogDoc=JsonDocument.Parse(ReadText(Backlog));
using var planDoc=JsonDocument.Parse(ReadText(Native));
var backlog=backlogDoc.RootElement;
var native=planDoc.RootElement;
var target=File.Exists(Path.Combine(Directory.GetCurrentDirectory(),
    Evidence.Replace('/',Path.DirectorySeparatorChar)));
Need(backlog.GetProperty("plan_version").GetString()==
    (target?"0.1.105":"0.1.104"),"Unexpected M7 precision baseline/target version.");
RequireState(Find(backlog.GetProperty("releases"),"M6"),"done");
RequireState(Find(native.GetProperty("milestones"),"M6"),"done","state");
var productM7=Find(backlog.GetProperty("releases"),"M7");
var productW01=Find(backlog.GetProperty("work_packages"),"M7-W01");
var productC01=Find(backlog.GetProperty("chunks"),"M7-W01-C01");
var planM7=Find(native.GetProperty("milestones"),"M7");
var planW01=Find(planM7.GetProperty("workPackages"),"M7-W01");
var planC01=Find(planW01.GetProperty("phases"),"M7-W01-C01");
RequireState(productM7,target?"in_progress":"planned");
RequireState(productW01,target?"in_progress":"planned");
RequireState(productC01,target?"in_progress":"planned");
RequireState(planM7,target?"in-progress":"not-ready","state");
RequireState(planW01,target?"in-progress":"not-ready","state");
RequireState(planC01,target?"in-progress":"not-ready","state");
var t1=Find(productC01.GetProperty("tasks"),"M7-W01-C01-T1");
var t2=Find(productC01.GetProperty("tasks"),"M7-W01-C01-T2");
var nativeT1=Find(planC01.GetProperty("tasks"),"M7-W01-C01-T1");
var nativeT2=Find(planC01.GetProperty("tasks"),"M7-W01-C01-T2");
RequireState(t1,target?"done":"planned");
RequireState(t2,"planned");
RequireState(nativeT1,target?"done":"not-ready","state");
RequireState(nativeT2,"not-ready","state");
foreach(var id in new[]{"M7-W01-C01-T1-A","M7-W01-C01-T1-B","M7-W01-C01-T1-C"})
{
    RequireState(Find(t1.GetProperty("subtasks"),id),target?"done":"planned");
    RequireState(Find(nativeT1.GetProperty("subtasks"),id),target?"done":"not-ready","state");
}
foreach(var id in new[]{"M7-W01-C01-T2-A","M7-W01-C01-T2-B","M7-W01-C01-T2-C"})
{
    RequireState(Find(t2.GetProperty("subtasks"),id),"planned");
    RequireState(Find(nativeT2.GetProperty("subtasks"),id),"not-ready","state");
}

// Exact independent integer oracle, not a repetition of Math product logic.
var large=double.Parse("10000000000000000",CultureInfo.InvariantCulture);
var observedBinary64=(large+1.0)-large;
var independentExact=(BigInteger.Parse("10000000000000000",CultureInfo.InvariantCulture)
    +BigInteger.One)-BigInteger.Parse("10000000000000000",CultureInfo.InvariantCulture);
var binarySpacing=double.BitIncrement(large)-large;
Need(independentExact==BigInteger.One && observedBinary64==0.0 &&
     binarySpacing==2.0,"Cancellation, exact integer reference or binary64 spacing changed.");
Console.WriteLine("M7-PRECISION-ORACLE:CANCELLATION:EXACT=1:BINARY64=0:ULP=2");

// Exact binary-number decoding and integer cross multiplication establish
// inclusion of one rational witness; they DO NOT implement interval arithmetic.
const double upper=0.1;
var lower=double.BitDecrement(upper);
var upperRatio=ExactBinary(upper);
var lowerRatio=ExactBinary(lower);
var knownUpperNumerator=BigInteger.Parse("3602879701896397",CultureInfo.InvariantCulture);
var knownUpperDenominator=BigInteger.Parse("36028797018963968",CultureInfo.InvariantCulture);
Need(upperRatio.Numerator*knownUpperDenominator==
         knownUpperNumerator*upperRatio.Denominator,
    "Decoded binary64 0.1 differs from independent rational reference.");
Need(lowerRatio.Numerator*10<lowerRatio.Denominator &&
     upperRatio.Numerator*10>upperRatio.Denominator &&
     lower<upper,
    "Adjacent binary64 values fail to enclose exact decimal rational 1/10.");
Console.WriteLine("M7-PRECISION-ORACLE:DECIMAL-TENTH:ADJACENT-BINARY64-ENCLOSURE=TRUE");

if(target)
{
    using var evidenceDoc=JsonDocument.Parse(ReadText(Evidence));
    var e=evidenceDoc.RootElement;
    Need(e.GetProperty("status").GetString()==
         "m7-c01-t1-bcl-exact-precision-interval-oracles-provider-screen-qualified" &&
         e.GetProperty("t1_done").GetBoolean() &&
         !e.GetProperty("t2_done").GetBoolean() &&
         !e.GetProperty("external_provider_adopted").GetBoolean() &&
         !e.GetProperty("public_precision_api_delivered").GetBoolean(),
        "Target evidence would advertise an unqualified precision provider.");
}
Console.WriteLine(Marker);
