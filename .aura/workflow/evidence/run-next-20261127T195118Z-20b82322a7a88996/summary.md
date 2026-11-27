# Run-next failure evidence

- Invocation: `run-next-20261127T195118Z-20b82322a7a88996`
- Run: `RS412`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS412 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Process status: timedOut=false cancelled=false exitCode=1 workflowExitCode=1 terminationAttempted=false terminationErrorPresent=false timeoutMilliseconds=300000 durationMilliseconds=4312
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4f3bb6acb64a4f84a41d12e1661126ba\repository\.workflow\runs\RS412\payload.cs.csproj (in 490 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4f3bb6acb64a4f84a41d12e1661126ba\repository\.workflow\runs\RS412\payload.cs(33,111): error IL2026: Using member 'System.Text.Json.Nodes.JsonArray.Add<T>(T)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Creating JsonValue instances with non-primitive types is not compatible with trimming. It can result in non-primitive types being serialized, which may have their members trimmed.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-4f3bb6acb64a4f84a41d12e1661126ba\repository\.workflow\runs\RS412\payload.cs(33,111): error IL3050: Using member 'System.Text.Json.Nodes.JsonArray.Add<T>(T)' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. Creating JsonValue instances with non-primitive types requires generating code at runtime.
Standard error:
<empty>
Payload SDK AOT-safe JSON array hint:
  Avoid JsonArray.Add<T>(T) for payload-authored arrays; it can trigger IL2026/IL3050 under the strict compilation gate.
  JsonArray Json.StringArray(String[] values)
  JsonArray Json.Array(JsonNode[] nodes) The source repository remains unchanged and no durable attempt was created.
