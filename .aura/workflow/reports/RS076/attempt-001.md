# RS076 Attempt 001 Payload Operations

- Operations: 96
- Changed: 28
- No change: 0
- Verified: 68
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS076 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS075/attempt-001.json` | run=RS075 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/isolated_consumer_probes_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/retained_sample_probes_passed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/compiled_red_observed |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/local_distribution_version |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/m5_w02_c01_phase_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/m5_w02_work_package_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json` | pointer=/m5_release_done |
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
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/tasks/0/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/tasks/0/subtasks/0/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/tasks/0/subtasks/1/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/tasks/0/subtasks/2/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/38/tasks/1/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/39/status |
| 31 | `require.text-contains` | Verified | `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj` | semantic text found |
| 32 | `require.text-contains` | Verified | `samples/m5-exact-vertical/m5-exact-vertical.csproj` | semantic text found |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=done |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=in-progress |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01 state=done |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02 state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-R state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-G state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-V state=not-ready |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T2 state=not-ready |
| 44 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C03 state=not-ready |
| 45 | `require.file-missing` | Verified | `src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1RedTests.cs` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/consumers/m5-w02-c02-t1/Program.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m5-w02-c02-t1/NuGet.Config` | path is absent |
| 51 | `require.file-missing` | Verified | `docs/distribution/m5-package-readme.md` | path is absent |
| 52 | `require.file-missing` | Verified | `docs/distribution/m5-package-notices.md` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/decisions/m5-w02-c02-t1-clean-packages.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/distribution/m5-local-support-evidence.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W02-C02-T1-contract-red.json` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json` | path is absent |
| 57 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1RedTests.cs` | bytes=633 |
| 58 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W02-C02-T1-contract-red.json` | bytes=871 |
| 59 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj` | bytes=1427 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs` | bytes=7912 |
| 61 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs` | bytes=14499 |
| 62 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj` | bytes=534 |
| 63 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w02-c02-t1/Program.cs` | bytes=5401 |
| 64 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w02-c02-t1/NuGet.Config` | bytes=113 |
| 65 | `files.replace-from-staged` | Changed | `samples/m5-exact-vertical/m5-exact-vertical.csproj` | bytes=533 |
| 66 | `files.replace-from-staged` | Changed | `docs/distribution/m5-package-readme.md` | bytes=916 |
| 67 | `files.replace-from-staged` | Changed | `docs/distribution/m5-package-notices.md` | bytes=717 |
| 68 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m5-w02-c02-t1-clean-packages.md` | bytes=3082 |
| 69 | `files.replace-from-staged` | Changed | `docs/distribution/m5-local-support-evidence.md` | bytes=1291 |
| 70 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json` | bytes=8031 |
| 71 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 72 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02 not-ready->ready |
| 73 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1 not-ready->ready |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-R not-ready->ready |
| 75 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M5-W02-C02-T1-R activated=M5-W02-C02,M5-W02-C02-T1,M5-W02-C02-T1-R |
| 76 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-R in-progress->implemented->validated->done |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-G not-ready->ready |
| 78 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-G ready->in-progress |
| 79 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-G in-progress->implemented->validated->done |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-V not-ready->ready |
| 81 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-V ready->in-progress |
| 82 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-V in-progress->implemented->validated->done |
| 83 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1 in-progress->implemented->validated->done |
| 84 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 85 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=done |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=in-progress |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C01 state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02 state=in-progress |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1 state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-R state=done |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-G state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T1-V state=done |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C02-T2 state=not-ready |
| 95 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02-C03 state=not-ready |
| 96 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS076 hierarchy=exact
- After SHA-256: `6bad7a369875b9fd8a44dd418713852fa3cfd8e924dafaa8cdff8b0fa8b47dc6`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS075/attempt-001.json`
- Summary: run=RS075 attempt=1 lineage=exact
- After SHA-256: `bcc8ffb498cc4cbcb03ded7b600d6f5f2de8536a37d97ab36deafc35b66185c8`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/isolated_consumer_probes_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/retained_sample_probes_passed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/compiled_red_observed

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/local_distribution_version

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w02_c01_phase_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w02_work_package_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m5_release_done

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
- Summary: pointer=/chunks/38/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/tasks/0/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/tasks/0/subtasks/0/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/tasks/0/subtasks/1/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/tasks/0/subtasks/2/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/38/tasks/1/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/39/status

