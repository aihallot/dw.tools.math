# Run-next failure evidence

- Invocation: `run-next-20261109T172634Z-bb10da7b98279310`
- Run: `RS171`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `no`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS171 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
source-check exit=1
E:\data.temp\fhallot.temp\dw-tools-workflow-disposable-run-c8320e062e9c42918886a532b2fb08dc\repository\docs\distribution\ValidateNugetNumericsOriginal.cs(24,15): error CS8321: The local function 'Sha' is declared but never used
The build failed. Fix the build errors and run again.
Payload source: payload.cs:line 125
