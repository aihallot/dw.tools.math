# Run-next failure evidence

- Invocation: `run-next-20261126T211313Z-6b84b19eaa528d69`
- Run: `RS367`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS367 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:workflow-json-traversal] payload.cs:49: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:69: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:90: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:166: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:243: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:318: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:327: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:336: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:345: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:workflow-json-traversal] payload.cs:365: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON. The source repository remains unchanged and no durable attempt was created.
