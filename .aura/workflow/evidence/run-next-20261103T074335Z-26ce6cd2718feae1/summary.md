# Run-next failure evidence

- Invocation: `run-next-20261103T074335Z-26ce6cd2718feae1`
- Run: `RS140`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS140 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
tool-build exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\Program.cs(63,14): error CS0103: The name 'Read' does not exist in the current context [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\dw.tools.math.nuget-expression-feed-verify.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\Program.cs(66,14): error CS0103: The name 'Read' does not exist in the current context [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\dw.tools.math.nuget-expression-feed-verify.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\Program.cs(67,14): error CS0103: The name 'Read' does not exist in the current context [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d2cf55586d9b462e81f4f47cab88c3ea\repository\tools\dw.tools.math.nuget-expression-feed-verify\dw.tools.math.nuget-expression-feed-verify.csproj]
Payload source: payload.cs:line 256
