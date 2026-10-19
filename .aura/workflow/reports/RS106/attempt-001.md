# RS106 Attempt 001 Payload Operations

- Operations: 107
- Changed: 24
- No change: 0
- Verified: 83
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS106 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS105/attempt-001.json` | run=RS105 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/m6_milestone_done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/qualified_phases |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/packages_publicly_published |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/stable_release_certified |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-gate.json` | pointer=/external_cutover_performed |
| 9 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 10 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 11 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/id |
| 13 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/id |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/version |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/gate |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/id |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/15/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/16/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/id |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/kind |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/0/id |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/0/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/1/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/52/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/53/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/0/subtasks/0/status |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/1/subtasks/0/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/0/subtasks/1/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/1/subtasks/1/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/0/subtasks/2/status |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/51/tasks/1/subtasks/2/status |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1 state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-A state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-B state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-C state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T2 state=not-ready |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T2-A state=not-ready |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T2-B state=not-ready |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T2-C state=not-ready |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C02 state=not-ready |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C03 state=not-ready |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W02 state=not-ready |
| 51 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 52 | `require.file-exists` | Verified | `src/projects/dw.quantities/ExactRational.cs` | file exists |
| 53 | `require.file-exists` | Verified | `src/projects/dw.quantities/ExactBinaryNumber.cs` | file exists |
| 54 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c01.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/ValidateM7PrecisionAdmission.cs` | path is absent |
| 57 | `require.file-missing` | Verified | `docs/planning/evidence/M7-W01-C01-T1-qualified.json` | path is absent |
| 58 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c01.md` | bytes=4881 |
| 59 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | bytes=4774 |
| 60 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM7PrecisionAdmission.cs` | bytes=7858 |
| 61 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 62 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 63 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 64 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 65 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 66 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 67 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 68 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01.md` | semantic text found |
| 69 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 70 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 71 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 72 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 73 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 74 | `require.text-contains` | Verified | `docs/planning/decisions/m7-w01-c01-provider-screen.md` | semantic text found |
| 75 | `require.text-contains` | Verified | `docs/planning/ValidateM7PrecisionAdmission.cs` | semantic text found |
| 76 | `require.text-contains` | Verified | `docs/planning/ValidateM7PrecisionAdmission.cs` | semantic text found |
| 77 | `files.write-complete` | Changed | `docs/planning/evidence/M7-W01-C01-T1-qualified.json` | bytes=2916 |
| 78 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7 not-ready->ready |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7 ready->in-progress |
| 81 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01 not-ready->ready |
| 82 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01 ready->in-progress |
| 83 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01 not-ready->ready |
| 84 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01 ready->in-progress |
| 85 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1 not-ready->ready |
| 86 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1 ready->in-progress |
| 87 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-A not-ready->ready |
| 88 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-A ready->in-progress |
| 89 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-A in-progress->implemented->validated->done |
| 90 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-B not-ready->ready |
| 91 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-B ready->in-progress |
| 92 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-B in-progress->implemented->validated->done |
| 93 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-C not-ready->ready |
| 94 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-C ready->in-progress |
| 95 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-C in-progress->implemented->validated->done |
| 96 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1 in-progress->implemented->validated->done |
| 97 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=in-progress |
| 98 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01 state=in-progress |
| 99 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01 state=in-progress |
| 100 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1 state=done |
| 101 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-A state=done |
| 102 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-B state=done |
| 103 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T1-C state=done |
| 104 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C01-T2 state=not-ready |
| 105 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7-W01-C02 state=not-ready |
| 106 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 107 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS106 hierarchy=exact
- After SHA-256: `00137165959692153332ad3fb0b4f49d9ae0c418ece949be0cf4209f755b3ed3`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS105/attempt-001.json`
- Summary: run=RS105 attempt=1 lineage=exact
- After SHA-256: `cf4e2831b09f822d79012bc1c274e1069ab8a7131da5a85731c6e705caa1861a`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/m6_milestone_done

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/qualified_phases

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/packages_publicly_published

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/stable_release_certified

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: pointer=/external_cutover_performed

## 9. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/id

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/id

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/version

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/gate

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/id

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/15/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/16/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/id

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/kind

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/0/id

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/0/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/1/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/52/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/53/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/0/subtasks/0/status

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/1/subtasks/0/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/0/subtasks/1/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/1/subtasks/1/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/0/subtasks/2/status

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/51/tasks/1/subtasks/2/status

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-A state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-B state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-C state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T2 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T2-A state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T2-B state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T2-C state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C02 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C03 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W02 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 51. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 52. `require.file-exists`

- Status: `Verified`
- Path: `src/projects/dw.quantities/ExactRational.cs`
- Summary: file exists

## 53. `require.file-exists`

- Status: `Verified`
- Path: `src/projects/dw.quantities/ExactBinaryNumber.cs`
- Summary: file exists

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PrecisionAdmission.cs`
- Summary: path is absent

