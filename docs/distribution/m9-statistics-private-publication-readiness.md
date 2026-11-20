# MATH-XR-004 / M9 — Private Statistics package publication readiness

Status: **prepublication assessment only**. No private-feed mutation or owner decision is implied.

## Verified qualification lineage

- RS268: source implementation of `SplitMix64Random.NextBoundedUInt64(ulong, int)` and `NextInt64Inclusive(long, long, int)`, rejection-based uniform mapping, explicit budget and replay.
- RS269: repeatable isolated Statistics candidate `dw.tools.math.statistics 20260.20261120.1001`, `net10.0`, exact dependency `dw.quantities [20260.20261102.1001]`, not published.
- RS270: independent Windows `net11.0` / C#15 `PackageReference` consumer; two-package lock topology, repeatable locked restore and twelve checks for loaded binary, bounds, forced rejection, budget and replay. Durable reports, not a preserved artifact in Git, are the authority.

The older `dw.tools.math.statistics 20260.20261106.1001` package is immutable and does not contain this API. The original `dw.quantities 20260.20261102.1001` must remain unchanged.

## Publication boundary

RS271 is limited to read-only source/rights/report/feed/candidate inspection. The candidate is regenerated only in ignored `build/artifacts/` by the previously qualified packaging recipe; this report does not establish that it has been copied to the shared private feed.

Before any publication, the owner must separately authorize **private** distribution of this new Statistics version after reviewing the exact candidate SHA-256 and DLL SHA-256, provenance, rights, dependencies, target environment, collision safeguards and readback procedure. Proprietary redistribution rights in `docs/planning/decisions/m0-w02-c01-source-rights.md` are not a public-license authorization.

After any separately authorized publication: check exact physical `NUGET_LOCAL` archive bytes and loaded DLL identity, ensure no overwrite of old versions, then provide Assessa with the real package ID/version, dependency closure, hashes and reproduction/report references. Only the Assessa owning agent may integrate `GENERATE-001 / EG-A-01 / T002` and qualify recipes; Math must not implement Study Buddy domain constructs.

No claims of cryptographic PRNG security, universal numerical behavior, Linux/RID support, installed rollback or public publication are made by this gate.
