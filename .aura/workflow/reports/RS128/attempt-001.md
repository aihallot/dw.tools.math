# RS128 Attempt 001 Payload Operations

- Operations: 42
- Changed: 4
- No change: 0
- Verified: 38
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS128 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS127/attempt-001.json` | run=RS127 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/packages_packed |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/source_free_consumers |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/m3_fresh_probes |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/native_nodes_retained |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/publicly_published |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/cross_version_abi_certified |
| 12 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json` | pointer=/external_requests_transmitted |
| 13 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS127/attempt-001.json` | pointer=/priorInvocationFailures/count |
| 14 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS127/attempt-001.json` | pointer=/priorInvocationFailures/preparationFailureCount |
| 15 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS127/attempt-001.json` | pointer=/priorInvocationFailures/firstPassSucceeded |
| 16 | `require.text-contains` | Verified | `docs/coordination/requests/README.md` | semantic text found |
| 17 | `require.text-contains` | Verified | `docs/discovery/README.md` | semantic text found |
| 18 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 19 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 20 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 21 | `require.file-missing` | Verified | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | path is absent |
| 24 | `require.file-missing` | Verified | `docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json` | path is absent |
| 25 | `files.replace-from-staged` | Changed | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | bytes=5552 |
| 26 | `files.replace-from-staged` | Changed | `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md` | bytes=3272 |
| 27 | `files.replace-from-staged` | Changed | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | bytes=17212 |
| 28 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-symbolic-codec-consumers.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md` | semantic text found |
| 34 | `require.text-contains` | Verified | `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md` | semantic text found |
| 35 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | semantic text found |
| 37 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | semantic text found |
| 38 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs` | semantic text found |
| 39 | `files.write-complete` | Changed | `docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json` | bytes=13252 |
| 40 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 41 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 42 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS128 hierarchy=exact
- After SHA-256: `c479d98cd34a46920b4a98e4ec776af043ccf9b6f5552ae3cdf12c8f07b6ec44`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS127/attempt-001.json`
- Summary: run=RS127 attempt=1 lineage=exact
- After SHA-256: `75e1d4fe7e2241ca3ff5a87f268022587639a1c5b1acf8fa8d9de6bd7c6d1b3a`

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
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/packages_packed

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/source_free_consumers

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/m3_fresh_probes

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/native_nodes_retained

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/publicly_published

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/cross_version_abi_certified

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS127-post-m7-complete-packaging-qualified.json`
- Summary: pointer=/external_requests_transmitted

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS127/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/count

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS127/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/preparationFailureCount

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS127/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/firstPassSucceeded

## 16. `require.text-contains`

- Status: `Verified`
- Path: `docs/coordination/requests/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.text-contains`

- Status: `Verified`
- Path: `docs/discovery/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 18. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 19. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 20. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json`
- Summary: path is absent

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: bytes=5552
- After SHA-256: `346a3304ef4ccd8d3e0d273c2a89a8ea548dda72931c9f4710a61cb8c45b9f1a`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md`
- Summary: bytes=3272
- After SHA-256: `af637ba7ce80ea6b635ab0c94d4e883184f674d7e977ac40cb8dafebe7ca349e`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: bytes=17212
- After SHA-256: `50161c4b4a45d7ac829dd050be6cb48d5c8caf1b202e8d37e6be0cce580d4c03`

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-symbolic-codec-consumers.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-symbolic-codec-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7SymbolicCodecConsumers.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/RS128-post-m7-symbolic-codec-qualified.json`
- Summary: bytes=13252
- After SHA-256: `249b788b97c8af7aba8229e11d2f32641547dbeaadffbd6655d6f26c439e2513`

## 40. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 41. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 42. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

