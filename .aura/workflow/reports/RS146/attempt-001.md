# RS146 Attempt 001 Payload Operations

- Operations: 33
- Changed: 5
- No change: 0
- Verified: 28
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS146 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS145/attempt-001.json` | run=RS145 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json` | pointer=/status |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f4-quantities-published-qualified.json` | pointer=/physically_published_package_verified |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 15 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs` | file exists |
| 16 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-ir-release/Program.cs` | file exists |
| 17 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f13-ir-published.md` | path is absent |
| 18 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetIrPublishedFeed.cs` | path is absent |
| 19 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj` | path is absent |
| 20 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs` | path is absent |
| 21 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | path is absent |
| 22 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f13-ir-published.md` | bytes=4030 |
| 23 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetIrPublishedFeed.cs` | bytes=11283 |
| 24 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj` | bytes=281 |
| 25 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs` | bytes=16779 |
| 26 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f13-ir-published.md` | semantic text found |
| 27 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs` | semantic text found |
| 28 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs` | semantic text found |
| 29 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs` | semantic text found |
| 30 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj` | semantic text found |
| 31 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | bytes=9274 |
| 32 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 33 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS146 hierarchy=exact
- After SHA-256: `8ebdf0ad0998974737650186f1f4babc76b33e088eed8f90a034531ff8d0608e`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS145/attempt-001.json`
- Summary: run=RS145 attempt=1 lineage=exact
- After SHA-256: `ac50efcc9ba2629cf6da77ad1722f0036e973b18626f6e0d3de02d4461883162`

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
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f11-ir-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json`
- Summary: pointer=/status

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f12-ir-publisher-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f4-quantities-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 15. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-candidate/fixtures/IrBinaryConsumer.cs`
- Summary: file exists

## 16. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-release/Program.cs`
- Summary: file exists

## 17. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published.md`
- Summary: path is absent

## 18. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetIrPublishedFeed.cs`
- Summary: path is absent

## 19. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj`
- Summary: path is absent

## 20. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs`
- Summary: path is absent

## 21. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: path is absent

## 22. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f13-ir-published.md`
- Summary: bytes=4030
- After SHA-256: `aab28cd2cf00cdf61ec1776ad79cedd19b9dc3d365fb453692dc526dc26eaf37`

## 23. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetIrPublishedFeed.cs`
- Summary: bytes=11283
- After SHA-256: `045cdf354e9edcd8d7941004d5d4e936acde1d350825b88ca07626bb1526258d`

## 24. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj`
- Summary: bytes=281
- After SHA-256: `ab3afa6c5f719b0ecc23e80f773a34d14ea487140140f3902d03ddce2d14537c`

## 25. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs`
- Summary: bytes=16779
- After SHA-256: `e2d78528225a4a90eb1503f280e023b4d9fe3374a3f24b4bbcd76ce93d39f489`

## 26. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 28. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 29. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 30. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-ir-feed-verify/dw.tools.math.nuget-ir-feed-verify.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 31. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: bytes=9274
- After SHA-256: `ff6a66a1b665733b2f3934846361898436ade06b1347b6c9bcb37c01e8924de9`

## 32. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 33. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

