# Run-next failure evidence

- Invocation: `run-next-20261123T110957Z-ea2b46b2e51689d2`
- Run: `RS296`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS296 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Project node 'M8-W01-C01-T2-G' state is 'in-progress', expected exactly 'ready'.
Signature: PayloadOperationResult ProjectPlan.RequireNodeState(string nodeId, string expectedState)
Payload source: PayloadProjectPlan.cs:line 67
