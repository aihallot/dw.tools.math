# Run-next failure evidence

- Invocation: `run-next-20261122T095159Z-61ace35584bc0319`
- Run: `RS281`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS281 failed before attempt allocation. classification=validation:m8-portable-discovery-skill-export-net11-private-package-locked. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS281: Canonical source-generated Discovery identity differs.
   at Program.<<Main>$>g__Require|0_5(Boolean good, String reason) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-63f5ead249e5453e9582b3f6d176c543\repository\docs\discovery\qualification\ExportM8PortableDiscoverySkill.cs:line 337
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-63f5ead249e5453e9582b3f6d176c543\repository\docs\discovery\qualification\ExportM8PortableDiscoverySkill.cs:line 49
