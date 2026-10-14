using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using AngouriMath;

// This is an experimental host/child-process safety harness, not a public
// Math symbolic adapter. No new library or AURA/BrokerReady API is exported.
const string DerivativeMarker = "M4-WORKER:derivative=residual-zero";
const string ConditionalMarker = "M4-WORKER:conditional=provided-text-only";
const string SuccessMarker = "dw.tools.math/m4-symbolic-boundaries/0.5 qualified; probes=14; timeout=host; cancellation=host; ir=unsupported";

if(args.Length==2 && args[0]=="--worker")
{
    switch(args[1])
    {
        case "derivative":
        {
            var polynomial=MathS.FromString("x^2 - 1");
            var delta=(polynomial.Differentiate("x").Simplify() -
                MathS.FromString("2*x")).Simplify();
            if(delta.ToString().Trim()!="0")
                throw new InvalidOperationException("The pinned derivative is not identically two x.");
            Console.WriteLine(DerivativeMarker);
            return;
        }
        case "conditional":
        {
            var conditional=MathS.FromString("(x^2 - 1)/(x - 1)").Simplify().ToString();
            if(!conditional.Contains("provided",StringComparison.OrdinalIgnoreCase)||
               !conditional.Contains("x",StringComparison.OrdinalIgnoreCase)||
               !conditional.Contains("1",StringComparison.Ordinal))
                throw new InvalidOperationException("Provider removed the observable x=1 exclusion.");
            // The provider string is NOT parsed as a verified Math-IR assumption:
            // only source-specific, human-readable conditional text was observed.
            Console.WriteLine(ConditionalMarker);
            return;
        }
        case "delay":
            Thread.Sleep(4000);
            Console.WriteLine("M4-WORKER:unexpected-delayed-success");
            return;
        case "failure":
            throw new InvalidOperationException("Intentional child failure for host isolation check.");
        default:
            Environment.ExitCode=64;
            return;
    }
}
if(args.Length!=0)
    throw new ArgumentException("Only the fixed, closed symbolic experiment is accepted.");

var passed=0;
void Probe(string name,Action test)
{
    try { test(); passed++; }
    catch(Exception error) { throw new InvalidOperationException("M4 symbolic boundary probe: "+name,error); }
}
static void Require(bool condition,string message)
{
    if(!condition)throw new InvalidOperationException(message);
}

Probe("bounded polynomial fixed corpus admitted",()=>
    Require(Inspect("x^2 - 1")==InputAdmission.Allowed,"polynomial fixture"));
Probe("bounded rational exclusion fixture admitted",()=>
    Require(Inspect("(x^2 - 1)/(x - 1)")==InputAdmission.Allowed,"rational fixture"));
Probe("empty input rejected before provider invocation",()=>
    Require(Inspect("")==InputAdmission.Invalid && Inspect(null)==InputAdmission.Invalid,
        "empty or null input must not invoke CAS"));
Probe("overlong expression blocked by independent length cap",()=>
    Require(Inspect(new string('x',GuardLimits.InputLengthLimit+1))==InputAdmission.BudgetExceeded,
        "128-character input limit"));
Probe("excessive nesting blocked by independent depth cap",()=>
    Require(Inspect("(((((((((x)))))))))")==InputAdmission.BudgetExceeded,
        "8-level nesting limit"));
Probe("token count cannot be bypassed by a short input",()=>
    Require(Inspect(new string('x',GuardLimits.InputTokenLimit+1))==InputAdmission.BudgetExceeded,
        "64 non-whitespace-token character cap"));
Probe("malformed or command-like text has no provider route",()=>
    Require(Inspect("x;System.IO.File.Delete()")==InputAdmission.Invalid,
        "code-like input is rejected by closed mathematical admission"));
Probe("sqrt square real-sign assumptions cannot be inferred",()=>
    Require(Inspect("sqrt(x^2)")==InputAdmission.Unsupported &&
        Inspect("y^2 - 1")==InputAdmission.Unsupported,
        "unknown assumptions and symbols are unsupported"));
Probe("domain hole stays excluded by explicit independent input policy",()=>
    Require(!AdmitKnownRationalPoint(1d) && AdmitKnownRationalPoint(2d) &&
        AdmitKnownRationalPoint(-1d),
        "simplification does not expand the domain at x=1"));

Probe("actual isolated Angouri derivative worker is bounded",()=>
{
    var result=InvokeChild("derivative",20000);
    Require(result.Status==WorkerStatus.Succeeded && result.Text.Trim()==DerivativeMarker,
        "the independently launched symbolic derivative must terminate successfully");
});
Probe("conditional text is visible but not a parsed Math IR guarantee",()=>
{
    var result=InvokeChild("conditional",20000);
    Require(result.Status==WorkerStatus.Succeeded && result.Text.Trim()==ConditionalMarker,
        "the provider must retain its observable provided condition");
});
Probe("child crash never becomes successful symbolic result",()=>
{
    var result=InvokeChild("failure",20000);
    Require(result.Status==WorkerStatus.ProviderFailure,
        "a nonzero worker exit is always a typed host failure");
});
Probe("host timeout forcibly stops a noncooperative worker",()=>
{
    var result=InvokeChild("delay",200);
    Require(result.Status==WorkerStatus.TimedOut && result.ChildTerminated,
        "noncooperative delay must be killed and cannot return a result");
});
Probe("host cancellation stops worker without relying on CAS cooperation",()=>
{
    var result=InvokeChild("delay",5000,cancelAfterStartMilliseconds:150);
    Require(result.Status==WorkerStatus.Cancelled && result.ChildTerminated,
        "host cancellation must kill a running child process");
});

