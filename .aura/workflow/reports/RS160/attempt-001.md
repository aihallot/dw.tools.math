# RS160 Attempt 001 Payload Operations

- Operations: 37
- Changed: 5
- No change: 0
- Verified: 32
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS160 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS159/attempt-001.json` | run=RS159 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | pointer=/status |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | pointer=/release_controller_compiled |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | pointer=/package_published_by_this_run |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | pointer=/original_archive_sha256 |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 19 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs` | file exists |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | file exists |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f27-statistics-published.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStatisticsPublishedFeed.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs` | path is absent |
| 25 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | path is absent |
| 26 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f27-statistics-published.md` | bytes=5176 |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStatisticsPublishedFeed.cs` | bytes=13126 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj` | bytes=281 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs` | bytes=18397 |
| 30 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f27-statistics-published.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj` | semantic text found |
| 35 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f27-statistics-published-qualified.json` | bytes=9863 |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS160 hierarchy=exact
- After SHA-256: `4f496ce750468c38432a94f3c59cc638686a4b065f568f5ec9aee71d3a318f64`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS159/attempt-001.json`
- Summary: run=RS159 attempt=1 lineage=exact
- After SHA-256: `aeeb66222dcd71a3293577ecd98b8af214cc86c9668259c6741e9ca86578ee1e`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
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
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: pointer=/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: pointer=/release_controller_compiled

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: pointer=/package_published_by_this_run

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: pointer=/original_archive_sha256

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 19. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-candidate/fixtures/StatisticsBinaryConsumer.cs`
- Summary: file exists

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: file exists

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStatisticsPublishedFeed.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
- Summary: path is absent

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f27-statistics-published.md`
- Summary: bytes=5176
- After SHA-256: `fe5e798ff3ef6ba2dd17412d4d9b252e6953f98bdd37df1ee9ebd51ebf92e517`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStatisticsPublishedFeed.cs`
- Summary: bytes=13126
- After SHA-256: `681baa039d1efa38044c3b78acbcdf05e94ecd4d9e3a8e22396b31029c165dd3`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs`
- Summary: bytes=18397
- After SHA-256: `da46f0170a4a1ed01870f140d4b92d34988f3c61369a2383d0a3c7bcdb1388bb`

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f27-statistics-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-feed-verify/dw.tools.math.nuget-statistics-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f27-statistics-published-qualified.json`
- Summary: bytes=9863
- After SHA-256: `9c5e107201f6fe376828e4042d9f61485c2c5ab655b846275fe895472ea2945a`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

