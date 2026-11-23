# Run-next failure evidence

- Invocation: `run-next-20261123T122552Z-a4a1bf0f6903c47f`
- Run: `RS297`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS297 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Project node 'M8-W01-C01-T2-V' state is 'ready', expected exactly 'not-ready'.
Signature: PayloadOperationResult ProjectPlan.RequireNodeState(string nodeId, string expectedState)
Payload source: PayloadProjectPlan.cs:line 67
