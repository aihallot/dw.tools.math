# RS096 Attempt 001 Payload Operations

- Operations: 109
- Changed: 31
- No change: 0
- Verified: 78
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS096 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS095/attempt-001.json` | run=RS095 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/local_product_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/independent_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/m6_w02_c03_s02_t1_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/m6_w02_c03_s02_phase_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/m6_w02_c03_parent_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json` | pointer=/publicly_published |
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
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/0/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/id |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/subtasks/0/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/subtasks/1/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/tasks/1/subtasks/2/status |
| 35 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 36 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj` | semantic text found |
| 37 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | semantic text found |
| 38 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | semantic text found |
| 39 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t1/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | semantic text found |
| 41 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | semantic text found |
| 42 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=in-progress |
| 52 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T1 state=done |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 state=not-ready |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-R state=not-ready |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-G state=not-ready |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-V state=not-ready |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=not-ready |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 60 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs` | path is absent |
| 61 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2RedTests.cs` | path is absent |
| 62 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2Tests.cs` | path is absent |
| 63 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | path is absent |
| 64 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | path is absent |
| 65 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c03-s02-t2/NuGet.Config` | path is absent |
| 66 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c03-s02-t2-signal-boundaries.md` | path is absent |
| 67 | `require.file-missing` | Verified | `docs/distribution/m6-direct-signal-boundaries.md` | path is absent |
| 68 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-contract-red.json` | path is absent |
| 69 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | path is absent |
| 70 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2RedTests.cs` | bytes=722 |
| 71 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S02-T2-contract-red.json` | bytes=668 |
| 72 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=681 |
| 73 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs` | bytes=8516 |
| 74 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2Tests.cs` | bytes=18223 |
| 75 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj` | bytes=533 |
| 76 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t1/Program.cs` | bytes=7237 |
| 77 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj` | bytes=544 |
| 78 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s01-t2/Program.cs` | bytes=6256 |
| 79 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=545 |
| 80 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6095 |
| 81 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | bytes=542 |
| 82 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | bytes=7663 |
| 83 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/NuGet.Config` | bytes=113 |
| 84 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c03-s02-t2-signal-boundaries.md` | bytes=4080 |
| 85 | `files.replace-from-staged` | Changed | `docs/distribution/m6-direct-signal-boundaries.md` | bytes=1619 |
| 86 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | bytes=8056 |
| 87 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 88 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 not-ready->ready |
| 89 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-R not-ready->ready |
| 90 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M6-W02-C03-S02-T2-R activated=M6-W02-C03-S02-T2,M6-W02-C03-S02-T2-R |
| 91 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-R in-progress->implemented->validated->done |
| 92 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-G not-ready->ready |
| 93 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-G ready->in-progress |
| 94 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-G in-progress->implemented->validated->done |
| 95 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-V not-ready->ready |
| 96 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-V ready->in-progress |
| 97 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-V in-progress->implemented->validated->done |
| 98 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 in-progress->implemented->validated->done |
| 99 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 in-progress->implemented->validated->done |
| 100 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=done |
| 101 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2 state=done |
| 102 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-R state=done |
| 103 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-G state=done |
| 104 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02-T2-V state=done |
| 105 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 106 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=not-ready |
| 107 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 108 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 109 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS096 hierarchy=exact
- After SHA-256: `2d04c8f296c279c94673a0c89ee5c7a88a4c79a1b9cd1131b5d543224795a2ff`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS095/attempt-001.json`
- Summary: run=RS095 attempt=1 lineage=exact
- After SHA-256: `d1f18307bb150eae0ddf346ec2a37091c4fd52689f57b772e376f0a4f60a7bf2`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/local_product_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_s02_t1_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_s02_phase_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
- Summary: pointer=/m6_w02_c03_parent_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T1-qualified.json`
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
- Summary: pointer=/chunks/47/tasks/0/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/1/id

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
- Summary: pointer=/chunks/47/tasks/1/subtasks/0/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/1/subtasks/1/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/tasks/1/subtasks/2/status

## 35. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t1/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=in-progress
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 52. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T1 state=done
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-R state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-G state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-V state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`

## 60. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs`
- Summary: path is absent

## 61. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2RedTests.cs`
- Summary: path is absent

## 62. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2Tests.cs`
- Summary: path is absent

## 63. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: path is absent

## 64. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: path is absent

## 65. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/NuGet.Config`
- Summary: path is absent

