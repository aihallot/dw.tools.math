# RS070 Attempt 001 Payload Operations

- Operations: 90
- Changed: 25
- No change: 0
- Verified: 65
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS070 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS069/attempt-001.json` | run=RS069 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/isolated_consumer_probes_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/compiled_red_observed |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/qualified_audit_rule |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/m5_w01_c02_t2_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/m5_w01_c02_phase_done |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/m5_w01_work_package_done |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json` | pointer=/m5_release_done |
| 14 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/4/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/11/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/12/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/33/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/34/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/tasks/0/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/tasks/0/subtasks/0/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/tasks/0/subtasks/1/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/tasks/0/subtasks/2/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/35/tasks/1/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/36/status |
| 29 | `require.text-contains` | Verified | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | semantic text found |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 31 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=in-progress |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C01 state=done |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C02 state=done |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03 state=not-ready |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1 state=not-ready |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T2 state=not-ready |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C04 state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-R state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-G state=not-ready |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-V state=not-ready |
| 43 | `require.file-missing` | Verified | `src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs` | path is absent |
| 44 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1RedTests.cs` | path is absent |
| 45 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1Tests.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `docs/planning/decisions/m5-w01-c03-t1-exact-mathml.md` | path is absent |
| 47 | `require.file-missing` | Verified | `docs/distribution/m5-exact-mathml.md` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/consumers/m5-w01-c03-t1/m5-mathml-consumer.csproj` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/consumers/m5-w01-c03-t1/Program.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m5-w01-c03-t1/NuGet.Config` | path is absent |
| 51 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W01-C03-T1-contract-red.json` | path is absent |
| 52 | `require.file-missing` | Verified | `docs/planning/evidence/M5-W01-C03-T1-contract-qualified.json` | path is absent |
| 53 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1RedTests.cs` | bytes=930 |
| 54 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W01-C03-T1-contract-red.json` | bytes=864 |
| 55 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj` | bytes=636 |
| 56 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs` | bytes=12070 |
| 57 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1Tests.cs` | bytes=15808 |
| 58 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m5-w01-c03-t1-exact-mathml.md` | bytes=4729 |
| 59 | `files.replace-from-staged` | Changed | `docs/distribution/m5-exact-mathml.md` | bytes=1844 |
| 60 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w01-c03-t1/m5-mathml-consumer.csproj` | bytes=522 |
| 61 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w01-c03-t1/Program.cs` | bytes=5496 |
| 62 | `files.replace-from-staged` | Changed | `tests/consumers/m5-w01-c03-t1/NuGet.Config` | bytes=117 |
| 63 | `files.write-complete` | Changed | `docs/planning/evidence/M5-W01-C03-T1-contract-qualified.json` | bytes=6199 |
| 64 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 65 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03 not-ready->ready |
| 66 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1 not-ready->ready |
| 67 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-R not-ready->ready |
| 68 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M5-W01-C03-T1-R activated=M5-W01-C03,M5-W01-C03-T1,M5-W01-C03-T1-R |
| 69 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-R in-progress->implemented->validated->done |
| 70 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-G not-ready->ready |
| 71 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-G ready->in-progress |
| 72 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-G in-progress->implemented->validated->done |
| 73 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-V not-ready->ready |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-V ready->in-progress |
| 75 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-V in-progress->implemented->validated->done |
| 76 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1 in-progress->implemented->validated->done |
| 77 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M4 state=done |
| 78 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=in-progress |
| 79 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01 state=in-progress |
| 80 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C01 state=done |
| 81 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C02 state=done |
| 82 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03 state=in-progress |
| 83 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1 state=done |
| 84 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T2 state=not-ready |
| 85 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C04 state=not-ready |
| 86 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W02 state=not-ready |
| 87 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-R state=done |
| 88 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-G state=done |
| 89 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5-W01-C03-T1-V state=done |
| 90 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS070 hierarchy=exact
- After SHA-256: `81248a7f1eba271f132e3faf793c33de581a21e823c55d69a3a0a3335e996fab`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS069/attempt-001.json`
- Summary: run=RS069 attempt=1 lineage=exact
- After SHA-256: `106ebecb424ecab995ca5dba28d0e70e8f47670cb1c73beaf8df90e71ad7fda2`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/isolated_consumer_probes_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/compiled_red_observed

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/qualified_audit_rule

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w01_c02_t2_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w01_c02_phase_done

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m5_w01_work_package_done

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m5_release_done

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
- Summary: pointer=/releases/4/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/11/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/12/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/33/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/34/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/tasks/0/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/tasks/0/subtasks/0/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/tasks/0/subtasks/1/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/tasks/0/subtasks/2/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/35/tasks/1/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/36/status

## 29. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 31. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=in-progress
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C01 state=done
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C02 state=done
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03 state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1 state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T2 state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C04 state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-R state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-G state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-V state=not-ready
- After SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`

