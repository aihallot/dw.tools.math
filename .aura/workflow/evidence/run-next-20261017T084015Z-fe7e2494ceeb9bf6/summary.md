# Run-next failure evidence

- Invocation: `run-next-20261017T084015Z-fe7e2494ceeb9bf6`
- Run: `RS079`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS079 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
Independent Math candidate dotnet run --file docs/planning/ValidateM5PublicationBoundary.cs -- --check failed with -532462766:
System.InvalidOperationException: M5 local publication boundary: Only local bounded docs may be read: .aura/workflow/plan/project.json
Payload source: payload.cs:line 340
