# RS113 Attempt 001 Payload Operations

- Operations: 123
- Changed: 20
- No change: 0
- Verified: 103
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS113 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS112/attempt-001.json` | run=RS112 attempt=1 lineage=exact |
| 3 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/disposition |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/prior_reference_root_value |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/s01_phase_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/plot_phase_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/parent_c03_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/m7_release_done |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/dag_runtime_api_delivered |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/plot_renderer_delivered |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json` | pointer=/aura_job_engine_copied |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/native_nodes_after |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/new_phase_count |
| 19 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/parent_chunk_still_planned |
| 20 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/plot_split_active |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M7-W01-C03-split-qualified.json` | pointer=/plot_renderer_delivered |
| 22 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/id |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/id |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/id |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/16/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/52/status |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/id |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/size |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/id |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/54/status |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/id |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/kind |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/size |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/parent_chunk |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/status |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/0/id |
| 42 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/0/status |
| 43 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/1/id |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/1/status |
| 45 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/56/status |
| 46 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/57/status |
| 47 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/0/subtasks/0/status |
| 48 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/1/subtasks/0/status |
| 49 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/0/subtasks/1/status |
| 50 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/1/subtasks/1/status |
| 51 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/0/subtasks/2/status |
| 52 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/55/tasks/1/subtasks/2/status |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=in-progress |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=in-progress |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01 state=done |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C02 state=done |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03 state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01 state=done |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 state=not-ready |
| 61 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 state=not-ready |
| 62 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-A state=not-ready |
| 63 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-B state=not-ready |
| 64 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-C state=not-ready |
| 65 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2 state=not-ready |
| 66 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2-A state=not-ready |
| 67 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2-B state=not-ready |
| 68 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2-C state=not-ready |
| 69 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-T1 state=not-ready |
| 70 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-T2 state=not-ready |
| 71 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W02 state=not-ready |
| 72 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | path is absent |
| 73 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | path is absent |
| 74 | `require.file-missing` | Verified | `docs/planning/ValidateM7PlotSampling.cs` | path is absent |
| 75 | `require.file-missing` | Verified | `docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json` | path is absent |
| 76 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | bytes=4102 |
| 77 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | bytes=3234 |
| 78 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM7PlotSampling.cs` | bytes=12739 |
| 79 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 80 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 81 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 82 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 83 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 84 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 85 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 86 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 87 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-t1.md` | semantic text found |
| 88 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 89 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 90 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 91 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 92 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 93 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c03-s02-ownership.md` | semantic text found |
| 94 | `require.text-contains` | Verified | `docs/planning/ValidateM7PlotSampling.cs` | semantic text found |
| 95 | `require.text-contains` | Verified | `docs/planning/ValidateM7PlotSampling.cs` | semantic text found |
| 96 | `files.write-complete` | Changed | `docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json` | bytes=3433 |
| 97 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 98 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 not-ready->ready |
| 99 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 ready->in-progress |
| 100 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 not-ready->ready |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 ready->in-progress |
| 102 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-A not-ready->ready |
| 103 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-A ready->in-progress |
| 104 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-A in-progress->implemented->validated->done |
| 105 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-B not-ready->ready |
| 106 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-B ready->in-progress |
| 107 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-B in-progress->implemented->validated->done |
| 108 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-C not-ready->ready |
| 109 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-C ready->in-progress |
| 110 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-C in-progress->implemented->validated->done |
| 111 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 in-progress->implemented->validated->done |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=in-progress |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=in-progress |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03 state=not-ready |
| 115 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S01 state=done |
| 116 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02 state=in-progress |
| 117 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1 state=done |
| 118 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-A state=done |
| 119 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-B state=done |
| 120 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T1-C state=done |
| 121 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-S02-T2 state=not-ready |
| 122 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03-T1 state=not-ready |
| 123 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS113 hierarchy=exact
- After SHA-256: `4640780a4ee609b168aa01ad708370d7b9c86b53cba13e2ee6fcce5196d346b8`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS112/attempt-001.json`
- Summary: run=RS112 attempt=1 lineage=exact
- After SHA-256: `705c0bf85fb51a174e57176c6588338a315b17f1fc8b1ee1dcef20ded7dad504`

## 3. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/disposition

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/prior_reference_root_value

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/s01_phase_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/plot_phase_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/parent_c03_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/m7_release_done

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/dag_runtime_api_delivered

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/plot_renderer_delivered

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json`
- Summary: pointer=/aura_job_engine_copied

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/native_nodes_after

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/new_phase_count

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/parent_chunk_still_planned

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/plot_split_active

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-split-qualified.json`
- Summary: pointer=/plot_renderer_delivered

## 22. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/id

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/id

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/id

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/16/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/52/status

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/id

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/size

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/id

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/54/status

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/id

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/kind

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/size

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/parent_chunk

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/status

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/0/id

## 42. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/0/status

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/1/id

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/1/status

## 45. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/56/status

## 46. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/57/status

## 47. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/0/subtasks/0/status

## 48. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/1/subtasks/0/status

## 49. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/0/subtasks/1/status

## 50. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/1/subtasks/1/status

## 51. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/0/subtasks/2/status

## 52. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/55/tasks/1/subtasks/2/status

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=in-progress
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=in-progress
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 state=done
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C02 state=done
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01 state=done
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 61. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 62. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-A state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 63. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-B state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 64. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-C state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 65. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 66. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2-A state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 67. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2-B state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 68. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2-C state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 69. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-T1 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 70. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-T2 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 71. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W02 state=not-ready
- After SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`

