# Run-next failure evidence

- Invocation: `run-next-20261008T214352Z-6299c3772798e646`
- Run: `RS024`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS024 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-e0fce12d210b47fcab869acc777b5113\repository\.workflow\runs\RS024\payload.cs.csproj (in 351 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-e0fce12d210b47fcab869acc777b5113\repository\.workflow\runs\RS024\payload.cs(11,14): error CS0219: The variable 'FirstRed' is assigned but its value is never used
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-e0fce12d210b47fcab869acc777b5113\repository\.workflow\runs\RS024\payload.cs(12,14): error CS0219: The variable 'SecondRed' is assigned but its value is never used
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
