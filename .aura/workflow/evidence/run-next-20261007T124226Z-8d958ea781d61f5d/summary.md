# Run-next failure evidence

- Invocation: `run-next-20261007T124226Z-8d958ea781d61f5d`
- Run: `RS001`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS001 failed before attempt allocation. classification=validation-mutation:planning-structure. Preparation-safe validation 'planning-structure' changed the disposable repository target. Preparation-safe validations must be repository-observational; generators, canonical renderers, migrations, and intended repository mutations belong in the payload. Mutation is refused even when the changed path is inside the declared mutation boundary. The source repository remains unchanged and no durable attempt was created.
