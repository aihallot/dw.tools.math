# Run-next failure evidence

- Invocation: `run-next-20261117T115911Z-f03a5795fc1b5d53`
- Run: `RS240`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS240 failed before attempt allocation. classification=validation:m8-wsl-net10-net11-host-readiness. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS240: Default WSL SDK selection is not 11.0.100.
   at Program.<<Main>$>g__Require|0_5(Boolean condition, String message) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-227a864d2ff94c7d9f008ad0a1a6baf7\repository\docs\engineering\ProbeM8WslNet11Host.cs:line 63
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-227a864d2ff94c7d9f008ad0a1a6baf7\repository\docs\engineering\ProbeM8WslNet11Host.cs:line 50
