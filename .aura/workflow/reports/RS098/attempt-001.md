# RS098 Attempt 001 Payload Operations

- Operations: 115
- Changed: 33
- No change: 0
- Verified: 82
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS098 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS097/attempt-001.json` | run=RS097 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/local_product_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/independent_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/m6_w02_c03_s03_t1_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/m6_w02_c03_s03_phase_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/m6_w02_c03_parent_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | pointer=/publicly_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/native_node_count |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/product_qualified |
| 13 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/id |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/id |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/id |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/id |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/subtasks/0/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/subtasks/1/status |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/subtasks/2/status |
| 36 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 37 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj` | semantic text found |
| 38 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | semantic text found |
| 39 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | semantic text found |
| 40 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | semantic text found |
| 41 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s03-t1/Program.cs` | semantic text found |
| 42 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | semantic text found |
| 43 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | semantic text found |
| 44 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | semantic text found |
| 45 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 52 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=done |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=in-progress |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 state=done |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 state=not-ready |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-R state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-G state=not-ready |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-V state=not-ready |
| 61 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 62 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 63 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs` | path is absent |
| 64 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2RedTests.cs` | path is absent |
| 65 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2Tests.cs` | path is absent |
| 66 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj` | path is absent |
| 67 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t2/Program.cs` | path is absent |
| 68 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t2/NuGet.Config` | path is absent |
| 69 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c03-s03-t2-uniform-grid-audit.md` | path is absent |
| 70 | `require.file-missing` | Verified | `docs/distribution/m6-uniform-grid-audit.md` | path is absent |
| 71 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T2-contract-red.json` | path is absent |
| 72 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json` | path is absent |
| 73 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2RedTests.cs` | bytes=746 |
| 74 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S03-T2-contract-red.json` | bytes=652 |
| 75 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=655 |
| 76 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs` | bytes=9924 |
| 77 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2Tests.cs` | bytes=23465 |
| 78 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj` | bytes=534 |
| 79 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t1/Program.cs` | bytes=6647 |
| 80 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | bytes=543 |
| 81 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | bytes=7664 |
| 82 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | bytes=545 |
| 83 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | bytes=6257 |
| 84 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=546 |
| 85 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6096 |
| 86 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj` | bytes=544 |
| 87 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t2/Program.cs` | bytes=7221 |
| 88 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t2/NuGet.Config` | bytes=113 |
| 89 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c03-s03-t2-uniform-grid-audit.md` | bytes=4776 |
| 90 | `files.replace-from-staged` | Changed | `docs/distribution/m6-uniform-grid-audit.md` | bytes=1884 |
| 91 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json` | bytes=9114 |
| 92 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 93 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 not-ready->ready |
| 94 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-R not-ready->ready |
| 95 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M6-W02-C03-S03-T2-R activated=M6-W02-C03-S03-T2,M6-W02-C03-S03-T2-R |
| 96 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-R in-progress->implemented->validated->done |
| 97 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-G not-ready->ready |
| 98 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-G ready->in-progress |
| 99 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-G in-progress->implemented->validated->done |
| 100 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-V not-ready->ready |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-V ready->in-progress |
| 102 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-V in-progress->implemented->validated->done |
| 103 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 in-progress->implemented->validated->done |
| 104 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 in-progress->implemented->validated->done |
| 105 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=done |
| 106 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 state=done |
| 107 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-R state=done |
| 108 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-G state=done |
| 109 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2-V state=done |
| 110 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 111 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 115 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS098 hierarchy=exact
- After SHA-256: `29f21dba327f9f650b6fc12c0202224a0dc1e3b365725b22dcc1e3060b8e1ddd`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS097/attempt-001.json`
- Summary: run=RS097 attempt=1 lineage=exact
- After SHA-256: `2dab96d1846aa32634fd335b0fd3caa56be895c3e232b23e07c12c4035aad6c7`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/local_product_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_s03_t1_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_s03_phase_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_parent_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: pointer=/publicly_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/native_node_count

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/product_qualified

## 13. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/id

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/id

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/id

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/id

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/id

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/subtasks/0/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/subtasks/1/status

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/subtasks/2/status

## 36. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t1/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 44. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 45. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 52. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-R state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-G state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-V state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 61. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 62. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 63. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs`
- Summary: path is absent

## 64. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2RedTests.cs`
- Summary: path is absent

## 65. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2Tests.cs`
- Summary: path is absent

## 66. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj`
- Summary: path is absent

## 67. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t2/Program.cs`
- Summary: path is absent

## 68. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t2/NuGet.Config`
- Summary: path is absent

