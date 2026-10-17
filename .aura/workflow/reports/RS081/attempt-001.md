# RS081 Attempt 001 Payload Operations

- Operations: 104
- Changed: 36
- No change: 0
- Verified: 68
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS081 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS080/attempt-001.json` | run=RS080 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/gate |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/qualified_m5_phases |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/qualified_tasks |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/qualified_subtasks |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/original_v1_criteria_checked |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/isolated_nuget_consumer_probes |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/m5_milestone_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/packages_publicly_published |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/stable_release_certified |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-gate.json` | pointer=/owner_stable_publication_approved |
| 14 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/tasks/0/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/tasks/1/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/tasks/0/subtasks/0/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/tasks/0/subtasks/1/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/tasks/0/subtasks/2/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/41/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 29 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=not-ready |
| 31 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=not-ready |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=not-ready |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1 state=not-ready |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-R state=not-ready |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-G state=not-ready |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-V state=not-ready |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T2 state=not-ready |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 41 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | path is absent |
| 42 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/packages.lock.json` | path is absent |
| 43 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/CurvePoint64.cs` | path is absent |
| 44 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/M6W01C01Contract.cs` | path is absent |
| 45 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/packages.lock.json` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1RedTests.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1Tests.cs` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c01-t1/Program.cs` | path is absent |
| 51 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c01-t1/NuGet.Config` | path is absent |
| 52 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w01-c01-t1-curve-analysis.md` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/distribution/m6-curve-analysis.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C01-T1-contract-red.json` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json` | path is absent |
| 56 | `require.text-contains` | Verified | `dw.tools.math.slnx` | semantic text found |
| 57 | `files.replace-from-staged` | Changed | `dw.tools.math.slnx` | bytes=2061 |
| 58 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | bytes=582 |
| 59 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/packages.lock.json` | bytes=62 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/CurvePoint64.cs` | bytes=192 |
| 61 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj` | bytes=687 |
| 62 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/packages.lock.json` | bytes=3878 |
| 63 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1RedTests.cs` | bytes=766 |
| 64 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C01-T1-contract-red.json` | bytes=803 |
| 65 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/M6W01C01Contract.cs` | bytes=8795 |
| 66 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1Tests.cs` | bytes=11062 |
| 67 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj` | bytes=531 |
| 68 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c01-t1/Program.cs` | bytes=5237 |
| 69 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c01-t1/NuGet.Config` | bytes=113 |
| 70 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w01-c01-t1-curve-analysis.md` | bytes=2647 |
| 71 | `files.replace-from-staged` | Changed | `docs/distribution/m6-curve-analysis.md` | bytes=2044 |
| 72 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json` | bytes=5474 |
| 73 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6 not-ready->ready |
| 75 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01 not-ready->ready |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01 not-ready->ready |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1 not-ready->ready |
| 78 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-R not-ready->ready |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6 ready->in-progress |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01 ready->in-progress |
| 81 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01 ready->in-progress |
| 82 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1 ready->in-progress |
| 83 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-R ready->in-progress |
| 84 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-R in-progress->implemented->validated->done |
| 85 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-G not-ready->ready |
| 86 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-G ready->in-progress |
| 87 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-G in-progress->implemented->validated->done |
| 88 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-V not-ready->ready |
| 89 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-V ready->in-progress |
| 90 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-V in-progress->implemented->validated->done |
| 91 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1 in-progress->implemented->validated->done |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 95 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=in-progress |
| 96 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1 state=done |
| 97 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-R state=done |
| 98 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-G state=done |
| 99 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T1-V state=done |
| 100 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01-T2 state=not-ready |
| 101 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=not-ready |
| 102 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 103 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 104 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS081 hierarchy=exact
- After SHA-256: `77cad275751c5c57fdd6734961de109944297445421c58e08f749157dec95bc8`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS080/attempt-001.json`
- Summary: run=RS080 attempt=1 lineage=exact
- After SHA-256: `6cbe985fd5be6e396549905fcbee4236d895179d298d05ed4c3473e2b3893a23`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/gate

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/qualified_m5_phases

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/qualified_tasks

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/qualified_subtasks

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/original_v1_criteria_checked

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/isolated_nuget_consumer_probes

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/m5_milestone_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/packages_publicly_published

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/stable_release_certified

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-gate.json`
- Summary: pointer=/owner_stable_publication_approved

## 14. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/tasks/0/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/tasks/1/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/tasks/0/subtasks/0/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/tasks/0/subtasks/1/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/tasks/0/subtasks/2/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/41/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 29. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 31. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-R state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-G state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-V state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T2 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`

## 41. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: path is absent

## 42. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/packages.lock.json`
- Summary: path is absent

## 43. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/CurvePoint64.cs`
- Summary: path is absent

## 44. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/M6W01C01Contract.cs`
- Summary: path is absent

