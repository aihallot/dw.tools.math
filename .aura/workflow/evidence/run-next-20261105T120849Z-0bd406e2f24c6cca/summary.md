# Run-next failure evidence

- Invocation: `run-next-20261105T120849Z-0bd406e2f24c6cca`
- Run: `RS151`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS151 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Expected semantic text was not found in docs/distribution/nuget-federation-f18-foundation-publisher.md.
Signature: PayloadOperationResult Require.TextContains(string relativePath, string value, StringComparison comparison = StringComparison.Ordinal, bool normalizeLineEndings = false, bool trimOuterWhitespace = false)
Payload source: PayloadContext.cs:line 419
