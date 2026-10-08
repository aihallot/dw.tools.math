# Run-next failure evidence

- Invocation: `run-next-20261008T060537Z-c82d97b3a835b8e6`
- Run: `RS008`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS008 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
M1-W01-C01 GREEN invalid: ExactRational GREEN tests failed:
Test run for E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-f520ea49fb86403fa6ceafc9adc553ef\repository\build\artifacts\bin\dw.quantities.tests\Release\net10.0\dw.quantities.tests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.
Failed ExactRationalTypeIsMaterialized [69 ms]
Payload source: payload.cs:line 182
