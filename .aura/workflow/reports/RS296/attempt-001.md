# RS296 Attempt 001 Payload Operations

- Operations: 17
- Changed: 6
- No change: 0
- Verified: 11
- Artifacts: 0

| # | Operation | Status | Path | Summary |
|---:|---|---|---|---|
| 1 | `workflow.require-current-run` | Verified | `.aura/workflow/state.json` | run=RS296 hierarchy=exact |
| 2 | `workflow.require-prior-success` | Verified | `.aura/workflow/reports/RS295/attempt-001.json` | run=RS295 attempt=1 lineage=exact |
| 3 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8 state=in-progress |
| 4 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T1 state=done |
| 5 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2 state=in-progress |
| 6 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2-R state=done |
| 7 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C01-T2-V state=not-ready |
| 8 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W01-C02 state=not-ready |
| 9 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M8-W02 state=not-ready |
| 10 | `project-plan.require-node-state` | Verified | `.aura/workflow/plan/project.json` | node=M9 state=not-ready |
| 11 | `require.json-value-equals` | Verified | `docs/planning/backlog.json` | pointer=/plan_version |
| 12 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ProbeM8SelectedClrGreenConsumer.cs` | bytes=5607 |
| 13 | `files.replace-from-staged` | Changed | `docs/planning/ValidateNativeDwfAdoption.cs` | bytes=11991 |
| 14 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs` | bytes=13512 |
| 15 | `files.replace-from-staged` | Changed | `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md` | bytes=27362 |
| 16 | `files.replace-from-staged` | Changed | `docs/planning/decisions/m8-w01-c01-t2-green-independent-admission-rs296.md` | bytes=1858 |
| 17 | `project-plan.activate-ready-continuation` | Changed | `.aura/workflow/plan/project.json` | leaf=M8-W01-C01-T2-G activated=M8-W01-C01-T2-G |

## 1. `workflow.require-current-run`

- Status: `Verified`
- Path: `.aura/workflow/state.json`
- Summary: run=RS296 hierarchy=exact
- After SHA-256: `a3dffb6e6263645b2718630ca2ee2767b5edd9657ec1df15d05b740154659c5d`

## 2. `workflow.require-prior-success`

- Status: `Verified`
- Path: `.aura/workflow/reports/RS295/attempt-001.json`
- Summary: run=RS295 attempt=1 lineage=exact
- After SHA-256: `84d13ac863e323cdec0da1dbad941c52c3b9becd5133a114e211ce097b205f74`

## 3. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8 state=in-progress
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 4. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T1 state=done
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 5. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2 state=in-progress
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 6. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2-R state=done
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 7. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C01-T2-V state=not-ready
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 8. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W01-C02 state=not-ready
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 9. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M8-W02 state=not-ready
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 10. `project-plan.require-node-state`

- Status: `Verified`
- Path: `.aura/workflow/plan/project.json`
- Summary: node=M9 state=not-ready
- After SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`

## 11. `require.json-value-equals`

- Status: `Verified`
- Path: `docs/planning/backlog.json`
- Summary: pointer=/plan_version

## 12. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ProbeM8SelectedClrGreenConsumer.cs`
- Summary: bytes=5607
- After SHA-256: `929550fa2d195fbd106595404748ebd186358e35690f12117244f7b73e0415ec`

## 13. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/ValidateNativeDwfAdoption.cs`
- Summary: bytes=11991
- Before SHA-256: `e414c35ded50358457d9c5be1ebee53358121534e2a1b881f43e9bd3ef621b16`
- After SHA-256: `566ec85edfac0cfedd4d443d8a4bff13aa43239943c40b5dc136c56d3dda708d`

## 14. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/ValidateM8W01EvidenceFrontier.cs`
- Summary: bytes=13512
- Before SHA-256: `e04d1be0e22a61697a93af535e9486425122c19d36294c2a0a5ef26a0c84f3e1`
- After SHA-256: `d8a37ac75644c6cb8658b370eae9625dca35bad59b6c30884185736a77ffc1cb`

## 15. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/discovery/qualification/m8-w01-acceptance-evidence-reconciliation.md`
- Summary: bytes=27362
- Before SHA-256: `85cebdd08d89ee1abc19461f0b01e9ee9cba113ec2b48da03bf1af4203b6c82a`
- After SHA-256: `a8628dc1bb1529064385e15b46a1e39c340bea8c4d0c2fc5439d3839718d92e2`

## 16. `files.replace-from-staged`

- Status: `Changed`
- Path: `docs/planning/decisions/m8-w01-c01-t2-green-independent-admission-rs296.md`
- Summary: bytes=1858
- After SHA-256: `38a45314398a8d9c3dce64a62f751eea9c4af2bc4bc8ca19a63174a30d3123e2`

## 17. `project-plan.activate-ready-continuation`

- Status: `Changed`
- Path: `.aura/workflow/plan/project.json`
- Summary: leaf=M8-W01-C01-T2-G activated=M8-W01-C01-T2-G
- Before SHA-256: `c40773c01ad875bb936661be7d23c75f2ddfbf4fb3664a943f2a39ec02291e60`
- After SHA-256: `ba07e43bb33d8193ad135af2781946b16404b0885c6834f9524ff257271f81b1`

