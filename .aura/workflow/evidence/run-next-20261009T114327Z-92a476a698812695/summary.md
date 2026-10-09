# Run-next failure evidence

- Invocation: `run-next-20261009T114327Z-92a476a698812695`
- Run: `RS031`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS031 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~M2W01C03BoundaryTests.MalformedJsonAndInvalidBoundSymbolAreNotAccepted --logger console;verbosity=normal exited 1: Error Message:
Assertion failed. Expected exception of exact type JsonException but caught JsonReaderException.
expected type:    System.Text.Json.JsonException
actual exception: System.Text.Json.JsonReaderException: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 0 | BytePositionInLine: 1.
Payload source: payload.cs:line 296
