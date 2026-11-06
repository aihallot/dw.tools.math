# RS158 Attempt 001 Payload Operations

- Operations: 43
- Changed: 6
- No change: 0
- Verified: 37
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS158 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS157/attempt-001.json` | run=RS157 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/originalEightSourceFilesLinked |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/originalSourceProjectReferences |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/originalSourceLockSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/package_published |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 23 | `require.text-contains` | Verified | `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj` | semantic text found |
| 24 | `require.text-contains` | Verified | `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj` | semantic text found |
| 25 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | file exists |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher.md` | path is absent |
| 27 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStatisticsPublisher.cs` | path is absent |
| 28 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj` | path is absent |
| 29 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props` | path is absent |
| 30 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | path is absent |
| 31 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | path is absent |
| 32 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f25-statistics-publisher.md` | bytes=4537 |
| 33 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStatisticsPublisher.cs` | bytes=12050 |
| 34 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj` | bytes=331 |
| 35 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props` | bytes=241 |
| 36 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | bytes=33332 |
| 37 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher.md` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props` | semantic text found |
| 41 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | bytes=12497 |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 43 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS158 hierarchy=exact
- After SHA-256: `d1959fe10cd9db6c674af936d4ac00edf0121899a04ba29d5ac8d96d09a07911`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS157/attempt-001.json`
- Summary: run=RS157 attempt=1 lineage=exact
- After SHA-256: `889e91c8ef19a9a77008e63e35f1c518ad20a1b23e8b4b77d523f2b4c64d1071`

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
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/originalEightSourceFilesLinked

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceProjectReferences

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceLockSha256

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/package_published

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 23. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.statistics/dw.tools.math.statistics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: file exists

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher.md`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStatisticsPublisher.cs`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: path is absent

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher.md`
- Summary: bytes=4537
- After SHA-256: `cb1e643ebfaf44f4e144360b824048e893e21f1311f6ca66381ff6a538a04ec8`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStatisticsPublisher.cs`
- Summary: bytes=12050
- After SHA-256: `8c4cc802db84bc52d085091e2300564992c1ae0ffb1fa2cad8c565b1073e4efc`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: bytes=33332
- After SHA-256: `d4ba2b3b00b29104cecd95a672fcce9777352ca12045276be583c6043b7bc920`

## 37. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: bytes=12497
- After SHA-256: `31c0cdb89393a9d021d14bbacd1833e75ccb9ea8e9a9a6a273cfc1df0bd00d05`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

