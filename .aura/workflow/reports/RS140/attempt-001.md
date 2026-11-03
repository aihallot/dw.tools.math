# RS140 Attempt 001 Payload Operations

- Operations: 36
- Changed: 6
- No change: 0
- Verified: 30
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS140 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS139/attempt-001.json` | run=RS139 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/packageId |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json` | pointer=/candidate/exactBasePackageVersion |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json` | pointer=/status |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json` | pointer=/package_published |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs` | file exists |
| 18 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-expression-release/Program.cs` | file exists |
| 19 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f7-expression-published.md` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetExpressionPublishedFeed.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/dw.tools.math.nuget-expression-feed-verify.csproj` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs` | path is absent |
| 24 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | path is absent |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f7-expression-published.md` | bytes=5209 |
| 26 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetExpressionPublishedFeed.cs` | bytes=11321 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-feed-verify/dw.tools.math.nuget-expression-feed-verify.csproj` | bytes=331 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props` | bytes=241 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs` | bytes=18078 |
| 30 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f7-expression-published.md` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props` | semantic text found |
| 34 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | bytes=9570 |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS140 hierarchy=exact
- After SHA-256: `aa35289f8926af09926b87827ff0255700205fd20ceff188d952d1f73db49fd9`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS139/attempt-001.json`
- Summary: run=RS139 attempt=1 lineage=exact
- After SHA-256: `4a552eb13a52f19ca3bbda2c64a73ac8ea951fdee7b1efe9c82f6decfbb0a796`

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
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f5-expression-candidate-qualified.json`
- Summary: pointer=/candidate/exactBasePackageVersion

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json`
- Summary: pointer=/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f6-expression-publisher-qualified.json`
- Summary: pointer=/package_published

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 17. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-candidate/fixtures/ExpressionBinaryConsumer.cs`
- Summary: file exists

## 18. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-release/Program.cs`
- Summary: file exists

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published.md`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetExpressionPublishedFeed.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/dw.tools.math.nuget-expression-feed-verify.csproj`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: path is absent

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f7-expression-published.md`
- Summary: bytes=5209
- After SHA-256: `86aa6c8adf6210a7b9adde859f548bd28378a334356a3a3b9adc32c2a0ff4063`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetExpressionPublishedFeed.cs`
- Summary: bytes=11321
- After SHA-256: `8d8c0030e8725ad3cb1cb0c2efde3adcdccc81a70d8afacbbc196fe38a194248`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/dw.tools.math.nuget-expression-feed-verify.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs`
- Summary: bytes=18078
- After SHA-256: `0163e9e42edbeaabde4783b002e9dda2664d67af92d94082b86e5427c9b1b0e9`

## 30. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-expression-feed-verify/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: bytes=9570
- After SHA-256: `4e0d2b1dffc5f8d4a2ba19a9cb13058d49ad974e54df5d61c6298649cbb071c1`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

