# RS074 Attempt 001 Payload Operations

- Operations: 100
- Changed: 31
- No change: 0
- Verified: 69
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS074 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS073/attempt-001.json` | run=RS073 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/isolated_consumer_probes_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json` | pointer=/m5_w01_work_package_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-work-package-qualified.json` | pointer=/status |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-work-package-qualified.json` | pointer=/m5_w01_work_package_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-work-package-qualified.json` | pointer=/m5_release_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-work-package-qualified.json` | pointer=/last_codecs_version |
| 13 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/4/status |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/11/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/12/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/33/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/34/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/36/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/tasks/0/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/tasks/0/subtasks/0/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/tasks/0/subtasks/1/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/tasks/0/subtasks/2/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/37/tasks/1/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/status |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 31 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=done |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=not-ready |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01 state=not-ready |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1 state=not-ready |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-R state=not-ready |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-G state=not-ready |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-V state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T2 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02 state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C03 state=not-ready |
| 42 | `require.file-missing` | Verified | `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj` | path is absent |
| 43 | `require.file-missing` | Verified | `src/projects/dw.tools.math.distribution/packages.lock.json` | path is absent |
| 44 | `require.file-missing` | Verified | `src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs` | path is absent |
| 45 | `require.file-missing` | Verified | `src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/packages.lock.json` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1RedTests.cs` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1Tests.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `samples/m5-exact-vertical/m5-exact-vertical.csproj` | path is absent |
| 51 | `require.file-missing` | Verified | `samples/m5-exact-vertical/Program.cs` | path is absent |
| 52 | `require.file-missing` | Verified | `samples/m5-exact-vertical/NuGet.Config` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/decisions/m5-w02-c01-t1-public-vertical.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/distribution/m5-exact-console-vertical.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W02-C01-T1-contract-red.json` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json` | path is absent |
| 57 | `require.text-contains` | Verified | `dw.tools.math.slnx` | semantic text found |
| 58 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj` | bytes=1130 |
| 59 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/packages.lock.json` | bytes=959 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs` | bytes=2830 |
| 61 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj` | bytes=893 |
| 62 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/packages.lock.json` | bytes=5153 |
| 63 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1RedTests.cs` | bytes=622 |
| 64 | `files.replace-from-staged` | Changed | `dw.tools.math.slnx` | bytes=1873 |
| 65 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W02-C01-T1-contract-red.json` | bytes=1099 |
| 66 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs` | bytes=9459 |
| 67 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1Tests.cs` | bytes=11331 |
| 68 | `files.replace-from-staged` | Changed | `samples/m5-exact-vertical/m5-exact-vertical.csproj` | bytes=533 |
| 69 | `files.replace-from-staged` | Changed | `samples/m5-exact-vertical/Program.cs` | bytes=5706 |
| 70 | `files.replace-from-staged` | Changed | `samples/m5-exact-vertical/NuGet.Config` | bytes=117 |
| 71 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m5-w02-c01-t1-public-vertical.md` | bytes=3071 |
| 72 | `files.replace-from-staged` | Changed | `docs/distribution/m5-exact-console-vertical.md` | bytes=1624 |
| 73 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json` | bytes=8535 |
| 74 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 75 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02 not-ready->ready |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01 not-ready->ready |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1 not-ready->ready |
| 78 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-R not-ready->ready |
| 79 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M5-W02-C01-T1-R activated=M5-W02,M5-W02-C01,M5-W02-C01-T1,M5-W02-C01-T1-R |
| 80 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-R in-progress->implemented->validated->done |
| 81 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-G not-ready->ready |
| 82 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-G ready->in-progress |
| 83 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-G in-progress->implemented->validated->done |
| 84 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-V not-ready->ready |
| 85 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-V ready->in-progress |
| 86 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-V in-progress->implemented->validated->done |
| 87 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1 in-progress->implemented->validated->done |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=in-progress |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01 state=in-progress |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1 state=done |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-R state=done |
| 95 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-G state=done |
| 96 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T1-V state=done |
| 97 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01-T2 state=not-ready |
| 98 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02 state=not-ready |
| 99 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C03 state=not-ready |
| 100 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS074 hierarchy=exact
- After SHA-256: `e4f97ef7ff5811a1b45eff118c2448e1fde9354f8dd0182b8014a2fc21024144`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS073/attempt-001.json`
- Summary: run=RS073 attempt=1 lineage=exact
- After SHA-256: `4b919f9f991fe3e3f8d3c4c0c6c0caf7dc5e7d1d703be48ca0c72ce4984e43f1`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/isolated_consumer_probes_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C04-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w01_work_package_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-work-package-qualified.json`
- Summary: pointer=/status

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-work-package-qualified.json`
- Summary: pointer=/m5_w01_work_package_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-work-package-qualified.json`
- Summary: pointer=/m5_release_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-work-package-qualified.json`
- Summary: pointer=/last_codecs_version

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
- Summary: pointer=/releases/4/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/11/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/12/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/33/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/34/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/36/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/tasks/0/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/tasks/0/subtasks/0/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/tasks/0/subtasks/1/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/tasks/0/subtasks/2/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/37/tasks/1/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/status

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 31. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=done
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-R state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-G state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-V state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T2 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C03 state=not-ready
- After SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`

