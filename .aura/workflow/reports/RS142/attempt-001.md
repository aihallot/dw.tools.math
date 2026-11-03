# RS142 Attempt 001 Payload Operations

- Operations: 41
- Changed: 6
- No change: 0
- Verified: 35
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS142 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS141/attempt-001.json` | run=RS141 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/consumerChecks |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/package_published |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/physically_published_package_verified |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/dependency_package_verified |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 19 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 20 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 21 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 22 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 23 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs` | file exists |
| 24 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher.md` | path is absent |
| 25 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStandardPublisher.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-release/dw.tools.math.nuget-standard-release.csproj` | path is absent |
| 27 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props` | path is absent |
| 28 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-release/Program.cs` | path is absent |
| 29 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json` | path is absent |
| 30 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f9-standard-publisher.md` | bytes=5959 |
| 31 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStandardPublisher.cs` | bytes=12248 |
| 32 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-release/dw.tools.math.nuget-standard-release.csproj` | bytes=331 |
| 33 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props` | bytes=241 |
| 34 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-release/Program.cs` | bytes=33449 |
| 35 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher.md` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-release/Program.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-release/Program.cs` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props` | semantic text found |
| 39 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json` | bytes=12175 |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS142 hierarchy=exact
- After SHA-256: `503120590a982233ec9394ae70d568a4778bb628e648c52e14e6dac38c06f149`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS141/attempt-001.json`
- Summary: run=RS141 attempt=1 lineage=exact
- After SHA-256: `3a64837e0c2fb5e9a146954ade6a30ec4da5491fb23e4863fb2d16407345ca6a`

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
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/consumerChecks

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/package_published

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/dependency_package_verified

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 20. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 21. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 22. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
- Summary: file exists

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher.md`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStandardPublisher.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/dw.tools.math.nuget-standard-release.csproj`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Program.cs`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json`
- Summary: path is absent

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher.md`
- Summary: bytes=5959
- After SHA-256: `4386962854ac386c6563f4f6dca5d23b5a5dc1a8ce5a35cc6b02b6370194f1d8`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStandardPublisher.cs`
- Summary: bytes=12248
- After SHA-256: `eb3630dbb29f27b28095410b189ac9009b18c5206e18073e175728cc3df3f858`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-release/dw.tools.math.nuget-standard-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-release/Program.cs`
- Summary: bytes=33449
- After SHA-256: `a929f487db9ce1bdf343f0595dff0d676092fbf607a8e5a8c8b77e4d54c554c2`

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json`
- Summary: bytes=12175
- After SHA-256: `c3d1f63e2b7ce953beb0cb724e0e0d7cef7bc9e836e7a9311b4c0892e7b946ce`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