## 43. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs`
- Summary: path is absent

## 44. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1RedTests.cs`
- Summary: path is absent

## 45. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1Tests.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m5-w01-c03-t1-exact-mathml.md`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m5-exact-mathml.md`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w01-c03-t1/m5-mathml-consumer.csproj`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w01-c03-t1/Program.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m5-w01-c03-t1/NuGet.Config`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C03-T1-contract-red.json`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M5-W01-C03-T1-contract-qualified.json`
- Summary: path is absent

## 53. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1RedTests.cs`
- Summary: bytes=930
- After SHA-256: `d7826bf54a3346b68806ca249b455fd295fb02166c95125c154b279fa69a0a7e`

## 54. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W01-C03-T1-contract-red.json`
- Summary: bytes=864
- After SHA-256: `50c6077964f1fc92877383e4c2631277b21a8cda830e0da865e4850cd9f00430`

## 55. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj`
- Summary: bytes=636
- Before SHA-256: `b262f46b0e2fa402d558ba23fde9117176886aadadbea24e1d9cf5c94256ccfc`
- After SHA-256: `bb0b7d58d89c5e25162deb24076ff79fcce1ad4ff32f9e7ee595b4cc6f3b02ec`

## 56. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.codecs/ExactMathMlInterop.cs`
- Summary: bytes=12070
- After SHA-256: `ea7bb23e56226918251cde00b14a7f004d9c438171c127dcf512b6795516e92f`

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.codecs.tests/M5W01C03T1Tests.cs`
- Summary: bytes=15808
- After SHA-256: `a5fd05a3b713db70ec59fb1f43bbf3afdb2dc2df3dc11854c657ad17429a4783`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m5-w01-c03-t1-exact-mathml.md`
- Summary: bytes=4729
- After SHA-256: `55be68aa2c54f70ee2ba29f02695c7afcd1fcc90f3b41b8707930eb1ffdcd89c`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m5-exact-mathml.md`
- Summary: bytes=1844
- After SHA-256: `e38d9966cc3f292aff4bd337213f991408f615e050fc5a50906dfcb8dfe7694f`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w01-c03-t1/m5-mathml-consumer.csproj`
- Summary: bytes=522
- After SHA-256: `2eed950378f61f4f419f250e383f7ebaa0870729b86950e51f0a9f8aaf2cf10c`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w01-c03-t1/Program.cs`
- Summary: bytes=5496
- After SHA-256: `aee6802d77ed9d1d7862e95096bf3562d3ebcc633f252cfb44f54df1eeb6872f`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m5-w01-c03-t1/NuGet.Config`
- Summary: bytes=117
- After SHA-256: `964302159308872b9729e1828a1ba897f30634f7bd751cde11ea8e6fed8cfb97`

## 63. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M5-W01-C03-T1-contract-qualified.json`
- Summary: bytes=6199
- After SHA-256: `ddb5131aca8d7c8f349d825fbeb6a7161b6e3bef17a100ff5073055097431a5d`

## 64. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `39326c9576a17ffb1e4e5e9474b79946d08b8304ccec70e4c6fdd9c070267ddd`
- After SHA-256: `1cdd36295a11ed524337b43b6ed60541d58ba272d4ea8b820403550a5a29ea30`

## 65. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03 not-ready->ready
- Before SHA-256: `1ef6a846c67922df9e92f9115fa71d549b6032361cfa454c177bf86bc1eae220`
- After SHA-256: `3be0d88acd460778275250455e000f9c68dfb8b07b22262abaf6eb75ac1c4977`

