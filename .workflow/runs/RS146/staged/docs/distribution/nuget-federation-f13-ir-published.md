# RS146 — Independent physical-feed readback for Math IR

## Authority and exact private package identity

RS144 qualified the **original**, unpublished `dw.tools.math.ir [20260.20261104.1001]` NuGet candidate. RS145 succeeded first-pass (attempt 001, DWF 0.1.63, result `2306b261920a304189d8a21e31c5d0770f0f0cdc`) and rehearsed its immutable publisher on a disposable feed. RS145 explicitly did not publish to `NUGET_LOCAL`.

At **2026-11-04 20:48 Europe/Brussels**, the owner separately supplied two successful PowerShell outputs from a clean `main` checkout on MSI-02:

- `MATH-IR-PUBLISHED|id=dw.tools.math.ir|version=20260.20261104.1001|sha256=d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480|dependency=20260.20261102.1001|feed=NUGET_LOCAL|verification=pending`
- `MATH-IR-VERIFY|id=dw.tools.math.ir|version=20260.20261104.1001|binary=15|archive=d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480|dependency=20260.20261102.1001|feed=NUGET_LOCAL|PASS`

Those are **operator-provided** publication and verification outputs, not independent DWF evidence. RS146 establishes a separate, independently inspected physical-feed qualification. This run **must not** call `--publish` or replace either package.

## Two-package exact dependency closure

| Published package | Version | Archive bytes | SHA-256 archive | SHA-256 net10.0 DLL |
|---|---|---:|---|---|
| `dw.tools.math.ir` | `20260.20261104.1001` | 21,926 | `d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480` | `5c81fafb632424d6a5abd1d94ce08e5d887e53a7a09f285af825d9f68f1e18af` |
| `dw.quantities` | `20260.20261102.1001` | 14,747 | `e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d` | `c7c729bfb97c295a23c5b84760326166bc4f9a876b8d602eee68b71a1ab9f8b4` |

The IR `.nuspec` must contain exactly one dependency `dw.quantities [20260.20261102.1001]`; Quantities must contain none. The separately rebuilt RS145 rehearsal archive and DLL have **different hashes** and must never substitute for the original RS144 candidate.

## Independent read-only verification

The new Math-owned, non-packable verifier `tools/dw.tools.math.nuget-ir-feed-verify` uses .NET BCL ZIP, cryptographic hashing, and XML inspection rather than the publisher's implementation. It reads **only** the two named top-level archives in the absolute existing `NUGET_LOCAL` directory, rejects redirections and unexpected file sizes, checks archive SHA-256, `.nuspec` identifiers and exact dependency versions, and hashes the packaged net10.0 DLLs. It never enumerates unrelated mixed-feed packages.

It copies only those two verified archives into a fresh ignored `build/artifacts` snapshot, compiles the unchanged RS144 `IrBinaryConsumer.cs` in a fresh isolated project, and resolves packages **only from that snapshot** with remote sources cleared and a new private cache. The consumer declares one direct `PackageReference`: `dw.tools.math.ir [20260.20261104.1001]`. No source `ProjectReference` participates. All 15 semantic cases and actual loaded DLL SHA-256 checks must pass. Physical `NUGET_LOCAL` archives are independently rehashed after the test to reject drift. The run records actual command receipts, selected SDK, OS and architecture.

The only intended persistent mutations are a bounded verifier, this contract, a read-only validator, and the DWF qualification receipt. All 541 already completed native roadmap nodes and historical Math source versions and references remain unchanged. RS146 publishes no NuGet archive, selects no public license, changes no sibling repository and authorizes no rollback claim.

## Qualification limits

A successful RS146 proves only exact private local-feed readback on its observed platform and the existing 15-case binary oracle. It does not prove feed-wide integrity, NuGet.org publication, other OS/RID behavior, concurrent writers, crash recovery, installed downgrade/rollback, Discovery skill generation, Decision migration, or AURA admission. These remain separately owned decisions.
