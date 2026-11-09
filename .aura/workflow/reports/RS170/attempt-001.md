# RS170 Attempt 001 Payload Operations

- Operations: 35
- Changed: 6
- No change: 0
- Verified: 29
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS170 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS169/attempt-002.json` | run=RS169 attempt=2 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json` | pointer=/status |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json` | pointer=/rehearsal/reconstructionIsOriginalArchiveProof |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json` | pointer=/shared_package_published |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | file exists |
| 18 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f37-numerics-release.md` | path is absent |
| 19 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetNumericsRelease.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-numerics-release/Directory.Packages.props` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-numerics-release/Program.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f37-numerics-release.md` | bytes=3184 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetNumericsRelease.cs` | bytes=5990 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj` | bytes=331 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-release/Directory.Packages.props` | bytes=241 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-release/Program.cs` | bytes=22617 |
| 29 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f37-numerics-release.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-release/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-release/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-release/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj` | semantic text found |
| 34 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | bytes=6207 |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS170 hierarchy=exact
- After SHA-256: `e1c909456746fe11a19171fd6f1605cb06773971a688483d44926489468bbd8d`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS169/attempt-002.json`
- Summary: run=RS169 attempt=2 lineage=exact
- After SHA-256: `11aaa740d0484f5df5da45918f706a0687c3cb254108c0966c01dfa1d8972953`

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
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json`
- Summary: pointer=/status

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json`
- Summary: pointer=/rehearsal/reconstructionIsOriginalArchiveProof

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json`
- Summary: pointer=/shared_package_published

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 17. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: file exists

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release.md`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetNumericsRelease.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/Directory.Packages.props`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/Program.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f37-numerics-release.md`
- Summary: bytes=3184
- After SHA-256: `c2a4e4d5a6471f3e9eda54dd5594910d92e41d8a607f21117a17517833fc7bb8`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetNumericsRelease.cs`
- Summary: bytes=5990
- After SHA-256: `b33e144df96716817bf5b444c1c2bf54a4e518aaf2750ca15bacec9f4bf5b5d3`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-release/Program.cs`
- Summary: bytes=22617
- After SHA-256: `b2141333208873730cad4e3785f2bac2c0910db0514d3eb5214387b23ea06695`

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-release/dw.tools.math.nuget-numerics-release.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: bytes=6207
- After SHA-256: `6a58ee23dc383ac081cd6a16624a72f3f17c9fa5ad9e8e83e0ab641d8d1507ad`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