## 45. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/packages.lock.json`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1RedTests.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1Tests.cs`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c01-t1/Program.cs`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c01-t1/NuGet.Config`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w01-c01-t1-curve-analysis.md`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-curve-analysis.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T1-contract-red.json`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json`
- Summary: path is absent

## 56. `require.text-contains`

- Status: `Verified`
- Path: `dw.tools.math.slnx`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `dw.tools.math.slnx`
- Summary: bytes=2061
- Before SHA-256: `7315e9bdbaec3cda7f7a9de9f61b2f43c477d4ec6af7586bd6761b69a6296416`
- After SHA-256: `30b6641dae58723c6d4c7d1e17bc321e35c0afa3204cb652eaa68f7cec23f460`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: bytes=582
- After SHA-256: `90d9f91cda3d063e1d3e9932f38b34c733eb07fb6dbb508086519d9020389448`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/packages.lock.json`
- Summary: bytes=62
- After SHA-256: `1624731bf151ec2ac6993023046a4abbac7c6ddc71ed2e242e8285bbc69d561e`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/CurvePoint64.cs`
- Summary: bytes=192
- After SHA-256: `51d7b2f2240a5ecd0d0c6da6c1122eb0b789cade5cfb8ba0f18a4372b9ab41a4`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/dw.tools.math.analysis.tests.csproj`
- Summary: bytes=687
- After SHA-256: `7e3a35c44dfb31ea6fc86a9890a985176e497116afc209aca85f79e8ec7f26a6`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/packages.lock.json`
- Summary: bytes=3878
- After SHA-256: `50c3bc42260c2d38244fea52aa4bcbb9bf903ac62985158b9a9c44307c701fb0`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1RedTests.cs`
- Summary: bytes=766
- After SHA-256: `01e166a3bf556cf7a794ebdfa50afe79b0d0a9ed950eb02ba57fb7eec9ba460e`

## 64. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C01-T1-contract-red.json`
- Summary: bytes=803
- After SHA-256: `4d68dd646ef10d2bffc89de41cc8f11ee11cdb54af61001d56bd9861814c8fdc`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/M6W01C01Contract.cs`
- Summary: bytes=8795
- After SHA-256: `cf4e53737e8f674ecd033dc2a869d1415251e3b7fcd192a0cb4aebe1b3042b7c`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C01T1Tests.cs`
- Summary: bytes=11062
- After SHA-256: `49dcddf6b541423bee3240def09da781dba2ccdb212fdcf697376b7a33971397`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c01-t1/m6-curve-analysis-consumer.csproj`
- Summary: bytes=531
- After SHA-256: `c9b3a5935b3a0cda025d9eaea0287cd112939f6ca3686a4533ac53b5999a58e8`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c01-t1/Program.cs`
- Summary: bytes=5237
- After SHA-256: `fe893650893e003abe551d7cee0e9c42a00f476c007d4c368112f41a6eba5c4a`

## 69. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c01-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 70. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w01-c01-t1-curve-analysis.md`
- Summary: bytes=2647
- After SHA-256: `924f5b209d6a5de8b80790ba515aebdd8c202439f398d52b087c95a74cb20b1c`

## 71. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-curve-analysis.md`
- Summary: bytes=2044
- After SHA-256: `d5ed55b9b26b14338637e87a1a217edb9b25e2d608969e7d1a185860b4ae052b`

## 72. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C01-T1-contract-qualified.json`
- Summary: bytes=5474
- After SHA-256: `49c6d5e1c2d474b8a14986a6539ecd96988e9d5eadb9dd879f771d018a97553c`

## 73. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `4b95da400cbf45a8b320cbdcde34a17825e1dd8292325e45707729c7ab511f7b`
- After SHA-256: `0a10957c0db4a721abcf77f4f732a592a766e3d24476a0f0882505c570fdcd3e`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 not-ready->ready
- Before SHA-256: `2306e0e1882bb7e8a4ee3e46760c28bb6ecf805581426c634dfb9e1cbe0b5bfe`
- After SHA-256: `1cb23903ed9e0378a2175141ea3368c2679873d4cb3f713db38e799ec2720e3f`

## 75. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 not-ready->ready
- Before SHA-256: `1cb23903ed9e0378a2175141ea3368c2679873d4cb3f713db38e799ec2720e3f`
- After SHA-256: `a33e4b190348d10d9d687cfa6fab1c3e385ecc2e114e11677f53adddb51d85e4`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 not-ready->ready
- Before SHA-256: `a33e4b190348d10d9d687cfa6fab1c3e385ecc2e114e11677f53adddb51d85e4`
- After SHA-256: `a2a84e17c0d87a6bad850be9f284323af959601cdb38740bb852c07b71092937`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1 not-ready->ready
- Before SHA-256: `a2a84e17c0d87a6bad850be9f284323af959601cdb38740bb852c07b71092937`
- After SHA-256: `9e874b3fe216865751189e8becd55af2d2a7c24ab69ed10201891d7b77c6c8c7`

