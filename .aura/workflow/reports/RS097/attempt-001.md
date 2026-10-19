# RS097 Attempt 001 Payload Operations

- Operations: 115
- Changed: 33
- No change: 0
- Verified: 82
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS097 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS096/attempt-001.json` | run=RS096 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_s02_t2_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_s02_phase_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_parent_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/native_node_count |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/product_qualified |
| 14 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/id |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/id |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/id |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/id |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/1/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/subtasks/0/status |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/subtasks/1/status |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/tasks/0/subtasks/2/status |
| 37 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 38 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | semantic text found |
| 39 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | semantic text found |
| 40 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | semantic text found |
| 41 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | semantic text found |
| 42 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | semantic text found |
| 43 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | semantic text found |
| 44 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 52 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=not-ready |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 state=not-ready |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-R state=not-ready |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-G state=not-ready |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-V state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 state=not-ready |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 61 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 62 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs` | path is absent |
| 63 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1RedTests.cs` | path is absent |
| 64 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1Tests.cs` | path is absent |
| 65 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj` | path is absent |
| 66 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t1/Program.cs` | path is absent |
| 67 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s03-t1/NuGet.Config` | path is absent |
| 68 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c03-s03-t1-uniform-series.md` | path is absent |
| 69 | `require.file-missing` | Verified | `docs/distribution/m6-uniform-series.md` | path is absent |
| 70 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-contract-red.json` | path is absent |
| 71 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | path is absent |
| 72 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1RedTests.cs` | bytes=719 |
| 73 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S03-T1-contract-red.json` | bytes=652 |
| 74 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=674 |
| 75 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs` | bytes=6543 |
| 76 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1Tests.cs` | bytes=11556 |
| 77 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | bytes=542 |
| 78 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | bytes=7663 |
| 79 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | bytes=544 |
| 80 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | bytes=6256 |
| 81 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=545 |
| 82 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6095 |
| 83 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj` | bytes=533 |
| 84 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t1/Program.cs` | bytes=6646 |
| 85 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t1/NuGet.Config` | bytes=113 |
| 86 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c03-s03-t1-uniform-series.md` | bytes=4209 |
| 87 | `files.replace-from-staged` | Changed | `docs/distribution/m6-uniform-series.md` | bytes=1800 |
| 88 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json` | bytes=7991 |
| 89 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 90 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 not-ready->ready |
| 91 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 not-ready->ready |
| 92 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-R not-ready->ready |
| 93 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 ready->in-progress |
| 94 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 ready->in-progress |
| 95 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-R ready->in-progress |
| 96 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-R in-progress->implemented->validated->done |
| 97 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-G not-ready->ready |
| 98 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-G ready->in-progress |
| 99 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-G in-progress->implemented->validated->done |
| 100 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-V not-ready->ready |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-V ready->in-progress |
| 102 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-V in-progress->implemented->validated->done |
| 103 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 in-progress->implemented->validated->done |
| 104 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=in-progress |
| 105 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1 state=done |
| 106 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-R state=done |
| 107 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-G state=done |
| 108 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T1-V state=done |
| 109 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03-T2 state=not-ready |
| 110 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 111 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 115 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS097 hierarchy=exact
- After SHA-256: `370effcbf9754496997d88428dbbc0f278e30b13a40c0df1c88d2f062ab7d7e1`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS096/attempt-001.json`
- Summary: run=RS096 attempt=1 lineage=exact
- After SHA-256: `79c9349bae3899303b5285011626ec9f47dfe275b8cf918c6ea91a9f5eb00da1`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_s02_t2_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_s02_phase_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_parent_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/publicly_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/native_node_count

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/product_qualified

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
- Summary: pointer=/work_packages/13/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/id

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/id

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/id

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/id

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/id

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/1/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/subtasks/0/status

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/subtasks/1/status

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/tasks/0/subtasks/2/status

## 37. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
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
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 44. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 52. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-R state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-G state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-V state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 61. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 62. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs`
- Summary: path is absent

## 63. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1RedTests.cs`
- Summary: path is absent

## 64. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1Tests.cs`
- Summary: path is absent

## 65. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj`
- Summary: path is absent

## 66. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t1/Program.cs`
- Summary: path is absent

## 67. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t1/NuGet.Config`
- Summary: path is absent

## 68. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c03-s03-t1-uniform-series.md`
- Summary: path is absent

## 69. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-uniform-series.md`
- Summary: path is absent

