# Run-next failure evidence

- Invocation: `run-next-20261015T173150Z-5f334d6bf2be8650`
- Run: `RS066`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS066 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~M5W01C01T1 --logger console;verbosity=normal exited 1: Error Message:
Assertion failed. Expected condition to be true.
Expected admitted canonical text: unit(27315/100,si.kelvin,Absolute); status=NonCanonical
actual: false
Payload source: payload.cs:line 454