if(passed!=14)throw new InvalidOperationException("Exactly fourteen independent boundary probes must pass.");
Console.WriteLine(SuccessMarker);

static InputAdmission Inspect(string? expression)
{
    if(string.IsNullOrWhiteSpace(expression))return InputAdmission.Invalid;
    if(expression.Length>GuardLimits.InputLengthLimit)return InputAdmission.BudgetExceeded;
    var depth=0;
    var tokens=0;
    foreach(var ch in expression)
    {
        if(char.IsWhiteSpace(ch))continue;
        tokens++;
        if(tokens>GuardLimits.InputTokenLimit)return InputAdmission.BudgetExceeded;
        if(ch=='(')
        {
            depth++;
            if(depth>GuardLimits.NestingLimit)return InputAdmission.BudgetExceeded;
        }
        else if(ch==')')
        {
            depth--;
            if(depth<0)return InputAdmission.Invalid;
        }
        else if(!((ch>='a' && ch<='z')||(ch>='0' && ch<='9')||
            ch is '^' or '-' or '+' or '*' or '/'))
            return InputAdmission.Invalid;
    }
    if(depth!=0)return InputAdmission.Invalid;
    // Deliberately NOT a generic text parser. Only this admitted closed
    // scientific corpus is callable in this experimental process.
    return expression is "x^2 - 1" or "(x^2 - 1)/(x - 1)"
        ? InputAdmission.Allowed : InputAdmission.Unsupported;
}
static bool AdmitKnownRationalPoint(double x) =>
    double.IsFinite(x) && x!=1d; // Only for (x^2-1)/(x-1), not any general CAS output.

static WorkerResult InvokeChild(string caseId,int timeoutMilliseconds,
    int? cancelAfterStartMilliseconds=null)
{
    if(timeoutMilliseconds is < 1 or > 30000)
        throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
    if(cancelAfterStartMilliseconds is < 1 or > 30000)
        throw new ArgumentOutOfRangeException(nameof(cancelAfterStartMilliseconds));
    var assembly=Assembly.GetExecutingAssembly().Location;
    if(!File.Exists(assembly))
        throw new InvalidOperationException("Bounded child assembly was not built.");
    var start=new ProcessStartInfo
    {
        FileName="dotnet",
        WorkingDirectory=Directory.GetCurrentDirectory(),
        UseShellExecute=false,
        RedirectStandardOutput=true,
        RedirectStandardError=true,
        CreateNoWindow=true
    };
    start.ArgumentList.Add(assembly);
    start.ArgumentList.Add("--worker");
    start.ArgumentList.Add(caseId);
    using var process=Process.Start(start) ??
        throw new InvalidOperationException("Could not start bounded .NET child.");
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    var sw=Stopwatch.StartNew();
    using var cancellation=new CancellationTokenSource();
    if(cancelAfterStartMilliseconds.HasValue)
        cancellation.CancelAfter(cancelAfterStartMilliseconds.Value);
    while(!process.WaitForExit(25))
    {
        if(cancellation.IsCancellationRequested)
        {
            process.Kill(entireProcessTree:true);
            if(!process.WaitForExit(5000))
                throw new InvalidOperationException("Cancelled child did not terminate.");
            return new(WorkerStatus.Cancelled,"",process.HasExited);
        }
        if(sw.ElapsedMilliseconds>=timeoutMilliseconds)
        {
            process.Kill(entireProcessTree:true);
            if(!process.WaitForExit(5000))
                throw new InvalidOperationException("Timed-out child did not terminate.");
            return new(WorkerStatus.TimedOut,"",process.HasExited);
        }
    }
    var text=stdout.GetAwaiter().GetResult();
    _=stderr.GetAwaiter().GetResult();
    return process.ExitCode==0
        ? new(WorkerStatus.Succeeded,text,process.HasExited)
        : new(WorkerStatus.ProviderFailure,"",process.HasExited);
}

enum InputAdmission { Allowed, Invalid, BudgetExceeded, Unsupported }
enum WorkerStatus { Succeeded, TimedOut, Cancelled, ProviderFailure }
readonly record struct WorkerResult(WorkerStatus Status,string Text,bool ChildTerminated);

static class GuardLimits
{
    public const int InputLengthLimit = 128;
    public const int NestingLimit = 8;
    public const int InputTokenLimit = 64;
}
