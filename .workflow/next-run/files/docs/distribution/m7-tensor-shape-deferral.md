# M7 C02 — bounded tensor provider/shape decision and re-entry gates

**Disposition: DEFER** a new Math tensor product/provider. No new Tensor API, third-party runtime, `dw.tools.math.advanced` package, sparse engine, broadcasting, graph runtime, native/GPU support, public NuGet, or AURA integration is qualified by this research spike.

## Retained independent shape evidence

| Property | Historical T1 witness | Boundary |
|---|---|---|
| Dense rank-three reference | `[2,2,2]`, eight scalar values `1..8` | Sum `36` and `[1,0,1] = 6`, explicit row-major indexing |
| Shape mismatch | `[2,2,2]` versus `[2,4]` | Same eight cells, **not** a compatible shape or implicit reshape/broadcast |
| Element cap | `[4,4,4]` = 64; `[4,4,5]` = 80 | 64-element research cap; over-cap requests refuse before allocation |
| Byte cap | 64 × 8-byte integers = 512 bytes | 512-byte research cap; no hidden copies |
| Invalid/oversized shape | zero extent, empty rank, huge products | Typed-policy refusal before multiplication overflow or allocation |

The independent BCL-only `docs/planning/ValidateM7TensorAdmission.cs` is retained and rerun for T2. These witnesses are not a product tensor algorithm, numerical precision theorem, license attestation or package-consumer certification.

## Provider options and admission status

| Candidate | Decision now | Missing evidence |
|---|---|---|
| Existing Math arrays and matrices | **Retain** | No demonstrated gap for an extra rank-3 API |
| Small managed dense tensor | **Defer** | Named operation, shape/stride/element budget, API semantics, independent source-free consumer |
| Sparse, broadcasting or graph tensor | **Defer** | Sparse threshold, implicit copying/alias policy, cycle and resource limits |
| Third-party managed tensor / ML package | **Defer** | Exact source/version SHA, license and transitive notices, .NET target/RID, isolated tests |
| Native/GPU tensor provider | **Defer** | Binary rights, architecture, FFI, loader/driver, target RID, rollback and consumer proof |

No external candidate is accepted merely because it exists or advertises tensor functionality. No API or library dependency is added by this document.

## Three review triggers — all required for reconsideration

1. **Consumer/operation evidence:** a named consuming Math use case, finite rank/shape domain and an independent example not supported by current exact or numerical contracts.
2. **Bounded shape and resources:** explicit dimensions, strides, storage and alias policy, checked product before allocation, refusal/cancellation statuses, memory/time cap, no implicit reshape/broadcast/copy.
3. **Provider and adoption evidence:** source/revision/license/RID/rights and notices pin, reproducible offline package verification, exact oracles and a restored **source-free consumer**, maintainer/rollback budget and owner approval.

**If any review trigger is missing, remain deferred.** No future implementation or public package is authorized automatically by completing the spike.

## Nonclaims and next dependency

M7-W01-C02 being done is a decision outcome, not a Tensor API. No copied AURA scheduling engine, product/DAG runtime, chart rendering, additional OS/RID support, GPU, certified numerical error or public release is implied. The research evidence belongs to Math only.

`M7-W01-C03` is the next roadmap research unit; it is size-L and must be split before readiness. Its DAG and PlotModel boundary cannot be satisfied by silently promoting this deferred tensor scope.
