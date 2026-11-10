# RS173 Attempt 001 Payload Operations

- Operations: 28
- Changed: 4
- No change: 0
- Verified: 24
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS173 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS172/attempt-001.json` | run=RS172 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f38-numerics-reconciled.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | pointer=/observation/packageVersion |
| 10 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs` | semantic text found |
| 11 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/StandardUnitCatalog.cs` | semantic text found |
| 12 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/dw.quantities.standard.csproj` | semantic text found |
| 13 | `require.file-missing` | Verified | `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs` | path is absent |
| 14 | `require.file-missing` | Verified | `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs` | path is absent |
| 15 | `require.file-missing` | Verified | `docs/distribution/electrochemical-unit-profile.md` | path is absent |
| 16 | `require.file-missing` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | path is absent |
| 17 | `files.replace-from-staged` | Changed | `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs` | bytes=2480 |
| 18 | `files.replace-from-staged` | Changed | `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs` | bytes=7382 |
| 19 | `files.replace-from-staged` | Changed | `docs/distribution/electrochemical-unit-profile.md` | bytes=3863 |
| 20 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs` | semantic text found |
| 21 | `require.text-contains` | Verified | `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs` | semantic text found |
| 22 | `require.text-contains` | Verified | `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs` | semantic text found |
| 23 | `require.text-contains` | Verified | `docs/distribution/electrochemical-unit-profile.md` | semantic text found |
| 24 | `files.write-complete` | Changed | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | bytes=4961 |
| 25 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 26 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | pointer=/status |
| 27 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | pointer=/tests/passed |
| 28 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-unit-profile-source-qualified.json` | pointer=/package_published |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS173 hierarchy=exact
- After SHA-256: `9bf7060d963d1e6106314f43cb978222b85418a9783d7171fd09dbdacbe70320`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS172/attempt-001.json`
- Summary: run=RS172 attempt=1 lineage=exact
- After SHA-256: `d93e116980f375adee23e3a28c85b88eb29601a53b78262de7c91c78586ccd0f`

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
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: pointer=/observation/packageVersion

## 10. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/InheritedStandardUnitCatalog.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 11. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/StandardUnitCatalog.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 12. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/dw.quantities.standard.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 13. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs`
- Summary: path is absent

## 14. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile.md`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: path is absent

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs`
- Summary: bytes=2480
- After SHA-256: `c64f9b0f9595889190e4d383853ba981b61dd241bd7565673d28e7d31b203b6a`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs`
- Summary: bytes=7382
- After SHA-256: `1e4607f1ccecee51ea3f3b98b99e3cf37b220b5347d01870e196903b093c8880`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-unit-profile.md`
- Summary: bytes=3863
- After SHA-256: `651a5ad10dae6f564734319edf10051560fafa9fe684659355374072d3beb31d`

## 20. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 21. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.quantities.standard/ElectrochemicalUnitProfile.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 22. `require.text-contains`

- Status: `Verified`
- Path: `tests/projects/dw.quantities.tests/ElectrochemicalUnitProfileTests.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: bytes=4961
- After SHA-256: `daf3c7d730885831b777af19f0ef280b824a1cd975cab071c0f2df7f7a99a364`

## 25. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M1 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: pointer=/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: pointer=/tests/passed

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-unit-profile-source-qualified.json`
- Summary: pointer=/package_published

