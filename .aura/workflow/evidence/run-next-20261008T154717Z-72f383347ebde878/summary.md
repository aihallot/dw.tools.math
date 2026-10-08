# Run-next failure evidence

- Invocation: `run-next-20261008T154717Z-72f383347ebde878`
- Run: `RS018`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS018 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet restore dw.tools.math.slnx --locked-mode exited 1: error NU1004: The project references dw.quantities.standard whose dependencies has changed.The packages lock file is inconsistent with the project dependencies so restore can't be run in locked mode. Disable the RestoreLockedMode MSBuild property or pass an explicit --force-evaluate option to run restore to update the lock file. [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5e22839791014f808f580c098e72f143\repository\dw.tools.math.slnx]
Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5e22839791014f808f580c098e72f143\repository\src\projects\dw.tools.math.foundation\dw.tools.math.foundation.csproj (in 89 ms).
Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5e22839791014f808f580c098e72f143\repository\src\projects\dw.quantities.standard\dw.quantities.standard.csproj (in 140 ms).
Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5e22839791014f808f580c098e72f143\repository\src\projects\dw.quantities\dw.quantities.csproj (in 89 ms).
Payload source: payload.cs:line 266
