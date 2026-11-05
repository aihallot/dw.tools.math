# RS152 Attempt 001 Payload Operations

- Operations: 38
- Changed: 5
- No change: 0
- Verified: 33
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS152 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS151/attempt-001.json` | run=RS151 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M0 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | pointer=/status |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | pointer=/package_published |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | pointer=/rehearsal/disposableFeedPublished |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 19 | `require.text-contains` | Verified | `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj` | semantic text found |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs` | file exists |
| 21 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | file exists |
| 22 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f19-foundation-published.md` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetFoundationPublishedFeed.cs` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | path is absent |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f19-foundation-published.md` | bytes=4311 |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetFoundationPublishedFeed.cs` | bytes=10541 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj` | bytes=281 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs` | bytes=14521 |
| 31 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f19-foundation-published.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj` | semantic text found |
| 36 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | bytes=8803 |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS152 hierarchy=exact
- After SHA-256: `bc4d912c0991feb3ff85e7f6caaed1ba1a13c222054ef6d4272c1d6828dd7f02`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS151/attempt-001.json`
- Summary: run=RS151 attempt=1 lineage=exact
- After SHA-256: `3185dc0c5dc800dcddd610f7995b0282e3f3d312e7993c8342de691b6edc4df9`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M0 state=done
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
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: pointer=/status

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: pointer=/package_published

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: pointer=/rehearsal/disposableFeedPublished

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 19. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs`
- Summary: file exists

## 21. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: file exists

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published.md`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetFoundationPublishedFeed.cs`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: path is absent

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f19-foundation-published.md`
- Summary: bytes=4311
- After SHA-256: `62c2e006aa7a42450f207a8f00adebeda0966cfb43b6885a0fceb12d4526d494`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetFoundationPublishedFeed.cs`
- Summary: bytes=10541
- After SHA-256: `b09432d947e4e1a30a35a6f69b0e04cecd99933aab267a4c36d242430b9e8649`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs`
- Summary: bytes=14521
- After SHA-256: `518102f6dc5d33b1837c4437d35116e6385dc4981972c2495a3a721af63b1f6c`

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-feed-verify/dw.tools.math.nuget-foundation-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: bytes=8803
- After SHA-256: `bb4f7f8890075144e6c7ed59a808640d8df138afdebbf315ca6bcd67b96e837e`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

