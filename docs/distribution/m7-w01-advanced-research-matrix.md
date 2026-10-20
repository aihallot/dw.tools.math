# M7-W01 — advanced research disposition and release boundaries

**Qualified state (after RS117):** M7-W01 research portfolio done; **five completed research phases**, **ten T1/T2 task receipts**, **541 preserved native project nodes**. This is a research conclusion, not a new public Math engine or package.

## Family-level evidence

| Accepted scope | BCL-only research witness | Binding product disposition |
|---|---|---|
| M7-W01-C01 — precision / intervals | At `1e16`, exact cancellation = 1 versus binary64 0; finite `1/10` enclosure witness | **Defer precision and interval provider/API**; no certified general interval |
| M7-W01-C02 — tensor shape | `2×2×2` has eight cells, sum 36 and index value 6; equal count incompatible shape; 64-element/512-byte study cap | **Defer Tensor API/provider**; no broadcasting, GPU or implicit reshape |
| M7-W01-C03-S01 — DAG | Four-node/four-edge diamond root 13, pure shared once; refuse cycle, invalid reference and over-quota without partial result | **Defer DAG runtime/provider**; no AURA job engine |
| M7-W01-C03-S02 — PlotModel | Known `1/x` pole: four finite points in 2 disconnected segments, including omitted-zero break and 8/8/2 study cap | **Defer PlotModel API/rendering**; no generic discontinuity detector |
| M7-W01-C03 original aggregate | RS115/RS116 SHA-pinned combined references and owner-gated research closure | No separate C03 product or public runtime admitted |

The decisive T2 receipts are `M7-W01-C01-T2-qualified.json`, `M7-W01-C02-T2-qualified.json`, `M7-W01-C03-S01-T2-qualified.json`, `M7-W01-C03-S02-T2-qualified.json`, and `M7-W01-C03-T2-qualified.json`, under `docs/planning/evidence/`. The original five T1 receipts and their source hashes remain independently checked as well.

## Future review triggers (four research families)

1. **Precision:** a real consumer, per-operation domain/rounding/error semantics, independent high-precision or interval oracle and source/license/TFM/RID/source-free package evidence.
2. **Tensor:** a real consumer, exact rank/shape/stride/broadcast/alias/byte limits and negative-shape/overflow tests, source/license/TFM/RID/source-free proof.
3. **Pure DAG:** an immutable pure consumer and graph operation, checked node/edge/depth, cancellation and no-partial-result statuses, independent oracles; keep AURA scheduling out of Math.
4. **PlotModel:** an exact function/domain and pole/segment policy with sample/point/segment/total byte/cancellation limits and source-free consumer; chart rendering stays with the actual client.

**No provider is automatically admitted** by these triggers. Each family's **three detailed owner-gated requirements** in its T2 decision must be met and separately approved before implementation. No broad extrapolation from one research fixture.

## Completed versus pending

- **Completed as research:** M7-W01-C01, C02, C03, C03-S01 and C03-S02, all subordinate accepted T1/T2 Q/O/D nodes, original C03 evidence reconciliation and the M7-W01 work-package checkpoint.
- **Pending:** M7-W02-C01 formal interoperability/proof ownership and M7-W02-C02 coverage/program review, as well as M7's own milestone/release gate.
- **Never inferred:** new `dw.tools.math.advanced` or formal NuGet library, `dw.discovery` callable API, public publishing, provider rights/RID, certified numerical error/physical uncertainty, AURA executor or graphical renderer.

**Budget semantics:** the binary64 and 1/x witnesses illustrate bounded cases only. Tensor 512 study bytes do not prove allocator usage; 128 binary64 coordinate bytes for eight plot points do not prove total heap or latency. No provider/consumer rights were silently approved.

**Independent gate:** `docs/planning/ValidateM7AdvancedWorkPackage.cs` cross-checks exact receipts, SHA pins, DWF state and these negative claims. `M7-W02` must be activated by its own successor run after W01 completion. No changes to sibling repositories or CI.
