# RS159 Attempt 001 Payload Operations

- Operations: 39
- Changed: 4
- No change: 0
- Verified: 35
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS159 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS158/attempt-001.json` | run=RS158 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/package_published |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/packageId |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/packageVersion |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/candidateArchiveBytes |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 16 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/status |
| 17 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/package_published |
| 18 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/rehearsal/consumerChecks |
| 19 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 20 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json` | pointer=/source_files/tools~1dw.tools.math.nuget-statistics-release~1Program.cs |
| 21 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 22 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 23 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | file exists |
| 24 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj` | file exists |
| 25 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props` | file exists |
| 26 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt.md` | path is absent |
| 27 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetStatisticsReleaseReceipt.cs` | path is absent |
| 28 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | path is absent |
| 29 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f26-statistics-release-receipt.md` | bytes=3457 |
| 30 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetStatisticsReleaseReceipt.cs` | bytes=9425 |
| 31 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | bytes=33340 |
| 32 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f26-statistics-release-receipt.md` | semantic text found |
| 33 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | semantic text found |
| 34 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | semantic text found |
| 35 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Program.cs` | semantic text found |
| 36 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props` | semantic text found |
| 37 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json` | bytes=3174 |
| 38 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 39 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS159 hierarchy=exact
- After SHA-256: `64c93b4031d4baba42d418374011133935d51963254af78f63727008195f49ef`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS158/attempt-001.json`
- Summary: run=RS158 attempt=1 lineage=exact
- After SHA-256: `8ba72cf360cfcc3a435de64bb0d6d8cf2de4064afc53a683bbedc58f997b9d66`

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
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/package_published

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/packageId

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/packageVersion

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveBytes

## 15. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f24-statistics-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 16. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/status

## 17. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/package_published

## 18. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/rehearsal/consumerChecks

## 19. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 20. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f25-statistics-publisher-qualified.json`
- Summary: pointer=/source_files/tools~1dw.tools.math.nuget-statistics-release~1Program.cs

## 21. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 22. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 23. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: file exists

## 24. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/dw.tools.math.nuget-statistics-release.csproj`
- Summary: file exists

## 25. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props`
- Summary: file exists

## 26. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt.md`
- Summary: path is absent

## 27. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetStatisticsReleaseReceipt.cs`
- Summary: path is absent

## 28. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: path is absent

## 29. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt.md`
- Summary: bytes=3457
- After SHA-256: `28e08ce3c88c4ce57bd9ebb29ba0e435c7a032cb2fba0b3580e8cffe3fd7dd93`

## 30. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetStatisticsReleaseReceipt.cs`
- Summary: bytes=9425
- After SHA-256: `bb874d551838cb55cfb6a1a742c06bcb3680edd7e45dc6246974b333cbf6a4d8`

## 31. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: bytes=33340
- Before SHA-256: `d4ba2b3b00b29104cecd95a672fcce9777352ca12045276be583c6043b7bc920`
- After SHA-256: `fcb6c2436c7ad030579ed6340ed9ef835cd0b3311865dcb3cd8eef728417b1c2`

## 32. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 34. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 35. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 36. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-statistics-release/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 37. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f26-statistics-release-receipt-qualified.json`
- Summary: bytes=3174
- After SHA-256: `883babb060a9a13adb412e5166e9cd21bb55a199915cbe07153d6ffa69d76fb1`

## 38. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 39. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