## 66. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c03-s02-t2-signal-boundaries.md`
- Summary: path is absent

## 67. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-direct-signal-boundaries.md`
- Summary: path is absent

## 68. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-contract-red.json`
- Summary: path is absent

## 69. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: path is absent

## 70. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2RedTests.cs`
- Summary: bytes=722
- After SHA-256: `a542f9b058d437abc541aac34f47b303ffde6c7c3931520ad4341455890d65f2`

## 71. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-contract-red.json`
- Summary: bytes=668
- After SHA-256: `84b4e7a2433ea483058ed6e76151783cb3b94ece43d89b415efdc963c144a550`

## 72. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=681
- Before SHA-256: `5920183bc250b531956f657cf98ba03be1dbd17c62262ec38e5982873a6a128c`
- After SHA-256: `1287b65e4675b58f076f15f77dcc5cf2a6c19fc7380bc39998366fb96eb6e53a`

## 73. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/FiniteSignalBoundaryAudit64.cs`
- Summary: bytes=8516
- After SHA-256: `da135763f464d65fc5f71d1bb09689259ea480ac26bde4b976db5ac6ee450ce3`

## 74. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C03S02T2Tests.cs`
- Summary: bytes=18223
- After SHA-256: `d9aea29c465f43341ae22fba850249ecce0227d636469322e1982704ca22c240`

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t1/m6-direct-signal-consumer.csproj`
- Summary: bytes=533
- Before SHA-256: `86affc55ea5be17f3106f31ef09d22ecafa4df4631194f67b8560eb5558d3c7e`
- After SHA-256: `cb5d9554f2e6046be655799b1f8255087277a41352292649873c087a38f334de`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t1/Program.cs`
- Summary: bytes=7237
- Before SHA-256: `184fd604156f77f26529f943f740d51c3e18012f90f5b36512dfb6958ad70a5b`
- After SHA-256: `db59b1f884e262077ae894120f381bd5cfb05129b3afd80ac16751a89d17477a`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/m6-finite-geometry-boundary-consumer.csproj`
- Summary: bytes=544
- Before SHA-256: `af654f816aaec1242c1d9f5a611b8f2cbd8769ae0427cb077ba8dc72ee603ed4`
- After SHA-256: `9f454443125dad3059269a5c002e2fcd6d8e6583dd5542cda362b13a228fa2ce`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s01-t2/Program.cs`
- Summary: bytes=6256
- Before SHA-256: `e36d48d1703253b63edcb2f087305ba00312ce6b39ff07c0dcb63166fade3a85`
- After SHA-256: `188ac9474712b7a442ed980a2e62118714a16cd8a12ec2a6eb9c9ef881466518`

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=545
- Before SHA-256: `72b6b0822683a1a2dae212459ef49dc4cb1b1b7793966cf08532fde5fe723531`
- After SHA-256: `87ab446ed20d6b205024c72c9c0ab30e776a5f0fa652287694ac0ff383cc51ca`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6095
- Before SHA-256: `aa5d9f4d2b345cbdf30973972d73b98fd85e956779eba428f7c48ed0f0cc7f8d`
- After SHA-256: `681537bd41139558c4533f44fed90232af6f364aa1560c02e0860833852e1139`

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: bytes=542
- After SHA-256: `8dd89bc2b35313e2d9808b45ce78918c27993b8060d1f90a025f481a550ed3c4`

## 82. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: bytes=7663
- After SHA-256: `652c3d285b7f59fd6d04e6b2a08e52656d4c0e61a4d4674258bde60c08200fdf`

## 83. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 84. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c03-s02-t2-signal-boundaries.md`
- Summary: bytes=4080
- After SHA-256: `3b099ab917a700e5f1fcd2898e64170a7adb8c4833311481ddbd14925e17259f`

## 85. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-direct-signal-boundaries.md`
- Summary: bytes=1619
- After SHA-256: `267c6f8ba42c640163bbe435794b2ff83582dc81abd807049b5db8a285c3cb8f`

## 86. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: bytes=8056
- After SHA-256: `ddc89685f67e8452a7f38832b017c42124b8c1ae65c6e6b66413bc37acdcdf02`

## 87. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `afc5386f70025828d93731c64fc4fec013883168df7fee6c0a4130ac644c1bfa`
- After SHA-256: `22aa499254114defb5e92958c8384efbe1b14b7c0ecfde076c5d30a5e77b0415`

