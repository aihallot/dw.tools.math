# Run-next failure evidence

- Invocation: `run-next-20261009T110432Z-0b137c58c59e4407`
- Run: `RS031`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS031 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-afa6376e6b374712baedf2a364390ac8\repository\tests\projects\dw.tools.math.ir.tests\dw.tools.math.ir.tests.csproj]
dw.quantities.tests -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-afa6376e6b374712baedf2a364390ac8\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-afa6376e6b374712baedf2a364390ac8\repository\tests\projects\dw.tools.math.ir.tests\M2W01C03BoundaryTests.cs(105,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-afa6376e6b374712baedf2a364390ac8\repository\tests\projects\dw.tools.math.ir.tests\dw.tools.math.ir.tests.csproj]
Payload source: payload.cs:line 296
