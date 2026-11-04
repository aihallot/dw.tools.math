# RS144 Attempt 001 Payload Operations

- Operations: 40
- Changed: 7
- No change: 0
- Verified: 33
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS144 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS143/attempt-001.json` | run=RS143 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/physically_published_package_verified |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/transitive_packages_verified |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/observation/compiledConsumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/observation/archiveSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | pointer=/package_count |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj` | semantic text found |
| 17 | `require.text-contains` | Verified | `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj` | semantic text found |
| 18 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | file exists |
| 19 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs` | file exists |
| 20 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate.md` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetIrCandidate.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-candidate/dw.tools.math.nuget-ir-candidate.csproj` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-candidate/Program.cs` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | path is absent |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f11-ir-candidate.md` | bytes=5445 |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetIrCandidate.cs` | bytes=11805 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-candidate/dw.tools.math.nuget-ir-candidate.csproj` | bytes=331 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props` | bytes=241 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-candidate/Program.cs` | bytes=21984 |
| 32 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` | bytes=6261 |
| 33 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate.md` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-candidate/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-candidate/Program.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props` | semantic text found |
| 38 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | bytes=17620 |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS144 hierarchy=exact
- After SHA-256: `a6eb4b9fa1fdee2495206350a90352abfdea21811a7ca2555b7f27438e817876`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS143/attempt-001.json`
- Summary: run=RS143 attempt=1 lineage=exact
- After SHA-256: `b2df47d9afaa7eb69d3be29b054a373b4aaf68c7ff4fe5d00d7bebf4b0854b68`

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
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/transitive_packages_verified

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/observation/compiledConsumerChecks

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: pointer=/package_count

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 18. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: file exists

## 19. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs`
- Summary: file exists

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate.md`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetIrCandidate.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/dw.tools.math.nuget-ir-candidate.csproj`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Program.cs`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: path is absent

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate.md`
- Summary: bytes=5445
- After SHA-256: `78d17a4e30d104c504ba4eade9bee2c56e7e0c1feb22585fa2496a3b0aea37d6`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetIrCandidate.cs`
- Summary: bytes=11805
- After SHA-256: `b7028fa806d7dca4c6aad30a6ea5aa001759687992b878556be8ec7d74313120`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-candidate/dw.tools.math.nuget-ir-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Program.cs`
- Summary: bytes=21984
- After SHA-256: `f9d3cbc6b8fe5e2cbe8f3f6c751d2850bb3bae6195cd0eb519bc102d0ca71b6a`

## 32. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs`
- Summary: bytes=6261
- After SHA-256: `9afec6096c5fa9c8b01194157db7b677082717c471f56309d984caa738adc7f1`

## 33. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: bytes=17620
- After SHA-256: `c631fe017af851e797a15c568f1c1442ae48fcf231577efc0cca222aed3451b3`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

