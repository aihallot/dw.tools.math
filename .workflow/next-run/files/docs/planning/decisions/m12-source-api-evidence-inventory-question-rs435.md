# M12 — Source, API and evidence inventory Question activation (RS435)

Status: bounded scope refinement and activation, not completed Question, not new build/API/benchmark qualification.

## Provenance and readiness

- RS431 delivered the exact five ready native nodes; RS434 succeeded first-pass, result a449b6e55fcbd3863e73bd2e68e6210692e48aa6, and recorded the separate M11 publication blocker. The distribution request remains unsent unless the owner relays it; no package mutation or new downstream claim.
- Canonical version 0.1.198 reconciles only those five ready statuses from RS431, refines C01 files/commands/criteria, regenerates derived planning pages. Native transitions ready -> in-progress on M12, M12-W01, M12-W01-C01, M12-W01-C01-T1, M12-W01-C01-T1-A. No completions or achievements. B/C and 314 other new native nodes remain not-ready. Subsequent canonical activation reconciliation must derive from the actual RS435 receipt.
- Preserve [readiness scope](m12-source-api-evidence-inventory-readiness-rs431.md), [original audit](../math-2.0/audit-original.fr.md) and [hierarchical Discovery specification](../math-2.0/discovery-generation.md).

## Reuse inventory (remote observation, not local execution)

At the RS434 source pin a non-truncated Git tree observation counted 88 production C# files, 151 test C# files, 49 tracked consumer csproj files and 123 tracked csproj files outside .workflow/.aura. No path matching BenchmarkDotNet/benchmark/coverlet/stryker/mutationtest naming was observed; this is a filename search, not proof no performance or coverage functionality exists. Reconcile these observations against tracked local inputs in Question/Observation.

| Existing mechanism | Actual inspected contract | Reuse decision / limit |
| --- | --- | --- |
| VerifyM10ProductionCutover.cs | --target verifies exact SDK11 migration, maintained inputs and historical project identities | Existing baseline check; do not repeat prior build or rollback. It is tied to the M10 snapshot, not a general future inventory tool. |
| InspectM10ProjectDispositions.cs | --write / --check renders the RS402 classification from RS400 inputs | Read historical classification and reconcile new tracked evidence. Do not regenerate historical artifact against net11. |
| InspectM8Net11PackageGraph.cs | --check inspects 13 declared packable projects and 16 direct edges | Reuse current source graph check; historical output is not binary or package proof. |
| InspectM8W02TestCoverage.cs | --check observes 13 libraries, 11 test projects, solution and declared ProjectReference relationships | Reuse structural test mapping, not line/branch coverage or test quality claim. |
| InspectM8Net11NumericApiSignals.cs | --check counts lexical numerical patterns in source files | Reuse triage hints only; not semantic API detection, performance evidence or compatibility. |
| InspectM8Net11SourceBaseline.cs | --check / --check-sdk11 still asserts historical net10/C#14 source policy | Historical evidence, not suitable as live net11 check. Do not run or rewrite it merely to make it green. |
| tools/dw.tools.math.discovery-bindings/Program.cs | --check / --write selects four exact public static methods and fingerprints known CLR signatures | Reuse signature/identity concepts; not a complete API inventory or safe general metadata loader. Do not alter selected capabilities. |
| tools/dw.tools.math.nuget-preflight/Program.cs | --inspect repository root, --self-test; current net10/net11 source graph and feed checks | Reuse evaluated project metadata extraction concepts, not a new feed acquisition/publication operation. Existing command is coupled to private-feed evidence; avoid requiring shared feed for source/API inventory. |
| docs/discovery/qualification/GenerateM8MathSkill.cs | --check / --write from selected manifest | Reuse in M13; no generated skill changes in M12 inventory. |

## Question stage investigation criteria

Use pinned Git tree, actual solution, project XML, evaluated MSBuild properties/items, locks and durable M10 reports as complementary authorities. First inspect existing metadata/PEReader readers and package identity witnesses; if a missing capability warrants a C# inventory helper, keep it bounded to source and API inventory, separate from admission/generation, with no arbitrary assembly execution. Determine exact existing command and output contract before implementation. The proposed observation uses one locked maintained solution Release build only if exact assemblies cannot be proven from retained artifacts; no full suite, NuGet package rebuild/publication or new dependency. Record the source pin, SDK/TFM, project graph, actual assembly identity and SHA-256, declared public type/member inventory and unsupported cases (generic, overload, inherited, properties/events/constructors). A metadata signature key must not be confused with a stable agent capability ID.

Question completion requires a precise bounded observation recipe: tracked paths and exclusion rules, identity/version pins, exact command and report schema, independent missing/duplicate/drift/unsupported negative controls, output and time budget, no source or external-feed mutations. The existing scoped checks listed in the canonical chunk are candidate read-only corroboration commands, not a claim that they were newly executed. Resolve any historical-only constraint before using a command in live verification. Observation is separate, and the Decision stage selects actual reuse and M12 benchmark scope from observed results.

RS435 executes structural readiness/parity and planning checks only. No current numerical results, coverage percentage or new package identity is claimed.
