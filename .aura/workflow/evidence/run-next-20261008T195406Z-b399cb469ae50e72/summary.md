# Run-next failure evidence

- Invocation: `run-next-20261008T195406Z-b399cb469ae50e72`
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
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-06267fb9734b4bac9c561c48efd41984\repository\.workflow\runs\RS021\payload.cs.csproj (in 839 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-06267fb9734b4bac9c561c48efd41984\repository\.workflow\runs\RS021\payload.cs(182,1): error CS0103: The name 'RunRequired' does not exist in the current context
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
