# Run-next failure evidence

- Invocation: `run-next-20261007T123937Z-7b97f9d3bb9d5e82`
- Run: `RS001`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS001 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7a16c4c54418456fa9f6b0262855155c\repository\.workflow\runs\RS001\payload.cs.csproj (in 221 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7a16c4c54418456fa9f6b0262855155c\repository\.workflow\runs\RS001\payload.cs(3,15): error CS0103: The name 'PayloadContext' does not exist in the current context
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
