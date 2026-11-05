# RS154 Attempt 001 Payload Operations

- Operations: 36
- Changed: 6
- No change: 0
- Verified: 30
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS154 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS153/attempt-001.json` | run=RS153 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f19-foundation-published-qualified.json` | pointer=/physically_published_package_verified |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs` | file exists |
| 18 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher.md` | path is absent |
| 19 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetScientificPublisher.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f21-scientific-publisher.md` | bytes=4265 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetScientificPublisher.cs` | bytes=10047 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj` | bytes=331 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props` | bytes=241 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | bytes=28266 |
| 29 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props` | semantic text found |
| 34 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | bytes=11887 |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS154 hierarchy=exact
- After SHA-256: `810920f1390908f3de2c1bd9b2b8fd82e549b3e6eb793b4e67a80110ad3caebf`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS153/attempt-001.json`
- Summary: run=RS153 attempt=1 lineage=exact
- After SHA-256: `b140c892bc1a9557e62e58e9fd97dcafb014c1fd2ec018c5cabe81caf72a4cb5`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
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
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/package_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/packageDependencies

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f19-foundation-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs`
- Summary: file exists

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher.md`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetScientificPublisher.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher.md`
- Summary: bytes=4265
- After SHA-256: `5bd88419d46cac2443b1f25982a966e305e3496615ba338a6e6d851753c98d40`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetScientificPublisher.cs`
- Summary: bytes=10047
- After SHA-256: `22656d49cf0aaa0fa0c5709d100ce144d405f56b5da17b309c6b12192c69fcdb`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: bytes=28266
- After SHA-256: `3dad3a39d3f6290c669d9f7bc7c2e561f215a9bf12fad026326305629907f394`

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: bytes=11887
- After SHA-256: `d7bf3f2c6c5e1f910b49b4da7c0c2d3fd403daee4811fc52a3b27f48bc6cca42`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

