# RS138 Attempt 001 Payload Operations

- Operations: 37
- Changed: 7
- No change: 0
- Verified: 30
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS138 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS137/attempt-001.json` | run=RS137 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/observation/packageVersion |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/observation/archiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 13 | `require.text-contains` | Verified | `src/projects/dw.quantities.expression/dw.quantities.expression.csproj` | semantic text found |
| 14 | `require.text-contains` | Verified | `src/projects/dw.quantities.expression/dw.quantities.expression.csproj` | semantic text found |
| 15 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 16 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-feed-verify/Program.cs` | file exists |
| 17 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate.md` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetExpressionCandidate.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-candidate/dw.tools.math.nuget-expression-candidate.csproj` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-candidate/Program.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f5-expression-candidate.md` | bytes=5374 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetExpressionCandidate.cs` | bytes=11283 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-candidate/dw.tools.math.nuget-expression-candidate.csproj` | bytes=331 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props` | bytes=241 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-candidate/Program.cs` | bytes=22415 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs` | bytes=2493 |
| 30 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-candidate/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-candidate/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props` | semantic text found |
| 35 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | bytes=17500 |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS138 hierarchy=exact
- After SHA-256: `8cc6970bf6b5b0b60ddb7358ea572c75545a36c949c46b63b99627beb52678bd`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS137/attempt-001.json`
- Summary: run=RS137 attempt=1 lineage=exact
- After SHA-256: `3099393c541d350cf612c076e1284b636ec6cac38d2feb6ef63243872d69576e`

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
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 13. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.expression/dw.quantities.expression.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 14. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.expression/dw.quantities.expression.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 15. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 16. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-feed-verify/Program.cs`
- Summary: file exists

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate.md`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetExpressionCandidate.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/dw.tools.math.nuget-expression-candidate.csproj`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Program.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate.md`
- Summary: bytes=5374
- After SHA-256: `a6c09428357fc6677fec1bdb58ea8b0c827182c630810a47759ced3aa9666402`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetExpressionCandidate.cs`
- Summary: bytes=11283
- After SHA-256: `d4e1010d391bbda11403ce2f3ce04c3bd2e9853773413920989a9078d363f216`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-candidate/dw.tools.math.nuget-expression-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Program.cs`
- Summary: bytes=22415
- After SHA-256: `cf99428467734443381f15b3f0a9eaee7f2dd6a804355fb1241385b6ed115dc1`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs`
- Summary: bytes=2493
- After SHA-256: `336573d2d389fcc57024b5a5d1337061efb28e2583f1695125e7ec1482b6d54c`

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: bytes=17500
- After SHA-256: `1e9f365a695f8ad4e29fb4fa426d7f7294c0cf1ad2fb1d0401cf27675a319019`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

