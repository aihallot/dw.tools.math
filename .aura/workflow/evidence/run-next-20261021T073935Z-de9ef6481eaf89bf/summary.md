# Run-next failure evidence

- Invocation: `run-next-20261021T073935Z-de9ef6481eaf89bf`
- Run: `RS124`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS124 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:filesystem-mutation] payload.cs:119: Direct directory mutation reimplements repository mutation outside the compiled Payload SDK. Use bounded staged installation or another existing typed Payload SDK operation; recursive directory mutation is not a payload escape hatch.
[payload-authoring:jsonarray-aot-unsafe-add] payload.cs:154: JsonArray.Add can bind the generic Add<T> overload and trigger IL2026/IL3050 under the strict AOT compilation gate. Payload SDK AOT-safe JSON array hint:
  Avoid JsonArray.Add<T>(T) for payload-authored arrays; it can trigger IL2026/IL3050 under the strict compilation gate.
  JsonArray Json.StringArray(String[] values)
  JsonArray Json.Array(JsonNode[] nodes)
[payload-authoring:workflow-hash-recalculation] payload.cs:219: Payload-local SHA-256 recalculation over known workflow authority duplicates identities already owned by compiled operations. Consume PayloadOperationResult hashes or typed workflow/revision/workspace identities instead of recalculating workflow authority hashes. The source repository remains unchanged and no durable attempt was created.
