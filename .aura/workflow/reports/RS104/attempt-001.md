# RS104 Attempt 001 Payload Operations

- Operations: 117
- Changed: 19
- No change: 0
- Verified: 98
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS104 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS103/attempt-001.json` | run=RS103 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/status |
| 4 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/qualified_family_count |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/previous_source_free_consumer_count |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/prior_package_version |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/prior_package_sha256 |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/prior_package_bytes |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/historical_receipts_preserved |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/decision_duplicate_verified |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/decision_owner_approval_obtained |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/decision_sibling_modified |
| 13 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/new_scientific_tests_executed |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/m6_w02_c05_t1_done |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/m6_w02_c05_t2_done |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/m6_w02_c05_parent_done |
| 17 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/m6_w02_work_package_done |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T1-qualified.json` | pointer=/m6_release_done |
| 19 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 20 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/actual_inspected_package_sha256 |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/actual_inspected_package_bytes |
| 22 | `require.json-value-equals` | Verified | `docs/planning/releases/m5-v1-publication-boundary.json` | pointer=/owner/stable_v1_publication_authorized |
| 23 | `require.json-value-equals` | Verified | `docs/planning/releases/m5-v1-publication-boundary.json` | pointer=/publication/public_nuget_published |
| 24 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 25 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-family-adoption.md` | semantic text found |
| 26 | `require.text-contains` | Verified | `docs/planning/requests/decision-bounded-bracketed-root-assessment.md` | semantic text found |
| 27 | `require.text-contains` | Verified | `docs/distribution/m5-package-notices.md` | semantic text found |
| 28 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 29 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 30 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 31 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 34 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 35 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/43/status |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/44/status |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/45/status |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/46/status |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/47/status |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/48/status |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/49/status |
| 42 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/id |
| 43 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/kind |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/status |
| 45 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/0/id |
| 46 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/0/status |
| 47 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/1/id |
| 48 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/1/status |
| 49 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/0/subtasks/0/status |
| 50 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/1/subtasks/0/status |
| 51 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/0/subtasks/1/status |
| 52 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/1/subtasks/1/status |
| 53 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/0/subtasks/2/status |
| 54 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/chunks/50/tasks/1/subtasks/2/status |
| 55 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 56 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 57 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 58 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 59 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=in-progress |
| 60 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 61 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 62 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=done |
| 63 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 64 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=done |
| 65 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=done |
| 66 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=done |
| 67 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=in-progress |
| 68 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T1 state=done |
| 69 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T1-A state=done |
| 70 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T1-B state=done |
| 71 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T1-C state=done |
| 72 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2 state=not-ready |
| 73 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-A state=not-ready |
| 74 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-B state=not-ready |
| 75 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-C state=not-ready |
| 76 | `require.file-missing` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | path is absent |
| 77 | `require.file-missing` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | path is absent |
| 78 | `require.file-missing` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | path is absent |
| 79 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-w02-c05-t2.md` | bytes=4398 |
| 80 | `files.replace-from-staged` | Changed | `docs/distribution/m6-scientific-adoption-notices.md` | bytes=3918 |
| 81 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 82 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 83 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 84 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 85 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 86 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 87 | `require.text-contains` | Verified | `docs/planning/decisions/m6-w02-c05-t2.md` | semantic text found |
| 88 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 89 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 90 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 91 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 92 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 93 | `require.text-contains` | Verified | `docs/distribution/m6-scientific-adoption-notices.md` | semantic text found |
| 94 | `files.write-complete` | Changed | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | bytes=3174 |
| 95 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 96 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2 not-ready->ready |
| 97 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-A not-ready->ready |
| 98 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2 ready->in-progress |
| 99 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-A ready->in-progress |
| 100 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-A in-progress->implemented->validated->done |
| 101 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-B not-ready->ready |
| 102 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-B ready->in-progress |
| 103 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-B in-progress->implemented->validated->done |
| 104 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-C not-ready->ready |
| 105 | `project-plan.transition-node` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-C ready->in-progress |
| 106 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-C in-progress->implemented->validated->done |
| 107 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2 in-progress->implemented->validated->done |
| 108 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02-C05 in-progress->implemented->validated->done |
| 109 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6-W02 in-progress->implemented->validated->done |
| 110 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-A state=done |
| 111 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-B state=done |
| 112 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2-C state=done |
| 113 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05-T2 state=done |
| 114 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=done |
| 115 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=done |
| 116 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 117 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS104 hierarchy=exact
- After SHA-256: `5d3be2fc2b35b54c7cfbfcc7a79d23c3f60c607c2af840914ea2ddb1d6165c5f`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS103/attempt-001.json`
- Summary: run=RS103 attempt=1 lineage=exact
- After SHA-256: `455ae240f221edb43fb48f7997545953c52047d14f0a337fc3a4d0805c63897a`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/status

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/qualified_family_count

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/previous_source_free_consumer_count

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/prior_package_version

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/prior_package_sha256

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/prior_package_bytes

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/historical_receipts_preserved

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/decision_duplicate_verified

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/decision_owner_approval_obtained

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/decision_sibling_modified

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/new_scientific_tests_executed

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/m6_w02_c05_t1_done

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/m6_w02_c05_t2_done

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/m6_w02_c05_parent_done

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/m6_w02_work_package_done

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T1-qualified.json`
- Summary: pointer=/m6_release_done

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/actual_inspected_package_sha256

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/actual_inspected_package_bytes

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/releases/m5-v1-publication-boundary.json`
- Summary: pointer=/owner/stable_v1_publication_authorized

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/releases/m5-v1-publication-boundary.json`
- Summary: pointer=/publication/public_nuget_published

