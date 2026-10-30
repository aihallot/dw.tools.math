# Run-next failure evidence

- Invocation: `run-next-20261030T212257Z-5d0ec8e0c2f08abd`
- Run: `RS133`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS133 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS133 neutral-restore failed exit=1
Determining projects to restore...
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-59aa7c6f35e243bd8a43bd6dfedc8e70\repository\docs\discovery\qualification\RS133-neutral-consumers\neutral.csproj : error NU1008: The following PackageReference items cannot define a value for Version: dw.discovery. Projects using Central Package Management must define a Version value on a PackageVersion item. For more information, visit https://aka.ms/nuget/cpm/gettingstarted
Failed to restore E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-59aa7c6f35e243bd8a43bd6dfedc8e70\repository\docs\discovery\qualification\RS133-neutral-consumers\neutral.csproj (in 53 ms).
Payload source: payload.cs:line 316
