# RS287 Attempt 001 Payload Operations

- Operations: 12
- Changed: 3
- No change: 0
- Verified: 9
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS287 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS286/attempt-001.json` | run=RS286 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1-R state=in-progress |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1-G state=not-ready |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1-V state=not-ready |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 8 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 9 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 10 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs` | bytes=8012 |
| 11 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=12445 |
| 12 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w01-c01-t1-strict-binding-metadata-rs287.md` | bytes=2175 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS287 hierarchy=exact
- After SHA-256: `41965f390f23f429c42542023432d265abe0da231aafd50a61ecae10e04118f5`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS286/attempt-001.json`
- Summary: run=RS286 attempt=1 lineage=exact
- After SHA-256: `fb5a9d0d18d528bcb2aee559bfa12dd1154797e77873f416cd6094cc35d0efea`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1-R state=in-progress
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1-G state=not-ready
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1-V state=not-ready
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=not-ready
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 8. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `8ddeb8eb2ec669ca0773e840c0a8c6955f229c2048c9b9f2db9ab05cd71726f1`

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 10. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8StrictBindingMetadata.cs`
- Summary: bytes=8012
- After SHA-256: `f1bab7d49942d15275bb51c30021521244ca2e0043743a4147dfd66acf5759bd`

## 11. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=12445
- Before SHA-256: `e122a0e4e71cd9a058b2c9a104f67eba53bd2b203d8fc7201ec52085fab49baf`
- After SHA-256: `6baac7b7e681b9e1386f783863fd597228d7e5b5ae5a16ff28ebe63608490694`

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w01-c01-t1-strict-binding-metadata-rs287.md`
- Summary: bytes=2175
- After SHA-256: `5360fc8fc4bb7e9556fff12d2158f80f7fc6c460acf96c18b98a7f6f6d44c361`

