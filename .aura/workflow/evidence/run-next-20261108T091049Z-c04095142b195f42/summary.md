# Run-next failure evidence

- Invocation: `run-next-20261108T091049Z-c04095142b195f42`
- Run: `RS165`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS165 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
baseline-check exit=-532462766
System.IO.InvalidDataException: RS165 Composition publisher: RS164 exact 17,101-byte Composition candidate or dependency closure changed
Payload source: payload.cs:line 288
