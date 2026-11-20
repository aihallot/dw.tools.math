# Run-next failure evidence

- Invocation: `run-next-20261120T152343Z-610c98c521589707`
- Run: `RS268`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS268 failed before attempt allocation. classification=validation:m9-bounded-integer-exact-regressions. The source repository remains unchanged and no durable attempt was created.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\src\projects\dw.quantities\dw.quantities.csproj (in 146 ms).
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\src\projects\dw.tools.math.statistics\dw.tools.math.statistics.csproj (in 149 ms).
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\tests\projects\dw.tools.math.statistics.tests\dw.tools.math.statistics.tests.csproj (in 548 ms).
  dw.quantities -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\build\artifacts\bin\dw.quantities\Release\net10.0\dw.quantities.dll
  dw.tools.math.statistics -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\build\artifacts\bin\dw.tools.math.statistics\Release\net10.0\dw.tools.math.statistics.dll
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\tests\projects\dw.tools.math.statistics.tests\M9W01C01BoundedSamplingTests.cs(13,9): error MSTEST0032: Review or remove the assertion as its condition is known to be always true (https://learn.microsoft.com/dotnet/core/testing/mstest-analyzers/mstest0032) [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3053d051a6d44a4a8a843818ca07efab\repository\tests\projects\dw.tools.math.statistics.tests\dw.tools.math.statistics.tests.csproj]
Standard error:
<empty>
