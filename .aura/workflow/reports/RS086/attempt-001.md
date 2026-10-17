# RS086 Attempt 001 Payload Operations

- Operations: 94
- Changed: 27
- No change: 0
- Verified: 67
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS086 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS085/attempt-001.json` | run=RS085 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/retained_c02_t2_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/retained_c02_t1_tests_passed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/retained_c01_t2_tests_passed |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/retained_c01_t1_tests_passed |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/independent_differential_consumer_probes |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/retained_univariate_consumer_probes |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/local_product_version |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/m6_w01_c03_t1_done |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/m6_w01_c03_phase_done |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/m6_w01_work_package_done |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | pointer=/publicly_published |
| 17 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/41/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/0/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/1/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/1/subtasks/0/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/1/subtasks/1/status |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/1/subtasks/2/status |
| 32 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 33 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj` | semantic text found |
| 34 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c03-t1/Program.cs` | semantic text found |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=in-progress |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 state=done |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-R state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-G state=not-ready |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-V state=not-ready |
| 46 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 47 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2RedTests.cs` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2Tests.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj` | path is absent |
| 51 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t2/Program.cs` | path is absent |
| 52 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t2/NuGet.Config` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w01-c03-t2-differential-boundaries.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/distribution/m6-differential-boundaries.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C03-T2-contract-red.json` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | path is absent |
| 57 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2RedTests.cs` | bytes=693 |
| 58 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C03-T2-contract-red.json` | bytes=590 |
| 59 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | bytes=551 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs` | bytes=7247 |
| 61 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2Tests.cs` | bytes=14455 |
| 62 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj` | bytes=530 |
| 63 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t1/Program.cs` | bytes=4791 |
| 64 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj` | bytes=538 |
| 65 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t2/Program.cs` | bytes=5483 |
| 66 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t2/NuGet.Config` | bytes=113 |
| 67 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w01-c03-t2-differential-boundaries.md` | bytes=2740 |
| 68 | `files.replace-from-staged` | Changed | `docs/distribution/m6-differential-boundaries.md` | bytes=1810 |
| 69 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | bytes=5576 |
| 70 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 71 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 not-ready->ready |
| 72 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-R not-ready->ready |
| 73 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M6-W01-C03-T2-R activated=M6-W01-C03-T2,M6-W01-C03-T2-R |
| 74 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-R in-progress->implemented->validated->done |
| 75 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-G not-ready->ready |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-G ready->in-progress |
| 77 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-G in-progress->implemented->validated->done |
| 78 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-V not-ready->ready |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-V ready->in-progress |
| 80 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-V in-progress->implemented->validated->done |
| 81 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 in-progress->implemented->validated->done |
| 82 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03 in-progress->implemented->validated->done |
| 83 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 84 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 85 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 state=done |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-R state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-G state=done |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2-V state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 94 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS086 hierarchy=exact
- After SHA-256: `8f56b909892054c453b339598fdf406c333dfb5c5024eae1d175d7f529317bd0`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS085/attempt-001.json`
- Summary: run=RS085 attempt=1 lineage=exact
- After SHA-256: `a41b738426f3eb7ce692db86f162d1a5e52341ee7ca8aa33d66f8b7a93fe3ec3`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/retained_c02_t2_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/retained_c02_t1_tests_passed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/retained_c01_t2_tests_passed

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/retained_c01_t1_tests_passed

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/independent_differential_consumer_probes

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/retained_univariate_consumer_probes

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/local_product_version

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/m6_w01_c03_t1_done

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/m6_w01_c03_phase_done

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: pointer=/publicly_published

## 17. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/40/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/41/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/0/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/1/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/1/subtasks/0/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/1/subtasks/1/status

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/1/subtasks/2/status

## 32. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t1/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-R state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-G state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-V state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 46. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 47. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2RedTests.cs`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2Tests.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t2/Program.cs`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t2/NuGet.Config`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w01-c03-t2-differential-boundaries.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-differential-boundaries.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-contract-red.json`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: path is absent

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2RedTests.cs`
- Summary: bytes=693
- After SHA-256: `e6af0d82f152c151ee929db605d6c7d77bfb7d5752fb942b6c61ab6289aa0a0e`

## 58. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C03-T2-contract-red.json`
- Summary: bytes=590
- After SHA-256: `dc38fe5f67ab2a2db0a7479a32f51422361c51c2426ceeea5eca45c912f9030b`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: bytes=551
- Before SHA-256: `4626e8d694fee290d5f4f374a8e8ffc4249dc88fc44814655f2e987ce3cf6743`
- After SHA-256: `f92d0bf06704bb4b153fa976e6e19aca04c744fb89d81884dc5451205c6a3da7`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/DifferentialBoundaryAudit64.cs`
- Summary: bytes=7247
- After SHA-256: `c9591c0ebdf62f64bae11b32d6cfa7639e6f2b86683f72855ea589c68649c77a`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T2Tests.cs`
- Summary: bytes=14455
- After SHA-256: `8c3f7d8e93169855fc6f12966de5c6e768ee4c695fdb290548536aa2fd4fbe67`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj`
- Summary: bytes=530
- Before SHA-256: `2be1c5e074e4e6051a46e4d74402cb45c0a7fda1eb6b957d770b68abc8acd808`
- After SHA-256: `b83dbc9e7f49da625e91f378c3f7d68373c1c0ed2a26dcdc7a39d09d40d0044a`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t1/Program.cs`
- Summary: bytes=4791
- Before SHA-256: `4e1178dc3a1f2dbeaa9f04afd664400dd93962563885793cd7010c5dc6521e64`
- After SHA-256: `890eea083d7654fe7872bd13e2390a6f891b9509a641b4b65c348d05a05b2fb6`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t2/m6-differential-boundary-consumer.csproj`
- Summary: bytes=538
- After SHA-256: `908a155139f592dce55a477d3f295f33fc1b1caeafda538f09a93974caf69118`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t2/Program.cs`
- Summary: bytes=5483
- After SHA-256: `cf31fce4dcae680f650713df717034839808dc69ce3f92cf70c16895bb3431d3`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t2/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w01-c03-t2-differential-boundaries.md`
- Summary: bytes=2740
- After SHA-256: `fc449a42ca3fdfc75421a5d8195ed79355e9737ac364fd6ec8e5f58e0c9c12e0`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-differential-boundaries.md`
- Summary: bytes=1810
- After SHA-256: `43f9dbb7df50faec0face2cf849f007a999e5898b865485c33b555f1881e5891`

