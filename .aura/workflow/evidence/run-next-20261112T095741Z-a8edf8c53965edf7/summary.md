# Run-next failure evidence

- Invocation: `run-next-20261112T095741Z-a8edf8c53965edf7`
- Run: `RS180`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS180 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
  Determining projects to restore...
  Restored E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6a7f5e38057f40adb6fda0cbc2d6306e\repository\.workflow\runs\RS180\payload.cs.csproj (in 213 ms).
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6a7f5e38057f40adb6fda0cbc2d6306e\repository\.workflow\runs\RS180\payload.cs(21,12): error IL2026: Using member 'System.Reflection.Assembly.GetType(String)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types might be removed by trimming. If the type name is a string literal, consider using Type.GetType instead.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6a7f5e38057f40adb6fda0cbc2d6306e\repository\.workflow\runs\RS180\payload.cs(22,12): error IL2026: Using member 'System.Reflection.Assembly.GetType(String)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types might be removed by trimming. If the type name is a string literal, consider using Type.GetType instead.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6a7f5e38057f40adb6fda0cbc2d6306e\repository\.workflow\runs\RS180\payload.cs(23,22): error IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicMethods' in call to 'System.Type.GetMethods(BindingFlags)'. The return value of method 'System.Reflection.Assembly.GetType(String)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-6a7f5e38057f40adb6fda0cbc2d6306e\repository\.workflow\runs\RS180\payload.cs(26,20): error IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicMethods' in call to 'System.Type.GetMethods(BindingFlags)'. The return value of method 'System.Reflection.Assembly.GetType(String)' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
Standard error:
<empty>
Payload SDK signature hints:
  PayloadContext Context.Create(string? repositoryRoot = null, string? runRoot = null, string? operationLogPath = null, string? artifactRoot = null) The source repository remains unchanged and no durable attempt was created.
