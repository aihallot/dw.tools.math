# Run-next failure evidence

- Invocation: `run-next-20261015T051017Z-2a674e4d476e0da1`
- Run: `RS058`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS058 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet run --file docs/planning/ValidatePlan.cs -- --write exited 1:
planning invalid: Missing array evidence
Payload source: payload.cs:line 471
