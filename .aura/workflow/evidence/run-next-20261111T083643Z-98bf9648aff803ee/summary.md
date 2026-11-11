# Run-next failure evidence

- Invocation: `run-next-20261111T083643Z-98bf9648aff803ee`
- Run: `RS178`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS178 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Expected baseline SHA-256 must be a canonical lowercase 64-character hexadecimal value.
Signature: PayloadOperationResult Files.ConvergeFromStaged(string stagedRelativePath, string destinationRelativePath, string expectedBaselineSha256)
Payload source: PayloadContext.cs:line 568
