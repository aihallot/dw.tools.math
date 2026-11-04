# RS143 Attempt 001 Payload Operations

- Operations: 38
- Changed: 6
- No change: 0
- Verified: 32
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS143 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS142/attempt-001.json` | run=RS142 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json` | pointer=/status |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json` | pointer=/package_published |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json` | pointer=/rehearsal/disposableFeedPublished |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f7-expression-published-qualified.json` | pointer=/physically_published_package_verified |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 19 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs` | file exists |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-standard-release/Program.cs` | file exists |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f10-standard-published.md` | path is absent |
| 22 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStandardPublishedFeed.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/dw.tools.math.nuget-standard-feed-verify.csproj` | path is absent |
| 24 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props` | path is absent |
| 25 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs` | path is absent |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | path is absent |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f10-standard-published.md` | bytes=5362 |
| 28 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStandardPublishedFeed.cs` | bytes=12992 |
| 29 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-feed-verify/dw.tools.math.nuget-standard-feed-verify.csproj` | bytes=331 |
| 30 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props` | bytes=241 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs` | bytes=20758 |
| 32 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f10-standard-published.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props` | semantic text found |
| 36 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f10-standard-published-qualified.json` | bytes=10357 |
| 37 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 38 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS143 hierarchy=exact
- After SHA-256: `bf3912c62295a299b8b318b571fbcb17f4af1144a942a10ec0e4d7dbfa8a9323`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS142/attempt-001.json`
- Summary: run=RS142 attempt=1 lineage=exact
- After SHA-256: `63f6ae6377b48711874e10ad1340871591d2548b86c1566a5440017df121b310`

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
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f8-standard-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json`
- Summary: pointer=/status

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json`
- Summary: pointer=/package_published

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f9-standard-publisher-qualified.json`
- Summary: pointer=/rehearsal/disposableFeedPublished

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f7-expression-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

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
- Path: `tools/dw.tools.math.nuget-standard-candidate/fixtures/StandardBinaryConsumer.cs`
- Summary: file exists

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-release/Program.cs`
- Summary: file exists

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published.md`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStandardPublishedFeed.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/dw.tools.math.nuget-standard-feed-verify.csproj`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs`
- Summary: path is absent

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: path is absent

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f10-standard-published.md`
- Summary: bytes=5362
- After SHA-256: `9e940a8281897e8b07f317b631188dcde8fd4da108dfa03311bc001cb756c895`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStandardPublishedFeed.cs`
- Summary: bytes=12992
- After SHA-256: `97f69fc09866716eb459250a70b17019ced78c3ebdb1d74dba53c7e11b5075e9`

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/dw.tools.math.nuget-standard-feed-verify.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs`
- Summary: bytes=20758
- After SHA-256: `b502f534928594c295cad1ed6b0b68f245695de94603da931721ffb9e0143b0b`

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f10-standard-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-standard-feed-verify/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f10-standard-published-qualified.json`
- Summary: bytes=10357
- After SHA-256: `cf835b6428e2ecdc60f88d3863f741ac22f2de16cb01f96f8ef0858cb4530f4d`

## 37. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 38. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

