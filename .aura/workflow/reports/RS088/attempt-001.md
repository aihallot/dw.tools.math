# RS088 Attempt 001 Payload Operations

- Operations: 101
- Changed: 34
- No change: 0
- Verified: 67
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS088 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS087/attempt-001.json` | run=RS087 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/prior_family_phases_completed |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/retained_targeted_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/independent_consumer_probes_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/source_free_consumer_count |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/m6_w01_work_package_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/m6_release_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/local_product_version |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/publicly_published |
| 12 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 13 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/40/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/41/status |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/42/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/tasks/0/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/tasks/1/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/tasks/0/subtasks/0/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/tasks/0/subtasks/1/status |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/tasks/0/subtasks/2/status |
| 29 | `require.text-contains` | Verified | `dw.tools.math.slnx` | semantic text found |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 31 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=not-ready |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=not-ready |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1 state=not-ready |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-R state=not-ready |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-G state=not-ready |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-V state=not-ready |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T2 state=not-ready |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=not-ready |
| 41 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 42 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | path is absent |
| 43 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/packages.lock.json` | path is absent |
| 44 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/ExactScientificContracts.cs` | path is absent |
| 45 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs` | path is absent |
| 46 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj` | path is absent |
| 47 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/packages.lock.json` | path is absent |
| 48 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1RedTests.cs` | path is absent |
| 49 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1Tests.cs` | path is absent |
| 50 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj` | path is absent |
| 51 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c01-t1/Program.cs` | path is absent |
| 52 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c01-t1/NuGet.Config` | path is absent |
| 53 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c01-t1-exact-integer-contract.md` | path is absent |
| 54 | `require.file-missing` | Verified | `docs/distribution/m6-exact-integer-science.md` | path is absent |
| 55 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C01-T1-contract-red.json` | path is absent |
| 56 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C01-T1-qualified.json` | path is absent |
| 57 | `files.replace-from-staged` | Changed | `dw.tools.math.slnx` | bytes=2257 |
| 58 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=628 |
| 59 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/packages.lock.json` | bytes=62 |
| 60 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/ExactScientificContracts.cs` | bytes=978 |
| 61 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj` | bytes=695 |
| 62 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/packages.lock.json` | bytes=3880 |
| 63 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1RedTests.cs` | bytes=867 |
| 64 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C01-T1-contract-red.json` | bytes=602 |
| 65 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs` | bytes=7093 |
| 66 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1Tests.cs` | bytes=10465 |
| 67 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj` | bytes=538 |
| 68 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c01-t1/Program.cs` | bytes=3930 |
| 69 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c01-t1/NuGet.Config` | bytes=113 |
| 70 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c01-t1-exact-integer-contract.md` | bytes=2574 |
| 71 | `files.replace-from-staged` | Changed | `docs/distribution/m6-exact-integer-science.md` | bytes=1341 |
| 72 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C01-T1-qualified.json` | bytes=5020 |
| 73 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 74 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02 not-ready->ready |
| 75 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01 not-ready->ready |
| 76 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1 not-ready->ready |
| 77 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-R not-ready->ready |
| 78 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02 ready->in-progress |
| 79 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01 ready->in-progress |
| 80 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1 ready->in-progress |
| 81 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-R ready->in-progress |
| 82 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-R in-progress->implemented->validated->done |
| 83 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-G not-ready->ready |
| 84 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-G ready->in-progress |
| 85 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-G in-progress->implemented->validated->done |
| 86 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-V not-ready->ready |
| 87 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-V ready->in-progress |
| 88 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-V in-progress->implemented->validated->done |
| 89 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1 in-progress->implemented->validated->done |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 92 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 93 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 94 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=in-progress |
| 95 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1 state=done |
| 96 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-R state=done |
| 97 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-G state=done |
| 98 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T1-V state=done |
| 99 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01-T2 state=not-ready |
| 100 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=not-ready |
| 101 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS088 hierarchy=exact
- After SHA-256: `05d6462720183cf691ffc28dec8db4e67ad5de8d8331cf146bfb4918b516d0c9`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS087/attempt-001.json`
- Summary: run=RS087 attempt=1 lineage=exact
- After SHA-256: `e3c4eb8e12d892bb9591c2f7ad8fda0b3318a87a59007abbd9fc29a9fbb2cc65`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/prior_family_phases_completed

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/retained_targeted_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/independent_consumer_probes_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/source_free_consumer_count

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/m6_release_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/local_product_version

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/publicly_published

## 12. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

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
- Summary: pointer=/chunks/40/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/41/status

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/42/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/tasks/0/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/tasks/1/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/tasks/0/subtasks/0/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/tasks/0/subtasks/1/status

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/tasks/0/subtasks/2/status

## 29. `require.text-contains`

- Status: `Verified`
- Path: `dw.tools.math.slnx`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 31. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-R state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-G state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-V state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T2 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 41. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`

## 42. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: path is absent

## 43. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/packages.lock.json`
- Summary: path is absent

