# Run-next failure evidence

- Invocation: `run-next-20261017T164140Z-175ba4465c46df3c`
- Run: `RS082`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS082 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3c22645faa554461ac18f290ac165dd6\repository\.workflow\runs\RS082\payload.cs.csproj (in 208 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3c22645faa554461ac18f290ac165dd6\repository\.workflow\runs\RS082\payload.cs(80,48): error CS1012: Too many characters in character literal
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-3c22645faa554461ac18f290ac165dd6\repository\.workflow\runs\RS082\payload.cs(81,5): error CS1012: Too many characters in character literal
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
