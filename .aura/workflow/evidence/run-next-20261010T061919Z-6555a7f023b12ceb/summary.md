# Run-next failure evidence

- Invocation: `run-next-20261010T061919Z-6555a7f023b12ceb`
- Run: `RS038`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS038 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:filesystem-mutation] payload.cs:101: Direct directory mutation reimplements repository mutation outside the compiled Payload SDK. Use bounded staged installation or another existing typed Payload SDK operation; recursive directory mutation is not a payload escape hatch.
[payload-authoring:jsonarray-aot-unsafe-add] payload.cs:126: JsonArray.Add can bind the generic Add<T> overload and trigger IL2026/IL3050 under the strict AOT compilation gate. Payload SDK AOT-safe JSON array hint:
  Avoid JsonArray.Add<T>(T) for payload-authored arrays; it can trigger IL2026/IL3050 under the strict compilation gate.
  JsonArray Json.StringArray(String[] values)
  JsonArray Json.Array(JsonNode[] nodes)
[payload-authoring:filesystem-mutation] payload.cs:137: Direct directory mutation reimplements repository mutation outside the compiled Payload SDK. Use bounded staged installation or another existing typed Payload SDK operation; recursive directory mutation is not a payload escape hatch.
[payload-authoring:filesystem-mutation] payload.cs:141: Direct filesystem mutation reimplements repository mutation outside the compiled Payload SDK. Use Payload Files/Text/Json/ProjectPlan/Workspace operations or another existing typed Payload SDK operation.
[payload-authoring:filesystem-mutation] payload.cs:154: Direct filesystem mutation reimplements repository mutation outside the compiled Payload SDK. Use Payload Files/Text/Json/ProjectPlan/Workspace operations or another existing typed Payload SDK operation.
[payload-authoring:filesystem-mutation] payload.cs:181: Direct directory mutation reimplements repository mutation outside the compiled Payload SDK. Use bounded staged installation or another existing typed Payload SDK operation; recursive directory mutation is not a payload escape hatch. The source repository remains unchanged and no durable attempt was created.
