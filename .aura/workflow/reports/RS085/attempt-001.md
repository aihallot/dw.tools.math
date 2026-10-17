# RS085 Attempt 001 Payload Operations

- Operations: 95
- Changed: 29
- No change: 0
- Verified: 66
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS085 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS084/attempt-001.json` | run=RS084 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/retained_c02_t1_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/retained_c01_t1_tests_passed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/retained_c01_t2_tests_passed |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/independent_audit_consumer_probes |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/retained_t1_consumer_probes |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/m6_w01_c02_phase_done |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/m6_w01_work_package_done |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 15 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/41/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/0/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/0/subtasks/0/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/0/subtasks/1/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/0/subtasks/2/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/tasks/1/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 30 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 31 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj` | semantic text found |
| 32 | `require.text-contains` | Verified | `tests/consumers/m6-w01-c02-t2/Program.cs` | semantic text found |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-R state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-G state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-V state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 45 | `require.file-missing` | Verified | `src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1RedTests.cs` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1Tests.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t1/Program.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w01-c03-t1/NuGet.Config` | path is absent |
| 51 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w01-c03-t1-differential-ode.md` | path is absent |
| 52 | `require.file-missing` | Verified | `docs/distribution/m6-differential-ode.md` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C03-T1-contract-red.json` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | path is absent |
| 55 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1RedTests.cs` | bytes=659 |
| 56 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C03-T1-contract-red.json` | bytes=594 |
| 57 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | bytes=543 |
| 58 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs` | bytes=11907 |
| 59 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1Tests.cs` | bytes=11686 |
| 60 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj` | bytes=538 |
| 61 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c02-t2/Program.cs` | bytes=5226 |
| 62 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj` | bytes=530 |
| 63 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t1/Program.cs` | bytes=4746 |
| 64 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w01-c03-t1/NuGet.Config` | bytes=113 |
| 65 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w01-c03-t1-differential-ode.md` | bytes=2982 |
| 66 | `files.replace-from-staged` | Changed | `docs/distribution/m6-differential-ode.md` | bytes=1571 |
| 67 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W01-C03-T1-qualified.json` | bytes=5282 |
| 68 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 69 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03 not-ready->ready |
| 70 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 not-ready->ready |
| 71 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-R not-ready->ready |
| 72 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03 ready->in-progress |
| 73 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 ready->in-progress |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-R ready->in-progress |
| 75 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-R in-progress->implemented->validated->done |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-G not-ready->ready |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-G ready->in-progress |
| 78 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-G in-progress->implemented->validated->done |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-V not-ready->ready |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-V ready->in-progress |
| 81 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-V in-progress->implemented->validated->done |
| 82 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 in-progress->implemented->validated->done |
| 83 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 84 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 85 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=in-progress |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=in-progress |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1 state=done |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-R state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-G state=done |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T1-V state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03-T2 state=not-ready |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 95 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS085 hierarchy=exact
- After SHA-256: `33cbe04dcbf5a45bdc2313a2bf1a0fb5b559a3eeaf863bce9c36ca052b6a9a8d`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS084/attempt-001.json`
- Summary: run=RS084 attempt=1 lineage=exact
- After SHA-256: `c22c50bae944567c98a56c18c98c7817f4fddb0aab5990a78b13ee9ccdcef5aa`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/retained_c02_t1_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/retained_c01_t1_tests_passed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/retained_c01_t2_tests_passed

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/independent_audit_consumer_probes

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/retained_t1_consumer_probes

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_c02_phase_done

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
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
- Summary: pointer=/chunks/40/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/41/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/0/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/0/subtasks/0/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/0/subtasks/1/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/0/subtasks/2/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/tasks/1/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 30. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-R state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-G state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-V state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`

## 45. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1RedTests.cs`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1Tests.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t1/Program.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w01-c03-t1/NuGet.Config`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w01-c03-t1-differential-ode.md`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-differential-ode.md`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-contract-red.json`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: path is absent

