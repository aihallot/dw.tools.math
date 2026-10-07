#:property PublishAot=false
#:property NuGetAudit=false
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// BCL only. Default check is observational; --write belongs in the payload.
try
{
    var mode = args.Length == 0 ? "--check" : args.Single();
    if (mode is not ("--check" or "--write" or "--self-test"))
        throw new InvalidOperationException("Usage: dotnet run --file docs/planning/ValidatePlan.cs -- [--check|--write|--self-test]");
    var repo = Directory.GetCurrentDirectory();
    var bytes = File.ReadAllBytes(Path.Combine(repo, "docs/planning/backlog.json"));
    var plan = JsonNode.Parse(bytes)!.AsObject();
    Validate(plan, repo);
    if (mode == "--self-test") { SelfTest(plan, repo); return 0; }
    var pages = Render(plan);
    var report = new JsonObject
    {
        ["schema_version"] = 1, ["plan"] = S(plan, "plan_version"),
        ["status"] = "documentary_validation_only", ["product_qualified"] = false,
        ["backlog_sha256"] = Hash(bytes),
        ["validator_sha256"] = Hash(File.ReadAllBytes(Path.Combine(repo, "docs/planning/ValidatePlan.cs"))),
        ["source_baseline_sha256"] = Hash(File.ReadAllBytes(Path.Combine(repo, "docs/planning/source-baseline.json"))),
        ["counts"] = new JsonObject
        {
            ["releases"] = A(plan,"releases").Count, ["work_packages"] = A(plan,"work_packages").Count,
            ["chunks"] = A(plan,"chunks").Count, ["requirements"] = A(plan,"requirements").Count,
            ["tasks"] = Objects(plan,"chunks").Sum(c=>A(c,"tasks").Count),
            ["subtasks"] = Objects(plan,"chunks").Sum(c=>Objects(c,"tasks").Sum(t=>A(t,"subtasks").Count))
        },
        ["generated_sha256"] = new JsonObject(pages.Select(p=>new KeyValuePair<string,JsonNode?>(p.Key,JsonValue.Create(Hash(Encoding.UTF8.GetBytes(p.Value)))))),
        ["limitations"] = "Not product tests, scientific correctness, native DWF compatibility, source rights, provider admission or consumer adoption."
    };
    pages["docs/planning/planning-validation.json"] = Json(report);
    if (mode == "--write")
        foreach (var page in pages)
        {
            var path = Path.Combine(repo,page.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path,page.Value,new UTF8Encoding(false));
        }
    foreach(var page in pages)
        Equal(File.ReadAllText(Path.Combine(repo,page.Key)),page.Value,"Stale generated file: "+page.Key);
    CheckLinks(repo);
    Console.WriteLine($"planning valid: {S(plan,"plan_version")} | {A(plan,"chunks").Count} chunks | {Hash(bytes)} | mode {mode}");
    return 0;
}
catch (Exception ex) { Console.Error.WriteLine("planning invalid: "+ex.Message); return 1; }

