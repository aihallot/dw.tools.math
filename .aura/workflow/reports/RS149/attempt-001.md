# RS149 Attempt 001 Payload Operations

- Operations: 38
- Changed: 5
- No change: 0
- Verified: 33
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS149 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS148/attempt-001.json` | run=RS148 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | pointer=/status |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | pointer=/package_published |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | pointer=/rehearsal/disposableFeedPublished |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 19 | `require.text-contains` | Verified | `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj` | semantic text found |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs` | file exists |
| 21 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-analysis-release/Program.cs` | file exists |
| 22 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f16-analysis-published.md` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetAnalysisPublishedFeed.cs` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | path is absent |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f16-analysis-published.md` | bytes=3957 |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetAnalysisPublishedFeed.cs` | bytes=10476 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj` | bytes=281 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs` | bytes=14482 |
| 31 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f16-analysis-published.md` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj` | semantic text found |
| 36 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | bytes=9025 |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS149 hierarchy=exact
- After SHA-256: `0649129ac1ef94b4b3ee08942f04273cad79f36f81b0063475dbe43157edca98`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS148/attempt-001.json`
- Summary: run=RS148 attempt=1 lineage=exact
- After SHA-256: `e05c25cbc18f79514ad06b390c198f508c30fce16bb4b89cca38ef153a36f089`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
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
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f14-analysis-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: pointer=/status

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: pointer=/package_published

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: pointer=/rehearsal/disposableFeedPublished

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f15-analysis-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 19. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.analysis/dw.tools.math.analysis.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-candidate/fixtures/AnalysisBinaryConsumer.cs`
- Summary: file exists

## 21. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-release/Program.cs`
- Summary: file exists

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published.md`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetAnalysisPublishedFeed.cs`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: path is absent

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f16-analysis-published.md`
- Summary: bytes=3957
- After SHA-256: `c9e1e6b6f80e49ae31d5516200500a78748a4a9cc3879bc4546fa6508777e0fe`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetAnalysisPublishedFeed.cs`
- Summary: bytes=10476
- After SHA-256: `53a3329dd43580e9669c11c1275633ec1e7d732b53db851bbabcdcad80b294bc`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs`
- Summary: bytes=14482
- After SHA-256: `c61a3b130d10789f59cf6497b74bae5aab926b630c2f656da46432d1cd8b884a`

## 31. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f16-analysis-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-analysis-feed-verify/dw.tools.math.nuget-analysis-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: bytes=9025
- After SHA-256: `d22b315f79b77a4e84a24ee2361379c1614811a3adefb4f78a8c2c8e0e272ed3`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