## 69. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: bytes=5576
- After SHA-256: `554d5ba8934c0e3faf2bd38aa3e42a8293f5dc264ca49be7344472a547448e2a`

## 70. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `8ae1d2b4fcfe93b459adf2ff341a41383c3789302a68ca24df3d775ee2d0a825`
- After SHA-256: `1e63ff086cacddc92d999405941b474c3639eae088ec3cee9935f61e478d6009`

## 71. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 not-ready->ready
- Before SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`
- After SHA-256: `70c2705944da478514ba29bb95b217240d51ddd73e54965b78352f52686bc71a`

## 72. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-R not-ready->ready
- Before SHA-256: `70c2705944da478514ba29bb95b217240d51ddd73e54965b78352f52686bc71a`
- After SHA-256: `b41894ead9644072fe5c716b06df0e642bad0af72c56ae8b40eb234a0bcfb136`

## 73. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M6-W01-C03-T2-R activated=M6-W01-C03-T2,M6-W01-C03-T2-R
- Before SHA-256: `b41894ead9644072fe5c716b06df0e642bad0af72c56ae8b40eb234a0bcfb136`
- After SHA-256: `16df27fc343412eb62119b9e8872858d7955f285848d680a04a859b9dd80dea5`

## 74. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-R in-progress->implemented->validated->done
- Before SHA-256: `16df27fc343412eb62119b9e8872858d7955f285848d680a04a859b9dd80dea5`
- After SHA-256: `e86c522328b4d78cdd79b1c3c059a458bb264221e88946243612abb60b9d0b23`

## 75. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-G not-ready->ready
- Before SHA-256: `e86c522328b4d78cdd79b1c3c059a458bb264221e88946243612abb60b9d0b23`
- After SHA-256: `b2e74aee67a6ff23ab19cacb7c214e19d198db80b709b9afbfcb332a44e5df71`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-G ready->in-progress
- Before SHA-256: `b2e74aee67a6ff23ab19cacb7c214e19d198db80b709b9afbfcb332a44e5df71`
- After SHA-256: `9c90b4b8f33447b8a1bde08e57ac9c4ca47302d4f229df93441044ec0c4ffc9e`

## 77. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-G in-progress->implemented->validated->done
- Before SHA-256: `9c90b4b8f33447b8a1bde08e57ac9c4ca47302d4f229df93441044ec0c4ffc9e`
- After SHA-256: `07bfce21b955c24314f7eca96ff70eb6b47b692e40718cec6b3eb66a0cc339da`

## 78. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-V not-ready->ready
- Before SHA-256: `07bfce21b955c24314f7eca96ff70eb6b47b692e40718cec6b3eb66a0cc339da`
- After SHA-256: `91e215f355ceb9716e97dcaed8a1a27bd9a781088fe2015b9252984057e7aadf`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-V ready->in-progress
- Before SHA-256: `91e215f355ceb9716e97dcaed8a1a27bd9a781088fe2015b9252984057e7aadf`
- After SHA-256: `aadd049d66fc5eb87eb12e67cdbefdc2aa201388a4b8247cace6014f165f76eb`

## 80. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-V in-progress->implemented->validated->done
- Before SHA-256: `aadd049d66fc5eb87eb12e67cdbefdc2aa201388a4b8247cace6014f165f76eb`
- After SHA-256: `4e23ba0de619d421ba776d9bc0c83430cd85c823a3de99633a5f6d79b0990e91`

## 81. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 in-progress->implemented->validated->done
- Before SHA-256: `4e23ba0de619d421ba776d9bc0c83430cd85c823a3de99633a5f6d79b0990e91`
- After SHA-256: `7287b5fe5ee7a97e4e99f242efc74d8f5870734290550b2be120fe5ad8434e93`

## 82. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 in-progress->implemented->validated->done
- Before SHA-256: `7287b5fe5ee7a97e4e99f242efc74d8f5870734290550b2be120fe5ad8434e93`
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 83. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 84. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 85. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-R state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-G state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2-V state=done
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `03d5864ffd243af7798ef1af0bc6bf2ca2eeed3ea8bddc7282594e965772a2f4`

## 94. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `4893a3d4641fb7594304767d0343f308387b5e7de0ab2c4acc79bf6f3ddb5290`
- After SHA-256: `d1135309b75153963b694970552c99bcd97e5011a66e879bd505b8e10cdfc5c7`

