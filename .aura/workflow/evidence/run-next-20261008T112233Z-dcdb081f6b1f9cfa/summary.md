# Run-next failure evidence

- Invocation: `run-next-20261008T112233Z-dcdb081f6b1f9cfa`
- Run: `RS015`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS015 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
dotnet exited 1: Test run for E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-2536d5aa59104e6288822c473293b090\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.
Passed ThreeAuthorizedPublicTypesAreInstalled [12 ms]
Passed AreaOfThreeMetresByFourMetresIsTwelveSquareMetres [3 ms]
Payload source: payload.cs:line 208
