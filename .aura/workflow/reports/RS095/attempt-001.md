# RS095 Attempt 001 Payload Operations

- Operations: 108
- Changed: 31
- No change: 0
- Verified: 77
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS095 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS094/attempt-001.json` | run=RS094 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_s01_phase_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_parent_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json` | pointer=/status |
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
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/id |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/subtasks/0/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/subtasks/1/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/subtasks/2/status |
| 35 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 36 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | semantic text found |
| 37 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | semantic text found |
| 38 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=not-ready |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 state=not-ready |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-R state=not-ready |
| 52 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-G state=not-ready |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-V state=not-ready |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 state=not-ready |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=not-ready |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 58 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/FiniteSignal64.cs` | path is absent |
| 59 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1RedTests.cs` | path is absent |
| 60 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1Tests.cs` | path is absent |
| 61 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj` | path is absent |
| 62 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t1/Program.cs` | path is absent |
| 63 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t1/NuGet.Config` | path is absent |
| 64 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c03-s02-t1-direct-signals.md` | path is absent |
| 65 | `require.file-missing` | Verified | `docs/distribution/m6-direct-signals.md` | path is absent |
| 66 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-contract-red.json` | path is absent |
| 67 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | path is absent |
| 68 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1RedTests.cs` | bytes=694 |
| 69 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S02-T1-contract-red.json` | bytes=655 |
| 70 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=657 |
| 71 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/FiniteSignal64.cs` | bytes=7851 |
| 72 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1Tests.cs` | bytes=13430 |
| 73 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | bytes=544 |
| 74 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | bytes=6256 |
| 75 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=545 |
| 76 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6095 |
| 77 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj` | bytes=533 |
| 78 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t1/Program.cs` | bytes=7237 |
| 79 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t1/NuGet.Config` | bytes=113 |
| 80 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c03-s02-t1-direct-signals.md` | bytes=4067 |
| 81 | `files.replace-from-staged` | Changed | `docs/distribution/m6-direct-signals.md` | bytes=1870 |
| 82 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | bytes=6935 |
| 83 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 84 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 not-ready->ready |
| 85 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 not-ready->ready |
| 86 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-R not-ready->ready |
| 87 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 ready->in-progress |
| 88 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 ready->in-progress |
| 89 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-R ready->in-progress |
| 90 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-R in-progress->implemented->validated->done |
| 91 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-G not-ready->ready |
| 92 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-G ready->in-progress |
| 93 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-G in-progress->implemented->validated->done |
| 94 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-V not-ready->ready |
| 95 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-V ready->in-progress |
| 96 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-V in-progress->implemented->validated->done |
| 97 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 in-progress->implemented->validated->done |
| 98 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=in-progress |
| 99 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 state=done |
| 100 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-R state=done |
| 101 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-G state=done |
| 102 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1-V state=done |
| 103 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 state=not-ready |
| 104 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 105 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=not-ready |
| 106 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 107 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 108 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS095 hierarchy=exact
- After SHA-256: `4ad64a37d4efb18d45502bf2603783669e2c639b770ee4052204ad527f704280`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS094/attempt-001.json`
- Summary: run=RS094 attempt=1 lineage=exact
- After SHA-256: `30c9d078f6e6a169beb7aeed7fdbf70b8dcafc546ef5004036d24480dfcb2c6a`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_s01_phase_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_parent_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/publicly_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json`
- Summary: pointer=/status

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
- Summary: pointer=/chunks/47/tasks/0/id

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/0/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/1/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/id

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/0/subtasks/0/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/0/subtasks/1/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/0/subtasks/2/status

## 35. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-R state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 52. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-G state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-V state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`

## 58. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/FiniteSignal64.cs`
- Summary: path is absent

## 59. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1RedTests.cs`
- Summary: path is absent

## 60. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1Tests.cs`
- Summary: path is absent

## 61. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj`
- Summary: path is absent

## 62. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t1/Program.cs`
- Summary: path is absent

## 63. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t1/NuGet.Config`
- Summary: path is absent

## 64. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c03-s02-t1-direct-signals.md`
- Summary: path is absent