static string S(JsonObject o,string key) => o[key]?.GetValue<string>() is { Length: > 0 } s ? s : throw new InvalidOperationException("Missing string "+key);
static JsonArray A(JsonObject o,string key) => o[key] as JsonArray ?? throw new InvalidOperationException("Missing array "+key);
static IEnumerable<JsonObject> Objects(JsonObject o,string key) => A(o,key).Select(n=>n?.AsObject()??throw new InvalidOperationException("Null object "+key));
static string[] Strings(JsonObject o,string key) => A(o,key).Select(n=>n?.GetValue<string>()??throw new InvalidOperationException("Null value "+key)).ToArray();
static void R(bool value,string message) { if(!value) throw new InvalidOperationException(message); }
static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Json(JsonNode node)=>node.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n";
static void Equal(string actual,string expected,string message)=>R(actual==expected,message);
static bool Active(JsonObject o)=>S(o,"status") is "ready" or "in_progress" or "done";
static bool RequiresActiveParent(JsonObject o)=>S(o,"status") is "ready" or "in_progress";
static bool ParentCanContainCompletedChild(JsonObject o)=>S(o,"status") is "in_progress" or "blocked" or "done";
static string Safe(string path)
{
    R(!string.IsNullOrWhiteSpace(path)&&!Path.IsPathRooted(path)&&!path.Contains(':')&&!path.Contains('\\'),"Unsafe path "+path);
    R(!path.Split('/').Any(p=>p is ".." or "." or ""),"Unsafe path "+path); return path;
}
static Dictionary<string,JsonObject> Index(JsonObject root,string key)
{
    var result=new Dictionary<string,JsonObject>(StringComparer.OrdinalIgnoreCase);
    foreach(var o in Objects(root,key))R(result.TryAdd(S(o,"id"),o),"Duplicate id "+S(o,"id"));
    return result;
}
static void Cycles(Dictionary<string,JsonObject> nodes,string field)
{
    var visited=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var stack=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    void Visit(string id)
    {
        R(nodes.ContainsKey(id),"Unknown dependency "+id);R(!stack.Contains(id),"Dependency cycle "+id);
        if(!visited.Add(id))return;
        stack.Add(id);foreach(var d in Strings(nodes[id],field))Visit(d);stack.Remove(id);
    }
    foreach(var id in nodes.Keys)Visit(id);
}
static void Validate(JsonObject p,string repo)
{
    R(p["schema_version"]?.GetValue<int>()==1,"Unsupported schema");
    R(S(p,"source_of_truth")=="docs/planning/backlog.json","Wrong source of truth");
    var releases=Index(p,"releases");var wps=Index(p,"work_packages");var chunks=Index(p,"chunks");
    var reqs=Index(p,"requirements");var exts=Index(p,"external_dependencies");var suites=Index(p,"test_suites");
    R(releases.Count>0&&chunks.Count>0,"Empty plan");
    var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    void Unique(JsonObject o)=>R(seen.Add(S(o,"id")),"Duplicate global id "+S(o,"id"));
    void Evidence(JsonObject o)
    {
        var e=Strings(o,"evidence");R(e.Length>0,"Missing evidence "+S(o,"id"));
        foreach(var path in e)R(File.Exists(Path.Combine(repo,Safe(path))),"Missing evidence file "+path);
    }
    void State(JsonObject o)
    {
        Unique(o);var state=S(o,"status");
        R(new[]{"planned","ready","in_progress","blocked","done"}.Contains(state),"Invalid state "+S(o,"id"));
        if(state=="done")Evidence(o);if(state=="blocked")_ = S(o,"blocker");
    }
    foreach(var r in releases.Values)State(r);
    foreach(var w in wps.Values)
    {
        State(w);R(releases.ContainsKey(S(w,"release")),"Unknown WP release");
        var children=chunks.Values.Where(c=>S(c,"work_package")==S(w,"id")).ToArray();
        R(children.Length>0,"Empty WP "+S(w,"id"));
        if(S(w,"status")=="done")R(children.All(c=>S(c,"status")=="done"),"Incomplete WP");
    }
    foreach(var q in reqs.Values) { Unique(q);R(chunks.ContainsKey(S(q,"owner")),"Orphan requirement");_ = S(q,"acceptance"); }
    foreach(var x in exts.Values)
    {
        Unique(x);R(new[]{"unverified","satisfied","blocked"}.Contains(S(x,"status")),"Invalid external state");
        R(chunks.ContainsKey(S(x,"qualified_by")),"Unknown external qualifier");
        if(S(x,"status")=="satisfied"){Evidence(x);R(S(chunks[S(x,"qualified_by")],"status")=="done","External qualifier incomplete");}
        if(S(x,"status")=="blocked")_ = S(x,"blocker");
    }
    foreach(var c in chunks.Values)
    {
        State(c);var id=S(c,"id");var rel=S(c,"release");var wp=S(c,"work_package");
        R(releases.ContainsKey(rel)&&wps.ContainsKey(wp),"Unknown parent "+id);
        R(S(wps[wp],"release")==rel,"Parent release mismatch "+id);
        R(new[]{"code","spike","document","aggregate"}.Contains(S(c,"kind")),"Unknown kind "+id);
        R(new[]{"S","M","L"}.Contains(S(c,"size")),"Unknown size "+id);
        foreach(var key in new[]{"deliverables","acceptance","boundary_acceptance"})_ = S(c,key);
        var deps=Strings(c,"depends_on");
        R(deps.Distinct(StringComparer.OrdinalIgnoreCase).Count()==deps.Length,"Duplicate dependency "+id);
        R(deps.All(chunks.ContainsKey),"Unknown chunk dependency "+id);
        R(Strings(c,"test_suites").Length>0&&Strings(c,"test_suites").All(suites.ContainsKey),"Unknown/empty suite "+id);
        var requirements=Strings(c,"requirements");R(requirements.Length>0&&requirements.All(reqs.ContainsKey),"Unknown/empty requirement "+id);
        foreach(var q in requirements)R(S(reqs[q],"owner")==id||((string?)c["parent_chunk"]==S(reqs[q],"owner")),"Requirement owner mismatch "+id);
        var external=Strings(c,"external_dependencies");R(external.All(exts.ContainsKey),"Unknown external dependency "+id);
        foreach(var path in Strings(c,"files"))_ = Safe(path);
        foreach(var path in Strings(c,"proposed_files"))_ = Safe(path);
        if(c["parent_chunk"] is not null)
        {
            var parent=S(c,"parent_chunk");
            R(chunks.ContainsKey(parent)&&parent!=id&&S(chunks[parent],"work_package")==wp,"Invalid split parent "+id);
        }
        if(Active(c))
        {
            R(deps.All(d=>S(chunks[d],"status")=="done"),"Incomplete dependency "+id);
            R(Strings(releases[rel],"depends_on").All(d=>releases.ContainsKey(d)&&S(releases[d],"status")=="done"),"Prior gate incomplete "+id);
            R(external.All(d=>S(exts[d],"status")=="satisfied"),"Unqualified external dependency "+id);
            R(Strings(c,"files").Length>0&&Strings(c,"commands").Length>0,"Missing readiness files/commands "+id);
            _ = S(c,"refinement");
            if(S(c,"size")=="L")R(chunks.Values.Any(n=>(string?)n["parent_chunk"]==id),"Unsplit L block "+id);
        }
        var tasks=Objects(c,"tasks").ToArray();R(tasks.Length>0,"Empty tasks "+id);
        foreach(var t in tasks)
        {
            State(t);_ = S(t,"behavior");_ = S(t,"test_case");
            var sub=Objects(t,"subtasks").ToArray();R(sub.Length>0,"Empty subtasks "+id);
            foreach(var s in sub)
            {
                State(s);_ = S(s,"description");
                if(RequiresActiveParent(s))R(Active(t),"Subtask active under inactive task");
                if(S(s,"status")=="done")R(ParentCanContainCompletedChild(t),"Completed subtask under unstarted task");
            }
            if(RequiresActiveParent(t))R(Active(c),"Task active under inactive chunk");
            if(S(t,"status")=="done")R(ParentCanContainCompletedChild(c),"Completed task under unstarted chunk");
            if(S(t,"status")=="done")R(sub.All(s=>S(s,"status")=="done"),"Incomplete task");
        }
        if(S(c,"status")=="done")
        {
            R(tasks.All(t=>S(t,"status")=="done"),"Incomplete chunk");
            R(chunks.Values.Where(n=>(string?)n["parent_chunk"]==id).All(n=>S(n,"status")=="done"),"Incomplete split");
        }
    }
    foreach(var q in reqs.Values)
    {
        R(Strings(chunks[S(q,"owner")],"requirements").Contains(S(q,"id")),"Unlinked requirement");
        R(S(q,"release")==S(chunks[S(q,"owner")],"release"),"Requirement release mismatch");
    }
    foreach(var r in releases.Values)
    {
        var id=S(r,"id");R(S(r,"gate")=="G-"+id,"Invalid gate");
        var listed=Strings(r,"required_chunks");
        var actual=chunks.Values.Where(c=>S(c,"release")==id).Select(c=>S(c,"id")).ToHashSet(StringComparer.OrdinalIgnoreCase);
        R(listed.Length==listed.Distinct(StringComparer.OrdinalIgnoreCase).Count()&&actual.SetEquals(listed),"Incomplete or duplicate gate "+id);
        R(Strings(r,"depends_on").All(releases.ContainsKey),"Unknown release dependency");
        if(S(r,"status")=="done")
        {
            R(listed.All(c=>S(chunks[c],"status")=="done"),"Release incomplete");
            R(wps.Values.Where(w=>S(w,"release")==id).All(w=>S(w,"status")=="done"),"Release WP incomplete");
            R(Strings(r,"depends_on").All(d=>S(releases[d],"status")=="done"),"Release prerequisites incomplete");
        }
    }
    Cycles(chunks,"depends_on");Cycles(releases,"depends_on");
    foreach(var start in chunks.Keys)
    {
        var parents=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var current=start;
        while(chunks[current]["parent_chunk"] is not null){R(parents.Add(current),"Split cycle");current=S(chunks[current],"parent_chunk");}
    }
    var coverage=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach(var row in Objects(p,"source_coverage"))
    {
        R(coverage.Add(S(row,"source_id")),"Duplicate coverage");
        R(Strings(row,"targets").Length>0&&Strings(row,"targets").All(chunks.ContainsKey),"Orphan coverage");
    }
    R(coverage.Count>0,"No source coverage");
    foreach(var d in Objects(p,"dispositions"))
    {
        Unique(d);foreach(var k in new[]{"owner","reason","review_trigger"})_ = S(d,k);
        R(Strings(d,"targets").All(chunks.ContainsKey),"Unknown disposition target");
    }
}
static Dictionary<string,string> Render(JsonObject p)
{
    var pages=new Dictionary<string,string>(StringComparer.Ordinal);
    var rs=Objects(p,"releases").ToArray();var wps=Objects(p,"work_packages").ToArray();var cs=Objects(p,"chunks").ToArray();
    var index=new StringBuilder("# Implementation plan — dw.tools.math\n\n");
    index.Append("**Objective:** extract the AURA mathematical core, then build an independent, typed, qualified platform.\n\n");
    index.Append("Product plan "+S(p,"plan_version")+". States come from the [canonical backlog](backlog.json). No documentation status proves a product capability.\n\n");
    index.Append("Hierarchy: **release -> work package -> chunk -> task -> subtask**. DWF maps these levels to milestone/workPackage/phase/task/subtask.\n\n");
    index.Append("| Milestone | Target version | Outcome | Status |\n|---|---|---|---|\n");
    foreach(var r in rs)index.Append($"| [{S(r,"id")}](versions/{S(r,"id")}.md) | {S(r,"version")} | {S(r,"outcome")} | {S(r,"status")} |\n");
    index.Append("\nM1 delivers the exact core early; 1.0 follows the M5 numerical/symbolic vertical. M7 spikes may conclude with deferral: their decisions are not delivered functions. Target versions are indicative, not dates or authorization to execute the whole program.\n\n");
    index.Append("- [Architecture](architecture.md) and [inventory](existing-code-inventory.md)\n- [Coverage](coverage.md), [sources](sources.md), and [qualification](testing-and-quality.md)\n- [Coordination](cross-repository-coordination.md) and [DWF handoff](implementation-handoff.md)\n- [Resource policy](resource-cost-policy.md)\n\n");
    index.Append($"Scope: {rs.Length} releases, {wps.Length} work packages, {cs.Length} chunks, {cs.Sum(c=>A(c,"tasks").Count)} tasks, {cs.Sum(c=>Objects(c,"tasks").Sum(t=>A(t,"subtasks").Count))} subtasks, {A(p,"requirements").Count} requirements.\n\n");
    index.Append("Verify: dotnet run --file docs/planning/ValidatePlan.cs -- --check. Regenerate after editing with -- --write, exclusively outside preparation-safe validation. Self-tests: -- --self-test.\n\n");
    index.Append("Functional dependencies, no agent fan-out. Refine files/commands before ready; split L blocks; do not create empty product packages in advance.\n");
    pages["docs/planning/README.md"]=index.ToString();
    var map=new JsonObject {["schema_version"]=1,["kind"]="product-id-mapping-not-native-dwf-plan",["plan_version"]=S(p,"plan_version"),["nodes"]=new JsonArray()};
    var nodes=map["nodes"]!.AsArray();
    void Node(string id,string level,string? parent)=>nodes.Add(new JsonObject{["product_id"]=id,["dwf_level"]=level,["parent_id"]=parent});
    foreach(var r in rs)
    {
        var rid=S(r,"id");Node(rid,"milestone",null);
        var text=new StringBuilder($"# {rid} — {S(r,"title")} / {S(r,"version")}\n\n[Index](../README.md) · [Backlog](../backlog.json) · [Handoff](../implementation-handoff.md)\n\n");
        text.Append($"Objective: {S(r,"outcome")}\n\nStatus: **{S(r,"status")}**. Gate: **{S(r,"gate")}**. Prerequisites: {string.Join(", ",Strings(r,"depends_on"))}.\n\n");
        foreach(var w in wps.Where(w=>S(w,"release")==rid))
        {
            var wid=S(w,"id");Node(wid,"workPackage",rid);
            text.Append($"## {wid} — {S(w,"title")}\n\nStatus: {S(w,"status")}. Proposed anchors: {string.Join(", ",Strings(w,"paths"))}.\n\n");
            foreach(var c in cs.Where(c=>S(c,"work_package")==wid))
            {
                var cid=S(c,"id");Node(cid,"phase",wid);
                text.Append($"### {cid} — {S(c,"title")}\n\nStatus: {S(c,"status")}; size: {S(c,"size")}; kind: {S(c,"kind")}.\n\n");
                text.Append($"Dependencies: {string.Join(", ",Strings(c,"depends_on"))}. External: {string.Join(", ",Strings(c,"external_dependencies"))}.\n\n");
                text.Append($"Requirements: {string.Join(", ",Strings(c,"requirements"))}. Suites: {string.Join(", ",Strings(c,"test_suites"))}.\n\n");
                text.Append($"**Deliverable:** {S(c,"deliverables")}\n\n**Independent acceptance:** {S(c,"acceptance")}\n\n**Boundaries/integration:** {S(c,"boundary_acceptance")}\n\n");
                text.Append("Proposed files, to refine: "+string.Join(", ",Strings(c,"proposed_files"))+".\n\n");
                text.Append("Proposed commands (runner/project still to qualify): "+string.Join(" ; ",Strings(c,"proposed_commands"))+".\n\n");
                foreach(var t in Objects(c,"tasks"))
                {
                    var tid=S(t,"id");Node(tid,"task",cid);
                    text.Append($"#### {tid} — {S(t,"title")}\n\n{S(t,"behavior")}\n\nFalsification: {S(t,"test_case")}\n\n");
                    foreach(var s in Objects(t,"subtasks"))
                    {
                        Node(S(s,"id"),"subtask",tid);
                        text.Append($"- [{(S(s,"status")=="done"?"x":" ")}] **{S(s,"id")} — {S(s,"title")}** ({S(s,"status")}) : {S(s,"description")}\n");
                    }
                    text.Append('\n');
                }
            }
        }
        pages[$"docs/planning/versions/{rid}.md"]=text.ToString();
    }
    pages["docs/planning/dwf-map.json"]=Json(map);
    var coverage=new StringBuilder("# Framing and request coverage\n\nBacklog projection. Coverage means planned responsibility, not acquired qualification.\n\n| Source | Need | Chunks |\n|---|---|---|\n");
    foreach(var row in Objects(p,"source_coverage"))coverage.Append($"| {S(row,"source_id")} | {S(row,"title")} | {string.Join(", ",Strings(row,"targets"))} |\n");
    coverage.Append("\n## V1 framing criteria\n\n1. Parse notation: M5-W01-C01/C02.\n2. Validate domains/assumptions: M2-W01-C02/C03.\n3. Numerical and symbolic computation: M3 and M4.\n4. Compose: M2-W02-C02.\n5. Reject incompatible composition: M2-W02-C02.\n6. Preserve exclusions: M4-W01-C02/C03.\n7. Render notation: M5-W01-C01/C03/C04.\n8. Results/errors/provenance: M2-W02-C01.\n9. Qualified replay: M2-W02-C03.\n10. Client without AURA: M1-W03-C01 and M5-W02-C01.\n\nGate M5-W02-C03: confront these criteria with real evidence.\n\n## Boundaries and exclusions\n\n");
    foreach(var d in Objects(p,"dispositions"))coverage.Append($"- **{S(d,"id")} — {S(d,"title")}** — {S(d,"status")}, owner: {S(d,"owner")}: {S(d,"reason")}. Review trigger: {S(d,"review_trigger")}.\n");
    pages["docs/planning/coverage.md"]=coverage.ToString();
    return pages;
}
static void CheckLinks(string repo)
{
    var paths=Directory.EnumerateFiles(Path.Combine(repo,"docs"),"*.md",SearchOption.AllDirectories)
        .Concat(Directory.EnumerateFiles(repo,"*.md",SearchOption.TopDirectoryOnly));
    foreach(var file in paths)
        foreach(Match match in Regex.Matches(File.ReadAllText(file),@"\[[^\]]*\]\(([^)]+)\)"))
        {
            var link=match.Groups[1].Value.Trim('<','>').Split('#')[0];
            if(link.Length==0||Uri.TryCreate(link,UriKind.Absolute,out _))continue;
            var target=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!,Uri.UnescapeDataString(link)));
            R(File.Exists(target)||Directory.Exists(target),"Broken local link "+link+" in "+Path.GetRelativePath(repo,file));
        }
}
static void SelfTest(JsonObject p,string repo)
{
    var count=0;
    void Reject(string label,Action<JsonObject> change)
    {
        var copy=p.DeepClone().AsObject();change(copy);
        try{Validate(copy,repo);}catch(InvalidOperationException){count++;return;}
        throw new InvalidOperationException("Self-test missed "+label);
    }
    Reject("duplicate id",q=>q["chunks"]!.AsArray().Add(q["chunks"]![0]!.DeepClone()));
    Reject("cycle",q=>q["chunks"]![0]!["depends_on"]!.AsArray().Add("M0-W01-C02"));
    Reject("unknown dependency",q=>q["chunks"]![0]!["depends_on"]!.AsArray().Add("UNKNOWN"));
    Reject("gate omission",q=>q["releases"]![0]!["required_chunks"]!.AsArray().RemoveAt(0));
    Reject("unknown parent",q=>q["chunks"]![0]!["work_package"]="UNKNOWN");
    Reject("orphan requirement",q=>q["requirements"]![0]!["owner"]="UNKNOWN");
    Reject("coverage orphan",q=>q["source_coverage"]![0]!["targets"]!.AsArray().Add("UNKNOWN"));
    Reject("invalid state",q=>q["chunks"]![0]!["status"]="shipped");
    Reject("done without evidence",q=>q["chunks"]![0]!["status"]="done");
    Reject("ready without refinement",q=>q["chunks"]![0]!["status"]="ready");
    Reject("unsafe mutation path",q=>q["chunks"]![0]!["files"]!.AsArray().Add("../aura/README.md"));
    Reject("false provider admission",q=>q["external_dependencies"]![0]!["status"]="satisfied");
    Reject("active child under planned parent",q=>q["chunks"]![2]!["tasks"]![0]!["subtasks"]![0]!["status"]="ready");
    Reject("completed child under planned parent",q=>
    {
        q["chunks"]![2]!["tasks"]![0]!["subtasks"]![0]!["status"]="done";
        q["chunks"]![2]!["tasks"]![0]!["subtasks"]![0]!["evidence"]=new JsonArray(".aura/workflow/reports/RS001/attempt-003.json");
    });
    var partialBlocked=p.DeepClone().AsObject();
    partialBlocked["chunks"]![2]!["status"]="blocked";
    partialBlocked["chunks"]![2]!["blocker"]="external decision pending";
    partialBlocked["chunks"]![2]!["tasks"]![0]!["status"]="blocked";
    partialBlocked["chunks"]![2]!["tasks"]![0]!["blocker"]="external decision pending";
    partialBlocked["chunks"]![2]!["tasks"]![0]!["subtasks"]![0]!["status"]="done";
    partialBlocked["chunks"]![2]!["tasks"]![0]!["subtasks"]![0]!["evidence"]=new JsonArray(".aura/workflow/reports/RS001/attempt-003.json");
    Validate(partialBlocked,repo);
    var staleRejected=false;try{Equal("stale","current","projection stale");}catch(InvalidOperationException){staleRejected=true;}
    R(staleRejected,"Stale comparison");count++;
    Console.WriteLine($"planning self-tests passed: {count} negative cases plus partial-blocked positive case; no repository writes");
}
