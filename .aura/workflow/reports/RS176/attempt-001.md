# RS176 Attempt 001 Payload Operations

- Operations: 17
- Changed: 3
- No change: 0
- Verified: 14
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS176 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS175/attempt-001.json` | run=RS175 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M1 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-publisher-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-publisher-qualified.json` | pointer=/publisher/controlsPassed |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-publisher-qualified.json` | pointer=/package_published |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/electrochemical-nuget-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.file-missing` | Verified | `docs/distribution/electrochemical-local-publication.md` | path is absent |
| 13 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-release/publication/dw.tools.math.nuget-electrochemical-publication.csproj` | path is absent |
| 14 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-electrochemical-release/publication/Program.cs` | path is absent |
| 15 | `files.replace-from-staged` | Changed | `docs/distribution/electrochemical-local-publication.md` | bytes=1872 |
| 16 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-release/publication/dw.tools.math.nuget-electrochemical-publication.csproj` | bytes=436 |
| 17 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-electrochemical-release/publication/Program.cs` | bytes=8204 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS176 hierarchy=exact
- After SHA-256: `ad0a96060da92d5a27203e1100db08e17f6f5f1083b97f4220883c2dfb9ee9f6`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS175/attempt-001.json`
- Summary: run=RS175 attempt=1 lineage=exact
- After SHA-256: `d6af7e98c74959c1d08cc7e1ca4b8a8a5554c98fb23855377600c3bf102db39c`

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
- Path: `docs/distribution/electrochemical-publisher-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-publisher-qualified.json`
- Summary: pointer=/publisher/controlsPassed

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-publisher-qualified.json`
- Summary: pointer=/package_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-nuget-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/electrochemical-local-publication.md`
- Summary: path is absent

## 13. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/publication/dw.tools.math.nuget-electrochemical-publication.csproj`
- Summary: path is absent

## 14. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/publication/Program.cs`
- Summary: path is absent

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/electrochemical-local-publication.md`
- Summary: bytes=1872
- After SHA-256: `514be070a12b39339676285b6ae72ed66124ee0dab6637a8e805f5374ea8ba74`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/publication/dw.tools.math.nuget-electrochemical-publication.csproj`
- Summary: bytes=436
- After SHA-256: `d8ffe813b1f4d16c63c93b32cf81886955fe6d3be8519391b6bab7ac28cc7f7f`

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-electrochemical-release/publication/Program.cs`
- Summary: bytes=8204
- After SHA-256: `814805853a26740f29c91d14f9edfcd352d2dc1239ffa6ce691ef913fc13908d`

