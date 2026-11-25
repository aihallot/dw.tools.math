#:property NuGetAudit=false
#:property PublishAot=false
using System.Diagnostics;
using System.Text.Json;

static void Need(bool ok, string reason)
{
    if (!ok) throw new InvalidDataException("M9 source Verification: " + reason);
}
static string[] Ids(JsonElement entries) => entries.EnumerateArray().Select(x => x.GetProperty("id").GetString()!).ToArray();
static JsonDocument Read(string path) => JsonDocument.Parse(File.ReadAllText(path));
foreach (var item in new[]
{
    (Run: "RS352", Subject: "test: qualify existing M9 bounded sampling GREEN", Count: 9, Preparation: "ae3aeb804dbdb42b1c7b56e72b729e8b28684cf8"),
    (Run: "RS353", Subject: "chore: complete M9 sampling GREEN and ready Verification", Count: 7, Preparation: "34eeb911404a3c929a8b4a75385fdc8121ed42ba")
})
{
    using var report = Read(".aura/workflow/reports/" + item.Run + "/attempt-001.json");
    using var proof = Read(".workflow/runs/" + item.Run + "/preparation-proof.json");
    var r = report.RootElement;
    var p = proof.RootElement;
    var prior = r.GetProperty("priorInvocationFailures");
    var observed = r.GetProperty("contribution").GetProperty("observed");
    var ids = Ids(observed.GetProperty("validations"));
    var progress = r.GetProperty("progress");
    var transitions = progress.GetProperty("transitions");
    var completions = progress.GetProperty("completions");
    Need(r.GetProperty("runId").GetString() == item.Run &&
        r.GetProperty("outcome").GetString() == "succeeded" &&
        r.GetProperty("attempt").GetInt32() == 1 &&
        r.GetProperty("executor").GetProperty("toolVersion").GetString() == "0.1.76" &&
        r.GetProperty("git").GetProperty("commitMessage").GetString() == item.Subject &&
        prior.GetProperty("count").GetInt32() == 0 && prior.GetProperty("firstPassSucceeded").GetBoolean() &&
        r.GetProperty("boundaryViolations").GetArrayLength() == 0 &&
        observed.GetProperty("mutationBoundaryClean").GetBoolean() &&
        !observed.GetProperty("changedPaths").EnumerateArray().Any(x => x.GetString()!.StartsWith("src/", StringComparison.Ordinal)),
        "Invalid first-pass source evidence: " + item.Run);
    Need(p.GetProperty("runId").GetString() == item.Run &&
        p.GetProperty("sourceCommit").GetString() == item.Preparation &&
        p.GetProperty("payloadCompiled").GetBoolean() &&
        p.GetProperty("rehearsedValidationCount").GetInt32() == item.Count &&
        ids.Length == item.Count &&
        p.GetProperty("preparationValidationIds").EnumerateArray().Select(x => x.GetString()!).SequenceEqual(ids, StringComparer.Ordinal),
        "Report/proof mismatch: " + item.Run);
    if (item.Run == "RS352")
    {
        Need(ids.Contains("m9-rs352-authentic-source-independent-oracle", StringComparer.Ordinal) &&
            ids.Contains("m9-rs352-statistics-locked-regression", StringComparer.Ordinal) &&
            transitions.GetArrayLength() == 1 && completions.GetArrayLength() == 0 &&
            transitions[0].GetProperty("id").GetString() == "M9-W01-C01-T1-G" &&
            transitions[0].GetProperty("beforeState").GetString() == "ready" &&
            transitions[0].GetProperty("afterState").GetString() == "in-progress", "RS352 activation differs.");
    }
    else
    {
        Need(ids.Contains("m9-rs353-green-closure-evidence", StringComparer.Ordinal) &&
            transitions.GetArrayLength() == 2 && completions.GetArrayLength() == 1 &&
            completions[0].GetProperty("id").GetString() == "M9-W01-C01-T1-G" &&
            completions[0].GetProperty("beforeState").GetString() == "in-progress" &&
            completions[0].GetProperty("afterState").GetString() == "done" &&
            transitions.EnumerateArray().Any(x => x.GetProperty("id").GetString() == "M9-W01-C01-T1-V" &&
                x.GetProperty("beforeState").GetString() == "not-ready" && x.GetProperty("afterState").GetString() == "ready"),
            "RS353 closure differs.");
    }
}
foreach (var item in new[]
{
    (Path: "src/projects/dw.tools.math.statistics/SplitMix64Random.cs", Blob: "a1e2817deeef8083fdc10f329afe4b9aa1e0fa00"),
    (Path: "src/projects/dw.tools.math.statistics/SplitMix64PositionCodec.cs", Blob: "39e6fb1884c690eed0f40abfefa240a01f8c5f35"),
    (Path: "tests/projects/dw.tools.math.statistics.tests/M9W01C01BoundedSamplingTests.cs", Blob: "b1c68801d80a88d470284925cce95b4c793f2f26"),
    (Path: "docs/engineering/fixtures/M9BoundedSamplingRedOracle.cs", Blob: "57a28e244074969c7255fc7c3d7a09487c622d65"),
    (Path: "docs/engineering/ProbeM9BoundedSamplingRed.cs", Blob: "9bf08087b3040013bcd1156ab43a1e3223d4961b"),
    (Path: "docs/distribution/bounded-integer-sampling.md", Blob: "f9da83d581f39efa8d10f791f09333b53d1a8581")
})
{
    var info = new ProcessStartInfo("git") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
    info.ArgumentList.Add("hash-object");
    info.ArgumentList.Add("--path=" + item.Path);
    info.ArgumentList.Add(item.Path);
    using var process = Process.Start(info) ?? throw new InvalidOperationException("git unavailable.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(15000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("git hash-object timed out."); }
    Need(process.ExitCode == 0 && stdout.GetAwaiter().GetResult().Trim() == item.Blob,
        "Qualified Git blob differs: " + item.Path + " " + stderr.GetAwaiter().GetResult());
}
Console.WriteLine("MATH-RS354-SOURCE-VERIFICATION|reports=2|compiledProofs=2|qualifiedGitBlobs=6|PASS");
