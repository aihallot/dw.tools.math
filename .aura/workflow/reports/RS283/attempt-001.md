# RS283 Attempt 001 Payload Operations

- Operations: 8
- Changed: 2
- No change: 0
- Verified: 6
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS283 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS282/attempt-001.json` | run=RS282 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=not-ready |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 6 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 7 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs` | bytes=12271 |
| 8 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-portable-discovery-bundle.md` | bytes=5300 |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS283 hierarchy=exact
- After SHA-256: `8b89d26b31cae95bc5a83e95f7fbdc83aa500aef06ec65bc0ef2fe1524ef88f8`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS282/attempt-001.json`
- Summary: run=RS282 attempt=1 lineage=exact
- After SHA-256: `debc4ab437bd3a4e287a17968443d79f2aa8ae6cc9cd05362f8d10138599e4ef`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=not-ready
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `32cd0df1cbe6ca643e7a3bf17cbf9b4d4fe2a6f5a3082bcb77a5449205fcfcd9`

## 6. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 7. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/AttestM8PortableDiscoveryExport.cs`
- Summary: bytes=12271
- After SHA-256: `eec2143e27d0676e535e136d96b0605c1ea86bca63d1ecd8bc21df1460486509`

## 8. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-portable-discovery-bundle.md`
- Summary: bytes=5300
- Before SHA-256: `00268c31844f86c28a095d2ee1221c5cd8938111cb1aa1f61416d03dd281dfbe`
- After SHA-256: `04b8a727b6c6f3e22ed20f5b008c30c2d136b011ee84dceaaef96526a42a6c1e`