## 57. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M7-W01-C01-T1-qualified.json`
- Summary: path is absent

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: bytes=4881
- After SHA-256: `7684422c0a0c48a6f6fffaef83434f72fc73aa0c0b8f5cafb2356c255d2ea22e`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: bytes=4774
- After SHA-256: `185280f6153b49d3f9f6d93001d86f82be9f62c7c6cc9a597dea90449c7c9df6`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM7PrecisionAdmission.cs`
- Summary: bytes=7858
- After SHA-256: `9db0534107983ad565eec0efe6ff27a7e2a031c8e4702648f0ab20b332a4c456`

## 61. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 62. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 63. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 64. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 65. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 66. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 67. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 68. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 69. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 70. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 71. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 72. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 73. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 74. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m7-w01-c01-provider-screen.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 75. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PrecisionAdmission.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 76. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM7PrecisionAdmission.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 77. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M7-W01-C01-T1-qualified.json`
- Summary: bytes=2916
- After SHA-256: `1add8ddc7172e4ca4b92e434731d1af50aef0e4a24d2ac86daef5d14187671c2`

## 78. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `33d12b20c843306d19bf49e87ba324a4a8655963f60466732539c07bea1dea41`
- After SHA-256: `0c0799166ab2c27641ccd8df84eeb05e8d61ccd62acd7e4a4e24139bb2d845df`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 not-ready->ready
- Before SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`
- After SHA-256: `ead38df2f41a16e4dd7b94a373a65e90ee998923da3d76ab307dc6522f0a1f28`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 ready->in-progress
- Before SHA-256: `ead38df2f41a16e4dd7b94a373a65e90ee998923da3d76ab307dc6522f0a1f28`
- After SHA-256: `b7a07d60eaeb74145ef06173d1ff33bdf237a9ac67d4462d5ed990177f0a6712`

## 81. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 not-ready->ready
- Before SHA-256: `b7a07d60eaeb74145ef06173d1ff33bdf237a9ac67d4462d5ed990177f0a6712`
- After SHA-256: `ecbb2e720cebe436b05939a7383928491953cde8a9c9c14545c1a5aa7457dd86`

## 82. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 ready->in-progress
- Before SHA-256: `ecbb2e720cebe436b05939a7383928491953cde8a9c9c14545c1a5aa7457dd86`
- After SHA-256: `a76bd00dc29e54a362a975ee2eb70cf595a118a620f343e1155fd79a0b359ddc`

## 83. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 not-ready->ready
- Before SHA-256: `a76bd00dc29e54a362a975ee2eb70cf595a118a620f343e1155fd79a0b359ddc`
- After SHA-256: `a7106550dad796fd9311635b0f1bc7abeed19da11959d4188271fe855dac3b64`

## 84. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 ready->in-progress
- Before SHA-256: `a7106550dad796fd9311635b0f1bc7abeed19da11959d4188271fe855dac3b64`
- After SHA-256: `3f007e81f31ac734662744b5e8a28f4f41a6ef32cf7a7e1dfab1c4b2ea514bbb`

## 85. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1 not-ready->ready
- Before SHA-256: `3f007e81f31ac734662744b5e8a28f4f41a6ef32cf7a7e1dfab1c4b2ea514bbb`
- After SHA-256: `5573dcd9b34e15fe2938750b5326c15c0957ca722be34e267303f72742c3d66f`

## 86. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1 ready->in-progress
- Before SHA-256: `5573dcd9b34e15fe2938750b5326c15c0957ca722be34e267303f72742c3d66f`
- After SHA-256: `988027f59d563f5763c14528455dd53fbf2422dffb3b74c6b3d30c0d4d9cbb77`

## 87. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-A not-ready->ready
- Before SHA-256: `988027f59d563f5763c14528455dd53fbf2422dffb3b74c6b3d30c0d4d9cbb77`
- After SHA-256: `1d23d34a51a35fa5d4a70e680cfdd2920c05b33dbacf4dfee698a111beb8eb0d`

## 88. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-A ready->in-progress
- Before SHA-256: `1d23d34a51a35fa5d4a70e680cfdd2920c05b33dbacf4dfee698a111beb8eb0d`
- After SHA-256: `d59f1af5534cbf66cf96e660e99a24b4f28cfdf6f6d9596b7af9a33ab15f8833`

## 89. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-A in-progress->implemented->validated->done
- Before SHA-256: `d59f1af5534cbf66cf96e660e99a24b4f28cfdf6f6d9596b7af9a33ab15f8833`
- After SHA-256: `1182e2563b21816191718eb5b236a7e05c682180b2a9b2d0f4e72c8296a26ad6`

## 90. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-B not-ready->ready
- Before SHA-256: `1182e2563b21816191718eb5b236a7e05c682180b2a9b2d0f4e72c8296a26ad6`
- After SHA-256: `f0d26b67b7b5df4c6fce2aca2ce8d6d29e7ac0e97f490f5f202430970b58f0fd`

## 91. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-B ready->in-progress
- Before SHA-256: `f0d26b67b7b5df4c6fce2aca2ce8d6d29e7ac0e97f490f5f202430970b58f0fd`
- After SHA-256: `c98fe30e1b96c226843f800825bc141583761a41e3e4c6a2ea949cb01e4ae895`

## 92. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-B in-progress->implemented->validated->done
- Before SHA-256: `c98fe30e1b96c226843f800825bc141583761a41e3e4c6a2ea949cb01e4ae895`
- After SHA-256: `9ca4f68867d87ec9c92ce5192aaddbd4eafd6eedfcda922e25cdc44b6a2d28ee`

## 93. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-C not-ready->ready
- Before SHA-256: `9ca4f68867d87ec9c92ce5192aaddbd4eafd6eedfcda922e25cdc44b6a2d28ee`
- After SHA-256: `85cf935a4d809afa27be93a8b9cb3140063f5a5bab5c8190ca442fe1d92b6fb9`

## 94. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-C ready->in-progress
- Before SHA-256: `85cf935a4d809afa27be93a8b9cb3140063f5a5bab5c8190ca442fe1d92b6fb9`
- After SHA-256: `88c97c6816b868e9caf601962a0ab09e1de38ea3eea7ff25eb42bf6dea61fb1f`

## 95. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-C in-progress->implemented->validated->done
- Before SHA-256: `88c97c6816b868e9caf601962a0ab09e1de38ea3eea7ff25eb42bf6dea61fb1f`
- After SHA-256: `1f26238aa63d31f5b831369332d9938b91602d3fd7884331c4269aa5d2b1ee81`

## 96. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1 in-progress->implemented->validated->done
- Before SHA-256: `1f26238aa63d31f5b831369332d9938b91602d3fd7884331c4269aa5d2b1ee81`
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 97. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=in-progress
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 98. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01 state=in-progress
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 99. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01 state=in-progress
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 100. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1 state=done
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 101. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-A state=done
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 102. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-B state=done
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 103. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T1-C state=done
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 104. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C01-T2 state=not-ready
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 105. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7-W01-C02 state=not-ready
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 106. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `f538f08be317c4a1a61f8b7cef258a9c096b204978059838370eea13a0ea1963`

## 107. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `88a61fc7cd288d051f64d3eea6b65373fa82580f656b3effdc32a69038d04521`
- After SHA-256: `75dff929000a6fd7f005009e65fb0ffe7cd3614e7629c6c6510a53e2a60ac12a`

