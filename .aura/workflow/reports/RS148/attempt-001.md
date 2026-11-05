# RS148 Attempt 001 Payload Operations

- Operations: 36
- Changed: 6
- No change: 0
- Verified: 30
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS148 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS147/attempt-001.json` | run=RS147 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs` | file exists |
| 18 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher.md` | path is absent |
| 19 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetAnalysisPublisher.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-release/dw.tools.math.nuget-analysis-release.csproj` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f15-analysis-publisher.md` | bytes=4146 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetAnalysisPublisher.cs` | bytes=9991 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-release/dw.tools.math.nuget-analysis-release.csproj` | bytes=331 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props` | bytes=241 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | bytes=27611 |
| 29 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props` | semantic text found |
| 34 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | bytes=11565 |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS148 hierarchy=exact
- After SHA-256: `a3a3b53bac329a7c8ea87205a781f7b5d6daa0b64149c515c0865c3ad8035501`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS147/attempt-001.json`
- Summary: run=RS147 attempt=1 lineage=exact
- After SHA-256: `1a1cdbd599784390bf4ceadfea1c60047a579b10de2b39edc0af6468101e4fa4`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
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
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/package_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs`
- Summary: file exists

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher.md`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetAnalysisPublisher.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/dw.tools.math.nuget-analysis-release.csproj`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher.md`
- Summary: bytes=4146
- After SHA-256: `61cf336f05775c4a7d3fde0d24ccdfc1868d6c0460dae6aa877698e7df202e76`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetAnalysisPublisher.cs`
- Summary: bytes=9991
- After SHA-256: `b9b83b0fb6658596e5baf48eb4d2c07fb013b37b4125b0eb11d429b3bace4259`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-release/dw.tools.math.nuget-analysis-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: bytes=27611
- After SHA-256: `cbd4adbdb5eaaf13147eb3fb41985024586f3c883394b342797a773139446b90`

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: bytes=11565
- After SHA-256: `8db5d7fd68657093f59a958db37f2a32b5e6963b97d37155496f5a32424c41b2`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

