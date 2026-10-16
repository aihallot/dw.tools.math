# Run-next failure evidence

- Invocation: `run-next-20261016T101706Z-cb0d6d873bdae671`
- Run: `RS070`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS070 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3f5b7183c81e409d9429b22de81d02ad\repository\tests\projects\dw.tools.math.codecs.tests\dw.tools.math.codecs.tests.csproj]
dw.tools.math.symbolics.tests -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3f5b7183c81e409d9429b22de81d02ad\repository\build\artifacts\bin\dw.tools.math.symbolics.tests\Release\net10.0\dw.tools.math.symbolics.tests.dll
dw.quantities.tests -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3f5b7183c81e409d9429b22de81d02ad\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll
Build FAILED.
Payload source: payload.cs:line 437
