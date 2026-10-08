# Run-next failure evidence

- Invocation: `run-next-20261008T084229Z-86111a16a173368b`
- Run: `RS009`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS009 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5950bdcc75d543a080b56143fdd7155c\repository\.workflow\runs\RS009\payload.cs.csproj (in 246 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5950bdcc75d543a080b56143fdd7155c\repository\.workflow\runs\RS009\payload.cs(53,9): error IL2026: Using member 'System.Text.Json.Nodes.JsonArray.Add<T>(T)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Creating JsonValue instances with non-primitive types is not compatible with trimming. It can result in non-primitive types being serialized, which may have their members trimmed.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-5950bdcc75d543a080b56143fdd7155c\repository\.workflow\runs\RS009\payload.cs(53,9): error IL3050: Using member 'System.Text.Json.Nodes.JsonArray.Add<T>(T)' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. Creating JsonValue instances with non-primitive types requires generating code at runtime.
Standard error:
<empty>
Payload SDK AOT-safe JSON array hint:
  Avoid JsonArray.Add<T>(T) for payload-authored arrays; it can trigger IL2026/IL3050 under the strict compilation gate.
  JsonArray Json.StringArray(String[] values)
  JsonArray Json.Array(JsonNode[] nodes) The source repository remains unchanged and no durable attempt was created.
