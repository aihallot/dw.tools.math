#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string PriorReport=".aura/workflow/reports/RS131/attempt-001.json";
const string PriorReceipt="docs/discovery/qualification/RS131-portable-protocol-qualified.json";
const string Candidate="docs/discovery/agent-surface-candidates.json";
const string Proposal="docs/planning/proposals/math-agent-discovery-work-package.json";
const string Decision="docs/discovery/qualification/RS132-clr-seed-boundary.md";
const string Matrix="docs/discovery/qualification/RS132-clr-seed-matrix.md";
const string Validator="docs/discovery/qualification/ValidateClrSeedReflection.cs";
const string Evidence="docs/discovery/qualification/RS132-clr-seed-qualified.json";
const string Status="rs132-four-candidate-static-clr-signatures-reflected-no-agent-skill";
const string Counts="MATH-RS132:STATIC-CANDIDATES=4;COMPILED-SIGNATURES=4;RS131-CASES=13;NATIVE=541";
const string Scope="MATH-RS132:REFLECTION=SOURCE-COMPILED;EXAMPLES=false;SKILL=false;BROKER=false;EXTERNAL=false";
const string Marker="dw.tools.math/rs132-clr-seed/1.0 qualified; signatures=4; examples=false; broker=false";
const string Output="build/artifacts/bin/dw.tools.math.composition/Release/net10.0";
static void Need(bool x,string message)
{if(!x)throw new InvalidOperationException("Math RS132 CLR reflection: "+message);}
static string Local(string p)
{
    Need(!string.IsNullOrWhiteSpace(p)&&!Path.IsPathRooted(p)&&
        !p.Contains("..",StringComparison.Ordinal)&&!p.Contains(':')&&
        !p.Contains('\\'),"Unsafe source path "+p);
    return Path.Combine(Directory.GetCurrentDirectory(),
        p.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string p)=>File.ReadAllText(Local(p));
static JsonDocument Open(string p)=>JsonDocument.Parse(Read(p));
static string SHA(string p)=>Convert.ToHexString(
    SHA256.HashData(File.ReadAllBytes(Local(p)))).ToLowerInvariant();
static string Blob(string p)
{
    var b=Encoding.UTF8.GetBytes(Read(p).Replace("\r\n","\n",
        StringComparison.Ordinal));
    var h=Encoding.ASCII.GetBytes("blob "+b.Length+"\0");
    var data=new byte[h.Length+b.Length];
    Buffer.BlockCopy(h,0,data,0,h.Length);
    Buffer.BlockCopy(b,0,data,h.Length,b.Length);
    return Convert.ToHexString(SHA1.HashData(data)).ToLowerInvariant();
}
static string Text(JsonElement x,string k)=>x.GetProperty(k).GetString()??
    throw new InvalidOperationException("Missing string "+k);
static bool Flag(JsonElement x,string k)=>x.GetProperty(k).GetBoolean();
static int Count(JsonElement x)
{
    var n=1;
    foreach(var k in new[]{"workPackages","phases","tasks","subtasks"})
        if(x.TryGetProperty(k,out var children))
            n+=children.EnumerateArray().Sum(Count);
    return n;
}
static int NotDone(JsonElement x)
{
    var n=Text(x,"state")=="done"?0:1;
    foreach(var k in new[]{"workPackages","phases","tasks","subtasks"})
        if(x.TryGetProperty(k,out var children))
            n+=children.EnumerateArray().Sum(NotDone);
    return n;
}
static string TypeName(Type t)
{
    if(t.IsArray)return TypeName(t.GetElementType()!)+"[]";
    if(t.IsByRef)return TypeName(t.GetElementType()!)+"&";
    if(t.IsGenericType)
    {
        var s=t.GetGenericTypeDefinition().FullName??
            throw new InvalidOperationException("Unbound generic type");
        var pos=s.IndexOf('`');
        if(pos>=0)s=s[..pos];
        return s+"<"+string.Join(",",t.GetGenericArguments().Select(TypeName))+">";
    }
    return t.FullName??throw new InvalidOperationException("Unidentified CLR type");
}
static string Canonical(string id,string assembly,string type,
    string member,string[] parameters,string result)=>
    id+"|"+assembly+"|"+type+"::"+member+"("+
    string.Join(",",parameters)+")=>"+result+"|public-static";
static string Digest(string text)=>Convert.ToHexString(
    SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
static void Contains(string path,string text)
{Need(Read(path).Contains(text,StringComparison.Ordinal),
    path+" missing "+text);}

Need(args.Length==0||(args.Length==1&&
    (args[0]=="--check"||args[0]=="--reflect")),
    "Only --check and --reflect are read-only supported modes");
var reflect=args.Length==1&&args[0]=="--reflect";
using var backlogDoc=Open(Backlog);
using var planDoc=Open(Native);
using var reportDoc=Open(PriorReport);
using var receiptDoc=Open(PriorReceipt);
using var seedDoc=Open(Candidate);
using var proposalDoc=Open(Proposal);
var b=backlogDoc.RootElement;
var plan=planDoc.RootElement;
var report=reportDoc.RootElement;
var previous=receiptDoc.RootElement;
var seed=seedDoc.RootElement;
var proposal=proposalDoc.RootElement;
Need(Text(b,"plan_version")=="0.1.122"&&Text(b,"status")=="done"&&
     b.GetProperty("releases").GetArrayLength()==8&&
     b.GetProperty("work_packages").GetArrayLength()==17&&
     b.GetProperty("chunks").GetArrayLength()==58,
     "Accepted Math roadmap drifted");
Need(plan.GetProperty("milestones").GetArrayLength()==8&&
     plan.GetProperty("milestones").EnumerateArray().Sum(Count)==541&&
     plan.GetProperty("milestones").EnumerateArray().Sum(NotDone)==0,
     "Native DWF plan is not the 541-node completed baseline");
Need(Text(report,"runId")=="RS131"&&
     report.GetProperty("attempt").GetInt32()==1&&
     Text(report,"outcome")=="succeeded"&&!Flag(report,"correctionMode")&&
     Text(report.GetProperty("executor"),"toolVersion")=="0.1.59"&&
     Text(report.GetProperty("git"),"commitMessage")==
        "docs: qualify CSharp Discovery portable protocol evidence without skill"&&
     report.GetProperty("priorInvocationFailures").GetProperty("count").GetInt32()==0&&
     Flag(report.GetProperty("priorInvocationFailures"),"firstPassSucceeded"),
     "RS131 must be durably pushed first-pass");
Need(Text(previous,"status")==
        "rs131-math-csharp-portable-discovery-corpus-qualified-without-agent-skill"&&
     previous.GetProperty("total_cases").GetInt32()==13&&
     previous.GetProperty("accepted_cases").GetInt32()==2&&
     previous.GetProperty("rejected_cases").GetInt32()==11&&
     previous.GetProperty("candidate_static_operations").GetInt32()==4&&
     previous.GetProperty("native_nodes_retained").GetInt32()==541&&
     Flag(previous,"csharp_protocol_verifier_executed")&&
     Flag(previous,"csharp_portable_envelope_conformant")&&
     !Flag(previous,"selected_clr_bindings_reflected")&&
     !Flag(previous,"compiled_binding_examples")&&
     !Flag(previous,"generated_math_skill")&&
     !Flag(previous,"broker_ready")&&
     !Flag(previous,"external_requests_transmitted")&&
     !Flag(previous,"publicly_published"),
     "Prior CSharp corpus must not be relabelled as skill/binder");
foreach(var (key,path) in new[]{
    ("discovery_readme_sha256","docs/discovery/README.md"),
    ("decision_sha256","docs/planning/decisions/post-m7-discovery-portable-conformance.md"),
    ("validator_sha256","docs/discovery/qualification/ValidateDwfPortableProtocolGate.cs")
})Need(Text(previous,key)==SHA(path),"RS131 source SHA changed: "+path);
Need(Text(seed,"status")=="proposed-not-implemented-not-admitted"&&
     Text(seed,"protocol")=="dw.discovery/1.0.0-draft.1"&&
     Text(seed,"source_repo")=="aihallot/dw.tools.math"&&
     Text(proposal,"status")=="draft-not-merged-into-canonical-backlog-or-native-DWF",
     "Discovery candidates or proposed work were promoted prematurely");
Need(!Directory.Exists(Local(".agents/skills/dw-tools-math")),
     "No generated Math skill is authorized");
var methods=new (string Id,string Package,string Version,string Assembly,
    string Type,string Method,string[] Parameters,string Result,string Source,string Blob)[]
{
    ("math.quantity.exact-pipeline.evaluate","dw.tools.math.composition",
      "0.3.0-preview.1","dw.tools.math.composition",
      "Dw.Tools.Math.Composition.ExactQuantityPipeline","Evaluate",
      ["System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>"],
      "Dw.Tools.Math.Composition.ExactPipelineResult",
      "src/projects/dw.tools.math.composition/ExactQuantityPipeline.cs",
      "1d689d8dc66691de0c9a3b7f6e7c3751be68ec7d"),
    ("math.replay.try-evaluate","dw.tools.math.composition",
      "0.3.0-preview.1","dw.tools.math.composition",
      "Dw.Tools.Math.Composition.ExactReplayRunner","TryRun",
      ["System.Collections.Generic.IEnumerable<Dw.Tools.Math.Composition.ExactPipelineStep>",
       "Dw.Tools.Math.Composition.ExactReplayContext",
       "Dw.Tools.Math.Composition.ExactReplayCache",
       "System.Threading.CancellationToken"],
      "Dw.Tools.Math.Composition.ExactReplayAttempt",
      "src/projects/dw.tools.math.composition/ExactReplayContracts.cs",
      "ffffe063ffdb698776d1bed82d683e4bd635dd40"),
    ("math.ir.canonical.encode","dw.tools.math.ir",
      "0.3.0-preview.1","dw.tools.math.ir",
      "Dw.Tools.Math.Ir.IrCanonicalJsonCodec","Encode",
      ["Dw.Tools.Math.Ir.IrNode"],"System.String",
      "src/projects/dw.tools.math.ir/IrCanonicalJsonCodec.cs",
      "4e6e055d94480fb8ac449750b2e4f23eeeec09e9"),
    ("math.ir.hash.semantic","dw.tools.math.ir",
      "0.3.0-preview.1","dw.tools.math.ir",
      "Dw.Tools.Math.Ir.IrCanonicalHashes","SemanticStructuralSha256",
      ["Dw.Tools.Math.Ir.IrNode"],"System.String",
      "src/projects/dw.tools.math.ir/IrCanonicalHashes.cs",
      "8bbab3059acfad2ee6863a6fd98703798bac688d")
};
var candidates=seed.GetProperty("proposed_agent_callable_seed")
    .EnumerateArray().ToArray();
Need(candidates.Length==4&&seed.GetProperty("not_selected").GetArrayLength()>0&&
     seed.GetProperty("deferred_agent_operations").GetArrayLength()>0,
     "Four selected operations must exclude helpers and instance members");
for(var i=0;i<methods.Length;i++)
{
    var method=methods[i];
    var c=candidates[i];
    Need(Text(c,"capabilityId")==method.Id&&
         Text(c,"package")==method.Package&&
         Text(c,"packageVersion")==method.Version&&
         Text(c,"assembly")==method.Assembly&&
         Text(c,"type")==method.Type&&Text(c,"member")==method.Method&&
         Text(c,"resultType")==method.Result&&Text(c,"kind")=="public static"&&
         Text(c,"exposure")=="AgentCallable"&&!Flag(c,"brokerReady")&&
         Text(c,"source")==method.Source&&Text(c,"source_blob")==method.Blob&&
         c.GetProperty("parameterTypes").EnumerateArray()
            .Select(x=>x.GetString()??"")
            .SequenceEqual(method.Parameters,StringComparer.Ordinal)&&
         Blob(method.Source)==method.Blob,
         "Candidate source/signature/visibility pin differs: "+method.Id);
    var project=XDocument.Parse(Read("src/projects/"+method.Package+"/"+
        method.Package+".csproj"));
    Need(project.Descendants().Any(x=>x.Name.LocalName=="PackageId"&&
             x.Value==method.Package)&&
         project.Descendants().Any(x=>x.Name.LocalName=="Version"&&
             x.Value==method.Version),
         "Current product package/version mismatch: "+method.Package);
}
foreach(var (file,terms) in new[]{
    (Decision,new[]{"## Objective and authorization",
        "## Exact evidence boundary and candidate identities",
        "## Controlled verification","## Next gate","M3-W03"}),
    (Matrix,new[]{"## Four source-pinned selected members",
        "## Exclusions and science limits",
        "## Supported DWF continuation","No Python"})
})foreach(var token in terms)Contains(file,token);

if(reflect)
{
    var binaries=new Dictionary<string,string>(StringComparer.Ordinal)
    {
        ["dw.quantities"]=Local(Output+"/dw.quantities.dll"),
        ["dw.tools.math.ir"]=Local(Output+"/dw.tools.math.ir.dll"),
        ["dw.tools.math.composition"]=Local(Output+"/dw.tools.math.composition.dll")
    };
    foreach(var (id,file) in binaries)
        Need(File.Exists(file)&&new FileInfo(file).Length>0,
            "Release composition graph DLL missing: "+id);
    AssemblyLoadContext.Default.Resolving+=(ctx,name)=>
        binaries.TryGetValue(name.Name??"",out var resolvedPath)?
            ctx.LoadFromAssemblyPath(resolvedPath):null;
    var loaded=new Dictionary<string,Assembly>(StringComparer.Ordinal);
    foreach(var (id,file) in binaries)
        loaded[id]=AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
    foreach(var m in methods)
    {
        var type=loaded[m.Assembly].GetType(m.Type,false,false);
        Need(type is not null&&type.IsPublic,
            "CLR declaring type inaccessible: "+m.Type);
        var matches=type!.GetMethods(BindingFlags.DeclaredOnly|
            BindingFlags.Public|BindingFlags.Static)
            .Where(x=>x.Name==m.Method&&!x.IsGenericMethod&&
                x.GetParameters().Select(p=>TypeName(p.ParameterType))
                  .SequenceEqual(m.Parameters,StringComparer.Ordinal))
            .ToArray();
        Need(matches.Length==1&&matches[0].IsPublic&&
             matches[0].IsStatic&&TypeName(matches[0].ReturnType)==m.Result,
             "Exact reflected CLR method not uniquely bound: "+m.Id);
        Console.WriteLine("RS132-CLR|"+m.Id+"|"+
            Digest(Canonical(m.Id,m.Assembly,m.Type,m.Method,
                m.Parameters,m.Result))+"|"+m.Assembly);
    }
}
if(File.Exists(Local(Evidence)))
{
    using var targetDoc=Open(Evidence);
    var r=targetDoc.RootElement;
    Need(Text(r,"status")==Status&&
         r.GetProperty("observed_static_bindings").GetInt32()==4&&
         r.GetProperty("observed_bindings").GetArrayLength()==4&&
         r.GetProperty("prior_csharp_protocol_cases").GetInt32()==13&&
         r.GetProperty("native_nodes_retained").GetInt32()==541&&
         Flag(r,"compiled_assembly_reflection_executed")&&
         Flag(r,"all_selected_signatures_exact")&&
         !Flag(r,"compiled_invocation_examples")&&
         !Flag(r,"generated_math_skill")&&
         !Flag(r,"broker_ready")&&
         !Flag(r,"json_broker_adapter_qualified")&&
         !Flag(r,"math_decision_runtime_interoperability_certified")&&
         !Flag(r,"new_discovery_work_package_admitted")&&
         !Flag(r,"m0_m7_plan_mutated")&&
         !Flag(r,"publicly_published")&&
         !Flag(r,"external_requests_transmitted")&&
         !Flag(r,"external_approvals_received")&&
         Text(r,"decision_sha256")==SHA(Decision)&&
         Text(r,"matrix_sha256")==SHA(Matrix)&&
         Text(r,"validator_sha256")==SHA(Validator),
         "RS132 target does not establish only bounded CLR source reflection");
    var commandIds=new[]{"locked-solution-restore",
        "composition-release-build","four-static-clr-reflection"};
    var commands=r.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.Length==3,"Expected three local execution records");
    for(var i=0;i<commandIds.Length;i++)
        Need(Text(commands[i],"id")==commandIds[i]&&
             commands[i].GetProperty("exit_code").GetInt32()==0,
             "A required source build or reflection failed: "+commandIds[i]);
    var rows=r.GetProperty("observed_bindings").EnumerateArray().ToArray();
    for(var i=0;i<methods.Length;i++)
    {
        var m=methods[i];
        var expected=Canonical(m.Id,m.Assembly,m.Type,m.Method,
            m.Parameters,m.Result);
        Need(Text(rows[i],"capability_id")==m.Id&&
             Text(rows[i],"assembly")==m.Assembly&&
             Text(rows[i],"source_git_blob")==m.Blob&&
             Text(rows[i],"canonical_signature")==expected&&
             Text(rows[i],"signature_sha256")==Digest(expected)&&
             !Flag(rows[i],"broker_ready"),
             "Recorded CLR signature hash or source pin changed: "+m.Id);
    }
}
Console.WriteLine(Counts);
Console.WriteLine(Scope);
Console.WriteLine(Marker);
return 0;
