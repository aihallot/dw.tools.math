# RS172 Attempt 001 Payload Operations

- Operations: 24
- Changed: 3
- No change: 0
- Verified: 21
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS172 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS171/attempt-001.json` | run=RS171 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS171/attempt-001.json` | pointer=/outcome |
| 9 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS171/attempt-001.json` | pointer=/attempt |
| 10 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS171/attempt-001.json` | pointer=/validations/0/Id |
| 11 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS171/attempt-001.json` | pointer=/validations/0/ExitCode |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f37-numerics-release-qualified.json` | pointer=/package_published |
| 14 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f38-numerics-reconciled.json` | path is absent |
| 15 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f38-numerics-reconciled.md` | path is absent |
| 16 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetNumericsF38Receipt.cs` | path is absent |
| 17 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f38-numerics-reconciled.json` | path is absent |
| 18 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f38-numerics-reconciled.md` | bytes=2562 |
| 19 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetNumericsF38Receipt.cs` | bytes=8857 |
| 20 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f38-numerics-reconciled.json` | bytes=5527 |
| 21 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f38-numerics-reconciled.md` | semantic text found |
| 22 | `require.text-contains` | Verified | `docs/distribution/ValidateNugetNumericsF38Receipt.cs` | semantic text found |
| 23 | `require.text-contains` | Verified | `docs/distribution/ValidateNugetNumericsF38Receipt.cs` | semantic text found |
| 24 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS172 hierarchy=exact
- After SHA-256: `90c10c9727d6bfcb863f2708c3220041969e546a1e3436587616ff365bc77265`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS171/attempt-001.json`
- Summary: run=RS171 attempt=1 lineage=exact
- After SHA-256: `c1e185a6d73f4b444035816966a20364853a529333dec95314a076a68edcd5c9`

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
- Path: `.aura/workflow/reports/RS171/attempt-001.json`
- Summary: pointer=/outcome

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS171/attempt-001.json`
- Summary: pointer=/attempt

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS171/attempt-001.json`
- Summary: pointer=/validations/0/Id

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS171/attempt-001.json`
- Summary: pointer=/validations/0/ExitCode

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f37-numerics-release-qualified.json`
- Summary: pointer=/package_published

## 14. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.json`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.md`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetNumericsF38Receipt.cs`
- Summary: path is absent

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.json`
- Summary: path is absent

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.md`
- Summary: bytes=2562
- After SHA-256: `8760855d51c3822559db2eeb7a3a2dacdc7638d2cad085a840fd27971282cb41`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetNumericsF38Receipt.cs`
- Summary: bytes=8857
- After SHA-256: `963ffa69fe35d0f48541dc057b9bdd4726b2a65ab1f8dcc293b40253f38072cc`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.json`
- Summary: bytes=5527
- After SHA-256: `32c1895280dd28391d56d6f94b6ef2f38c6ae65b1a64e466ab61e33494c77a76`

## 21. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f38-numerics-reconciled.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 22. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetNumericsF38Receipt.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetNumericsF38Receipt.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

