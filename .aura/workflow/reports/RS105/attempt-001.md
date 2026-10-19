# RS105 Attempt 001 Payload Operations

- Operations: 92
- Changed: 7
- No change: 0
- Verified: 85
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS105 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS104/attempt-001.json` | run=RS104 attempt=1 lineage=exact |
| 3 | `require.json-value-baseline-or-target` | Verified | `docs/planning/backlog.json` | pointer=/plan_version state=baseline |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/5/status |
| 7 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/id |
| 8 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/version |
| 9 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/gate |
| 10 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/6/status |
| 11 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/releases/7/status |
| 12 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/id |
| 13 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/13/status |
| 14 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/id |
| 15 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/work_packages/14/status |
| 16 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 17 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=in-progress |
| 18 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01 state=done |
| 19 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02 state=done |
| 20 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 21 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C01 state=done |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json` | pointer=/status |
| 23 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 24 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C02 state=done |
| 25 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/status |
| 26 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 27 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W01-C03 state=done |
| 28 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/status |
| 29 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 30 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C01 state=done |
| 31 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/status |
| 32 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 33 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C02 state=done |
| 34 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json` | pointer=/status |
| 35 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03 state=done |
| 37 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/status |
| 38 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S01 state=done |
| 40 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/status |
| 41 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 42 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S02 state=done |
| 43 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/status |
| 44 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 45 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C03-S03 state=done |
| 46 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json` | pointer=/status |
| 47 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 48 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C04 state=done |
| 49 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/status |
| 50 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 51 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6-W02-C05 state=done |
| 52 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/status |
| 53 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/m6_release_done |
| 54 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/status |
| 55 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/local_product_version |
| 56 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/actual_inspected_package_sha256 |
| 57 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/actual_inspected_package_bytes |
| 58 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/source_free_consumer_count |
| 59 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/independent_consumer_probes_passed |
| 60 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/m6_w01_work_package_done |
| 61 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W01-local-gate-qualified.json` | pointer=/m6_release_done |
| 62 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/local_product_version |
| 63 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/actual_inspected_package_sha256 |
| 64 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/actual_inspected_package_bytes |
| 65 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json` | pointer=/m6_release_done |
| 66 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/status |
| 67 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/m6_w02_work_package_done |
| 68 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/owner_approval_obtained |
| 69 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/external_cutover_performed |
| 70 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/publicly_published |
| 71 | `require.json-value-equals` | Verified | `docs/planning/evidence/M6-W02-C05-T2-qualified.json` | pointer=/m6_release_done |
| 72 | `require.json-value-equals` | Verified | `docs/planning/releases/m5-v1-publication-boundary.json` | pointer=/owner/stable_v1_publication_authorized |
| 73 | `require.json-value-equals` | Verified | `docs/planning/releases/m5-v1-publication-boundary.json` | pointer=/publication/public_nuget_published |
| 74 | `require.text-contains` | Verified | `docs/planning/ValidateNativeDwfAdoption.cs` | semantic text found |
| 75 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 76 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 77 | `require.file-missing` | Verified | `docs/planning/ValidateM6ReleaseGate.cs` | path is absent |
| 78 | `require.file-missing` | Verified | `docs/planning/decisions/m6-gate.md` | path is absent |
| 79 | `require.file-missing` | Verified | `docs/distribution/m6-local-release-gate.md` | path is absent |
| 80 | `require.file-missing` | Verified | `docs/planning/evidence/M6-gate.json` | path is absent |
| 81 | `files.replace-from-staged` | Changed | `docs/planning/ValidateM6ReleaseGate.cs` | bytes=13156 |
| 82 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m6-gate.md` | bytes=4097 |
| 83 | `files.replace-from-staged` | Changed | `docs/distribution/m6-local-release-gate.md` | bytes=3076 |
| 84 | `require.text-contains` | Verified | `docs/planning/ValidateM6ReleaseGate.cs` | semantic text found |
| 85 | `require.text-contains` | Verified | `docs/planning/decisions/m6-gate.md` | semantic text found |
| 86 | `require.text-contains` | Verified | `docs/distribution/m6-local-release-gate.md` | semantic text found |
| 87 | `files.write-complete` | Changed | `docs/planning/evidence/M6-gate.json` | bytes=5473 |
| 88 | `json.edit-object` | Changed | `docs/planning/backlog.json` | semantic object updated |
| 89 | `project-plan.converge-node-to-done` | Changed | `.aura/workflow/plan/project.json` | node=M6 in-progress->implemented->validated->done |
| 90 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 91 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=not-ready |
| 92 | `json.edit-object` | Changed | `.aura/workflow/state.json` | semantic object updated |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS105 hierarchy=exact
- After SHA-256: `d21140cc55f64666e031cdd23a99c82e703229d7f2b202a1b0c6255bd59de4bb`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS104/attempt-001.json`
- Summary: run=RS104 attempt=1 lineage=exact
- After SHA-256: `4a09025dc5ad4c9347572f7a43eb93b8c40290fd4be68a781e578aa317dfcf66`

