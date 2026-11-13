# Run-next failure evidence

- Invocation: `run-next-20261113T140956Z-57a8cf44d05b4dc7`
- Run: `RS198`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS198 failed before attempt allocation. classification=validation:m8-private-packaged-invocations. The source repository remains unchanged and no durable attempt was created.
Standard output:
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4562d5b6c0864733b64ae826daa82159\repository\docs\discovery\qualification\VerifyM8PrivateBinaryBindings.cs(215,25): error CS0136: A local or parameter named 'context' cannot be declared in this scope because that name is used in an enclosing local scope to define a local or parameter
Standard error:

The build failed. Fix the build errors and run again.
