# Run-next failure evidence

- Invocation: `run-next-20261111T091439Z-4ab6c665a6d89aa6`
- Run: `RS178`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS178 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
Required missing path already exists: docs/coordination/requests/MATH-XR-006-aura-electrochemical-unit-profile.md
Signature: PayloadOperationResult Require.FileMissing(string relativePath)
Payload source: PayloadContext.cs:line 328
