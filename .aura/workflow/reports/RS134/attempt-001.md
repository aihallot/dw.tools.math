# RS134 Attempt 001 Payload Operations

- Operations: 27
- Changed: 6
- No change: 0
- Verified: 21
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS134 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS133/attempt-001.json` | run=RS133 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M3 state=done |
| 5 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |
| 7 | `require.json-value-equals` | Verified | `docs/workflow/manifest.json` | pointer=/guidanceVersion |
| 8 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/package_from_dw_tools_math |
| 9 | `require.json-value-equals` | Verified | `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json` | pointer=/authorization/redistribute_from_dw_tools_math |
| 10 | `require.json-value-equals` | Verified | `docs/discovery/qualification/RS133-neutral-candidate-qualified.json` | pointer=/neutral_migration_qualified |
| 11 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f1.md` | path is absent |
| 12 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | path is absent |
| 13 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-preflight/Directory.Packages.props` | path is absent |
| 14 | `require.file-missing` | Verified | `tools/dw.tools.math.nuget-preflight/Program.cs` | path is absent |
| 15 | `require.file-missing` | Verified | `docs/distribution/ValidateNugetFederationF1.cs` | path is absent |
| 16 | `require.file-missing` | Verified | `docs/distribution/nuget-federation-f1-qualified.json` | path is absent |
| 17 | `files.replace-from-staged` | Changed | `docs/distribution/nuget-federation-f1.md` | bytes=6509 |
| 18 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj` | bytes=331 |
| 19 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-preflight/Directory.Packages.props` | bytes=241 |
| 20 | `files.replace-from-staged` | Changed | `tools/dw.tools.math.nuget-preflight/Program.cs` | bytes=16750 |
| 21 | `files.replace-from-staged` | Changed | `docs/distribution/ValidateNugetFederationF1.cs` | bytes=6780 |
| 22 | `require.text-contains` | Verified | `docs/distribution/nuget-federation-f1.md` | semantic text found |
| 23 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-preflight/Program.cs` | semantic text found |
| 24 | `require.text-contains` | Verified | `tools/dw.tools.math.nuget-preflight/Directory.Packages.props` | semantic text found |
| 25 | `files.write-complete` | Changed | `docs/distribution/nuget-federation-f1-qualified.json` | bytes=15335 |
| 26 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M7 state=done |
| 27 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/status |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS134 hierarchy=exact
- After SHA-256: `d1f5695ff86f9d4f013414a2b8275e13928b4d065b156039aa8b7d4582c3c3f0`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS133/attempt-001.json`
- Summary: run=RS133 attempt=1 lineage=exact
- After SHA-256: `1a60be413fbac5ca4907bd0e5ace210d7a48a0f01f53c82017b0267663354c73`

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
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/package_from_dw_tools_math

## 9. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/evidence/M0-W02-C01-owner-rights-attestation.json`
- Summary: pointer=/authorization/redistribute_from_dw_tools_math

## 10. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/discovery/qualification/RS133-neutral-candidate-qualified.json`
- Summary: pointer=/neutral_migration_qualified

## 11. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1.md`
- Summary: path is absent

## 12. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: path is absent

## 13. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/Directory.Packages.props`
- Summary: path is absent

## 14. `require.file-missing`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/Program.cs`
- Summary: path is absent

## 15. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/ValidateNugetFederationF1.cs`
- Summary: path is absent

## 16. `require.file-missing`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: path is absent

## 17. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f1.md`
- Summary: bytes=6509
- After SHA-256: `58e13bdca8eb2299e5ec80957020338a475b19846abceaa56f7b0c53ded2f296`

## 18. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-preflight/dw.tools.math.nuget-preflight.csproj`
- Summary: bytes=331
- After SHA-256: `5b6790df82d14838a958d40826dc27321ed4f171a08843ab726067c9048a548f`

## 19. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-preflight/Directory.Packages.props`
- Summary: bytes=241
- After SHA-256: `42b8b5536d24cc3e60b1832f627c9e82422ae63634b0af47e4071a235bada554`

## 20. `files.replace-from-staged`

- Status: `Changed`
- Path: `tools/dw.tools.math.nuget-preflight/Program.cs`
- Summary: bytes=16750
- After SHA-256: `5a49800bc40f232cd3b347cba0968bcf1d57f2b6d79fff01765dc405e5e03c92`

## 21. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/distribution/ValidateNugetFederationF1.cs`
- Summary: bytes=6780
- After SHA-256: `d03d51d5f6873c2dbba7f0b2a7f7f99ed5f59e4d211796de08f02f5b37fa924e`

## 22. `require.text-contains`

- Status: `Verified`
- Path: `docs/distribution/nuget-federation-f1.md`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 23. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/Program.cs`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 24. `require.text-contains`

- Status: `Verified`
- Path: `tools/dw.tools.math.nuget-preflight/Directory.Packages.props`
- Summary: semantic text found
- Details: comparison=Ordinal normalizeLineEndings=False trimOuterWhitespace=False

## 25. `files.write-complete`

- Status: `Changed`
- Path: `docs/distribution/nuget-federation-f1-qualified.json`
- Summary: bytes=15335
- After SHA-256: `07c40ad32e19f9197207533f12a2f0db37b55084f61f2a733a4ecc776c920291`

## 26. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M7 state=done
- After SHA-256: `c28fc9a86b1ff97e0ba2b4ee0f6149cb5a610b934cc38fe7f3c175ac6b82ab0f`

## 27. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/status