## 65. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-direct-signals.md`
- Summary: path is absent

## 66. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-contract-red.json`
- Summary: path is absent

## 67. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: path is absent

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1RedTests.cs`
- Summary: bytes=694
- After SHA-256: `ed5d145c9dc6d84a19bd1b205ebab08e6cb9df5b80c68515ae5a5eade04d4d78`

## 69. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-contract-red.json`
- Summary: bytes=655
- After SHA-256: `af54ae3806aaacd132a7ac80fe0841e1e72b5e01d3cb0d9b63344dde82171daa`

## 70. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=657
- Before SHA-256: `9d7df62173146122a11fb4172eb49b6a4a76dde8e886513d2a9c23c10ef062cd`
- After SHA-256: `5920183bc250b531956f657cf98ba03be1dbd17c62262ec38e5982873a6a128c`

## 71. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/FiniteSignal64.cs`
- Summary: bytes=7851
- After SHA-256: `5afd547024f3b4a0928f8916fc6bac359f129d9524273a4b7a8b5cdeb397caff`

## 72. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T1Tests.cs`
- Summary: bytes=13430
- After SHA-256: `bd03a8f90467ead08fde57262724166e81c62242e259e3b34e63c12599abd42b`

## 73. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: bytes=544
- Before SHA-256: `c8662b2540ef5412057cd270e4611df8d89cb82be9e9f9d548ee9c2fd84ef44d`
- After SHA-256: `af654f816aaec1242c1d9f5a611b8f2cbd8769ae0427cb077ba8dc72ee603ed4`

## 74. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: bytes=6256
- Before SHA-256: `eefc2c3e7abbbbdfb7a80bed869c6c53d81827f9e7b47818bf0cc5c30fa1fc5d`
- After SHA-256: `e36d48d1703253b63edcb2f087305ba00312ce6b39ff07c0dcb63166fade3a85`

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=545
- Before SHA-256: `323b670aa891790f2e8840b3702f0bdcb1256e007df6052c0ef7bd8fdc2ee2dd`
- After SHA-256: `72b6b0822683a1a2dae212459ef49dc4cb1b1b7793966cf08532fde5fe723531`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6095
- Before SHA-256: `147868d5b11faa2c91915f48503713addf72b56ef6f994066fd4190992bcb4eb`
- After SHA-256: `aa5d9f4d2b345cbdf30973972d73b98fd85e956779eba428f7c48ed0f0cc7f8d`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj`
- Summary: bytes=533
- After SHA-256: `86affc55ea5be17f3106f31ef09d22ecafa4df4631194f67b8560eb5558d3c7e`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t1/Program.cs`
- Summary: bytes=7237
- After SHA-256: `184fd604156f77f26529f943f740d51c3e18012f90f5b36512dfb6958ad70a5b`

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c03-s02-t1-direct-signals.md`
- Summary: bytes=4067
- After SHA-256: `605419564080830197afb3eeef9233da2c86ec8390848b4a010c353c9a51b702`

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-direct-signals.md`
- Summary: bytes=1870
- After SHA-256: `049823d99ca09f267546458a43e61237e2cda087b8568789f0ca034f51278045`

## 82. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: bytes=6935
- After SHA-256: `8a20e0b11c18a9ca0bca58df7e5c8ebc85e37816c2bbfdafd058e2e73fbc8890`

## 83. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `cb4a04c0998b93b0e21f7c769be74b2c816cc92336a384dc01492d9003588a04`
- After SHA-256: `da66ddeb005dc8f0e24cd23a458b45e08e03030158e430e8005017e528d63acb`

## 84. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 not-ready->ready
- Before SHA-256: `e7327d98f62d264728c3cc88fad63cf04bf4b1ddde8c2a9ee18d0b896bdc9c5e`
- After SHA-256: `011cd6c0b373fa923d953e48dc1ddf0b3deaca8d62733af285fe50dacd968a89`

## 85. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 not-ready->ready
- Before SHA-256: `011cd6c0b373fa923d953e48dc1ddf0b3deaca8d62733af285fe50dacd968a89`
- After SHA-256: `2d052b5d7e1d42031d0a12a416c82463167f667017730efd5062263229aa6e86`

