# Run-next failure evidence

- Invocation: `run-next-20261020T181938Z-b9f576d9b7099f98`
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
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-be5ba9ea16294f75818d4ef6e6a562fb\repository\docs\planning\ValidateM7CoverageInventory.cs(270,10): error CS0103: The name 'Get' does not exist in the current context
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-be5ba9ea16294f75818d4ef6e6a562fb\repository\docs\planning\ValidateM7CoverageInventory.cs(286,10): error CS0103: The name 'Get' does not exist in the current context
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-be5ba9ea16294f75818d4ef6e6a562fb\repository\docs\planning\ValidateM7CoverageInventory.cs(287,10): error CS0103: The name 'Get' does not exist in the current context
Payload source: payload.cs:line 324
