# Run-next failure evidence

- Invocation: `run-next-20261008T111551Z-3b7365bf806d0665`
- Run: `RS015`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS015 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet exited 1:   Determining projects to restore...
Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0770942c0fc14087b938f2d89a9a11af\repository\src\projects\dw.quantities\dw.quantities.csproj (in 88 ms).
Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0770942c0fc14087b938f2d89a9a11af\repository\tests\projects\dw.quantities.tests\dw.quantities.tests.csproj (in 275 ms).
dw.quantities -> E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0770942c0fc14087b938f2d89a9a11af\repository\build\artifacts\bin\dw.quantities\Release\net10.0\dw.quantities.dll
Payload source: payload.cs:line 202
