# Run-next failure evidence

- Invocation: `run-next-20261014T083411Z-20893c7d99ae2ed9`
- Run: `RS052`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS052 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-06bce80c0fd2463c934beb620360b518\repository\.workflow\runs\RS052\payload.cs.csproj (in 517 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-06bce80c0fd2463c934beb620360b518\repository\.workflow\runs\RS052\payload.cs(162,13): error CS0136: A local or parameter named 'output' cannot be declared in this scope because that name is used in an enclosing local scope to define a local or parameter
Standard error:
<empty> The source repository remains unchanged and no durable attempt was created.
