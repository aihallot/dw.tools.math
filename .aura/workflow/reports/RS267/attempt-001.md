# RS267 Attempt 001 Payload Operations

- Operations: 8
- Changed: 4
- No change: 0
- Verified: 4
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS267 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS266/attempt-001.json` | run=RS266 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3-W02-C03 state=done |
| 5 | `files.replace-from-staged` | Changed | `docs/planning/backlog.json` | bytes=534400 |
| 6 | `files.replace-from-staged` | Changed | `.aura/workflow/plan/project.json` | bytes=191868 |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=11364 |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/ValidateAssessaM9Admission.cs` | bytes=3090 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS267 hierarchy=exact
- After SHA-256: `f2e597cbbaecafa0ebd01e972a19c04feb2fd00a44cb9b08d438c4a193dc29fd`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS266/attempt-001.json`
- Summary: run=RS266 attempt=1 lineage=exact
- After SHA-256: `defddef44593ba54982c1d030507dc28480787e64356e080e59e3393264556f3`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=not-ready
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3-W02-C03 state=done
- After SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`

## 5. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: bytes=534400
- Before SHA-256: `804ceda0c3c6e5d85f8d9c22c51f797549396822dfb35ab287cc8be05b6cc18b`
- After SHA-256: `ea3cf8fbdbfb6b92e13a92909679395e3c81d875bdd964e2d346426a03cb1782`

## 6. `files.replace-from-staged`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: bytes=191868
- Before SHA-256: `ba92e3d481499113d33c5319ecc52e6283b22f5c616edde206a16a38bb5de0c2`
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=11364
- Before SHA-256: `758cc1c167cb97046cbc6430cc287cc13751d4df717dc06e106452e4c38fad43`
- After SHA-256: `0f4868693466a20c29d6198b48fe38fd3eb600f99b0b0bad21ce6bdbcf7ec97e`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateAssessaM9Admission.cs`
- Summary: bytes=3090
- After SHA-256: `da11e51c975896197a481bb018b7a7a7b430a091ad2d1c5451ba89ce7cbcdedd`

