# RS167 Attempt 001 Payload Operations

- Operations: 40
- Changed: 5
- No change: 0
- Verified: 35
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS167 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS166/attempt-001.json` | run=RS166 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/status |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/package_published |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json` | pointer=/status |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json` | pointer=/direct_dependency_count |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json` | pointer=/package_published_by_this_run |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 20 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 21 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-release/Program.cs` | file exists |
| 22 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs` | file exists |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f34-composition-published.md` | path is absent |
| 24 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCompositionPublishedFeed.cs` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj` | path is absent |
| 26 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs` | path is absent |
| 27 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | path is absent |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f34-composition-published.md` | bytes=5013 |
| 29 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCompositionPublishedFeed.cs` | bytes=14052 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj` | bytes=281 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs` | bytes=21334 |
| 32 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f34-composition-published.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj` | semantic text found |
| 37 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f34-composition-published-qualified.json` | bytes=10969 |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 39 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 40 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS167 hierarchy=exact
- After SHA-256: `1da3ed7a119a2b30a674c0aded696bcb8dfb1bf1bbb9f2a08632134297cf0979`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS166/attempt-001.json`
- Summary: run=RS166 attempt=1 lineage=exact
- After SHA-256: `a449763c925e8a82dcc85cc040119ee9255f41628bb5789c970bcc3a5121df0b`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 5. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

## 7. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/workflow/manifest.json`
- Summary: pointer=/guidanceVersion

## 8. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f31-composition-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/status

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/package_published

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f32-composition-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json`
- Summary: pointer=/status

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json`
- Summary: pointer=/direct_dependency_count

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f33-composition-release-receipt-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 21. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-release/Program.cs`
- Summary: file exists

## 22. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-candidate/fixtures/CompositionBinaryConsumer.cs`
- Summary: file exists

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published.md`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCompositionPublishedFeed.cs`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: path is absent

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f34-composition-published.md`
- Summary: bytes=5013
- After SHA-256: `5bb42a9e271d8d3c1567c96c2d73f7ec5e14d46c0869cd590b22c01d9dc78681`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCompositionPublishedFeed.cs`
- Summary: bytes=14052
- After SHA-256: `2702614e83a5264b3f382a133976bf69473ff3094fb8aa35ba8c42c65639fac2`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs`
- Summary: bytes=21334
- After SHA-256: `af04016de7587df0cee90211227d976c2b6d24409b9b6328a359047889b7bcc2`

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f34-composition-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-composition-feed-verify/dw.tools.math.nuget-composition-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f34-composition-published-qualified.json`
- Summary: bytes=10969
- After SHA-256: `43d75b0ae7f19b60703160ecb4ebc9ae1f4bd5e8759618423b8a31098bc550b0`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 39. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 40. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

