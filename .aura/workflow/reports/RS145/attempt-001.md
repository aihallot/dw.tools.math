# RS145 Attempt 001 Payload Operations

- Operations: 40
- Changed: 6
- No change: 0
- Verified: 34
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS145 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS144/attempt-001.json` | run=RS144 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/package_published |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 19 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 20 | `require.text-contains` | Verified | `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj` | semantic text found |
| 21 | `require.text-contains` | Verified | `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj` | semantic text found |
| 22 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` | file exists |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f12-ir-publisher.md` | path is absent |
| 24 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetIrPublisher.cs` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj` | path is absent |
| 26 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props` | path is absent |
| 27 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-release/Program.cs` | path is absent |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json` | path is absent |
| 29 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f12-ir-publisher.md` | bytes=5688 |
| 30 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetIrPublisher.cs` | bytes=10955 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj` | bytes=331 |
| 32 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props` | bytes=241 |
| 33 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-release/Program.cs` | bytes=31479 |
| 34 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f12-ir-publisher.md` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-release/Program.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-release/Program.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props` | semantic text found |
| 38 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json` | bytes=11786 |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS145 hierarchy=exact
- After SHA-256: `fdcee03bbd02ea4c7844c9b0df0d15975d20ee69d94511140c273217c921485b`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS144/attempt-001.json`
- Summary: run=RS144 attempt=1 lineage=exact
- After SHA-256: `1294512dda81f6e8860ad6605fb5002113bc6123ad51b4f73752866c564dfcde`

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
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/package_published

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

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
- Path: `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 21. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 22. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs`
- Summary: file exists

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher.md`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetIrPublisher.cs`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Program.cs`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json`
- Summary: path is absent

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher.md`
- Summary: bytes=5688
- After SHA-256: `bf0a86fb6710f2bdb5760adc2408c6663d5e5b4063e4d8dccb50c65ca78a16fb`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetIrPublisher.cs`
- Summary: bytes=10955
- After SHA-256: `f989044497ee1f24de23a4d2bc787b9d37b89f8178e8dbe8396259776bfae6a0`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-release/dw.tools.math.nuget-ir-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-release/Program.cs`
- Summary: bytes=31479
- After SHA-256: `2a399d81366d5805d470610e82751c148dbbf88defdfbcbf0a01fe9c51c08fc4`

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json`
- Summary: bytes=11786
- After SHA-256: `95a0dc69ddfe46067ab522ea2f1631ca15a12d402266c2351a6160e4f8b3c7c2`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

