# RS294 Attempt 001 Payload Operations

- Operations: 15
- Changed: 3
- No change: 0
- Verified: 12
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS294 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS293/attempt-001.json` | run=RS293 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1 state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2 state=in-progress |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2-R state=in-progress |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2-G state=not-ready |
| 8 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2-V state=not-ready |
| 9 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 10 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 11 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 12 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 13 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs` | bytes=16870 |
| 14 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=24852 |
| 15 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w01-c01-t2-real-clr-admission-rs294.md` | bytes=2408 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS294 hierarchy=exact
- After SHA-256: `f455369fe25a4b707f279b4d45768920ddc4e41de312667f34f19865a72f8564`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS293/attempt-001.json`
- Summary: run=RS293 attempt=1 lineage=exact
- After SHA-256: `0b4037362da755e766b3bad8d5e52788c81aa1ce44404a29cd5deec6788245b7`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1 state=done
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2 state=in-progress
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2-R state=in-progress
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2-G state=not-ready
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 8. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2-V state=not-ready
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 9. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 10. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=not-ready
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 11. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `5ef0a9c55156d4fb8c8a0d9d59f840cf6a9ea15621f76f7860f5e92e937bbf4f`

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs`
- Summary: bytes=16870
- Before SHA-256: `41b658319b6086c8b1b2ee83ffbaa78514029fda27fc828d9d8d9bd8f787b09d`
- After SHA-256: `45ea046a49f1933bb79ea317b245de5781d832a852fcfb65af98ad521f226368`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=24852
- Before SHA-256: `a8c2545a7b8ab63e4c04b496f0b9f7c5dedd03f8d8632d79ccd9ec8ea62fa369`
- After SHA-256: `1262ca0d3df3975ebcb6304be679d726cd8f42df658e5f693fdacfe8ed7498d7`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w01-c01-t2-real-clr-admission-rs294.md`
- Summary: bytes=2408
- After SHA-256: `b7857be57e39eaa8d2ee4973f1b1c1f62a99c9faeecf43ecd5aa06838e34feb4`

