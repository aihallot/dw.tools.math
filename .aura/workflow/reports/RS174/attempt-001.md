# RS174 Attempt 001 Payload Operations

- Operations: 33
- Changed: 7
- No change: 0
- Verified: 26
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS174 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS173/attempt-001.json` | run=RS173 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | pointer=/tests/passed |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/observation/archiveSha256 |
| 11 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 12 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs` | semantic text found |
| 13 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/StandardUnitCatalog.cs` | semantic text found |
| 14 | `require.file-missing` | Verified | `docs/distribution/electrochemical-nuget-candidate.md` | path is absent |
| 15 | `require.file-missing` | Verified | `docs/distribution/ValidateElectrochemicalNugetCandidate.cs` | path is absent |
| 16 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj` | path is absent |
| 17 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/Directory.Packages.props` | path is absent |
| 18 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | path is absent |
| 21 | `files.replace-from-staged` | Changed | `docs/distribution/electrochemical-nuget-candidate.md` | bytes=3549 |
| 22 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateElectrochemicalNugetCandidate.cs` | bytes=7411 |
| 23 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj` | bytes=331 |
| 24 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-candidate/Directory.Packages.props` | bytes=241 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs` | bytes=30432 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs` | bytes=8095 |
| 27 | `require.text-contains` | Verified | `docs/distribution/electrochemical-nuget-candidate.md` | semantic text found |
| 28 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj` | semantic text found |
| 32 | `files.write-complete` | Changed | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | bytes=17887 |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS174 hierarchy=exact
- After SHA-256: `7800c8514155fe401788a9263f74ad25a4f8ce4a1e438073bb295ed609ddb0af`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS173/attempt-001.json`
- Summary: run=RS173 attempt=1 lineage=exact
- After SHA-256: `43e718822567185c91ecc8d1a5691a34d7bcc67959a75cbec2bf50047597de06`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M1 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: pointer=/tests/passed

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/observation/archiveSha256

## 11. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 12. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 13. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/StandardUnitCatalog.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 14. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate.md`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateElectrochemicalNugetCandidate.cs`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Directory.Packages.props`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: path is absent

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-nuget-candidate.md`
- Summary: bytes=3549
- After SHA-256: `2027adaa4720aa645d7252e8514636dd1189b104489f151620ba0bc73e15c7bc`

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateElectrochemicalNugetCandidate.cs`
- Summary: bytes=7411
- After SHA-256: `8ebb2b8d323a68a28519414932cd29614d209153018a5053a9324fb90ac25c9b`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs`
- Summary: bytes=30432
- After SHA-256: `e69d79a0503fc3c24830401edda61ef4c0efab48b7d8c6cae1e7eb938158f5ec`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs`
- Summary: bytes=8095
- After SHA-256: `09138f76c10eeb50bebd50d1caf4adf35938362452c4bfd7dbc00c084fe492db`

## 27. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/fixtures/ElectrochemicalBinaryConsumer.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-candidate/dw.tools.math.nuget-electrochemical-candidate.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: bytes=17887
- After SHA-256: `1c0a309ff2303bd38e532b53ac7409a80c0e675efb934a5f0a714708e219a70e`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M1 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

