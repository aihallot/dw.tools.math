# RS161 Attempt 001 Payload Operations

- Operations: 50
- Changed: 7
- No change: 0
- Verified: 43
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS161 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS160/attempt-001.json` | run=RS160 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | pointer=/package_published_by_this_run |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packageVersion |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/archiveSha256 |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/packagedDllSha256 |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/archiveSha256 |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/observation/exactDependencies/0/packagedDllSha256 |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/4/packageId |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/4/historicalSourceVersion |
| 21 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/observed/packages/4/projectReferences/0 |
| 22 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.codecs/packages.lock.json` | pointer=/dependencies/net10.0/dw.tools.math.ir/type |
| 23 | `require.json-value-equals` | Verified | `src/projects/dw.tools.math.codecs/packages.lock.json` | pointer=/dependencies/net10.0/dw.quantities/type |
| 24 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 25 | `require.text-contains` | Verified | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | semantic text found |
| 26 | `require.text-contains` | Verified | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | semantic text found |
| 27 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate.md` | path is absent |
| 29 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCodecsCandidate.cs` | path is absent |
| 30 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj` | path is absent |
| 31 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props` | path is absent |
| 32 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/Program.cs` | path is absent |
| 33 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | path is absent |
| 34 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | path is absent |
| 35 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f28-codecs-candidate.md` | bytes=5230 |
| 36 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCodecsCandidate.cs` | bytes=14046 |
| 37 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj` | bytes=331 |
| 38 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props` | bytes=241 |
| 39 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-candidate/Program.cs` | bytes=27599 |
| 40 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | bytes=4453 |
| 41 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | file exists |
| 42 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate.md` | semantic text found |
| 43 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/Program.cs` | semantic text found |
| 44 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/Program.cs` | semantic text found |
| 45 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | semantic text found |
| 46 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj` | semantic text found |
| 47 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props` | semantic text found |
| 48 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | bytes=18784 |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 50 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS161 hierarchy=exact
- After SHA-256: `2c87e5b5e9bbda209311f4b3ce61fd8489e2e462782ca9553895b8bbccd8e958`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS160/attempt-001.json`
- Summary: run=RS160 attempt=1 lineage=exact
- After SHA-256: `724b89b5d3aa6efae35082775ec274bad4005cf93f1bf426d82128c922d4916f`

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
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
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
- Summary: pointer=/observed/packages/4/packageId

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/4/historicalSourceVersion

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/observed/packages/4/projectReferences/0

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.tools.math.ir/type

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/packages.lock.json`
- Summary: pointer=/dependencies/net10.0/dw.quantities/type

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 25. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate.md`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCodecsCandidate.cs`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props`
- Summary: path is absent

## 32. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Program.cs`
- Summary: path is absent

## 33. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: path is absent

## 34. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: path is absent

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate.md`
- Summary: bytes=5230
- After SHA-256: `36ce78fd74039e08bca1d70bc1e55a89df0dd4b073e3bf86b43b5419050f3e29`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCodecsCandidate.cs`
- Summary: bytes=14046
- After SHA-256: `06579e569b173bd965bffb0b1fbd4792a54b1102526da79fec6284ce743b3d5c`

## 37. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 38. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 39. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Program.cs`
- Summary: bytes=27599
- After SHA-256: `ad54d5da62a53c53b2dfcbec9da6ab4630cdd2d7d0b16be40590bfded101cf22`

## 40. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: bytes=4453
- After SHA-256: `951ff56b14b066a8ee42c0fa992dcf5afe59684ed4086f5a9652ace50f1b3d54`

## 41. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: file exists

## 42. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 44. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 45. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 46. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/dw.tools.math.nuget-codecs-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 47. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 48. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: bytes=18784
- After SHA-256: `6281b91325c0f4e2fc844aadc73892105b85a07b78631a31d1538d7e2e77fa6b`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 50. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

