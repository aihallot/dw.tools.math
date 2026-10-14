# Run-next failure evidence

- Invocation: `run-next-20261014T103409Z-69b8a9d75d97f4e4`
- Run: `RS053`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS053 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Ready continuation activation requires an existing active project hierarchy.
Signature: PayloadOperationResult ProjectPlan.ActivateReadyContinuation(string leafId)
---> Dw.Tools.Workflow.WorkflowException: Ready continuation activation requires an existing active project hierarchy.
Payload source: PayloadProjectPlan.cs:line 246
