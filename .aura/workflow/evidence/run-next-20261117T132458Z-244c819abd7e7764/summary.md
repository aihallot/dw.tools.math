# Run-next failure evidence

- Invocation: `run-next-20261117T132458Z-244c819abd7e7764`
- Run: `RS241`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS241 failed before attempt allocation. classification=validation:m8-wsl-linux-net11-quantities-source-consumer. The source repository remains unchanged and no durable attempt was created.
Standard output:
<empty>
Standard error:
Unhandled exception. System.InvalidOperationException: RS241 WSL failed: --exec bash -c cd "$1" && shift && dotnet "$@" bash /mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5/repository/build/artifacts/m8-rs241-wsl-quantities-source --version
10.0.115 [/usr/lib/dotnet/sdk]

The command could not be loaded, possibly because:
  * You intended to execute a .NET application:
      The application '--version' does not exist or is not a managed .dll or .exe.
  * You intended to execute a .NET SDK command:
      A compatible .NET SDK was not found.

Requested SDK version: 11.0.100
global.json file: /mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5/repository/build/artifacts/m8-rs241-wsl-quantities-source/global.json

Installed SDKs:

Install the [11.0.100] .NET SDK or update [/mnt/e/data.temp/fhallot.temp/dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5/repository/build/artifacts/m8-rs241-wsl-quantities-source/global.json] to match an installed SDK.

Learn about SDK resolution:
https://aka.ms/dotnet/sdk-not-found

   at Program.<<Main>$>g__Wsl|0_2(String[] arguments) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5\repository\docs\engineering\ProbeM8WslNet11QuantitiesConsumer.cs:line 72
   at Program.<<Main>$>g__RunLinux|0_3(String[] arguments, <>c__DisplayClass0_0&) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5\repository\docs\engineering\ProbeM8WslNet11QuantitiesConsumer.cs:line 86
   at Program.<Main>$(String[] args) in E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-268872291ec7486dab490d78943d3aa5\repository\docs\engineering\ProbeM8WslNet11QuantitiesConsumer.cs:line 88
