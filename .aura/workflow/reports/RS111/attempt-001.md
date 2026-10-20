# RS111 Attempt 001 Payload Operations

- Operations: 123
- Changed: 17
- No change: 1
- Verified: 105
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS111 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS110/attempt-001.json` | run=RS110 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/split_parent |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/native_nodes_after |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/new_phase_count |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/new_task_count |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/new_subtask_count |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/dag_split_active |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/plot_split_active |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/dag_oracles_executed |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/dag_runtime_api_delivered |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/plot_renderer_delivered |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/m7_release_done |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C02-T2-qualified.json` | pointer=/status |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C02-T2-qualified.json` | pointer=/m7_w01_c02_parent_done |
| 17 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C02-T2-qualified.json` | pointer=/advanced_tensor_api_delivered |
| 18 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/id |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/id |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/16/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/52/id |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/52/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/kind |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/size |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/id |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/kind |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/size |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/parent_chunk |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/status |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/0/id |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/0/status |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/1/status |
| 42 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/id |
| 43 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/status |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/56/status |
| 45 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/57/status |
| 46 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/0/subtasks/0/status |
| 47 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/1/subtasks/0/status |
| 48 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/0/subtasks/1/status |
| 49 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/1/subtasks/1/status |
| 50 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/0/subtasks/2/status |
| 51 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/tasks/1/subtasks/2/status |
| 52 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=in-progress |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=in-progress |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01 state=done |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C02 state=done |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03 state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01 state=in-progress |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 state=not-ready |
| 61 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1 state=not-ready |
| 62 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-A state=not-ready |
| 63 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-B state=not-ready |
| 64 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-C state=not-ready |
| 65 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T2 state=not-ready |
| 66 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T2-A state=not-ready |
| 67 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T2-B state=not-ready |
| 68 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T2-C state=not-ready |
| 69 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 state=not-ready |
| 70 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2 state=not-ready |
| 71 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | path is absent |
| 72 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | path is absent |
| 73 | `require.file-missing` | Verified | `docs/planning/ValidateM7DagAdmission.cs` | path is absent |
| 74 | `require.file-missing` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json` | path is absent |
| 75 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | bytes=4063 |
| 76 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | bytes=3184 |
| 77 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM7DagAdmission.cs` | bytes=13723 |
| 78 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 79 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 80 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 81 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 82 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 83 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 84 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 85 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 86 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 87 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-t1.md` | semantic text found |
| 88 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 89 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 90 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 91 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 92 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 93 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 94 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 95 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md` | semantic text found |
| 96 | `require.text-contains` | Verified | `docs/planning/ValidateM7DagAdmission.cs` | semantic text found |
| 97 | `require.text-contains` | Verified | `docs/planning/ValidateM7DagAdmission.cs` | semantic text found |
| 98 | `files.write-complete` | Changed | `docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json` | bytes=3366 |
| 99 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 100 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1 not-ready->ready |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1 ready->in-progress |
| 102 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-A not-ready->ready |
| 103 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-A ready->in-progress |
| 104 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-A in-progress->implemented->validated->done |
| 105 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-B not-ready->ready |
| 106 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-B ready->in-progress |
| 107 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-B in-progress->implemented->validated->done |
| 108 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-C not-ready->ready |
| 109 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-C ready->in-progress |
| 110 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-C in-progress->implemented->validated->done |
| 111 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1 in-progress->implemented->validated->done |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=in-progress |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=in-progress |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03 state=not-ready |
| 115 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01 state=in-progress |
| 116 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 state=not-ready |
| 117 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1 state=done |
| 118 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-A state=done |
| 119 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-B state=done |
| 120 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T1-C state=done |
| 121 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01-T2 state=not-ready |
| 122 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 state=not-ready |
| 123 | `json.edit-object` | NoChange | `.aura/workflow/state.json` | semantic content already matches |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS111 hierarchy=exact
- After SHA-256: `337379548546bb8da3e6d326ff52d112679cfd7ac6b32a5d4ea0b6f68fecac27`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS110/attempt-001.json`
- Summary: run=RS110 attempt=1 lineage=exact
- After SHA-256: `7cb9cc9e00fa4aa6fd019e68d7eaf2f425ec54db80deeda3dbbc5554815597b1`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/split_parent

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/native_nodes_after

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/new_phase_count

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/new_task_count

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/new_subtask_count

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/dag_split_active

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/plot_split_active

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/dag_oracles_executed

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/dag_runtime_api_delivered

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/plot_renderer_delivered

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/m7_release_done

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C02-T2-qualified.json`
- Summary: pointer=/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C02-T2-qualified.json`
- Summary: pointer=/m7_w01_c02_parent_done

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C02-T2-qualified.json`
- Summary: pointer=/advanced_tensor_api_delivered

## 18. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/id

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/id

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/16/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/52/id

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/52/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/id

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/kind

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/size

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/id

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/kind

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/size

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/parent_chunk

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/status

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/0/id

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/0/status

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/1/status

## 42. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/id

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/status

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/56/status

## 45. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/57/status

## 46. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/0/subtasks/0/status

## 47. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/1/subtasks/0/status

## 48. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/0/subtasks/1/status

## 49. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/1/subtasks/1/status

## 50. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/0/subtasks/2/status

## 51. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/tasks/1/subtasks/2/status

## 52. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=in-progress
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=in-progress
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 state=done
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C02 state=done
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01 state=in-progress
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 61. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 62. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-A state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 63. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-B state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 64. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-C state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 65. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T2 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 66. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T2-A state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 67. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T2-B state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 68. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T2-C state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 69. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 70. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2 state=not-ready
- After SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`

## 71. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: path is absent

## 72. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: path is absent

## 73. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidateM7DagAdmission.cs`
- Summary: path is absent

