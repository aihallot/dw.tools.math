# Run-next failure evidence

- Invocation: `run-next-20261109T145841Z-736043888aef899d`
- Run: `RS170`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS170 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
tool-build exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-03293331a57d4b2aba651ff9d1acb187\repository\tools\dw.tools.math.nuget-numerics-release\Program.cs(197,29): error CS7036: There is no argument given that corresponds to the required parameter 'token' of 'DotNetLocalPackageRepository.PreparePublication(string, string, CancellationToken, bool)' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-03293331a57d4b2aba651ff9d1acb187\repository\tools\dw.tools.math.nuget-numerics-release\dw.tools.math.nuget-numerics-release.csproj]
Build FAILED.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-03293331a57d4b2aba651ff9d1acb187\repository\tools\dw.tools.math.nuget-numerics-release\Program.cs(197,29): error CS7036: There is no argument given that corresponds to the required parameter 'token' of 'DotNetLocalPackageRepository.PreparePublication(string, string, CancellationToken, bool)' [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-03293331a57d4b2aba651ff9d1acb187\repository\tools\dw.tools.math.nuget-numerics-release\dw.tools.math.nuget-numerics-release.csproj]
Payload source: payload.cs:line 188