## 31. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `samples/m5-exact-vertical/m5-exact-vertical.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=done
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=in-progress
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01 state=done
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02 state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1 state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-R state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-G state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-V state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T2 state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 44. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C03 state=not-ready
- After SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`

## 45. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1RedTests.cs`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w02-c02-t1/Program.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w02-c02-t1/NuGet.Config`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m5-package-readme.md`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m5-package-notices.md`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m5-w02-c02-t1-clean-packages.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m5-local-support-evidence.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C02-T1-contract-red.json`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json`
- Summary: path is absent

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1RedTests.cs`
- Summary: bytes=633
- After SHA-256: `d5debb80e49da1d3da20694e3fd7f0900429587dceaa33a02ec682a1ca619fda`

## 58. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W02-C02-T1-contract-red.json`
- Summary: bytes=871
- After SHA-256: `f14e348a9b44b21f8b5f66a61d1bb67f129430e51f0c334dd3934a40a32eb7f4`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/dw.tools.math.distribution.csproj`
- Summary: bytes=1427
- Before SHA-256: `3bf50cd09a6d04bd6634d47222689ad36a03e2b7fc5fa9c82aca25f86e0f07e9`
- After SHA-256: `a1289d3f4262d44ad31f458936b1082b568541f98db7b5807d4dc5ffc4c3b3fd`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.distribution/MathLocalReleaseQualification.cs`
- Summary: bytes=7912
- After SHA-256: `0eba615d6ba67e12d4a32ef9cab9afe508f5cc53ef4f9514e96f8ebf5f63fcb0`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.distribution.tests/M5W02C02T1Tests.cs`
- Summary: bytes=14499
- After SHA-256: `f4055ff9b71b5f12652b0816656e010cc2befa3f3683a41f12414b3a3349c97d`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w02-c02-t1/m5-clean-package-consumer.csproj`
- Summary: bytes=534
- After SHA-256: `801f7f5e0a54ad49b0bef51240d4b796b22cbfd38d99f1dad8bd5f8aec1912a9`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w02-c02-t1/Program.cs`
- Summary: bytes=5401
- After SHA-256: `8120a5a3f9c6ea688e1280a674c92394a2a7f27075500133f6a14f5975ea54cf`

## 64. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w02-c02-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `samples/m5-exact-vertical/m5-exact-vertical.csproj`
- Summary: bytes=533
- Before SHA-256: `f4160b8e1fd5c0849ff1073e1e9e710ef14338318394c54c329886f9321f89ab`
- After SHA-256: `7b746681955e28f2ef31d4a1707e5c6e84bc15fe28980c12955f6e0ad44874ca`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m5-package-readme.md`
- Summary: bytes=916
- After SHA-256: `b3005e5d4242f1630783e6b077d7b49e16b71cf1093e3b2a1c4a47abbfb9bb62`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m5-package-notices.md`
- Summary: bytes=717
- After SHA-256: `c157b9b621b1b452b89edab322e7d1094c5df890180de1e729da8a3149a52e6e`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m5-w02-c02-t1-clean-packages.md`
- Summary: bytes=3082
- After SHA-256: `423708cee899b70be86219e1588f0699a0cf79e68c9c15d0c6a7c74758c94b02`

## 69. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m5-local-support-evidence.md`
- Summary: bytes=1291
- After SHA-256: `0307bc486c6d9c51def3df82e66036993be4f390db5584bf99158677f8fb0359`

## 70. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W02-C02-T1-contract-qualified.json`
- Summary: bytes=8031
- After SHA-256: `a8bae67f171b5adca23526be179c5b9ee3a04817bde44c3ff3825f93715b1f4f`

## 71. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `767776a58805563559cd866eab8d862497b8b10a57f9604580de24608e6db0f2`
- After SHA-256: `8a6bd1e7dc343e70e0e19065ce8e829ccfed82b893271fea5cb8d0383d14201d`