## 42. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj`
- Summary: path is absent

## 43. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/packages.lock.json`
- Summary: path is absent

## 44. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs`
- Summary: path is absent

## 45. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/packages.lock.json`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1RedTests.cs`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1Tests.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `samples/m5-exact-vertical/m5-exact-vertical.csproj`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `samples/m5-exact-vertical/Program.cs`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `samples/m5-exact-vertical/NuGet.Config`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m5-w02-c01-t1-public-vertical.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m5-exact-console-vertical.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T1-contract-red.json`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json`
- Summary: path is absent

## 57. `require.text-contains`

- Status: `Verified`
- Path: `dw.tools.math.slnx`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj`
- Summary: bytes=1130
- After SHA-256: `2fe69b302d01ffa5a089a402dc1db8e8a80a6bb72a0834926884ac5534536b3a`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/packages.lock.json`
- Summary: bytes=959
- After SHA-256: `a2cb96aba0fbeb87f86576b16d3b24a01511e6691a77f2e2ab0ed491d3c9da1b`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/ExactVerticalContracts.cs`
- Summary: bytes=2830
- After SHA-256: `68590d91d4da670b7073f176852a4ad7d802a39198b254f8a5069d48ec0cd239`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/dw.tools.math.distribution.tests.csproj`
- Summary: bytes=893
- After SHA-256: `94210bb115dd1af2e2d573fe90dde063cae6344ee001401d6150323fe1c60342`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/packages.lock.json`
- Summary: bytes=5153
- After SHA-256: `3dcb6ce1951ab25deb689fc678f917ff7220f82eb13c87f319a66552d36ce0f0`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1RedTests.cs`
- Summary: bytes=622
- After SHA-256: `3e32c2810f3651638698116c8532c3b5aa448691b072c783111600afdbacf096`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `dw.tools.math.slnx`
- Summary: bytes=1873
- Before SHA-256: `ec812d4e598044b8ac065aa9bb50172e904f776027b5cf017067a96eba9f88df`
- After SHA-256: `7315e9bdbaec3cda7f7a9de9f61b2f43c477d4ec6af7586bd6761b69a6296416`

## 65. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W02-C01-T1-contract-red.json`
- Summary: bytes=1099
- After SHA-256: `6ce428ede9aa0fefa956ebbe6dd3bf80f6c3daaeded0644a7499d82a5a819a49`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/ExactMathVerticalRunner.cs`
- Summary: bytes=9459
- After SHA-256: `556bac495038ccbf2a66a0e648f8cc0c07336cfb791fb373499e2d58907e64ea`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C01T1Tests.cs`
- Summary: bytes=11331
- After SHA-256: `7a16b44d2c858683544f95185cf05641bfcae5098b1850865f4cd52299f50493`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `samples/m5-exact-vertical/m5-exact-vertical.csproj`
- Summary: bytes=533
- After SHA-256: `326b10ac589126db4ca251451fff97811a1b2eeefcec97f72d688e0fee14299d`

## 69. `files.replace-from-staged`

- Status: `Changed`
- Path: `samples/m5-exact-vertical/Program.cs`
- Summary: bytes=5706
- After SHA-256: `3ed86c842fb39594d2fd2ac2a4b3946115a679fbd23bf198e75541ed401fee14`

## 70. `files.replace-from-staged`

- Status: `Changed`
- Path: `samples/m5-exact-vertical/NuGet.Config`
- Summary: bytes=117
- After SHA-256: `964302159308872b9729e1828a1ba897f30634f7bd751cde11ea8e6fed8cfb97`

## 71. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m5-w02-c01-t1-public-vertical.md`
- Summary: bytes=3071
- After SHA-256: `3e3d68a1571545f2cbc10f3fa4925ea9e95425a935ebc5fe2d8e6c7763de5f6d`

## 72. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m5-exact-console-vertical.md`
- Summary: bytes=1624
- After SHA-256: `1f5125518cda3f99118d5639b950090862c27ec006ac590cf7e58191710eedc4`

## 73. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W02-C01-T1-contract-qualified.json`
- Summary: bytes=8535
- After SHA-256: `366117336b79281647858dd21816e3ffe8a3fe745ac502be02aa217346426705`

