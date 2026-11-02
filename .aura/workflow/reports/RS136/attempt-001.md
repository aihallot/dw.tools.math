# RS136 Attempt 001 Payload Operations

- Operations: 30
- Changed: 6
- No change: 0
- Verified: 24
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS136 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS135/attempt-001.json` | run=RS135 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/candidate/packageVersion |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/package_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 13 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f3-quantities-publisher.md` | path is absent |
| 14 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetQuantitiesReleaseController.cs` | path is absent |
| 15 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj` | path is absent |
| 16 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-release/Directory.Packages.props` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-release/Program.cs` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json` | path is absent |
| 19 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f3-quantities-publisher.md` | bytes=5587 |
| 20 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetQuantitiesReleaseController.cs` | bytes=8758 |
| 21 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj` | bytes=331 |
| 22 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-release/Directory.Packages.props` | bytes=241 |
| 23 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-release/Program.cs` | bytes=22627 |
| 24 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f3-quantities-publisher.md` | semantic text found |
| 25 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-release/Program.cs` | semantic text found |
| 26 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-release/Program.cs` | semantic text found |
| 27 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-release/Directory.Packages.props` | semantic text found |
| 28 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json` | bytes=7575 |
| 29 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS136 hierarchy=exact
- After SHA-256: `e581da041a0bf2929978501c7e1e9be663dec2eed5ecf9877f4f6290811b9c99`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS135/attempt-001.json`
- Summary: run=RS135 attempt=1 lineage=exact
- After SHA-256: `a85123117094166e4627abfccb6f470c6691e9a1f73ddfc60bbb524394efc760`

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
- Path: `docs/distribution/nuget-federation-f2-quantities-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f2-quantities-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f2-quantities-qualified.json`
- Summary: pointer=/package_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 13. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher.md`
- Summary: path is absent

## 14. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetQuantitiesReleaseController.cs`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/Directory.Packages.props`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/Program.cs`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json`
- Summary: path is absent

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher.md`
- Summary: bytes=5587
- After SHA-256: `0a0dd5ef28d03d3f4e61f9dfb36378d7cee068ac6b04b1c6cb4827af1b705184`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetQuantitiesReleaseController.cs`
- Summary: bytes=8758
- After SHA-256: `e45096a1975cf0b0729b503bf544688abfcbd34646d28fefeb4a66a790edfdb2`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-release/dw.tools.math.nuget-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-release/Program.cs`
- Summary: bytes=22627
- After SHA-256: `fa460363242e3be1717121d1d1aaa13c41fa864aa3319077dbb8912f70caf6b8`

## 24. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json`
- Summary: bytes=7575
- After SHA-256: `2f72524b5d43b1db51b68ab7da706031e2dd4791cc5e27d02bee602649b14022`

## 29. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

