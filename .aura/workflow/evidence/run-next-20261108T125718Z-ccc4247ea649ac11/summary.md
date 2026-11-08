# Run-next failure evidence

- Invocation: `run-next-20261108T125718Z-ccc4247ea649ac11`
- Run: `RS166`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS166 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Required missing path already exists: tools/dw.tools.math.nuget-composition-release/Program.cs
Signature: PayloadOperationResult Require.FileMissing(string relativePath)
Payload source: PayloadContext.cs:line 328
