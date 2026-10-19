# RS101 Attempt 001 Payload Operations

- Operations: 116
- Changed: 37
- No change: 0
- Verified: 79
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS101 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS100/attempt-001.json` | run=RS100 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/independent_green_tests_passed |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/red_recovered_and_green_tests_passed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_t1_done |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_t2_done |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/m6_w02_c03_parent_done |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/m6_w02_work_package_done |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/publicly_published |
| 12 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 13 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 16 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 17 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 18 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 19 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 20 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/id |
| 21 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 22 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/status |
| 24 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 25 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/id |
| 26 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/status |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/0/id |
| 28 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/0/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/1/status |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/id |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/0/subtasks/0/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/0/subtasks/1/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/tasks/0/subtasks/2/status |
| 35 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 36 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-t2/m6-scientific-boundary-consumer.csproj` | semantic text found |
| 37 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-t1/m6-scientific-families-consumer.csproj` | semantic text found |
| 38 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj` | semantic text found |
| 39 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | semantic text found |
| 40 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | semantic text found |
| 41 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-t2/Program.cs` | semantic text found |
| 42 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-t1/Program.cs` | semantic text found |
| 43 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s03-t2/Program.cs` | semantic text found |
| 44 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | semantic text found |
| 45 | `require.text-contains` | Verified | `tests/consumers/m6-w02-c02-t2/Program.cs` | semantic text found |
| 46 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 47 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 49 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 50 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 52 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 53 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=done |
| 54 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=not-ready |
| 55 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1 state=not-ready |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-R state=not-ready |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-G state=not-ready |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-V state=not-ready |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T2 state=not-ready |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 61 | `require.file-missing` | Verified | `src/projects/dw.tools.math.scientific/M6W02C04Contract.cs` | path is absent |
| 62 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1RedTests.cs` | path is absent |
| 63 | `require.file-missing` | Verified | `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1Tests.cs` | path is absent |
| 64 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c04-t1/m6-effective-finance-consumer.csproj` | path is absent |
| 65 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c04-t1/Program.cs` | path is absent |
| 66 | `require.file-missing` | Verified | `tests/consumers/m6-w02-c04-t1/NuGet.Config` | path is absent |
| 67 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c04-t1-financial-rate-contracts.md` | path is absent |
| 68 | `require.file-missing` | Verified | `docs/distribution/m6-pure-finance-t1.md` | path is absent |
| 69 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C04-T1-contract-red.json` | path is absent |
| 70 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C04-T1-qualified.json` | path is absent |
| 71 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1RedTests.cs` | bytes=677 |
| 72 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C04-T1-contract-red.json` | bytes=638 |
| 73 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | bytes=635 |
| 74 | `files.replace-from-staged` | Changed | `src/projects/dw.tools.math.scientific/M6W02C04Contract.cs` | bytes=8393 |
| 75 | `files.replace-from-staged` | Changed | `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1Tests.cs` | bytes=11047 |
| 76 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-t2/m6-scientific-boundary-consumer.csproj` | bytes=541 |
| 77 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-t2/Program.cs` | bytes=10734 |
| 78 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-t1/m6-scientific-families-consumer.csproj` | bytes=539 |
| 79 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-t1/Program.cs` | bytes=8178 |
| 80 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj` | bytes=544 |
| 81 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s03-t2/Program.cs` | bytes=7217 |
| 82 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj` | bytes=543 |
| 83 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c03-s02-t2/Program.cs` | bytes=7660 |
| 84 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj` | bytes=546 |
| 85 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c02-t2/Program.cs` | bytes=6092 |
| 86 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c04-t1/m6-effective-finance-consumer.csproj` | bytes=537 |
| 87 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c04-t1/Program.cs` | bytes=6220 |
| 88 | `files.replace-from-staged` | Changed | `tests/consumers/m6-w02-c04-t1/NuGet.Config` | bytes=113 |
| 89 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c04-t1-financial-rate-contracts.md` | bytes=4958 |
| 90 | `files.replace-from-staged` | Changed | `docs/distribution/m6-pure-finance-t1.md` | bytes=1962 |
| 91 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C04-T1-qualified.json` | bytes=9875 |
| 92 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 93 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04 not-ready->ready |
| 94 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1 not-ready->ready |
| 95 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-R not-ready->ready |
| 96 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04 ready->in-progress |
| 97 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1 ready->in-progress |
| 98 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-R ready->in-progress |
| 99 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-R in-progress->implemented->validated->done |
| 100 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-G not-ready->ready |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-G ready->in-progress |
| 102 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-G in-progress->implemented->validated->done |
| 103 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-V not-ready->ready |
| 104 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-V ready->in-progress |
| 105 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-V in-progress->implemented->validated->done |
| 106 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1 in-progress->implemented->validated->done |
| 107 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=in-progress |
| 108 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1 state=done |
| 109 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-R state=done |
| 110 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-G state=done |
| 111 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T1-V state=done |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04-T2 state=not-ready |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=not-ready |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 115 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 116 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS101 hierarchy=exact
- After SHA-256: `250171a5c5b5343d1ff9ec0856718e28dd641d28c279267b4793d8d8594f559e`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS100/attempt-001.json`
- Summary: run=RS100 attempt=1 lineage=exact
- After SHA-256: `390451316b9ed2799608d384fd371c1479b1f1dcf2cbb0335ccc624d29971c7f`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/independent_green_tests_passed

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/red_recovered_and_green_tests_passed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_t1_done

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_t2_done

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_c03_parent_done

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_w02_work_package_done

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
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
- Summary: pointer=/work_packages/13/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/id

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/status

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/status

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/id

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/status

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/0/id

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/0/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/1/status

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/id

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/0/subtasks/0/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/0/subtasks/1/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/tasks/0/subtasks/2/status

