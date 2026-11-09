# RS169 Attempt 002 Payload Operations

- Operations: 28
- Changed: 3
- No change: 3
- Verified: 22
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS169 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS168/attempt-003.json` | run=RS168 attempt=3 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/status |
| 9 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateArchiveSha256 |
| 10 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/candidateDllSha256 |
| 11 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/candidate/compiledBinaryConsumerChecks |
| 12 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json` | pointer=/package_published |
| 13 | `require.json-value-equals` | Verified | `docs/distribution/nuget-federation-f13-ir-published-qualified.json` | pointer=/physically_published_package_verified |
| 14 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 15 | `require.file-exists` | Verified | `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs` | file exists |
| 16 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json` | path is absent |
| 17 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f36-numerics-rehearsal.md` | bytes=3394 |
| 18 | `files.replace-from-staged` | NoChange | `docs/distribution/ValidateNugetNumericsRehearsal.cs` | staged content already installed |
| 19 | `files.replace-from-staged` | NoChange | `tools/dw.tools.math.nuget-numerics-rehearsal/dw.tools.math.nuget-numerics-rehearsal.csproj` | staged content already installed |
| 20 | `files.replace-from-staged` | NoChange | `tools/dw.tools.math.nuget-numerics-rehearsal/Directory.Packages.props` | staged content already installed |
| 21 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs` | bytes=22104 |
| 22 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f36-numerics-rehearsal.md` | semantic text found |
| 23 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs` | semantic text found |
| 24 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs` | semantic text found |
| 25 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs` | semantic text found |
| 26 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-numerics-rehearsal/dw.tools.math.nuget-numerics-rehearsal.csproj` | semantic text found |
| 27 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json` | bytes=12192 |
| 28 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS169 hierarchy=exact
- After SHA-256: `fdb95fa12cf8081c5145ef05a073f913df4d7a4d63598e0e0ddc9f30e8a61d8c`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS168/attempt-003.json`
- Summary: run=RS168 attempt=3 lineage=exact
- After SHA-256: `cd5c1494d43f119228faa53d14926084c1fe978237f5173a47a729224081b5cb`

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
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/status

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateArchiveSha256

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/candidateDllSha256

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/candidate/compiledBinaryConsumerChecks

## 12. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json`
- Summary: pointer=/package_published

## 13. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f13-ir-published-qualified.json`
- Summary: pointer=/physically_published_package_verified

## 14. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 15. `require.file-exists`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-candidate/fixtures/NumericsBinaryConsumer.cs`
- Summary: file exists

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json`
- Summary: path is absent

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal.md`
- Summary: bytes=3394
- Before SHA-256: `c85e2b04a9d9e0cb07588e731b0a90883ac0d934c680fb2a6b30b62079f66afc`
- After SHA-256: `90518abfe2e5cfacf5777e5b8c42e3fb77b537c0d9f5a1adfc3d03999a4723d8`

## 18. `files.replace-from-staged`

- Status: `NoChange`
- Path: `docs/distribution/ValidateNugetNumericsRehearsal.cs`
- Summary: staged content already installed
- Before SHA-256: `f56e7978aa653060aa07b0a9421573622b1dd78a228f60f995bf39f9cb60d1cd`
- After SHA-256: `f56e7978aa653060aa07b0a9421573622b1dd78a228f60f995bf39f9cb60d1cd`

## 19. `files.replace-from-staged`

- Status: `NoChange`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/dw.tools.math.nuget-numerics-rehearsal.csproj`
- Summary: staged content already installed
- Before SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 20. `files.replace-from-staged`

- Status: `NoChange`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/Directory.Packages.props`
- Summary: staged content already installed
- Before SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs`
- Summary: bytes=22104
- Before SHA-256: `c5d75a791dea0a8cba8dc3e0ba2850deeb7159a93fee746aa87ce70064d69a77`
- After SHA-256: `d194dc6cd1bd207f6732863bb57d3a50996e360958baac9fb99a37e312882fca`

## 22. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 26. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-numerics-rehearsal/dw.tools.math.nuget-numerics-rehearsal.csproj`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 27. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f36-numerics-rehearsal-qualified.json`
- Summary: bytes=12192
- After SHA-256: `e3bc9f68d5c51b547e92adcb0f763ddaf806fecb40392d3472d90d84710c8add`

## 28. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M3 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

