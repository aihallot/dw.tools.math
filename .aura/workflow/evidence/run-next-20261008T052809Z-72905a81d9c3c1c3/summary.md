# Run-next failure evidence

- Invocation: `run-next-20261008T052809Z-72905a81d9c3c1c3`
- Run: `RS008`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS008 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:filesystem-mutation] payload.cs:66: Direct directory mutation reimplements repository mutation outside the compiled Payload SDK. Use bounded staged installation or another existing typed Payload SDK operation; recursive directory mutation is not a payload escape hatch.
[payload-authoring:filesystem-mutation] payload.cs:67: Direct filesystem mutation reimplements repository mutation outside the compiled Payload SDK. Use Payload Files/Text/Json/ProjectPlan/Workspace operations or another existing typed Payload SDK operation. The source repository remains unchanged and no durable attempt was created.