## 3. `require.json-value-baseline-or-target`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version state=baseline

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/5/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/id

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/version

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/gate

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/6/status

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/releases/7/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/id

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/13/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/id

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/work_packages/14/status

## 16. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 17. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=in-progress
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 18. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 19. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 20. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 21. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C01 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 24. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C02 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 25. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 26. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 27. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W01-C03 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 28. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 29. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 30. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C01 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 31. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 32. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 33. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C02 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 34. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 35. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S01 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S01-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 42. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S02 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 43. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S02-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 45. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C03-S03 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 46. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 47. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C03-S03-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 48. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C04 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 49. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/status

## 50. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 51. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6-W02-C05 state=done
- After SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`

## 52. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/status

## 53. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/m6_release_done

## 54. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/status

## 55. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/local_product_version

## 56. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/actual_inspected_package_sha256

## 57. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/actual_inspected_package_bytes

## 58. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/source_free_consumer_count

## 59. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/independent_consumer_probes_passed

## 60. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/m6_w01_work_package_done

## 61. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W01-local-gate-qualified.json`
- Summary: pointer=/m6_release_done

## 62. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/local_product_version

## 63. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/actual_inspected_package_sha256

## 64. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/actual_inspected_package_bytes

## 65. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C04-T2-boundaries-qualified.json`
- Summary: pointer=/m6_release_done

## 66. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/status

## 67. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/m6_w02_work_package_done

## 68. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/owner_approval_obtained

## 69. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/external_cutover_performed

## 70. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/publicly_published

## 71. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-W02-C05-T2-qualified.json`
- Summary: pointer=/m6_release_done

## 72. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/releases/m5-v1-publication-boundary.json`
- Summary: pointer=/owner/stable_v1_publication_authorized

## 73. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/releases/m5-v1-publication-boundary.json`
- Summary: pointer=/publication/public_nuget_published

## 74. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 75. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 76. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 77. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidateM6ReleaseGate.cs`
- Summary: path is absent

## 78. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-gate.md`
- Summary: path is absent

## 79. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/m6-local-release-gate.md`
- Summary: path is absent

## 80. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: path is absent

## 81. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateM6ReleaseGate.cs`
- Summary: bytes=13156
- After SHA-256: `7380f28216eff0a0b14ddf9ecbd30f7296cc9c921e2ebc59f54daa09f800a52c`

## 82. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m6-gate.md`
- Summary: bytes=4097
- After SHA-256: `cd7bad36de228a1e42794a328921a77fd4bdeb6e288303b65b843cf47e331ed7`

## 83. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/m6-local-release-gate.md`
- Summary: bytes=3076
- After SHA-256: `e1c4a538ccce56af4ff1520d43b8f873272113521ae055c52086cb82c6bf9449`

## 84. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidateM6ReleaseGate.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 85. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/m6-gate.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 86. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/m6-local-release-gate.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 87. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/M6-gate.json`
- Summary: bytes=5473
- After SHA-256: `a41893f1571bd3b6144fe44a5e0c7dda4caf03287d957e3477ff1b97f8453e04`

## 88. `json.edit-object`

- Status: `Changed`
- Path: `docs/planning/backlog.json`
- Summary: semantic object updated
- Before SHA-256: `e3f00359dc2e69e923092d1af4aa2ce78d929cf59b9c90635dfaf57525fbf84b`
- After SHA-256: `80058e1632be7e97ce7be7b9eff965c4d08207c758f795610e5980748f1ab0e9`

## 89. `project-plan.converge-node-to-done`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 in-progress->implemented->validated->done
- Before SHA-256: `8a81419b2ec50018432f6023f5d07dbe2372bf20bd488601153946cdef081522`
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 90. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 91. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=not-ready
- After SHA-256: `e24e7ccbad53a724fdbf39716068ffda94f938088afc00a6faf48529a8b308fb`

## 92. `json.edit-object`

- Status: `Changed`
- Path: `.aura/workflow/state.json`
- Summary: semantic object updated
- Before SHA-256: `ad5d7cae72d5e671d9aafec12177bcc5372eba779998f89fc8fbbe6582457d2a`
- After SHA-256: `b80b5984dc47be3c1ab92de6ed0878f553bcdf1c04f60624ceafbea385791e81`

