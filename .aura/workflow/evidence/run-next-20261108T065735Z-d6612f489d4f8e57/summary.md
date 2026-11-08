# Run-next failure evidence

- Invocation: `run-next-20261108T065735Z-d6612f489d4f8e57`
- Run: `RS165`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS165 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Expected semantic text was not found in docs/distribution/nuget-federation-f32-composition-publisher.md.
Signature: PayloadOperationResult Require.TextContains(string relativePath, string value, StringComparison comparison = StringComparison.Ordinal, bool normalizeLineEndings = false, bool trimOuterWhitespace = false)
Payload source: PayloadContext.cs:line 419
