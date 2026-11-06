# RS157 Attempt 001 Payload Operations

- Operations: 44
- Changed: 7
- No change: 0
- Verified: 37
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS157 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS156/attempt-001.json` | run=RS156 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/status |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/observation/packageVersion |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/observation/archiveSha256 |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/11/packageId |
| 17 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.statistics/packages.lock.json` | pointer=/dependencies/net10.0/dw.quantities/type |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 19 | `require.text-contains` | Verified | `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj` | semantic text found |
| 20 | `require.text-contains` | Verified | `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj` | semantic text found |
| 21 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 22 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate.md` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStatisticsCandidate.cs` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props` | path is absent |
| 26 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/Program.cs` | path is absent |
| 27 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | path is absent |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | path is absent |
| 29 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f24-statistics-candidate.md` | bytes=7856 |
| 30 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStatisticsCandidate.cs` | bytes=13315 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj` | bytes=331 |
| 32 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props` | bytes=241 |
| 33 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-candidate/Program.cs` | bytes=26111 |
| 34 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | bytes=5865 |
| 35 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | file exists |
| 36 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate.md` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/Program.cs` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj` | semantic text found |
| 41 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props` | semantic text found |
| 42 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | bytes=18837 |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS157 hierarchy=exact
- After SHA-256: `29f179fd1686c782fa4de7c3532e40ba164352768e7c5d9b45b99eb60e2c6034`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS156/attempt-001.json`
- Summary: run=RS156 attempt=1 lineage=exact
- After SHA-256: `9e5940fd5773e01bb5bcc35c514f04f3313e6179472f1c30f7032a7390cbd8be`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
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
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: pointer=/observation/compiledConsumerChecks

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/status

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/11/packageId

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.statistics/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.quantities/type

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 19. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 20. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 21. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate.md`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStatisticsCandidate.cs`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Program.cs`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: path is absent

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate.md`
- Summary: bytes=7856
- After SHA-256: `0faaefa254c7c33540c39022571223354e0cb5efb191f020fa15bb44401a3382`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStatisticsCandidate.cs`
- Summary: bytes=13315
- After SHA-256: `037f0fd89011ea0b5923253ffe2e4be4bcc95e8af3c00206c6b848d5900b9a65`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Program.cs`
- Summary: bytes=26111
- After SHA-256: `5d2dfc83cbead46a31d4ffd4aca6f9360aac49a63b90ce0dd767f5effb6ffcd7`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: bytes=5865
- After SHA-256: `c40797f726fac878a06d9b1e61e76d6ec418e7c6c997b81738c7ae889f066190`

## 35. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: file exists

## 36. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/dw.tools.math.nuget-statistics-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: bytes=18837
- After SHA-256: `42a6759f5ca9cbf008188b07617e19233cf7947ac7db2ea4afb0f2bc1436f0b0`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