## 69. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c03-s03-t2-uniform-grid-audit.md`
- Summary: path is absent

## 70. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-uniform-grid-audit.md`
- Summary: path is absent

## 71. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-contract-red.json`
- Summary: path is absent

## 72. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json`
- Summary: path is absent

## 73. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2RedTests.cs`
- Summary: bytes=746
- After SHA-256: `f4b2ed640b02ad58f74bcbcdd8a1849538568e431a1f8e9888e7c01a1a332332`

## 74. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-contract-red.json`
- Summary: bytes=652
- After SHA-256: `c7a4b6ddef3c045f1d7924a3e1d5db8b2081dba36020543f0d265a56cad242b4`

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=655
- Before SHA-256: `1b41df466619d04cb800d4b8df9bf555992a643618b4b98a00e97773aa9e86ab`
- After SHA-256: `11c611f31cc8c594ace587973e2297eecd3de68f14d43bbae8461735e3204c80`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/FiniteUniformSeriesBoundaryAudit64.cs`
- Summary: bytes=9924
- After SHA-256: `908f18668b23ea788e5f1be531bf8a3bb6108c048a1e663f6c1927bdd9ab5d10`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T2Tests.cs`
- Summary: bytes=23465
- After SHA-256: `c4bdc96b2df80472dee7efbca121a8b3b0cdd7266db60adc43509afe5c798850`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj`
- Summary: bytes=534
- Before SHA-256: `686c2f4da250b04176b9d33999d482e4fe99470dd683c84db43cc16915eeb8cd`
- After SHA-256: `0c113a104263d1e8d8be90524376984474115484e8bfa3b253d58b3e85b1d7f4`

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t1/Program.cs`
- Summary: bytes=6647
- Before SHA-256: `50457c89a74e4d5f5db5aec5a1a12013cb9d4b90991f2349b67c60d6675b0dbd`
- After SHA-256: `dd4171efec5f604f16011000a4f2478313092155821f859043a38c8c8e1799fa`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: bytes=543
- Before SHA-256: `4d6ba4f1cc2b9d4ba605a0d8ad730677940ddb0ed81e8b052af11010a404e301`
- After SHA-256: `a2e2d8187b543d3a8f5c32d74dcd1ba51b97e111fa5d176635b31ccedcbc9636`

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: bytes=7664
- Before SHA-256: `ba06100f7844e687a89a89f541621e4832a87553f014a92b8ea30e76aa6d9258`
- After SHA-256: `8b1f1f4898553c92709792e8964fe72b2b1605be4ee68306e73d02c2b5597487`

## 82. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: bytes=545
- Before SHA-256: `0e3390b6553688fd3a9df70442c17d30c43c14545e0519169e6921e745daa96b`
- After SHA-256: `42aaaa3a89c0f797da5d57bf8bcab1caa98a8d4b4eef1b6e03c756cbe17372b1`

## 83. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: bytes=6257
- Before SHA-256: `7f139d2aa94b9a4480676303eb5244122e0c340c1793d1d9b68ed71da4f4dbc5`
- After SHA-256: `3bc613c2b3e91b43d902eb22899c5f30832800d5750c13f6bd79142808f97d68`

## 84. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=546
- Before SHA-256: `64b7f91fdb11825bf917e61873fd054f08a6d95907d40b362b2c757bcfb73b06`
- After SHA-256: `ec1989711acc720af1d6070e083cf483b1982aa1eb5ffbc0dcaa5b86cb7e8bb7`

## 85. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6096
- Before SHA-256: `66dbf04926afe027406597d65bda1bdd9fddad2233e18eb4fdb88a2704178d0a`
- After SHA-256: `02a7ebbb51cef4df2cc6c12104898e25bf5a8bafcf1492e1f3e2295c92ed80b6`

## 86. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj`
- Summary: bytes=544
- After SHA-256: `29e5242b8ce6c3c28fa40fecf6208e9b47d7aa2b24471a3fa04782b9d18ef8b5`

## 87. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t2/Program.cs`
- Summary: bytes=7221
- After SHA-256: `43eeb1325f2df859b5d1e636b72278b5cd1a39ff2a89c10a2c75a4e02234668a`

## 88. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t2/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 89. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c03-s03-t2-uniform-grid-audit.md`
- Summary: bytes=4776
- After SHA-256: `764de83b52b35a24e283d4db9fde94ff8294ce60f893f6c508d18d4373bbaaca`

## 90. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-uniform-grid-audit.md`
- Summary: bytes=1884
- After SHA-256: `6443ed8bfa9ab800fe61f65da9bbce4577d351e6be499a9be04fc66c78ab15f1`

