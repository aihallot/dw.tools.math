# Run-next failure evidence

- Invocation: `run-next-20261014T202753Z-567ce0c7edafefa8`
- Run: `RS057`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS057 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet build tests/consumers/m4-w01-c03-t1/m4-exact-transform-consumer.csproj -c Release --no-restore exited 1: error CS8602: Dereference of a possibly null reference. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-97731a9d11404fc5942dfa3e5918119b\repository\tests\consumers\m4-w01-c03-t1\m4-exact-transform-consumer.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-97731a9d11404fc5942dfa3e5918119b\repository\tests\consumers\m4-w01-c03-t1\Program.cs(35,40): error CS8602: Dereference of a possibly null reference. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-97731a9d11404fc5942dfa3e5918119b\repository\tests\consumers\m4-w01-c03-t1\m4-exact-transform-consumer.csproj]
0 Warning(s)
Payload source: payload.cs:line 416
