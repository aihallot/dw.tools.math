# RS090 Attempt 001 Payload Operations

- Operations: 98
- Changed: 30
- No change: 0
- Verified: 68
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS090 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS089/attempt-001.json` | run=RS089 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/retained_t1_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/independent_boundary_consumer_probes_passed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/retained_t1_consumer_probes_passed |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/m6_w02_c01_phase_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/m6_w02_work_package_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M3-W01-C01-provider-qualified.json` | pointer=/selected_package_id |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M3-W01-C01-provider-qualified.json` | pointer=/selected_package_version |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M3-W01-C01-provider-qualified.json` | pointer=/package_license_expression |
| 16 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/external_dependencies/2/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/0/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/1/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/0/subtasks/0/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/0/subtasks/1/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/tasks/0/subtasks/2/status |
| 31 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 32 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj` | semantic text found |
| 33 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c01-t2/Program.cs` | semantic text found |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-R state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-G state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-V state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 state=not-ready |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 46 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs` | path is absent |
| 47 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1RedTests.cs` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1Tests.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj` | path is absent |
| 51 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t1/Program.cs` | path is absent |
| 52 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c02-t1/NuGet.Config` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c02-t1-managed-special-functions.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/distribution/m6-special-functions-real-complex.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C02-T1-contract-red.json` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | path is absent |
| 57 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs` | bytes=2349 |
| 58 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1RedTests.cs` | bytes=902 |
| 59 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C02-T1-contract-red.json` | bytes=617 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=644 |
| 61 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs` | bytes=7369 |
| 62 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1Tests.cs` | bytes=16341 |
| 63 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj` | bytes=540 |
| 64 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c01-t2/Program.cs` | bytes=5283 |
| 65 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj` | bytes=536 |
| 66 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t1/Program.cs` | bytes=4785 |
| 67 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t1/NuGet.Config` | bytes=113 |
| 68 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c02-t1-managed-special-functions.md` | bytes=2916 |
| 69 | `files.replace-from-staged` | Changed | `docs/distribution/m6-special-functions-real-complex.md` | bytes=1774 |
| 70 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C02-T1-qualified.json` | bytes=5973 |
| 71 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 72 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02 not-ready->ready |
| 73 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 not-ready->ready |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-R not-ready->ready |
| 75 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02 ready->in-progress |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 ready->in-progress |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-R ready->in-progress |
| 78 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-R in-progress->implemented->validated->done |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-G not-ready->ready |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-G ready->in-progress |
| 81 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-G in-progress->implemented->validated->done |
| 82 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-V not-ready->ready |
| 83 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-V ready->in-progress |
| 84 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-V in-progress->implemented->validated->done |
| 85 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 in-progress->implemented->validated->done |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=in-progress |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1 state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-R state=done |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-G state=done |
| 95 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T1-V state=done |
| 96 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02-T2 state=not-ready |
| 97 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=not-ready |
| 98 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS090 hierarchy=exact
- After SHA-256: `020871408fe88976d18a6f24e62a944845e4ba6b6e3b9fee8d3bac387d162f53`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS089/attempt-001.json`
- Summary: run=RS089 attempt=1 lineage=exact
- After SHA-256: `5102903282162daf49a1209b8cf5d097747adb38e697540517e75119e68035ee`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/retained_t1_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/independent_boundary_consumer_probes_passed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/retained_t1_consumer_probes_passed

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c01_phase_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_work_package_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/publicly_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M3-W01-C01-provider-qualified.json`
- Summary: pointer=/selected_package_id

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M3-W01-C01-provider-qualified.json`
- Summary: pointer=/selected_package_version

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M3-W01-C01-provider-qualified.json`
- Summary: pointer=/package_license_expression

## 16. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/external_dependencies/2/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/0/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/1/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/0/subtasks/0/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/0/subtasks/1/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/tasks/0/subtasks/2/status

## 31. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c01-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-R state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-G state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-V state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`

