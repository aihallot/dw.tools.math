# RS291 — Activate independent Math Discovery protocol Verification

## Native scope

RS290 succeeded attempt 001, first-pass, with one GREEN completion achievement and the Verification sibling admitted to `ready`. RS291 activates only `M8-W01-C01-T1-V` through the documented target-reentrant `ProjectPlan.ActivateReadyContinuation` operation. Its ancestors M8, W01, C01 and T1 stay `in-progress`; RED and GREEN remain `done`; all other M8/M9 descendants stay `not-ready`. No node is completed in RS291, so `contribution.achievements=[]`.

## Independent positive boundary

The Math-owned `ProbeM8VerificationPortableConsumer.cs` runs `AttestM8PortableDiscoveryExport.cs --export` as a public CLI invocation, outside the wrapper's own `--check` control branch, into a fresh temporary folder. It copies fourteen files (thirteen attested payload files plus one unsigned receipt) to a separate consumer folder and independently verifies each receipt SHA-256. The underlying canonical Decision draft protocol pins 13 cases, 2 accepted and 11 refused, without re-executing prior RS131/209 history. It checks the four selected Math capability IDs, CLR canonical signatures, fingerprints and runtime-only authorizations against the source-qualified manifest and calls the existing RS287 strict gate on the **copied** manifest.

A separate verification host executes the **copied** `verify-export.cs` in `--verify` mode against the copied consumer directory, without using a Math repository-relative path. Exact SDK `11.0.100` remains pinned by the exported `global.json`. This proves portable structural and checksum verification, not a fresh NuGet publication or compiled Math method execution (which RS279/282 have separately qualified).

## Independent negative boundary

Mutate only the copied consumer manifest: (1) change the CLR member without changing canonical signature/fingerprint; (2) grant execution authorization in metadata. In both cases, the existing strict gate must refuse with the exact RS287 reason, and the standalone verifier must refuse the now-stale checksum. Restore clean bytes and requalify offline verification after each refusal. Existing Math metadata, the exported source directory and all published package archives remain untouched.

## Excluded claims and next step

The receipt is **unsigned**, so checksums cannot authenticate provenance against an adversary able to replace both the payload and the receipt. No installed agent host, AURA integration, BrokerReady promotion, public package, runtime grant or release completion follows from these tests. The canonical product backlog `0.1.124` remains planned. If RS291 succeeds, the owning agent may separately propose Verification completion on the newly recorded evidence.
