# RS164 Attempt 001 Payload Operations

- Operations: 52
- Changed: 7
- No change: 0
- Verified: 45
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS164 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS163/attempt-001.json` | run=RS163 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packageVersion |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/archiveSha256 |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packagedDllSha256 |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/archiveSha256 |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/packagedDllSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/5/packageId |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/5/historicalSourceVersion |
| 21 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/5/projectReferences/0 |
| 22 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/5/projectReferences/1 |
| 23 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.composition/packages.lock.json` | pointer=/dependencies/net10.0/dw.tools.math.ir/type |
| 24 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.composition/packages.lock.json` | pointer=/dependencies/net10.0/dw.quantities/type |
| 25 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 26 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 27 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 28 | `require.text-contains` | Verified | `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj` | semantic text found |
| 29 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 30 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate.md` | path is absent |
| 31 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCompositionCandidate.cs` | path is absent |
| 32 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj` | path is absent |
| 33 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props` | path is absent |
| 34 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-candidate/Program.cs` | path is absent |
| 35 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | path is absent |
| 36 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | path is absent |
| 37 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f31-composition-candidate.md` | bytes=4210 |
| 38 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCompositionCandidate.cs` | bytes=14444 |
| 39 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj` | bytes=331 |
| 40 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props` | bytes=241 |
| 41 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-candidate/Program.cs` | bytes=28288 |
| 42 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | bytes=5021 |
| 43 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | file exists |
| 44 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate.md` | semantic text found |
| 45 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-candidate/Program.cs` | semantic text found |
| 46 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-candidate/Program.cs` | semantic text found |
| 47 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | semantic text found |
| 48 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj` | semantic text found |
| 49 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props` | semantic text found |
| 50 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | bytes=18485 |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 52 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS164 hierarchy=exact
- After SHA-256: `0d1f7710688261122b612eb96653c89f3bf9bb6956fb0510c8fc538aba6376de`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS163/attempt-001.json`
- Summary: run=RS163 attempt=1 lineage=exact
- After SHA-256: `4bc6c18c9f11227feee061a9cc815d560330fedcbe5d68173b26de1abcea8fd6`

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
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/packagedDllSha256

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/exactDependencies/0/archiveSha256

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/observation/exactDependencies/0/packagedDllSha256

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/5/packageId

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/5/historicalSourceVersion

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/5/projectReferences/0

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/5/projectReferences/1

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.tools.math.ir/type

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.quantities/type

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 26. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.composition/dw.tools.math.composition.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 30. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate.md`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCompositionCandidate.cs`
- Summary: path is absent

## 32. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj`
- Summary: path is absent

## 33. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props`
- Summary: path is absent

## 34. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Program.cs`
- Summary: path is absent

## 35. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: path is absent

## 36. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: path is absent

## 37. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate.md`
- Summary: bytes=4210
- After SHA-256: `d06ee2a5d55cf47a93809a1436aed258bf12ba07e2799b975e7297130763f1c5`

## 38. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCompositionCandidate.cs`
- Summary: bytes=14444
- After SHA-256: `68bc3addcc3b99dd7140d729593478640c7727bea44bfebba55b0562b3cf3ba8`

## 39. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 40. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 41. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Program.cs`
- Summary: bytes=28288
- After SHA-256: `1b68c9e039264494330df973ba2cdf09c0bf7c61f1e314cd7f0270d8e0ad83e9`

## 42. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: bytes=5021
- After SHA-256: `a2ad2e2f0dce0a11aa8308bbe47e0afad25447c3cc208bbaaaa5c7815d144307`

## 43. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: file exists

## 44. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 45. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 46. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 47. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 48. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/dw.tools.math.nuget-composition-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 49. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 50. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: bytes=18485
- After SHA-256: `e929865b5311835fc5c4feb294ab2053010b2b9101fe49e54121a3187fef91d4`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 52. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

