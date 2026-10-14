# Run-next failure evidence

- Invocation: `run-next-20261014T204844Z-416721c6a3616bc1`
- Run: `RS057`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS057 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
JSON pointer was not found in docs/planning/evidence/M4-W01-C03-T1-contract-qualified.json: /new_green_tests_passed
Signature: PayloadOperationResult Require.JsonValueEquals(string relativePath, string jsonPointer, JsonNode? expectedValue)
Payload source: PayloadContext.cs:line 462
