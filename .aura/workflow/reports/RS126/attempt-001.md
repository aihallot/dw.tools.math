# RS126 Attempt 001 Payload Operations

- Operations: 39
- Changed: 4
- No change: 0
- Verified: 35
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS126 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS125/attempt-001.json` | run=RS125 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/source_free_consumers |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/fresh_consumer_probes |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/fresh_packages_built |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/native_nodes_retained |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/publicly_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | pointer=/cross_version_abi_certified |
| 12 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS125/attempt-001.json` | pointer=/priorInvocationFailures/count |
| 13 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS125/attempt-001.json` | pointer=/priorInvocationFailures/firstPassSucceeded |
| 14 | `require.text-contains` | Verified | `docs/coordination/requests/README.md` | semantic text found |
| 15 | `require.text-contains` | Verified | `docs/discovery/README.md` | semantic text found |
| 16 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 17 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 18 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 19 | `require.file-missing` | Verified | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/post-m7-seven-package-surface-matrix.md` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/planning/ValidatePostM7DistributionGraph.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | path is absent |
| 23 | `files.replace-from-staged` | Changed | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | bytes=6374 |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/post-m7-seven-package-surface-matrix.md` | bytes=3775 |
| 25 | `files.replace-from-staged` | Changed | `docs/planning/ValidatePostM7DistributionGraph.cs` | bytes=19590 |
| 26 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | semantic text found |
| 27 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | semantic text found |
| 28 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-distribution-graph-qualification.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `docs/distribution/post-m7-seven-package-surface-matrix.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `docs/distribution/post-m7-seven-package-surface-matrix.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `docs/distribution/post-m7-seven-package-surface-matrix.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7DistributionGraph.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7DistributionGraph.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7DistributionGraph.cs` | semantic text found |
| 36 | `files.write-complete` | Changed | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | bytes=18835 |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS126 hierarchy=exact
- After SHA-256: `e93be76b6f66361a32930c53e120d61ede849262ace124cf7f642e3aa069dc21`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS125/attempt-001.json`
- Summary: run=RS125 attempt=1 lineage=exact
- After SHA-256: `acdfda90043ab3e92c64ebf8cd9485e7b422291b7456442e2a4405803ece5737`

## 3. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 4. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/source_free_consumers

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/fresh_consumer_probes

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/fresh_packages_built

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/native_nodes_retained

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/publicly_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: pointer=/cross_version_abi_certified

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS125/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/count

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS125/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/firstPassSucceeded

## 14. `require.text-contains`

- Status: `Verified`
- Path: `docs/coordination/requests/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 15. `require.text-contains`

- Status: `Verified`
- Path: `docs/discovery/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 16. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 17. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 18. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/post-m7-seven-package-surface-matrix.md`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7DistributionGraph.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: path is absent

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: bytes=6374
- After SHA-256: `21fffc56d01a81d7d5e5d398ca1807daa218fc39e846304b80fd40f03e802ab0`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/post-m7-seven-package-surface-matrix.md`
- Summary: bytes=3775
- After SHA-256: `a9965bb29a75adbdc1c5d00a38fc98ab7820171a5ee063bc690312605f13cca1`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidatePostM7DistributionGraph.cs`
- Summary: bytes=19590
- After SHA-256: `517c2604d75ae7860504a63de591c792201838e78a6585d5dc2877e797b03cde`

## 26. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-distribution-graph-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-seven-package-surface-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-seven-package-surface-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-seven-package-surface-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7DistributionGraph.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7DistributionGraph.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7DistributionGraph.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: bytes=18835
- After SHA-256: `ea5135974dd250274f74059350c44a2d43500f64511f591876c42096fe4742f9`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