## 46. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1RedTests.cs`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1Tests.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t1/Program.cs`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t1/NuGet.Config`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c02-t1-managed-special-functions.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-special-functions-real-complex.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-contract-red.json`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: path is absent

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/SpecialFunctionContracts64.cs`
- Summary: bytes=2349
- After SHA-256: `5add6c1e86a47004b759197115a2bc9a0386eb4bc806bcb6ab4a665b6c916582`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1RedTests.cs`
- Summary: bytes=902
- After SHA-256: `3b8edeb484a10bf38d0bb65858c00ef7d8ac4c2663d3082173c8eccc5d02c9e1`

## 59. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C02-T1-contract-red.json`
- Summary: bytes=617
- After SHA-256: `6dc4dc3672dabe2bb72a96a77a3516a848dfcee6fcafd14ceb6a007a90c15ad7`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=644
- Before SHA-256: `33a56779ae2be0fe3b632b91d6bf9708391398764c842927231c9b02c2b2d215`
- After SHA-256: `7acb6dc4d8255586ae3705924172494f544b0f2c8ffaeec5c8cbee556e35f44d`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/ManagedSpecialFunctions64.cs`
- Summary: bytes=7369
- After SHA-256: `ecc8004f78e14584df80ac66cec069ce43ab5f04d742e934c6bb7119e2f24e78`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C02T1Tests.cs`
- Summary: bytes=16341
- After SHA-256: `d5de835fdc49a58b4df07d42087be59dec39a9818e57278240b30656c48c447e`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c01-t2/m6-exact-boundary-consumer.csproj`
- Summary: bytes=540
- Before SHA-256: `3834a63cca4f7b0bc25375746b1ac6868f9934d79f0d03b1bdfd19bee2d5c945`
- After SHA-256: `92c07eab44cfddef4f137c7b3009e1a28b7918fa30a1462b9672e7ba5a3dd625`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c01-t2/Program.cs`
- Summary: bytes=5283
- Before SHA-256: `06216a6ce55ba9ed941b9cd05f562da35cfd853d8946ff6880bce3228deae84b`
- After SHA-256: `0a06730f60e0801ca54729443ee00e932ea44f803b34014a29581d669bb2752e`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t1/m6-special-functions-consumer.csproj`
- Summary: bytes=536
- After SHA-256: `eb0a8109c2daa360b936ab4976a5de57ff03793adc292558d4e4c4992308a758`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t1/Program.cs`
- Summary: bytes=4785
- After SHA-256: `4d4644069c38088542766558d204d37dd91abf944de06c16fe1e17c7ddbaf6f1`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c02-t1-managed-special-functions.md`
- Summary: bytes=2916
- After SHA-256: `3f1e45c4afe966df728c751a7c17a4ef7553c311de46ca4ca457701a70675c6d`

## 69. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-special-functions-real-complex.md`
- Summary: bytes=1774
- After SHA-256: `06542b886f4886ebb1cdb0d8c60e30f9b262cdf70e626abd90c9386ebb70f11b`

## 70. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C02-T1-qualified.json`
- Summary: bytes=5973
- After SHA-256: `da219c46fff835d931badbfb31e29f30e65d8e59765487915ab00a7cfbc43467`

## 71. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `ec81ebb96510b01ec98961011112bedb6dc648cb81ac4fb467fd65620f9a8bc4`
- After SHA-256: `e4de72705055888af9cfd65191409e7e57ea4fd8ae2c4e55ea30f04eff121bf4`

## 72. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 not-ready->ready
- Before SHA-256: `d852a79a3b398214b2516feb8e19d3df1358c599f4fdd1d6843f89bfab2108a1`
- After SHA-256: `dddf553b555866ea5650cf3ab39d254ef9a895996b00f9ef4f6d081b011d97c8`

## 73. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 not-ready->ready
- Before SHA-256: `dddf553b555866ea5650cf3ab39d254ef9a895996b00f9ef4f6d081b011d97c8`
- After SHA-256: `e5581b49c8319408aa110100daab89c99ee56803c2251ba7730c70887adc9065`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-R not-ready->ready
- Before SHA-256: `e5581b49c8319408aa110100daab89c99ee56803c2251ba7730c70887adc9065`
- After SHA-256: `c5abe975962dd92911424de1ba0cca9dd74a08e36d8eba99e6cddc2be1b86724`

