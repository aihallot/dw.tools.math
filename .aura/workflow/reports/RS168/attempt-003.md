# RS168 Attempt 003 Payload Operations

- Operations: 46
- Changed: 3
- No change: 4
- Verified: 39
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS168 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS167/attempt-001.json` | run=RS167 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/observation/packageId |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/status |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packageVersion |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/archiveSha256 |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packagedDllSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/archiveSha256 |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/packagedDllSha256 |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 21 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/9/packageId |
| 22 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/9/historicalSourceVersion |
| 23 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/9/projectReferences/0 |
| 24 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.numerics/packages.lock.json` | pointer=/dependencies/net10.0/dw.tools.math.ir/type |
| 25 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.numerics/packages.lock.json` | pointer=/dependencies/net10.0/dw.quantities/type |
| 26 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 27 | `require.text-contains` | Verified | `src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj` | semantic text found |
| 28 | `require.text-contains` | Verified | `src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj` | semantic text found |
| 29 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 30 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | path is absent |
| 31 | `files.replace-from-staged` | NoChange | `docs/distribution/nuget-federation-f35-numerics-candidate.md` | staged content already installed |
| 32 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetNumericsCandidate.cs` | bytes=14385 |
| 33 | `files.replace-from-staged` | NoChange | `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj` | staged content already installed |
| 34 | `files.replace-from-staged` | NoChange | `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props` | staged content already installed |
| 35 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-candidate/Program.cs` | bytes=28196 |
| 36 | `files.replace-from-staged` | NoChange | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | staged content already installed |
| 37 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | file exists |
| 38 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate.md` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/Program.cs` | semantic text found |
| 41 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | semantic text found |
| 42 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj` | semantic text found |
| 43 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props` | semantic text found |
| 44 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | bytes=18386 |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 46 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS168 hierarchy=exact
- After SHA-256: `620dc57a4f4235c46b774d0b7e97aa48a01a10693a4f567ff064d87b82372377`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS167/attempt-001.json`
- Summary: run=RS167 attempt=1 lineage=exact
- After SHA-256: `ae712e0a396d4d0cafa49db625ffa5b38ce9e70591150f2362c06f75f0dafe09`

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
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/observation/packageId

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: pointer=/observation/compiledConsumerChecks

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/packagedDllSha256

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/exactDependencies/0/archiveSha256

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/exactDependencies/0/packagedDllSha256

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/9/packageId

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/9/historicalSourceVersion

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/9/projectReferences/0

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.numerics/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.tools.math.ir/type

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.numerics/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.quantities/type

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 27. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.numerics/dw.tools.math.numerics.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 30. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: path is absent

## 31. `files.replace-from-staged`

- Status: `NoChange`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate.md`
- Summary: staged content already installed
- Before SHA-256: `42386683fbe057bb93d42d4d7db0c5d0275cce89f8a189c734e0f5e0223500e7`
- After SHA-256: `42386683fbe057bb93d42d4d7db0c5d0275cce89f8a189c734e0f5e0223500e7`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetNumericsCandidate.cs`
- Summary: bytes=14385
- Before SHA-256: `fd9b056ab32975a8eb269e8ede056782dc6e42ea2ee046f0f54abf62c9fa8985`
- After SHA-256: `b1ea7a4ccd8a3f0603eae233aa30b05edaaf400f5d19627ca8e8cc3d22f1dac9`

## 33. `files.replace-from-staged`

- Status: `NoChange`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj`
- Summary: staged content already installed
- Before SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 34. `files.replace-from-staged`

- Status: `NoChange`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props`
- Summary: staged content already installed
- Before SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/Program.cs`
- Summary: bytes=28196
- Before SHA-256: `38bc445052cea58a73841fb0e395eb7dfad5548e5c2a4f06565217905105219c`
- After SHA-256: `3d7e755f54598f74eeded0901238d9ce322678f9578a00e499a50f98a7ff330d`

## 36. `files.replace-from-staged`

- Status: `NoChange`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: staged content already installed
- Before SHA-256: `98383b85fde7a4c773f68bddccda34e351db7f7ece038a77c92a759ea7f6766b`
- After SHA-256: `98383b85fde7a4c773f68bddccda34e351db7f7ece038a77c92a759ea7f6766b`

## 37. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: file exists

## 38. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/dw.tools.math.nuget-numerics-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 44. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: bytes=18386
- After SHA-256: `5f037c783cee1ca66b495db598fa7eb9b4f9e7a5038a58b0fe15a8fe4b88c3f5`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 46. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

