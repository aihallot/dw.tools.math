# Run-next failure evidence

- Invocation: `run-next-20261021T112728Z-445d4791e3a2969d`
- Run: `RS127`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS127 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS127 local command failure: baseline-complete-package-check exit=-532462766
System.InvalidOperationException: Missing csproj property TargetFramework
Payload source: payload.cs:line 280
