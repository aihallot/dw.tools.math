# Run-next failure evidence

- Invocation: `run-next-20261127T181002Z-f04a16ca27e826cc`
- Run: `RS406`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS406 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Workflow report correction mode mismatch: expected False, found True.
Signature: PayloadOperationResult Workflow.RequirePriorSuccess(string runId, int attempt, string commitMessage, string executorToolVersion, int priorInvocationFailureCount, int preparationFailureCount, int allocatedAttemptFailureCount, bool firstPassSucceeded, bool expectedCorrectionMode = false, Nullable<int> expectedCorrectionSourceAttempt = null)
---> Dw.Tools.Workflow.WorkflowException: Workflow report correction mode mismatch: expected False, found True.
Payload source: PayloadWorkflow.cs:line 147