## 35. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-t2/m6-scientific-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-t1/m6-scientific-families-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-t1/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 43. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s03-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 44. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 45. `require.text-contains`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 46. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 47. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 49. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 50. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 52. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 53. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=done
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 54. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 55. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1 state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-R state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-G state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-V state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T2 state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`

## 61. `require.file-missing`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/M6W02C04Contract.cs`
- Summary: path is absent

## 62. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1RedTests.cs`
- Summary: path is absent

## 63. `require.file-missing`

- Status: `Verified`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1Tests.cs`
- Summary: path is absent

## 64. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c04-t1/m6-effective-finance-consumer.csproj`
- Summary: path is absent

## 65. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c04-t1/Program.cs`
- Summary: path is absent

## 66. `require.file-missing`

- Status: `Verified`
- Path: `tests/consumers/m6-w02-c04-t1/NuGet.Config`
- Summary: path is absent

## 67. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c04-t1-financial-rate-contracts.md`
- Summary: path is absent

## 68. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-pure-finance-t1.md`
- Summary: path is absent

## 69. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T1-contract-red.json`
- Summary: path is absent

## 70. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T1-qualified.json`
- Summary: path is absent

## 71. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1RedTests.cs`
- Summary: bytes=677
- After SHA-256: `f0bb837654e38be48aee9eba504212098f48e4cd33c68550d6f3620e4b5fc0c4`

## 72. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C04-T1-contract-red.json`
- Summary: bytes=638
- After SHA-256: `eed98152f2462934c6498fa69f577128db99643b0777166927e5dba04f2cde28`

## 73. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: bytes=635
- Before SHA-256: `5898838ea2252f1cd9326891c26422ae9a8e84046eeefe9a1867280caedf27bb`
- After SHA-256: `1f5ed19cf5389204cca9a8758f7a5fad8c5bbc54d4f3b2659798c231e5c6266f`

## 74. `files.replace-from-staged`

- Status: `Changed`
- Path: `src/projects/dw.tools.math.scientific/M6W02C04Contract.cs`
- Summary: bytes=8393
- After SHA-256: `31e1c8fff54c129313f6409df7ca0790c507a61f3dba384a0bb8a182e8ee5ec2`

## 75. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/projects/dw.tools.math.scientific.tests/M6W02C04T1Tests.cs`
- Summary: bytes=11047
- After SHA-256: `f73e35e8dcafbb1194a46df114e934c5393880cc15770d0d3488b8231cf5280e`

## 76. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-t2/m6-scientific-boundary-consumer.csproj`
- Summary: bytes=541
- Before SHA-256: `dd49980455dd5899c96ed6de98e98b2f1239bae212ccdc146efaede29fa1097b`
- After SHA-256: `e15fd9f17b2c04c895a3a60bd62eb4895148eff6108ea3ebc499890a6eee3528`

## 77. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-t2/Program.cs`
- Summary: bytes=10734
- Before SHA-256: `fbc22fb7211e088a9a7382c75400013624c0395642a722f118c0ac59d2cfc3b5`
- After SHA-256: `93bd6f3dd615b4df5eac648d187a6b2e9f4410298b3080c712b5b94d00975612`