## 66. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1 not-ready->ready
- Before SHA-256: `3be0d88acd460778275250455e000f9c68dfb8b07b22262abaf6eb75ac1c4977`
- After SHA-256: `8e1e1cd6045417b937ad92205b595fa84707621fb0b70e9898299e885983e89b`

## 67. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-R not-ready->ready
- Before SHA-256: `8e1e1cd6045417b937ad92205b595fa84707621fb0b70e9898299e885983e89b`
- After SHA-256: `fad991de749097af50ad3050fa1f3f1eed32edafba254fd0827652865f26fd65`

## 68. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M5-W01-C03-T1-R activated=M5-W01-C03,M5-W01-C03-T1,M5-W01-C03-T1-R
- Before SHA-256: `fad991de749097af50ad3050fa1f3f1eed32edafba254fd0827652865f26fd65`
- After SHA-256: `a3ca63499f12343e5f7f3a1fa6e647086eb4d43ef67571719094c66485b6592d`

## 69. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-R in-progress->implemented->validated->done
- Before SHA-256: `a3ca63499f12343e5f7f3a1fa6e647086eb4d43ef67571719094c66485b6592d`
- After SHA-256: `41d3bfe6456c83ac696a54ff76749d6a482a67bc0c800a0016bd6ac0e71adf14`

## 70. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-G not-ready->ready
- Before SHA-256: `41d3bfe6456c83ac696a54ff76749d6a482a67bc0c800a0016bd6ac0e71adf14`
- After SHA-256: `7056925daa70b8ab8feafefa13d60978a1c00df8d8f76068c7bf2ac93243bc56`

## 71. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-G ready->in-progress
- Before SHA-256: `7056925daa70b8ab8feafefa13d60978a1c00df8d8f76068c7bf2ac93243bc56`
- After SHA-256: `a8d23282a5e9e34070acf4d5f5743dd4320cc0b983a0ba568b20fe9e8416176d`

## 72. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-G in-progress->implemented->validated->done
- Before SHA-256: `a8d23282a5e9e34070acf4d5f5743dd4320cc0b983a0ba568b20fe9e8416176d`
- After SHA-256: `a044070720461d8b2a06a5eb7cf992b9bb490b3db0a1e99ff681ef612d7dcd9a`

## 73. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-V not-ready->ready
- Before SHA-256: `a044070720461d8b2a06a5eb7cf992b9bb490b3db0a1e99ff681ef612d7dcd9a`
- After SHA-256: `1f044e97298065da0f55cf3cec268e0b488c53cc0c97d8b6cb34ebc728edfaff`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-V ready->in-progress
- Before SHA-256: `1f044e97298065da0f55cf3cec268e0b488c53cc0c97d8b6cb34ebc728edfaff`
- After SHA-256: `26a94eab182aaf3f778325e31a1373c238373980227af4d69b0eb650c74761ba`

## 75. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-V in-progress->implemented->validated->done
- Before SHA-256: `26a94eab182aaf3f778325e31a1373c238373980227af4d69b0eb650c74761ba`
- After SHA-256: `24cb96faca2da9837df330aa03af88741af41e5e3aa827c73e8e62eacf21230b`

## 76. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1 in-progress->implemented->validated->done
- Before SHA-256: `24cb96faca2da9837df330aa03af88741af41e5e3aa827c73e8e62eacf21230b`
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 77. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M4 state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 78. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=in-progress
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 79. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01 state=in-progress
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 80. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C01 state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 81. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C02 state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 82. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03 state=in-progress
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 83. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1 state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 84. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T2 state=not-ready
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 85. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C04 state=not-ready
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 86. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W02 state=not-ready
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 87. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-R state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 88. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-G state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 89. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5-W01-C03-T1-V state=done
- After SHA-256: `94697b8c45ad005a1f89062f63d908c6a0c79de84b93ec51f6a681c6e0950bbf`

## 90. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `a68a419df1023492708ad6f3b5ae7f0845a7425badad975375ac4c69249c56bc`
- After SHA-256: `0f269601039dce9a4b3025d38bb266e6ca9d892536da1501e15a0db936bf2860`

