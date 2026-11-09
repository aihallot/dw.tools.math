# Run-next failure evidence

- Invocation: `run-next-20261109T102801Z-e3557eeaefba7d14`
- Run: `RS168`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `yes`
- Safe next action: `dwf run next`

## Error

Run preparation gate RS168 failed before attempt allocation. classification=payload. The source repository remains unchanged and no durable attempt was created.
Payload error:
RS168: retained RS168 target differs from original attempt and corrected staged bytes: tools/dw.tools.math.nuget-numerics-candidate/Program.cs
Payload source: payload.cs:line 289

## Changed paths
- `docs/distribution/ValidateNugetNumericsCandidate.cs`
- `docs/distribution/nuget-federation-f35-numerics-candidate.md`
- `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props`
- `tools/dw.tools.math.nuget-numerics-candidate/Program.cs`
- `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj`
- `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
