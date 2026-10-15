# Run-next failure evidence

- Invocation: `run-next-20261015T135904Z-b37f18ccac470190`
- Run: `RS065`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS065 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build tests/consumers/m4-gate/m4-gate-consumer.csproj -c Release --no-restore exited 1: error CS8321: The local function 'Mul' is declared but never used [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-9e45eb88cd924109b4c257f8a73f6e9b\repository\tests\consumers\m4-gate\m4-gate-consumer.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-9e45eb88cd924109b4c257f8a73f6e9b\repository\tests\consumers\m4-gate\Program.cs(26,15): error CS8321: The local function 'Mul' is declared but never used [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-9e45eb88cd924109b4c257f8a73f6e9b\repository\tests\consumers\m4-gate\m4-gate-consumer.csproj]
0 Warning(s)
Payload source: payload.cs:line 365
