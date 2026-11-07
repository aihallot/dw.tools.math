# Run-next failure evidence

- Invocation: `run-next-20261107T202113Z-99236f3a79853ffa`
- Run: `RS163`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS163 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
source-check exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6ff7a3fdef994797b215ae543bc0c71c\repository\docs\distribution\ValidateNugetCodecsPublishedFeed.cs(30,13): error CS8321: The local function 'IsSha' is declared but never used
The build failed. Fix the build errors and run again.
Payload source: payload.cs:line 259
