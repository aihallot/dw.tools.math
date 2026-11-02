# RS137 Attempt 001 Payload Operations

- Operations: 31
- Changed: 6
- No change: 0
- Verified: 25
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS137 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS136/attempt-001.json` | run=RS136 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/candidate/packageVersion |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 14 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f4-quantities-published.md` | path is absent |
| 15 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetQuantitiesPublishedFeed.cs` | path is absent |
| 16 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-feed-verify/dw.tools.math.nuget-feed-verify.csproj` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props` | path is absent |
| 18 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-feed-verify/Program.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | path is absent |
| 20 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f4-quantities-published.md` | bytes=4916 |
| 21 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetQuantitiesPublishedFeed.cs` | bytes=9075 |
| 22 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-feed-verify/dw.tools.math.nuget-feed-verify.csproj` | bytes=331 |
| 23 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props` | bytes=241 |
| 24 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-feed-verify/Program.cs` | bytes=15069 |
| 25 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f4-quantities-published.md` | semantic text found |
| 26 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-feed-verify/Program.cs` | semantic text found |
| 27 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-feed-verify/Program.cs` | semantic text found |
| 28 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props` | semantic text found |
| 29 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | bytes=8568 |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS137 hierarchy=exact
- After SHA-256: `009a81817b8c3d1e573d6e46e5ca3ecb7226aa2cdf6771f8be8b5fa03e0fb171`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS136/attempt-001.json`
- Summary: run=RS136 attempt=1 lineage=exact
- After SHA-256: `33f2b2ef1787a2d7c70913605d6d10bd50f5576ec3fb0622faaacd7741c3ccd5`

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
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f3-quantities-publisher-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 14. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published.md`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetQuantitiesPublishedFeed.cs`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/dw.tools.math.nuget-feed-verify.csproj`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Program.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: path is absent

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f4-quantities-published.md`
- Summary: bytes=4916
- After SHA-256: `b141dc34e40c8a5c8d967e71e590c09062d377521f88537f10b1f2814de4c002`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetQuantitiesPublishedFeed.cs`
- Summary: bytes=9075
- After SHA-256: `8da23601439b4428b15bcac592ca9768967e21d8847113fc7f4ae1dbda19a74a`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-feed-verify/dw.tools.math.nuget-feed-verify.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-feed-verify/Program.cs`
- Summary: bytes=15069
- After SHA-256: `5cd1f85e76162b855edda7b45c2eff0e5759e96866ceb2086665196534e967a3`

## 25. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: bytes=8568
- After SHA-256: `d8a8fb3119b2a2f4d2b84d2adb8b94f8dc1746ff257c9d33f07c64aad99709e6`

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

