# RS155 Attempt 001 Payload Operations

- Operations: 37
- Changed: 4
- No change: 0
- Verified: 33
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS155 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS154/attempt-001.json` | run=RS154 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/status |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/package_published |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/rehearsal/consumerChecks |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 18 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 19 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 20 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | file exists |
| 21 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj` | file exists |
| 22 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props` | file exists |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages.md` | path is absent |
| 24 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetScientificReleaseMessages.cs` | path is absent |
| 25 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | path is absent |
| 26 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f22-scientific-release-messages.md` | bytes=4054 |
| 27 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetScientificReleaseMessages.cs` | bytes=10766 |
| 28 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | bytes=28268 |
| 29 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages.md` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props` | semantic text found |
| 35 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | bytes=3322 |
| 36 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 37 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS155 hierarchy=exact
- After SHA-256: `849d52eb036b45bee4d52a8b0c7936bfc844df954ff2ec8d0e0cd758d18f7594`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS154/attempt-001.json`
- Summary: run=RS154 attempt=1 lineage=exact
- After SHA-256: `864aa2231f8eb07a569e1ed7f7f2f07d70b0c7c4ad35514cc08304e7f6e22b1d`

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
- Summary: pointer=/candidate/candidateArchiveBytes

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/status

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/package_published

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/rehearsal/consumerChecks

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 20. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: file exists

## 21. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/dw.tools.math.nuget-scientific-release.csproj`
- Summary: file exists

## 22. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props`
- Summary: file exists

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages.md`
- Summary: path is absent

## 24. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetScientificReleaseMessages.cs`
- Summary: path is absent

## 25. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: path is absent

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages.md`
- Summary: bytes=4054
- After SHA-256: `757b33d294ab21aa28cb86f2af9b25cbbb9edf44ee31fe6997d74f1f8e7b0feb`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetScientificReleaseMessages.cs`
- Summary: bytes=10766
- After SHA-256: `92d6f7e22192a85d93fd5abb64fb0584f427c4a57dd2f71379cbb8386bb72fb4`

## 28. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: bytes=28268
- Before SHA-256: `3dad3a39d3f6290c669d9f7bc7c2e561f215a9bf12fad026326305629907f394`
- After SHA-256: `5da012e8a4c6e8c43dba4aed3eace8c187a852a55ba31226c889fe6f38406734`

## 29. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages.md`
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
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: bytes=3322
- After SHA-256: `053632e15f9d9d056c58ae2d7339eb39d90548297db8b20614a2171d32616233`

## 36. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M6 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 37. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

