# Run-next failure evidence

- Invocation: `run-next-20261016T055857Z-3a83c2fd4b09cc63`
- Run: `RS068`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS068 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
JSON value differs at pointer '/status' in docs/planning/evidence/M5-W01-C02-T1-contract-qualified.json.
Signature: PayloadOperationResult Require.JsonValueEquals(string relativePath, string jsonPointer, JsonNode? expectedValue)
Payload source: PayloadContext.cs:line 469
