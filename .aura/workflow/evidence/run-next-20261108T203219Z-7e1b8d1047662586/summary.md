# Run-next failure evidence

- Invocation: `run-next-20261108T203219Z-7e1b8d1047662586`
- Run: `RS168`
- Attempt allocated: `no`
- Failed stage: `pre-attempt`
- Classification: `workflow-refusal`
- Working tree changed: `yes`
- Safe next action: `Resolve unrelated working-tree changes, then run: dwf run next`

## Error

The repository working tree is not clean. Same-run correction is selected automatically only when a correction receipt exists; otherwise resolve unrelated working-tree changes before running 'dwf run next' again.

## Changed paths
- `docs/distribution/ValidateNugetNumericsCandidate.cs`
- `docs/distribution/nuget-federation-f35-numerics-candidate.md`
- `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props`
- `tools/dw.tools.math.nuget-numerics-candidate/Program.cs`
- `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj`
- `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