## 74. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json`
- Summary: path is absent

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: bytes=4063
- After SHA-256: `1208a6f60dced14dbc85ef050f0939e0f95f3b072cf7d92b4031b08805dbbec0`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: bytes=3184
- After SHA-256: `1de9039d07f24ee2092a56007d4f3db03e1c76e876d2e34eceb0e29ef0454e9c`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM7DagAdmission.cs`
- Summary: bytes=13723
- After SHA-256: `c8c5acf9608504b350e51027b4af60d718058fa3a25c9d5859e5cfdc67020d92`

## 78. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 79. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 80. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 81. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 82. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 83. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 84. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 85. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 86. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 87. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 88. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 89. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 90. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 91. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 92. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 93. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 94. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 95. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s01-owner-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 96. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7DagAdmission.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 97. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7DagAdmission.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 98. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json`
- Summary: bytes=3366
- After SHA-256: `fa6087c9bef8e50fce06dc8b9fcd7a82627f5decc73fbcc957f2bc71a88d6ff7`

## 99. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `c99774247244cd76bf488ab33f5d8ea4d15442bc739573798ba7d336ee80008c`
- After SHA-256: `9d903262a088f31902b012fbc449c1e7ea15c95f917689c8143d3d9a956bba76`

## 100. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1 not-ready->ready
- Before SHA-256: `00ac0c2dc6e85b4a3f608bb595ac587e4a3af5f76d063e9cbe7ff5a88df7445f`
- After SHA-256: `50ba6e6c408368726f81e0908f20123fa9dc22951e1c4d810f1c905fb5a2e324`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1 ready->in-progress
- Before SHA-256: `50ba6e6c408368726f81e0908f20123fa9dc22951e1c4d810f1c905fb5a2e324`
- After SHA-256: `a27f52498ade9d1597dae9224e1fe00ac767ad312dc17537b47a0e5a927296d1`

## 102. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-A not-ready->ready
- Before SHA-256: `a27f52498ade9d1597dae9224e1fe00ac767ad312dc17537b47a0e5a927296d1`
- After SHA-256: `c4ba26c8c6a81cd78890c6aadf4defacb6dc10f3f8c1d9d41be7bc26b60bbc59`

## 103. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-A ready->in-progress
- Before SHA-256: `c4ba26c8c6a81cd78890c6aadf4defacb6dc10f3f8c1d9d41be7bc26b60bbc59`
- After SHA-256: `b95567b93ec6c1fc706ee2d3a30e71564974e27d51e75632fa1c2bb02debf85b`

## 104. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-A in-progress->implemented->validated->done
- Before SHA-256: `b95567b93ec6c1fc706ee2d3a30e71564974e27d51e75632fa1c2bb02debf85b`
- After SHA-256: `d19ed4de77f93ec6c098e983dce8e5a5d4a486eb42a83ca077849ac5e016630b`

## 105. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-B not-ready->ready
- Before SHA-256: `d19ed4de77f93ec6c098e983dce8e5a5d4a486eb42a83ca077849ac5e016630b`
- After SHA-256: `0b73bc9608cddf8351d6f6a6ddc2488466b290cdcfa3c4f3f138049f02f6914a`

## 106. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-B ready->in-progress
- Before SHA-256: `0b73bc9608cddf8351d6f6a6ddc2488466b290cdcfa3c4f3f138049f02f6914a`
- After SHA-256: `93d67331961df77ae1024a51657011036afe55b4a50d08cd6e890855aa91c904`

## 107. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-B in-progress->implemented->validated->done
- Before SHA-256: `93d67331961df77ae1024a51657011036afe55b4a50d08cd6e890855aa91c904`
- After SHA-256: `4e99f138709b802bd253b8aab6e51c16f5b1bfa00b30da98e8da62989e6e5eb2`

## 108. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-C not-ready->ready
- Before SHA-256: `4e99f138709b802bd253b8aab6e51c16f5b1bfa00b30da98e8da62989e6e5eb2`
- After SHA-256: `c04125535749da92dd2adb4814fe52ff47891cd1fdb802a92eb30686c13778cc`

## 109. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-C ready->in-progress
- Before SHA-256: `c04125535749da92dd2adb4814fe52ff47891cd1fdb802a92eb30686c13778cc`
- After SHA-256: `cb4d51fbd40192b41068b967e6b08e9c0c1a8ce1b4f4c81c73a330140e83b454`

## 110. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-C in-progress->implemented->validated->done
- Before SHA-256: `cb4d51fbd40192b41068b967e6b08e9c0c1a8ce1b4f4c81c73a330140e83b454`
- After SHA-256: `981878fc98905400fe5c24b58cf71c7e1ddfaec1ac19adb52da3a65adb44faf3`

## 111. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1 in-progress->implemented->validated->done
- Before SHA-256: `981878fc98905400fe5c24b58cf71c7e1ddfaec1ac19adb52da3a65adb44faf3`
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=in-progress
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=in-progress
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03 state=not-ready
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 115. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01 state=in-progress
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 116. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 state=not-ready
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 117. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1 state=done
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 118. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-A state=done
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 119. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-B state=done
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 120. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T1-C state=done
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 121. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01-T2 state=not-ready
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 122. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 state=not-ready
- After SHA-256: `0799e3b7dd17f4af3c8575496cddbe0183ede18f693fafa8c3f96ba62acfcc6c`

## 123. `json.edit-object`

- Status: `NoChange`
- Path: `.aura/workflow/state.json`
- Summary: semantic content already matches

