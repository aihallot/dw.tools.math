# RS091 Attempt 001 Payload Operations

- Operations: 92
- Changed: 27
- No change: 0
- Verified: 65
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS091 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS090/attempt-001.json` | run=RS090 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/retained_c01_t1_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/retained_c01_t2_tests_passed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/independent_special_consumer_probes_passed |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/retained_exact_consumer_probes_passed |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/local_product_version |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/m6_w02_c02_t1_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/m6_w02_c02_phase_done |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/m6_w02_work_package_done |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | pointer=/publicly_published |
| 15 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/external_dependencies/2/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/0/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/1/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/1/subtasks/0/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/1/subtasks/1/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/1/subtasks/2/status |
| 30 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 31 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj` | semantic text found |
| 32 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t1/Program.cs` | semantic text found |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=in-progress |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 state=done |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-R state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-G state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-V state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 45 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2RedTests.cs` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2Tests.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t2/NuGet.Config` | path is absent |
| 51 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c02-t2-special-boundaries.md` | path is absent |
| 52 | `require.file-missing` | Verified | `docs/distribution/m6-special-function-boundaries.md` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C02-T2-contract-red.json` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json` | path is absent |
| 55 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2RedTests.cs` | bytes=706 |
| 56 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C02-T2-contract-red.json` | bytes=618 |
| 57 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=668 |
| 58 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs` | bytes=7897 |
| 59 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2Tests.cs` | bytes=19975 |
| 60 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj` | bytes=536 |
| 61 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t1/Program.cs` | bytes=4830 |
| 62 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=545 |
| 63 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6091 |
| 64 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/NuGet.Config` | bytes=113 |
| 65 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c02-t2-special-boundaries.md` | bytes=3273 |
| 66 | `files.replace-from-staged` | Changed | `docs/distribution/m6-special-function-boundaries.md` | bytes=1788 |
| 67 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json` | bytes=5859 |
| 68 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 69 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 not-ready->ready |
| 70 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-R not-ready->ready |
| 71 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M6-W02-C02-T2-R activated=M6-W02-C02-T2,M6-W02-C02-T2-R |
| 72 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-R in-progress->implemented->validated->done |
| 73 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-G not-ready->ready |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-G ready->in-progress |
| 75 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-G in-progress->implemented->validated->done |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-V not-ready->ready |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-V ready->in-progress |
| 78 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-V in-progress->implemented->validated->done |
| 79 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 in-progress->implemented->validated->done |
| 80 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02 in-progress->implemented->validated->done |
| 81 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 82 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 83 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 84 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 85 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 state=done |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-R state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-G state=done |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2-V state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 92 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS091 hierarchy=exact
- After SHA-256: `922c32f0e80c0e32be6bab975d4a111b3ef205b3f36ab6907a3fcb7759972369`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS090/attempt-001.json`
- Summary: run=RS090 attempt=1 lineage=exact
- After SHA-256: `05b1e2c41760a6755b859fad8dcba9ca1565f49e933d32a3944ba837b4d91c6a`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/retained_c01_t1_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/retained_c01_t2_tests_passed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/independent_special_consumer_probes_passed

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/retained_exact_consumer_probes_passed

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/local_product_version

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/m6_w02_c02_t1_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/m6_w02_c02_phase_done

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/m6_w02_work_package_done

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: pointer=/publicly_published

## 15. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

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
- Summary: pointer=/external_dependencies/2/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/0/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/1/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/1/subtasks/0/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/1/subtasks/1/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/1/subtasks/2/status

## 30. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t1/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-R state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-G state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-V state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 45. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2RedTests.cs`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2Tests.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/NuGet.Config`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c02-t2-special-boundaries.md`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-special-function-boundaries.md`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T2-contract-red.json`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json`
- Summary: path is absent

## 55. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2RedTests.cs`
- Summary: bytes=706
- After SHA-256: `e570cbd071ea14d9e3e972fe1ca13d2fb5738d84afbd91d3f3b8194918fcd4f3`

## 56. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C02-T2-contract-red.json`
- Summary: bytes=618
- After SHA-256: `f614b4992eaf94440f015f93794d3e7fb859773ac0c09e6cf19b9547c2ad161c`

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=668
- Before SHA-256: `7acb6dc4d8255586ae3705924172494f544b0f2c8ffaeec5c8cbee556e35f44d`
- After SHA-256: `561954eae3aec000ff53f6664a51a4ff991827f2036771b7067b8f9510bf0c7c`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/SpecialFunctionBoundaryAudit64.cs`
- Summary: bytes=7897
- After SHA-256: `f9d8067dfcb1112ff997c15529fb3c866f2aba1b70b7c7eff064dc3433ecf467`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T2Tests.cs`
- Summary: bytes=19975
- After SHA-256: `08ffea92342cf23ed7c9c916738960d521130de8691c386935e310d4e4eb60ab`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj`
- Summary: bytes=536
- Before SHA-256: `eb0a8109c2daa360b936ab4976a5de57ff03793adc292558d4e4c4992308a758`
- After SHA-256: `f1fba04e478e1d259881c4f45f2a954f9078ad67b1cc645bdd6fa4b83d71a9f6`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t1/Program.cs`
- Summary: bytes=4830
- Before SHA-256: `4d4644069c38088542766558d204d37dd91abf944de06c16fe1e17c7ddbaf6f1`
- After SHA-256: `1645aad3fb92bd54de5e48a3c91d1a4b8b1ff2cdcf47047ca0a28f03810a5bd6`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=545
- After SHA-256: `802a3ac15d0caa460ea6717163644832e15899212c8356299bfc063059a63968`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6091
- After SHA-256: `20aa87fdb16e7f76558ea118959a190fea7478146d01100a2fbb9b9750141a2f`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c02-t2-special-boundaries.md`
- Summary: bytes=3273
- After SHA-256: `2df8d72a51e38955ab7fdd3b5f475316919f0510bfd5a14fc23c6e6cf2fefb1e`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-special-function-boundaries.md`
- Summary: bytes=1788
- After SHA-256: `b420c387706fcd49203e839cc646330f0725a2d4bf070a388e37fd4678a2defd`

