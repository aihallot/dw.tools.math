# Run-next failure evidence

- Invocation: `run-next-20261014T085730Z-f37ded5904f858aa`
- Run: `RS052`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS052 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build tests/consumers/m3-gate/m3-gate-consumer.csproj -c Release --no-restore exited 1: error CS8629: Nullable value type may be null. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-323ed7bf89134d9aa2a3459e1cae9402\repository\tests\consumers\m3-gate\m3-gate-consumer.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-323ed7bf89134d9aa2a3459e1cae9402\repository\tests\consumers\m3-gate\Program.cs(205,10): error CS8629: Nullable value type may be null. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-323ed7bf89134d9aa2a3459e1cae9402\repository\tests\consumers\m3-gate\m3-gate-consumer.csproj]
0 Warning(s)
Payload source: payload.cs:line 365