## 78. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-t1/m6-scientific-families-consumer.csproj`
- Summary: bytes=539
- Before SHA-256: `45626d488c908c0e270301a265c0f02a787cec5b2168167e05770311f92e74aa`
- After SHA-256: `6b24586237dce406d94d960cf7955b6f37f828424d0418545b369ddce5c8dd08`

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-t1/Program.cs`
- Summary: bytes=8178
- Before SHA-256: `2d680e0ab0241494881c3ed4e8104352d6aa7fbad6ef96acab0b1767999e4cdf`
- After SHA-256: `3cda2f29f757941176086dc7a2563c20fdd3135cbdc04f77263a02c0fc5ffef5`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t2/m6-uniform-series-boundary-consumer.csproj`
- Summary: bytes=544
- Before SHA-256: `19c50ddf5e7675276c4538fc7ca1b40a2dfe9bb33026ffd80d0772b4491330c5`
- After SHA-256: `d2f0bb40826a9a81973ebbfd4864bb4ce2c66d11aaba9f5a0430c4b9e38d34cf`

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s03-t2/Program.cs`
- Summary: bytes=7217
- Before SHA-256: `75b3d08128def9c9788267d99985a3456a5c1be8022a4f4dc026d4ba78e14dfc`
- After SHA-256: `3d7d5f80924e2de35660d0ae9cafc3826cfde8c59880b66697953d0891d35dd8`

## 82. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/m6-direct-signal-boundary-consumer.csproj`
- Summary: bytes=543
- Before SHA-256: `66e6ff8f08e50acc0a2c575e4f63d5d75c2a00080ba95e2ac0152e5d3f0e130d`
- After SHA-256: `ce48e2fb6a6a547c9931d264aeb6cca1f18ccd353392e89bff99d36ef063867d`

## 83. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c03-s02-t2/Program.cs`
- Summary: bytes=7660
- Before SHA-256: `b4216b20ebd0584c298b556250964b6f5fd2f80b9dc0843a6de7f7392c0f4a82`
- After SHA-256: `09755bff6605d349e2c24727c4a2d337238acd36f6c90f01464f3edbbf2869cf`

## 84. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/m6-special-function-boundary-consumer.csproj`
- Summary: bytes=546
- Before SHA-256: `ec8c719ad62df951a128cb7a15d7c3cc57a52846446925e0be909786983ee8ae`
- After SHA-256: `8f818fc44bbcf93ebbf7ea2102a48d81f42e184e3f1f6fbd3bc0904e5c5c1ce6`

## 85. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c02-t2/Program.cs`
- Summary: bytes=6092
- Before SHA-256: `1146aa5bc6034541e7c5e4d72e8659e76865617ac61af9baf4998847e1a948ea`
- After SHA-256: `d4e1adaad6c0ee0c48ec8ceb82c4cfd20ea8dec7abf443445422aca22480cdea`

## 86. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c04-t1/m6-effective-finance-consumer.csproj`
- Summary: bytes=537
- After SHA-256: `396ab7ee623bf983f38b86fb946f8f8afa8516344a378917419ed2f07ed91e39`

## 87. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c04-t1/Program.cs`
- Summary: bytes=6220
- After SHA-256: `8d6a69ff08154c5771ba0fbe0a85123bd3d8786dded9b524cb67bdff372d6e15`

## 88. `files.replace-from-staged`

- Status: `Changed`
- Path: `tests/consumers/m6-w02-c04-t1/NuGet.Config`
- Summary: bytes=113
- After SHA-256: `2142cfb2dfa2c7feefbe40fabcc68b0dfecadafa99a1db9b70239d977cf2a7ef`

## 89. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c04-t1-financial-rate-contracts.md`
- Summary: bytes=4958
- After SHA-256: `445a6c93762afaff4e00e44c560e146b028d9eab6c676a1c797ded1a6821c6e0`

## 90. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-pure-finance-t1.md`
- Summary: bytes=1962
- After SHA-256: `f682f9682533f7279bf34caaaef61165a001b9a5094db9b6fce37ccda9881852`

## 91. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C04-T1-qualified.json`
- Summary: bytes=9875
- After SHA-256: `edfd1455fdd5c34459372a3b6531ebdde056e6281badca9eed89c450457735a6`

## 92. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `52aab3b68e9d48a85a1a6f1c2d4de925092f5171ca422d7e68e85b267173d82f`
- After SHA-256: `02964ab20e0a2384451e89cbd523f2e6f8c7de0dbe61e1f988ae5bd4fb2f4567`

## 93. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 not-ready->ready
- Before SHA-256: `01b0f455d4d048e3c2feeeecfb33a338963dba5fdef47e8762e136aef1ed010c`
- After SHA-256: `605ca2b49d0e1886d03d7878b35d125e380f4a0dc158d2b5cfb959d1c5d3bb32`

