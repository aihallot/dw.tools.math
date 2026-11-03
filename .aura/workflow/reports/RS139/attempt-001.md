# RS139 Attempt 001 Payload Operations

- Operations: 38
- Changed: 6
- No change: 0
- Verified: 32
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS139 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS138/attempt-001.json` | run=RS138 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/packagedDependencies |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/package_published |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 17 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 18 | `require.text-contains` | Verified | `src/projects/dw.quantities.expression/dw.quantities.expression.csproj` | semantic text found |
| 19 | `require.text-contains` | Verified | `src/projects/dw.quantities.expression/dw.quantities.expression.csproj` | semantic text found |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs` | file exists |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f6-expression-publisher.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetExpressionPublisher.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-release/dw.tools.math.nuget-expression-release.csproj` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-release/Program.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json` | path is absent |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f6-expression-publisher.md` | bytes=6237 |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetExpressionPublisher.cs` | bytes=10300 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-release/dw.tools.math.nuget-expression-release.csproj` | bytes=331 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props` | bytes=241 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-release/Program.cs` | bytes=29817 |
| 32 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f6-expression-publisher.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-release/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-release/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props` | semantic text found |
| 36 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json` | bytes=11535 |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS139 hierarchy=exact
- After SHA-256: `0e67b6d34506e69d743005c1405c51ab5b580029ec93f2907cb42235596f6455`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS138/attempt-001.json`
- Summary: run=RS138 attempt=1 lineage=exact
- After SHA-256: `9f946829aae80b5c80048e7ae4f624c88ffe08cd92cf581556baf76b5331e8f1`

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
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/packagedDependencies

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/package_published

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 18. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.expression/dw.quantities.expression.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 19. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.expression/dw.quantities.expression.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs`
- Summary: file exists

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetExpressionPublisher.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/dw.tools.math.nuget-expression-release.csproj`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Program.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json`
- Summary: path is absent

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher.md`
- Summary: bytes=6237
- After SHA-256: `f79e6fbb092740a692de6af910352c38d6e1ba19366a5499e8314f8ba1e2a992`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetExpressionPublisher.cs`
- Summary: bytes=10300
- After SHA-256: `fd0677a0dd7a0ee205414472709e7b25ebef3ef5e04dd8bb52ca758cc7f02145`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-release/dw.tools.math.nuget-expression-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-release/Program.cs`
- Summary: bytes=29817
- After SHA-256: `700e262e13ce26d841d11b8914679b9ba801fdf1ffb06f8eac4edb7fdfa4bf20`

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json`
- Summary: bytes=11535
- After SHA-256: `14e7e4bd8b15319853cb943b8362785a7cba10c1dc73eaa18517110f0a19e8ef`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

