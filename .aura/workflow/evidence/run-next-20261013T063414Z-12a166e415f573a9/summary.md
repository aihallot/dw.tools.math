# Run-next failure evidence

- Invocation: `run-next-20261013T063414Z-12a166e415f573a9`
- Run: `RS043`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS043 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Workflow state activeWorkPackage mismatch: expected '<null>', found 'M3-W01'.
Signature: PayloadOperationResult Workflow.RequireCurrentRun(string runId, string? expectedMilestoneId, string? expectedWorkPackageId, string? expectedPhaseId, string? expectedTaskId, string? expectedSubtaskId)
Payload source: PayloadWorkflow.cs:line 229