## 55. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1RedTests.cs`
- Summary: bytes=659
- After SHA-256: `eef229a2b97cd06b05f86230365b232522f02130b347bf2779f0da7e0650a9d4`

## 56. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C03-T1-contract-red.json`
- Summary: bytes=594
- After SHA-256: `72132d283c0df50fc5ae2af0c24fd2efde979c12f76f0cdc9935513e888be3aa`

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: bytes=543
- Before SHA-256: `0587568bdac6483a500ca882d5f2883b9e44923ae1e698ba5f62906a8c3dbe9b`
- After SHA-256: `4626e8d694fee290d5f4f374a8e8ffc4249dc88fc44814655f2e987ce3cf6743`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.analysis/BoundedDifferentialAnalysis64.cs`
- Summary: bytes=11907
- After SHA-256: `5db05839052dea9265438d9210dcfaa9d522298039104f3a915148e28a7ec674`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.analysis.tests/M6W01C03T1Tests.cs`
- Summary: bytes=11686
- After SHA-256: `5bb283d1e44f3fe9e3292c9701b53ac2fa1b6bf89846c4ac89a81a9a11f74b9e`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c02-t2/m6-univariate-boundary-consumer.csproj`
- Summary: bytes=538
- Before SHA-256: `f7af22827535433e0965a37103f6daafb7db68540f99195f5069fe2740750678`
- After SHA-256: `01092fe0b2213a5caaef667843aed7ce757b886fdef551e6d1b9d0776ebd6a68`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c02-t2/Program.cs`
- Summary: bytes=5226
- Before SHA-256: `a4ac6fd3fd02326788e1534b11e8e9f8d6b0b0cbbf4b7c266b24aaa64e4e26c1`
- After SHA-256: `028124f6d8c0b9dafda5ad68644b894b7747e7a6298dbd4b8b11462d53c472c2`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t1/m6-differential-consumer.csproj`
- Summary: bytes=530
- After SHA-256: `2be1c5e074e4e6051a46e4d74402cb45c0a7fda1eb6b957d770b68abc8acd808`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t1/Program.cs`
- Summary: bytes=4746
- After SHA-256: `4e1178dc3a1f2dbeaa9f04afd664400dd93962563885793cd7010c5dc6521e64`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w01-c03-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w01-c03-t1-differential-ode.md`
- Summary: bytes=2982
- After SHA-256: `7e831869e1fcbdc9f7022676ce60fc4684a3f57b04fb3569906c0c73ef7c3d5a`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-differential-ode.md`
- Summary: bytes=1571
- After SHA-256: `84123a0500df260504327711b59e8c7b540e4d59d635fee0e675c42936083424`

## 67. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W01-C03-T1-qualified.json`
- Summary: bytes=5282
- After SHA-256: `d92c228513d43f3c47cf1e99b77f83dd2e53cc9e19556d2f8b40bcff853e497f`

## 68. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `243db4656c4cd6c8091eb25cf452f92932535a5a6c8fe4e07c6fa52cb13b8b57`
- After SHA-256: `5b7dc27b1e92d4c383374bd52d3ea24edc88be01ff639268dbd329bbee4556fc`

## 69. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 not-ready->ready
- Before SHA-256: `8e5eb965c9bc57c25bc5fcc9c6c3d8b43353b4cd9e580e7465009c54b83df7c5`
- After SHA-256: `0d8aa2de4ba07d47ff7b1b3e4a14b7ebce59c5e1d0a648385d30a5436fc90afb`

## 70. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 not-ready->ready
- Before SHA-256: `0d8aa2de4ba07d47ff7b1b3e4a14b7ebce59c5e1d0a648385d30a5436fc90afb`
- After SHA-256: `7c07a578b5ec24b5b72821a324eb193dcd5674ad3445e1edaefd05130cda9125`

