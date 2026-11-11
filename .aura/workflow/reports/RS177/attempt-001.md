# RS177 Attempt 001 Payload Operations

- Operations: 20
- Changed: 5
- No change: 0
- Verified: 15
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS177 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS176/attempt-001.json` | run=RS176 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/candidate/consumerChecks |
| 9 | `require.file-missing` | Verified | `tests/consumers/electrochemical-published-readback/electrochemical-published-readback.csproj` | path is absent |
| 10 | `require.file-missing` | Verified | `tests/consumers/electrochemical-published-readback/NuGet.Config` | path is absent |
| 11 | `require.file-missing` | Verified | `tests/consumers/electrochemical-published-readback/Program.cs` | path is absent |
| 12 | `require.file-missing` | Verified | `docs/distribution/ValidateElectrochemicalPublishedReadback.cs` | path is absent |
| 13 | `files.replace-from-staged` | Changed | `tests/consumers/electrochemical-published-readback/electrochemical-published-readback.csproj` | bytes=596 |
| 14 | `files.replace-from-staged` | Changed | `tests/consumers/electrochemical-published-readback/NuGet.Config` | bytes=515 |
| 15 | `files.replace-from-staged` | Changed | `tests/consumers/electrochemical-published-readback/Program.cs` | bytes=9249 |
| 16 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateElectrochemicalPublishedReadback.cs` | bytes=5181 |
| 17 | `require.file-missing` | Verified | `docs/distribution/electrochemical-published-readback.json` | path is absent |
| 18 | `files.write-complete` | Changed | `docs/distribution/electrochemical-published-readback.json` | bytes=15695 |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-published-readback.json` | pointer=/status |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-published-readback.json` | pointer=/publicationCommit |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS177 hierarchy=exact
- After SHA-256: `0aea617c96abb010a3cc3b4d615bc2d3b71d1567797a7c45caad197f199f9d31`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS176/attempt-001.json`
- Summary: run=RS176 attempt=1 lineage=exact
- After SHA-256: `0f40b6bdafb0ed7dbc69385d679bbd37eebfb573b5436e6d29db90161a345a1b`

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
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/candidate/consumerChecks

## 9. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/electrochemical-published-readback/electrochemical-published-readback.csproj`
- Summary: path is absent

## 10. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/electrochemical-published-readback/NuGet.Config`
- Summary: path is absent

## 11. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/electrochemical-published-readback/Program.cs`
- Summary: path is absent

## 12. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateElectrochemicalPublishedReadback.cs`
- Summary: path is absent

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/electrochemical-published-readback/electrochemical-published-readback.csproj`
- Summary: bytes=596
- After SHA-256: `c81765885280c53b07681d020780e3c700b76787372c1751c6fdb2ce8cb8a718`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/electrochemical-published-readback/NuGet.Config`
- Summary: bytes=515
- After SHA-256: `92c7d9fe7b93d80ad751aa7d9b924d323fe37c638cf8effee96e9b71c0c95e02`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/electrochemical-published-readback/Program.cs`
- Summary: bytes=9249
- After SHA-256: `7da0aae81a0363b468fd9b3f7219bed3ede46143dbf9bc311cbcd92cea0e4d31`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateElectrochemicalPublishedReadback.cs`
- Summary: bytes=5181
- After SHA-256: `6451b932895115cc3a2cdf28c82ce8e2e8fa2354d7433453d7e9229142ef12e4`

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: path is absent

## 18. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: bytes=15695
- After SHA-256: `fcdfdc9204927452384b5061ded541a7f9a4b78ca9904e674912108dce44d305`

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: pointer=/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-published-readback.json`
- Summary: pointer=/publicationCommit