## 44. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/ExactScientificContracts.cs`
- Summary: path is absent

## 45. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs`
- Summary: path is absent

## 46. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj`
- Summary: path is absent

## 47. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/packages.lock.json`
- Summary: path is absent

## 48. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1RedTests.cs`
- Summary: path is absent

## 49. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1Tests.cs`
- Summary: path is absent

## 50. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj`
- Summary: path is absent

## 51. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c01-t1/Program.cs`
- Summary: path is absent

## 52. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c01-t1/NuGet.Config`
- Summary: path is absent

## 53. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c01-t1-exact-integer-contract.md`
- Summary: path is absent

## 54. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-exact-integer-science.md`
- Summary: path is absent

## 55. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T1-contract-red.json`
- Summary: path is absent

## 56. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T1-qualified.json`
- Summary: path is absent

## 57. `files.replace-from-staged`

- Status: `Changed`
- Path: `dw.tools.math.slnx`
- Summary: bytes=2257
- Before SHA-256: `30b6641dae58723c6d4c7d1e17bc321e35c0afa3204cb652eaa68f7cec23f460`
- After SHA-256: `818a6e83c6a5de599887dd252bb90ebdcc294dc7d17d66450b8cd9c4a1b272b7`

## 58. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=628
- After SHA-256: `87538cd02105d612631afb38cdadaa26c7233122ed8ce393601922c3853dd251`

## 59. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/packages.lock.json`
- Summary: bytes=62
- After SHA-256: `1624731bf151ec2ac6993023046a4abbac7c6ddc71ed2e242e8285bbc69d561e`

## 60. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/ExactScientificContracts.cs`
- Summary: bytes=978
- After SHA-256: `acdf3e354623209771496d55f6c602685f6b68de552b642c860867e7664f8ea6`

## 61. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj`
- Summary: bytes=695
- After SHA-256: `2fd2d837ccb4b28796576692f09dd8f62b0465187a8a0aa564e65d87f69a2d84`

## 62. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/packages.lock.json`
- Summary: bytes=3880
- After SHA-256: `ce5e7d4a11549b8fb39562773ddec282c218b2766661e7301b18eebcb3216ff6`

## 63. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1RedTests.cs`
- Summary: bytes=867
- After SHA-256: `925f48a5f42817d44bff76de801e88dca91d1df18025d425495dd2d434c399a1`

## 64. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C01-T1-contract-red.json`
- Summary: bytes=602
- After SHA-256: `67bc5df862d44251a591dadc683031b74ab373ec2c4673c0f60c27be8c16e2af`

## 65. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/BoundedExactScientific.cs`
- Summary: bytes=7093
- After SHA-256: `8161f07a760066e345db2c2f2297a9f93045443c65a5facb613e0e609f620e45`

## 66. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C01T1Tests.cs`
- Summary: bytes=10465
- After SHA-256: `46a45ecca8aa1e4464d6ef3c5059eb8ba24f84d121a9de2982041a7a10190eec`

## 67. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c01-t1/m6-exact-science-consumer.csproj`
- Summary: bytes=538
- After SHA-256: `a697a12289c769ef48a07c10172152721563e5e3492456fdc871a9ba151c0830`

## 68. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c01-t1/Program.cs`
- Summary: bytes=3930
- After SHA-256: `f1301a40851c72377b19c9c47cd718101371dea4c9ac7f76a69ba1700bce2d9d`

## 69. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c01-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 70. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c01-t1-exact-integer-contract.md`
- Summary: bytes=2574
- After SHA-256: `77bfb7dd4b9e43a41b0858c7321fa1b7a0060ca603ea8bd42a56c3a851729db4`

## 71. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-exact-integer-science.md`
- Summary: bytes=1341
- After SHA-256: `517788f1b777e69721e642abd66af354e56ef8fc5c97b70911b18ade16a7cdf3`

## 72. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C01-T1-qualified.json`
- Summary: bytes=5020
- After SHA-256: `b23b05325f75adee15580c512c3d76db741bcc87bde0b8bfd5a70ef544c0697b`

## 73. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `67af7f6cc8d1e185359ef3590ca5db4e732b94523a6c09bed235bd2f26e158c6`
- After SHA-256: `f616cc0aa2fbae9172f284a944e642d4765a259f5939164348952a10b61fef85`

## 74. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 not-ready->ready
- Before SHA-256: `e5e00ae25f18b26ca3cfc638cb67d6dd6f723f9be98bbbb6163831662ea5b4f2`
- After SHA-256: `bbc318eb0bde1bda38caf2fb362f55dd28e4dfb5a6ddef54ab28020052216617`

