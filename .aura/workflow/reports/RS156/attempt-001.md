# RS156 Attempt 001 Payload Operations

- Operations: 44
- Changed: 5
- No change: 0
- Verified: 39
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS156 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS155/attempt-001.json` | run=RS155 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M6 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/packageDependencies |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f20-scientific-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/status |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/package_published |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/rehearsal/disposableFeedPublished |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json` | pointer=/rehearsal/consumerChecks |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | pointer=/status |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | pointer=/release_controller_compiled |
| 21 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | pointer=/release_modes_invoked |
| 22 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | pointer=/operator_message_archive_bytes |
| 23 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json` | pointer=/operator_message_binary_checks |
| 24 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 25 | `require.text-contains` | Verified | `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj` | semantic text found |
| 26 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs` | file exists |
| 27 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-scientific-release/Program.cs` | file exists |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f23-scientific-published.md` | path is absent |
| 29 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetScientificPublishedFeed.cs` | path is absent |
| 30 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj` | path is absent |
| 31 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs` | path is absent |
| 32 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | path is absent |
| 33 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f23-scientific-published.md` | bytes=4432 |
| 34 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetScientificPublishedFeed.cs` | bytes=12479 |
| 35 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj` | bytes=281 |
| 36 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs` | bytes=15552 |
| 37 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f23-scientific-published.md` | semantic text found |
| 38 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs` | semantic text found |
| 39 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs` | semantic text found |
| 40 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs` | semantic text found |
| 41 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj` | semantic text found |
| 42 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f23-scientific-published-qualified.json` | bytes=9304 |
| 43 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 44 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS156 hierarchy=exact
- After SHA-256: `b05fc1913d7346e4993283b6ecc6a91621290e80f511923de972d57ed4b05365`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS155/attempt-001.json`
- Summary: run=RS155 attempt=1 lineage=exact
- After SHA-256: `abefc8bc276a586e1f6770f8b74689d8053025ee6e0076a6e6501b394a8341a3`

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
- Summary: pointer=/candidate/candidateArchiveBytes

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
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/status

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/package_published

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/rehearsal/disposableFeedPublished

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f21-scientific-publisher-qualified.json`
- Summary: pointer=/rehearsal/consumerChecks

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: pointer=/status

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: pointer=/release_controller_compiled

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: pointer=/release_modes_invoked

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: pointer=/operator_message_archive_bytes

## 23. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f22-scientific-release-messages-qualified.json`
- Summary: pointer=/operator_message_binary_checks

## 24. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 25. `require.text-contains`

- Status: `Verified`
- Path: `src/projects/dw.tools.math.scientific/dw.tools.math.scientific.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-candidate/fixtures/ScientificBinaryConsumer.cs`
- Summary: file exists

## 27. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-release/Program.cs`
- Summary: file exists

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published.md`
- Summary: path is absent

## 29. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetScientificPublishedFeed.cs`
- Summary: path is absent

## 30. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj`
- Summary: path is absent

## 31. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs`
- Summary: path is absent

## 32. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: path is absent

## 33. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f23-scientific-published.md`
- Summary: bytes=4432
- After SHA-256: `0c981c6a928b27f59dd29e67a0f0f4e2c55db05af4e8ca7fdc3248fb27bf4d98`

## 34. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetScientificPublishedFeed.cs`
- Summary: bytes=12479
- After SHA-256: `5bfe6b2e7e51f829aab4185a26877260a9be01ae4eda6bf788f7986e3f379fef`

## 35. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 36. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs`
- Summary: bytes=15552
- After SHA-256: `adfce36eb554199b1959d8bb8a71f4fd32bbe2e38f9ef01bbaf56cc23286ae62`

## 37. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f23-scientific-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 38. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 39. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 40. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 41. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-scientific-feed-verify/dw.tools.math.nuget-scientific-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 42. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f23-scientific-published-qualified.json`
- Summary: bytes=9304
- After SHA-256: `3cd3edf5f9b91ff6b10ec9e465114c7a2aedec9b024f0af894bcb080688ba74c`

## 43. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 44. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

