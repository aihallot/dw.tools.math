using System.Diagnostics;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;
var payload=PayloadContext.Create();
payload.Files.ReplaceFromStaged("staged/docs/planning/backlog.json","docs/planning/backlog.json");
payload.Files.ReplaceFromStaged("staged/docs/planning/ValidateExchangeContract.cs","docs/planning/ValidateExchangeContract.cs");
payload.Files.ReplaceFromStaged("staged/docs/planning/evidence/M0-W02-C03-exchange-contract.json","docs/planning/evidence/M0-W02-C03-exchange-contract.json");
payload.Files.ReplaceFromStaged("staged/docs/coordination/requests/MATH-XR-001-aura-baseline.json","docs/coordination/requests/MATH-XR-001-aura-baseline.json");
payload.Files.ReplaceFromStaged("staged/docs/coordination/requests/MATH-XR-001-aura.md","docs/coordination/requests/MATH-XR-001-aura.md");
payload.Files.ReplaceFromStaged("staged/docs/planning/decisions/m0-w02-c03.md","docs/planning/decisions/m0-w02-c03.md");
RunDotNet(payload.RepositoryRoot,"run","--file","docs/planning/ValidatePlan.cs","--","--write");
ConvergeNativeProgress(payload);
return payload.Complete();

static void ConvergeNativeProgress(PayloadContext payload)
{
    var ids=new[]{
      "M0","M0-W02","M0-W02-C03",
      "M0-W02-C03-T1","M0-W02-C03-T1-A","M0-W02-C03-T1-B","M0-W02-C03-T1-C",
      "M0-W02-C03-T2","M0-W02-C03-T2-A","M0-W02-C03-T2-B","M0-W02-C03-T2-C",
      "M1","M1-W01","M1-W01-C01","M1-W01-C01-T1","M1-W01-C01-T1-R"
    };
    var s=ReadStates(payload.RepositoryRoot,ids);
    var baseline=s["M0"]=="in-progress"&&s["M0-W02"]=="in-progress"&&s["M0-W02-C03"]=="ready"&&s["M0-W02-C03-T1"]=="ready"&&s["M0-W02-C03-T1-A"]=="ready"&&s["M0-W02-C03-T1-B"]=="not-ready"&&s["M0-W02-C03-T1-C"]=="not-ready"&&s["M0-W02-C03-T2"]=="not-ready"&&s["M0-W02-C03-T2-A"]=="not-ready"&&s["M0-W02-C03-T2-B"]=="not-ready"&&s["M0-W02-C03-T2-C"]=="not-ready"&&s["M1"]=="not-ready"&&s["M1-W01"]=="not-ready"&&s["M1-W01-C01"]=="not-ready"&&s["M1-W01-C01-T1"]=="not-ready"&&s["M1-W01-C01-T1-R"]=="not-ready";
    var target=s["M0"]=="done"&&s["M0-W02"]=="done"&&s["M0-W02-C03"]=="done"&&s["M0-W02-C03-T1"]=="done"&&s["M0-W02-C03-T1-A"]=="done"&&s["M0-W02-C03-T1-B"]=="done"&&s["M0-W02-C03-T1-C"]=="done"&&s["M0-W02-C03-T2"]=="done"&&s["M0-W02-C03-T2-A"]=="done"&&s["M0-W02-C03-T2-B"]=="done"&&s["M0-W02-C03-T2-C"]=="done"&&s["M1"]=="ready"&&s["M1-W01"]=="ready"&&s["M1-W01-C01"]=="ready"&&s["M1-W01-C01-T1"]=="ready"&&s["M1-W01-C01-T1-R"]=="ready";
    if(target){foreach(var id in ids)payload.ProjectPlan.RequireNodeState(id,s[id]);return;}
    if(!baseline)throw new InvalidOperationException("M0-W02-C03 is neither the declared RS006 baseline nor exact target.");
    payload.ProjectPlan.ActivateReadyContinuation("M0-W02-C03-T1-A");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C03-T1-A");
    CompleteSibling(payload,"M0-W02-C03-T1-B");CompleteSibling(payload,"M0-W02-C03-T1-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C03-T1");
    payload.ProjectPlan.TransitionNode("M0-W02-C03-T2","not-ready","ready");
    payload.ProjectPlan.TransitionNode("M0-W02-C03-T2-A","not-ready","ready");
    payload.ProjectPlan.ActivateReadyContinuation("M0-W02-C03-T2-A");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C03-T2-A");
    CompleteSibling(payload,"M0-W02-C03-T2-B");CompleteSibling(payload,"M0-W02-C03-T2-C");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C03-T2");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02-C03");
    payload.ProjectPlan.ConvergeNodeToDone("M0-W02");
    payload.ProjectPlan.ConvergeNodeToDone("M0");
    payload.ProjectPlan.TransitionNode("M1","not-ready","ready");
    payload.ProjectPlan.TransitionNode("M1-W01","not-ready","ready");
    payload.ProjectPlan.TransitionNode("M1-W01-C01","not-ready","ready");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1","not-ready","ready");
    payload.ProjectPlan.TransitionNode("M1-W01-C01-T1-R","not-ready","ready");
}
static void CompleteSibling(PayloadContext payload,string id){payload.ProjectPlan.TransitionNode(id,"not-ready","ready");payload.ProjectPlan.ActivateReadyContinuation(id);payload.ProjectPlan.ConvergeNodeToDone(id);}
static Dictionary<string,string> ReadStates(string root,IEnumerable<string> ids){var wanted=ids.ToHashSet(StringComparer.Ordinal);var result=new Dictionary<string,string>(StringComparer.Ordinal);var node=JsonNode.Parse(File.ReadAllText(Path.Combine(root,".aura","workflow","plan","project.json")))??throw new InvalidOperationException("Native DWF project plan is empty.");Visit(node,wanted,result);foreach(var id in wanted)if(!result.ContainsKey(id))throw new InvalidOperationException("Missing native node: "+id);return result;}
static void Visit(JsonNode? node,IReadOnlySet<string>wanted,IDictionary<string,string>result){if(node is JsonObject o){var id=o["id"]?.GetValue<string>();if(id is not null&&wanted.Contains(id))result[id]=o["state"]?.GetValue<string>()??throw new InvalidOperationException("Node has no state: "+id);foreach(var p in o)Visit(p.Value,wanted,result);return;}if(node is JsonArray a)foreach(var x in a)Visit(x,wanted,result);}
static void RunDotNet(string root,params string[] args){using var p=new Process{StartInfo=new ProcessStartInfo{FileName="dotnet",WorkingDirectory=root,UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true}};p.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";p.StartInfo.Environment["DOTNET_NOLOGO"]="1";foreach(var a in args)p.StartInfo.ArgumentList.Add(a);if(!p.Start())throw new InvalidOperationException("Failed to start dotnet planning generation.");var so=p.StandardOutput.ReadToEndAsync();var se=p.StandardError.ReadToEndAsync();if(!p.WaitForExit(180000)){p.Kill(entireProcessTree:true);throw new TimeoutException("dotnet planning generation exceeded 180 seconds.");}var stdout=so.GetAwaiter().GetResult();var stderr=se.GetAwaiter().GetResult();if(!string.IsNullOrWhiteSpace(stdout))Console.Write(stdout);if(!string.IsNullOrWhiteSpace(stderr))Console.Error.Write(stderr);if(p.ExitCode!=0)throw new InvalidOperationException("dotnet planning generation failed with exit code "+p.ExitCode+".");}
