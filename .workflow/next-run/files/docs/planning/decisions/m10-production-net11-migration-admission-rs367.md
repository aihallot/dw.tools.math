# M10 — Owner-approved production .NET 11 migration

Decision: the owner accepted the proposed actual production migration after durable RS366 completion (2026-11-26). This prospective authorization does not rewrite the historical M8 `productionMigrationAuthorized=false` evidence. M10 is a new milestone, not reopening M8/M9. Version 1.5.0-preview is a functional planning label, not a package release version.

The baseline remains SDK 10.0.401/latestPatch, net10.0 and C# 14. The current solution contains 13 production libraries and 11 test projects. M8 already inventoried source and package risk and demonstrated bounded Foundation dual-SDK, actual ExactRational span integration and isolated Statistics/Quantities net11 candidate qualification; M9 independently qualified existing immutable private Statistics packages on Windows and Linux/WSL. Reuse those proofs as bounded evidence, not full migration or semantic compatibility.

M10-W01 contains three dependent phases: C01 readiness/rollback, C02 actual production cutover, C03 post-cutover qualification. Plan the smallest coherent cutover with the actual project graph; qualify explicit exceptions for historical net10 reproducers, maintained tools and consumers. Preserve exact package and mathematical contracts, warning and restore policy; do not introduce unrelated modernization or publish/overwrite packages. New production candidates must use distinct identities and isolated feeds. Public/private publication and downstream adoption require separate authorization. No new native completion in RS367. After admission, activate the first eligible readiness leaf rather than starting production changes without evidence.

## Preselection correction

RS367 retry 0 was refused before allocation by DWF 0.1.78 authoring preflight (`workflow-json-traversal`), with no repository mutation or attempt. Evidence: https://github.com/aihallot/dw.tools.math/commit/2d8e22df5e880ad88ab9ab435c386b9bf4622714. The corrected preparation uses complete reviewed staged mirrors for the two authoritative plans, as in RS267, instead of runtime JSON traversal. Historical records are unchanged; the M10 admission scope and validators are retained.
