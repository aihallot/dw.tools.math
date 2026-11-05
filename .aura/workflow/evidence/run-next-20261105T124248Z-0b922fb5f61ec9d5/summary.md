# Run-next failure evidence

- Invocation: `run-next-20261105T124248Z-0b922fb5f61ec9d5`
- Run: `RS151`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS151 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
baseline-check exit=-532462766
System.IO.InvalidDataException: RS151 Foundation publisher: RS150 must be the observed successful first-pass predecessor
Payload source: payload.cs:line 237
