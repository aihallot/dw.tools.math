# Run-next failure evidence

- Invocation: `run-next-20261018T184918Z-5024bb4873fb21aa`
- Run: `RS094`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS094 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Expected semantic text was not found in tests/consumers/m6-w02-c03-s01-t1/Program.cs.
Signature: PayloadOperationResult Require.TextContains(string relativePath, string value, StringComparison comparison = StringComparison.Ordinal, bool normalizeLineEndings = false, bool trimOuterWhitespace = false)
Payload source: PayloadContext.cs:line 419
