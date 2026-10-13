# Run-next failure evidence

- Invocation: `run-next-20261013T191524Z-bdd256a83baa71f8`
- Run: `RS050`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS050 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:workflow-hash-recalculation] payload.cs:274: Payload-local SHA-256 recalculation over known workflow authority duplicates identities already owned by compiled operations. Consume PayloadOperationResult hashes or typed workflow/revision/workspace identities instead of recalculating workflow authority hashes. The source repository remains unchanged and no durable attempt was created.