## 72. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: path is absent

## 73. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: path is absent

## 74. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PlotSampling.cs`
- Summary: path is absent

## 75. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json`
- Summary: path is absent

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: bytes=4102
- After SHA-256: `c9f08a604456b16c3c4beca5cd1dca4313bc25c8dd68ce0082d203cf5c7526d7`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: bytes=3234
- After SHA-256: `f613c5a41b176c61019a2623e535d916e5b6167ce5504705195361c3c3c4f70d`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM7PlotSampling.cs`
- Summary: bytes=12739
- After SHA-256: `58972677e8c5e94454ace7fcae19b39d3b610432fbd433700ab51147b6c960d4`

## 79. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 80. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 81. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 82. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 83. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 84. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 85. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 86. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 87. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-t1.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 88. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 89. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 90. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 91. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 92. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 93. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c03-s02-ownership.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 94. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PlotSampling.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 95. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PlotSampling.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 96. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json`
- Summary: bytes=3433
- After SHA-256: `820f63abf11abdcaa4da54426f5fd3ab258bdac43398c6ef87b859c97b540298`

## 97. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `15847f922d9f05bdb0c1c61333a0e74ae35c98e453ffe958e449b6bc3be6d675`
- After SHA-256: `bbbce2f1f4ef8345154669a4abcc33138c39b2af8700b47ad4e20234f675e11e`

## 98. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 not-ready->ready
- Before SHA-256: `53f25c746cb8bce78f0428c535336d6dc22b09f1d3cd98dff752e1fa5e7222d8`
- After SHA-256: `21593392d504e6a41aa24eedc044d347e1b98ff779a2e4ace0a663e20fcf76ca`

## 99. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 ready->in-progress
- Before SHA-256: `21593392d504e6a41aa24eedc044d347e1b98ff779a2e4ace0a663e20fcf76ca`
- After SHA-256: `200be9f68acd2b78799bff18c58b895445eaec4e0831dc2e2c82ed850f0684fd`

## 100. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 not-ready->ready
- Before SHA-256: `200be9f68acd2b78799bff18c58b895445eaec4e0831dc2e2c82ed850f0684fd`
- After SHA-256: `c25298ab3348841df3df49e1acb4b645138f30d19258765104e7f66aab21b289`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 ready->in-progress
- Before SHA-256: `c25298ab3348841df3df49e1acb4b645138f30d19258765104e7f66aab21b289`
- After SHA-256: `739536e55dcdd8a0cd09ef3f25ed7aff6ad926b48c33dadd79be7b89e20b5790`

