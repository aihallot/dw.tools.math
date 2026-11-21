# RS273 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS273 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS272/attempt-001.json` | run=RS272 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3-W02-C03 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9-W01-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/engineering/ProbeM9PublishedStatisticsReadback.cs` | bytes=3321 |
| 8 | `files.replace-from-staged` | Changed | `docs/distribution/m9-statistics-physical-private-publication-readback.md` | bytes=1805 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS273 hierarchy=exact
- After SHA-256: `eab2895459444e0407af8f96531c328b879161ebcb7f432fec480a32b9c8a3b9`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS272/attempt-001.json`
- Summary: run=RS272 attempt=1 lineage=exact
- After SHA-256: `45b2938cfb7ca2762ead6fc24c70f3d7c4ce6111b7af074b0b22d617db43ab78`

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
- Path: `docs/engineering/ProbeM9PublishedStatisticsReadback.cs`
- Summary: bytes=3321
- After SHA-256: `3a65f9367eb9463aaa74ca8cf11648c01dca44318ba24a27f2f4f98952ee9996`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m9-statistics-physical-private-publication-readback.md`
- Summary: bytes=1805
- After SHA-256: `6257e4d27b8608f721275e0900cdfe1e632b34a924d2f993e0e5b9ce5efe37fa`