## 75. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 not-ready->ready
- Before SHA-256: `bbc318eb0bde1bda38caf2fb362f55dd28e4dfb5a6ddef54ab28020052216617`
- After SHA-256: `aaf2bf7113bcb00518e1216ebd562e245a4f5daeda061105e7b657a0af50d126`

## 76. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1 not-ready->ready
- Before SHA-256: `aaf2bf7113bcb00518e1216ebd562e245a4f5daeda061105e7b657a0af50d126`
- After SHA-256: `ba25fc30d9307e3e762d714b2a5896449558599bc2d0d3de94638c094cff48ea`

## 77. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-R not-ready->ready
- Before SHA-256: `ba25fc30d9307e3e762d714b2a5896449558599bc2d0d3de94638c094cff48ea`
- After SHA-256: `85c2d3cd67b296d9c0f52ab32797bb4570e7c9a60f6fc978820d7f17935c8c67`

## 78. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 ready->in-progress
- Before SHA-256: `85c2d3cd67b296d9c0f52ab32797bb4570e7c9a60f6fc978820d7f17935c8c67`
- After SHA-256: `856ef5d8e770c4f60883eaf2d445f116be7abeefb73ad79e788d4f73234ad4c4`

## 79. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 ready->in-progress
- Before SHA-256: `856ef5d8e770c4f60883eaf2d445f116be7abeefb73ad79e788d4f73234ad4c4`
- After SHA-256: `cda08cb447f63df0ec6a5c442a6f2aacad7cf61b6b43967db0a3917e83003bbd`

## 80. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1 ready->in-progress
- Before SHA-256: `cda08cb447f63df0ec6a5c442a6f2aacad7cf61b6b43967db0a3917e83003bbd`
- After SHA-256: `03992423f698a1593d40483e655327efda473d87a700229911bb869653213a19`

## 81. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-R ready->in-progress
- Before SHA-256: `03992423f698a1593d40483e655327efda473d87a700229911bb869653213a19`
- After SHA-256: `c89e72d6e52fbc81205921b14713e0cf207761e76e28899019d02120bdd44185`

## 82. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-R in-progress->implemented->validated->done
- Before SHA-256: `c89e72d6e52fbc81205921b14713e0cf207761e76e28899019d02120bdd44185`
- After SHA-256: `31a369fdf8269a57fa4bde190683dd068e733725e77447c28fbcba842b2a7ed1`

## 83. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-G not-ready->ready
- Before SHA-256: `31a369fdf8269a57fa4bde190683dd068e733725e77447c28fbcba842b2a7ed1`
- After SHA-256: `bf3becefe2d44e927497c8c02840cc95170722295c207092efa788f57f691193`

## 84. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-G ready->in-progress
- Before SHA-256: `bf3becefe2d44e927497c8c02840cc95170722295c207092efa788f57f691193`
- After SHA-256: `0c76befcff72701aa52f76a1b1bc6635f1554c16d5d93099a6fac268de2ee5b5`

## 85. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-G in-progress->implemented->validated->done
- Before SHA-256: `0c76befcff72701aa52f76a1b1bc6635f1554c16d5d93099a6fac268de2ee5b5`
- After SHA-256: `fac4f6c95afb64fcca0bba2a5af6539faff6ffd2a9d24cb24dc20046f765b164`

## 86. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-V not-ready->ready
- Before SHA-256: `fac4f6c95afb64fcca0bba2a5af6539faff6ffd2a9d24cb24dc20046f765b164`
- After SHA-256: `904a650da1ccb31764bfcfeb7915cdf7d56943157561197047f4237cf9601f19`

## 87. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-V ready->in-progress
- Before SHA-256: `904a650da1ccb31764bfcfeb7915cdf7d56943157561197047f4237cf9601f19`
- After SHA-256: `b7d226853fd340429fae01cbeb7b48ae72f820bfd26dbe2caec9af8fc5bc40db`

## 88. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-V in-progress->implemented->validated->done
- Before SHA-256: `b7d226853fd340429fae01cbeb7b48ae72f820bfd26dbe2caec9af8fc5bc40db`
- After SHA-256: `9b63ab498dcde4da65432710f44794314ff410646deb2c8ac8bf5d1d2233c9a5`

## 89. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1 in-progress->implemented->validated->done
- Before SHA-256: `9b63ab498dcde4da65432710f44794314ff410646deb2c8ac8bf5d1d2233c9a5`
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 92. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 93. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 94. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=in-progress
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 95. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1 state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 96. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-R state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 97. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-G state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 98. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T1-V state=done
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 99. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01-T2 state=not-ready
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 100. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=not-ready
- After SHA-256: `89c608bc7b5c80f4402a42ec0a88bde530073b795c0c826164058efefe4da5b4`

## 101. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `299fa64ddc0da8f7a60693c5a183dc89f3c06047acfab8f0b37391bfad1bedf2`
- After SHA-256: `7ff3f5323e75b683dd3d9991f478a4853bd35e1ae3f86582fcba2f99b64c0780`

