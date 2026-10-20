# Run-next failure evidence

- Invocation: `run-next-20261020T191339Z-19db4fefa8c880a9`
- Run: `RS120`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS120 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS120 Math source-of-truth coverage validation failed: dotnet run --file docs/planning/ValidateM7CoverageInventory.cs -- --check
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7a69320736c248f2afd3fecf31b8a3a3\repository\docs\planning\ValidateM7CoverageInventory.cs(25,14): error CS0219: The variable 'CountsMarker' is assigned but its value is never used
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7a69320736c248f2afd3fecf31b8a3a3\repository\docs\planning\ValidateM7CoverageInventory.cs(26,14): error CS0219: The variable 'ScopeMarker' is assigned but its value is never used
The build failed. Fix the build errors and run again.
Payload source: payload.cs:line 324