## 88. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 not-ready->ready
- Before SHA-256: `079416a9feaadeda0a6bc99975b7e4a43b88581677446f401daf1d64eb2313cb`
- After SHA-256: `2eb2ce1f0df9260b26035b49f0dabdae12e5cd03b91294779411db9e6fc9b4de`

## 89. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-R not-ready->ready
- Before SHA-256: `2eb2ce1f0df9260b26035b49f0dabdae12e5cd03b91294779411db9e6fc9b4de`
- After SHA-256: `119cf0f4a641cb5ddd6d0f49e054229568d7379c791b228ee598ce5dbafe9537`

## 90. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M6-W02-C03-S02-T2-R activated=M6-W02-C03-S02-T2,M6-W02-C03-S02-T2-R
- Before SHA-256: `119cf0f4a641cb5ddd6d0f49e054229568d7379c791b228ee598ce5dbafe9537`
- After SHA-256: `1ccc0ba451f1c9d7a9a9780a5634f753a3c85f63f8609e011199707dc450c1bd`

## 91. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-R in-progress->implemented->validated->done
- Before SHA-256: `1ccc0ba451f1c9d7a9a9780a5634f753a3c85f63f8609e011199707dc450c1bd`
- After SHA-256: `8d14ed15140eefb470a37bf62bf3d2b66b713286033b61ee69eaf668b18b256c`

## 92. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-G not-ready->ready
- Before SHA-256: `8d14ed15140eefb470a37bf62bf3d2b66b713286033b61ee69eaf668b18b256c`
- After SHA-256: `655814abfeb6a1061786a1e5851015430082a493b0f9e3678beaa48ef5869138`

## 93. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-G ready->in-progress
- Before SHA-256: `655814abfeb6a1061786a1e5851015430082a493b0f9e3678beaa48ef5869138`
- After SHA-256: `09190250209b2039622f8c87760a613462fe0899ebfa3dc9d8394dbba394cd58`

## 94. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-G in-progress->implemented->validated->done
- Before SHA-256: `09190250209b2039622f8c87760a613462fe0899ebfa3dc9d8394dbba394cd58`
- After SHA-256: `dc3ea8b4d28ef5791f45a5bfb3088a8c94807404184cccb1538f7a737e653cd5`

## 95. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-V not-ready->ready
- Before SHA-256: `dc3ea8b4d28ef5791f45a5bfb3088a8c94807404184cccb1538f7a737e653cd5`
- After SHA-256: `c1c3d2ed82fedc0287e3538386ffefcc196bfa9f7ee3fd2ecc86c44c3f2faa58`

## 96. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-V ready->in-progress
- Before SHA-256: `c1c3d2ed82fedc0287e3538386ffefcc196bfa9f7ee3fd2ecc86c44c3f2faa58`
- After SHA-256: `a3b10dd333bfa47154068cc0ff5f3faa014af187d2a18ac5d3002a41415c6a12`

## 97. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-V in-progress->implemented->validated->done
- Before SHA-256: `a3b10dd333bfa47154068cc0ff5f3faa014af187d2a18ac5d3002a41415c6a12`
- After SHA-256: `706b94c8af78356ba96de5315e9a11582019d4c3ba4ceaa5ee91cd4ab3d9e970`

## 98. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 in-progress->implemented->validated->done
- Before SHA-256: `706b94c8af78356ba96de5315e9a11582019d4c3ba4ceaa5ee91cd4ab3d9e970`
- After SHA-256: `3c9050c3461cedcefebd082b98ca8db1c99e1b2b56682aacaa4c0cf32279596c`

## 99. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 in-progress->implemented->validated->done
- Before SHA-256: `3c9050c3461cedcefebd082b98ca8db1c99e1b2b56682aacaa4c0cf32279596c`
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 100. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 101. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2 state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 102. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-R state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 103. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-G state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 104. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02-T2-V state=done
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 105. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 106. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=not-ready
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 107. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 108. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `0037044057ab8f2e73fbef1b9f48ff01c8adc25bd22ca2d38ea45152cab3df0a`

## 109. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `0e8136a03ba4251e997102cdfe4a5eb1b807d09e6b52d760015193a0889766b2`
- After SHA-256: `0d39a703ac6e9bafabe626c76e8ecdc07255fca686a9246ca2c7d49f57b7a493`

