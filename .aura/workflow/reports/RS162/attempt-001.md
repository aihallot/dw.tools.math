# RS162 Attempt 001 Payload Operations

- Operations: 43
- Changed: 6
- No change: 0
- Verified: 37
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS162 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS161/attempt-001.json` | run=RS161 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/originalNineSourceFilesLinked |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/originalSourceProjectReferences |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/originalSourceLockSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/package_published |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 23 | `require.text-contains` | Verified | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | semantic text found |
| 24 | `require.text-contains` | Verified | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | semantic text found |
| 25 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | file exists |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher.md` | path is absent |
| 27 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCodecsPublisher.cs` | path is absent |
| 28 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-release/dw.tools.math.nuget-codecs-release.csproj` | path is absent |
| 29 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props` | path is absent |
| 30 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-release/Program.cs` | path is absent |
| 31 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json` | path is absent |
| 32 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f29-codecs-publisher.md` | bytes=3745 |
| 33 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCodecsPublisher.cs` | bytes=12602 |
| 34 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-release/dw.tools.math.nuget-codecs-release.csproj` | bytes=331 |
| 35 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props` | bytes=241 |
| 36 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-release/Program.cs` | bytes=35755 |
| 37 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher.md` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-release/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-release/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props` | semantic text found |
| 41 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json` | bytes=12419 |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 43 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS162 hierarchy=exact
- After SHA-256: `ee0c666f634a3b71ede6e907c1bdbf1be073a0777bdd849ab6693ee1ee47b672`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS161/attempt-001.json`
- Summary: run=RS161 attempt=1 lineage=exact
- After SHA-256: `723f36a10acf832ffb79d3554c5cbfe309d51d475e7b1d8c81ba40beb1867870`

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
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/originalNineSourceFilesLinked

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceProjectReferences

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceLockSha256

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/package_published

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
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
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: file exists

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher.md`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCodecsPublisher.cs`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/dw.tools.math.nuget-codecs-release.csproj`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/Program.cs`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json`
- Summary: path is absent

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher.md`
- Summary: bytes=3745
- After SHA-256: `2bb154197bccf04a89a597e71c21ad3e1f15371616da0a8d99a2cee5ca3b4b0e`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCodecsPublisher.cs`
- Summary: bytes=12602
- After SHA-256: `7906dfebe367d1219ff3faccee88c9b6489655da6365ca27c998cb85e2d44399`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-release/dw.tools.math.nuget-codecs-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-release/Program.cs`
- Summary: bytes=35755
- After SHA-256: `6833566c0c1fd05055e3a76ffa48c11b43df1ab009104561cfa2d0adf6988b29`

## 37. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json`
- Summary: bytes=12419
- After SHA-256: `bf16863b1815c2f445f7c2e106806b21d07be072df713f1ea3a14649141e9980`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

