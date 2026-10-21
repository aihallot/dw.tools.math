# Run-next failure evidence

- Invocation: `run-next-20261021T123617Z-a564643520c57d95`
- Run: `RS127`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS127 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS127 local command failure: target-complete-package-check exit=-532462766
System.InvalidOperationException: RS127 local Math package qualification: A required local command failed or is absent: source-locked-restore
Payload source: payload.cs:line 280
