# Run-next failure evidence

- Invocation: `run-next-20261018T084648Z-9a3b2ca8180b4646`
- Run: `RS092`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS092 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-1d3e975b0b25453db5e67a129af97b18\repository\.workflow\runs\RS092\payload.cs.csproj (in 249 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-1d3e975b0b25453db5e67a129af97b18\repository\.workflow\runs\RS092\payload.cs(17,11): error CS0219: The variable 'PriorNodes' is assigned but its value is never used
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-1d3e975b0b25453db5e67a129af97b18\repository\.workflow\runs\RS092\payload.cs(18,14): error CS0219: The variable 'NextGeometry' is assigned but its value is never used
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
