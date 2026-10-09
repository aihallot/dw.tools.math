# Run-next failure evidence

- Invocation: `run-next-20261009T061851Z-90c4f49768dad911`
- Run: `RS027`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS027 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Ready continuation activation requires an existing active project hierarchy.
Signature: PayloadOperationResult ProjectPlan.ActivateReadyContinuation(string leafId)
---> Dw.Tools.Workflow.WorkflowException: Ready continuation activation requires an existing active project hierarchy.
Payload source: PayloadProjectPlan.cs:line 246
