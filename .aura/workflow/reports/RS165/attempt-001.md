# RS165 Attempt 001 Payload Operations

- Operations: 44
- Changed: 6
- No change: 0
- Verified: 38
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS165 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS164/attempt-001.json` | run=RS164 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/originalFourSourceFilesLinked |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/originalSourceProjectReferences |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/originalSourceLockSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/package_published |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 23 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 24 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 25 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 26 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | file exists |
| 27 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher.md` | path is absent |
| 28 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCompositionPublisher.cs` | path is absent |
| 29 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj` | path is absent |
| 30 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props` | path is absent |
| 31 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | path is absent |
| 32 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | path is absent |
| 33 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f32-composition-publisher.md` | bytes=4024 |
| 34 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCompositionPublisher.cs` | bytes=12859 |
| 35 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj` | bytes=331 |
| 36 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props` | bytes=241 |
| 37 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-release/Program.cs` | bytes=36547 |
| 38 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher.md` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | semantic text found |
| 41 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props` | semantic text found |
| 42 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | bytes=13036 |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS165 hierarchy=exact
- After SHA-256: `0021f980b8093025c1c6f99b46acb1b7ba2224af8759cb3f285a80256b424836`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS164/attempt-001.json`
- Summary: run=RS164 attempt=1 lineage=exact
- After SHA-256: `a612194241e652c32d7ca15417eb8cac7ac32054922448bc3409844a2ad102b6`

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
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/originalFourSourceFilesLinked

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceProjectReferences

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/originalSourceLockSha256

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
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
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: file exists

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher.md`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCompositionPublisher.cs`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: path is absent

## 32. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: path is absent

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher.md`
- Summary: bytes=4024
- After SHA-256: `c321f9e28d6cffd039826afe7881b53ec19fffb97779cd75496b7465740f3111`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCompositionPublisher.cs`
- Summary: bytes=12859
- After SHA-256: `042303fdcc6c984cf6a397955e8236812477b23dcefc7cfbd9f25024938677cd`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 37. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: bytes=36547
- After SHA-256: `08a7dccd4fd8e7df6f0e94f17a37dafaadad68c94a6d3e8b9bb28bfeebf64f30`

## 38. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: bytes=13036
- After SHA-256: `4e5a41e918f6e98898641b17223a5fdfa91499b53dd4217bb9eb68eabb312904`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

