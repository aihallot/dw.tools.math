# Run-next failure evidence

- Invocation: `run-next-20261008T185353Z-b32f3768631ad9c3`
- Run: `RS021`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS021 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-de61a7f589a74f34a04720a64514ca5f\repository\.workflow\runs\RS021\payload.cs.csproj (in 524 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-de61a7f589a74f34a04720a64514ca5f\repository\.workflow\runs\RS021\payload.cs(141,93): error CS1003: Syntax error, '=' expected
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
