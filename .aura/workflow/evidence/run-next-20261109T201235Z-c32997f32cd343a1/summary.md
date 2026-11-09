# Run-next failure evidence

- Invocation: `run-next-20261109T201235Z-c32997f32cd343a1`
- Run: `RS172`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS172 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:workflow-json-traversal] payload.cs:70: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:75: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-hash-recalculation] payload.cs:92: Payload-local SHA-256 recalculation over known workflow authority duplicates identities already owned by compiled operations. Consume PayloadOperationResult hashes or typed workflow/revision/workspace identities instead of recalculating workflow authority hashes.
[payload-authoring:workflow-json-traversal] payload.cs:99: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON. The source repository remains unchanged and no durable attempt was created.
