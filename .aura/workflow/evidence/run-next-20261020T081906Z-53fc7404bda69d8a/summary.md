# Run-next failure evidence

- Invocation: `run-next-20261020T081906Z-53fc7404bda69d8a`
- Run: `RS110`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS110 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS110 local DWF planning failed: dotnet run --file docs/planning/ValidateM7DagSplit.cs -- --check
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-0410bfa7f24a410fa50ba9692e9f3b21\repository\docs\planning\ValidateM7DagSplit.cs(9,1): error CS0161: '<top-level-statements-entry-point>': not all code paths return a value
The build failed. Fix the build errors and run again.
Payload source: payload.cs:line 362
