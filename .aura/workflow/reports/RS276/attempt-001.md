# RS276 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS276 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS275/attempt-001.json` | run=RS275 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3-W02-C03 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9-W01-C01 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m9-w01-c01-evidence-lifecycle-assessment.md` | bytes=3078 |
| 8 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM9EvidenceLifecycleAssessment.cs` | bytes=3942 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS276 hierarchy=exact
- After SHA-256: `940debc4223ad9d0b1a2ccb5d9ba8ea145f01d21bad33c8cf5ff80d86af6ee64`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS275/attempt-001.json`
- Summary: run=RS275 attempt=1 lineage=exact
- After SHA-256: `34fa85522bf4c9c8f395064ff9b61e080b252e59b6db199af1a2cd6569b2b39a`

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
- Path: `docs/planning/decisions/m9-w01-c01-evidence-lifecycle-assessment.md`
- Summary: bytes=3078
- After SHA-256: `66310604c13711cd1fa272c29b9aa763fe228ee344718eadc67a921444bbac2f`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM9EvidenceLifecycleAssessment.cs`
- Summary: bytes=3942
- After SHA-256: `f8b5fc89bc29bd655ebdbafa3a7ef41616f0e1c9bc4904704d224b4b9a9b7b3f`

