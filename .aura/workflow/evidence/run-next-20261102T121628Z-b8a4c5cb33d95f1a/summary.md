# Run-next failure evidence

- Invocation: `run-next-20261102T121628Z-b8a4c5cb33d95f1a`
- Run: `RS134`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS134 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
build exited 1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-ed97b827c36a46eeae5c003e0dc9c98e\repository\tools\dw.tools.math.nuget-preflight\Program.cs(107,46): error CS8604: Possible null reference argument for parameter 'path' in 'string Path.GetFullPath(string path)'. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-ed97b827c36a46eeae5c003e0dc9c98e\repository\tools\dw.tools.math.nuget-preflight\dw.tools.math.nuget-preflight.csproj]
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-ed97b827c36a46eeae5c003e0dc9c98e\repository\tools\dw.tools.math.nuget-preflight\Program.cs(114,46): error CS8604: Possible null reference argument for parameter 'path1' in 'string Path.Combine(string path1, string path2)'. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-ed97b827c36a46eeae5c003e0dc9c98e\repository\tools\dw.tools.math.nuget-preflight\dw.tools.math.nuget-preflight.csproj]
Build FAILED.
Payload source: payload.cs:line 198
