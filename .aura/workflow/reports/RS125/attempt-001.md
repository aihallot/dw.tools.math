# RS125 Attempt 001 Payload Operations

- Operations: 40
- Changed: 4
- No change: 0
- Verified: 36
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS125 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS124/attempt-001.json` | run=RS124 attempt=1 lineage=exact |
| 3 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 4 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 5 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/status |
| 6 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/source_projects |
| 7 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/test_projects |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/fresh_local_package_archives |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/fresh_source_free_consumer_rerun |
| 10 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/native_nodes_retained |
| 11 | `require.json-value-equals` | Verified | `docs/planning/evidence/RS124-post-m7-delivery-qualified.json` | pointer=/publicly_published |
| 12 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS124/attempt-001.json` | pointer=/priorInvocationFailures/count |
| 13 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS124/attempt-001.json` | pointer=/priorInvocationFailures/preparationFailureCount |
| 14 | `require.json-value-equals` | Verified | `.aura/workflow/reports/RS124/attempt-001.json` | pointer=/priorInvocationFailures/firstPassSucceeded |
| 15 | `require.text-contains` | Verified | `docs/coordination/requests/README.md` | semantic text found |
| 16 | `require.text-contains` | Verified | `docs/discovery/README.md` | semantic text found |
| 17 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M5 state=done |
| 18 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 19 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 20 | `require.file-missing` | Verified | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/distribution/post-m7-source-free-consumer-matrix.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/planning/ValidatePostM7ConsumerIsolation.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | bytes=5659 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/post-m7-source-free-consumer-matrix.md` | bytes=3621 |
| 26 | `files.replace-from-staged` | Changed | `docs/planning/ValidatePostM7ConsumerIsolation.cs` | bytes=14001 |
| 27 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | semantic text found |
| 28 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `docs/planning/decisions/post-m7-source-free-consumer-qualification.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `docs/distribution/post-m7-source-free-consumer-matrix.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `docs/distribution/post-m7-source-free-consumer-matrix.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `docs/distribution/post-m7-source-free-consumer-matrix.md` | semantic text found |
| 34 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7ConsumerIsolation.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7ConsumerIsolation.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `docs/planning/ValidatePostM7ConsumerIsolation.cs` | semantic text found |
| 37 | `files.write-complete` | Changed | `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json` | bytes=10042 |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS125 hierarchy=exact
- After SHA-256: `0e657815d015cdde922958a0d95f7476e02364e3a6abf0dea11a65b935ce7072`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS124/attempt-001.json`
- Summary: run=RS124 attempt=1 lineage=exact
- After SHA-256: `2cd1820f76c53600d7bd0e918ed88347dbf040472aad740ad44f37751e3c05f3`

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
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/status

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/source_projects

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/test_projects

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/fresh_local_package_archives

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/fresh_source_free_consumer_rerun

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/native_nodes_retained

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/RS124-post-m7-delivery-qualified.json`
- Summary: pointer=/publicly_published

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS124/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/count

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS124/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/preparationFailureCount

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS124/attempt-001.json`
- Summary: pointer=/priorInvocationFailures/firstPassSucceeded

## 15. `require.text-contains`

- Status: `Verified`
- Path: `docs/coordination/requests/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 16. `require.text-contains`

- Status: `Verified`
- Path: `docs/discovery/README.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M5 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 18. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 19. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/post-m7-source-free-consumer-matrix.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7ConsumerIsolation.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: bytes=5659
- After SHA-256: `e748eb7b5f8072c7ca84a74c773e129cd2756261becb6aaad721a7445b0fb0aa`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/post-m7-source-free-consumer-matrix.md`
- Summary: bytes=3621
- After SHA-256: `d63e25371d063f4fa77c1b69e1be5e5aa5d74dd40f55798a682991b7bfd56471`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidatePostM7ConsumerIsolation.cs`
- Summary: bytes=14001
- After SHA-256: `ca3260e088363311694a96ee22b492e4855b3eb9dc764794b7075b1a744cdac4`

## 27. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/decisions/post-m7-source-free-consumer-qualification.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-source-free-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-source-free-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/post-m7-source-free-consumer-matrix.md`
- Summary: semantic text found
- Details: comparison=OrdinalIgnoreCase normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7ConsumerIsolation.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7ConsumerIsolation.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `docs/planning/ValidatePostM7ConsumerIsolation.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `files.write-complete`

- Status: `Changed`
- Path: `docs/planning/evidence/RS125-post-m7-nuget-consumers-qualified.json`
- Summary: bytes=10042
- After SHA-256: `f91749feed7bdab42a9ec2ed208592cbd96d2469c708e4a2f57fff8b415d0d90`

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

