# Run-next failure evidence

- Invocation: `run-next-20261008T155618Z-235d7885a8333385`
- Run: `RS018`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS018 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6f12b328face4768b2269eb121c3f618\repository\tests\projects\dw.quantities.tests\dw.quantities.tests.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6f12b328face4768b2269eb121c3f618\repository\tests\projects\dw.quantities.tests\M1W02C01Tests.cs(14,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6f12b328face4768b2269eb121c3f618\repository\tests\projects\dw.quantities.tests\dw.quantities.tests.csproj]
0 Warning(s)
Payload source: payload.cs:line 266
