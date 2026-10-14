# Run-next failure evidence

- Invocation: `run-next-20261014T050437Z-0fbcc46fc53ad9c2`
- Run: `RS050`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS050 failed before attempt allocation. classification=payload-compilation. The pending C# payload failed compilation preflight before attempt allocation. No attempt was started, no payload was executed, and no repository mutation was authorized.
Standard output:
8.0.426 [C:\Program Files\dotnet\sdk]
9.0.122 [C:\Program Files\dotnet\sdk]
9.0.315 [C:\Program Files\dotnet\sdk]
10.0.113 [C:\Program Files\dotnet\sdk]
10.0.301 [C:\Program Files\dotnet\sdk]
10.0.402 [C:\Program Files\dotnet\sdk]
Standard error:
The command could not be loaded, possibly because:
  * You intended to execute a .NET application:
      The application 'build' does not exist or is not a managed .dll or .exe.
  * You intended to execute a .NET SDK command:
      A compatible .NET SDK was not found.

Requested SDK version: 10.0.401
global.json file: E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-27ba5c5da4384cae8654402af5b7b9b9\repository\global.json

Installed SDKs:

Install the [10.0.401] .NET SDK or update [E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-27ba5c5da4384cae8654402af5b7b9b9\repository\global.json] to match an installed SDK.

Learn about SDK resolution:
https://aka.ms/dotnet/sdk-not-found The source repository remains unchanged and no durable attempt was created.
