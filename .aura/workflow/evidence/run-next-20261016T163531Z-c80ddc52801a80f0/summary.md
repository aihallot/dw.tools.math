# Run-next failure evidence

- Invocation: `run-next-20261016T163531Z-c80ddc52801a80f0`
- Run: `RS075`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS075 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-2e28017724df45328d39b2dc35ffaf16\repository\.workflow\runs\RS075\payload.cs.csproj (in 240 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-2e28017724df45328d39b2dc35ffaf16\repository\.workflow\runs\RS075\payload.cs(29,14): error CS0219: The variable 'SampleSource' is assigned but its value is never used
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
