# Run-next failure evidence

- Invocation: `run-next-20261127T200659Z-dfe3e4b433a636e2`
- Run: `RS413`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS413 failed before attempt allocation. classification=validation:math-rs413-actual-target. The source repository remains unchanged and no durable attempt was created.
Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=22528
Standard output:
[dwf-validation] strategy=declared-authority stdoutPresent=false stderrPresent=true nugetDiagnosticObserved=false compilerOrMsbuildDiagnosticObserved=false Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=22528
Standard error:
Unhandled exception. System.IO.InvalidDataException: M10 production cutover: Target mismatch: tools/dw.tools.math.nuget-preflight/Program.cs
   at Program.<<Main>$>g__Need|0_0(Boolean ok, String message) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7d025ff6db5249038f0054bcd4ba9d1c\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 10
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7d025ff6db5249038f0054bcd4ba9d1c\repository\docs\engineering\VerifyM10ProductionCutover.cs:line 123