## 24. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-family-adoption.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/requests/decision-bounded-bracketed-root-assessment.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m5-package-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 30. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/43/status

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/44/status

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/45/status

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/46/status

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/47/status

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/48/status

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/49/status

## 42. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/id

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/kind

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/status

## 45. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/0/id

## 46. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/0/status

## 47. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/1/id

## 48. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/1/status

## 49. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/0/subtasks/0/status

## 50. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/1/subtasks/0/status

## 51. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/0/subtasks/1/status

## 52. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/1/subtasks/1/status

## 53. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/0/subtasks/2/status

## 54. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/chunks/50/tasks/1/subtasks/2/status

## 55. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 56. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 57. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 58. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 59. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=in-progress
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 60. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 61. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 62. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 63. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 64. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 65. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 66. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 67. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=in-progress
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 68. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T1 state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 69. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T1-A state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 70. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T1-B state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 71. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T1-C state=done
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 72. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2 state=not-ready
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 73. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-A state=not-ready
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 74. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-B state=not-ready
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 75. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-C state=not-ready
- After SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`

## 76. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: path is absent

## 77. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: path is absent

## 78. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: path is absent

## 79. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: bytes=4398
- After SHA-256: `031746f355254c5b1c050af35dc386aaf3b916b5a45ec6cbc1d9a5f10b567b79`

## 80. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: bytes=3918
- After SHA-256: `f2493ba1c3cf09963722c54c7f2d95a72c7cf5ee2b0fd5e67b6f60e6cfe5fa1c`

## 81. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 82. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 83. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 84. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 85. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 86. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 87. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-w02-c05-t2.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 88. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 89. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 90. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 91. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 92. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 93. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-scientific-adoption-notices.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 94. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: bytes=3174
- After SHA-256: `57388740eabba724737d2d93f4eb0263000fa0ec350eb6e5bfde596513a6d487`

## 95. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `9001cc6f811eec67ea43130b185e30b602ed76a0ade8610ae0584d6b6af513d9`
- After SHA-256: `df304c20c61b42ceace8554d910591622c00cf856659801e85b26ab2cffa6632`

## 96. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2 not-ready->ready
- Before SHA-256: `ef850d4750040ce6e1ff9b52d7b50fbf9a2cb851f5cf3dc936bba79c07666f75`
- After SHA-256: `43e25e90a51a8c83ac521715f5f2277dc8ebb9d1812902cc5e5300b1877ab4aa`

## 97. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-A not-ready->ready
- Before SHA-256: `43e25e90a51a8c83ac521715f5f2277dc8ebb9d1812902cc5e5300b1877ab4aa`
- After SHA-256: `d2ea70434c2530d8f391045de3b7edaa7def6d5d92ec1d985c1d2e70a31d106c`

## 98. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2 ready->in-progress
- Before SHA-256: `d2ea70434c2530d8f391045de3b7edaa7def6d5d92ec1d985c1d2e70a31d106c`
- After SHA-256: `97a01a5de23b3dfc45ca0c5ba34dd0f0d64488ac59f85a1496be4f6afa122e7e`

## 99. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-A ready->in-progress
- Before SHA-256: `97a01a5de23b3dfc45ca0c5ba34dd0f0d64488ac59f85a1496be4f6afa122e7e`
- After SHA-256: `d5e3c059d39308ab2a848416d6086b851323b0a3d852b405b4cd15936bd54a6a`

## 100. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-A in-progress->implemented->validated->done
- Before SHA-256: `d5e3c059d39308ab2a848416d6086b851323b0a3d852b405b4cd15936bd54a6a`
- After SHA-256: `b2b1c2c7264974ac78d3bfbc42a289d1d37c3ffa8abbfbf943703ddf4e5a3b34`

## 101. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-B not-ready->ready
- Before SHA-256: `b2b1c2c7264974ac78d3bfbc42a289d1d37c3ffa8abbfbf943703ddf4e5a3b34`
- After SHA-256: `04e8eec422fa70d0da4212828dac80cc7ce82482dd4a5c07d9098aac26da06a4`

## 102. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-B ready->in-progress
- Before SHA-256: `04e8eec422fa70d0da4212828dac80cc7ce82482dd4a5c07d9098aac26da06a4`
- After SHA-256: `76bf41a479b32e5faae3e41f7bc0d0be2a2cd62761d6b686f27071abe0051084`

## 103. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-B in-progress->implemented->validated->done
- Before SHA-256: `76bf41a479b32e5faae3e41f7bc0d0be2a2cd62761d6b686f27071abe0051084`
- After SHA-256: `918119d2b93416cafb6ec6110dc28cdfed591de55a34da278b9801d78b97a1d2`

## 104. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-C not-ready->ready
- Before SHA-256: `918119d2b93416cafb6ec6110dc28cdfed591de55a34da278b9801d78b97a1d2`
- After SHA-256: `d13daed7d2b1bbaede29de035a2b1c2b67237568899c876324fed7b6b8e4d335`

## 105. `project-plan.transition-node`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-C ready->in-progress
- Before SHA-256: `d13daed7d2b1bbaede29de035a2b1c2b67237568899c876324fed7b6b8e4d335`
- After SHA-256: `a3232af4eecf45274651277b2dd47cfede765e9518ee6b799cd397911f2bead7`

## 106. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-C in-progress->implemented->validated->done
- Before SHA-256: `a3232af4eecf45274651277b2dd47cfede765e9518ee6b799cd397911f2bead7`
- After SHA-256: `87e6c3cf66c87f943f2dcc83d3b5af61a8c8120bd2b7c4233e82ff2f8008c4b3`

## 107. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2 in-progress->implemented->validated->done
- Before SHA-256: `87e6c3cf66c87f943f2dcc83d3b5af61a8c8120bd2b7c4233e82ff2f8008c4b3`
- After SHA-256: `1f7e7081439fe5c54335a7bf748837b595a7b2fb0ed981b2b4bfa421a462e87d`

## 108. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 in-progress->implemented->validated->done
- Before SHA-256: `1f7e7081439fe5c54335a7bf748837b595a7b2fb0ed981b2b4bfa421a462e87d`
- After SHA-256: `a166796f7a2119c0d2a1f4996e2959e38e75a95d6989f162b6b7205a49ffc46f`

## 109. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 in-progress->implemented->validated->done
- Before SHA-256: `a166796f7a2119c0d2a1f4996e2959e38e75a95d6989f162b6b7205a49ffc46f`
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 110. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-A state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 111. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-B state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 112. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2-C state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 113. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05-T2 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 114. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 115. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 116. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 117. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `dfb5eb6c37b9b2daf8ef9c9a450fec258a2c326d69a88ad7976e3c16aa058a74`
- After SHA-256: `ec1e28d38ff8354d9de6057015365f1346eeeac8aad7544320d58e5646c78355`