## 86. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-R not-ready->ready
- Before SHA-256: `2d052b5d7e1d42031d0a12a416c82463167f667017730efd5062263229aa6e86`
- After SHA-256: `f9601cbcc14e9acf87172df33a4d261f5ba446059b248de7e9b54795f6a9713a`

## 87. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 ready->in-progress
- Before SHA-256: `f9601cbcc14e9acf87172df33a4d261f5ba446059b248de7e9b54795f6a9713a`
- After SHA-256: `cd493baef2998438d265deceba575053bf1a35b9d4be10ecc6d7025a7f373809`

## 88. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 ready->in-progress
- Before SHA-256: `cd493baef2998438d265deceba575053bf1a35b9d4be10ecc6d7025a7f373809`
- After SHA-256: `c1be58b0ffcc89783d4b2f7a01c199391b5f31d0451b0071ace81be34bdb3c05`

## 89. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-R ready->in-progress
- Before SHA-256: `c1be58b0ffcc89783d4b2f7a01c199391b5f31d0451b0071ace81be34bdb3c05`
- After SHA-256: `f9fa0fad71c7a66f11c2f6d9475035c5375d738b863f599bcf8be4f2ae0b60c6`

## 90. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-R in-progress->implemented->validated->done
- Before SHA-256: `f9fa0fad71c7a66f11c2f6d9475035c5375d738b863f599bcf8be4f2ae0b60c6`
- After SHA-256: `6b6cf803381200f2d5c3478c6149ff42cb8e4f7408961f5c68ad57161433fbfd`

## 91. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-G not-ready->ready
- Before SHA-256: `6b6cf803381200f2d5c3478c6149ff42cb8e4f7408961f5c68ad57161433fbfd`
- After SHA-256: `0b043b0c9a3abb5257b0f6564438117f426e03570225204cd4203c90ee001d24`

## 92. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-G ready->in-progress
- Before SHA-256: `0b043b0c9a3abb5257b0f6564438117f426e03570225204cd4203c90ee001d24`
- After SHA-256: `66710c781c7324d7a3f5e0df79e56cf789b9d6925f4310048419f0fcb10a8f94`

## 93. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-G in-progress->implemented->validated->done
- Before SHA-256: `66710c781c7324d7a3f5e0df79e56cf789b9d6925f4310048419f0fcb10a8f94`
- After SHA-256: `f79a1d38fdc786606f446859e288c6579cdde4c03be0642cb6888154d811f503`

## 94. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-V not-ready->ready
- Before SHA-256: `f79a1d38fdc786606f446859e288c6579cdde4c03be0642cb6888154d811f503`
- After SHA-256: `a553087abaa049fcf5743e73ae56ef6f2788ecd72fcde2cbcd7842b7d42d1dc8`

## 95. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-V ready->in-progress
- Before SHA-256: `a553087abaa049fcf5743e73ae56ef6f2788ecd72fcde2cbcd7842b7d42d1dc8`
- After SHA-256: `648cd394c9f8e39713dafe2ad585d95bfdceae1d09c97ee4d6f7c220ea461086`

## 96. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-V in-progress->implemented->validated->done
- Before SHA-256: `648cd394c9f8e39713dafe2ad585d95bfdceae1d09c97ee4d6f7c220ea461086`
- After SHA-256: `cdf6bdfc7ad0b370e2a936041c48a99b2cc6718d40f7385c46b4bc210e65f18a`

## 97. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 in-progress->implemented->validated->done
- Before SHA-256: `cdf6bdfc7ad0b370e2a936041c48a99b2cc6718d40f7385c46b4bc210e65f18a`
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 98. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 99. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 100. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-R state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 101. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-G state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 102. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1-V state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 103. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 104. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 105. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 106. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 107. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 108. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `57f610cd9a8524e73d9d4ce137b52f3b301e723d725198a3f02a8ee33fccca0b`
- After SHA-256: `8abecd869b13a8621fe4266247512b33f54607785899e5e21a4ec923ac82d743`

