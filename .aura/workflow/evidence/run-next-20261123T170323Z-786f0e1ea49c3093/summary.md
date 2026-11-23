# Run-next failure evidence

- Invocation: `run-next-20261123T170323Z-786f0e1ea49c3093`
- Run: `RS301`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS301 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Project node 'M8-W01-C02-T1' must be effectively ready before continuation activation; found not-ready.
Signature: PayloadOperationResult ProjectPlan.ActivateReadyContinuation(string leafId)
---> Dw.Tools.Workflow.WorkflowException: Project node 'M8-W01-C02-T1' must be effectively ready before continuation activation; found not-ready.
Payload source: PayloadProjectPlan.cs:line 246
