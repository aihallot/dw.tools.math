# Run-next failure evidence

- Invocation: `run-next-20261012T162607Z-0438bb811539ed74`
- Run: `RS041`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS041 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5df47487278b48d294558fdaf3940821\repository\tests\projects\dw.tools.math.numerics.tests\dw.tools.math.numerics.tests.csproj]
dw.quantities.tests -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5df47487278b48d294558fdaf3940821\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5df47487278b48d294558fdaf3940821\repository\tests\projects\dw.tools.math.numerics.tests\M3W01C02T2Tests.cs(111,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5df47487278b48d294558fdaf3940821\repository\tests\projects\dw.tools.math.numerics.tests\dw.tools.math.numerics.tests.csproj]
Payload source: payload.cs:line 282
