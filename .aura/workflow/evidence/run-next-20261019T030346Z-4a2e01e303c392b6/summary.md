# Run-next failure evidence

- Invocation: `run-next-20261019T030346Z-4a2e01e303c392b6`
- Run: `RS095`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS095 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj -c Release --no-restore exited 1: error CS0165: Use of unassigned local variable 'bins' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-00a74e9040404cebb9dabc330fa8272b\repository\tests\consumers\m6-w02-c03-s02-t1\m6-direct-signal-consumer.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-00a74e9040404cebb9dabc330fa8272b\repository\tests\consumers\m6-w02-c03-s02-t1\Program.cs(37,25): error CS0165: Use of unassigned local variable 'bins' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-00a74e9040404cebb9dabc330fa8272b\repository\tests\consumers\m6-w02-c03-s02-t1\m6-direct-signal-consumer.csproj]
0 Warning(s)
Payload source: payload.cs:line 484
