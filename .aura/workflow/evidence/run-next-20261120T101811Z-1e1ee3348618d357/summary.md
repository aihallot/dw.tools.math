# Run-next failure evidence

- Invocation: `run-next-20261120T101811Z-1e1ee3348618d357`
- Run: `RS267`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS267 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
JSON value differs at pointer '/plan_version' in docs/planning/backlog.json.
Signature: PayloadOperationResult Require.JsonValueEquals(string relativePath, string jsonPointer, JsonNode? expectedValue)
Payload source: PayloadContext.cs:line 469
