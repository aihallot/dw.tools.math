# RS294 — Reuse the existing Math Discovery probe for strict real-CLR admission

## Decision

RS293 succeeded attempt 001, first-pass, reproducing three coherent metadata forgeries that the RS287 metadata-only guard correctly admits but the actual published CLR reflection boundary refuses. Instead of cloning its already-qualified assembly loader, private package SHA-256 checks and method-signature reader into a new tool, RS294 extends the **existing** `docs/discovery/qualification/ProbeM8SelectedClrReflectionRed.cs` with a reusable read-only `--validate <manifest-path>` mode. The historical `--check` RED test remains available.

Both modes call a **single** `AdmitRealPublishedBindings` routine. That routine accepts exactly the four source-selected Math capabilities under schema-2 `dw.discovery/1.0.0-draft.1`, verifies owner, source package versions, agent-callable-only runtime authorization, canonical fingerprints, selected source pins, and the **actual method definitions** in the pinned Composition and IR assemblies loaded with Quantities dependency from the three immutable private archives.

The new `--validate` CLI returns success only when all four declarations identify real published methods and match source pins. The retained RED `--check` executes the same shared routine against canonical positives and the three coherently re-signed forgeries, requiring each forgery to be **refused by real CLR validation** even while the RS287 metadata-only CLI admits it. The prior runtime-only authorization refusal also remains mandatory.

## Lifecycle, guarantees and limits

RS294 advances no native DWF node: `M8-W01-C01-T2-R` stays `in-progress`, its parent T2 and M8/W01/C01 remain `in-progress`, T2 GREEN and Verification remain `not-ready`, and T1 stays `done`. No contribution achievement is permitted. The new gate is a Math-owned local private-feed qualification utility; it does not execute any mathematical method, install a skill in an agent host, authorize broker calls, rewrite historical source-only binding flags, mutate published archives or imply public release.

Use two focused DWF validations: `--validate` with the canonical four-capability source manifest and `--check` covering the three new real-method refusals. A later, separate run may close RED and admit GREEN using the newly recorded RS294 evidence, not retroactively reassigning older RS279/282 successes.
