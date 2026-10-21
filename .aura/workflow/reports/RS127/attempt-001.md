# RS127 Attempt 001 Payload Operations

- Operations: 40
- Changed: 4
- No change: 0
- Verified: 36
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS127 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS126/attempt-001.json` | run=RS126 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/packages_packed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/source_free_consumers |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/fresh_consumer_probes |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/native_nodes_retained |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/publicly_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS126-post-m7-distribution-qualified.json` | pointer=/cross_version_abi_certified |
| 12 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS126/attempt-001.json` | pointer=/priorInvocationFailures/count |
| 13 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS126/attempt-001.json` | pointer=/priorInvocationFailures/firstPassSucceeded |
| 14 | `require.text-contains` | Verified | `docs/coordination/requests/README.md` | semantic text found |
| 15 | `require.text-contains` | Verified | `docs/discovery/README.md` | semantic text found |
| 16 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 17 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 18 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 19 | `require.file-missing` | Verified | `docs/planning/decisions/post-m7-complete-local-packaging.md` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/post-m7-complete-package-matrix.md` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/planning/ValidatePostM7CompletePackaging.cs` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | path is absent |
| 23 | `files.replace-from-staged` | Changed | `docs/planning/decisions/post-m7-complete-local-packaging.md` | bytes=6211 |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/post-m7-complete-package-matrix.md` | bytes=3920 |
| 25 | `files.replace-from-staged` | Changed | `docs/planning/ValidatePostM7CompletePackaging.cs` | bytes=20454 |
| 26 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-complete-local-packaging.md` | semantic text found |
| 27 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-complete-local-packaging.md` | semantic text found |
| 28 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-complete-local-packaging.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-complete-local-packaging.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `docs/distribution/post-m7-complete-package-matrix.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `docs/distribution/post-m7-complete-package-matrix.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `docs/distribution/post-m7-complete-package-matrix.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7CompletePackaging.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7CompletePackaging.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7CompletePackaging.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7CompletePackaging.cs` | semantic text found |
| 37 | `files.write-complete` | Changed | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | bytes=23870 |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS127 hierarchy=exact
- After SHA-256: `775e665346b17cdcc5b9dcff4358f69f50a4cfef9aea7f01a67cb99e44ed7bbe`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS126/attempt-001.json`
- Summary: run=RS126 attempt=1 lineage=exact
- After SHA-256: `c8812d9ff4dc789276bc7f5c3b940cbd0f38469a083122f42a9827d4aabf4d06`

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
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/packages_packed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/source_free_consumers

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/fresh_consumer_probes

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/native_nodes_retained

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/publicly_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS126-post-m7-distribution-qualified.json`
- Summary: pointer=/cross_version_abi_certified

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS126/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/count

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS126/attempt-001.json`
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
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/post-m7-complete-package-matrix.md`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: path is absent

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: bytes=6211
- After SHA-256: `41b90a52ff1514c1ce54812be077e278e940eae7a5bb595837d97ec3285e88db`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/post-m7-complete-package-matrix.md`
- Summary: bytes=3920
- After SHA-256: `50369ab9d9333555c655a8f400474a359dee5a2baaf342154581bf68632b200f`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: bytes=20454
- After SHA-256: `416f4bce9f50add87bb73a097c1c1377cc0c792fab780c9fd27bcaac571f02bd`

## 26. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-complete-local-packaging.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-complete-package-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-complete-package-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-complete-package-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7CompletePackaging.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: bytes=23870
- After SHA-256: `9128ec0ca7794d87e7611a269e21187cd61e60d4cec1e6893e21260f48a9c621`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

