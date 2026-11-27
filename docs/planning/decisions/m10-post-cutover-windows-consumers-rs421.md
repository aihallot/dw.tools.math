# M10 independent locked Windows consumers (RS421)

RS420 succeeded first-pass and produced thirteen actual net11.0 archives with sixteen exact edges. The immutable identity ledger docs/engineering/m10-post-cutover-candidate-identities-rs420.json records the delivered archive, assembly and producer lock digests and repository-relative feed location from the durable report. No rebuilding is allowed.

RS421 qualifies two source-free consumers in a new isolated directory, with exact package references, package-only transitive closure, generated then locked restore, restored and loaded DLL hashes and runtime RID. The numeric consumer reuses M9/M11 fixtures; the broad consumer reuses exact-math-smoke and bounded Distribution, Analysis, Scientific and Foundation goldens. Their union loads all thirteen assemblies. The Windows invocation must observe win-x64. The same recipe admits an explicit Linux invocation for a later controlled run, but RS421 does not claim Linux evidence.

Only the durable local report establishes passing gates. No source or archive mutation, publication, reservation or full suite repetition is authorized.