## 71. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-R not-ready->ready
- Before SHA-256: `7c07a578b5ec24b5b72821a324eb193dcd5674ad3445e1edaefd05130cda9125`
- After SHA-256: `29a9727071fa6025b9c79763c015c79f301ee246c63e68f4a4aaa0319aec5acd`

## 72. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 ready->in-progress
- Before SHA-256: `29a9727071fa6025b9c79763c015c79f301ee246c63e68f4a4aaa0319aec5acd`
- After SHA-256: `1f82e2848431712ce68054f6f32fd95f37f617cc299890d20bdf1547bd1ecf60`

## 73. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 ready->in-progress
- Before SHA-256: `1f82e2848431712ce68054f6f32fd95f37f617cc299890d20bdf1547bd1ecf60`
- After SHA-256: `181c983f35368478a3e356b7b3b9a37237e991116d1362873fb647d5bc8d76ae`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-R ready->in-progress
- Before SHA-256: `181c983f35368478a3e356b7b3b9a37237e991116d1362873fb647d5bc8d76ae`
- After SHA-256: `63767029f99c5a8f44f3ee0e0346f66786d16e6a4d8113ba436c55938b2504dd`

## 75. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-R in-progress->implemented->validated->done
- Before SHA-256: `63767029f99c5a8f44f3ee0e0346f66786d16e6a4d8113ba436c55938b2504dd`
- After SHA-256: `52be8d740b3b3e4e297e4bf5cd60d938fdd7ec9db9989c4a0981fb41e92cf8fe`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-G not-ready->ready
- Before SHA-256: `52be8d740b3b3e4e297e4bf5cd60d938fdd7ec9db9989c4a0981fb41e92cf8fe`
- After SHA-256: `c640b9fe45327157ae94489c8ea3c84fcf0ec5861106172d35dd227f4318a1e1`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-G ready->in-progress
- Before SHA-256: `c640b9fe45327157ae94489c8ea3c84fcf0ec5861106172d35dd227f4318a1e1`
- After SHA-256: `1ce2b6cc7a8ccde02d981becb50478e467300fb07af046fcb0883c5649db58e1`

## 78. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-G in-progress->implemented->validated->done
- Before SHA-256: `1ce2b6cc7a8ccde02d981becb50478e467300fb07af046fcb0883c5649db58e1`
- After SHA-256: `db37b2995a3c1f727e0bb83cdd03306b4f6486899b9dc7314f32f6bb355757ed`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-V not-ready->ready
- Before SHA-256: `db37b2995a3c1f727e0bb83cdd03306b4f6486899b9dc7314f32f6bb355757ed`
- After SHA-256: `00af3943e50906647d3e783a7107ccc0c71f708607a631bffbd9bf9e6d09d3f6`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-V ready->in-progress
- Before SHA-256: `00af3943e50906647d3e783a7107ccc0c71f708607a631bffbd9bf9e6d09d3f6`
- After SHA-256: `e25c2257e00a03c7520736af6dd4153fbf438317ab0ae6c46e20d91214478f73`

## 81. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-V in-progress->implemented->validated->done
- Before SHA-256: `e25c2257e00a03c7520736af6dd4153fbf438317ab0ae6c46e20d91214478f73`
- After SHA-256: `244764773d305f33d9ea89d377a9737d7a523346ddf2453744661321f9634c83`

## 82. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 in-progress->implemented->validated->done
- Before SHA-256: `244764773d305f33d9ea89d377a9737d7a523346ddf2453744661321f9634c83`
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 83. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 84. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 85. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=in-progress
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1 state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-R state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-G state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T1-V state=done
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03-T2 state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `989fb44c94c132247d08390c136e7354655d9091e8e83a838b16bf4c88e05203`

## 95. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `c57d99df6af976e67f012580186f7a0779b7a8731388bc142b81221ec9347b68`
- After SHA-256: `4600adc2bd9febc6955aff4790890287b994dc4c3e601382dec6abcb2412bb68`