## 75. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 ready->in-progress
- Before SHA-256: `c5abe975962dd92911424de1ba0cca9dd74a08e36d8eba99e6cddc2be1b86724`
- After SHA-256: `52ccbc10e914d75a004f86adb0d51c41ade2620fbe51ba64a0d6e1f21df8b932`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 ready->in-progress
- Before SHA-256: `52ccbc10e914d75a004f86adb0d51c41ade2620fbe51ba64a0d6e1f21df8b932`
- After SHA-256: `03de927e7aaa8891cfbf484ef93aeeb7009dd85ec29940b4376bb4a067a4e2a6`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-R ready->in-progress
- Before SHA-256: `03de927e7aaa8891cfbf484ef93aeeb7009dd85ec29940b4376bb4a067a4e2a6`
- After SHA-256: `d737a9b9868b51e63a085c9b249abee811b5045e0538ad7cfb63e8e5e27b659a`

## 78. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-R in-progress->implemented->validated->done
- Before SHA-256: `d737a9b9868b51e63a085c9b249abee811b5045e0538ad7cfb63e8e5e27b659a`
- After SHA-256: `41246ebd53c7f628e81c9b39b7cd322853680d54ca42c02489343a3cc62eec25`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-G not-ready->ready
- Before SHA-256: `41246ebd53c7f628e81c9b39b7cd322853680d54ca42c02489343a3cc62eec25`
- After SHA-256: `afe3ada202f87954dfd821879c36d1cb852374812d5686b1bf6e84e57dd883c6`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-G ready->in-progress
- Before SHA-256: `afe3ada202f87954dfd821879c36d1cb852374812d5686b1bf6e84e57dd883c6`
- After SHA-256: `68d357a530ec07c8e41cac8d00bc84e1556a84fc3d79c535398de124e7bf301d`

## 81. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-G in-progress->implemented->validated->done
- Before SHA-256: `68d357a530ec07c8e41cac8d00bc84e1556a84fc3d79c535398de124e7bf301d`
- After SHA-256: `ccf6823f4634e4753cd0b006f5341606b44446b133e0d4a4d9eca1acbea2be01`

## 82. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-V not-ready->ready
- Before SHA-256: `ccf6823f4634e4753cd0b006f5341606b44446b133e0d4a4d9eca1acbea2be01`
- After SHA-256: `dcfefe0c54379b172e8db7a87f35cbbaf347d54806de48e005257822216e2035`

## 83. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-V ready->in-progress
- Before SHA-256: `dcfefe0c54379b172e8db7a87f35cbbaf347d54806de48e005257822216e2035`
- After SHA-256: `4da313ef3ccde60addac13f9bc2d21db898591318a08cdace1cd4adb5b3ee15b`

## 84. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-V in-progress->implemented->validated->done
- Before SHA-256: `4da313ef3ccde60addac13f9bc2d21db898591318a08cdace1cd4adb5b3ee15b`
- After SHA-256: `7cd76835afe5434d0651b60c7310b2f1f22774622d99dcccbb92d54f2ce082d2`

## 85. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 in-progress->implemented->validated->done
- Before SHA-256: `7cd76835afe5434d0651b60c7310b2f1f22774622d99dcccbb92d54f2ce082d2`
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=in-progress
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1 state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-R state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-G state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 95. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T1-V state=done
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 96. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02-T2 state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 97. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=not-ready
- After SHA-256: `13b004a8d71c7d8e9de8d47cd83ec203fa417320cb2027293435f1e7cb88d6f7`

## 98. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `bde42000c0e82fd3e56d7e8a559c7ab1882154417ac251215a31c3c6c909de08`
- After SHA-256: `e00e59b114482ef08b818e305e5c301f9bf21ada4b0cf178a5e7a571e687fabe`

