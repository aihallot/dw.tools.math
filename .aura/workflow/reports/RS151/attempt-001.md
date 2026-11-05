# RS151 Attempt 001 Payload Operations

- Operations: 36
- Changed: 6
- No change: 0
- Verified: 30
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS151 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS150/attempt-001.json` | run=RS150 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M0 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f17-foundation-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f16-analysis-published-qualified.json` | pointer=/physically_published_package_verified |
| 15 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 16 | `require.text-contains` | Verified | `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj` | semantic text found |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs` | file exists |
| 18 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher.md` | path is absent |
| 19 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetFoundationPublisher.cs` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-release/dw.tools.math.nuget-foundation-release.csproj` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f18-foundation-publisher.md` | bytes=5991 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetFoundationPublisher.cs` | bytes=10033 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-release/dw.tools.math.nuget-foundation-release.csproj` | bytes=331 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props` | bytes=241 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | bytes=27480 |
| 29 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f18-foundation-publisher.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-release/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props` | semantic text found |
| 34 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json` | bytes=11380 |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS151 hierarchy=exact
- After SHA-256: `03aa29807a5321361037e1372fe7c4396792a120a2a7736df219a5297419321a`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS150/attempt-001.json`
- Summary: run=RS150 attempt=1 lineage=exact
- After SHA-256: `02bb5e539afb48e8d165e8a84744b5b4ca9a408101378310821609d659dfad70`

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
- Summary: pointer=/package_published

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
- Path: `docs/distribution/nuget-federation-f16-analysis-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 16. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.foundation/dw.tools.math.foundation.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 17. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-candidate/fixtures/FoundationBinaryConsumer.cs`
- Summary: file exists

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher.md`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetFoundationPublisher.cs`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/dw.tools.math.nuget-foundation-release.csproj`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher.md`
- Summary: bytes=5991
- After SHA-256: `32147b2a8087db3aa972b95c7484f1f3bf26f80aa285d35ed15fa212f2a37eca`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetFoundationPublisher.cs`
- Summary: bytes=10033
- After SHA-256: `cbc3543f2cc6296cb14ef4683f94d3ceff24c5bbc1924bf3e74a9864f6f00184`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-release/dw.tools.math.nuget-foundation-release.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: bytes=27480
- After SHA-256: `434201cd0b685e2d470de91eb6d965333c2c2fa96285b38ac71de8be9d5e5fd0`

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-foundation-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f18-foundation-publisher-qualified.json`
- Summary: bytes=11380
- After SHA-256: `fd853cdacee8f9c0afaac376561d199ced402dbd23633f31e92dda7e514412d6`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