## 72. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02 not-ready->ready
- Before SHA-256: `f54aba6d102a6768e7f928fc88fe6b53031f1b564b110d002b32ff1b68be1d08`
- After SHA-256: `03e18cd2afafab91954e69d1976ef8ac95f34b034da0db02beb90092e00c5e7e`

## 73. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1 not-ready->ready
- Before SHA-256: `03e18cd2afafab91954e69d1976ef8ac95f34b034da0db02beb90092e00c5e7e`
- After SHA-256: `24393a47c75c54419af2349dbfd6046f8ddcbe2b175b07e0e57e0fab9d3737c1`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-R not-ready->ready
- Before SHA-256: `24393a47c75c54419af2349dbfd6046f8ddcbe2b175b07e0e57e0fab9d3737c1`
- After SHA-256: `ea9961bab9bcfc0cd1f5ee6159339e730b251ce9f441b556a7d8472ba751d9b2`

## 75. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M5-W02-C02-T1-R activated=M5-W02-C02,M5-W02-C02-T1,M5-W02-C02-T1-R
- Before SHA-256: `ea9961bab9bcfc0cd1f5ee6159339e730b251ce9f441b556a7d8472ba751d9b2`
- After SHA-256: `b837334e6d465a0edb7489ca4ea904cc4ce38831e13d4e6202bf04440f419db8`

## 76. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-R in-progress->implemented->validated->done
- Before SHA-256: `b837334e6d465a0edb7489ca4ea904cc4ce38831e13d4e6202bf04440f419db8`
- After SHA-256: `3996ad6d1e03b0ee719d870a5b7ff6b8952fef8f2b2c924a6bbea8e6181e9228`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-G not-ready->ready
- Before SHA-256: `3996ad6d1e03b0ee719d870a5b7ff6b8952fef8f2b2c924a6bbea8e6181e9228`
- After SHA-256: `e0a11e2bb2ea2c9cac308a539be84f21413aa317f9ea9d90232d72eb6ab59572`

## 78. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-G ready->in-progress
- Before SHA-256: `e0a11e2bb2ea2c9cac308a539be84f21413aa317f9ea9d90232d72eb6ab59572`
- After SHA-256: `48f161e09cace02ce64d23a065e8bf3c912d21e9232e5450dbaf0002925cfcbc`

## 79. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-G in-progress->implemented->validated->done
- Before SHA-256: `48f161e09cace02ce64d23a065e8bf3c912d21e9232e5450dbaf0002925cfcbc`
- After SHA-256: `253a563d9a741cfc3a9dd641d13c4b8a178a721fdc34c22331fc33a5cbce4404`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-V not-ready->ready
- Before SHA-256: `253a563d9a741cfc3a9dd641d13c4b8a178a721fdc34c22331fc33a5cbce4404`
- After SHA-256: `555b99207af91817c496f3765506f585cc2577ee7e883be8a9ccfc75ffaeb5d2`

## 81. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-V ready->in-progress
- Before SHA-256: `555b99207af91817c496f3765506f585cc2577ee7e883be8a9ccfc75ffaeb5d2`
- After SHA-256: `1b0fcde485b782bc444fc497fc7bd05d734362681ad0458324ca19beda862a2a`

## 82. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-V in-progress->implemented->validated->done
- Before SHA-256: `1b0fcde485b782bc444fc497fc7bd05d734362681ad0458324ca19beda862a2a`
- After SHA-256: `fce57fbcc4f59c21c99c1bcaac14bb86e9c4416f1d287783301bc78138a10572`

## 83. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1 in-progress->implemented->validated->done
- Before SHA-256: `fce57fbcc4f59c21c99c1bcaac14bb86e9c4416f1d287783301bc78138a10572`
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 84. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 85. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=in-progress
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C01 state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02 state=in-progress
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1 state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-R state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-G state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T1-V state=done
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C02-T2 state=not-ready
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 95. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02-C03 state=not-ready
- After SHA-256: `440dee5421fa6c4049d492f0021e4f051b8e19708075b164603f2a9a3616604b`

## 96. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `58a424efa2b28283c9c069c77b20a7b8b915d63bcf8b2095104b575187f54ca7`
- After SHA-256: `0a64d3d7b5efea736f4c1d48efc41dbaa8d318f0582c3e9779f6797f1dcd1573`

