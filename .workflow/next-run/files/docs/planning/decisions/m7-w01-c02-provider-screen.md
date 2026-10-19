# M7 tensors and advanced structures — provider and ownership screening

**State:** M7-W01-C02-T1 research only. No new consumer requirement, tensor library, C# product API, NuGet package, third-party provider, AURA dependency, GPU support or published release is qualified here.

## Independent shape and resource oracle

| Criterion | Exact research reference | Interpretation |
|---|---|---|
| Dense shape | rank 3, `[2,2,2]`, 8 elements | Explicit row-major offset, zero-based indices |
| Data and scalar oracle | `[1,2,3,4,5,6,7,8]`, sum `36`, value `[1,0,1] = 6` | Finite integer reference only, not a general tensor evaluator |
| Shape refusal | `[2,2,2]` versus `[2,4]` | Same element count does **not** authorize implicit broadcasting or reshape |
| Count limit | `[4,4,4]` = 64 accepted, `[4,4,5]` = 80 refused | Research cap: **64 elements** |
| Byte limit | 8-byte integer × 64 = **512 bytes** | Cap applies before allocation; no hidden copies |
| Invalid shape | zero dimension, rank 0 and huge dimensions | Deterministic refusal before indexing, overflow or allocation |

The local read-only BCL checker is `docs/planning/ValidateM7TensorAdmission.cs`. It is an **independent oracle**, not a shipped tensor implementation. It executes only after RS108 is admitted by DWF.

## Candidate approach and ownership matrix

| Option | Potential utility | Missing qualification and cost | T1 disposition |
|---|---|---|---|
| Existing arrays and Math matrix contracts | Finite typed scalar and matrix calculations; no extra runtime | Evaluate real rank-3 consumer gap, explicit shape/stride semantics and scope | **Retain** existing engines, use BCL research oracles |
| Small managed dense rank-3 helper | Explicit shape, row-major layout, bounded indexing | New API/maintenance cost; operations, ownership and source-free consumers not demonstrated | **Unadmitted** until T2 |
| Sparse / broadcasting / tensor graph | Large sparse domains or composable mathematical operations | Cost/benefit, density thresholds, zero/alias semantics, allocation and cycle prevention | **Defer**; no implicit conversion or copying |
| Third-party managed tensor or ML provider | Potential batched/higher-rank compute | Exact version/revision SHA, license, transitive notices, net10.0/TFM, independent oracles and rollback | **Unqualified**, no selection |
| Native/GPU tensor runtime | Possible accelerated throughput | Runtime/RID native assets, loader/FFI, driver and memory lifetime, security, licenses and source-free consumer verification | **Unqualified**, outside T1 |

Do not copy AURA's job engine, scheduling, UI visualization or permissions into Math. A consumer may depend on a qualified Math interface, never the other way round. A proposed `PlotModel` or DAG belongs to the independent later M7-W01-C03 research and requires its own admission.

## Next qualification gate

T2 must either **defer with trigger** or admit a named limited operation with independent proof: (1) a concrete consumer and operation missing from the existing Math family; (2) shape, rank, stride, element type, finite input and output memory budget, overflow/cancellation/error statuses and no implicit broadcast; (3) source/rights/version/RID identity and a source-free consumer regression for any new provider; (4) cost and rollback, with owner approval before introduction.

No completed spike grants an automatic tensor API, publicly published package, GPU feature, certified numerical guarantee or external adoption.
