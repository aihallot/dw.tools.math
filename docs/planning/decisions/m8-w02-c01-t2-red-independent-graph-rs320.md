# RS320 — Activate independent package dependency risk RED under DWF 0.1.72

## Verified authority and deliberate guidance hold

RS319 succeeded first-pass at allocated attempt 001 using dw.tools.workflow 0.1.72. It closed the official SDK/toolchain inventory T1 and readied M8-W02-C01-T2 and its RED leaf. The Math repository still has generated DWF guidance 0.26 and no pending run after this delivery.

A separate known DWF consumer incident, BL0028/RS0 (documented in aihallot/dw.tools.workflow/docs/rs1106-guidance-rs0-divergent-delivery-intake.json), concerns non-fast-forward guidance upgrade delivery and a missing reserved RS0 terminal descriptor. The meta 0.1.72 promotion explicitly has consumer guidance upgrades unauthorized while this issue is unresolved. Do not run an implicit upgrade or hand the operator Git merge/rebase/reset/receipt edits. This RS320 is normal repository development under the current guidance only, with the exact new executor version taken from the prior delivered report.

## Bounded new RED evidence without a competing graph implementation

Reuse the existing Math-owned docs/engineering/InspectM8Net11PackageGraph.cs --check, which already validates exactly thirteen current packable source projects, sixteen direct dependencies, no cycles, and no references to sibling repositories. A fresh source-only C# harness copies the existing inspector and thirteen actual csproj declarations to an isolated directory under build/artifacts with a short Windows-safe root, then invokes the copied inspector through its public CLI.

The copied inspector must accept a pristine graph and separately reject (1) a ProjectReference outside the qualified thirteen projects, (2) a duplicate direct dependency, (3) removal of one of sixteen direct edges, and (4) an internal graph cycle with unchanged edge count. After each rejection, restore exact source bytes and require clean acceptance. Verify the original 14 Math files were never changed. No source code, package binaries, feed directories, lockfiles or production SDK settings are rewritten.

## Lifecycle and residual acceptance

Activate M8-W02-C01-T2 and M8-W02-C01-T2-R to in-progress only. T1, its RED/GREEN/Verification and Discovery W01 remain done; T2 GREEN/Verification and W02/C02 remain not-ready. Do not complete RED in this run.

This RED tests actual source graph refusals, not a .NET 11 published-package dependency closure, locked restore, CPU/RID parity, arbitrary official API applicability or migration approval. Later GREEN must examine physical pinned package identities, isolated restore and version/TFM compatibility without overwriting existing .NET 10 archives or authorizing public distribution.
