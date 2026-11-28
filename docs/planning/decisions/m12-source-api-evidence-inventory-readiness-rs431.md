# M12 source, API and evidence inventory — readiness (RS431)

Status: scope refinement and prospective readiness only. No inventory execution, new benchmark, new build, API admission or qualification is claimed by this decision.

## Authority and scope

- Admission result: RS430, attempt 001, first-pass success, DWF 0.1.81, commit 3805e8b69416081a9030371917f9586f167b09b6.
- Canonical backlog 0.1.197; M0–M11 remain closed; M12–M20 admitted, not implemented.
- First eligible path: M12 / M12-W01 / M12-W01-C01 / M12-W01-C01-T1 / M12-W01-C01-T1-A (Question). Ready only this five-node chain; leave Observation, Decision and successors not-ready. No completion achievements.
- Preserve the [original audit](../math-2.0/audit-original.fr.md) and [Discovery specification](../math-2.0/discovery-generation.md). RS429 remains a historical evidence frontier, not an assertion about a future runtime.
- Reconcile the canonical readiness from the delivered RS431 transition evidence before proceeding, using the existing renderer. Do not edit derived version pages manually.

## Refined investigation

The inventory phase has three bounded questions/observation/decision stages. The question stage must identify the reusable inventory and qualification mechanisms and define the exact observation recipe, not implement a second discovery engine. The observation stage must record actual repository and compiled binary evidence; the decision stage selects reusable responsibilities and the scope of the performance laboratory.

| Surface | Existing anchor / authority | Expected observation and falsification |
|---|---|---|
| Maintained solution graph | dw.tools.math.slnx, global.json, Directory.Build.props, Directory.Packages.props, NuGet.config | Reconcile the actual 24 solution entries (13 source libraries, 11 test projects), assembly/package identities, direct reference graph, lockfiles, target and toolchain. Refuse missing, duplicate or unexpected entries rather than blindly using historical counts. |
| Historical/maintained tools and consumers | docs/engineering/m10-cutover-readiness-verification-rs409.json; docs/engineering/VerifyM10ProductionCutover.cs | Reuse the qualified scope classification. Distinguish 2 maintained tools from historical project fixtures (97 in RS409), consumer project subset, and current source libraries. Recheck identities against Git-tracked files; do not conflate 49 historical consumer-project count from audit with all historical project files. |
| Public API and assemblies | src/projects; tools/dw.tools.math.discovery-bindings; tools/dw.tools.math.nuget-preflight | Inspect existing CLR binding and NuGet inspectors before authoring any inventory helper. Prefer metadata inspection with no arbitrary assembly code execution. Observe declared public types/members, overloads, signatures, assembly versions and project/package relationships from exact local build outputs; correlate to source and package locks. No publication, installation or inferred physical availability of private archives. |
| Discovery | docs/discovery/qualification/GenerateM8MathSkill.cs and its pinned manifest and strict validator; .agents/skills/dw-tools-math/SKILL.md | Separate existing four qualified IDs and actual binary binding proof from API inventory; list ownership and reuse options for M13. Do not change the generated skill, protocol or exposure. |
| Evidence and quality | RS419–RS429 durable reports; docs/planning/ValidatePlan.cs; docs/planning/ValidateNativeDwfAdoption.cs; docs/planning/ValidateMath20Admission.cs | Identify current vs historical validators and existing focused checks, report the last observed tests, not a new run. Record existing performance/coverage infrastructure and gaps; no full test suite or unmeasured optimization claims. |
| Documentation | README.md and planning/distribution/engineering guides | Identify stale operational net10 references separately from immutable history. Inventory only; remediation belongs to M12-W01-C03. |

## Planned observation contract

Read the exact admitted source snapshot and pin all relevant source/config/evidence identities. Use tracked paths and the actual evaluated project graph, not unrestricted filesystem scans. Use existing maintained inspector entry points if supported; qualify their actual command syntax in Question before choosing any new C# helper. A later observation recipe may use a locked Release build of the maintained solution (only if actual assemblies are needed) followed by metadata-only inspection and a machine-readable evidence report; output under the repository's established build/ convention or a bounded ignored workspace. No new source packages, SDK updates, broad benchmark campaign or full suite. Reject wrong SDK/TFM, missing locks or assemblies, unsupported/unclassified paths, unexpected reference edges, mismatched package/assembly identities and invented validation claims. Explicitly record unsupported surfaces and the absence of current physical package evidence where applicable. Keep source and package identity distinct.

The Question deliverable will specify exact paths, commands, time budget, reusable components, negative fixtures and report schema before execution; Observation and Decision require their own evidence. The only commands admitted in this readiness run are observational planning verification and Git diff check. The existing C# admission validator gains an explicit readiness mode checking the exact five-state delta, retaining the no-argument historical admission check. Existing parity and renderer remain unchanged.

## Successor

After durable RS431, activate the Question leaf with a bounded, evidence-backed investigation and canonical readiness reconciliation as required by project policy. Do not begin benchmarking or implement the M13 generator ahead of that scope.
