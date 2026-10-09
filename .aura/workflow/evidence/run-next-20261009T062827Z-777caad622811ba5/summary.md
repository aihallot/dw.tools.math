# Run-next failure evidence

- Invocation: `run-next-20261009T062827Z-777caad622811ba5`
- Run: `RS027`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS027 failed before attempt allocation. classification=payload-target-reentry. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet run --file docs/planning/ValidateM1Gate.cs -- --check exited 1:
M1 gate invalid: M1 product plan not at expected release-gate version.
Payload source: payload.cs:line 233
