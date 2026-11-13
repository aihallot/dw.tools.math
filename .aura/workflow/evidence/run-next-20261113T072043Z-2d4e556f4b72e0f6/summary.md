# Run-next failure evidence

- Invocation: `run-next-20261113T072043Z-2d4e556f4b72e0f6`
- Run: `RS192`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS192 failed before attempt allocation. classification=validation:m8-inspection-self-test. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.IO.InvalidDataException: RS192: Explain determinism or qualified metadata.
   at Program.<<Main>$>g__Require|0_1(Boolean condition, String reason) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-fbbb2011a8954ec2a7d2be9b41c1a676\repository\docs\discovery\qualification\DescribeM8Discovery.cs:line 16
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-fbbb2011a8954ec2a7d2be9b41c1a676\repository\docs\discovery\qualification\DescribeM8Discovery.cs:line 96