## 74. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `5776aeb785e3153008815787ca11b9301326f6e478c16240fb62906c8fce4d27`
- After SHA-256: `12510a179356852fde559dde8e2343b332739bcaa4610b87ea0323d1581900b2`

## 75. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 not-ready->ready
- Before SHA-256: `7f40b706ec3358da17badd82acc8323161d1f65a42533ec5eb5781c3cadd8e61`
- After SHA-256: `42eb2e3c20ce47f3d64ec5e055b02d017eb84ca17bc8d09311e69f9c02a0add2`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01 not-ready->ready
- Before SHA-256: `42eb2e3c20ce47f3d64ec5e055b02d017eb84ca17bc8d09311e69f9c02a0add2`
- After SHA-256: `51a63ede390f05b23890da4f7f461f0b42fb3822cae29b287f1b25d50c8d8b66`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1 not-ready->ready
- Before SHA-256: `51a63ede390f05b23890da4f7f461f0b42fb3822cae29b287f1b25d50c8d8b66`
- After SHA-256: `a0833f64f30c71b4abcebcfadd44e6d85a5024e85d2e18fb469474fa4025fc64`

## 78. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-R not-ready->ready
- Before SHA-256: `a0833f64f30c71b4abcebcfadd44e6d85a5024e85d2e18fb469474fa4025fc64`
- After SHA-256: `48f2f47479f9c810c84f56ee8c0063b48ab2a3407fa0a28c4863f5a3c26dbcbd`

## 79. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M5-W02-C01-T1-R activated=M5-W02,M5-W02-C01,M5-W02-C01-T1,M5-W02-C01-T1-R
- Before SHA-256: `48f2f47479f9c810c84f56ee8c0063b48ab2a3407fa0a28c4863f5a3c26dbcbd`
- After SHA-256: `9bfe680b372eacfe5c388768917411bae67808ce5f9d37590fd3016704fc9830`

## 80. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-R in-progress->implemented->validated->done
- Before SHA-256: `9bfe680b372eacfe5c388768917411bae67808ce5f9d37590fd3016704fc9830`
- After SHA-256: `db15c7826ec39b698de4ea0332ccdcb54727bc9cac1df1da5ff691c5669e7c3e`

## 81. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-G not-ready->ready
- Before SHA-256: `db15c7826ec39b698de4ea0332ccdcb54727bc9cac1df1da5ff691c5669e7c3e`
- After SHA-256: `5d1555a35e56bcc5769b2e8755ed97f3fd693a19851b5135ebe6e624e82763fa`

## 82. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-G ready->in-progress
- Before SHA-256: `5d1555a35e56bcc5769b2e8755ed97f3fd693a19851b5135ebe6e624e82763fa`
- After SHA-256: `960bf11f5458bcba44fdf20d6f7cb56acf657aeb9e2b7f22e8cfd30a3bc82383`

## 83. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-G in-progress->implemented->validated->done
- Before SHA-256: `960bf11f5458bcba44fdf20d6f7cb56acf657aeb9e2b7f22e8cfd30a3bc82383`
- After SHA-256: `7d711cf1503cb49e5a6c67add44b7a9ed0b070e2de96a7269419da140a05cfac`

## 84. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-V not-ready->ready
- Before SHA-256: `7d711cf1503cb49e5a6c67add44b7a9ed0b070e2de96a7269419da140a05cfac`
- After SHA-256: `dfb699bc577c073982883e50f13ea8755b18632417743ec03cbbf70946f11872`

## 85. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-V ready->in-progress
- Before SHA-256: `dfb699bc577c073982883e50f13ea8755b18632417743ec03cbbf70946f11872`
- After SHA-256: `91558c418fe38207050e7265d5cfa424cd5d0c4bba321b66b9330854f7efaa4f`

## 86. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-V in-progress->implemented->validated->done
- Before SHA-256: `91558c418fe38207050e7265d5cfa424cd5d0c4bba321b66b9330854f7efaa4f`
- After SHA-256: `07adbc4990b99ae26b71cf694bc063feaf096bbf6f00d2a43d52790530082c3e`

## 87. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1 in-progress->implemented->validated->done
- Before SHA-256: `07adbc4990b99ae26b71cf694bc063feaf096bbf6f00d2a43d52790530082c3e`
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=in-progress
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01 state=in-progress
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1 state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-R state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 95. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-G state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 96. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T1-V state=done
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 97. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01-T2 state=not-ready
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 98. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02 state=not-ready
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 99. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C03 state=not-ready
- After SHA-256: `79acb39880d276e77178cbc33e310ae0bfe1709aef416d2f465bd9d23bbd7d04`

## 100. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `28d189bd00755790e1531542953b30169996e068b024b5e8ad04bf653cfcf02c`
- After SHA-256: `1c3b050f73a95e03a6126784ccaa1aa3ecc5d7092e95166a6c88a8a51cc62c89`

