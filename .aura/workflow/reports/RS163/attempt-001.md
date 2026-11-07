# RS163 Attempt 001 Payload Operations

- Operations: 36
- Changed: 5
- No change: 0
- Verified: 31
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS163 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS162/attempt-001.json` | run=RS162 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json` | pointer=/rehearsal/publishedToNugetLocal |
| 14 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 15 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 16 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 17 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-codecs-release/Program.cs` | file exists |
| 18 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs` | file exists |
| 19 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f30-codecs-published.md` | path is absent |
| 20 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetCodecsPublishedFeed.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj` | path is absent |
| 22 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs` | path is absent |
| 23 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | path is absent |
| 24 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f30-codecs-published.md` | bytes=4153 |
| 25 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetCodecsPublishedFeed.cs` | bytes=12514 |
| 26 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj` | bytes=281 |
| 27 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs` | bytes=19501 |
| 28 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f30-codecs-published.md` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs` | semantic text found |
| 31 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs` | semantic text found |
| 32 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj` | semantic text found |
| 33 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f30-codecs-published-qualified.json` | bytes=10293 |
| 34 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 35 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 36 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS163 hierarchy=exact
- After SHA-256: `4cbbe9ad59b7a3d3a3c29a500f34f65ab634d24384d6d33275d7103cbfcc213b`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS162/attempt-001.json`
- Summary: run=RS162 attempt=1 lineage=exact
- After SHA-256: `4ba2af1638911b8b687dfc5f1ca74a2804f665cb1a8e45e94aaf2d5505ee99e6`

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
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f28-codecs-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f29-codecs-publisher-qualified.json`
- Summary: pointer=/rehearsal/publishedToNugetLocal

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

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
- Path: `tools/dw.tools.math.nuget-codecs-release/Program.cs`
- Summary: file exists

## 18. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-candidate/fixtures/CodecsBinaryConsumer.cs`
- Summary: file exists

## 19. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f30-codecs-published.md`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetCodecsPublishedFeed.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj`
- Summary: path is absent

## 22. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs`
- Summary: path is absent

## 23. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: path is absent

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f30-codecs-published.md`
- Summary: bytes=4153
- After SHA-256: `156ad42125a2e45436a6a3b1d38104cecd39bdf7200c3c588a34bd462049f5e0`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetCodecsPublishedFeed.cs`
- Summary: bytes=12514
- After SHA-256: `f7b272081e03932a211f8eac16162e23b55318ca031e2cab71eb275918218156`

## 26. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 27. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs`
- Summary: bytes=19501
- After SHA-256: `9403559ae074164577dd75602a7331f3b660f7cad6b57c181870da2d4ba54b10`

## 28. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f30-codecs-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 32. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-codecs-feed-verify/dw.tools.math.nuget-codecs-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 33. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f30-codecs-published-qualified.json`
- Summary: bytes=10293
- After SHA-256: `1ed8f7fd424c65f9636dde3227ca56f89b7152b6cf921a711e168662924f3d67`

## 34. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 35. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 36. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

