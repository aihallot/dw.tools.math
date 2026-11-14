# Run-next failure evidence

- Invocation: `run-next-20261114T220821Z-ee436a0555a67a4e`
- Run: `RS217`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS217 failed before attempt allocation. classification=validation:m8-thirteen-package-dependency-topology. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS217: Unexpected direct dependency edge count.
   at Program.<<Main>$>g__Require|0_5(Boolean condition, String message) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a81c8e5330bc42c0bccdee33d11e17e7\repository\docs\engineering\InspectM8Net11PackageGraph.cs:line 86
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-a81c8e5330bc42c0bccdee33d11e17e7\repository\docs\engineering\InspectM8Net11PackageGraph.cs:line 56