## 91. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json`
- Summary: bytes=9114
- After SHA-256: `64a3cbec67c37e4a0a7a2b7e981a536f8e2f19e3daf0e23f1d2bfedc2525f7e9`

## 92. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `c8f18a031e22c6747cffeeaad0646ea893dd36da813be1ce6939e6360389e07d`
- After SHA-256: `5956eefaa311f4afc11000c07727c8d307b980aeff044c6dd093b3669b332347`

## 93. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 not-ready->ready
- Before SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`
- After SHA-256: `70a305b2c528d2ed39e4e8ae85930087f3f788c69ad0d9215e211735541d9189`

## 94. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-R not-ready->ready
- Before SHA-256: `70a305b2c528d2ed39e4e8ae85930087f3f788c69ad0d9215e211735541d9189`
- After SHA-256: `97dc197b5b1313cc6276def8afd9c9aa393625509be7b8451aa475154768f168`

## 95. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M6-W02-C03-S03-T2-R activated=M6-W02-C03-S03-T2,M6-W02-C03-S03-T2-R
- Before SHA-256: `97dc197b5b1313cc6276def8afd9c9aa393625509be7b8451aa475154768f168`
- After SHA-256: `2d9372836a48c126f5f8eb6e61ba9e287a47ecb059479149fbc5462ccb3f31ad`

## 96. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-R in-progress->implemented->validated->done
- Before SHA-256: `2d9372836a48c126f5f8eb6e61ba9e287a47ecb059479149fbc5462ccb3f31ad`
- After SHA-256: `68da2dd1321a006dff92837e34d4bbe76cab95e106bf33a3dad00c29897596ce`

## 97. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-G not-ready->ready
- Before SHA-256: `68da2dd1321a006dff92837e34d4bbe76cab95e106bf33a3dad00c29897596ce`
- After SHA-256: `ab41b1b93e6029922aa9f1ff43dcd2632527e179126bbae562094bfffed97fe8`

## 98. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-G ready->in-progress
- Before SHA-256: `ab41b1b93e6029922aa9f1ff43dcd2632527e179126bbae562094bfffed97fe8`
- After SHA-256: `eef4319ec9ba0c34574e42d81a573272d9f716449841e9397b31cfd18108b04b`

## 99. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-G in-progress->implemented->validated->done
- Before SHA-256: `eef4319ec9ba0c34574e42d81a573272d9f716449841e9397b31cfd18108b04b`
- After SHA-256: `2069d24a276f8e49e115eff01e8172197db351266d453c3dbeb6e339001a9545`

## 100. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-V not-ready->ready
- Before SHA-256: `2069d24a276f8e49e115eff01e8172197db351266d453c3dbeb6e339001a9545`
- After SHA-256: `be17eab322a00167b8170b0780c5f6df3567fe6134cc718f09e1e289009ad352`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-V ready->in-progress
- Before SHA-256: `be17eab322a00167b8170b0780c5f6df3567fe6134cc718f09e1e289009ad352`
- After SHA-256: `59d2a5c0db801153b98f6a35e4e884fd98eeb0a044d934e4aea76d4488d00e0e`

## 102. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-V in-progress->implemented->validated->done
- Before SHA-256: `59d2a5c0db801153b98f6a35e4e884fd98eeb0a044d934e4aea76d4488d00e0e`
- After SHA-256: `5485e1f6a1cbdf0ef957dff0e5e842444edfc2bf7e01562dbc502af9ab7314b9`

## 103. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 in-progress->implemented->validated->done
- Before SHA-256: `5485e1f6a1cbdf0ef957dff0e5e842444edfc2bf7e01562dbc502af9ab7314b9`
- After SHA-256: `4085baada4d8c38502f2c57cf5bb18cae6e7a8cd133880752307eaf135bfdc53`

## 104. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 in-progress->implemented->validated->done
- Before SHA-256: `4085baada4d8c38502f2c57cf5bb18cae6e7a8cd133880752307eaf135bfdc53`
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 105. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=done
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 106. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 state=done
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 107. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-R state=done
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 108. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-G state=done
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 109. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2-V state=done
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 110. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 111. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `e13cdad9943d667a2d4f00f5bcabd30cd8ff52c7cbd6b37a5d9a239c3f78bc04`

## 115. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `6f106c714a1e1effc4359a76d91c28a9d8f594d274e0c7f9baa793dffb675371`
- After SHA-256: `ce36062ac964d71e40bec24a12219a89978225d0a06bb7029647b22c2376cd79`

