# Run-next failure evidence

- Invocation: `run-next-20261016T083336Z-3225e316580d8f09`
- Run: `RS070`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS070 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build dw.tools.math.slnx -c Release --no-restore exited 1: error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-286ba3d65add4e43942a1a41a051683d\repository\tests\projects\dw.tools.math.codecs.tests\dw.tools.math.codecs.tests.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-286ba3d65add4e43942a1a41a051683d\repository\tests\projects\dw.tools.math.codecs.tests\M5W01C03T1Tests.cs(424,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-286ba3d65add4e43942a1a41a051683d\repository\tests\projects\dw.tools.math.codecs.tests\dw.tools.math.codecs.tests.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-286ba3d65add4e43942a1a41a051683d\repository\tests\projects\dw.tools.math.codecs.tests\M5W01C03T1Tests.cs(425,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-286ba3d65add4e43942a1a41a051683d\repository\tests\projects\dw.tools.math.codecs.tests\dw.tools.math.codecs.tests.csproj]
Build FAILED.
Payload source: payload.cs:line 437