## 78. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-R not-ready->ready
- Before SHA-256: `9e874b3fe216865751189e8becd55af2d2a7c24ab69ed10201891d7b77c6c8c7`
- After SHA-256: `0b3fa2bde608dabec7c26b17441a3f421e960fa448ace7bdf835ca6ef702f6b0`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 ready->in-progress
- Before SHA-256: `0b3fa2bde608dabec7c26b17441a3f421e960fa448ace7bdf835ca6ef702f6b0`
- After SHA-256: `d0a92e68c9b009f618f337fcc78c820150da6a2b10e80b09c8b99f56da82cf57`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 ready->in-progress
- Before SHA-256: `d0a92e68c9b009f618f337fcc78c820150da6a2b10e80b09c8b99f56da82cf57`
- After SHA-256: `9cc162b84c388ad3709a2e6384925b71f1a242d6410a1ece8fa2e5ddd0af07f0`

## 81. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 ready->in-progress
- Before SHA-256: `9cc162b84c388ad3709a2e6384925b71f1a242d6410a1ece8fa2e5ddd0af07f0`
- After SHA-256: `a1f16ab72f18bd92fac8d6f9fffb0c66f97df3df41b06a8c17973c64c5d6e0db`

## 82. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1 ready->in-progress
- Before SHA-256: `a1f16ab72f18bd92fac8d6f9fffb0c66f97df3df41b06a8c17973c64c5d6e0db`
- After SHA-256: `462a3a3124e5ee9d5eafb753e19ef440de616de7e8f4af363ab48629193092a6`

## 83. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-R ready->in-progress
- Before SHA-256: `462a3a3124e5ee9d5eafb753e19ef440de616de7e8f4af363ab48629193092a6`
- After SHA-256: `2833d96e3d287272e4ac9a889fbb219908f109a5e5c76f03c2e692107f340b66`

## 84. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-R in-progress->implemented->validated->done
- Before SHA-256: `2833d96e3d287272e4ac9a889fbb219908f109a5e5c76f03c2e692107f340b66`
- After SHA-256: `9a9f6ffbd051882e8fec2c7c09e3c8099d3d28d8fea79dbe8f8b3187e6dd0d4b`

## 85. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-G not-ready->ready
- Before SHA-256: `9a9f6ffbd051882e8fec2c7c09e3c8099d3d28d8fea79dbe8f8b3187e6dd0d4b`
- After SHA-256: `9e69d780a0f399475d6dfa45ac3099123e2da45cb99b0b6859240fe1af85ebe6`

## 86. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-G ready->in-progress
- Before SHA-256: `9e69d780a0f399475d6dfa45ac3099123e2da45cb99b0b6859240fe1af85ebe6`
- After SHA-256: `41b16bc5398c7d3c3a37df3d94de9d92aee8e296a6390222476d9a9e96cb88d2`

## 87. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-G in-progress->implemented->validated->done
- Before SHA-256: `41b16bc5398c7d3c3a37df3d94de9d92aee8e296a6390222476d9a9e96cb88d2`
- After SHA-256: `5a27a7890da319f66a9f943eb203e18c1164797747ede46cf3e398973883e3dc`

## 88. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-V not-ready->ready
- Before SHA-256: `5a27a7890da319f66a9f943eb203e18c1164797747ede46cf3e398973883e3dc`
- After SHA-256: `e15315d494ed5d156a73574a8f62f931a69585ea1c694fbe43bc1224c6ba0fea`

## 89. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-V ready->in-progress
- Before SHA-256: `e15315d494ed5d156a73574a8f62f931a69585ea1c694fbe43bc1224c6ba0fea`
- After SHA-256: `c8f834633466e22511bc700fdbc0c5e11abbf369e36fb98fbcd693612343edc4`

## 90. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-V in-progress->implemented->validated->done
- Before SHA-256: `c8f834633466e22511bc700fdbc0c5e11abbf369e36fb98fbcd693612343edc4`
- After SHA-256: `4024a2f1417e3d387bc06c6cb341c8413cd302cd427a1132cce2c8cbbbe4d603`

## 91. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1 in-progress->implemented->validated->done
- Before SHA-256: `4024a2f1417e3d387bc06c6cb341c8413cd302cd427a1132cce2c8cbbbe4d603`
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 95. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=in-progress
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 96. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1 state=done
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 97. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-R state=done
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 98. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-G state=done
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 99. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T1-V state=done
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 100. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01-T2 state=not-ready
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 101. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=not-ready
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 102. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 103. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `8c63fecde51be413c12ec3fdc91862a7956944ea05da80092a1820d177064f11`

## 104. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `7401de6876d25d3c3db1435c96708cd7ae693bd88b082df66cbb994d2fc4e157`
- After SHA-256: `13f1cd20d26f300c29782dd37476252fdb7221ddb9097d33dfd55de77321a734`

