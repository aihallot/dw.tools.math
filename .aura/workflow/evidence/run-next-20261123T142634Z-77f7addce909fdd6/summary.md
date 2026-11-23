# Run-next failure evidence

- Invocation: `run-next-20261123T142634Z-77f7addce909fdd6`
- Run: `RS299`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS299 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Project node 'M8-W01-C01-T2' state is 'done', expected exactly 'in-progress'.
Signature: PayloadOperationResult ProjectPlan.RequireNodeState(string nodeId, string expectedState)
Payload source: PayloadProjectPlan.cs:line 67
