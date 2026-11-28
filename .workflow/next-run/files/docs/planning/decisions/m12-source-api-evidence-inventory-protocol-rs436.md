# M12 — Source, API and evidence inventory Question protocol (RS436)

Status: Question specification, not a completed observation, binary qualification or benchmark.

## Authority and scope

RS435 succeeded first-pass under DWF 0.1.81 (attempt 001, ten preparation-safe validations, no boundary violations), result `155d0d9aa2c3b2237c5a2b30e44a504a710cc366`. Reconcile its five native activation transitions into canonical 0.1.199. Close only Question and ready Observation; Decision remains not-ready. Canonical closure reconciliation awaits the durable RS436 report. Preserve M0–M11 and the independent M11 publication blocker.

The [machine-readable protocol](../../engineering/m12-source-api-inventory-protocol-rs436.json) pins 13 production projects, 11 maintained tests, 24 maintained projects, 88 production and 151 test C# paths, 49 historical consumer projects and 123 tracked projects outside workflow authority. These are remote path counts, not build, execution or coverage evidence. Reconcile the Git source pin, locks, actual solution and evaluated project inputs. Historical consumers are classified, not executed.

## Reuse and limits

- InspectM8Net11PackageGraph and InspectM8W02TestCoverage provide declared dependency and test-reference corroboration, not actual test coverage. Reuse them for that bounded purpose.
- InspectM10ProjectDispositions regenerates historical classification; VerifyM10ProductionCutover is a cutover-specific check. Reuse durable evidence, not a historical rebuild. InspectM8Net11SourceBaseline is historical net10 evidence. Lexical numeric signals are not semantic API analysis.
- The inspected discovery-bindings tool uses selected runtime reflection methods, not arbitrary metadata enumeration; retain its exact selected fingerprints and do not broaden them into a full API inventory. The inspected NuGet preflight evaluates project properties but couples its outer command to private-feed inspection; reuse its MSBuild query pattern without invoking feed acquisition or publication.
- No general metadata-only reader was established by the inspected mechanisms. Search absence is not proof. Before adding the proposed C# reader, inspect remaining candidates and reuse a proven equivalent if available. No new package dependency or competing Discovery generator is admitted. The proposed reader CLI is specified but not implemented or qualified in RS436.

## Observation and independent acceptance

Execute the protocol commands with SDK 11.0.100, evaluating all 24 maintained projects in Release. Reconcile declared and evaluated project/compile/package items, lock hashes and package metadata. Use evaluated TargetPath, not a guessed DLL path. If retained binaries cannot independently be tied to the complete pinned source, perform one locked restore and one non-incremental Release solution build. Preserve configured NuGet Audit; no tests, pack, publication or feed mutation.

Read the 13 assemblies via PEReader/MetadataReader or a proven metadata-only equivalent. Never load or invoke scanned code. Account for externally visible nested types, generic arity and constraints, overloads, constructors, operators, properties, events, fields and accessors; distinguish declared/inherited members and record forwarded types. Record unsupported or malformed shapes; do not silently discard them or guess semantic resolution. Deterministic raw signature keys are scoped to assembly evidence, not stable Discovery capability IDs. Record SHA-256, MVID, package-vs-source evidence and all unresolved cases.

Falsify missing project, duplicate project/member identity, source/assembly digest drift, malformed/unsupported metadata, nonpublic inclusion and collapsed overloads using bounded fixtures with independent expected outcomes and reason codes. The positive oracle requires exact source and solution roster, 13 assembly identities and all public metadata accounted for. Respect the protocol limits; on mismatch, timeout or unhandled shape report blocked/incomplete, not a complete inventory. Distinguish actual package/feed presence from project metadata.

RS436 verifies the protocol specification and exact planning delta, not the future reader or numerical evidence. Observation first qualifies the reader and independent controls, then gathers real evidence. Decision selects reuse, benchmark scope and deferrals. No production source, benchmark, package promotion, Discovery or external adoption here.