## 70. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-contract-red.json`
- Summary: path is absent

## 71. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: path is absent

## 72. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1RedTests.cs`
- Summary: bytes=719
- After SHA-256: `d652c5b47aab1ed874cfc5b325084a693f39d5e66538c332ee18b565b92eff27`

## 73. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-contract-red.json`
- Summary: bytes=652
- After SHA-256: `9b1295ff6a6610a6707311fc47e224b4493532bd5c70a18cd6b5e7a74564529b`

## 74. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=674
- Before SHA-256: `1287b65e4675b58f076f15f77dcc5cf2a6c19fc7380bc39998366fb96eb6e53a`
- After SHA-256: `1b41df466619d04cb800d4b8df9bf555992a643618b4b98a00e97773aa9e86ab`

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/FiniteUniformSeries64.cs`
- Summary: bytes=6543
- After SHA-256: `cde666251758da2e808a035a83a1f9d6b3d4ba072006e511895b71b63ea4f508`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S03T1Tests.cs`
- Summary: bytes=11556
- After SHA-256: `8487d49ae20486a078ddd992e9e31e4a23e1f0c3caee0e865b501be7c47138da`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: bytes=542
- Before SHA-256: `8dd89bc2b35313e2d9808b45ce78918c27993b8060d1f90a025f481a550ed3c4`
- After SHA-256: `4d6ba4f1cc2b9d4ba605a0d8ad730677940ddb0ed81e8b052af11010a404e301`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: bytes=7663
- Before SHA-256: `652c3d285b7f59fd6d04e6b2a08e52656d4c0e61a4d4674258bde60c08200fdf`
- After SHA-256: `ba06100f7844e687a89a89f541621e4832a87553f014a92b8ea30e76aa6d9258`

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: bytes=544
- Before SHA-256: `9f454443125dad3059269a5c002e2fcd6d8e6583dd5542cda362b13a228fa2ce`
- After SHA-256: `0e3390b6553688fd3a9df70442c17d30c43c14545e0519169e6921e745daa96b`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: bytes=6256
- Before SHA-256: `188ac9474712b7a442ed980a2e62118714a16cd8a12ec2a6eb9c9ef881466518`
- After SHA-256: `7f139d2aa94b9a4480676303eb5244122e0c340c1793d1d9b68ed71da4f4dbc5`

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=545
- Before SHA-256: `87ab446ed20d6b205024c72c9c0ab30e776a5f0fa652287694ac0ff383cc51ca`
- After SHA-256: `64b7f91fdb11825bf917e61873fd054f08a6d95907d40b362b2c757bcfb73b06`

## 82. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6095
- Before SHA-256: `681537bd41139558c4533f44fed90232af6f364aa1560c02e0860833852e1139`
- After SHA-256: `66dbf04926afe027406597d65bda1bdd9fddad2233e18eb4fdb88a2704178d0a`

## 83. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t1/m6-uniform-series-consumer.csproj`
- Summary: bytes=533
- After SHA-256: `686c2f4da250b04176b9d33999d482e4fe99470dd683c84db43cc16915eeb8cd`

## 84. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t1/Program.cs`
- Summary: bytes=6646
- After SHA-256: `50457c89a74e4d5f5db5aec5a1a12013cb9d4b90991f2349b67c60d6675b0dbd`

## 85. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 86. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c03-s03-t1-uniform-series.md`
- Summary: bytes=4209
- After SHA-256: `36d682fea13799363bf2b93ad390813319a19efac20ece2d50f44f91941df985`

## 87. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-uniform-series.md`
- Summary: bytes=1800
- After SHA-256: `09e67dadfe5642bc567f167d491976333521a3b1936f1c0c5f34dbad99a7821c`

## 88. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T1-qualified.json`
- Summary: bytes=7991
- After SHA-256: `6ef4778227f64cab29efb9e6b45fbfdafbd161fbb474ac4f07c28965c9ff92d6`

## 89. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `2f0a42f63ba26c18caa59faeb36131661d38aef54e056c5a1e9d07c909f11d4b`
- After SHA-256: `bd070c803b0fd0952e36aedcd6164e72da611d392be7e55192d13760c53a5b12`