## 67. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json`
- Summary: bytes=5859
- After SHA-256: `4509fa0838035665d24a9a2e8ac5c5a1cec14868b9280ab5ad0a33046737f436`

## 68. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `e601cf6ea923238e78eae72c6f298b47ca0feda1b0fab6467756e15b46e174e0`
- After SHA-256: `ce6253bb09cea29a01ed698460fff83c84d984d7ada9ea678345217154a04bb8`

## 69. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 not-ready->ready
- Before SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`
- After SHA-256: `3d5399ef3829a735111f8cc34b11d803659a25c6a634c419e9952a7884333cb3`

## 70. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-R not-ready->ready
- Before SHA-256: `3d5399ef3829a735111f8cc34b11d803659a25c6a634c419e9952a7884333cb3`
- After SHA-256: `7e198fd1513323b146f23fc0deb0141686858f956caa68390e2fca3d45fc0392`

## 71. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M6-W02-C02-T2-R activated=M6-W02-C02-T2,M6-W02-C02-T2-R
- Before SHA-256: `7e198fd1513323b146f23fc0deb0141686858f956caa68390e2fca3d45fc0392`
- After SHA-256: `413066c195a9610f68cf394825cd20fc4181802de089abfab3c7e78e20e0d531`

## 72. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-R in-progress->implemented->validated->done
- Before SHA-256: `413066c195a9610f68cf394825cd20fc4181802de089abfab3c7e78e20e0d531`
- After SHA-256: `6f161b9ab0f1731749a64ed64726537c3c6cb3d565e4fee3c604a253be8f37f5`

## 73. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-G not-ready->ready
- Before SHA-256: `6f161b9ab0f1731749a64ed64726537c3c6cb3d565e4fee3c604a253be8f37f5`
- After SHA-256: `5f3dae8f70bb23dedf51f0f0bc640e0a7aad55a9c8990683a23f54804ba65c0b`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-G ready->in-progress
- Before SHA-256: `5f3dae8f70bb23dedf51f0f0bc640e0a7aad55a9c8990683a23f54804ba65c0b`
- After SHA-256: `239e3a5c8366fb70dbda947c76e3a3e195943fed5228ef2150314b5cbc19bf3b`

## 75. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-G in-progress->implemented->validated->done
- Before SHA-256: `239e3a5c8366fb70dbda947c76e3a3e195943fed5228ef2150314b5cbc19bf3b`
- After SHA-256: `5a8e2bf90c87ba62f9631d787f11f219a09fa68a64c129b813c4975c3150274a`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-V not-ready->ready
- Before SHA-256: `5a8e2bf90c87ba62f9631d787f11f219a09fa68a64c129b813c4975c3150274a`
- After SHA-256: `4cb76d879d414ce2ba7fc912806c4d8058d59817fd3cf5435041a92731e5785e`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-V ready->in-progress
- Before SHA-256: `4cb76d879d414ce2ba7fc912806c4d8058d59817fd3cf5435041a92731e5785e`
- After SHA-256: `b18098920b09279d3c6b283d471708dcb1dbeb997edfcb0ff038413ab1b2b448`

## 78. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-V in-progress->implemented->validated->done
- Before SHA-256: `b18098920b09279d3c6b283d471708dcb1dbeb997edfcb0ff038413ab1b2b448`
- After SHA-256: `744debebf8ddf0d87b1c39a02ea6d928b6c3cfd2e4c53adeb20ea660b33cd243`

## 79. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 in-progress->implemented->validated->done
- Before SHA-256: `744debebf8ddf0d87b1c39a02ea6d928b6c3cfd2e4c53adeb20ea660b33cd243`
- After SHA-256: `5207f043bbed207a6118b047b75442d5664833c6bccb1f009ca33c117b999c62`

## 80. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 in-progress->implemented->validated->done
- Before SHA-256: `5207f043bbed207a6118b047b75442d5664833c6bccb1f009ca33c117b999c62`
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 81. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 82. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 83. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 84. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 85. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-R state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-G state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2-V state=done
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `6e2c95384eb9664e1bbd0a74b2ef9b075e0556da4ae212e40eb90a3aebe8ea8a`

## 92. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `c3d376c7591c5ff14730f4de0e42c016dffae6ea25dc2ed703b415a6cd456ced`
- After SHA-256: `5f7607392cebe21c1f2c1863cb6f7200a6d29efc1324a9ddef1c548863cac2fb`

