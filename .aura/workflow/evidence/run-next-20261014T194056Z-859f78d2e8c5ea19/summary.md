# Run-next failure evidence

- Invocation: `run-next-20261014T194056Z-859f78d2e8c5ea19`
- Run: `RS057`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS057 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~M4W01C03T1 --logger console;verbosity=normal exited 1: Error Message:
Assertion failed. Expected values to be equal.
expected: BudgetExceeded
actual:   Unsupported
Payload source: payload.cs:line 416