## 90. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 not-ready->ready
- Before SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`
- After SHA-256: `8d6809d966492cc1103e049c4509a76886bd306515be262e13b1a4711e9e7bda`

## 91. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 not-ready->ready
- Before SHA-256: `8d6809d966492cc1103e049c4509a76886bd306515be262e13b1a4711e9e7bda`
- After SHA-256: `87c6383694f2fcfa84dd62a8d082ed5f824232212f1b58629fcec2bbb608fb65`

## 92. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-R not-ready->ready
- Before SHA-256: `87c6383694f2fcfa84dd62a8d082ed5f824232212f1b58629fcec2bbb608fb65`
- After SHA-256: `10e0e167fe34d6827f9ffa5491d2f6c382abc0a9636e2743c280e8f4d1c9f865`

## 93. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 ready->in-progress
- Before SHA-256: `10e0e167fe34d6827f9ffa5491d2f6c382abc0a9636e2743c280e8f4d1c9f865`
- After SHA-256: `0410a7eb5b4afaeadd19ff431aabd4a9c2339348414f5b33eeb7cc3b7e139336`

## 94. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 ready->in-progress
- Before SHA-256: `0410a7eb5b4afaeadd19ff431aabd4a9c2339348414f5b33eeb7cc3b7e139336`
- After SHA-256: `3b9eb81c4ad9dad086185289fdf0123877ea672fba4bebd38ba350397e7cf28f`

## 95. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-R ready->in-progress
- Before SHA-256: `3b9eb81c4ad9dad086185289fdf0123877ea672fba4bebd38ba350397e7cf28f`
- After SHA-256: `fd6be8e1e83ff64db91eeec9ccfed9e87774b832b218545d36b3069a6a2b5d40`

## 96. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-R in-progress->implemented->validated->done
- Before SHA-256: `fd6be8e1e83ff64db91eeec9ccfed9e87774b832b218545d36b3069a6a2b5d40`
- After SHA-256: `c2aca6ad471499cf333aa1ac19b35d68f6a9f3aac8301f63e955c2e44640d16a`

## 97. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-G not-ready->ready
- Before SHA-256: `c2aca6ad471499cf333aa1ac19b35d68f6a9f3aac8301f63e955c2e44640d16a`
- After SHA-256: `30f3b85ce517e113fa062dd4a2c075ac3445e8aa6f626856e98e672984077d70`

## 98. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-G ready->in-progress
- Before SHA-256: `30f3b85ce517e113fa062dd4a2c075ac3445e8aa6f626856e98e672984077d70`
- After SHA-256: `5b323f8140607050ecad893ea8e8990feb5bd6f3de2958de4305b7f05501c4ed`

## 99. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-G in-progress->implemented->validated->done
- Before SHA-256: `5b323f8140607050ecad893ea8e8990feb5bd6f3de2958de4305b7f05501c4ed`
- After SHA-256: `c4851812448013b3a504a789e31ae528d155e036c09113bdb3dae8211a5b74af`

## 100. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-V not-ready->ready
- Before SHA-256: `c4851812448013b3a504a789e31ae528d155e036c09113bdb3dae8211a5b74af`
- After SHA-256: `f63ac98493123d942c0e4a9c9693b488e7991b874d8356212df61bbce7cc67ae`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-V ready->in-progress
- Before SHA-256: `f63ac98493123d942c0e4a9c9693b488e7991b874d8356212df61bbce7cc67ae`
- After SHA-256: `d5800618b503e4ad0303f1b65f8ddaccbe524a0fc4ee8ce264b53aa50b3ba0ac`

## 102. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-V in-progress->implemented->validated->done
- Before SHA-256: `d5800618b503e4ad0303f1b65f8ddaccbe524a0fc4ee8ce264b53aa50b3ba0ac`
- After SHA-256: `8a387421f2f1e7b11dd7b4ce08b31daf239da7a9028594f5b3c5b383fe2104c9`

## 103. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 in-progress->implemented->validated->done
- Before SHA-256: `8a387421f2f1e7b11dd7b4ce08b31daf239da7a9028594f5b3c5b383fe2104c9`
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 104. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 105. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1 state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 106. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-R state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 107. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-G state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 108. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T1-V state=done
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 109. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03-T2 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 110. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 111. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `a771f778334c8df61f1743c3d6c066be70165bda0aaec3d18e9601fef625f212`

## 115. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `2cbf8cc44a73f659f1c210a2e2446d7507aad0bf7ae2c43eec836e8db41ddbeb`
- After SHA-256: `5791f0b1dd2e870135c0c151d5c2554a8676ab037c807dfcad5df88976f8ffde`

