# Run-next failure evidence

- Invocation: `run-next-20261010T064612Z-b9c4e4817b5941e3`
- Run: `RS038`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS038 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7f47d9f807104732990ebba59866b452\repository\.workflow\runs\RS038\payload.cs.csproj (in 540 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-7f47d9f807104732990ebba59866b452\repository\.workflow\runs\RS038\payload.cs(146,56): error CS0119: 'Version' is a type, which is not valid in the given context
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
