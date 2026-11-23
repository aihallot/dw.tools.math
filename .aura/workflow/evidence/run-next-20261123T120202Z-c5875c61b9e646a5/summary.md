# Run-next failure evidence

- Invocation: `run-next-20261123T120202Z-c5875c61b9e646a5`
- Run: `RS297`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS297 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:workflow-json-traversal] payload.cs:6: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:13: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON. The source repository remains unchanged and no durable attempt was created.