## 94. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1 not-ready->ready
- Before SHA-256: `605ca2b49d0e1886d03d7878b35d125e380f4a0dc158d2b5cfb959d1c5d3bb32`
- After SHA-256: `c7eb905c845fb44a7e8240919befee7e55e946a7c5255d9c8b31cf83e9de69bf`

## 95. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-R not-ready->ready
- Before SHA-256: `c7eb905c845fb44a7e8240919befee7e55e946a7c5255d9c8b31cf83e9de69bf`
- After SHA-256: `bd014d06517f4a1a9e17847771dd6737c6997705a8fee957a862959266dca334`

## 96. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 ready->in-progress
- Before SHA-256: `bd014d06517f4a1a9e17847771dd6737c6997705a8fee957a862959266dca334`
- After SHA-256: `d946e95f4a8c20b13f1de259da74efbd196f04973a5fe74b6cc06470c4ae0692`

## 97. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1 ready->in-progress
- Before SHA-256: `d946e95f4a8c20b13f1de259da74efbd196f04973a5fe74b6cc06470c4ae0692`
- After SHA-256: `114263b7002bb2a76d4f9ba34b4f1b527dc6cf44fb93ad1b35486a7683ef25e0`

## 98. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-R ready->in-progress
- Before SHA-256: `114263b7002bb2a76d4f9ba34b4f1b527dc6cf44fb93ad1b35486a7683ef25e0`
- After SHA-256: `0d7e5c41d4ea41e1be4efa269663a9fa39b0b434fed8a5ec1dea0c029cdd0426`

## 99. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-R in-progress->implemented->validated->done
- Before SHA-256: `0d7e5c41d4ea41e1be4efa269663a9fa39b0b434fed8a5ec1dea0c029cdd0426`
- After SHA-256: `863a8b5330a04cbb0eac08f9111591b643722abdb7e5705c7c4576393d9bc5b1`

## 100. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-G not-ready->ready
- Before SHA-256: `863a8b5330a04cbb0eac08f9111591b643722abdb7e5705c7c4576393d9bc5b1`
- After SHA-256: `b74a8c06abb870e7034628c7162776603098ed2b4d098c19a533145f84b8ac62`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-G ready->in-progress
- Before SHA-256: `b74a8c06abb870e7034628c7162776603098ed2b4d098c19a533145f84b8ac62`
- After SHA-256: `d803b3b9439f820a948f2bc1b4b26e32e278b7cf53d5b47dc753068c9c0c80e6`

## 102. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-G in-progress->implemented->validated->done
- Before SHA-256: `d803b3b9439f820a948f2bc1b4b26e32e278b7cf53d5b47dc753068c9c0c80e6`
- After SHA-256: `e6a7c510a842bea06ff145af622ebff32fa11e12e4b63120dacdbff531cce1f2`

## 103. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-V not-ready->ready
- Before SHA-256: `e6a7c510a842bea06ff145af622ebff32fa11e12e4b63120dacdbff531cce1f2`
- After SHA-256: `e785092df15d0e0e7ad28a81632eee5b1572a092371f995de7bf5821251a1f8a`

## 104. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-V ready->in-progress
- Before SHA-256: `e785092df15d0e0e7ad28a81632eee5b1572a092371f995de7bf5821251a1f8a`
- After SHA-256: `291c1e5c6834c8079fb75a61519584fd7a5af16af0661d8ec5506a364fb295fb`

## 105. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-V in-progress->implemented->validated->done
- Before SHA-256: `291c1e5c6834c8079fb75a61519584fd7a5af16af0661d8ec5506a364fb295fb`
- After SHA-256: `c7973742ff9b38cb5c5c55b7bfb79253a1afa568a94a4c8c512faf414238cb01`

## 106. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1 in-progress->implemented->validated->done
- Before SHA-256: `c7973742ff9b38cb5c5c55b7bfb79253a1afa568a94a4c8c512faf414238cb01`
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 107. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=in-progress
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 108. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1 state=done
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 109. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-R state=done
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 110. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-G state=done
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 111. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T1-V state=done
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04-T2 state=not-ready
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=not-ready
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 115. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `3f8a73f0353cc9d98f3cb77b7c7215b313074a2260c8fb981eb57cc4704555e0`

## 116. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `f27e02328d5f301635d85a857a2d5b4d516ee73613c698f1315b7dd224e5d199`
- After SHA-256: `b164e37763f759c8c701d4608355dc3aa364b567aedc16f079927b99e5701237`

