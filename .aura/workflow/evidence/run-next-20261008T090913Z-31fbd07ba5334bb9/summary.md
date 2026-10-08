# Run-next failure evidence

- Invocation: `run-next-20261008T090913Z-31fbd07ba5334bb9`
- Run: `RS009`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS009 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Project node 'M1-W01-C01-T2-R' state is 'done', neither explicit baseline 'ready' nor target 'in-progress'.
Signature: PayloadOperationResult ProjectPlan.TransitionNode(string nodeId, string baselineState, string targetState)
Payload source: PayloadProjectPlan.cs:line 453
