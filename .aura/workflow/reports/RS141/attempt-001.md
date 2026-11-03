# RS141 Attempt 001 Payload Operations

- Operations: 41
- Changed: 7
- No change: 0
- Verified: 34
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS141 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS140/attempt-001.json` | run=RS140 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/dependency_package_verified |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/observation/archiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/observation/exactDependencyArchiveSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 16 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 17 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 18 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 19 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs` | file exists |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStandardCandidate.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-candidate/dw.tools.math.nuget-standard-candidate.csproj` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-candidate/Program.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs` | path is absent |
| 27 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | path is absent |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f8-standard-candidate.md` | bytes=5799 |
| 29 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStandardCandidate.cs` | bytes=13571 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-candidate/dw.tools.math.nuget-standard-candidate.csproj` | bytes=331 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props` | bytes=241 |
| 32 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-candidate/Program.cs` | bytes=23711 |
| 33 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs` | bytes=4027 |
| 34 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate.md` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-candidate/Program.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-candidate/Program.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props` | semantic text found |
| 39 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | bytes=18111 |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS141 hierarchy=exact
- After SHA-256: `fbb20d8e8bd496f2c895895f1152d23dcdb2ae68ec7e118c7c756124e2bea5bf`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS140/attempt-001.json`
- Summary: run=RS140 attempt=1 lineage=exact
- After SHA-256: `ba34867d3f4b495e243145fa733c0fdd8aeb1558bebe37e9c85b23fbde8cd361`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/dependency_package_verified

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/observation/exactDependencyArchiveSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/observation/compiledConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 18. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 19. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs`
- Summary: file exists

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStandardCandidate.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/dw.tools.math.nuget-standard-candidate.csproj`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Program.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: path is absent

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate.md`
- Summary: bytes=5799
- After SHA-256: `dc0893208c3b6739c02a61c04096e809f4a60e4b33728beab2d331586fe54668`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStandardCandidate.cs`
- Summary: bytes=13571
- After SHA-256: `93b7f655cf2fa8c0d27fa5cdbd3e7836b84fe526c64fd96e1c220ebd9962a223`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-candidate/dw.tools.math.nuget-standard-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Program.cs`
- Summary: bytes=23711
- After SHA-256: `770f6e046d949d29292f79fb12eef7ef7f035ab1c6c7c42c0f5100f00a229c30`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
- Summary: bytes=4027
- After SHA-256: `57795fbcf0b456f5922bb98681fe0065598f31e6a79c36d50c0c29c3a3c84632`

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: bytes=18111
- After SHA-256: `09aa0cb9c54109605e9bae53b0ca60109ebe195f363034219fc0c6ac6c6ef437`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

