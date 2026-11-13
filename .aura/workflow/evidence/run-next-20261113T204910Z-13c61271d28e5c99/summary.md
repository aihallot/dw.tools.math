# Run-next failure evidence

- Invocation: `run-next-20261113T204910Z-13c61271d28e5c99`
- Run: `RS201`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS201 failed before attempt allocation. classification=validation:m8-skill-and-manifest-determinism. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: Math skill: Bundled manifest is missing or differs from the qualified source bytes.
   at Program.<<Main>$>g__Require|0_2(Boolean value, String message) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-452f7f9619aa4df9883e173331b7b5b8\repository\docs\discovery\qualification\GenerateM8MathSkill.cs:line 19
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-452f7f9619aa4df9883e173331b7b5b8\repository\docs\discovery\qualification\GenerateM8MathSkill.cs:line 91
