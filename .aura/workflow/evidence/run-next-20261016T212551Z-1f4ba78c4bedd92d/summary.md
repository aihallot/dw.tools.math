# Run-next failure evidence

- Invocation: `run-next-20261016T212551Z-1f4ba78c4bedd92d`
- Run: `RS077`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS077 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj -c Release --no-build --no-restore exited 1: Error Message:
Assertion failed. Expected strings to be equal.
Strings have same length (15) and differ at 1 location(s). First difference at index 14.
expected:   "1.0.0-preview.3"
Payload source: payload.cs:line 427