## 102. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-A not-ready->ready
- Before SHA-256: `739536e55dcdd8a0cd09ef3f25ed7aff6ad926b48c33dadd79be7b89e20b5790`
- After SHA-256: `ec716ab0bd68c8e3e1c8def9b73cfb550001d14726d64434f36e5011fe68a524`

## 103. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-A ready->in-progress
- Before SHA-256: `ec716ab0bd68c8e3e1c8def9b73cfb550001d14726d64434f36e5011fe68a524`
- After SHA-256: `e89a2fc5ba90a0cc5aaaefaf72cb4427f13fa10f029c7104e12a90c2554396e5`

## 104. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-A in-progress->implemented->validated->done
- Before SHA-256: `e89a2fc5ba90a0cc5aaaefaf72cb4427f13fa10f029c7104e12a90c2554396e5`
- After SHA-256: `637df8c3d9b346a61e9ae41f8103ad2a0308c26c2378b2964de34caafd047fd3`

## 105. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-B not-ready->ready
- Before SHA-256: `637df8c3d9b346a61e9ae41f8103ad2a0308c26c2378b2964de34caafd047fd3`
- After SHA-256: `3c1654f57c36e8d5d77d4de34ae974bd018d7de775872cbf8e8fd9e4ad4a680c`

## 106. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-B ready->in-progress
- Before SHA-256: `3c1654f57c36e8d5d77d4de34ae974bd018d7de775872cbf8e8fd9e4ad4a680c`
- After SHA-256: `c34c05fe190d5a22a88c5a4548a747702859cf4bbaa681815c657745e1d6b323`

## 107. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-B in-progress->implemented->validated->done
- Before SHA-256: `c34c05fe190d5a22a88c5a4548a747702859cf4bbaa681815c657745e1d6b323`
- After SHA-256: `60a88354f66df1caca4ff2c0a36cf516f630dc7982fe5003429d5bac2b9e8ee2`

## 108. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-C not-ready->ready
- Before SHA-256: `60a88354f66df1caca4ff2c0a36cf516f630dc7982fe5003429d5bac2b9e8ee2`
- After SHA-256: `04f8fbb5285b777d48367a2653381cff3d6fd1f8a7ba7ef8b6df89fa6f6888da`

## 109. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-C ready->in-progress
- Before SHA-256: `04f8fbb5285b777d48367a2653381cff3d6fd1f8a7ba7ef8b6df89fa6f6888da`
- After SHA-256: `3af6bf58b1d77bb847f4e321867395ea3a7f9db913f57442571730679b22c6c2`

## 110. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-C in-progress->implemented->validated->done
- Before SHA-256: `3af6bf58b1d77bb847f4e321867395ea3a7f9db913f57442571730679b22c6c2`
- After SHA-256: `cee9328ec472f63b8c672506faa8cdd49d6f028bb6df201bf614c98ddf683474`

## 111. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 in-progress->implemented->validated->done
- Before SHA-256: `cee9328ec472f63b8c672506faa8cdd49d6f028bb6df201bf614c98ddf683474`
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=in-progress
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=in-progress
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03 state=not-ready
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 115. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S01 state=done
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 116. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02 state=in-progress
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 117. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1 state=done
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 118. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-A state=done
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 119. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-B state=done
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 120. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T1-C state=done
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 121. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-S02-T2 state=not-ready
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 122. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03-T1 state=not-ready
- After SHA-256: `db24ed6afb1fd2e50ebe4f52eb1ab68238ba488016463803c0b1ec6840cfd88d`

## 123. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `d75980ab349f057bdd169f4b06758cce2b72ad30e2fd93f6138595cd382766d3`
- After SHA-256: `4754e75ff66e1be25b1dcdbf96b2ce4102e5d57193983717b7c26e6b87949b83`

