# RS271 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS271 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS270/attempt-001.json` | run=RS270 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3-W02-C03 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9-W01-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM9StatisticsPrivateReadiness.cs` | bytes=6708 |
| 8 | `files.replace-from-staged` | Changed | `docs/distribution/m9-statistics-private-publication-readiness.md` | bytes=2324 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS271 hierarchy=exact
- After SHA-256: `5afee88b8d195ec893188ac2e5b6afd3e117f95bac98bed88b487c02a1d0277f`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS270/attempt-001.json`
- Summary: run=RS270 attempt=1 lineage=exact
- After SHA-256: `b8c60a1bcbd3596f4270ad797edc1dc37aa577b248203f48f9f14e9c9f3b0091`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3-W02-C03 state=done
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9-W01-C01 state=not-ready
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/engineering/ProbeM9StatisticsPrivateReadiness.cs`
- Summary: bytes=6708
- After SHA-256: `9d2da9294bd56e4f8e35332bed1a38bd2c6567c4d8fcc47dfe744bce5d2a79f9`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m9-statistics-private-publication-readiness.md`
- Summary: bytes=2324
- After SHA-256: `1a062b2cf1fa5413e369f505653402bdb4bcd34045a83bd749fa0c2aef180af9`

