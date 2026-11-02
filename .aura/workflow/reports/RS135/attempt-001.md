# RS135 Attempt 001 Payload Operations

- Operations: 34
- Changed: 7
- No change: 0
- Verified: 27
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS135 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS134/attempt-001.json` | run=RS134 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/discovery/qualification/RS133-neutral-candidate-qualified.json` | pointer=/neutral_migration_qualified |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 12 | `require.text-contains` | Verified | `src/projects/dw.quantities/dw.quantities.csproj` | semantic text found |
| 13 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 14 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/Program.cs` | file exists |
| 15 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f2-quantities.md` | path is absent |
| 16 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetQuantitiesCandidate.cs` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-candidate/dw.tools.math.nuget-candidate.csproj` | path is absent |
| 18 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-candidate/Directory.Packages.props` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-candidate/Program.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | path is absent |
| 22 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f2-quantities.md` | bytes=4953 |
| 23 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetQuantitiesCandidate.cs` | bytes=8131 |
| 24 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-candidate/dw.tools.math.nuget-candidate.csproj` | bytes=331 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-candidate/Directory.Packages.props` | bytes=241 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-candidate/Program.cs` | bytes=16733 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs` | bytes=1412 |
| 28 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f2-quantities.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-candidate/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-candidate/Directory.Packages.props` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs` | semantic text found |
| 32 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f2-quantities-qualified.json` | bytes=14441 |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS135 hierarchy=exact
- After SHA-256: `c90ba24e5050ea6e77cafd36e195d69c491bf85e25eb71c2e881301dab5150ae`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS134/attempt-001.json`
- Summary: run=RS134 attempt=1 lineage=exact
- After SHA-256: `339b63f4d2b0a41b276a71848f8779e35641dd653cd599dc8faf0f8521997f0f`

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
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/RS133-neutral-candidate-qualified.json`
- Summary: pointer=/neutral_migration_qualified

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 12. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities/dw.quantities.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 13. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 14. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/Program.cs`
- Summary: file exists

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f2-quantities.md`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetQuantitiesCandidate.cs`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/dw.tools.math.nuget-candidate.csproj`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/Directory.Packages.props`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/Program.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f2-quantities-qualified.json`
- Summary: path is absent

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f2-quantities.md`
- Summary: bytes=4953
- After SHA-256: `3fc2490f37c2d13c03e004a24eda5fada83b00c17d7a06f9986408fa11e0d917`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetQuantitiesCandidate.cs`
- Summary: bytes=8131
- After SHA-256: `bc906095b28288ef2911f1e02c6864956803fae4de639d14067d30be6b7f85b6`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-candidate/dw.tools.math.nuget-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-candidate/Program.cs`
- Summary: bytes=16733
- After SHA-256: `663d165508202afdd7137381f7a76c87a46fe9d6a006236895daaba1cf443c4e`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs`
- Summary: bytes=1412
- After SHA-256: `ed590be248e5660d34ebb7d911b11924b11767737ec228e67728b41774c534f0`

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f2-quantities.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-candidate/fixtures/QuantitiesBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f2-quantities-qualified.json`
- Summary: bytes=14441
- After SHA-256: `272a6f7bca35dbdf3e5f5723cc2b96e1d31a03167fda851d25f5480348ce484e`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

