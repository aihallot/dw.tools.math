# Run-next failure evidence

- Invocation: `run-next-20261121T111522Z-760569d1db6ef0fa`
- Run: `RS274`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS274 failed before attempt allocation. classification=validation:m9-assessa-private-package-handoff-durable-identities. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS274: External contract boundaries or native-plan caveat missing.
   at Program.<<Main>$>g__Need|0_0(Boolean value, String reason) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-07d1d2d1a2674259beeaab3551550bac\repository\docs\engineering\ValidateM9AssessaPublishedHandoff.cs:line 14
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-07d1d2d1a2674259beeaab3551550bac\repository\docs\engineering\ValidateM9AssessaPublishedHandoff.cs:line 33
