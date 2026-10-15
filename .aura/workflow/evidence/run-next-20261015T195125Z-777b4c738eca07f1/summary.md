# Run-next failure evidence

- Invocation: `run-next-20261015T195125Z-777b4c738eca07f1`
- Run: `RS067`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS067 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-fb62a0bc04574ccca98c0fdec419887b\repository\.workflow\runs\RS067\payload.cs.csproj (in 212 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-fb62a0bc04574ccca98c0fdec419887b\repository\.workflow\runs\RS067\payload.cs(161,9): error CS0136: A local or parameter named 'observed' cannot be declared in this scope because that name is used in an enclosing local scope to define a local or parameter
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
