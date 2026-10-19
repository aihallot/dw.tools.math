# Run-next failure evidence

- Invocation: `run-next-20261019T111851Z-e092c2a028634bc9`
- Run: `RS103`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS103 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed authoring preflight before compilation and attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
[payload-authoring:workflow-json-traversal] payload.cs:161: Hand-written JSON traversal over known workflow authority duplicates typed workflow contracts. Use ProjectPlan, Workspace, Workflow, Require, or another bounded compiled Payload operation for canonical workflow JSON.
[payload-authoring:jsonarray-aot-unsafe-add] payload.cs:212: JsonArray.Add can bind the generic Add<T> overload and trigger IL2026/IL3050 under the strict AOT compilation gate. Payload SDK AOT-safe JSON array hint:
  Avoid JsonArray.Add<T>(T) for payload-authored arrays; it can trigger IL2026/IL3050 under the strict compilation gate.
  JsonArray Json.StringArray(String[] values)
  JsonArray Json.Array(JsonNode[] nodes) The source repository remains unchanged and no durable attempt was created.
