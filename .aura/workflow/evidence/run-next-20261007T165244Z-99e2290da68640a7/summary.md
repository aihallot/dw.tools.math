# Run-next failure evidence

- Invocation: `run-next-20261007T165244Z-99e2290da68640a7`
- Run: `RS003`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS003 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
planning invalid: Subtask active under inactive task
System.InvalidOperationException: dotnet planning generation failed with exit code 1.
Payload source: payload.cs:line 147
