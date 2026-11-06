# Run-next failure evidence

- Invocation: `run-next-20261106T101031Z-c4980352814d6788`
- Run: `RS157`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS157 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Required file is missing: tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs
Signature: PayloadOperationResult Require.FileExists(string relativePath)
Payload source: PayloadContext.cs:line 320
