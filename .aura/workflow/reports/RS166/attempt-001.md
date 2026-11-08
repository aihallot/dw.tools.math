# RS166 Attempt 001 Payload Operations

- Operations: 42
- Changed: 4
- No change: 0
- Verified: 38
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS166 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS165/attempt-001.json` | run=RS165 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageId |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/status |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/package_published |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/rehearsal/dependencyCount |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/source_files/tools~1dw.tools.math.nuget-composition-release~1Program.cs |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 21 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 23 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 24 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props` | file exists |
| 25 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj` | file exists |
| 26 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | file exists |
| 27 | `require.file-exists` | Verified | `.workflow/runs/RS165/staged/tools/dw.tools.math.nuget-composition-release/Program.cs` | file exists |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt.md` | path is absent |
| 29 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCompositionReleaseReceipt.cs` | path is absent |
| 30 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json` | path is absent |
| 31 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | file exists |
| 32 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f33-composition-release-receipt.md` | bytes=3741 |
| 33 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCompositionReleaseReceipt.cs` | bytes=13476 |
| 34 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-release/Program.cs` | bytes=37323 |
| 35 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt.md` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props` | semantic text found |
| 40 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json` | bytes=3848 |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 42 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS166 hierarchy=exact
- After SHA-256: `1a184dea90bce661cd9f5e29de5aa18684f7c6b607db60e1e6d2b39905c21c65`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS165/attempt-001.json`
- Summary: run=RS165 attempt=1 lineage=exact
- After SHA-256: `ace0c77c96e54400483bef92d97f33de301e687b29e37baa6e48eb7695b39941`

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
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/package_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/package_published

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/rehearsal/dependencyCount

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/source_files/tools~1dw.tools.math.nuget-composition-release~1Program.cs

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 24. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props`
- Summary: file exists

## 25. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/dw.tools.math.nuget-composition-release.csproj`
- Summary: file exists

## 26. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: file exists

## 27. `require.file-exists`

- Status: `Verified`
- Path: `.workflow/runs/RS165/staged/tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: file exists

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt.md`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCompositionReleaseReceipt.cs`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json`
- Summary: path is absent

## 31. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: file exists

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt.md`
- Summary: bytes=3741
- After SHA-256: `9bd1dedf7e7872cfaac0b3751974cf553772ad3332a1e84dd70ed68824211f84`

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCompositionReleaseReceipt.cs`
- Summary: bytes=13476
- After SHA-256: `f08d060be48fe94cfc24530efed5720b94975ab9b5250ef3e6d5cfb1b3b43306`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: bytes=37323
- Before SHA-256: `08a7dccd4fd8e7df6f0e94f17a37dafaadad68c94a6d3e8b9bb28bfeebf64f30`
- After SHA-256: `dc469c68d9d5ca559f8629ae06da7d524f7ffdd30ce5c81f4d4234b951f612ce`

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json`
- Summary: bytes=3848
- After SHA-256: `6f7bc006573c80d3993e76769fa17a8e014c5861484a9b4fda2abb4adabdd214`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 42. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

