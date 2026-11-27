# Run-next failure evidence

- Invocation: `run-next-20261127T170741Z-b336ddbaff581a31`
- Run: `RS402`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS402 failed before attempt allocation. classification=validation:math-rs402-project-dispositions. The source repository remains unchanged and no durable attempt was created.
Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=3920
Standard output:
[dwf-validation] strategy=declared-authority stdoutPresent=false stderrPresent=true nugetDiagnosticObserved=false compilerOrMsbuildDiagnosticObserved=false Process status: timedOut=false cancelled=false exitCode=-532462766 workflowExitCode=-532462766 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=240000 durationMilliseconds=3920
Standard error:
Unhandled exception. System.IO.InvalidDataException: M10 dispositions: Disposition ledger differs from observed project files.
   at Program.<<Main>$>g__Need|0_0(Boolean ok, String message) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-b10791ddad5243c0b9cebf45a3136192\repository\docs\engineering\InspectM10ProjectDispositions.cs:line 8
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-b10791ddad5243c0b9cebf45a3136192\repository\docs\engineering\InspectM10ProjectDispositions.cs:line 94
