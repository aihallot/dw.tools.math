# Run-next failure evidence

- Invocation: `run-next-20261008T114208Z-3e29aa83b60be129`
- Run: `RS015`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS015 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~M1W01C03Tests.FahrenheitAndCelsiusAbsoluteConversionsAreExact --logger console;verbosity=normal exited 1: Test run for E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-d30a8ba8d38b4551b7db966287127706\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.
Failed FahrenheitAndCelsiusAbsoluteConversionsAreExact [24 ms]
Error Message:
Payload source: payload.cs:line 223
